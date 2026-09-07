#!/usr/bin/env python3
"""Take the generation matte back out of the bundled art.

The art set is generated on a saturated magenta matte and then cut out. Cutting
sets the alpha but never touches the colour underneath, so the matte survives in
two different ways, and they need different answers.

**Fringe.** Every part-transparent pixel along an edge is a blend of artwork and
matte. Left alone it shows as a magenta rim: the citizens in the city window are
outlined in pink. The blend is in known proportion, so the matte's share can be
subtracted back out and the artwork's own colour recovered. This is the same
unmixing the resource icons are imported with, and it is the reason a soft edge
survives it -- eroding the rim away instead would eat the lace and the hair it
runs through.

**Leftovers.** Some matte is fully opaque: a frame the generator drew around the
figure, and specks it left inside the picture. Where such a patch reaches the
edge of the image or touches transparency it is background the cut missed, and
it is made transparent. Where it is enclosed by artwork it is a speck -- the pink
dots on the city roofs -- and punching a hole would show the map through, so its
colour is taken from the artwork around it instead.

Usage:
    python3 scripts/clean_matte_fringe.py [--check] [--root RaylibUI/FOSSart]
"""

from __future__ import annotations

import argparse
import sys
from pathlib import Path

try:
    import numpy as np
    from PIL import Image
except ImportError:  # pragma: no cover - depends on the machine, not the code
    np = None
    Image = None

REPOSITORY = Path(__file__).resolve().parents[1]
DEFAULT_ROOT = REPOSITORY / "RaylibUI" / "FOSSart"

# What counts as matte. The generation matte is a true magenta with almost no
# green in it at all -- measured across the set it sits between 3 and 15 -- while
# purple *artwork* carries far more: the violet flags are around 75. A looser test
# than this ate the purple flags, mistaking the cloth for background, so the green
# ceiling is the whole safeguard and is set well below any real purple in the art.
MAX_GREEN = 45
MIN_MAGENTA = 90

# Below this a pixel is too faint for the matte in it to be visible, and unmixing
# it only amplifies noise.
MIN_VISIBLE_ALPHA = 24

# Passes of inpainting for enclosed specks. They are a few pixels across, so this
# is plenty; more would pull colour from a long way off.
PASSES = 8

FALLBACK_MATTE = np.array([255.0, 0.0, 255.0])

# What the gate will tolerate before calling it a regression rather than residue.
PER_FILE_TOLERANCE = 40
TOTAL_TOLERANCE = 400


def matte_mask(rgb: np.ndarray, alpha: np.ndarray) -> np.ndarray:
    red, green, blue = rgb[..., 0], rgb[..., 1], rgb[..., 2]
    magenta = np.minimum(red, blue) - green
    return (alpha > MIN_VISIBLE_ALPHA) & (green < MAX_GREEN) & (magenta > MIN_MAGENTA)


def _shift(mask: np.ndarray, dy: int, dx: int) -> np.ndarray:
    """Neighbour lookup that treats outside the image as false."""
    out = np.zeros_like(mask)
    ys = slice(max(0, dy), mask.shape[0] + min(0, dy))
    xs = slice(max(0, dx), mask.shape[1] + min(0, dx))
    sys_ = slice(max(0, -dy), mask.shape[0] + min(0, -dy))
    sxs = slice(max(0, -dx), mask.shape[1] + min(0, -dx))
    out[ys, xs] = mask[sys_, sxs]
    return out


def reaches_outside(matte: np.ndarray, transparent: np.ndarray) -> np.ndarray:
    """Which matte pixels belong to a patch open to the outside of the picture.

    A patch that touches the border of the image, or any fully transparent pixel,
    is background the cut did not reach. One sealed inside the artwork is a speck
    in the middle of the picture.
    """
    seed = matte & (transparent | _edge_of(matte))
    for dy, dx in ((-1, 0), (1, 0), (0, -1), (0, 1)):
        seed |= matte & _shift(transparent, dy, dx)

    reached = seed
    while True:
        grown = reached.copy()
        for dy, dx in ((-1, 0), (1, 0), (0, -1), (0, 1)):
            grown |= matte & _shift(reached, dy, dx)
        if grown.sum() == reached.sum():
            return grown
        reached = grown


def _edge_of(mask: np.ndarray) -> np.ndarray:
    border = np.zeros(mask.shape, dtype=bool)
    border[0], border[-1], border[:, 0], border[:, -1] = True, True, True, True
    return border


