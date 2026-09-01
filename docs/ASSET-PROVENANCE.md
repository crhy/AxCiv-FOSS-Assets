# Asset provenance policy

`ASSET-MANIFEST.tsv` is the canonical, machine-readable inventory of distributed
art, fonts, and standalone data. Each row contains:

| Field | Meaning |
|---|---|
| `path` | Repository-relative path |
| `sha256` | Exact content identity |
| `bytes` | File size used as a quick integrity check |
| `kind` | Project original, generated adapter, third-party font, or derived data |
| `author` | Creator/rightsholder attribution |
| `license` | SPDX identifier governing that file |
| `source` | Canonical project, commit, or input group |
| `generator` | Reproducible generator or `none` |

Run `python3 scripts/generate_asset_manifest.py` after any asset change. The
quality gate recomputes the complete inventory and fails for a missing, stale, or
incomplete row.

## Current source groups

- Project-original art by crhy/rhYciv contributors: GPL-3.0-only.
- Generated icons and compatibility atlases: GPL-3.0-only; source inputs and
  generator are identified in their rows.
- Freeciv-derived `RULES.txt` and `CITY.txt`: GPL-2.0-or-later, pinned to commit
  `94beba8bc7d6512e485ae35103bdb8fb55babb4f`.
- Liberation Sans/Serif: unmodified OFL-1.1 fonts with the complete license in
  `Civ2/Fonts/OFL-1.1.txt`.
- Two small upstream AxxCiv UI atlases: GPL-3.0-only, with introduction commits
  and author attribution recorded per file.

## Accepting new assets

Do not use a search result, wiki, social post, fan download, screenshot, or
commercial installation as an asset source. A factual reference may inform a
new original work, but copying pixels, audio, prose, or a distinctive composition
is not acceptable.

For an external source, retain its license file and pin the exact source URL and
revision. For generated work, retain the preferred editable source or a
deterministic generator, document any external model/tool used, and assert that
the contributor has the right to license the output. If provenance cannot be
established, do not merge the file.
