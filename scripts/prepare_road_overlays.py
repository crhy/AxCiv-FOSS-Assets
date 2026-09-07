#!/usr/bin/env python3
"""Paint the road and railroad connection spokes from the generated source art.

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

This script used to cut those halves out of the painted sources by measuring
where their ink lay and keeping the part running in the wanted direction. That
does not work, and is why roads appeared as scattered dashes rather than a
network: the sources are free-hand, so their ink begins and ends wherever the
brush did, and a half cut from them started somewhere near the tile centre and
stopped somewhere near the edge. Neither end landed on the point that the
neighbouring tile's spoke would meet.

Now the geometry is constructed exactly -- centre to boundary point, every time
-- and the painted material is swept along it, so a spoke cannot be misplaced.
Roads sweep a median cross-section of their source, because gravel track has no
structure along its length to lose. Railways resample the source picture along
the spoke instead, because their sleepers and rails do.

Input
-----
~/rhYcivtextures/roads and ~/rhYcivtextures/railroads, each holding painted
straight-through pieces on a magenta generation matte. For every spoke the
source whose painted axis best matches that spoke's direction is used, so the
baked lighting and isometric perspective stay consistent with the direction
drawn. Nothing is ever rotated: rotating art with baked lighting is what makes
tile overlays look wrong.

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

import isotile
from isotile import CANVAS, CENTRE, EIGHT, ENDPOINTS, StraightSource

REPOSITORY = Path(__file__).resolve().parents[1]
OVERLAYS = REPOSITORY / "RaylibUI" / "FOSSart" / "Terrain" / "Overlays"
DEFAULT_SOURCE = Path.home() / "rhYcivtextures"

# Painted width of each surface, in canvas pixels (the tile is 512 wide here).
# A Civ II road is a few pixels on a 64-wide tile; these are the same share.
ROAD_WIDTH = 26.0
RAIL_WIDTH = 34.0

# The longest spoke, centre to the E or W corner. Railway sleepers are pitched
# against this so the short diagonal spokes do not show squashed track.
REFERENCE_LENGTH = math.hypot(ENDPOINTS["e"][0] - CENTRE[0], ENDPOINTS["e"][1] - CENTRE[1])

# Radius of the isolated stub, as a fraction of the tile width.
ISOLATED_RADIUS = 0.15


def source_axis(path: Path) -> tuple[float, float]:
    """The painted segment's own direction, as a unit vector."""
    _, alpha = isotile.key_matte(path)
    ys, xs = np.nonzero(alpha > 0.25)
    points = np.stack([xs, ys], axis=1).astype(np.float64)
    centred = points - points.mean(axis=0)
    _, _, vectors = np.linalg.svd(centred, full_matrices=False)
    return float(vectors[0][0]), float(vectors[0][1])


def straightness(path: Path) -> float:
    """How linear a painted segment is: across-spread over along-spread."""
    _, alpha = isotile.key_matte(path)
    ys, xs = np.nonzero(alpha > 0.25)
    points = np.stack([xs, ys], axis=1).astype(np.float64)
    centred = points - points.mean(axis=0)
    values = np.linalg.svd(centred, compute_uv=False)
    return float(values[1] / max(values[0], 1e-6))


def best_source(sources: list[Path], axes: dict[Path, tuple[float, float]],
                direction: str) -> Path:
    """The source painted most nearly along a given spoke's direction.

    The tile is 2:1, so a spoke's screen direction is not its map direction; the
    match is made in screen space, which is the space the art was painted in.
    """
    end = ENDPOINTS[direction]
    dx, dy = end[0] - CENTRE[0], end[1] - CENTRE[1]
    length = math.hypot(dx, dy)
    wanted = (dx / length, dy / length)

    def alignment(path: Path) -> float:
        axis = axes[path]
        return abs(wanted[0] * axis[0] + wanted[1] * axis[1])

    return max(sources, key=alignment)


def isolated_stub(profile, seed: int) -> tuple[np.ndarray, np.ndarray]:
    """The short mark drawn on a tile whose road reaches no neighbour.

    Civ II draws a stub rather than nothing, so a newly built road is visible
    before it is connected to anything.
    """
    radius = ISOLATED_RADIUS * CANVAS[0]
    start = (CENTRE[0] - radius / 2.0, CENTRE[1] - radius / 4.0)
    end = (CENTRE[0] + radius / 2.0, CENTRE[1] + radius / 4.0)
    return isotile.sweep(profile, start, end, ROAD_WIDTH, seed, wobble=0.08)


def build(source_directory: Path, out_directory: Path, stem: str,
          use_warp: bool, width: float, check: bool) -> int:
    sources = sorted(p for p in source_directory.glob("*.png"))
    if not sources:
        print(f"no source art in {source_directory}", file=sys.stderr)
        return 1

    axes = {path: source_axis(path) for path in sources}

    # A junction piece is not a segment: its ink spreads in several directions at
    # once, so it has no usable axis and would blur a swept profile. Keep the
    # pieces that read as a single stroke.
    segments = [path for path in sources if straightness(path) < 0.30]
    if not segments:
        segments = sources

    out_directory.mkdir(parents=True, exist_ok=True)
    written = []

    profile = None
    if not use_warp:
        # One material for every spoke: the straightest segment available.
        material = min(segments, key=straightness)
        profile = isotile.cross_section(material)

    for index, direction in enumerate(EIGHT):
        start, end = isotile.spoke_path(direction)
        if use_warp:
            chosen = best_source(segments, axes, direction)
            rgb, alpha = isotile.warp(StraightSource(chosen), start, end, width,
                                      seed=1000 + index,
                                      reference_length=REFERENCE_LENGTH)
        else:
            rgb, alpha = isotile.sweep(profile, start, end, width, seed=1000 + index)

        rgb, alpha = isotile.fill_holes(rgb, alpha)
        image = isotile.to_image(rgb, alpha)
        target = out_directory / f"{stem}_{direction}.png"
        if not check:
            image.save(target, optimize=True)
        written.append(target)

    stub_profile = profile if profile is not None else isotile.cross_section(
        min(segments, key=straightness))
    rgb, alpha = isolated_stub(stub_profile, seed=999)
    rgb, alpha = isotile.fill_holes(rgb, alpha)
    target = out_directory / f"{stem}_iso.png"
    if not check:
        isotile.to_image(rgb, alpha).save(target, optimize=True)
    written.append(target)

    missing = [path for path in written if not path.exists()]
    if check:
        if missing:
            print(f"missing {stem} overlays: {', '.join(p.name for p in missing)}",
                  file=sys.stderr)
            return 1
        print(f"  {stem}: {len(written)} sprites present")
        return 0

    print(f"  {stem}: wrote {len(written)} sprites to {out_directory}")
    return 0


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("--source", type=Path, default=DEFAULT_SOURCE)
    parser.add_argument("--check", action="store_true",
                        help="verify the overlays exist without rebuilding them")
    args = parser.parse_args()

    status = 0
    status |= build(args.source / "roads", OVERLAYS / "Roads", "road",
                    use_warp=False, width=ROAD_WIDTH, check=args.check)
    status |= build(args.source / "railroads", OVERLAYS / "Railroads", "railroad",
                    use_warp=True, width=RAIL_WIDTH, check=args.check)
    return status


if __name__ == "__main__":
    raise SystemExit(main())
