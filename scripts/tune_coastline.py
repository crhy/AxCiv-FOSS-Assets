#!/usr/bin/env python3
"""Narrow the shelf and the beach on the isometric coastline tiles.

`generate_coastline_iso.py` paints each tile from a signed distance to the
shoreline, `d`, measured in world pixels across a 300px tile, through the colour
ramp copied below. Two things in that ramp make the coast read wrongly: the water
does not reach deep ocean until `d = -150`, half a tile from the shore, so every
ocean tile touching land is bright turquoise edge to edge; and everything on the
land side of the shoreline is sand, so the beach reaches half a tile out to sea.

Real coasts seen from above, and Civilization's own, are the other way round: a
thin bright shelf hugging the shore and deep water immediately beyond, with the
beach a narrow strip rather than a wedge.

That generator needs Pillow and numpy. This script does the same retune without
them, by working backwards from the painted tiles: for each pixel it recovers the
`d` the painter used, moves it, and repaints through the same ramp. The texture -
grain, foam, swell, caustics - is carried across as each pixel's deviation from
the ramp, so only the band positions move.

It is a pure function from one directory of tiles to another, so it is not
idempotent: run it on the tiles `generate_coastline_iso.py` produced, not on its
own output.
"""

from __future__ import annotations

import argparse
import zlib
import struct
from pathlib import Path


# The ramp from generate_coastline_iso.py, unchanged: d in world pixels, negative
# to seaward. Recovering d means inverting this, so the two must stay in step.
STOPS = [
    (-420, (5, 28, 60)), (-150, (5, 28, 60)), (-130, (7, 44, 82)),
    (-108, (10, 66, 110)), (-84, (14, 94, 138)), (-64, (19, 126, 164)),
    (-48, (26, 160, 184)), (-36, (42, 192, 196)), (-26, (66, 214, 206)),
    (-18, (100, 228, 214)), (-11, (140, 236, 220)), (-5, (176, 226, 205)),
    (-1, (166, 146, 116)), (4, (146, 124, 96)), (13, (178, 154, 122)),
    (32, (210, 190, 154)), (72, (233, 219, 187)), (150, (245, 234, 208)),
    (420, (245, 234, 208)),
]

# How far the sea reaches full depth, as a fraction of the painted distance. The
# painted shelf ran to -150; at 0.38 it is spent by about -57, so a tile touching
# land shows a bright fringe and then open water.
SHELF = 0.38

# How far the shoreline moves towards the land corners, in world pixels. The
# marching-squares shoreline runs through the middle of the tile, which is what
# keeps a coast smooth rather than stair-stepped; this leaves that alone and only
# trims the sand behind it back to a strip.
BEACH_TRIM = 46.0


def ramp(d: float) -> tuple[float, float, float]:
    """The painter's colour at a signed distance."""
    if d <= STOPS[0][0]:
        return STOPS[0][1]
    for (d0, c0), (d1, c1) in zip(STOPS, STOPS[1:]):
        if d <= d1:
            t = 0.0 if d1 == d0 else (d - d0) / (d1 - d0)
            return tuple(c0[k] + (c1[k] - c0[k]) * t for k in range(3))
    return STOPS[-1][1]


def _monotone_inverse(stops, channel):
    """Distance lookup along one channel over a run of stops where it rises."""
    pairs = [(c[channel], d) for d, c in stops]
    return pairs


WATER_STOPS = [(d, c) for d, c in STOPS if d <= -5]
SAND_STOPS = [(d, c) for d, c in STOPS if d >= -1]
# Blue rises with d through the water, red rises with d through the sand, so each
# run can be inverted on that one channel.
_WATER = _monotone_inverse(WATER_STOPS, 2)
_SAND = _monotone_inverse(SAND_STOPS, 0)


def _invert(pairs, value):
    if value <= pairs[0][0]:
        return pairs[0][1]
    for (v0, d0), (v1, d1) in zip(pairs, pairs[1:]):
        if value <= v1:
            t = 0.0 if v1 == v0 else (value - v0) / (v1 - v0)
            return d0 + (d1 - d0) * t
    return pairs[-1][1]


def recover_distance(r: int, g: int, b: int) -> float:
    """The signed distance the painter most likely used for this pixel."""
    return _invert(_SAND, r) if r > b else _invert(_WATER, b)


