# Flatpak packaging

The manifest builds an x86_64 beta against Freedesktop 25.08 and its .NET 9
SDK extension. NuGet dependencies are pinned by URL and SHA-256 checksum in
`nuget-sources.json`, so the sandboxed build does not access NuGet directly.

## Local build

Install `flatpak-builder`, add Flathub, then run from the repository root:

```sh
flatpak-builder --user --install-deps-from=flathub --force-clean \
  --repo=.flatpak-repo .flatpak-build \
  packaging/flatpak/io.github.crhy.rhYciv.yml

flatpak build-bundle .flatpak-repo rhYciv-x86_64.flatpak \
  io.github.crhy.rhYciv master \
  --runtime-repo=https://dl.flathub.org/repo/flathub.flatpakrepo
```

Install the resulting bundle with:

```sh
flatpak install --user ./rhYciv-x86_64.flatpak
flatpak run io.github.crhy.rhYciv
```

The GitHub Actions workflow performs the same build for tags and manual runs.

The application is standalone and stores saves under its private XDG data
directory. The manifest therefore needs no broad host-filesystem permission.

## Updating NuGet sources

Use the official `flatpak-builder-tools` .NET generator whenever package
references change:

```sh
python3 flatpak-dotnet-generator.py \
  packaging/flatpak/nuget-sources.json RaylibUI/RaylibUI.csproj \
  --dotnet 9 --freedesktop 25.08 --runtime linux-x64
```
