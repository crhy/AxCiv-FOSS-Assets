# Changelog

Notable changes to rhYciv. Entries reference the issue they close.

The AppStream release notes in `packaging/flatpak/io.github.crhy.rhYciv.metainfo.xml`
carry a shorter, user-facing summary of each release; this file is the full record.

## [Unreleased] — 0.1.1

### Fixed

- The Alt key no longer moves focus into the menu bar. It opened the menus and
  had no way to close them again, so the key was a one-way trip that had to be
  undone with the mouse. The menus are still reachable by clicking them. (#103)
- Citizens in the city window are justified to the left of their panel. They were
  centred, which meant a citizen moved every time the city grew or a worker became
  a specialist, so the faces never stayed where they were last clicked. (#64)
- The Supplies and Demands lines in the city window are inset from the panel
  border instead of rendering hard against it. (#83)
- Disbanding a unit from the city window clears it from the Units Present and
  Units Supported boxes straight away. Both boxes only rebuilt their contents when
  the window was rescaled, so a disbanded unit stayed listed until something else
  resized the window. If the disbanded unit was the active one, the map now moves
  to the next unit instead of leaving it blinking where it stood. (#94)
- A unit put to sleep to recover now wakes when it is back to full health, instead
  of staying asleep for a player who has stopped thinking about it. A unit that was
  already healthy when told to sleep is unaffected and stays asleep until woken,
  and a fortified unit stays fortified. (#96)
- The Change Production list keeps a stable order: units first, then improvements,
  each in the order the ruleset declares them. The list is appended to as advances
  are discovered, so an item unlocked mid-game used to land at the bottom of sixty
  entries rather than in its usual place — which is how a buildable Temple went
  unnoticed after Ceremonial Burial. (#91)
- **Crash:** researching an advance that enables a terrain improvement — fortresses,
  for one — took the game down with a NullReferenceException. The notification went
  through a player interface that nothing in the solution implements, so the field
  was always null and the message could never be shown.
- Units and improvements in the Change Production list are drawn at a readable
  size. Their icons were scaled by a constant that divided by 1024, the size the
  art used to be; it is now 300 square, so units rendered at about fourteen pixels
  and improvement icons at roughly one. Icons are now fitted to the row, so the
  same thing cannot happen again when art is redrawn at a new size. (#81)

### Changed

- The Civilopedia's city-improvement pages no longer offer a Description button.
  The description is already on the page it navigated away from. (#104)

## [0.1.0] — 2026-09-05

The first release built for Linux, Windows and macOS together, and the first that
is not a beta. See [the release notes](docs/RELEASE-NOTES.md).

### Added

- Self-contained downloads for Windows x64, macOS (Apple silicon and Intel) and
  Linux x64, alongside the Linux Flatpak.
- Painted road and railway connection sprites, cut into the eight half-spokes the
  renderer composites per connected neighbour.
- Continuous integration running the quality gate on Linux, Windows and macOS.

### Changed

- The project is separated from its upstream fork. The solution, the projects, the
  engine namespace, the interface classes and the save directory all carry the
  rhYciv name. Saves written by earlier builds are migrated on first launch. (#67)
- The city window is finished in the same painted stone as the rest of the
  interface rather than flat grey.

### Fixed

- Road and railway connection sprites drew the full four-way rosette into all nine
  slots, so a tile with one neighbour showed roads running off every side.
