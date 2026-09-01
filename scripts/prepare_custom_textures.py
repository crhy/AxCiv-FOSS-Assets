#!/usr/bin/env python3
"""Prepare rhYciv's generated source art as compact, transparent game assets.

The input art deliberately remains outside the repository.  This script makes the
conversion repeatable: it removes the generation mattes and frames, downsamples
once with a high quality filter, and writes optimized 300x300 RGBA PNGs.
"""

from __future__ import annotations

import argparse
from collections import deque
from pathlib import Path

from PIL import Image, ImageDraw, ImageFilter


UNIT_NAMES = {
    "Pikeman.png": "pikemen.png",
    "aegis cruiser.png": "aegiscruiser.png",
    "alpinetroops.png": "alpinetroops.png",
    "archer.png": "archers.png",
    "armor.png": "armour.png",
    "artillery.png": "artillery.png",
    "battleship.png": "battleship.png",
    "bomber.png": "bombers.png",
    "cannon.png": "cannon.png",
    "caravan.png": "caravan.png",
    "caravel.png": "caravel.png",
    "carrier.png": "carrier.png",
    "catapult.png": "catapult.png",
    "cavalry.png": "cavalry.png",
    "chariot.png": "chariot.png",
    "chopper.png": "helicopter.png",
    "cruiser.png": "cruiser.png",
    "crusaders.png": "crusaders.png",
    "destroyer.png": "destroyer.png",
    "diplomat.png": "diplomat.png",
    "dragoons.png": "dragoons.png",
    "elephants.png": "elephant.png",
    "engineer.png": "engineers.png",
    "explorer.png": "explorer.png",
    "fanatic.png": "fanatics.png",
    "fighter.png": "fighters.png",
    "freight.png": "freight.png",
    "frigate.png": "frigate.png",
    "galleon.png": "galleon.png",
    "horseman.png": "horsemen.png",
    "howitzer.png": "howitzer.png",
    "icbm.png": "nuclearmissile.png",
    "ironsides.png": "ironclad.png",
    "knight.png": "knights.png",
    "legion.png": "legions.png",
    "marines.png": "marines.png",
    "mechanizedinfantry.png": "mechanizedinfantry.png",
    "missile.png": "cruisemissile.png",
    "muskateer.png": "musketeers.png",
    "paratrooper.png": "paratroopers.png",
    "partisans.png": "partisans.png",
    "phalanx.png": "phalanx.png",
    "riflemen.png": "riflemen.png",
    "settler.png": "settlers.png",
    "spy.png": "spy.png",
    "stealthbomber.png": "stealthbomber.png",
    "stealthfighter.png": "stealthfighter.png",
    "submarine.png": "submarine.png",
    "transport.png": "transport.png",
    "trireme.png": "trireme.png",
    "warrior.png": "warriors.png",
}

WATER_UNIT_NAMES = {
    "aegiscruiser",
    "battleship",
    "caravel",
    "carrier",
    "cruiser",
    "destroyer",
    "frigate",
    "galleon",
    "ironclad",
    "submarine",
    "transport",
    "trireme",
}


def is_unit_purple(pixel: tuple[int, int, int]) -> bool:
    r, g, b = pixel
    maximum = max(pixel)
    minimum = min(pixel)
    return (
        r > 24
        and b > 28
        and g < min(r, b) * 0.80
        and min(r, b) > max(r, b) * 0.50
        and maximum - minimum > 8
    )


def is_unit_background(pixel: tuple[int, int, int]) -> bool:
    r, g, b = pixel
    green = g > 65 and g > r * 1.17 and g > b * 1.10
    purple = is_unit_purple(pixel)
    return green or purple


def is_matte_background(pixel: tuple[int, int, int]) -> bool:
    r, g, b = pixel
    green = g > 60 and g > r * 1.18 and g > b * 1.10
    magenta = r > 90 and b > 105 and g < min(r, b) * 0.70
    return green or magenta


def is_flag_background(pixel: tuple[int, int, int]) -> bool:
    r, g, b = pixel
    # The sheet matte clusters tightly around RGB(212, 4, 224).  Keep this
    # deliberately narrow so dark-purple flags survive intact.
    return r > 160 and b > 170 and g < 70 and abs(r - b) < 80


def connected_background(image: Image.Image, classifier) -> bytearray:
    """Find only matte-colored pixels connected to an image edge."""
    rgb = image.convert("RGB")
    pixels = rgb.load()
    width, height = rgb.size
    background = bytearray(width * height)
    pending: deque[tuple[int, int]] = deque()

    def enqueue(x: int, y: int) -> None:
        offset = y * width + x
        if not background[offset] and classifier(pixels[x, y]):
            background[offset] = 1
            pending.append((x, y))

    for x in range(width):
        enqueue(x, 0)
        enqueue(x, height - 1)
    for y in range(1, height - 1):
        enqueue(0, y)
        enqueue(width - 1, y)

    while pending:
        x, y = pending.popleft()
        if x:
            enqueue(x - 1, y)
        if x + 1 < width:
            enqueue(x + 1, y)
        if y:
            enqueue(x, y - 1)
        if y + 1 < height:
            enqueue(x, y + 1)
    return background


