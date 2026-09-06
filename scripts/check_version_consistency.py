#!/usr/bin/env python3
"""Assert the build version and the newest AppStream release entry agree.

The version is declared in two places that cannot see each other:
Directory.Build.props, which stamps the assemblies, and the Flatpak AppStream
metainfo, which is what a software centre shows. A release built with those two
disagreeing is a silent packaging bug -- the bundle installs and runs, but
reports a version nobody can match to a commit -- so the quality gate fails
rather than letting a tag go out that way.
"""

from __future__ import annotations

import re
import sys
from pathlib import Path

REPOSITORY = Path(__file__).resolve().parents[1]
BUILD_PROPS = REPOSITORY / "Directory.Build.props"
METAINFO = REPOSITORY / "packaging" / "flatpak" / "io.github.crhy.rhYciv.metainfo.xml"


def build_version() -> str:
    props = BUILD_PROPS.read_text(encoding="utf-8")
    prefix = re.search(r"<VersionPrefix>([^<]+)</VersionPrefix>", props)
    if prefix is None:
        raise SystemExit(f"Error: no <VersionPrefix> in {BUILD_PROPS.name}")
    suffix = re.search(r"<VersionSuffix>([^<]*)</VersionSuffix>", props)
    tail = suffix.group(1).strip() if suffix else ""
    return f"{prefix.group(1).strip()}-{tail}" if tail else prefix.group(1).strip()


def newest_release() -> str:
    metainfo = METAINFO.read_text(encoding="utf-8")
    release = re.search(r'<release\s+version="([^"]+)"', metainfo)
    if release is None:
        raise SystemExit(f"Error: no <release version=...> in {METAINFO.name}")
    return release.group(1)


def main() -> int:
    build, newest = build_version(), newest_release()
    if build != newest:
        print(
            f"ERROR: Directory.Build.props declares {build} but the newest AppStream "
            f"release is {newest}. Add the release entry to "
            f"packaging/flatpak/io.github.crhy.rhYciv.metainfo.xml before tagging.",
            file=sys.stderr,
        )
        return 1
    print(f"Version {build} agrees with the AppStream metainfo.")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
