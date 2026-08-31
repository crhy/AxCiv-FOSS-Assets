# rhYciv performance and custom-art audit

## Scope

This pass reviewed the solution's model, engine, Civ II data adapters, both Raylib
front ends, the legacy Eto front end, tests, and bundled art-loading paths.  It
also validated every image in `/home/rhy/rhYcivtextures` and used
`/home/rhy/RemasterCiv2icons` only as a visual reference.  No pixels were copied
from the remaster sheets.

The original save fields for spaceships, throne rooms, and advisors remain in the
model and serializer so classic saves and scripts still load.  Their gameplay UI
entry points are intentionally absent in rhYciv.

## Simplified gameplay

- New games default to bloodlust and do not respawn eliminated civilizations.
- Throne-room and advisor commands are not registered, so their old keyboard
  shortcuts cannot open them.
- Throne-room, advisor, and spaceship menu entries are removed from Gold, Test of
  Time, and the legacy Eto UI.
- The maximum map zoom is raised from level 8 (2x) to level 16 (3x).  All buttons,
  wheel handling, and shortcut commands share the same bounds.

## Performance work

- High-resolution unit images are no longer decoded during unit-table setup just
  to calculate their dimensions.  Map textures now load on first use.
- FOSS-art filenames are indexed once per category instead of rescanning the
  directory for every unit or icon lookup.
- The image cache now records ownership and releases decoded files, extracted
  sprites, and recolored copies when a ruleset/interface is changed.
- Temporary CPU images are released immediately after GPU upload for generated
  dialog panels, buttons, scrollbars, shields, Civilopedia art, and bordered
  textures.
- Texture-cache clearing no longer mutates a dictionary while enumerating it and
  now releases generated shield textures too.
- Rebuilt and invalidated tile images are unloaded.  Map dimensions are cached by
  map and zoom level.
- The map renderer reuses its city draw list rather than allocating it every
  frame.  Shift-hover unit and road paths are cached until their inputs or view
  change instead of rerunning A* every frame.

The tracked unit PNG set fell from 76.3 MiB to 2.9 MiB.  Its theoretical decoded
RGBA footprint fell from about 300 MiB (50 images at 1254x1254) to about 17.9 MiB
(52 images at 300x300), a reduction of roughly 94%.  The extra two files cover
the missing Legion and Mechanized Infantry art while retaining a legacy alias.  More
importantly, that memory is no longer all committed at startup.

## Custom texture results

The source folder contained 173 valid PNGs (about 255 MiB); Pillow verification
found no corrupt files.  Most were 1254x1254 RGB generation outputs with colored
mattes rather than runtime-ready sprites.

`scripts/prepare_custom_textures.py` reproducibly creates optimized 300x300 RGBA
PNGs with transparency and high-quality downsampling.  Land units receive a
flattened silhouette shadow projected behind and left.  Naval units omit the land
shadow and use short baked waterline wake/splash strokes.  Both approaches keep
placement readable without adding another draw or runtime blur.  A final
output-stage color-key pass removes hot-pink matte remnants after resampling.

Accepted output:

- 52 unit files: all 51 standard named unit sources plus one legacy filename
  alias.  This includes the previously missing Legion and the new Mechanized
  Infantry cutout.
- 48 city sprites: eight each for Aztec, German, Greek, Japanese, London, and USA.
- 34 terrain/improvement overlays: eight mountains, eight hills, eight forests,
  eight rivers, a fort, and a fortified marker.
- 18 individual flags sliced from the source sheet.

Reviewed but deliberately excluded:

- Eight files in `cities/aborted`.
- `airstrip.png`, an abstract red placeholder, and `airstripfull.png`, which adds
  a modern passenger aircraft inconsistent with the unit scale and period.
- Redundant pre-downsampled RGB/magenta copies and alternate terrain batches where
  the clean source-derived overlay set already supplies all eight variations.
- Four full river terrain plates; the eight isolated river overlays are the useful
  compositing assets.

Run the conversion again with:

```sh
python3 scripts/prepare_custom_textures.py
```

The script requires Pillow and defaults to the source folder in the current
user's home directory.  `--source` and `--output` can override both locations.
