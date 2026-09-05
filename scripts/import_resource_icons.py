#!/usr/bin/env python3
"""Cut the medallion resource icons out of their black plates.

The food, gold, science, production and trade art arrives as 1254x1254 RGB
plates: a round medallion centred on a near-black square. The game needs them
with an alpha channel so they can sit on the city window's panels.

The cut is a circular mask, not a colour key. The medallions have no hard edge
against their plate - the stone rim is itself nearly black and fades into the
background - so both a luminance threshold and a flood from the border eat
straight through the rim and into the badge. What the art does have is a
reliable shape: a disc centred in the plate with four compass points reaching
exactly as far as the disc. So the medallion extent is measured, and a circle
inscribed in it keeps the badge whole and drops the corners.

Each icon is also written in a "loss" colour, which the city window uses for
hunger, corruption, waste and shortage.
"""

from __future__ import annotations

import sys
from pathlib import Path

import numpy as np
from PIL import Image

SOURCE = Path.home() / "Pictures"
OUT = Path(__file__).resolve().parent.parent / "RaylibUI" / "FOSSart" / "Icons" / "Resources"

# Big enough to stay sharp in the city window's resource rows at full scale and
# in the Civilopedia, small enough that five of them cost little to load.
SIZE = 96

ICONS = ["food", "gold", "science", "production", "trade"]

# A pixel this dark is bare plate. Used only to measure how far the medallion
# reaches, never to decide a pixel's own transparency.
PLATE = 12

# Feather on the cut edge, as a fraction of the radius: enough to avoid a hard
# jagged rim, small enough not to eat the frame.
FEATHER = 0.012


def medallion_box(rgb: np.ndarray) -> tuple[int, int, int, int]:
    """The bounds of everything that is not bare plate, as (l, t, r, b)."""
    lit = rgb.mean(axis=2) > PLATE
    rows = np.flatnonzero(lit.any(axis=1))
    cols = np.flatnonzero(lit.any(axis=0))
    if rows.size == 0 or cols.size == 0:
        return 0, 0, rgb.shape[1], rgb.shape[0]
    return int(cols[0]), int(rows[0]), int(cols[-1]) + 1, int(rows[-1]) + 1


def circular_alpha(shape: tuple[int, int], box: tuple[int, int, int, int]) -> np.ndarray:
    left, top, right, bottom = box
    cx, cy = (left + right - 1) / 2, (top + bottom - 1) / 2
    radius = min(right - left, bottom - top) / 2

    ys, xs = np.ogrid[: shape[0], : shape[1]]
    distance = np.sqrt((xs - cx) ** 2 + (ys - cy) ** 2)
    edge = max(1.0, radius * FEATHER)
    return np.clip((radius - distance) / edge + 0.5, 0, 1) * 255


def cut(name: str) -> Image.Image:
    plate = Image.open(SOURCE / f"{name}.png").convert("RGB")
    rgb = np.asarray(plate).astype(np.uint8)

    box = medallion_box(rgb)
    alpha = circular_alpha(rgb.shape[:2], box).astype(np.uint8)
    cutout = Image.fromarray(np.dstack([rgb, alpha]), "RGBA")

    # Crop to the disc, so every icon is the same square footprint and rows of
    # them line up without any dead margin.
    left, top, right, bottom = box
    cx, cy = (left + right - 1) / 2, (top + bottom - 1) / 2
    radius = min(right - left, bottom - top) / 2
    cutout = cutout.crop((round(cx - radius), round(cy - radius),
                          round(cx + radius), round(cy + radius)))

    return cutout.resize((SIZE, SIZE), Image.LANCZOS)


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


def to_lux(gold: Image.Image) -> Image.Image:
    """A stand-in luxuries medallion, until there is drawn art for it.

    Luxuries sit in the same row as taxes and science in the city window, so
    leaving it as the old 14-pixel sheet sprite would have put one coarse icon
    between two medallions. This tints the gold badge violet, which reads as
    luxury and keeps the row consistent, and should be replaced by real art.
    """
    pixels = np.asarray(gold).astype(np.float32)
    rgb, alpha = pixels[..., :3], pixels[..., 3:]
    grey = rgb @ np.array([0.299, 0.587, 0.114], dtype=np.float32)
    tinted = np.dstack([
        np.clip(grey * 0.92 + 28, 0, 255),
        np.clip(grey * 0.44, 0, 255),
        np.clip(grey * 1.02 + 34, 0, 255),
    ])
    return Image.fromarray(np.dstack([tinted, alpha]).astype(np.uint8), "RGBA")


def main() -> int:
    missing = [n for n in ICONS if not (SOURCE / f"{n}.png").exists()]
    if missing:
        print(f"missing source art: {', '.join(missing)}", file=sys.stderr)
        return 1

    OUT.mkdir(parents=True, exist_ok=True)
    for name in ICONS:
        icon = cut(name)
        icon.save(OUT / f"{name}.png")
        print(f"{OUT.name}/{name}.png {icon.size}")

    for name in ("food", "production", "trade"):
        loss = to_loss(Image.open(OUT / f"{name}.png"))
        loss.save(OUT / f"{name}_loss.png")
        print(f"{OUT.name}/{name}_loss.png {loss.size}")

    lux = to_lux(Image.open(OUT / "gold.png"))
    lux.save(OUT / "lux.png")
    print(f"{OUT.name}/lux.png {lux.size} (placeholder)")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
