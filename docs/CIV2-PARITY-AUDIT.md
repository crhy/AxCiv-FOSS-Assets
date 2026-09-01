# Civilization II gameplay parity audit

## Scope

This pass compared the engine's rules against Civilization II Multiplayer Gold as
documented in the published combat and rules references. It covers unit
abilities, combat resolution, the unit lifecycle, wonders, player actions, and
the AI opponents. Rendering and art are covered separately by
[4K rendering](4K-RENDERING.md) and the
[standalone asset audit](STANDALONE-ASSET-AUDIT.md).

Items marked **fixed** were corrected in the same pass that produced this
document. Everything else is an open divergence.

Out of scope by design, and not treated as gaps: the throne room, advisor and
high-council sequences, wonder movies, spaceships and spaceship victory, and
global warming.

## Rules that were already faithful

These were checked against the reference and need no work:

- The 14-bit unit flag field parses correctly, and every flag lands on the unit
  the standard ruleset intends.
- Firepower modifiers: shore bombardment reduces both sides to 1, a ship caught
  on a land square has its firepower reduced to 1 while the attacker's doubles,
  and a helicopter attacked by a fighter is reduced to 1.
- Defence multipliers for veteran (x1.5), fortified (x1.5), fortress (x2), city
  walls (x3), SAM batteries, SDI, coastal fortress, and terrain.
- Killstack, with the correct exceptions for city, fortress and airbase tiles.
- Post-combat movement loss proportional to hit points lost, with the sea-unit
  minimum.
- Zones of control, including the flag and the automatic exemption for sea and
  air units, in both movement and pathfinding.
- Happiness: martial law, Temple/Colosseum/Cathedral, specialists, and the
  happiness wonders — Hanging Gardens, Michelangelo's Chapel, Oracle, Women's
  Suffrage, Cure for Cancer, Shakespeare's Theatre, J.S. Bach's Cathedral.
- Corruption, pollution accumulation, and city growth/starvation.

## Bundled standalone data

Three faults in the bundled ruleset made the game close to unplayable, none of
them in the engine.

**No dialogs.** `FOSSart/Standalone/Game.txt` held only its metadata header.
Every dialog in the game is read from that file, and `ShowPopup` silently does
nothing when a dialog is absent, so pressing B on a Settler opened no naming
prompt and founded no city. The same silence covered research selection, go-to,
find city, pillage, selling buildings, save and quit confirmation, the options
screens, every error message and all nine city notifications. Forty-five
definitions have been written.

**Six terrain overlays were the same placeholder.** `build_terrain1` wrote six
tinted copies of the fortress icon into the sheet column the renderer reads as
irrigation, farmland, mine, pollution, the grassland shield and the goody hut.
A red stone wall therefore appeared on every shielded grassland square. Each now
has its own overlay, drawn in the generator.

**City names parsed as a single list.** `CITY.txt` separated its sections with
`@STOP` and no blank line, but the reader ends a section on a blank line, so the
whole file resolved to one 714-entry `AMERICANS` list. Every other tribe fell
through to an `EXTRA` key that no longer existed, and the lookup indexed the
dictionary directly — so any non-American civilisation, including every AI,
threw `KeyNotFoundException` on founding its first city. Sections are now
separated correctly and the lookup no longer throws.

Still open on the same sheet: **the special-resource cells are plain coloured
circles.** The twenty-two resource paintings in `FOSSart/Other/` — buffalo, fish,
whales, wine, gems, gold, furs, silk, spice, oasis and the rest — are not drawn
by anything.

## Combat and unit abilities

**Fixed in this pass:**

- Pikemen-style x1.5 defence against mounted attackers. The flag was parsed and
  exposed on the model but never read. Civ II has no "is a horse" flag; a
  mounted attacker is identified as a land unit with two movement points, one
  hit point and one firepower, which selects exactly Horsemen through Cavalry in
  the standard rules and still behaves sensibly for modified rulesets.
- AEGIS-style defence: x3 against aircraft, x5 against missiles. Also parsed but
  never read.
- Cruise Missiles and Nuclear Missiles are expended by their own attack. The
  "destroyed after attacking" flag had no effect, so missiles survived combat.
- Veteran promotion. There was none; a unit surviving a battle now has the
  documented 50% chance of promotion.
- Air fuel range. `MovementFunctions` carried a bare `// TODO: Air unit out of
  fuel check`, so fighters and bombers could stay airborne indefinitely. Air
  units now count consecutive turns away from a city, airbase or carrier and
  crash when they reach their range.
- Ships that must stay near land. Triremes had no risk of loss at sea. The
  documented odds now apply — 1-in-2, improving to 1-in-4 with Seafaring and
  1-in-8 with Navigation, and nil while the Lighthouse stands. Civ II's bug where
  a trireme in a one-tile-island city drowns is deliberately not reproduced.
- The helicopter firepower penalty keyed on the attacker being an air unit while
  the matching defence penalty keyed on the attacker being able to engage air
  units. Both now use the latter.

**Open:**

- Paradrops. The flag is parsed, the menu carries a Paradrop entry, and no
  command implements it, so Paratroopers are ordinary infantry.
- Airlift. Menu entry present, no command, and the Airport improvement grants
  only veteran air units.
- Amphibious assault is referenced by the AI scripting layer but is not enforced
  in movement, so the marine restriction on attacking from a ship is not applied.
- Submarine visibility. Submarines are never hidden from other players; the
  "spot submarines" property is wired to the fighter flag and is read nowhere.
- Nuclear attack. A Nuclear Missile resolves as ordinary combat. There is no
  strike, no area destruction, no fallout.
