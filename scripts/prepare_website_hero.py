#!/usr/bin/env python3
"""Cut the site's hero background out of the map screenshot.

The hero card sits behind the logo, the unit art and the version badge, so it
wants terrain and nothing else. The screenshot it comes from is a full window
capture: a menu bar across the top and the world/status panel down the right.
Both are interface chrome, and neither belongs behind a headline.

The crop keeps the coastline, the bison and the whale -- the part of the shot
that reads as a game rather than as a screenshot -- and leaves the chrome out.

This is committed rather than generated at deploy time: Cloudflare Pages runs
`website/build.sh`, which is plain file copying with no Python available. Re-run
this whenever the screenshot is replaced.

Usage:
    python3 scripts/prepare_website_hero.py
"""

from __future__ import annotations

import sys
from pathlib import Path

from PIL import Image

REPOSITORY = Path(__file__).resolve().parents[1]
SOURCE = REPOSITORY / "website" / "screenshot-map.jpg"
TARGET = REPOSITORY / "website" / "hero-map.jpg"

# Left/top/right/bottom in the 1920x1015 source. Starts below the menu bar and
# stops well clear of the side panel.
CROP = (300, 190, 1500, 1005)

# Wider than the card so it still covers when the layout goes single-column.
WIDTH = 1100


def main() -> int:
    if not SOURCE.exists():
        print(f"ERROR: no screenshot at {SOURCE}", file=sys.stderr)
        return 1

    with Image.open(SOURCE) as shot:
        hero = shot.convert("RGB").crop(CROP)

    hero = hero.resize((WIDTH, round(WIDTH * hero.height / hero.width)), Image.LANCZOS)
    hero.save(TARGET, quality=82, optimize=True, progressive=True)

    print(f"Wrote {TARGET.relative_to(REPOSITORY)} at {hero.width}x{hero.height}, "
          f"{TARGET.stat().st_size // 1024} KB")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
