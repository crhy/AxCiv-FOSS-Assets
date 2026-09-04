#!/usr/bin/env bash

set -euo pipefail

repo_root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"

if command -v dotnet >/dev/null 2>&1; then
    dotnet_cmd="$(command -v dotnet)"
elif [[ -x "${HOME}/.dotnet/dotnet" ]]; then
    dotnet_cmd="${HOME}/.dotnet/dotnet"
else
    echo "Error: .NET 9 SDK was not found in PATH or ${HOME}/.dotnet." >&2
    exit 1
fi

cd "$repo_root"

echo "Auditing redistributable assets..."
python3 scripts/generate_asset_manifest.py --check

echo "Verifying Civilopedia text..."
python3 scripts/build_civilopedia_text.py --check

echo "Restoring dependencies..."
"$dotnet_cmd" restore Civ2clone.sln

echo "Building solution..."
"$dotnet_cmd" build Civ2clone.sln --no-restore

echo "Running tests..."
"$dotnet_cmd" test Civ2clone.sln --no-build

echo "Quality gate passed."
