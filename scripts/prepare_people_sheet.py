#!/usr/bin/env python3
"""Clean the high-resolution citizen sheet and export runtime portraits.

The generated source uses a near-magenta presentation matte.  A simple
``#ff00ff`` replacement leaves a bright fringe because antialiased pixels are
blends of the portrait and that matte.  This tool derives a soft alpha channel,
removes the matte contribution from edge colors, and exports both a cleaned
sheet and 44 consistently sized 300x300 citizen portraits.
"""

from __future__ import annotations

import argparse
from pathlib import Path

from PIL import Image


MATTE = (240, 4, 239)
ERAS = ("ancient", "renaissance", "industrial", "modern")
ROLES = (
    "happy_1",
    "happy_2",
    "content_1",
    "content_2",
    "unhappy_1",
    "unhappy_2",
    "angry_1",
    "angry_2",
    "entertainer",
    "taxman",
    "scientist",
)

# Interior rectangles of the four-by-eleven master grid.  The intentionally
# uneven one-pixel spacing reflects the source sheet's hand-drawn rules.
X_EDGES = (20, 125, 229, 332, 435, 539, 642, 746, 850, 954, 1057, 1157)
Y_EDGES = (25, 166, 312, 455, 595)


def remove_matte(image: Image.Image) -> Image.Image:
    """Return RGBA art with a clean, antialiased edge and no pink spill."""
    rgb = image.convert("RGB")
    output = Image.new("RGBA", rgb.size)
    source = rgb.load()
    target = output.load()

    for y in range(rgb.height):
        for x in range(rgb.width):
            color = source[x, y]
            distance = sum((color[channel] - MATTE[channel]) ** 2 for channel in range(3)) ** 0.5

            # The matte itself varies by roughly 25 RGB units due to generation
            # and PNG resampling.  The 28..78 transition retains fine hair and
            # fabric edges while making the broad presentation background clear.
            alpha = max(0, min(255, round((distance - 28) * 255 / 50)))
            if alpha == 0:
                target[x, y] = (0, 0, 0, 0)
                continue

            # Reverse the source-over-matte blend.  This is what prevents a pink
            # outline when the portrait is drawn over a dark city background.
            fraction = alpha / 255
            cleaned = tuple(
                max(0, min(255, round((color[channel] - (1 - fraction) * MATTE[channel]) / fraction)))
                for channel in range(3)
            )
            target[x, y] = (*cleaned, alpha)

    return output


def fit_portrait(portrait: Image.Image) -> Image.Image:
    bounds = portrait.getchannel("A").getbbox()
    if bounds is None:
        raise RuntimeError("Citizen cell contains no visible portrait")
    portrait = portrait.crop(bounds)
    scale = min(284 / portrait.width, 284 / portrait.height)
    portrait = portrait.resize(
        (max(1, round(portrait.width * scale)), max(1, round(portrait.height * scale))),
        Image.Resampling.LANCZOS,
    )
    canvas = Image.new("RGBA", (300, 300), (0, 0, 0, 0))
    canvas.alpha_composite(portrait, ((300 - portrait.width) // 2, 292 - portrait.height))
    return canvas


def main() -> None:
    parser = argparse.ArgumentParser()
    parser.add_argument("source", type=Path, help="edited 1448x1086 PeopleUpdate PNG")
    parser.add_argument("output_sheet", type=Path, help="clean RGBA master sheet")
    parser.add_argument("portraits", type=Path, help="directory for 44 300x300 portraits")
    args = parser.parse_args()

    with Image.open(args.source) as loaded:
        if loaded.size != (1448, 1086):
            parser.error(f"expected a 1448x1086 sheet, got {loaded.width}x{loaded.height}")
        cleaned = remove_matte(loaded)

    args.output_sheet.parent.mkdir(parents=True, exist_ok=True)
    cleaned.save(args.output_sheet, "PNG", optimize=True, compress_level=9)
    args.portraits.mkdir(parents=True, exist_ok=True)

    count = 0
    for row, era in enumerate(ERAS):
        for column, role in enumerate(ROLES):
            cell = cleaned.crop((X_EDGES[column], Y_EDGES[row], X_EDGES[column + 1], Y_EDGES[row + 1]))
            output = fit_portrait(cell)
            output.save(args.portraits / f"{era}_{role}.png", "PNG", optimize=True, compress_level=9)
            count += 1

    print(f"Prepared transparent citizen sheet and {count} 300x300 portraits")


if __name__ == "__main__":
    main()
