#!/usr/bin/env python3
"""Convert Freeciv's GPL-2.0-or-later civ2 ruleset to rhYciv text data.

The generated files contain no MicroProse prose or binary art.  They retain
rhYciv's established numeric IDs so existing gameplay code and save files can
continue to use compact integer references.
"""

from __future__ import annotations

import argparse
import re
from collections import OrderedDict
from pathlib import Path


def sections(path: Path) -> OrderedDict[str, str]:
    result: OrderedDict[str, list[str]] = OrderedDict()
    current: list[str] | None = None
    for raw in path.read_text(encoding="utf-8").splitlines():
        match = re.match(r"\s*\[([^]]+)]", raw)
        if match:
            current = []
            result[match.group(1)] = current
        elif current is not None:
            current.append(raw)
    return OrderedDict((name, "\n".join(lines)) for name, lines in result.items())


def field(block: str, key: str, default: str = "") -> str:
    match = re.search(rf"(?m)^\s*{re.escape(key)}\s*=\s*(.*)$", block)
    return match.group(1).strip() if match else default


def field_block(block: str, key: str) -> str:
    match = re.search(rf"(?ms)^\s*{re.escape(key)}\s*=\s*(.*?)(?=^\s*[A-Za-z][\w.]*\s*=|\Z)", block)
    return match.group(1).strip() if match else ""


def quoted(value: str, default: str = "") -> str:
    strings = re.findall(r'"([^"\\]*(?:\\.[^"\\]*)*)"', value)
    if not strings:
        return default
    text = strings[0]
    if ":" in text and text.startswith("?"):
        text = text.split(":", 1)[1]
    return text.replace("\\\n", " ").replace("\\", "").strip()


def number(block: str, key: str, default: int = 0) -> int:
    match = re.search(r"-?\d+", field(block, key))
    return int(match.group()) if match else default


def quoted_list(value: str) -> list[str]:
    return [item.split(":", 1)[-1] if item.startswith("?") else item for item in re.findall(r'"([^"\\]*)"', value)]


def tech_requirement(block: str) -> str:
    match = re.search(r'"Tech"\s*,\s*"([^"]+)"', field_block(block, "reqs"))
    return match.group(1) if match else "None"


def bitfield(enabled: set[int], length: int = 14) -> str:
    return "".join("1" if index in enabled else "0" for index in reversed(range(length)))


def make_codes(names: list[str]) -> dict[str, str]:
    return {name: f"A{index:02X}" for index, name in enumerate(names)}


def tech_depth(name: str, prereqs: dict[str, tuple[str, str]], cache: dict[str, int]) -> int:
    if name in cache:
        return cache[name]
    parents = [parent for parent in prereqs.get(name, ()) if parent not in ("None", "Never", name)]
    cache[name] = 0 if not parents else 1 + max(tech_depth(parent, prereqs, cache) for parent in parents)
    return cache[name]


