# Contributing to rhYciv

Contributions must keep the repository independently redistributable.

## Before opening a change

1. Do not copy art, audio, fonts, text, maps, saves, DLLs, or screenshots from a
   commercial game, fan wiki, unlicensed download, or “abandonware” archive.
2. Submit only work you created or material whose license is compatible with
   GPL-3.0 distribution. Record the author, SPDX license, canonical source URL,
   and any generation steps.
3. Keep compatibility observations factual. Reimplement behavior; do not copy
   source code or expressive text from a proprietary implementation.
4. Do not add binary save fixtures. Build deterministic test state through
   `CleanRoomGameFactory` or a smaller code-created fixture.
5. Avoid renumbering rule tables casually. Unit, advance, improvement, terrain,
   government, and leader indices are compatibility contracts used by saves,
   scripts, atlas slots, and gameplay code.

After adding or changing media, run:

```sh
python3 scripts/generate_asset_manifest.py
./scripts/quality_gate.sh
```

The first command updates hashes and attribution rows. Review those rows in the
same commit. A pull request that leaves the manifest stale will fail validation.

## Code and test expectations

- Target .NET 9 and preserve nullable annotations.
- Add focused tests for gameplay or serialization changes.
- Prefer deterministic seeds in map/game tests.
- Dispose CPU images and GPU textures explicitly; do not decode whole art sets
  during startup.
- Keep standalone startup working without a configured external path.

See `docs/ARCHITECTURE.md`, `docs/ASSET-PROVENANCE.md`, and
`docs/CLEAN-ROOM-STATUS.md` before making structural or asset changes.
