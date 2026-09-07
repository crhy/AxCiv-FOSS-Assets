**The map, from a full beta test.**

Everything here was reported against 0.1.2 in issue #111.

## Install

| Platform | Download |
|---|---|
| **Windows** (x64) | `rhYciv-0.1.3-win-x64.zip` — unzip, run `RaylibUI.exe` |
| **macOS** (Apple silicon) | `rhYciv-0.1.3-osx-arm64.zip` — unzip, drag `rhYciv.app` to Applications |
| **macOS** (Intel) | `rhYciv-0.1.3-osx-x64.zip` — same |
| **Linux** (x64) | `rhYciv-0.1.3-linux-x64.tar.gz` — extract, run `./RaylibUI` |
| **Linux** (Flatpak) | `rhYciv-0.1.3-x86_64.flatpak` |

Nothing else is needed. No commercial Civilization II installation, no runtime to install — each download carries its own .NET runtime and the complete art set.

### The builds are unsigned — please read this before reporting a launch failure

They are not code-signed, because signing certificates cost money this project does not have yet. Both desktop platforms will try to stop you:

**macOS** will say the app "is damaged and can't be opened" or is from an unidentified developer. It is not damaged; that is the quarantine flag on anything downloaded unsigned. Clear it:

```
xattr -dr com.apple.quarantine /Applications/rhYciv.app
```

**Windows** will show a SmartScreen warning. Choose *More info* → *Run anyway*.

**Linux Flatpak**:

```
flatpak install --user ./rhYciv-0.1.3-x86_64.flatpak
flatpak run io.github.crhy.rhYciv
```

## The map

**Roads and railways connect.** A road is not one picture per square: the renderer composites one half-spoke per connected neighbour, and each has to run from the centre of the square to the exact point on the boundary where that neighbour is reached, so the two halves meet. They were being cut from the painted sources by measuring where the ink happened to lie — free-hand art, so it began and ended wherever the brush did. The geometry is constructed now and the painted surface swept along it.

**Rivers run as rivers, and reach the sea.** A river *is* one picture per square, chosen by which of the four neighbouring squares also carry water — sixteen distinct pictures. The art set held eight free-hand meanders, handed out by index modulo eight, so what was drawn had nothing to do with where the river ran and no two squares lined up. All sixteen are now composed from halves that meet on the boundary. River mouths had never been replaced at all: that coarse blue arc where a river met the coast was the compatibility sheet showing through.

**An ocean square is drawn as water.** The coastline art is chosen by how many of a square's four corners are land, and with three or four of them the shoreline never crossed the square at all, so it came out as solid grass. A one-square bay was a meadow, and the whales in it appeared to be breaching out of a field.

**Irrigation and farmland are ploughed fields.** They were still the compatibility sheet's 64×32 cell scaled up, which over photographic terrain reads as a blue lattice thrown across the square. Irrigation is hand-cut ditches; farmland is the same field cross-ploughed, with the channels meeting at the junctions.

**The flag over a city is sharp**, and the goody hut is the painted art. Both were drawing the classic sprite — a dozen pixels across — enlarged to match a map composed several times larger.

**The map no longer scrolls off into the fog.** Movement was being announced to every player who had ever *explored* the square it happened on rather than to those who could see it now, so every enemy step through territory you had once walked was animated on your map, and the view went after it.

## Playing a turn

**A road is worth a third of a movement point to every unit.** There was a rule that a unit whose whole allowance was a single movement point spent all of it on any move costing less than a full point — which is every move along a road. Settlers, Warriors, Phalanx and Musketeers were all walking their own roads at one square a turn.

**Enter ends the turn**, and the side panel says so, flashing *End of Turn (Press ENTER)* once every unit has moved.

**A kill can be seen.** Combat ended on the last frame of the explosion and handed straight back, so a unit killed during someone else's turn was gone before you could see it die. The map holds on the square for about a second now and marks it with a fallen-soldier icon; another civilisation's move is held at its destination for the same reason.

**Huts usually hold something.** The six outcomes were drawn evenly, and several of the others degrade into a consolation of their own, so a good third of huts came up empty. Mercenaries also arrive as soldiers now, rather than as a copy of whatever unit walked into the village — which had been handing a free Settlers to any settler that found a hut.

**A new city builds something it can build.** The opening item was the cheapest thing in the entire ruleset, drawn from tables that carry every slot the file format defines, including disabled ones costing nothing.

## Reading the screen

- **Small text is legible.** The font atlases are rasterised at 96–112 pixels and most text is drawn between 14 and 20. With only bilinear filtering, shrinking a glyph five times samples a twentieth of the pixels it covers and drops most of the stroke. They are mipmapped now.
- **City names in the Go To dialog** are set at a readable size; the listbox text size was fixed at 12 when the interface was laid out against a much smaller window.
- **The Civilopedia's technology description** is inset from its panel border instead of starting hard against the rule.
- **Production shields are justified** across the width of the box, so the row reads as a gauge.

## Known limitations

Diplomacy, multiplayer and several advisor screens are not implemented yet. Some interface art is still placeholder. Scripted Lua dialogs do not work at all — see #110.

Each download is about 190 MB because it is fully self-contained. It cannot be trimmed: the game discovers its interface implementations by reflection at startup, and trimming removes exactly those assemblies.

## Reporting problems

Please open an issue at https://github.com/crhy/rhYciv/issues. Say which platform and which download, and attach the log from:

- **Linux** `~/.local/share/rhYciv/Logs` (Flatpak: `~/.var/app/io.github.crhy.rhYciv/data/rhYciv/Logs`)
- **Windows** `%LOCALAPPDATA%\rhYciv\Logs`
- **macOS** `~/Library/Application Support/rhYciv/Logs`