- Disband. No command, so units cannot be disbanded and cannot be disbanded in a
  city for shields.

## Non-combat unit actions

None of the following exist in any form:

- **Diplomat and Spy actions** — establish embassy, investigate city, sabotage,
  steal technology, incite revolt, bribe unit. `MinBribe` is parsed from the
  rules and read by nothing. Both unit types are currently unable to do anything
  a Warrior cannot.
- **Caravan and Freight actions** — establish trade route, help build wonder.
  Trade-route fields exist in the save format and in the classic-save reader, but
  no gameplay path creates or uses one.

## Wonders

The happiness wonders listed above were already implemented. Everything else was
cosmetic: built, occupying a city slot, costing shields, and doing nothing.

**Obsolescence, fixed in this pass.** The bundled ruleset's `@ENDWONDER` block was
28 lines of `nil`, so no wonder ever expired — including the happiness wonders
that were otherwise working. The block now carries Civ II's obsoleting advances:
Hanging Gardens/Railroad, Colossus/Flight, Lighthouse/Magnetism, Great
Library/Electricity, Oracle/Theology, Great Wall/Metallurgy, Sun Tzu/Mobile
Warfare, King Richard's/Industrialization, Marco Polo/Communism, and
Leonardo's/Automobile. The rules parser now accepts a trailing `;` comment in that
section, as it already did for sounds, so each line can name its wonder.

**Effects implemented in this pass:**

- Pyramids — a granary in every city its owner holds.
- Colossus — an extra trade arrow on each worked tile of its city that already
  produces trade.
- Lighthouse — ships lost at sea prevented, and one extra movement point for
  every sea unit.
- Great Wall — free city walls in every city its owner holds, and double attack
  strength against barbarians.
- Sun Tzu's War Academy — every ground unit its owner builds is a veteran.
- King Richard's Crusade — an extra shield on each worked tile of its city.
- Copernicus' Observatory — half again the science in its city.
- Magellan's Expedition — two extra movement points for every sea unit.
- Isaac Newton's College — double science in its city.
- A. Smith's Trading Co. — pays the upkeep of every building that costs one gold.
- SETI Program — a research lab in every city.

**Still cosmetic**, and why:

- Great Library and Darwin's Voyage need a turn-level hook to grant advances.
- Leonardo's Workshop needs a unit-upgrade pass.
- Hoover Dam counts as a hydro plant, but power plants have no effect yet — see
  below.
- Marco Polo's Embassy, United Nations and the Eiffel Tower all grant embassies
  or reputation, which requires diplomacy to exist.
- The Statue of Liberty removes the anarchy between governments, which requires
  revolution to exist.
- The Manhattan Project gates nuclear weapons, which are not implemented.

## City improvements

Discovered while working on the wonders, and worth its own entry: **no building
provides a shield bonus.** The `Effects` enum has multipliers for tax, luxury and
science but none for production, and `CalculateOutput` applies none. So the
Factory, Manufacturing Plant, Power Plant, Hydro Plant, Nuclear Plant and Solar
Plant contribute nothing to production. They cost shields and upkeep and are pure
loss. This also blocks the Hoover Dam.

## Government and diplomacy

- **No revolution.** Governments affect city output, unit support and tax limits,
  but a civilisation keeps its starting government forever. The Kingdom menu's
  REVOLUTION entry has no command behind it, and there is no anarchy period.
- **No diplomacy.** Treaty state — contact, cease-fire, peace, alliance,
  vendetta, embassy, war — is read from classic saves and never used. There is
  no contact, no negotiation, no declaration of war, and no reputation. Civs are
  permanently hostile to each other.
- The Senate, which constrains aggression under Republic and Democracy, does not
  exist.

## AI opponents

`Engine/Scripts/default.ai.lua` is 153 lines and drives every non-barbarian
opponent. Against Civ II's AI:

- **No production logic.** The script registers no `City_Production_Complete`
  handler, so every AI city falls through to `ProductionPossibilities.AutoNext`.
  The AI does not choose what to build.
- **No expansion strategy.** Settlers check fertility of the current tile and
  otherwise wander. There is no site evaluation, no target selection, no
  escorting, and no sense of how many cities to aim for.
- **No terrain improvement.** The settler branch is a `--TODO terrain
  improvements` comment.
- **Research** picks the highest AI-value technology biased toward earlier
  epochs. There are no technology goals and no reaction to the game state.
- **Military behaviour** is a per-unit priority sort: attack if a move is an
  attack, otherwise drift toward the nearest enemy within 12 tiles. No stacks, no
  operational objectives, no defence of threatened cities beyond a garrison
  count, no naval transport planning.
- **Difficulty does not scale AI behaviour.** `AiPlayer` receives a difficulty
  level and never reads it.
- The script `print`s on every unit order and every unit move. In a late-game
  turn with hundreds of AI units this is a meaningful cost on top of being noise.

The C# fallback in `AiPlayer.GetFallbackAction` is a reasonable safety net —
attack an adjacent enemy, settle, otherwise do nothing — but it is a safety net,
not an opponent.

## Suggested order of work

1. Wonders. Largest gameplay gap per unit of effort, well-bounded, and each one
   is independent of the others.
2. Diplomat, Spy, Caravan and Freight actions. Four unit types that currently do
   nothing.
3. AI production and expansion. The single biggest determinant of whether the
   game feels like Civ II to play against.
4. Revolution and governments, then diplomacy.
5. The remaining unit abilities — paradrop, airlift, disband, amphibious
   enforcement, submarine visibility, nuclear strikes.
