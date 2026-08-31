#!/usr/bin/env python3
"""Build clean-room compatibility sheets from rhYciv's individual FOSS art."""

from __future__ import annotations

from pathlib import Path

from PIL import Image, ImageDraw, ImageEnhance, ImageFilter, ImageOps


ROOT = Path(__file__).resolve().parents[1]
ART = ROOT / "RaylibUI" / "FOSSart"
OUT = ART / "Standalone"

UNIT_FILES = (
    "settlers", "engineers", "warriors", "phalanx", "archers", "legions", "pikemen", "musketeers",
    "fanatics", "partisans", "alpinetroops", "riflemen", "marines", "paratroopers", "mechanizedinfantry",
    "horsemen", "chariot", "elephant", "crusaders", "knights", "dragoons", "cavalry", "armour",
    "catapult", "cannon", "artillery", "howitzer", "fighters", "bombers", "helicopter", "stealthfighter",
    "stealthbomber", "trireme", "caravel", "galleon", "frigate", "ironclad", "destroyer", "cruiser",
    "aegiscruiser", "battleship", "submarine", "carrier", "transport", "cruisemissile", "nuclearmissile",
    "diplomat", "spy", "caravan", "freight", "explorer",
)

TERRAIN_FILES = (
    "desert", "plains", "grassland", "grassland", "hills", "mountains", "tundra", "glacier", "swamp",
    "jungle", "ocean",
)

PLAYER_COLORS = (
    (96, 96, 96), (229, 45, 45), (48, 92, 214), (239, 196, 45), (53, 174, 78),
    (238, 137, 38), (131, 75, 183), (70, 190, 194), (226, 92, 159),
)


