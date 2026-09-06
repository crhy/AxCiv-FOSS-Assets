#!/usr/bin/env python3
"""Cut painted road and railroad connection spokes from the generated source art.

Background
----------
The map renderer does not draw a road as one picture per tile. For each of a
tile's eight neighbours that also carries the improvement, it composites one
sprite -- `ImprovementGraphic.Levels[level, neighbour + 1]` -- on top of the
tile, and falls back to `Levels[level, 0]` when the tile stands alone
(RaylibUI/RunGame/GameControls/Mapping/MapImage.cs). So the art this script has
to produce is eight *half-spokes*, each running from the tile centre out to the
point where that neighbour is reached, plus one isolated stub. Two adjacent
tiles each draw their own half, and the halves meet on the shared boundary.

Where each spoke ends, on the 64x32 diamond with corners N(32,0) E(63,16)
S(32,31) W(0,16):

    sprite  neighbour  map offset   ends at
    1       NE         ( 0,-1)      midpoint of the N-E edge
    2       E          ( 1, 0)      the E corner
    3       SE         ( 0, 1)      midpoint of the E-S edge
    4       S          ( 0, 2)      the S corner
    5       SW         (-1, 1)      midpoint of the S-W edge
    6       W          (-1, 0)      the W corner
    7       NW         (-1,-1)      midpoint of the W-N edge
    8       N          ( 0,-2)      the N corner

A neighbour that shares an edge with this tile is reached at that edge's
midpoint; one that only touches a corner is reached at the corner. That is why
the four "diagonal" spokes are shorter than the four axial ones.

Input
-----
The generated art lives outside the repository, in ~/rhYcivtextures/roads and
~/rhYcivtextures/railroads. Each folder holds straight-through pieces painted on
a magenta generation matte: one image per axis (E-W, N-S, NE-SW, NW-SE), plus
junctions this script does not need. The pieces are already drawn in 2:1
isometric screen space, so their painted angles are used as-is and never
rotated -- rotating a piece with baked lighting and perspective is what makes
tile art look wrong.

For each direction the script picks, out of the candidate sources, the piece
whose ink is most strongly aligned with that axis; scales it so the whole
through-piece spans exactly the distance between its two endpoints on the
diamond; centres it; and keeps the half running in the wanted direction, with a
small overlap past the centre so opposite spokes join without a seam.

Output
------
RaylibUI/FOSSart/Terrain/Overlays/Roads/road_<dir>.png and
.../Railroads/railroad_<dir>.png, at tile aspect (2:1). TerrainLoader composes
them onto the working tile, so they bypass the 64x32 legacy sheet cell entirely
and keep their detail at high zoom.

Usage
-----
    python3 scripts/prepare_road_overlays.py [--source ~/rhYcivtextures] [--check]
"""

from __future__ import annotations

import argparse
import math
import sys
from pathlib import Path

import numpy as np
from PIL import Image, ImageDraw, ImageFilter

REPOSITORY = Path(__file__).resolve().parents[1]
OVERLAYS = REPOSITORY / "RaylibUI" / "FOSSart" / "Terrain" / "Overlays"
DEFAULT_SOURCE = Path.home() / "rhYcivtextures"

# Output canvas. The tile is 2:1, and the art is authored at that aspect so the
# isometric angles survive the compose step without being stretched.
CANVAS = (512, 256)

# The diamond, in output canvas pixels (the 64x32 tile scaled by 8).
SCALE = CANVAS[0] / 64.0
CORNER_N = (32.0 * SCALE, 0.0)
CORNER_E = (63.0 * SCALE, 16.0 * SCALE)
CORNER_S = (32.0 * SCALE, 31.0 * SCALE)
CORNER_W = (0.0, 16.0 * SCALE)
CENTRE = (32.0 * SCALE, 16.0 * SCALE)


def _midpoint(a: tuple[float, float], b: tuple[float, float]) -> tuple[float, float]:
    return ((a[0] + b[0]) / 2.0, (a[1] + b[1]) / 2.0)


# Sprite index -> (name, endpoint on the diamond). Order matches the neighbour
# order in Engine/src/MapObjects/MapNavigationFunctions.Neighbours.
ENDPOINTS: dict[str, tuple[float, float]] = {
    "ne": _midpoint(CORNER_N, CORNER_E),
    "e": CORNER_E,
    "se": _midpoint(CORNER_E, CORNER_S),
    "s": CORNER_S,
    "sw": _midpoint(CORNER_S, CORNER_W),
    "w": CORNER_W,
    "nw": _midpoint(CORNER_W, CORNER_N),
    "n": CORNER_N,
}

SPRITE_ORDER = ["ne", "e", "se", "s", "sw", "w", "nw", "n"]

