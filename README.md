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