def contain(source: Image.Image, size: tuple[int, int], padding: int = 0) -> Image.Image:
    source = source.convert("RGBA")
    bounds = source.getchannel("A").getbbox()
    if bounds:
        source = source.crop(bounds)
    available = (max(1, size[0] - 2 * padding), max(1, size[1] - 2 * padding))
    source.thumbnail(available, Image.Resampling.LANCZOS)
    result = Image.new("RGBA", size, (0, 0, 0, 0))
    result.alpha_composite(source, ((size[0] - source.width) // 2, size[1] - padding - source.height))
    return result


def diamond_texture(path: Path) -> Image.Image:
    with Image.open(path) as loaded:
        tile = ImageOps.fit(loaded.convert("RGB"), (64, 32), Image.Resampling.LANCZOS)
    mask = Image.new("L", tile.size, 0)
    ImageDraw.Draw(mask).polygon([(32, 0), (63, 15), (32, 31), (0, 16)], fill=255)
    tile.putalpha(mask)
    return tile


def build_units() -> None:
    sheet = Image.new("RGBA", (612, 344), (0, 0, 0, 0))
    for index in range(63):
        name = UNIT_FILES[index] if index < len(UNIT_FILES) else "warriors"
        path = ART / "Units" / f"{name}.png"
        if not path.exists():
            continue
        with Image.open(path) as loaded:
            sprite = contain(loaded, (64, 48), 1)
        x, y = 1 + 65 * (index % 9), 1 + 49 * (index // 9)
        sheet.alpha_composite(sprite, (x, y))
        sheet.putpixel((x + 5, y - 1), (0, 0, 255, 255))
        sheet.putpixel((x - 1, y + 31), (0, 0, 255, 255))

    draw = ImageDraw.Draw(sheet)
    shield = [(1, 0), (10, 0), (11, 4), (9, 16), (6, 19), (3, 16), (0, 4)]
    for left in (586, 599):
        draw.polygon([(left + x, 1 + y) for x, y in shield], fill=(210, 45, 45, 255), outline=(30, 30, 30, 255))
    draw.polygon([(597 + x, 30 + y) for x, y in shield], fill=(210, 45, 45, 255), outline=(30, 30, 30, 255))
    sheet.save(OUT / "UNITS.png", optimize=True)


def build_cities() -> None:
    sheet = Image.new("RGBA", (600, 472), (0, 0, 0, 0))
    cultures = ("Aztec", "German", "Greek", "Japanese", "London", "USA")
    for row, culture in enumerate(cultures):
        for column in range(8):
            path = ART / "Cities" / culture / f"city_{column + 1:02}.png"
            with Image.open(path) as loaded:
                city = contain(loaded, (64, 48))
            x = 1 + 65 * (column % 4) if column < 4 else 334 + 65 * (column - 4)
            y = 39 + 49 * row
            sheet.alpha_composite(city, (x, y))
            sheet.putpixel((x + 5, y - 1), (0, 0, 255, 255))
            sheet.putpixel((x - 1, y + 28), (0, 0, 255, 255))
            sheet.putpixel((x + 50, y - 1), (255, 155, 0, 255))
            sheet.putpixel((x - 1, y + 39), (255, 155, 0, 255))

    draw = ImageDraw.Draw(sheet)
    for column, color in enumerate(PLAYER_COLORS):
        x = 1 + 15 * column
        draw.rectangle((x, 423, x + 13, 423), fill=(*color, 255))
        for row in range(2):
            flag_path = ART / "Flags" / f"flag_{row * 9 + column + 1:02}.png"
            with Image.open(flag_path) as loaded:
                flag = contain(loaded, (14, 22))
            sheet.alpha_composite(flag, (x, 425 + 23 * row))

    improvements = ART / "Terrain" / "Overlays" / "Improvements"
    for name, location in {
        "fortify": (143, 423), "fort": (208, 423), "airstrip": (273, 423), "airstripfull": (338, 423)
    }.items():
        with Image.open(improvements / f"{name}.png") as loaded:
            sheet.alpha_composite(contain(loaded, (64, 48)), location)
    sheet.save(OUT / "CITIES.png", optimize=True)


def draw_connections(draw: ImageDraw.ImageDraw, origin: tuple[int, int], railroad: bool = False) -> None:
    x, y = origin
    color = (212, 212, 205, 255) if railroad else (123, 81, 45, 255)
    width = 3 if railroad else 2
    center = (x + 32, y + 16)
    for edge in ((x, y + 16), (x + 32, y), (x + 63, y + 16), (x + 32, y + 31)):
        draw.line((center, edge), fill=color, width=width)
    if railroad:
        for offset in range(-24, 25, 8):
            draw.line((x + 32 + offset, y + 11, x + 32 + offset, y + 21), fill=(55, 55, 52, 255), width=1)


def build_terrain1() -> None:
    sheet = Image.new("RGBA", (586, 480), (0, 0, 0, 0))
    draw = ImageDraw.Draw(sheet)
    for row, name in enumerate(TERRAIN_FILES):
        base = diamond_texture(ART / "Terrain" / f"{name}.jpg")
        for x in (1, 66):
            sheet.alpha_composite(base, (x, 1 + 33 * row))
        for special, x in enumerate((131, 196), 1):
            resource = Image.new("RGBA", (64, 32), (0, 0, 0, 0))
            rdraw = ImageDraw.Draw(resource)
            rdraw.ellipse((27, 10, 37, 20), fill=((246, 205, 47, 230) if special == 1 else (235, 235, 235, 230)), outline=(40, 40, 40, 255), width=1)
            sheet.alpha_composite(resource, (x, 1 + 33 * row))

    for column in range(9):
        draw_connections(draw, (1 + 65 * column, 363), False)
        draw_connections(draw, (1 + 65 * column, 397), True)

    improvement = ART / "Terrain" / "Overlays" / "Improvements" / "fort.png"
    with Image.open(improvement) as loaded:
        icon = contain(loaded, (64, 32))
    for y, tint in ((100, None), (133, (80, 160, 80)), (166, (100, 100, 100)), (199, (120, 80, 80)), (232, (215, 55, 55)), (265, (205, 175, 75))):
        item = icon.copy()
        if tint:
            overlay = Image.new("RGBA", item.size, (*tint, 90))
            item = Image.alpha_composite(item, overlay)
        sheet.alpha_composite(item, (456, y))

    dither = Image.new("RGBA", (64, 32), (160, 160, 160, 255))
    dd = ImageDraw.Draw(dither)
    for y in range(32):
        for x in range(64):
            if (x + y) % 2:
                dd.point((x, y), fill=(0, 0, 0, 255))
    sheet.alpha_composite(dither, (1, 447))
    sheet.alpha_composite(diamond_texture(ART / "Terrain" / "ocean.jpg"), (131, 447))
    sheet.save(OUT / "TERRAIN1.png", optimize=True)


def build_terrain2() -> None:
    sheet = Image.new("RGBA", (530, 480), (0, 0, 0, 0))
    overlay_groups = (("Rivers", "river", 67), ("Forest", "forest", 133), ("Mountains", "mountain", 199), ("Hills", "hill", 265))
    for directory, stem, y in overlay_groups:
        for index in range(16):
            path = ART / "Terrain" / "Overlays" / directory / f"{stem}_{index % 8 + 1:02}.png"
            with Image.open(path) as loaded:
                item = contain(loaded, (64, 32))
            sheet.alpha_composite(item, (1 + 65 * (index % 8), y + 33 * (index // 8)))

    draw = ImageDraw.Draw(sheet)
    for index in range(16):
        x, y = 1 + 65 * (index % 8), 1 + 33 * (index // 8)
        draw.line((x, y + 16, x + 63, y + 16), fill=(68, 132, 178, 120), width=2)
    for column in range(4):
        x = 1 + 65 * column
        draw.arc((x + 4, 335, x + 60, 355), 180, 360, fill=(110, 187, 220, 220), width=3)
    for index in range(8):
        x = 1 + 66 * index
        draw.line((x, 445, x + 31, 429), fill=(100, 180, 215, 255), width=2)
        draw.line((x, 446, x + 31, 461), fill=(100, 180, 215, 255), width=2)
        draw.line((x, 463, x + 31, 479), fill=(100, 180, 215, 255), width=2)
        draw.line((x + 33, 479, x + 64, 463), fill=(100, 180, 215, 255), width=2)
    sheet.save(OUT / "TERRAIN2.png", optimize=True)


def build_icons() -> None:
    sheet = Image.new("RGBA", (600, 480), (0, 0, 0, 0))
    draw = ImageDraw.Draw(sheet)
    for y in range(sheet.height):
        shade = 78 + (y % 32) * 2
        draw.line((0, y, sheet.width, y), fill=(shade, shade, min(255, shade + 8), 255))

    draw.rounded_rectangle((199, 322, 262, 353), 7, fill=(65, 69, 78, 255), outline=(205, 210, 220, 255), width=2)
    draw.rounded_rectangle((298, 190, 329, 221), 5, fill=(218, 221, 227, 255), outline=(75, 78, 86, 255), width=2)
    draw.polygon([(199, 272), (231, 256), (262, 272), (231, 287)], outline=(245, 245, 245, 255), fill=(40, 55, 70, 180))
    for visible, x in ((False, 183), (True, 248)):
        for i in range(0, 64, 8):
            draw.line((x + i, 430, x + i, 461), fill=(245, 245, 245, 210 if visible else 110))
        for i in range(0, 32, 8):
            draw.line((x, 430 + i, x + 63, 430 + i), fill=(245, 245, 245, 210 if visible else 110))

    for column in range(8):
        x = 1 + 33 * column
        draw.ellipse((x + 4, 360, x + 27, 383), outline=(255, 215 - column * 12, 45, 255), width=3)
    for row_y, color in ((290, (85, 180, 255, 255)), (305, (255, 130, 45, 255))):
        for column in range(4):
            x = 49 + 15 * column
            draw.rectangle((x, row_y, x + 13, row_y + 13), fill=tuple(max(0, c - 18 * column) if i < 3 else c for i, c in enumerate(color)), outline=(25, 25, 25, 255))

    draw.rectangle((1, 389, 16, 404), fill=(185, 65, 65, 255), outline=(245, 245, 245, 255))
    draw.line((5, 393, 12, 400), fill=(255, 255, 255, 255), width=2)
    draw.line((12, 393, 5, 400), fill=(255, 255, 255, 255), width=2)
    for x, symbol in ((18, "+"), (35, "-")):
        draw.rectangle((x, 389, x + 15, 404), fill=(70, 100, 130, 255), outline=(245, 245, 245, 255))
        draw.text((x + 4, 388), symbol, fill=(255, 255, 255, 255))

    draw.ellipse((16, 320, 29, 333), fill=(245, 196, 45, 255), outline=(80, 55, 15, 255))
    draw.ellipse((31, 320, 44, 333), fill=(90, 185, 245, 255), outline=(20, 55, 80, 255))
    draw.polygon([(71, 343), (76, 334), (81, 343)], fill=(245, 225, 145, 255))

    for row in range(4):
        for column in range(5):
            x, y = 343 + 37 * column, 211 + 21 * row
            draw.rectangle((x, y, x + 35, y + 19), fill=(55 + 35 * column, 65 + 30 * row, 120 + 15 * column, 255), outline=(225, 225, 225, 255))
    sheet.save(OUT / "ICONS.png", optimize=True)


def build_backgrounds() -> None:
    backgrounds = OUT / "Backgrounds"
    backgrounds.mkdir(parents=True, exist_ok=True)
    menu = Image.new("RGB", (1600, 900))
    draw = ImageDraw.Draw(menu)
    for y in range(menu.height):
        t = y / (menu.height - 1)
        draw.line((0, y, menu.width, y), fill=(round(16 + 14 * t), round(38 + 38 * t), round(62 + 42 * t)))
    for x in range(-400, 1800, 160):
        draw.ellipse((x, 500 + (x % 240), x + 650, 1040 + (x % 240)), fill=(37, 86, 65), outline=(80, 130, 95), width=4)
    menu = menu.filter(ImageFilter.GaussianBlur(2))
    menu.save(backgrounds / "main_menu.jpg", quality=88, optimize=True, progressive=True)

    for name, color in (("panel", (68, 74, 84)), ("city_land", (93, 125, 77)), ("city_river", (70, 119, 137)), ("city_ocean", (40, 90, 132))):
        image = Image.new("RGB", (1280, 520), color)
        idraw = ImageDraw.Draw(image)
        for y in range(0, image.height, 26):
            idraw.line((0, y, image.width, y), fill=tuple(min(255, channel + 8) for channel in color), width=1)
        image.save(backgrounds / f"{name}.jpg", quality=86, optimize=True, progressive=True)


def main() -> None:
    OUT.mkdir(parents=True, exist_ok=True)
    build_units()
    build_cities()
    build_terrain1()
    build_terrain2()
    build_icons()
    build_backgrounds()
    print(f"Built standalone compatibility art in {OUT}")


if __name__ == "__main__":
    main()