# Opposite directions share a straight-through source image.
AXES = {"ne": "sw", "sw": "ne", "e": "w", "w": "e",
        "se": "nw", "nw": "se", "n": "s", "s": "n"}

# How far past the tile centre a spoke is kept, as a fraction of the tile width.
# Without it two opposite spokes meet on an exact pixel boundary and show a hairline.
OVERLAP = 0.06

# Radius of the isolated stub, as a fraction of the tile width.
ISOLATED_RADIUS = 0.16


def diamond_mask() -> np.ndarray:
    """The tile's diamond, as a boolean mask over the output canvas.

    Every spoke is clipped to it. Art that spilled outside the diamond would be
    drawn into the corners of the tile's bounding rectangle, which overlap the
    neighbouring tiles on screen -- a road would appear to run across squares
    that have none.
    """
    mask = Image.new("L", CANVAS, 0)
    ImageDraw.Draw(mask).polygon(
        [CORNER_N, CORNER_E, CORNER_S, CORNER_W], fill=255)
    return np.asarray(mask) > 0


def unit_vector(direction: str) -> tuple[float, float]:
    ex, ey = ENDPOINTS[direction]
    dx, dy = ex - CENTRE[0], ey - CENTRE[1]
    length = math.hypot(dx, dy)
    return dx / length, dy / length


def through_length(direction: str) -> float:
    """Distance between a direction's endpoint and its opposite."""
    a, b = ENDPOINTS[direction], ENDPOINTS[AXES[direction]]
    return math.hypot(a[0] - b[0], a[1] - b[1])


def key_matte(path: Path) -> tuple[np.ndarray, np.ndarray]:
    """Return RGB and alpha with the magenta generation matte removed.

    The matte test is the same one the rest of the art pipeline uses
    (`prepare_custom_textures.is_matte_background`), applied to every pixel
    rather than only to the edge-connected region -- road and rail art contains
    no legitimate magenta, and the gaps between railway sleepers are interior
    matte pockets that an edge flood fill reaches only by luck.

    The painted edges are airbrushed, so a band of pixels around each piece is a
    blend of paint and matte. Keying those on colour alone leaves a pink halo
    that is glaring against grassland, so the foreground is eroded past the
    contaminated band and then feathered back to a soft edge.
    """
    rgb = np.asarray(Image.open(path).convert("RGB")).astype(np.int16)
    r, g, b = rgb[..., 0], rgb[..., 1], rgb[..., 2]
    lowest = np.minimum(r, b)
    magenta = (r > 90) & (b > 105) & (g < lowest * 0.70)
    green = (g > 60) & (g > r * 1.18) & (g > b * 1.10)

    solid = Image.fromarray(np.where(magenta | green, 0, 255).astype(np.uint8), "L")
    # MinFilter is an erosion: a 7px window pulls the edge in by 3px, which is
    # wider than the blend band the generator leaves.
    eroded = solid.filter(ImageFilter.MinFilter(7))
    alpha = np.asarray(eroded.filter(ImageFilter.GaussianBlur(1.2))).astype(np.float64) / 255.0

    colour = rgb.astype(np.float64)
    return colour, alpha


def axis_alignment(alpha: np.ndarray, direction: str) -> float:
    """Fraction of a source's ink that lies along `direction`'s axis."""
    ys, xs = np.nonzero(alpha > 0.35)
    if len(xs) < 64:
        return 0.0
    cx, cy = xs.mean(), ys.mean()
    radius = np.hypot(xs - cx, ys - cy)
    outer = radius > 0.18 * alpha.shape[1]
    if outer.sum() < 32:
        return 0.0

    ux, uy = unit_vector(direction)
    target = math.degrees(math.atan2(uy, ux))
    angle = np.degrees(np.arctan2(ys[outer] - cy, xs[outer] - cx))
    delta = np.abs((angle - target + 180.0) % 360.0 - 180.0)
    # count both ends of the axis: a through-piece serves either direction
    opposite = np.abs((angle - (target + 180.0) + 180.0) % 360.0 - 180.0)
    return float(((delta < 16.0) | (opposite < 16.0)).sum()) / float(outer.sum())


