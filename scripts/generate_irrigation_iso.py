#!/usr/bin/env python3
"""Draw the irrigation and farmland overlays at map resolution.

Both were still coming from the compatibility sheet: a 64x32 cell of pixel art
that the tile compositor point-scales up to whatever the map is composed at.
Against photographic terrain that reads as a chunky pale-blue lattice thrown
over the square rather than as worked ground, which is what "irrigation still
looks stupid" was about.

They are drawn here instead, in the tile's own diamond space, as ploughed
furrows: channels running along the square with wet earth in the bottom and a
turned lip either side. Irrigation is one set of furrows; farmland is the same
field cross-ploughed, which is how Civ II distinguishes them.

Output
------
RaylibUI/FOSSart/Terrain/Overlays/Improvements/irrigation.png and farmland.png,
at tile aspect (2:1).

Usage
-----
    python3 scripts/generate_irrigation_iso.py [--check]
"""

from __future__ import annotations

import argparse
import sys
from pathlib import Path

import numpy as np
from PIL import Image, ImageDraw, ImageFilter

import isotile
from isotile import CANVAS, CORNER_E, CORNER_N, CORNER_S, CORNER_W

REPOSITORY = Path(__file__).resolve().parents[1]
OUT = REPOSITORY / "RaylibUI" / "FOSSart" / "Terrain" / "Overlays" / "Improvements"

# The worked ground stops short of the tile's edge, so a field reads as one
# square's worth of farming rather than running into its neighbours.
INSET = 0.88

# Number of furrows across the field, and their width in canvas pixels.
FURROWS = 9
FURROW_WIDTH = 7.0

# How far a channel wanders off a straight line, as a share of its length.
IRRIGATION_ROUGHNESS = 0.032
FARMLAND_ROUGHNESS = 0.011

# Wet channel, turned earth either side, and the standing water in the bottom of
# the furrow. The water is a bright blue on purpose: it is the whole point of an
# irrigated square, and against green terrain a muted teal simply read as another
# shade of the field.
CHANNEL = (66, 82, 70)
LIP = (128, 122, 88)
WATER = (80, 178, 232)


def field_mask(inset: float) -> Image.Image:
    """The diamond the furrows are confined to, softened at its rim."""
    width, height = CANVAS
    mask = Image.new("L", CANVAS, 0)
    draw = ImageDraw.Draw(mask)
    centre = (width / 2.0, height / 2.0)

    def pull(point: tuple[float, float]) -> tuple[float, float]:
        return (centre[0] + (point[0] - centre[0]) * inset,
                centre[1] + (point[1] - centre[1]) * inset)

    draw.polygon([pull(CORNER_N), pull(CORNER_E), pull(CORNER_S), pull(CORNER_W)], fill=255)
    return mask.filter(ImageFilter.GaussianBlur(3))


def _wobbled(start: np.ndarray, end: np.ndarray, roughness: float,
             rng: np.random.Generator, steps: int = 56) -> list[tuple[float, float]]:
    """A furrow as a hand-cut channel rather than a ruled line.

    The lateral drift is a pair of sines under a half-sine envelope, so it is
    zero at both ends and the furrow still spans the field it was laid out for.
    """
    t = np.linspace(0.0, 1.0, steps)
    line = start + (end - start) * t[:, None]

    direction = end - start
    length = float(np.hypot(*direction))
    if length < 1e-6 or roughness <= 0.0:
        return [tuple(point) for point in line]

    normal = np.array([-direction[1], direction[0]]) / length
    first, second = rng.uniform(0.0, 2.0 * np.pi, 2)
    drift = (np.sin(np.pi * t)
             * (0.66 * np.sin(2.0 * np.pi * 1.7 * t + first)
                + 0.34 * np.sin(2.0 * np.pi * 3.3 * t + second))
             * roughness * length)
    return [tuple(point) for point in line + normal * drift[:, None]]


