**The game answers the keyboard and the mouse again.**

The main thing reported against 0.1.4 in issue #114 was that the interface felt
unresponsive: Enter had to be pressed several times before a turn would end, and
selecting a unit or clicking a city often did nothing. There were two separate
causes, and both are fixed. Everything else here was found while measuring them.

## Install

| Platform | Download |
|---|---|
| **Windows** (x64) | `rhYciv-0.1.5-win-x64.zip` — unzip, run `RaylibUI.exe` |
| **macOS** (Apple silicon) | `rhYciv-0.1.5-osx-arm64.zip` — unzip, drag `rhYciv.app` to Applications |
| **macOS** (Intel) | `rhYciv-0.1.5-osx-x64.zip` — same |
| **Linux** (x64) | `rhYciv-0.1.5-linux-x64.tar.gz` — extract, run `./RaylibUI` |
| **Linux** (Flatpak) | `rhYciv-0.1.5-x86_64.flatpak` |

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
flatpak install --user ./rhYciv-0.1.5-x86_64.flatpak
flatpak run io.github.crhy.rhYciv
```

## Pressing Enter over and over

**One press of End Turn plays one turn, not one civilisation.**

Starting a civilisation's turn handed control back and waited. That is right for you and wrong for everybody else: a computer civilisation is not interactive, so nothing ever came back to ask it for its next unit. Each rival moved a single unit and then stopped, holding the turn. One press of End Turn advanced the world by one civilisation rather than one turn — so with eight rivals on the board you had to press Enter eight times, into a game that appeared to be ignoring you, before you could move again.

Two things followed from the same fault, and are fixed with it. Rival civilisations only ever moved their **first** unit, all game; they now move their whole armies. And none of their end-of-turn orders ever ran, so a computer unit told to fortify never became fortified and their settlers never finished a road or an irrigation ditch they had started.

**A keypress is no longer lost because a frame ran long.**

Keys were read by asking about every key on the keyboard once a frame. A key pressed and let go between two frames is already back up by the time it is asked about, so on any frame that ran long the press simply never happened. That is the other half of having to press Enter several times, and it is why a quick click sometimes did nothing. Keys now come from the window's own queue of what was actually pressed, which cannot lose one however briefly it was held.

## Speed

**Redrawing the map is about ten times faster.** A screenful of map is a thousand tiles or more, and the whole picture is composed again whenever the view moves or anything on it changes. Every tile was handed to raylib's general-purpose image drawing, which resamples the tile to the size it is drawn at and then throws the answer away — once per tile, every redraw. Measured on a revealed map: seventeen to forty milliseconds before, two to three now.

That mattered for more than smoothness. A hundred-millisecond frame is long enough to swallow the click that follows it, which is why clicking a city sometimes had to be done twice.

**A redraw happens when it is asked for.** The static view of the map ticks once every two seconds, and a requested redraw waited for that tick — so ground coming into view, a city founded, a unit lost, or the grid being switched on could sit unseen for two seconds. A move being played out is still allowed to finish.

**The side panel keeps answering the mouse.** Almost everything in it is built again whenever the active unit changes, and the screen went on delivering the mouse to the controls that had just been replaced, so clicks in the panel did nothing until the pointer was moved away and back.

**A rival's turn cannot queue up minutes of watching.** Every move you can see is composed into a short animation the moment it happens. Now that rivals play a whole turn at once, a turn spent in sight of a dozen units would have built and then played a dozen of them; past a limit the move is drawn rather than animated.

## Research

**Choose Your Research has a Goal button.** Name an advance to work towards — anything you do not already have, however far off — and the chooser will list only the research that leads there: the goal's outstanding prerequisites, and the goal itself once everything it needs is known. When nothing you can begin now brings the goal any nearer, it says so plainly rather than showing you an empty list. The goal is kept in saved games and retires itself when you reach it.

**The chooser no longer asks the same question twice.** The guard against a second chooser stacking behind the first was being cancelled every turn by the backstop meant to protect it: the engine asks for research from the start of the turn's bookkeeping, which runs before you are told the turn has begun, so the flag was cleared on the same turn it was set. It now lasts until the question has actually been answered.

## For bug reports

`RHYCIV_FRAME_LOG=<seconds>` makes the game report how many frames it is drawing and how long the worst one took. A game that stops answering the keyboard is nearly always a game whose frames have grown long enough to swallow a keypress, and that is very hard to judge by eye.

## Known limitations

Diplomacy, multiplayer and several advisor screens are not implemented yet. Some interface art is still placeholder. Scripted Lua dialogs do not work at all — see #110.

Each download is about 190 MB because it is fully self-contained. It cannot be trimmed: the game discovers its interface implementations by reflection at startup, and trimming removes exactly those assemblies.

## Reporting problems

Please open an issue at https://github.com/crhy/rhYciv/issues. Say which platform and which download, and attach the log from:

- **Linux** `~/.local/share/rhYciv/Logs` (Flatpak: `~/.var/app/io.github.crhy.rhYciv/data/rhYciv/Logs`)
- **Windows** `%LOCALAPPDATA%\rhYciv\Logs`
- **macOS** `~/Library/Application Support/rhYciv/Logs`