def remove_small_islands(alpha: Image.Image, minimum_pixels: int = 16) -> Image.Image:
    pixels = alpha.load()
    width, height = alpha.size
    visited = bytearray(width * height)
    for start_y in range(height):
        for start_x in range(width):
            start = start_y * width + start_x
            if visited[start] or pixels[start_x, start_y] == 0:
                continue
            visited[start] = 1
            pending = [(start_x, start_y)]
            component: list[tuple[int, int]] = []
            while pending:
                x, y = pending.pop()
                component.append((x, y))
                for next_x, next_y in ((x - 1, y), (x + 1, y), (x, y - 1), (x, y + 1)):
                    if 0 <= next_x < width and 0 <= next_y < height:
                        offset = next_y * width + next_x
                        if not visited[offset] and pixels[next_x, next_y] != 0:
                            visited[offset] = 1
                            pending.append((next_x, next_y))
            if len(component) < minimum_pixels:
                for x, y in component:
                    pixels[x, y] = 0
    return alpha


def remove_hot_pink(sprite: Image.Image) -> Image.Image:
    """Remove residual generation-matte pixels after resampling."""
    pixels = sprite.load()
    for y in range(sprite.height):
        for x in range(sprite.width):
            r, g, b, a = pixels[x, y]
            if (
                a
                and r > 55
                and b > 40
                and r > b * 0.65
                and b > r * 0.40
                and g < min(r, b) * 0.70
            ):
                pixels[x, y] = (r, g, b, 0)
    return sprite


def add_unit_shadow(sprite: Image.Image) -> Image.Image:
    """Project a flattened silhouette behind and left of a land unit."""
    bounds = sprite.getchannel("A").getbbox()
    if bounds is None:
        return sprite
    left, top, right, bottom = bounds
    subject_width = right - left
    subject_height = bottom - top
    silhouette = sprite.getchannel("A").crop(bounds)
    shadow_width = max(24, round(subject_width * 0.88))
    shadow_height = max(9, round(subject_height * 0.27))
    silhouette = silhouette.resize((shadow_width, shadow_height), Image.Resampling.LANCZOS)
    silhouette = silhouette.rotate(8, resample=Image.Resampling.BICUBIC, expand=True)
    silhouette = silhouette.point(lambda value: round(value * 0.38))
    silhouette = silhouette.filter(ImageFilter.GaussianBlur(radius=1.2))

    mask = Image.new("L", sprite.size, 0)
    center_x = (left + right) // 2
    paste_x = max(0, center_x - round(silhouette.width * 0.86))
    paste_y = min(sprite.height - silhouette.height, bottom - round(silhouette.height * 0.62))
    mask.paste(silhouette, (paste_x, max(0, paste_y)), silhouette)
    shadow = Image.new("RGBA", sprite.size, (0, 0, 0, 0))
    shadow.putalpha(mask)
    return Image.alpha_composite(shadow, sprite)


def add_water_splash(sprite: Image.Image) -> Image.Image:
    """Bake a small waterline wake behind naval units instead of a land shadow."""
    bounds = sprite.getchannel("A").getbbox()
    if bounds is None:
        return sprite
    left, top, right, bottom = bounds
    width = right - left
    waterline = bottom - max(2, round((bottom - top) * 0.035))

    wake = Image.new("RGBA", sprite.size, (0, 0, 0, 0))
    draw = ImageDraw.Draw(wake, "RGBA")
    draw.arc((left - 7, waterline - 9, left + round(width * 0.38), waterline + 11),
             5, 174, fill=(218, 248, 255, 175), width=3)
    draw.arc((right - round(width * 0.38), waterline - 9, right + 7, waterline + 11),
             6, 175, fill=(230, 251, 255, 180), width=3)
    draw.arc((left + round(width * 0.22), waterline - 3,
              right - round(width * 0.22), waterline + 10),
             8, 172, fill=(105, 202, 233, 125), width=2)
    soft_wake = wake.filter(ImageFilter.GaussianBlur(radius=1.1))
    return Image.alpha_composite(Image.alpha_composite(soft_wake, wake), sprite)


