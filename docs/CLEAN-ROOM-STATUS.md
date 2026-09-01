# Clean-room redistribution status

As of 2026-08-31, current `master` is runtime-standalone and its shipped
dependencies are documented for redistribution. A commercial game installation
is neither required nor bundled.

## Completed boundary

- Bundled rules and city names come from a pinned GPL Freeciv revision and a
  reproducible importer.
- Units, cities, citizens, terrain, icons, backgrounds, and generated atlases are
  project-original or generated from project-original inputs.
- Commercial Arial and Times New Roman files were deleted. Unmodified Liberation
  Sans/Serif and the full OFL-1.1 license are bundled instead.
- Legacy-derived `RULES.TXT`, `Labels.txt`, classic `.sav`, and converted JSON
  fixture were deleted from `Core.Tests`.
- Tests create a seeded game from standalone data and serialize it in memory.
- Every shipped media/data asset has a path, hash, author, SPDX license, source,
  and generation field in `ASSET-MANIFEST.tsv`.
- The quality gate verifies manifest coverage and rejects the removed commercial
  font names or binary save fixtures.
- Standalone metadata uses the rhYciv name; commercial titles occur only in
  compatibility code, historical documentation, or non-affiliation context.

## Compatibility code that remains

Readers and adapters for user-supplied classic saves/rules may remain. Some
compile-disabled DLL extraction tables and legacy model fields also remain for
format compatibility. They contain code/data structure knowledge, not bundled
commercial binaries or art. A downstream fork that does not want import support
can remove `Engine/src/OriginalSaves`, `Civ2TOT`, and the dormant model fields in
a versioned save-format migration.

## Release rule

A release is redistribution-ready only when all of these pass from a clean tree:

```sh
python3 scripts/generate_asset_manifest.py --check
./scripts/quality_gate.sh
```

Package maintainers should also inspect the final archive/Flatpak file list and
retain `LICENSE`, `NOTICE.md`, `ASSET-MANIFEST.tsv`, the Freeciv notice, and the
OFL font license. The previously published `v0.2.0-beta.1` bundle predates this
conversion and should be superseded by a new build.

This document records an engineering/licensing audit, not legal advice. Copyright
does not disappear because software is unavailable or colloquially called
“abandonware”; only material with documented redistribution permission belongs
in this repository.
