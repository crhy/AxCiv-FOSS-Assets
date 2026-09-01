# Standalone asset audit

rhYciv's bundled standalone ruleset does not read art or data from a commercial
game installation. The files in `FOSSart/Standalone` are generated
compatibility atlases assembled from the project's individual replacement art;
they are not copies of commercial sprite sheets.

This document tracks visual/runtime coverage. Redistribution provenance is
tracked separately for every file in the repository-root `ASSET-MANIFEST.tsv`;
see `ASSET-PROVENANCE.md` and `CLEAN-ROOM-STATUS.md`.

## Completed replacements

- **PEOPLE:** all 44 era/role portraits are transparent 300x300 PNGs. The
  ancient through modern rows each contain happy, content, unhappy, and angry
  male/female pairs plus male entertainer, tax collector, and scientist roles.
- **TERRAIN1/TERRAIN2:** base terrain, blending masks, coast transitions, roads,
  railroads, irrigation, mines, pollution, fortresses, airstrips, and map UI
  compatibility regions are generated from the high-resolution terrain set.
- **UNITS:** all 51 standard unit slots map to the high-resolution individual
  unit PNGs. Land and air units retain rear-left silhouette shadows; ships use
  water wakes and no drop shadow. Unit flags and crisp health shields are
  generated into the legacy-compatible regions used by the current renderer.
- **CITIES:** all city styles, sizes, walls, ownership flags, and status markers
  have standalone equivalents.
- **ICONS:** every coordinate still accessed by the renderer is present in the
  generated adapter, including view controls, close/zoom controls, progress
  indicators, category markers, and resource indicators.
- **Backgrounds:** main menu, panel, and simplified city-view backgrounds are
  bundled. Intro, advisor, throne-room, and spaceship art is intentionally not
  part of the streamlined game.

## Art that can still be polished

These are attributed, functional placeholders, not missing runtime assets:

- unique illustrated terrain-special icons rather than the current clear
  procedural resource markers;
- bespoke high-resolution Civilopedia concept/category illustrations;
- richer battle, global-warming, and production-progress animation frames;
- additional city-view scenery if the optional decorative city panorama is
  restored later.

The generators are `scripts/build_standalone_sheets.py`,
`scripts/prepare_custom_textures.py`, and `scripts/prepare_people_sheet.py`.
Running them is deterministic and does not require original-game files.
