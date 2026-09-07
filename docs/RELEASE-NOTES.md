**Saving, and a turn that ends when you press Enter.**

Everything here was reported against 0.1.3 in issue #113, or found while fixing it.

## Install

| Platform | Download |
|---|---|
| **Windows** (x64) | `rhYciv-0.1.4-win-x64.zip` — unzip, run `RaylibUI.exe` |
| **macOS** (Apple silicon) | `rhYciv-0.1.4-osx-arm64.zip` — unzip, drag `rhYciv.app` to Applications |
| **macOS** (Intel) | `rhYciv-0.1.4-osx-x64.zip` — same |
| **Linux** (x64) | `rhYciv-0.1.4-linux-x64.tar.gz` — extract, run `./RaylibUI` |
| **Linux** (Flatpak) | `rhYciv-0.1.4-x86_64.flatpak` |

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
flatpak install --user ./rhYciv-0.1.4-x86_64.flatpak
flatpak run io.github.crhy.rhYciv
```

## Saving

**Save Game and Load Game work from the menu.** Both entries carried no command id, so they were drawn and did nothing when clicked. The commands behind them existed and were bound to Ctrl+S and Ctrl+L, so saving worked — but only if you already knew the shortcut, which is not where anyone looks for it.

**Autosave each turn actually saves.** It has been a checkbox in Game Options for as long as the dialog has existed, and nothing ever read it. It writes at the start of your turn, before anything has moved, so the newest autosave is always a position that can be picked up cleanly. Three slots rotate, and a failure part way through cannot destroy the one it is writing over.

**Opening the Save dialog could take the game down.** The name it offers is built from the leader's initials, taken with `Substring(0, 2)` — which throws on a one-letter leader, or a custom civilisation saved with the name left blank. It crashed before the dialog drew, so nothing on screen said why.

## The turn

**Enter ends the turn on the first press.** Ending a turn walks every unit giving it its end-of-turn processing, and it returned the moment it met one that needed a decision from you — a GoTo whose route no longer exists, a settler freed by finishing what it was building — abandoning the rest of the list. So each press got only as far as the next such unit.

It was not only slow. The units behind the one that stopped the walk were **never processed at all**, so a unit told to fortify did not become fortified, and did not get its defensive bonus, until whatever preceded it in the list had been dealt with.

## Diplomats

A Diplomat has no attack strength, so walking one into an enemy unit or city was refused outright: the unit could be researched, built and marched across the map, and then did nothing whatever.

Moving one onto somebody else's unit or city now offers to buy it. A lone unit in the open can be bribed; a stack cannot, which is the point of standing a second unit beside a valuable one, and nor can a garrison inside a city — that is bought by inciting the city, which brings its defenders over with it. A capital cannot be bought at any price. Both prices rise with the treasury the owner is sitting on and fall away with distance from the seat of their government.

## Rules

**Huts use the original game's measured odds.** There are five outcomes, equally likely — tribes, gold, mercenaries, scrolls, barbarians — and an empty village is **not one of them**. It exists only as a consolation when one of the five cannot be delivered. It was a sixth outcome here, drawn as often as the rest. Tribes and barbarians are also withheld in favour of mercenaries near a city, or before you have founded one.

**Switching production says what it will cost.** Changing between a unit, a building and a wonder forfeits a share of the work already done. The engine has always charged it; nothing said so, and the shields simply disappeared.

## The map

**The coastline.** It ran through saturated turquoise from 48 pixels out with a wide cream beach behind it, which drew a lit outline round every island. Measured against a photograph of a real fjord coast instead: the sea is nearly black up to the rock, the lightening at the shore is slight, and the beach is a thread. The shoreline also wanders further, so a coast is lobed rather than a chain of straight facets, and the water inside an enclosed square is lopsided instead of the perfect circle it was.

**The generation matte is gone.** The art is drawn on magenta and cut out, and cutting sets the alpha without touching the colour underneath — so citizens in the city window were outlined and veiled in pink, and several city sprites had magenta specks on the roofs.

**The marker for a unit killed in combat appears.** It worked out who had died from the per-round hitpoint series, which records each unit's health at the *start* of a round, before that round's damage. The loser's last entry is therefore its health just before the fatal blow — always above zero — so it never once decided anybody had died, and the pause and the marker never happened at all.

**Production shields are an even block**, every row the same length and as near square as the cost allows, rather than filling to the panel's width and stranding a single shield on the last row. **The flag over a city is three times the size**, and **special resources sit high in their square**, so a whale breaches out of water rather than sand.

## Elsewhere

- **Left and right step between cities** from inside the city window.
- The caret in a text box is clamped rather than trusted, so a key arriving with it out of range cannot end a session.


## Known limitations

Diplomacy, multiplayer and several advisor screens are not implemented yet. Some interface art is still placeholder. Scripted Lua dialogs do not work at all — see #110.

Each download is about 190 MB because it is fully self-contained. It cannot be trimmed: the game discovers its interface implementations by reflection at startup, and trimming removes exactly those assemblies.

## Reporting problems

Please open an issue at https://github.com/crhy/rhYciv/issues. Say which platform and which download, and attach the log from:

- **Linux** `~/.local/share/rhYciv/Logs` (Flatpak: `~/.var/app/io.github.crhy.rhYciv/data/rhYciv/Logs`)
- **Windows** `%LOCALAPPDATA%\rhYciv\Logs`
- **macOS** `~/Library/Application Support/rhYciv/Logs`
