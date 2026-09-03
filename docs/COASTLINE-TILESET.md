# Coastline tileset — integration brief

Hand this to the Claude Code session working in the rhYciv repo, along with the
`coastline_iso/` asset folder. It specifies the art contract only; it makes no
claims about rhYciv's existing structure — read the code for that.

## What the assets are

16 RGBA PNGs, isometric 2:1 diamonds, **300×150**, transparent outside the
diamond. A **300×300** padded variant sits in `padded_300x300/` — same diamond,
centred horizontally, bottom-anchored, 150px of transparent headroom above.

Pick one set and delete the other. Use the padded set only if rhYciv's existing
tiles are 300×300 with the diamond in the lower half; otherwise use 300×150.

Each tile is opaque within the diamond — it carries both the sand and the water,
so it does not need to be composited over a water base layer.

## Sprite selection

Corner-based (marching squares). The four diamond vertices are N (top), E
(right), S (bottom), W (left). Bit weights: **N=8, E=4, S=2, W=1**; a bit is set
when that vertex is land.

```
mask = (land[y][x]     << 3)   // N
     | (land[y][x+1]   << 2)   // E
     | (land[y+1][x+1] << 1)   // S
     |  land[y+1][x]           // W
```

This reads a grid of land/water **corner points**, which is one larger in each
axis than the tile grid. Files are named `coast_<mask>_<shape>.png`, mask
zero-padded to two digits; all 16 masks 0–15 exist, so the lookup is total and
needs no fallback.

| mask | file suffix | mask | file suffix |
|---|---|---|---|
| 0 | ocean | 8 | corner_land_N |
| 1 | corner_land_W | 9 | edge_land_NW |
| 2 | corner_land_S | 10 | diagonal_N_S |
| 3 | edge_land_SW | 11 | inner_water_E |
| 4 | corner_land_E | 12 | edge_land_NE |
| 5 | diagonal_E_W | 13 | inner_water_S |
| 6 | edge_land_SE | 14 | inner_water_W |
| 7 | inner_water_N | 15 | land |

## Placement

For the 300×150 set, tile (x, y) blits at:

```
screenX = originX + (x - y) * 150
screenY = originY + (x + y) * 75
```

Halve those constants proportionally if tiles are scaled. Draw in row-major
order (y outer, x inner) so overlap resolves correctly. The diamond mask is
deliberately dilated by about half a pixel, so adjacent tiles overlap slightly —
this is what prevents an antialiased hairline grid across the map. Do not trim
or tighten the alpha.

## Things to determine by reading the repo, not by assuming

1. **Does the map model store terrain per tile or per corner?** The mask formula
   needs corner points. If rhYciv stores land/water per tile, add a derivation
   step — do not change the sprite contract to match. A tile-centre value can be
   promoted to corners (a corner is land if any adjacent tile is land, or if all
   are, depending on whether coastlines should read generous or tight); pick one,
   make it explicit, and note it in the commit.
2. **Whether an existing coast/terrain atlas is being replaced**, and whether
   anything else indexes into it by frame number.
3. **Asset path and load conventions** — match whatever the repo already does.

## Acceptance checks

- Render a small island and confirm the shoreline is continuous across tile
  boundaries, with no stair-stepping at the seams.
- Confirm no hairline grid appears between tiles at 100% zoom.
- Confirm solid-land and open-ocean tiles are the same tone as the coast tiles
  they touch (they should be; the art is built so the far field matches).
- Confirm draw order does not clip the surf where it crosses a diamond edge.

## Regenerating

`generate_coastline_iso.py` rebuilds the whole set. `STOPS` is the sand → teal →
deep blue colour ramp keyed by distance in world pixels. `AMP` controls how much
the shoreline wanders. The seed in `default_rng(...)` rerolls the coastline
shape. `W, H` and `PAD` control output dimensions. If tile dimensions change,
the ±150 distance clamp in `build()` should be revisited — it exists so that
solid tiles and coast tiles agree in colour along shared edges.
