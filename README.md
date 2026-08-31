## rhYciv

rhYciv recreates the streamlined core gameplay of Civilization II with a
high-resolution renderer, expanded zoom controls, and freely licensed
replacement artwork. Spaceships, advisors, and the throne room are deliberately
outside the project's scope; conquest is the default endgame.

## FOSS art assets

- A primary goal of this fork is to replace the original art so the game can
  become a completely open-source standalone game.
![fusionpower](RaylibUI/FOSSart/Advances/fusionpower.jpg)

The bundled rules, city names, interface atlases, terrain, units, cities, and
citizens now run without a Civilization II installation. See the
[standalone asset audit](docs/STANDALONE-ASSET-AUDIT.md) for the few functional
placeholders that can still receive more polished art.

## Install the Flatpak beta

Download `rhYciv-v0.1.0-beta.1-x86_64.flatpak` from the
[latest GitHub release](https://github.com/crhy/rhYciv/releases/latest), then:

```bash
flatpak install --user ./rhYciv-v0.1.0-beta.1-x86_64.flatpak
flatpak run io.github.crhy.rhYciv
```

The Flatpak uses Freedesktop 25.08. Saves are stored in the app's private user
data directory, so the package does not request broad home-directory access.

## Local validation

- `dotnet build Civ2clone.sln`
- `dotnet test Civ2clone.sln`
- `./scripts/quality_gate.sh`

## Run locally

The desktop client requires the .NET 9 SDK:

```bash
dotnet run --project RaylibUI/RaylibUI.csproj
```

The game launches directly with its bundled standalone ruleset. An external
compatible rules directory remains optional for mod and scenario testing.

See [Flatpak packaging](packaging/flatpak/README.md) for local package builds.

### 4K and high-DPI rendering

- The interface automatically scales from a stable 1920x1080 logical canvas when the window is enlarged on a high-resolution display.
- Fonts, vector primitives, high-resolution FOSS units/icons, and the map backing render at native display density rather than being stretched from a 1080p frame.
- Bundled FOSS terrain is composed at 128x64 per normal-zoom tile, retaining twice the detail of the original 64x32 map pipeline.
- Press `F11` to toggle borderless desktop resolution. The mouse coordinate system scales with the display, so map interaction and dialogs retain their original layout.

See [4K rendering](docs/4K-RENDERING.md) for implementation details and current asset limitations.

### Enhanced map controls

- `F11`: toggle borderless desktop resolution
- `Ctrl` + mouse wheel: zoom the map
- `Ctrl` + middle click: reset to 1:1 zoom
- Middle click: center the map; middle drag: pan
- Hold `Ctrl`: show quick information for the tile under the pointer
- Hold `Shift`: preview the active unit path; with a city selected, preview road paths or its trade routes
- `Shift` + right click: move eligible units of the active unit's type
- `Ctrl` + `Alt` + arrows: adjust brightness/saturation; Page Up/Down adjusts gamma; Home resets
- Mouse wheel: scroll lists and tax sliders; over a specialist it changes type
- `Shift` + click/wheel on a specialist: change all specialists

See [Civ2 UI Additions compatibility](docs/CIV2-UI-ADDITIONS.md) for the complete upstream feature matrix.
