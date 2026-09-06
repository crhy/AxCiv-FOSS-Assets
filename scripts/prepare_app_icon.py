#!/usr/bin/env python3
"""Cut the app icon from the project logo.

The logo is delivered as a badge on a flat near-black field. An application icon
has to be transparent around the badge: it sits on a desktop, a dock, a title bar
and a software-centre listing, none of which are black, and a square black tile
reads as a mistake in all of them.

The background cannot be removed by brightness alone. The metal in the badge goes
fully black in its own shadows -- darker than the field around it -- so a
threshold punches holes through the artwork. The background is instead found by
flooding inwards from the border, which only reaches the field itself, using the
same helper the rest of the art pipeline uses for generation mattes.

Usage:
    python3 scripts/prepare_app_icon.py [--source ~/rhYcivtextures/rhYcivLogo.png]
"""

from __future__ import annotations

import argparse
import sys
from pathlib import Path

from PIL import Image, ImageFilter

sys.path.insert(0, str(Path(__file__).resolve().parent))
from prepare_custom_textures import connected_background  # noqa: E402

REPOSITORY = Path(__file__).resolve().parents[1]
TARGET = REPOSITORY / "RaylibUI" / "FOSSart" / "rhyciv-app-icon.png"
DEFAULT_SOURCE = Path.home() / "rhYcivtextures" / "rhYcivLogo.png"

# Edge of the icon left clear, as a fraction of its width. A badge that touches
# the edge looks cramped everywhere it is drawn next to other icons.
MARGIN = 0.02

SIZE = 1024


def is_backdrop(pixel: tuple[int, int, int]) -> bool:
    """The flat field the badge was rendered on: near-black and neutral."""
    r, g, b = pixel
    return max(r, g, b) < 46 and (max(r, g, b) - min(r, g, b)) < 16


def cut(source: Path) -> Image.Image:
    art = Image.open(source).convert("RGB")

    background = connected_background(art, is_backdrop)
    alpha = Image.frombytes("L", art.size,
                            bytes(0 if flagged else 255 for flagged in background))

    # Pull the edge in before feathering it back: the badge is rendered against
    # the field with a soft edge, so the outermost ring of pixels is part
    # backdrop and stays as a dark halo if it is kept.
    alpha = alpha.filter(ImageFilter.MinFilter(5)).filter(ImageFilter.GaussianBlur(1.0))

    art.putalpha(alpha)
    art = art.crop(art.getbbox())

    # Square canvas, badge centred, with a margin so nothing touches the edge.
    inner = round(SIZE * (1 - 2 * MARGIN))
    art.thumbnail((inner, inner), Image.LANCZOS)
    icon = Image.new("RGBA", (SIZE, SIZE), (0, 0, 0, 0))
    icon.alpha_composite(art, ((SIZE - art.width) // 2, (SIZE - art.height) // 2))
    return icon


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("--source", type=Path, default=DEFAULT_SOURCE)
    args = parser.parse_args()

    if not args.source.exists():
        print(f"ERROR: no logo at {args.source}", file=sys.stderr)
        return 1

    icon = cut(args.source)
    icon.save(TARGET, optimize=True)

    opaque = icon.split()[3].point(lambda value: 255 if value > 8 else 0).getbbox()
    print(f"Wrote {TARGET.relative_to(REPOSITORY)} at {icon.size[0]}x{icon.size[1]}, "
          f"badge occupies {opaque[2] - opaque[0]}x{opaque[3] - opaque[1]}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
