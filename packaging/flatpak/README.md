# Flatpak packaging

The manifest builds x86_64 against Freedesktop 25.08 and its .NET 9 SDK
extension. NuGet dependencies are pinned by URL and SHA-256 in
`nuget-sources.json`, so the sandboxed build does not contact NuGet.

## Before packaging

Run the repository quality gate. It verifies the complete asset manifest before
building or testing:

```sh
./scripts/quality_gate.sh
```

The published `v0.2.0-beta.1` bundle predates the Liberation-font and clean-test
conversion. Build current `master`, and supersede that beta rather than
republishing the older binary as clean-room complete.

## Local build

```sh
flatpak-builder --user --install-deps-from=flathub --force-clean \
  --repo=.flatpak-repo .flatpak-build \
  packaging/flatpak/io.github.crhy.rhYciv.yml

flatpak build-bundle .flatpak-repo rhYciv-x86_64.flatpak \
  io.github.crhy.rhYciv master \
  --runtime-repo=https://dl.flathub.org/repo/flathub.flatpakrepo
```

Install and run:

```sh
flatpak install --user ./rhYciv-x86_64.flatpak
flatpak run io.github.crhy.rhYciv
```

The application stores saves under its private XDG data directory and requests
no broad host-filesystem permission. The final package must contain `LICENSE`,
`NOTICE.md`, `ASSET-MANIFEST.tsv`, `RaylibUI/FOSSart/Standalone/SOURCES.md`, and
`UI.Classic/Fonts/OFL-1.1.txt`; inspect the exported file list before release.

## Updating NuGet sources

Use the official `flatpak-builder-tools` .NET generator when package references
change:

```sh
python3 flatpak-dotnet-generator.py \
  packaging/flatpak/nuget-sources.json RaylibUI/RaylibUI.csproj \
  --dotnet 9 --freedesktop 25.08 --runtime linux-x64
```
