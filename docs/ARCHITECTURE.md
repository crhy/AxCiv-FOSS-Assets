# Architecture and clean forking

## Solution layout

| Project | Responsibility |
|---|---|
| `Model` | Interfaces, definitions, controls, events, and shared game objects |
| `Engine/Core` | Game state, turns, AI, map generation, production, scripting, saves, and rules parsing |
| `Civ2` | Shared classic-layout interface adapters, dialogs, menus, and atlas loading |
| `Civ2Gold` | Primary compact desktop interface implementation used by standalone rhYciv |
| `Civ2TOT` | Optional compatibility adapter for alternate external rules/content layouts |
| `RaylibUtils` | Raylib drawing and resource helpers |
| `RaylibUI` | Executable, initialization, rendering, input, dialogs, sound, shaders, and bundled assets |
| `Core.Tests` | Engine/unit tests and deterministic clean-room save-fixture generation |

`RaylibUI` discovers `IUserInterface` implementations from built assemblies.
`Game.txt` identifies the bundled `rhYciv Standalone` ruleset, which is routed to
the Gold-layout adapter without requiring a commercial product title in its own
metadata.

## Runtime data flow

1. `Settings` searches `FOSSart/Standalone` before optional external paths.
2. `RULES.txt`, `CITY.txt`, and `Game.txt` define gameplay and names.
3. `Civ2Interface` loads generated layout atlases plus individual high-resolution
   Civilopedia art.
4. The engine creates one map for the standard map descriptor, runs Lua behavior
   scripts from `Engine/Scripts`, and hands game state to the Raylib client.
5. Decoded/GPU images are cached lazily and disposed when the active interface or
   ruleset changes.

## Compatibility contracts

Many arrays are index-addressed. Preserve ordering for the 51 standard unit
slots, 88 advance slots, improvements/wonders, 11 terrain types, governments,
leaders, atlas coordinates, Lua calls, and serialized IDs. Add explicit mapping
or a save-version migration before changing an index.

The model still contains some spaceship, throne, advisor, and classic-save fields
so older JSON or user-owned saves can be read. Those dormant fields do not enable
the omitted gameplay or introduce an asset dependency.

## Saves and writable data

Saves are JSON `.sav` files. On Linux they live under
`$XDG_DATA_HOME/AxxCiv/Saves` (normally `~/.local/share/AxxCiv/Saves`); the
Flatpak redirects XDG storage into its private app data directory. Windows uses
Local Application Data and macOS uses Application Support.

## Generators and validation

| Command | Purpose |
|---|---|
| `scripts/import_freeciv_rules.py` | Rebuild adapted standalone rules and city names |
| `scripts/prepare_custom_textures.py` | Produce 300x300 transparent units/cities, shadows/wakes, terrain overlays, flags |
| `scripts/prepare_people_sheet.py` | Normalize the 44 high-resolution citizen/specialist portraits |
| `scripts/build_foss_icons.sh` | Build compact original icons |
| `scripts/build_standalone_sheets.py` | Assemble runtime compatibility atlases/backgrounds |
| `scripts/generate_asset_manifest.py` | Pin attribution and SHA-256 for every distributed media/data asset |
| `scripts/quality_gate.sh` | Verify manifest, restore, build, and test |

Tests do not ship copied rule, label, or save fixtures. `CleanRoomGameFactory`
parses the bundled rules, generates a seeded map, creates civilizations, writes a
save in memory, and reloads a fresh copy for each test.
