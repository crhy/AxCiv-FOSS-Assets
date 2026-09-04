#!/usr/bin/env python3
"""Shapes for the food, shield and trade icons in the standalone ICONS sheet.

The city window reads these three icons out of `ICONS.png` at the coordinates
Civilization II uses, and draws them for the food and trade bars, the food
storage box and the production shield box. They live here rather than inline in
the sheet generator so that anything which has to write them - the generator
itself, or a repair run in an environment without Pillow - draws the same pixels.

A mask is a list of rows of single characters: ``.`` transparent, ``o`` outline,
``f`` fill, ``h`` highlight.
"""

from __future__ import annotations


# Fill, outline and highlight for each icon, plus the "loss" variant the city
# window shows for hunger and for shields lost to waste.
PALETTES = {
    "food": ((232, 188, 68), (108, 74, 16), (255, 232, 150)),
    "shields": ((126, 142, 172), (32, 42, 64), (198, 210, 232)),
    "trade": ((236, 170, 58), (104, 62, 8), (255, 226, 152)),
}

LOSS_PALETTES = {
    "food": ((176, 66, 54), (74, 22, 18), (226, 138, 128)),
    "shields": ((150, 74, 68), (60, 24, 20), (206, 140, 132)),
    "trade": ((176, 88, 48), (78, 34, 12), (224, 158, 112)),
}


def _blank(size: int) -> list[list[str]]:
    return [["." for _ in range(size)] for _ in range(size)]


def _outline(grid: list[list[str]]) -> list[list[str]]:
    """Ring any filled pixel that touches empty space with an outline pixel."""
    size = len(grid)
    out = [row[:] for row in grid]
    for y in range(size):
        for x in range(size):
            if grid[y][x] != "f":
                continue
            for dy, dx in ((-1, 0), (1, 0), (0, -1), (0, 1)):
                ny, nx = y + dy, x + dx
                if 0 <= ny < size and 0 <= nx < size and grid[ny][nx] == ".":
                    out[ny][nx] = "o"
                elif not (0 <= ny < size and 0 <= nx < size):
                    out[y][x] = "o"
    return out


def food_mask(size: int) -> list[list[str]]:
    """A grain: a rounded body on a short stalk."""
    grid = _blank(size)
    cx = (size - 1) / 2
    body_bottom = size * 0.72
    ry = body_bottom / 2
    cy = ry
    rx = size * 0.30
    for y in range(size):
        for x in range(size):
            if ((x - cx) / rx) ** 2 + ((y - cy) / ry) ** 2 <= 1.0:
                grid[y][x] = "f"
    for y in range(int(body_bottom), size - 1):
        grid[y][int(round(cx))] = "f"
    grid = _outline(grid)
    for y in range(1, max(2, int(ry))):
        grid[y][int(round(cx - rx * 0.35))] = "h"
    return grid


def shield_mask(size: int) -> list[list[str]]:
    """A shield: square shoulders tapering to a point."""
    grid = _blank(size)
    margin = max(1, size // 8)
    shoulder = size * 0.45
    for y in range(margin, size - margin):
        if y <= shoulder:
            half = (size - 2 * margin) / 2
        else:
            t = (y - shoulder) / max(1.0, (size - margin) - shoulder)
            half = (size - 2 * margin) / 2 * (1.0 - t * t)
        cx = (size - 1) / 2
        for x in range(size):
            if abs(x - cx) <= half:
                grid[y][x] = "f"
    grid = _outline(grid)
    for y in range(margin + 1, int(shoulder)):
        grid[y][margin + 1] = "h"
    return grid


def trade_mask(size: int) -> list[list[str]]:
    """A trade arrow: a diamond, the shape Civ II uses for commerce."""
    grid = _blank(size)
    cx = cy = (size - 1) / 2
    radius = (size - 1) / 2 - 0.5
    for y in range(size):
        for x in range(size):
            if abs(x - cx) + abs(y - cy) <= radius:
                grid[y][x] = "f"
    grid = _outline(grid)
    grid[max(1, int(cy) - 1)][max(1, int(cx) - 1)] = "h"
    return grid


MASKS = {"food": food_mask, "shields": shield_mask, "trade": trade_mask}

# Where the interface reads each icon out of the sheet: (x, y, size) per name,
# matching Civ2Interface.ResourceImages.
LARGE_SLOTS = {"food": (1, 305, 14), "shields": (16, 305, 14), "trade": (31, 305, 14)}
LOSS_SLOTS = {"food": (1, 290, 14), "shields": (16, 290, 14), "trade": (31, 290, 14)}
SMALL_SLOTS = {"food": (49, 334, 10), "shields": (60, 334, 10), "trade": (71, 334, 10)}


def draw_icon(put_pixel, name: str, x0: int, y0: int, size: int, loss: bool = False) -> None:
    """Plot one icon through `put_pixel(x, y, (r, g, b, a))`."""
    fill, outline, highlight = (LOSS_PALETTES if loss else PALETTES)[name]
    colours = {"f": fill + (255,), "o": outline + (255,), "h": highlight + (255,)}
    grid = MASKS[name](size)
    for y, row in enumerate(grid):
        for x, cell in enumerate(row):
            colour = colours.get(cell)
            put_pixel(x0 + x, y0 + y, colour if colour else (0, 0, 0, 0))


def draw_all(put_pixel) -> None:
    """Plot every resource icon the city window needs into the sheet."""
    for slots, loss in ((LARGE_SLOTS, False), (LOSS_SLOTS, True), (SMALL_SLOTS, False)):
        for name, (x, y, size) in slots.items():
            draw_icon(put_pixel, name, x, y, size, loss=loss)
