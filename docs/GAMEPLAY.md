# Gameplay

## Design target

rhYciv is a streamlined classic empire game. A session begins with settlers on
a generated world and grows through exploration, settlement, terrain work,
research, production, trade, diplomacy, and combined-arms war. The intended
victory is eliminating every rival civilization.

This is gameplay compatibility, not a pixel-identical preservation project.
The renderer, input model, art, packaging, and default rules are native rhYciv
work. Classic save/rules import remains optional compatibility functionality.

## Core loop

1. Reveal terrain and evaluate food, shield, trade, resource, river, and coast
   opportunities.
2. Found cities, assign workers/specialists, manage happiness, and select units,
   improvements, or wonders for production.
3. Improve the map with roads, railroads, irrigation, mines, fortresses, and
   airstrips while growing a transport network.
4. Allocate taxes, luxuries, and science; research a prerequisite-based
   technology tree and unlock governments, units, and buildings.
5. Negotiate where useful, establish trade routes, then defeat rival cities and
   units across land, sea, and air.

Combat uses attack, defense, hit points, firepower, movement, veteran status,
terrain defense, zones of control, transport capacity, and domain restrictions.
Health appears as a crisp green/yellow/red bar in the classic pointed unit shield.

## Victory and deliberate simplifications

New games enable conquest-only (“bloodlust”) victory and permanent elimination
by default. The advanced-rules dialog can expose those settings, but conquest is
the supported product path.

The following are intentionally omitted:

- spaceship construction and space-race victory;
- throne-room progression;
- animated high-council/advisor presentation;
- wonder movies and original intro cinematics;
- CD audio and legacy multiplayer/DirectPlay behavior.

Informational city, science, trade, attitude, military, and diplomacy screens
may still use familiar “advisor” terminology in code or UI. They are functional
reports, not the removed animated advisor feature.

## Display and input

The 1920x1080 logical interface scales in quarter steps to the display. Text,
vectors, textures, and map backing are rendered at display density. The map
supports zoom levels -7 through 16; level 0 is 1:1.

| Input | Action |
|---|---|
| `F11` | Toggle borderless desktop resolution |
| `Ctrl` + wheel | Zoom map |
| `Ctrl` + middle click | Reset map to 1:1 |
| Middle click / drag | Center / pan |
| Hold `Ctrl` | Tile, city, and unit quick information |
| Hold `Shift` | Unit/road path or city trade-route preview |
| `Shift` + right click | Mass-move eligible active-type units |
| Wheel over lists/sliders/specialists | Scroll or adjust |
| `Shift` + specialist click/wheel | Change every specialist |
| `Ctrl` + `Alt` + arrows | Brightness/saturation |
| `Ctrl` + `Alt` + Page Up/Down | Gamma |
| `Ctrl` + `Alt` + Home | Reset color correction |

## Current beta boundaries

Native multiplayer is not implemented. Some Civilopedia prose and category art
remain concise or procedural, and several animation/background assets are
functional placeholders. These gaps do not require external game data and are
tracked in `STANDALONE-ASSET-AUDIT.md`.
