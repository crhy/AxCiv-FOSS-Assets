#!/usr/bin/env python3
"""Cut the resource icons out of their matte.

These are the food, production, trade, gold, luxury and science icons the city
window draws in its resource rows, and the Civilopedia reuses.

The art used to be round medallions on a near-black plate, and was cut with a
circular mask because the stone rim faded into the plate with no edge to key
against. The current art is the bare icon on a flat rose matte, which keys
cleanly: the matte is a single colour with almost no variance, and there is a
wide gap between it and anything in the artwork. A circular mask would now cut
the corners off icons that are not round.

Luxuries used to be the gold badge tinted violet, a stand-in noted as wanting
real art. There is real art now, so the tint is gone.

Each icon is also written in a "loss" colour, which the city window uses for
hunger, corruption, waste and shortage.

Usage:
    python3 scripts/import_resource_icons.py [--source ~/rhYcivtextures/resources]
"""

from __future__ import annotations

import argparse
import sys
from pathlib import Path

import numpy as np
from PIL import Image

REPOSITORY = Path(__file__).resolve().parents[1]
OUT = REPOSITORY / "RaylibUI" / "FOSSart" / "Icons" / "Resources"
DEFAULT_SOURCE = Path.home() / "rhYcivtextures" / "resources"

# Big enough to stay sharp in the city window's resource rows at full scale and
# in the Civilopedia, small enough that a row of them costs little to load.
SIZE = 96

# Source name -> shipped name. The city window has always called luxuries "lux".
ICONS = {
    "food": "food",
    "production": "production",
    "trade": "trade",
    "gold": "gold",
    "luxury": "lux",
    "science": "science",
}

# Which icons the city window also needs in the loss colour.
LOSS_ICONS = ("food", "production", "trade")

# Distance from the matte colour at which a pixel is fully matte, and at which it
# is fully artwork. The matte is flat to within about two levels, and the nearest
# artwork sits far outside this band, so the ramp is narrow on purpose.
MATTE_NEAR = 30.0
MATTE_FAR = 80.0

# Fraction of the canvas left clear around the icon.
MARGIN = 0.04


def matte_colour(rgb: np.ndarray) -> np.ndarray:
    """The matte, taken from the border rather than assumed."""
    border = np.concatenate([
        rgb[:3].reshape(-1, 3), rgb[-3:].reshape(-1, 3),
        rgb[:, :3].reshape(-1, 3), rgb[:, -3:].reshape(-1, 3),
    ])
    values, counts = np.unique(border, axis=0, return_counts=True)
    return values[counts.argmax()].astype(np.float64)


def cut(path: Path) -> Image.Image:
    rgb = np.asarray(Image.open(path).convert("RGB")).astype(np.float64)
    matte = matte_colour(rgb)

    distance = np.linalg.norm(rgb - matte, axis=-1)
    alpha = np.clip((distance - MATTE_NEAR) / (MATTE_FAR - MATTE_NEAR), 0.0, 1.0)

    # Take the matte back out of the edge rather than eroding it away. These icons
    # are full of thin detail -- the ring's band, the sparkles, the wheat ears --
    # and an erosion wide enough to remove the pink rim eats those too. Every
    # part-transparent pixel is a blend of artwork and matte in known proportion,
    # so the matte's share can simply be subtracted.
    with np.errstate(divide="ignore", invalid="ignore"):
        unmixed = (rgb - (1.0 - alpha)[..., None] * matte) / np.maximum(alpha, 1e-6)[..., None]
    cleaned = np.where(alpha[..., None] > 0.0, np.clip(unmixed, 0, 255), 0.0)

    icon = Image.fromarray(cleaned.astype(np.uint8), "RGB")
    icon.putalpha(Image.fromarray((alpha * 255).astype(np.uint8), "L"))

    bounds = icon.getbbox()
    if bounds is None:
        raise SystemExit(f"{path.name}: nothing left after keying the matte")
    icon = icon.crop(bounds)

    inner = round(SIZE * (1 - 2 * MARGIN))
    icon.thumbnail((inner, inner), Image.LANCZOS)
    canvas = Image.new("RGBA", (SIZE, SIZE), (0, 0, 0, 0))
    canvas.alpha_composite(icon, ((SIZE - icon.width) // 2, (SIZE - icon.height) // 2))
    return canvas


def to_loss(icon: Image.Image) -> Image.Image:
    """The red-shifted variant shown for hunger, corruption, waste, shortage."""
    pixels = np.asarray(icon).astype(np.float32)
    rgb, alpha = pixels[..., :3], pixels[..., 3:]
    grey = rgb @ np.array([0.299, 0.587, 0.114], dtype=np.float32)
    tinted = np.dstack([
        np.clip(grey * 1.05 + 40, 0, 255),
        np.clip(grey * 0.42, 0, 255),
        np.clip(grey * 0.34, 0, 255),
    ])
    return Image.fromarray(np.dstack([tinted, alpha]).astype(np.uint8), "RGBA")


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("--source", type=Path, default=DEFAULT_SOURCE)
    args = parser.parse_args()

    missing = [n for n in ICONS if not (args.source / f"{n}.png").exists()]
    if missing:
        print(f"missing source art in {args.source}: {', '.join(missing)}", file=sys.stderr)
        return 1

    OUT.mkdir(parents=True, exist_ok=True)
    for source_name, shipped_name in ICONS.items():
        icon = cut(args.source / f"{source_name}.png")
        icon.save(OUT / f"{shipped_name}.png", optimize=True)
        opaque = np.asarray(icon)[..., 3] > 8
        print(f"  {shipped_name}.png  {icon.size[0]}x{icon.size[1]}  "
              f"{100 * opaque.mean():.0f}% covered")

    for name in LOSS_ICONS:
        loss = to_loss(Image.open(OUT / f"{name}.png"))
        loss.save(OUT / f"{name}_loss.png", optimize=True)
        print(f"  {name}_loss.png  {loss.size[0]}x{loss.size[1]}")

    return 0


if __name__ == "__main__":
    raise SystemExit(main())