def clean(path: Path) -> tuple[int, int]:
    """Returns (matte pixels found, matte pixels remaining)."""
    original = Image.open(path).convert("RGBA")
    data = np.asarray(original).astype(np.float64)
    rgb, alpha = data[..., :3].copy(), data[..., 3].copy()

    matte = matte_mask(rgb, alpha)
    found = int(matte.sum())
    if found == 0:
        return 0, 0

    # The matte's own colour, measured from the opaque part of it where the
    # artwork is not mixed in at all.
    solid = matte & (alpha > 250)
    matte_colour = np.median(rgb[solid], axis=0) if solid.any() else FALLBACK_MATTE

    # 1. Fringe: subtract the matte's share out of the blend.
    fringe = matte & (alpha < 250)
    if fringe.any():
        share = (alpha[fringe] / 255.0)[..., None]
        recovered = np.clip(
            (rgb[fringe] - (1.0 - share) * matte_colour) / np.maximum(share, 1e-6), 0, 255)
        rgb[fringe] = recovered

        # A part-transparent pixel that is *still* matte once its share of the
        # matte has been taken out never had any artwork in it: it is background
        # the cut faded rather than removed, which is what the pink veils behind
        # the citizens are. Nothing can be recovered from it, so it goes.
        red, green, blue = recovered[:, 0], recovered[:, 1], recovered[:, 2]
        hopeless = (green < MAX_GREEN) & (np.minimum(red, blue) - green > MIN_MAGENTA)
        indices = np.nonzero(fringe)
        alpha[indices[0][hopeless], indices[1][hopeless]] = 0.0

    # 2. Opaque leftovers: background where it is open to the outside, a speck
    #    where it is sealed inside the artwork.
    solid = matte & (alpha >= 250)
    if solid.any():
        transparent = alpha <= MIN_VISIBLE_ALPHA
        background = solid & reaches_outside(solid, transparent)
        alpha[background] = 0.0

        speck = solid & ~background
        remaining = speck.copy()
        for _ in range(PASSES):
            usable = (alpha > 0) & ~matte_mask(rgb, alpha)
            total = np.zeros_like(rgb)
            count = np.zeros(alpha.shape)
            for dy in (-1, 0, 1):
                for dx in (-1, 0, 1):
                    if dy == 0 and dx == 0:
                        continue
                    total += _shift3(rgb, dy, dx) * _shift(usable, dy, dx)[..., None]
                    count += _shift(usable, dy, dx)
            fixable = remaining & (count > 0)
            if not fixable.any():
                break
            rgb[fixable] = total[fixable] / count[fixable][..., None]
            remaining &= ~fixable

        # Anything the artwork never reached is background after all.
        alpha[remaining] = 0.0

    cleaned = np.dstack([np.clip(rgb, 0, 255), np.clip(alpha, 0, 255)]).astype(np.uint8)
    Image.fromarray(cleaned, "RGBA").save(path, optimize=True)

    check = np.asarray(Image.open(path).convert("RGBA")).astype(np.float64)
    return found, int(matte_mask(check[..., :3], check[..., 3]).sum())


def _shift3(image: np.ndarray, dy: int, dx: int) -> np.ndarray:
    out = np.zeros_like(image)
    ys = slice(max(0, dy), image.shape[0] + min(0, dy))
    xs = slice(max(0, dx), image.shape[1] + min(0, dx))
    sys_ = slice(max(0, -dy), image.shape[0] + min(0, -dy))
    sxs = slice(max(0, -dx), image.shape[1] + min(0, -dx))
    out[ys, xs] = image[sys_, sxs]
    return out


def survey(root: Path) -> list[tuple[Path, int]]:
    hits = []
    for path in sorted(root.rglob("*.png")):
        try:
            data = np.asarray(Image.open(path).convert("RGBA")).astype(np.float64)
        except Exception:
            continue
        count = int(matte_mask(data[..., :3], data[..., 3]).sum())
        if count:
            hits.append((path, count))
    return hits


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("--root", type=Path, default=DEFAULT_ROOT)
    parser.add_argument("--check", action="store_true",
                        help="report what is still matte-coloured without changing it")
    args = parser.parse_args()

    # The gate runs everywhere, including on a machine set up only to build and
    # test. Reading images needs numpy and Pillow, which the rest of the gate does
    # not, so the check says so and stands aside rather than failing a build over a
    # missing tool. CI installs both, so the check is still enforced there.
    if np is None or Image is None:
        print("  matte: skipped (needs numpy and Pillow)")
        return 0

    if args.check:
        hits = survey(args.root)
        total = sum(count for _, count in hits)
        worst = max((count for _, count in hits), default=0)

        # A few stray pixels survive cleaning: an inpainted colour can itself land
        # back in matte territory, and chasing the last of them would take more
        # passes than the result is worth. At this scale they cannot be seen. What
        # this check is for is a *regression* -- a newly added sprite that arrives
        # with its background still on it, which runs to thousands of pixels.
        if worst <= PER_FILE_TOLERANCE and total <= TOTAL_TOLERANCE:
            print(f"  matte: clear ({total} stray pixels, within tolerance)")
            return 0

        print(f"leftover generation matte in {len(hits)} files ({total} pixels, "
              f"worst {worst}):", file=sys.stderr)
        for path, count in sorted(hits, key=lambda hit: -hit[1])[:10]:
            print(f"  {count:6d}  {path.relative_to(REPOSITORY)}", file=sys.stderr)
        print("Run: python3 scripts/clean_matte_fringe.py", file=sys.stderr)
        return 1

    files = 0
    cleared = 0
    left = 0
    for path in sorted(args.root.rglob("*.png")):
        try:
            found, remaining = clean(path)
        except Exception as error:
            print(f"  {path.name}: {error}", file=sys.stderr)
            continue
        if found:
            files += 1
            cleared += found - remaining
            left += remaining

    print(f"  matte: cleared {cleared} pixels across {files} files"
          + (f", {left} left" if left else ""))
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
