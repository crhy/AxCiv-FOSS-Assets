# Texture gap list

What is still procedural, placeholder, or on a legacy square tile after the
2026-09-01 terrain-art integration pass. Paired with
[STANDALONE-ASSET-AUDIT.md](STANDALONE-ASSET-AUDIT.md), which tracks the atlases
as a whole; this file is the shot list for painted replacements.

Provenance rules in [ASSET-PROVENANCE.md](ASSET-PROVENANCE.md) apply to every
item here: new art is project-original or generated from project-original
inputs, never traced or sampled from a commercial installation.

## Done: road and railroad connections

Painted spokes now replace the generator-drawn rosettes. The renderer composites
one sprite per connected neighbour, so the art is eight half-spokes running from
the tile centre to the point where each neighbour is reached -- the midpoint of
the shared edge for a neighbour that shares one, the diamond corner for a
neighbour that only touches at a corner -- plus an isolated stub for a tile with
no connections. `scripts/prepare_road_overlays.py` cuts them from the
straight-through pieces in `~/rhYcivtextures/roads` and `~/rhYcivtextures/railroads`
into `FOSSart/Terrain/Overlays/{Roads,Railroads}`, and `TerrainLoader.ApplyFossConnectionArt`
composes them at the working tile size, so they keep their detail at high zoom
instead of being routed through the 64x32 sheet cell.

The procedural fallback in `build_standalone_sheets.draw_connections` was fixed
at the same time: it had been drawing the full four-way rosette into all nine
sprite slots, so a tile with a single neighbour showed roads running off every
side. It now draws one spoke per slot, in the renderer's neighbour order.

## Done in this pass

Painted isometric base diamonds now feed `Standalone/TERRAIN1.png` for **every
row**: desert, plains, grassland, hills, mountains, tundra, glacier, swamp,
jungle, and ocean — each with a 2nd interchangeable variant except desert and
mountains. Painted special-resource cutouts now render for **desert 1/2 (oasis,
oil), plains 1/2 (bison, grain), grassland 1/2 (pheasant, sheep), hills 2
(wine), tundra 1/2 (game, furs), glacier 2 (oil), swamp 1 (sugarcane), jungle
1/2 (bananas, cacao), ocean 1/2 (salmon, whale)**. Source art lives in
`~/rhYcivtextures/terrain/`; the pipeline is
`scripts/prepare_custom_textures.py` → `scripts/build_standalone_sheets.py`.

## Missing — terrain base tiles

| Terrain | State | Note |
|---|---|---|
| river (as terrain tint) | legacy `Terrain/river.jpg` | overlays exist, base does not |
| desert / mountains 2nd variant | none | single diamond, so those terrains visibly repeat |
| forest | n/a in this renderer's TERRAIN1 rows | drawn as base + `Overlays/Forest`; a painted forest-floor diamond exists in `terrain/alts/` if the row is ever added |
| coast blending | generator-drawn strokes in `build_terrain2()` | painted ocean base exists now, but the shoreline transitions are still vector |

## Missing — special-resource paintings

Still the procedural coloured marker in `build_terrain1()`:

| Terrain / slot | Civ II resource | Have art? |
|---|---|---|
| grassland row-2 pair | (shield) | reuses grassland 1/2 |
| hills 1 | coal | no |
| mountains 1 / 2 | gold / iron | no |
| glacier 1 | ivory | no tusked-ivory art (seal + crab keyed in `terrain/alts/`) |
| swamp 2 | peat / oil | no (swamp 1 uses sugarcane) |

Unused source art already keyed and available in `~/rhYcivtextures/terrain/alts/`:
oasis alt, grassland lawn/conifer variants, oil-slick alt, yak, seal, crab.

## Missing — overlays and markers (generator-drawn vector, not painted)

- `TERRAIN1`: irrigation, farmland, mine, pollution, grassland shield and goody
  hut. The road and railroad connection sprites are now painted -- see below.
- `TERRAIN2`: coast, river-mouth, and ocean-edge transition strokes.
- `Standalone/ICONS.png`: every map-UI glyph (view toggles, zoom controls,
  progress rings, resource swatches, category chips) is drawn procedurally in
  `build_standalone_sheets.build_icons()`.

## Missing — UI and screen art

- Menu and panel backgrounds are procedural gradients
  (`build_backgrounds()`); painted source art exists but is not wired:
  `~/rhYcivtextures/NewCartographerBackground.png` (panel/parchment),
  `GoldenSunsetWin.png` / `GoldenSunsetWinText.png` (conquest victory screen).
- Civilopedia concept and category illustrations remain concise or procedural.
- No painted frames for battle resolution, production progress, or
  global-warming state changes (global warming is out of scope for rules but the
  art hooks exist).

## Orphaned reference images

`RaylibUI/FOSSart/Other/*.jpg` (19 files: buffalo, fish, whales, wine, gems,
gold, furs, silk, spice, oasis, ivory, fruit, pheasant, shield-grassland, and
several "from above" mine/well shots) are **referenced by no runtime code**.
They predate the `Terrain/Specials` pipeline. Either wire them into the
Civilopedia resource pages or remove them; do not add more art to that folder.

## City styles

Six cultures (Aztec, German, Greek, Japanese, London, USA) each ship 8 size
sprites plus the generated walls / flags / status markers. Not yet spot-checked
per culture for size-progression continuity or wall alignment at 3x zoom.
