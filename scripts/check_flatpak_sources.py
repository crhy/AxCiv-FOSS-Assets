#!/usr/bin/env python3
"""Assert the Flatpak's offline NuGet mirror covers the declared package versions.

The Flatpak build has no network. It restores from `packaging/flatpak/nuget-sources.json`,
a vendored mirror of every .nupkg the build needs. That file is generated
separately from `Directory.Packages.props`, so the two can silently disagree --
and when they do, nothing notices until a release build fails, because every
other build in the project restores online.

That is exactly what happened to 0.1.0: `Microsoft.Extensions.Configuration` was
lifted from 5.0.0 to 9.0.0 without regenerating the mirror, the local quality
gate passed, CI passed on all three platforms, and the Flatpak job failed with
NU1102 twenty minutes into the release.

This checks the direct package references. It cannot see transitive ones, which
are resolved by NuGet rather than declared here, so regenerating the mirror
after any dependency change is still the rule -- see packaging/flatpak/README.md.
What it does guarantee is that the mismatch above can never reach a release again.
"""

from __future__ import annotations

import json
import re
import sys
from pathlib import Path

REPOSITORY = Path(__file__).resolve().parents[1]
PACKAGES = REPOSITORY / "Directory.Packages.props"
SOURCES = REPOSITORY / "packaging" / "flatpak" / "nuget-sources.json"


def declared_packages() -> dict[str, str]:
    text = PACKAGES.read_text(encoding="utf-8")
    return {
        match.group(1): match.group(2)
        for match in re.finditer(
            r'<PackageVersion\s+Include="([^"]+)"\s+Version="([^"]+)"\s*/>', text)
    }


def mirrored_files() -> set[str]:
    return {entry["dest-filename"].lower() for entry in json.loads(SOURCES.read_text())}


def main() -> int:
    declared, mirrored = declared_packages(), mirrored_files()
    if not declared:
        print(f"ERROR: no <PackageVersion> entries found in {PACKAGES.name}", file=sys.stderr)
        return 1

    missing = [
        f"{package} {version}"
        for package, version in sorted(declared.items())
        # Test-only packages are never part of the Flatpak build.
        if not package.lower().startswith(("xunit", "coverlet", "moq", "microsoft.net.test"))
        and f"{package}.{version}.nupkg".lower() not in mirrored
    ]

    if missing:
        print("ERROR: the Flatpak offline mirror is missing packages this build declares:",
              file=sys.stderr)
        for entry in missing:
            print(f"  {entry}", file=sys.stderr)
        print("\nRegenerate it -- see 'Updating NuGet sources' in "
              "packaging/flatpak/README.md.", file=sys.stderr)
        return 1

    print(f"Flatpak offline mirror covers all {len(declared)} declared packages.")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
