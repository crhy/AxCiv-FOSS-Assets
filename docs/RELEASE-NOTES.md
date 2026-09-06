**Fixes two crashes and a round of interface problems reported against 0.1.0.**

If you are running 0.1.0, update — one of the crashes made saving fail permanently.

## Install

| Platform | Download |
|---|---|
| **Windows** (x64) | `rhYciv-0.1.1-win-x64.zip` — unzip, run `RaylibUI.exe` |
| **macOS** (Apple silicon) | `rhYciv-0.1.1-osx-arm64.zip` — unzip, drag `rhYciv.app` to Applications |
| **macOS** (Intel) | `rhYciv-0.1.1-osx-x64.zip` — same |
| **Linux** (x64) | `rhYciv-0.1.1-linux-x64.tar.gz` — extract, run `./RaylibUI` |
| **Linux** (Flatpak) | `rhYciv-0.1.1-x86_64.flatpak` |

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
flatpak install --user ./rhYciv-0.1.1-x86_64.flatpak
flatpak run io.github.crhy.rhYciv
```

## Crashes fixed

**Saving stopped working once the barbarians took a city.** The saved per-tribe city counter is indexed by tribe, and the barbarians have no slot in it. From the moment they founded or captured their first city, every save attempt failed and the game could not be written to disk. If you lost a game to this in 0.1.0, that is what happened.

**Researching certain advances took the game down.** Anything that enabled a terrain improvement — fortresses among them — tried to announce itself through an interface that was never implemented, and crashed instead.

## The city screen

Units and improvements in the **Change Production** list are drawn at a readable size. They were being scaled against the size the art used to be, so units rendered at about fourteen pixels and improvement icons at roughly one (#81).

That list also **keeps a stable order** now — units first, then improvements, each in ruleset order. It used to have newly unlocked items appended to the end, so something you had just researched appeared at the bottom of sixty entries instead of where you would look for it. That is why a buildable Temple could seem to be missing after Ceremonial Burial (#91).

The window itself is finished in the **same painted stone as the rest of the interface** instead of flat grey, citizens are justified left so they stop moving as the city grows (#64), and the Supplies and Demands lines no longer render hard against the panel border (#83).

## Units

- A unit that has finished its turn **stops blinking**. Nothing cleared the selection when no unit was left to move, so whichever unit spent the last move point stayed lit for the rest of the turn (#74).
- A unit put to sleep to recover **wakes when it is back to full health**. One that was already healthy when told to sleep stays asleep until you wake it (#96).
- **Disbanding a unit** clears it from the city window at once, instead of leaving it listed until something else resized the window (#94).

## Elsewhere

- The **Alt** key no longer opens the menus. It had no way to close them again, so it was a one-way trip that had to be undone with the mouse (#103).
- The Civilopedia's city-improvement pages drop the **Description** button, which navigated to text already on the page (#104).

## Known limitations

Diplomacy, multiplayer and several advisor screens are not implemented yet. Some interface art is still placeholder. Scripted Lua dialogs do not work at all — see #110.

Each download is about 190 MB because it is fully self-contained. It cannot be trimmed: the game discovers its interface implementations by reflection at startup, and trimming removes exactly those assemblies.

## Reporting problems

Please open an issue at https://github.com/crhy/rhYciv/issues. Say which platform and which download, and attach the log from:

- **Linux** `~/.local/share/rhYciv/Logs` (Flatpak: `~/.var/app/io.github.crhy.rhYciv/data/rhYciv/Logs`)
- **Windows** `%LOCALAPPDATA%\rhYciv\Logs`
- **macOS** `~/Library/Application Support/rhYciv/Logs`

The crash log is what made both of the fixes above possible — it names the exact line. Please attach it.