def clean(source: Path, destination: Path, *, unit: bool = False, flag: bool = False) -> None:
    with Image.open(source) as loaded:
        # Segment at an intermediate size.  It is much faster than flood-filling
        # 1254px sources while preserving substantially more detail than output.
        working = loaded.convert("RGB")
        if unit:
            # A subset of the unit renders has a thin green/black presentation
            # frame.  Cropping it makes the actual studio matte the edge seed.
            margin = round(min(working.size) * 0.06)
            working = working.crop((margin, margin, working.width - margin, working.height - margin))
        working.thumbnail((600, 600), Image.Resampling.LANCZOS)
        canvas = Image.new("RGB", (600, 600), (255, 0, 255))
        canvas.paste(working, ((600 - working.width) // 2, (600 - working.height) // 2))

    classifier = is_unit_background if unit else (is_flag_background if flag else is_matte_background)
    matte = connected_background(canvas, classifier)
    if unit:
        # Subjects can partition small islands of the deliberately vivid magenta
        # key color from the edge-connected matte.  Remove that generation color
        # globally; the stricter saturation test preserves normal purples.
        pixels = canvas.load()
        width, height = canvas.size
        for y in range(height):
            for x in range(width):
                r, g, b = pixels[x, y]
                peak = max(r, b)
                if peak > 55 and g < peak * 0.48 and min(r, b) > peak * 0.55:
                    matte[y * width + x] = 1
    rgba = canvas.convert("RGBA")
    alpha = Image.frombytes("L", canvas.size, bytes(0 if value else 255 for value in matte))
    rgba.putalpha(alpha)
    output = rgba.resize((300, 300), Image.Resampling.LANCZOS)
    if unit:
        output = remove_hot_pink(output)
        # Suppress sub-visible matte/framing remnants introduced by Lanczos
        # interpolation while retaining the useful antialiasing at sprite edges.
        output_alpha = output.getchannel("A")
        output_alpha = output_alpha.point(lambda value: 0 if value < 64 else value)
        output_alpha = remove_small_islands(output_alpha)
        output.putalpha(output_alpha)
        output = (add_water_splash(output) if destination.stem in WATER_UNIT_NAMES
                  else add_unit_shadow(output))

    destination.parent.mkdir(parents=True, exist_ok=True)
    output.save(destination, "PNG", optimize=True, compress_level=9)


def numbered_sources(directory: Path, prefix: str = "ChatGPT Image") -> list[Path]:
    return sorted(path for path in directory.glob("*.png") if path.name.startswith(prefix))


def prepare_units(source: Path, output: Path) -> int:
    for input_name, output_name in UNIT_NAMES.items():
        clean(source / "units" / input_name, output / "Units" / output_name, unit=True)
    # Preserve the legacy singular lookup name without keeping its former
    # 1254px RGB copy.
    return len(UNIT_NAMES) + 1


def prepare_cities(source: Path, output: Path) -> int:
    count = 0
    for culture in ("Aztec", "German", "Greek", "Japanese", "London", "USA"):
        for index, image in enumerate(numbered_sources(source / "cities" / culture), 1):
            clean(image, output / "Cities" / culture / f"city_{index:02}.png")
            count += 1
    return count


def prepare_terrain(source: Path, output: Path) -> int:
    terrain = output / "Terrain" / "Overlays"
    mountains = numbered_sources(source / "mountains")
    groups = (
        (mountains[:8], terrain / "Mountains", "mountain"),
        (mountains[16:24], terrain / "Hills", "hill"),
        (numbered_sources(source / "trees"), terrain / "Forest", "forest"),
        (numbered_sources(source / "rivers")[4:12], terrain / "Rivers", "river"),
    )
    count = 0
    for images, directory, stem in groups:
        if len(images) != 8:
            raise RuntimeError(f"Expected 8 {stem} sources, found {len(images)}")
        for index, image in enumerate(images, 1):
            clean(image, directory / f"{stem}_{index:02}.png")
            count += 1
    for name in ("fort", "fortify", "airstrip", "airstripfull"):
        clean(source / f"{name}.png", terrain / "Improvements" / f"{name}.png")
        count += 1
    return count


def prepare_flags(source: Path, output: Path) -> int:
    with Image.open(source / "flags.png") as sheet:
        sheet = sheet.convert("RGB")
        left, top, right, bottom = 24, 168, 1959, 721
        width = (right - left) / 9
        height = (bottom - top) / 2
        temp = output / ".flag-source.png"
        count = 0
        for row in range(2):
            for column in range(9):
                crop = sheet.crop((
                    round(left + column * width),
                    round(top + row * height),
                    round(left + (column + 1) * width),
                    round(top + (row + 1) * height),
                ))
                # Drop the source sheet's green cell rules before keying.  Green
                # is a valid flag color and must not be treated as background.
                crop = crop.crop((32, 18, crop.width - 18, crop.height - 18))
                crop.save(temp)
                clean(temp, output / "Flags" / f"flag_{row * 9 + column + 1:02}.png", flag=True)
                count += 1
        temp.unlink()
    return count


def main() -> None:
    parser = argparse.ArgumentParser()
    parser.add_argument("--source", type=Path, default=Path.home() / "rhYcivtextures")
    parser.add_argument(
        "--output",
        type=Path,
        default=Path(__file__).resolve().parents[1] / "RaylibUI" / "FOSSart",
    )
    args = parser.parse_args()

    if not args.source.is_dir():
        parser.error(f"source directory does not exist: {args.source}")
    counts = {
        "units": prepare_units(args.source, args.output),
        "cities": prepare_cities(args.source, args.output),
        "terrain/improvements": prepare_terrain(args.source, args.output),
        "flags": prepare_flags(args.source, args.output),
    }
    print("Prepared " + ", ".join(f"{count} {name}" for name, count in counts.items()))


if __name__ == "__main__":
    main()