def build_spoke(colour: np.ndarray, alpha: np.ndarray, direction: str) -> Image.Image:
    """Place a through-piece on the tile and keep the half running `direction`."""
    ux, uy = unit_vector(direction)
    px, py = -uy, ux

    ys, xs = np.nonzero(alpha > 0.35)
    proj = xs * ux + ys * uy
    perp = xs * px + ys * py

    # Scale so the painted piece spans exactly endpoint-to-endpoint on the tile.
    span = proj.max() - proj.min()
    scale = through_length(direction) / span
    # Centre on the middle of the piece's axis, and on the bulk of its width.
    axis_mid = (proj.max() + proj.min()) / 2.0
    perp_mid = float(np.median(perp))
    source_centre = (axis_mid * ux + perp_mid * px, axis_mid * uy + perp_mid * py)

    height, width = alpha.shape
    art = np.dstack([colour, alpha[..., None] * 255.0]).astype(np.uint8)
    scaled = Image.fromarray(art, "RGBA").resize(
        (max(1, round(width * scale)), max(1, round(height * scale))), Image.LANCZOS)

    canvas = Image.new("RGBA", CANVAS, (0, 0, 0, 0))
    canvas.paste(scaled,
                 (round(CENTRE[0] - source_centre[0] * scale),
                  round(CENTRE[1] - source_centre[1] * scale)))

    # Keep only the half-plane running in `direction`, plus the join overlap.
    grid_y, grid_x = np.mgrid[0:CANVAS[1], 0:CANVAS[0]]
    ahead = ((grid_x - CENTRE[0]) * ux + (grid_y - CENTRE[1]) * uy) >= -(OVERLAP * CANVAS[0])

    out = np.asarray(canvas).copy()
    out[..., 3] = np.where(ahead & diamond_mask(), out[..., 3], 0)
    return Image.fromarray(out, "RGBA")


def build_isolated(colour: np.ndarray, alpha: np.ndarray, direction: str) -> Image.Image:
    """A short centre stub for a tile whose improvement has no neighbours."""
    spoke = build_spoke(colour, alpha, direction)
    grid_y, grid_x = np.mgrid[0:CANVAS[1], 0:CANVAS[0]]
    # An ellipse, so the stub reads as a patch lying on the ground rather than a
    # circle painted on a 2:1 tile.
    radius = ISOLATED_RADIUS * CANVAS[0]
    inside = (((grid_x - CENTRE[0]) / radius) ** 2
              + ((grid_y - CENTRE[1]) / (radius / 2.0)) ** 2) <= 1.0

    out = np.asarray(spoke).copy()
    out[..., 3] = np.where(inside, out[..., 3], 0)
    return Image.fromarray(out, "RGBA")


def prepare(source_dir: Path, out_dir: Path, stem: str, check: bool) -> list[str]:
    sources = sorted(source_dir.glob("*.png"))
    if not sources:
        raise SystemExit(f"Error: no source art in {source_dir}")

    keyed = {path: key_matte(path) for path in sources}
    problems: list[str] = []
    out_dir.mkdir(parents=True, exist_ok=True)

    chosen: dict[str, Path] = {}
    for direction in SPRITE_ORDER:
        best = max(sources, key=lambda p: axis_alignment(keyed[p][1], direction))
        score = axis_alignment(keyed[best][1], direction)
        if score < 0.30:
            problems.append(
                f"{stem}: no source is aligned with {direction.upper()} "
                f"(best {best.name} at {score:.2f})")
            continue
        chosen[direction] = best

    for direction, path in chosen.items():
        colour, alpha = keyed[path]
        image = build_spoke(colour, alpha, direction)
        problems += _emit(image, out_dir / f"{stem}_{direction}.png", check)

    # The isolated stub is cut from the E-W piece, whose paint is the least
    # foreshortened of the four axes.
    if "e" in chosen:
        colour, alpha = keyed[chosen["e"]]
        problems += _emit(build_isolated(colour, alpha, "e"),
                          out_dir / f"{stem}_iso.png", check)

    print(f"{stem}: {len(chosen)}/8 spokes from "
          f"{len({p.name for p in chosen.values()})} source images"
          + ("" if "e" not in chosen else " + isolated stub"))
    for direction in SPRITE_ORDER:
        if direction in chosen:
            print(f"    {direction.upper():>2} <- {chosen[direction].name}")
    return problems


def _emit(image: Image.Image, target: Path, check: bool) -> list[str]:
    buffer = target.with_suffix(".tmp.png")
    image.save(buffer, optimize=True)
    produced = buffer.read_bytes()
    buffer.unlink()
    if check:
        if not target.exists():
            return [f"missing: {target.relative_to(REPOSITORY)}"]
        if target.read_bytes() != produced:
            return [f"stale: {target.relative_to(REPOSITORY)}"]
        return []
    target.write_bytes(produced)
    return []


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("--source", type=Path, default=DEFAULT_SOURCE,
                        help="folder holding roads/ and railroads/ (default ~/rhYcivtextures)")
    parser.add_argument("--check", action="store_true",
                        help="verify the committed art matches the source, without rewriting")
    args = parser.parse_args()

    problems: list[str] = []
    for folder, stem, out in (("roads", "road", "Roads"),
                              ("railroads", "railroad", "Railroads")):
        problems += prepare(args.source / folder, OVERLAYS / out, stem, args.check)

    for problem in problems:
        print(f"ERROR: {problem}", file=sys.stderr)
    return 1 if problems else 0


if __name__ == "__main__":
    raise SystemExit(main())