def furrows(sets: list[tuple[tuple[float, float], tuple[float, float]]],
            roughness: float, seed: int) -> Image.Image:
    """Draw one or more sets of parallel furrows as a single water network.

    Each set is ``(along, across)``: the direction a furrow runs, and the
    direction its neighbours are spaced out in. Both are given as diamond edges,
    so the furrows lie in the tile's own plane and share the isometric angles of
    everything else drawn on it.

    ``roughness`` is how far a channel wanders off its line, as a share of its
    length. Ditches cut by hand do not run true, and drawn dead straight they
    read as a printed hatch pattern rather than as worked ground.

    The three passes -- turned earth, wet channel, standing water -- are each
    drawn for *every* set before the next pass begins. Drawing one whole set and
    then the other would put the second set's banks across the first set's water
    at every crossing, so a cross-ploughed field read as one ribbon lying over
    another rather than as channels that meet. Painting by pass lets the water
    run through the junctions.
    """
    width, height = CANVAS
    centre = np.array([width / 2.0, height / 2.0])
    rng = np.random.default_rng(seed)

    lines = []
    for along, across in sets:
        along_vector = np.array(along, dtype=float)
        across_vector = np.array(across, dtype=float)
        for index in range(FURROWS):
            # Spread the furrows across the field, with the spacing wandering a
            # little so the field is not a ruled grid.
            offset = (index + 0.5) / FURROWS - 0.5
            offset += rng.uniform(-0.34, 0.34) * roughness * 12.0 / FURROWS
            base = centre + across_vector * offset
            lines.append(_wobbled(base - along_vector * 0.62,
                                  base + along_vector * 0.62, roughness, rng))

    field = Image.new("RGBA", CANVAS, (0, 0, 0, 0))
    passes = [
        ((*LIP, 190), int(FURROW_WIDTH * 1.9)),
        ((*CHANNEL, 225), int(FURROW_WIDTH)),
        ((*WATER, 235), max(1, int(FURROW_WIDTH * 0.62))),
    ]
    for colour, thickness in passes:
        layer = Image.new("RGBA", CANVAS, (0, 0, 0, 0))
        draw = ImageDraw.Draw(layer)
        for points in lines:
            draw.line(points, fill=colour, width=thickness, joint="curve")
        field = Image.alpha_composite(field, layer)

    return field.filter(ImageFilter.GaussianBlur(0.7))


def build(cross_ploughed: bool) -> Image.Image:
    # A furrow runs parallel to the tile's NE edge; the sets are spaced out along
    # the NW edge. Cross-ploughing adds the second set at right angles to it.
    north_east = tuple(np.array(CORNER_E) - np.array(CORNER_N))
    north_west = tuple(np.array(CORNER_N) - np.array(CORNER_W))

    sets = [(north_east, north_west)]
    if cross_ploughed:
        sets.append((north_west, north_east))

    # Irrigation is ditches dug by hand; farmland is the same field worked after
    # Refrigeration, so its channels run much closer to true.
    field = furrows(sets, FARMLAND_ROUGHNESS if cross_ploughed else IRRIGATION_ROUGHNESS,
                    seed=4130 if cross_ploughed else 811)

    mask = field_mask(INSET)
    cut = Image.new("RGBA", CANVAS, (0, 0, 0, 0))
    cut.paste(field, (0, 0), mask)
    return cut


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("--check", action="store_true")
    args = parser.parse_args()

    targets = {"irrigation.png": False, "farmland.png": True}

    if args.check:
        missing = [name for name in targets if not (OUT / name).exists()]
        if missing:
            print(f"missing improvement overlays: {', '.join(missing)}", file=sys.stderr)
            return 1
        print(f"  improvements: {len(targets)} present")
        return 0

    OUT.mkdir(parents=True, exist_ok=True)
    for name, cross_ploughed in targets.items():
        image = build(cross_ploughed)
        image.save(OUT / name, optimize=True)
        opaque = np.asarray(image)[..., 3] > 8
        print(f"  {name}  {image.size[0]}x{image.size[1]}  {100 * opaque.mean():.0f}% covered")

    return 0


if __name__ == "__main__":
    raise SystemExit(main())
