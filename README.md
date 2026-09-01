# rhYciv

rhYciv is a standalone, high-resolution turn-based empire strategy game. It
preserves the map, city, research, production, diplomacy, and tactical-unit
rhythm of 1990s 4X games while deliberately focusing the endgame on conquest.

No commercial game installation, CD, DLL, font, rules file, artwork, or save
file is required. The repository is designed to be a redistribution-ready base
for downstream forks. rhYciv is an independent project and is not affiliated
with or endorsed by Take-Two Interactive, 2K, Firaxis, MicroProse, or any other
publisher of the Civilization series.

![Fusion Power artwork](RaylibUI/FOSSart/Advances/fusionpower.jpg)

## Gameplay scope

- Explore a procedurally generated world, found cities, improve terrain, trade,
  research technologies, build wonders, and conduct diplomacy and war.
- New games default to conquest-only victory and permanent elimination.
- Spaceship victory, the throne room, animated advisors/high council, and wonder
  movies are outside the streamlined product scope.
- The renderer uses a 1920x1080 logical layout, native high-DPI drawing,
  128x64 working terrain tiles, 300x300 unit/citizen sources, and map zoom levels
  from -7 through 16.
- Land and air units have a rear-left silhouette shadow. Naval units have wakes
  and waterline splashes instead of land shadows.

See [the gameplay guide](docs/GAMEPLAY.md) for systems, differences, and controls.

## Install the Flatpak beta

Download `rhYciv-v0.3.0-beta.1-x86_64.flatpak` from the
[latest GitHub release](https://github.com/crhy/rhYciv/releases/latest), then:

```sh
flatpak install --user ./rhYciv-v0.3.0-beta.1-x86_64.flatpak
flatpak run io.github.crhy.rhYciv
```

The current public beta predates the clean-room font and fixture conversion.
Build the current `master` branch for the fully audited asset set until the next
release is published.

## Build and run

The desktop client requires the .NET 9 SDK:

```sh
dotnet run --project RaylibUI/RaylibUI.csproj
```

It launches directly with the bundled ruleset. An external compatible rules
directory is optional for mod and scenario testing.

Run the complete validation gate before distributing a build:

```sh
./scripts/quality_gate.sh
```

The gate verifies every attributed asset, restores dependencies, builds the
solution, and runs the test suite. Flatpak instructions are in
[packaging/flatpak/README.md](packaging/flatpak/README.md).

## Essential controls

- `F11`: toggle borderless desktop resolution
- `Ctrl` + mouse wheel: zoom the map
- `Ctrl` + middle click: reset to 1:1 zoom
- Middle click/drag: center or pan the map
- Hold `Ctrl`: inspect the tile under the pointer
- Hold `Shift`: preview unit, road, or trade-route paths
- `Shift` + right click: move eligible units of the active type
- `Ctrl` + `Alt` + arrows: brightness/saturation; Page Up/Down: gamma; Home: reset

## Forking and licensing

Code and project-original art are GPL-3.0-only unless an asset’s manifest row
says otherwise. Freeciv-derived standalone data remains GPL-2.0-or-later, and
Liberation fonts remain OFL-1.1. Every shipped media/data file has a pinned row
in [ASSET-MANIFEST.tsv](ASSET-MANIFEST.tsv).

Start with [the documentation index](docs/README.md),
[architecture guide](docs/ARCHITECTURE.md), [clean-room status](docs/CLEAN-ROOM-STATUS.md),
and [contribution rules](CONTRIBUTING.md). Legal and attribution notices are in
[NOTICE.md](NOTICE.md).
