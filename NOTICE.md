# Notices and attribution

rhYciv is an independent open-source project. It is not affiliated with,
sponsored by, approved by, or endorsed by Take-Two Interactive, 2K, Firaxis,
MicroProse, or the owners of the Civilization trademarks. Product and company
names mentioned for compatibility or historical comparison belong to their
respective owners.

## Project license

Unless a file or manifest row says otherwise, rhYciv source code and
project-original art are distributed under GNU GPL version 3 only. See
`LICENSE`. A downstream distributor must preserve the license, source offer,
copyright notices, and this attribution information.

## Third-party material

- `RaylibUI/FOSSart/Standalone/RULES.txt` and `CITY.txt` are adaptations of
  Freeciv's `data/civ2` data at commit
  `94beba8bc7d6512e485ae35103bdb8fb55babb4f`, licensed GPL-2.0-or-later.
- Liberation Sans and Liberation Serif are unmodified Liberation Fonts releases
  licensed under SIL Open Font License 1.1. Copyright and license text are in
  `UI.Classic/Fonts/OFL-1.1.txt`.
- `UI.Classic/buttons.png` and `UI.Classic/explorer_icons.png` originated in the GPL-3.0
  AxxCiv/Civ2-clone codebase. Exact introduction commits and authors are pinned
  in the asset manifest.

*The New Textures are largely based on ideas from the original Civilization II,
however most of them were redone from sratch using ChatGPT as heavily enhanced
versions of the work by Blake from Blake's Sanctum as part of Better Terrain
Graphics mod by Blake in 2023

*None of this would have been possible without the genius of Sid Meier, the
creator of the original Civilization, and also part of one of the greatest
games ever made:  Civilization II, the gameplay of which we are trying to
emulate fairly precisely (minus some of the stupid stuff, like global warming
and the spaceship part of the game).

The authoritative per-file inventory is `ASSET-MANIFEST.tsv`. It records exact
SHA-256 hashes, sizes, authors, SPDX license identifiers, sources, and generators.

## Clean-room boundary

The runtime, tests, and package contain no commercial game assets. Compatibility
code may describe external file formats and can optionally import a user's own
files, but no such file is included or required. “Abandonware” is not treated as
a license or permission to redistribute copyrighted material.
