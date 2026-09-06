**The first release for Linux, Windows and macOS together — and the first that is not a beta.**

## Install

| Platform | Download |
|---|---|
| **Windows** (x64) | `rhYciv-0.1.0-win-x64.zip` — unzip, run `RaylibUI.exe` |
| **macOS** (Apple silicon) | `rhYciv-0.1.0-osx-arm64.zip` — unzip, drag `rhYciv.app` to Applications |
| **macOS** (Intel) | `rhYciv-0.1.0-osx-x64.zip` — same |
| **Linux** (x64) | `rhYciv-0.1.0-linux-x64.tar.gz` — extract, run `./RaylibUI` |
| **Linux** (Flatpak) | `rhYciv-0.1.0-x86_64.flatpak` |

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
flatpak install --user ./rhYciv-0.1.0-x86_64.flatpak
flatpak run io.github.crhy.rhYciv
```

## What's in this release

**Roads and railways are painted.** They were drawn as vector lines before, and every one of the nine connection sprites was the same four-way rosette — so a tile with a single neighbour showed roads running off all four sides. They are now painted art, cut into eight half-spokes that meet the neighbouring tile correctly on the shared edge.

**The city window is finished.** It filled its own panels with a flat grey that matched nothing else on screen, while the rest of the interface was painted stone. It now uses the same material.

**Separated from the upstream fork.** rhYciv began as a fork of Civ2-clone and had kept its names. The solution, the project names, the engine namespace, the interface classes and the save directory all now carry the rhYciv name. **Your existing saves are migrated automatically on first launch** — the old `AxxCiv` directory is copied across and left in place, so an older build still works.

**Checked on every platform, every change.** There was no build or test CI at all before this; there is now a quality gate running on Linux, Windows and macOS, which caught two platform-specific bugs during this release alone.

## Known limitations

Diplomacy, multiplayer and several advisor screens are not implemented yet. Some interface art is still placeholder. The Windows and macOS builds are new with this release and have had far less real-world use than the Linux one — please report anything that looks wrong.

Each download is about 190 MB because it is fully self-contained. It cannot be trimmed: the game discovers its interface implementations by reflection at startup, and trimming removes exactly those assemblies.

## Reporting problems

Please open an issue at https://github.com/crhy/rhYciv/issues. Say which platform and which download, and attach the log from:

- **Linux** `~/.local/share/rhYciv/Logs`
- **Windows** `%LOCALAPPDATA%\rhYciv\Logs`
- **macOS** `~/Library/Application Support/rhYciv/Logs`