def build_rules(source: Path) -> tuple[str, str]:
    tech_data = sections(source / "techs.ruleset")
    unit_data = sections(source / "units.ruleset")
    building_data = sections(source / "buildings.ruleset")
    terrain_data = sections(source / "terrain.ruleset")
    nation_data = sections(source / "nations.ruleset")

    tech_blocks = [block for name, block in tech_data.items() if name.startswith("advance_")]
    tech_names = [quoted(field(block, "name")) for block in tech_blocks]
    codes = make_codes(tech_names)
    prereqs = {
        name: (quoted(field(block, "req1"), "None"), quoted(field(block, "req2"), "None"))
        for name, block in zip(tech_names, tech_blocks)
    }
    depths = {name: tech_depth(name, prereqs, {}) for name in tech_names}

    lines = [
        "; rhYciv standalone rules", "; Derived from Freeciv's civ2 ruleset; see SOURCES.md.", "", "@COSMIC",
        "3", "2", "2", "10", "10", "1", "2", "7", "14", "8", "12", "10", "20", "3", "3", "10",
        "0", "50", "50", "10", "75", "5", "1", "1", "1", "1", "1", "1", "1", "1", "", "@CIVILIZE"
    ]
    military = {"warfare", "weapon", "gun", "tactics", "leadership", "conscription", "chivalry"}
    economic = {"trade", "currency", "bank", "econom", "corporation", "industrial"}
    social = {"law", "government", "democracy", "republic", "communism", "theology", "philosophy"}
    for name in tech_names:
        depth = depths[name]
        epoch = 0 if depth <= 3 else 1 if depth <= 6 else 2 if depth <= 9 else 3
        lower = name.lower()
        category = 0 if any(word in lower for word in military) else 1 if any(word in lower for word in economic) else 2 if any(word in lower for word in social) else 3 if depth % 2 == 0 else 4
        req1, req2 = prereqs[name]
        lines.append(f"{name}, 4, 0, {codes.get(req1, 'nil')}, {codes.get(req2, 'nil')}, {epoch}, {category} ; {codes[name]}")

    improvement_names = (
        "Nothing", "Palace", "Barracks", "Granary", "Temple", "Marketplace", "Library", "Courthouse",
        "City Walls", "Aqueduct", "Bank", "Cathedral", "University", "Mass Transit", "Colosseum", "Factory",
        "Mfg. Plant", "SDI Defense", "Recycling Center", "Power Plant", "Hydro Plant", "Nuclear Plant",
        "Stock Exchange", "Sewer System", "Supermarket", "Super Highways", "Research Lab", "SAM Battery",
        "Coastal Defense", "Solar Plant", "Harbor", "Offshore Platform", "Airport", "Police Station",
        "Port Facility", "Unused Structural", "Unused Component", "Unused Module", "Capitalization", "Pyramids",
        "Hanging Gardens", "Colossus", "Lighthouse", "Great Library", "Oracle", "Great Wall",
        "Sun Tzu's War Academy", "King Richard's Crusade", "Marco Polo's Embassy", "Michelangelo's Chapel",
        "Copernicus' Observatory", "Magellan's Expedition", "Shakespeare's Theatre", "Leonardo's Workshop",
        "J.S. Bach's Cathedral", "Isaac Newton's College", "A.Smith's Trading Co.", "Darwin's Voyage",
        "Statue of Liberty", "Eiffel Tower", "Hoover Dam", "Women's Suffrage", "Manhattan Project",
        "United Nations", "Apollo Program", "SETI Program", "Cure for Cancer",
    )
    building_by_name: dict[str, str] = {}
    for section_name, block in building_data.items():
        if section_name.startswith("building_"):
            building_by_name[quoted(field(block, "name"))] = block
    aliases = {
        "SDI Defense": "SDI Defense", "Recycling Center": "Recycling Center",
        "Coastal Defense": "Coastal Defense", "Super Highways": "Super Highways",
        "Sun Tzu's War Academy": "Sun Tzu's War Academy", "King Richard's Crusade": "King Richard's Crusade",
        "Marco Polo's Embassy": "Marco Polo's Embassy", "Michelangelo's Chapel": "Michelangelo's Chapel",
        "Copernicus' Observatory": "Copernicus' Observatory", "Magellan's Expedition": "Magellan's Expedition",
        "Shakespeare's Theatre": "Shakespeare's Theatre", "Leonardo's Workshop": "Leonardo's Workshop",
        "J.S. Bach's Cathedral": "J.S. Bach's Cathedral", "Isaac Newton's College": "Isaac Newton's College",
        "A.Smith's Trading Co.": "A.Smith's Trading Co.", "Darwin's Voyage": "Darwin's Voyage",
        "Shakespeare's Theatre": "Shakespeare's Theater", "Cure for Cancer": "Cure For Cancer",
    }
    lines += ["", "@IMPROVE"]
    for index, name in enumerate(improvement_names):
        source_name = aliases.get(name, name)
        block = building_by_name.get(source_name, "")
        cost = number(block, "build_cost", 1 if index in (0, 38) else 40)
        upkeep = number(block, "upkeep", 0)
        req = tech_requirement(block)
        if index in (0, 35, 36, 37):
            req = "Never"
        lines.append(f"{name}, {cost}, {upkeep}, {codes.get(req, 'nil')}")
    lines += ["", "@ENDWONDER", *("nil" for _ in range(28)), "", "@UNITS"]

    unit_blocks = [(name, block) for name, block in unit_data.items() if name.startswith("unit_") and name != "unit_barbarian_leader"]
    for index, (section_name, block) in enumerate(unit_blocks):
        name = quoted(field(block, "name"), section_name.removeprefix("unit_").replace("_", " ").title())
        unit_class = quoted(field(block, "class"), "Land")
        domain = 2 if unit_class == "Sea" else 1 if unit_class in ("Air", "Helicopter", "Missile") else 0
        roles = set(quoted_list(field_block(block, "roles")))
        flags = set(quoted_list(field_block(block, "flags")))
        role = 5 if "Settlers" in roles else 6 if "Diplomat" in flags else 7 if {"TradeRoute", "HelpWonder"} & flags else 4 if domain == 2 and number(block, "transport_cap") else 3 if domain == 1 else 2 if domain == 2 else 1 if "DefendOk" in roles else 0
        enabled: set[int] = set()
        if number(block, "vision_radius_sq", 2) > 2: enabled.add(0)
        if "IgZOC" in flags: enabled.add(1)
        if "Marines" in flags: enabled.add(2)
        if "Submarine" in name: enabled.add(3)
        if name in ("Fighter", "Stealth Fighter", "AEGIS Cruiser"): enabled.add(4)
        if name == "Trireme": enabled.add(5)
        if name == "Howitzer": enabled.add(6)
        if name == "Carrier": enabled.add(7)
        if "Paratrooper" in name: enabled.add(8)
        if "Alpine" in name: enabled.add(9)
        if name == "Pikemen": enabled.add(10)
        if name == "Fanatics": enabled.add(11)
        if unit_class == "Missile": enabled.add(12)
        if name == "AEGIS Cruiser": enabled.add(13)
        req = tech_requirement(block)
        hitpoints = max(1, number(block, "hitpoints", 10) // 10)
        lines.append(
            f"{name}, nil, {domain}, {number(block, 'move_rate', 1)}, {number(block, 'fuel', 0)}, "
            f"{number(block, 'attack')}, {number(block, 'defense')}, {hitpoints}, {max(1, number(block, 'firepower', 1))}, "
            f"{number(block, 'build_cost', 10)}, {number(block, 'transport_cap')}, {role}, {codes.get(req, 'nil')}, {bitfield(enabled)}"
        )

    lines += ["", "@GOVERNMENTS"]
    government_titles = (
        ("Anarchy", "Leader", "Leader"), ("Despotism", "Chief", "Chief"), ("Monarchy", "King", "Queen"),
        ("Communism", "Chairman", "Chairwoman"), ("Fundamentalism", "Reverend", "Reverend"),
        ("Republic", "Consul", "Consul"), ("Democracy", "President", "President"),
    )
    lines += [f"{name}, {male}, {female}" for name, male, female in government_titles]

    nation_blocks = [(name, block) for name, block in nation_data.items() if name.startswith("nation_") and name not in ("nation_barbarian", "nation_pirate")][:21]
    lines += ["", "@LEADERS"]
    city_sections: list[tuple[str, list[str]]] = []
    for index, (section_name, block) in enumerate(nation_blocks):
        adjective = quoted(field(block, "name"), section_name.removeprefix("nation_").title())
        plural = quoted(field(block, "plural"), adjective + "s")
        leaders = re.findall(r'"([^"\\]+)"\s*,\s*"(Male|Female)"', field_block(block, "leaders"))
        male = next((name for name, sex in leaders if sex == "Male"), leaders[0][0] if leaders else f"Leader {index + 1}")
        female = next((name for name, sex in leaders if sex == "Female"), male)
        lines.append(f"{male}, {female}, 0, {index % 8 + 1}, {index % 4}, {plural}, {adjective}, 1, 1, 1")
        city_values = quoted_list(field_block(block, "cities"))
        city_names = [re.sub(r"\s*\([^)]*\)\s*$", "", city) for city in city_values[:32]]
        city_sections.append((plural.upper(), city_names or [f"{adjective} City"]))

    lines += [
        "", "@ORDERS", "Fortify,F", "Fortified,F", "Sleep,S", "Build Fortress,F", "Build Road,R",
        "Build Irrigation,I", "Build Mine,M", "Transform,O", "Clean Pollution,P", "Build Airbase,E",
        "Transport One,1", "Transport Two,2", "Go To,G", "", "@CARAVAN", "Food", "Textiles", "Metals",
        "Oil", "Manufactured Goods", "Medicine", "Machinery", "Technology", "Wine", "Spices", "Silk", "Gems",
        "Gold", "Coal", "Hides", "Salt", "", "@DIFFICULTY", "Chieftain", "Warlord", "Prince", "King", "Emperor", "Deity",
        "", "@ATTITUDES", "Worshipful", "Enthusiastic", "Cordial", "Receptive", "Neutral", "Uncooperative", "Hostile", "Enraged",
    ]

    terrain_order = ("Desert", "Plains", "Grassland", "Forest", "Hills", "Mountains", "Tundra", "Glacier", "Swamp", "Jungle", "Ocean")
    terrain_by_name = {
        quoted(field(block, "name")): (name, block)
        for name, block in terrain_data.items() if name.startswith("terrain_")
    }
    terrain_blocks = [terrain_by_name[name] for name in terrain_order]
    resource_blocks = {quoted(field(block, "extra")): block for name, block in terrain_data.items() if name.startswith("resource_")}
    terrain_names = [quoted(field(block, "name")) for _, block in terrain_blocks]
    terrain_codes = {name: name[:3] for name in terrain_names}
    lines += ["", "@TERRAIN"]
    terrain_resources: list[list[str]] = []
    for (_, block), name in zip(terrain_blocks, terrain_names):
        defense = max(1, (100 + number(block, "defense_bonus")) // 50)
        irrigate = "yes" if number(block, "irrigation_food_incr") > 0 else "no"
        mine = "yes" if number(block, "mining_shield_incr") > 0 else "no"
        transform_name = quoted(field(block, "transform_result"), "no")
        transform = terrain_codes.get(transform_name, "no")
        impassable = "yes" if "NoCities" in quoted_list(field_block(block, "flags")) and name != "Ocean" else "no"
        lines.append(
            f"{name}, {number(block, 'movement_cost', 1)}, {defense}, {number(block, 'food')}, {number(block, 'shield')}, {number(block, 'trade')}, "
            f"{irrigate}, {number(block, 'irrigation_food_incr')}, {number(block, 'irrigation_time')}, 0, "
            f"{mine}, {number(block, 'mining_shield_incr')}, {number(block, 'mining_time')}, 0, {transform}, {impassable} ; {terrain_codes[name]}"
        )
        terrain_resources.append(quoted_list(field_block(block, "resources"))[:2])

    for resource_index in range(2):
        for (_, block), resources in zip(terrain_blocks, terrain_resources):
            resource_name = resources[resource_index] if resource_index < len(resources) else "Bonus"
            resource = resource_blocks.get(resource_name, "")
            lines.append(
                f"{resource_name}, {number(block, 'movement_cost', 1)}, {max(1, (100 + number(block, 'defense_bonus')) // 50)}, "
                f"{number(block, 'food') + number(resource, 'food')}, {number(block, 'shield') + number(resource, 'shield')}, "
                f"{number(block, 'trade') + number(resource, 'trade')}"
            )

    rules = "\n".join(lines).rstrip() + "\n"
    city_lines = ["; City names derived from Freeciv's GPL civ2 nation data."]
    for name, cities in city_sections:
        city_lines += [f"@{name}", *cities, "@STOP"]
    city_lines += ["@EXTRA", "New Hope", "New Town", "New City", "Frontier", "@STOP", "@BARBARIANS", "Stronghold", "@STOP", "@end"]
    return rules, "\n".join(city_lines) + "\n"


def main() -> None:
    parser = argparse.ArgumentParser()
    parser.add_argument("source", type=Path, help="Freeciv data/civ2 directory")
    parser.add_argument("output", type=Path, help="rhYciv standalone data directory")
    args = parser.parse_args()
    required = ("techs.ruleset", "units.ruleset", "buildings.ruleset", "terrain.ruleset", "nations.ruleset")
    missing = [name for name in required if not (args.source / name).is_file()]
    if missing:
        parser.error("missing Freeciv sources: " + ", ".join(missing))
    rules, cities = build_rules(args.source)
    args.output.mkdir(parents=True, exist_ok=True)
    (args.output / "RULES.txt").write_text(rules, encoding="utf-8")
    (args.output / "CITY.txt").write_text(cities, encoding="utf-8")
    print(f"Generated standalone RULES.txt and CITY.txt in {args.output}")


if __name__ == "__main__":
    main()
