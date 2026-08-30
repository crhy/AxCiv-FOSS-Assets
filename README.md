## FOSS Art Assets
- The Main point of this fork is to add the FOSS Art Assets so the game can be a completely Open Source standalone game at some point.
![fusionpower](RaylibUI/FOSSart/Advances/fusionpower.jpg)

I'm also trying to vibe code some code validation tools and clean things up:

## Local validation

- `dotnet build Civ2clone.sln`
- `dotnet test Civ2clone.sln`
- `./scripts/quality_gate.sh`

## Run locally

The desktop client requires the .NET 9 SDK:

```bash
dotnet run --project RaylibUI/RaylibUI.csproj
```

On first launch, select a local Civilization II data directory containing
`RULES.TXT`. The bundled FOSS artwork is used where replacements exist, but it
does not yet include a complete standalone ruleset.

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