def move(d: float) -> float:
    """Where that distance should sit now."""
    if d < 0:
        return d / SHELF
    return d - BEACH_TRIM


def read_png(path: Path):
    data = path.read_bytes()
    pos, idat = 8, b""
    while pos < len(data):
        length = struct.unpack(">I", data[pos:pos + 4])[0]
        kind = data[pos + 4:pos + 8]
        chunk = data[pos + 8:pos + 8 + length]
        if kind == b"IHDR":
            width, height, depth, colour, _, _, interlace = struct.unpack(">IIBBBBB", chunk)
        elif kind == b"IDAT":
            idat += chunk
        pos += 12 + length
    if depth != 8 or colour != 6 or interlace != 0:
        raise ValueError(f"{path}: expected 8-bit RGBA, not interlaced")

    stride = width * 4
    raw = zlib.decompress(idat)
    out = bytearray(width * height * 4)
    previous = bytearray(stride)
    i = 0
    for y in range(height):
        filter_type = raw[i]
        i += 1
        line = bytearray(raw[i:i + stride])
        i += stride
        if filter_type == 1:
            for x in range(4, stride):
                line[x] = (line[x] + line[x - 4]) & 255
        elif filter_type == 2:
            for x in range(stride):
                line[x] = (line[x] + previous[x]) & 255
        elif filter_type == 3:
            for x in range(stride):
                left = line[x - 4] if x >= 4 else 0
                line[x] = (line[x] + ((left + previous[x]) >> 1)) & 255
        elif filter_type == 4:
            for x in range(stride):
                a = line[x - 4] if x >= 4 else 0
                b = previous[x]
                c = previous[x - 4] if x >= 4 else 0
                p = a + b - c
                pa, pb, pc = abs(p - a), abs(p - b), abs(p - c)
                pr = a if (pa <= pb and pa <= pc) else (b if pb <= pc else c)
                line[x] = (line[x] + pr) & 255
        out[y * stride:(y + 1) * stride] = line
        previous = line
    return width, height, out


def write_png(path: Path, width: int, height: int, pixels: bytearray) -> None:
    def chunk(kind: bytes, payload: bytes) -> bytes:
        return (struct.pack(">I", len(payload)) + kind + payload
                + struct.pack(">I", zlib.crc32(kind + payload) & 0xFFFFFFFF))

    stride = width * 4
    body = b"".join(b"\x00" + bytes(pixels[y * stride:(y + 1) * stride]) for y in range(height))
    path.write_bytes(
        b"\x89PNG\r\n\x1a\n"
        + chunk(b"IHDR", struct.pack(">IIBBBBB", width, height, 8, 6, 0, 0, 0))
        + chunk(b"IDAT", zlib.compress(body, 9))
        + chunk(b"IEND", b""))


def retune(source: Path, destination: Path) -> None:
    width, height, pixels = read_png(source)
    for index in range(0, len(pixels), 4):
        alpha = pixels[index + 3]
        if alpha == 0:
            continue

        r, g, b = pixels[index], pixels[index + 1], pixels[index + 2]
        d = recover_distance(r, g, b)
        was_land = d > 0
        base = ramp(d)
        moved = move(d)
        target = ramp(moved)

        # Carry the painted texture across as the pixel's departure from the ramp.
        # A pixel that changes side keeps less of it, so sand grain does not end up
        # scattered over open water.
        keep = 1.0 if (moved > 0) == was_land else 0.3
        for k in range(3):
            value = target[k] + (float((r, g, b)[k]) - base[k]) * keep
            pixels[index + k] = max(0, min(255, int(round(value))))

    write_png(destination, width, height, pixels)


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("--input", required=True, type=Path,
                        help="directory of tiles from generate_coastline_iso.py")
    parser.add_argument("--output", required=True, type=Path)
    args = parser.parse_args()

    args.output.mkdir(parents=True, exist_ok=True)
    tiles = sorted(args.input.glob("coast_*.png"))
    if not tiles:
        print(f"no coast_*.png under {args.input}")
        return 1

    for tile in tiles:
        retune(tile, args.output / tile.name)
        print(f"retuned {tile.name}")
    print(f"{len(tiles)} tiles: shelf x{SHELF}, beach trimmed {BEACH_TRIM:.0f} world px")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
