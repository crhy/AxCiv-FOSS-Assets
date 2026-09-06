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
