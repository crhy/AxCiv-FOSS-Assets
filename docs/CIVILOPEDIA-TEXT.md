# rhYciv — Civilopedia Text

**Purpose:** original descriptive copy for every Civilopedia entry. Two or three
sentences each: what it was historically, then what it means for the player in game.

> Same voice as `PROMPT-EVENT-TEXT.md` — vivid, concrete, never a stat dump. The
> numbers are already shown next to the entry; this is the part that gives them meaning.

**Do not reorder or rename the headings.** They are in `RULES.txt` order, which is
what the generated `describe.txt` index maps onto. Write prose in the blank line
under each heading; leave a heading empty and it is simply skipped.

Entries marked *(placeholder — skip)* are engine slots with no player-facing meaning.

Concepts are the exception to the `RULES.txt` rule: they have no ruleset rows, so
that section's headings are both the menu the player browses and the entry titles.

Run `scripts/build_civilopedia_text.py` after editing. It regenerates
`RaylibUI/FOSSart/Standalone/describe.txt` and `pedia.txt`, which is what the game
actually reads, and fails if this document and `RULES.txt` disagree.


## Advances (88)

### Advanced Flight
<!-- requires: Radio, Machine Tools; unlocks: Bomber, Carrier -->

Advanced Flight turns basic aviation into strategic air power. Bombers let you strike hard at distant targets while Carriers project aircraft far beyond friendly shorelines, making this advance a major step toward global combined-arms warfare.

### Alphabet
<!-- requires: none, none -->

Alphabet is one of the foundational discoveries of civilization. It opens the path to law, writing, mathematics, and seafaring knowledge, so an early investment here rapidly expands both your scientific options and your ability to organize a growing empire.

### Amphibious Warfare
<!-- requires: Navigation, Tactics; unlocks: Marines, Port Facility -->

Amphibious Warfare makes coastlines far less secure. Marines can assault directly from the sea, while Port Facilities improve naval readiness, giving you the tools to invade defended continents without first finding a friendly beachhead.

### Astronomy
<!-- requires: Mysticism, Mathematics; unlocks: Copernicus' Observatory -->

Astronomy pushes navigation and science beyond the limits of simple observation. It unlocks Copernicus' Observatory and helps open the routes toward Navigation and Theory of Gravity, making it valuable to both scientific and maritime civilizations.

### Atomic Theory
<!-- requires: Theory of Gravity, Physics -->

Atomic Theory is the crucial theoretical bridge from classical physics to nuclear science. It does not transform your empire overnight, but it puts Nuclear Fission within reach and therefore brings the nuclear age dangerously close.

### Automobile
<!-- requires: Combustion, Steel; unlocks: Battleship, Super Highways; obsoletes: Leonardo's Workshop -->

Automobile accelerates both commerce and heavy warfare. Super Highways turn developed cities into stronger trade centers, Battleships deliver immense naval firepower, and the advance opens the road toward Mass Production and Mobile Warfare.

### Banking
<!-- requires: Trade, The Republic; unlocks: Bank -->

Banking deepens the value of every productive trade city. Banks multiply the economic output created by Marketplaces and this advance is also a gateway to Democracy, Economics, and Industrialization, making it one of the most important midgame economic technologies.

### Bridge Building
<!-- requires: Iron Working, Construction -->

Bridge Building removes rivers as barriers to a coherent road network. Connecting both sides of river systems improves reinforcement speed, trade, settlement efficiency, and eventually leads directly toward Railroad.

### Bronze Working
<!-- requires: none, none; unlocks: Phalanx, Colossus -->

Bronze Working is an early military and civic breakthrough. It gives you the defensive Phalanx, makes the Colossus possible, and begins important branches toward Currency and Iron Working, so it supports both survival and long-term growth.

### Ceremonial Burial
<!-- requires: none, none; unlocks: Temple -->

Ceremonial Burial gives young cities their first reliable tool for controlling unhappiness through Temples. It also begins the religious and governmental branches that lead toward Mysticism, Monarchy, and Polytheism.

### Chemistry
<!-- requires: University, Medicine -->

Chemistry is a powerful enabling science rather than an immediate military prize. It leads toward Explosives and Refining, which in turn unlock faster Engineers, combustion technology, modern industry, and the infrastructure of the industrial age.

### Chivalry
<!-- requires: Feudalism, Horseback Riding; unlocks: Knights -->

Chivalry fields Knights, fast mounted troops that can exploit weak fronts and overrun poorly defended territory. It also leads to Leadership, so researching it is the beginning of a long mounted-warfare upgrade path.

### Code of Laws
<!-- requires: Alphabet, none; unlocks: Courthouse -->

Code of Laws makes a larger empire easier to govern. Courthouses help control corruption and waste, while the technology opens major political and economic paths including Monarchy, Republic, Literacy, and Trade.

### Combined Arms
<!-- requires: Mobile Warfare, Advanced Flight; unlocks: Paratroopers, Helicopter -->

Combined Arms joins modern armor and advanced aviation into a single doctrine. Paratroopers can leap over front lines and Helicopters provide powerful mobile attack, giving you tools for rapid breakthroughs and attacks far behind conventional defenses.

### Combustion
<!-- requires: Refining, Explosives; unlocks: Submarine -->

Combustion brings modern engines to war and industry. It unlocks Submarines and leads directly toward Automobile and Flight, making it a pivotal transition from the industrial era into mechanized and air warfare.

### Communism
<!-- requires: Philosophy, Industrialization; unlocks: Police Station, United Nations, Communism government; obsoletes: Marco Polo's Embassy -->

Communism unlocks a government built for large, centrally controlled empires, together with Police Stations and the United Nations. It is especially useful when distance and internal order are becoming serious problems, and it opens Espionage and Guerilla Warfare as new strategic tools.

### Computers
<!-- requires: Mass Production, Miniaturization; unlocks: Research Lab, SETI Program -->

Computers transform late-game science. Research Labs and the SETI Program can produce enormous research output, while the technology leads onward to Robotics and Space Flight, making it one of the strongest accelerators in the modern tech race.

### Conscription
<!-- requires: Democracy, Metallurgy; unlocks: Riflemen -->

Conscription modernizes your defensive army with Riflemen. It also leads toward Tactics and Fundamentalism, so it serves as both an immediate defensive upgrade and a gateway to stronger military doctrines.

### Construction
<!-- requires: Masonry, Currency; unlocks: Aqueduct, Colosseum -->

Construction lets cities grow and remain orderly through Aqueducts and Colosseums. It also unlocks Bridge Building and Engineering, making it a central technology for civilizations that want bigger cities and better infrastructure.

### Currency
<!-- requires: Bronze Working, none; unlocks: Marketplace -->

Currency makes trade a deliberate source of power. Marketplaces improve taxes and luxuries, while Currency leads directly to Trade and Construction, strengthening both your treasury and your cities.

### Democracy
<!-- requires: Banking, Invention; unlocks: Statue of Liberty, Democracy government -->

Democracy offers one of the strongest peacetime governments, minimizing corruption and rewarding a highly developed trade economy, though overseas warfare can create serious unhappiness. It also unlocks the Statue of Liberty and helps open Espionage, Conscription, and Recycling.

### Economics
<!-- requires: Banking, University; unlocks: Stock Exchange, A.Smith's Trading Co. -->

Economics greatly strengthens a mature commercial empire. Stock Exchanges multiply taxable wealth and Adam Smith's Trading Company can erase much of the maintenance burden from inexpensive improvements, while the technology leads to The Corporation.

### Electricity
<!-- requires: Metallurgy, Magnetism; unlocks: Destroyer; obsoletes: Great Library -->

Electricity is a major gateway into the modern era. It unlocks Destroyers and leads to Electronics, Radio, Refrigeration, and Steel, but it also ends the Great Library's free-technology benefit, so reaching it can change the balance of a science strategy.

### Electronics
<!-- requires: The Corporation, Electricity; unlocks: Hydro Plant, Hoover Dam -->

Electronics turns electricity into large-scale industrial power. Hydro Plants and Hoover Dam can dramatically improve production without the dirtier profile of conventional power, while the technology opens several advanced military and scientific branches.

### Engineering
<!-- requires: The Wheel, Construction; unlocks: King Richard's Crusade -->

Engineering strengthens both infrastructure and production. It unlocks King Richard's Crusade and leads toward Invention and Sanitation, helping turn a medieval empire into one capable of rapid technological and urban development.

### Environmentalism
<!-- requires: Recycling, Space Flight; unlocks: Solar Plant -->

Environmentalism gives highly industrialized civilizations a cleaner way to keep producing. Solar Plants help reduce the pollution cost of enormous industrial output, making this technology especially useful to large late-game cities.

### Espionage
<!-- requires: Communism, Democracy; unlocks: Spy -->

Espionage upgrades diplomacy into professional intelligence warfare. Spies are faster and more capable than Diplomats, giving you a powerful way to steal technology, sabotage rivals, investigate cities, bribe units, and manipulate enemy plans without a conventional battle.

### Explosives
<!-- requires: Gunpowder, Chemistry; unlocks: Engineers -->

Explosives revolutionizes terrain development by unlocking Engineers. Their greater speed and work rate let you build roads, railroads, irrigation, mines, fortifications, and pollution cleanup far faster, which can transform both your economy and your military logistics.

### Feudalism
<!-- requires: Warrior Code, Monarchy; unlocks: Pikemen, Sun Tzu's War Academy -->

Feudalism improves defense with Pikemen and makes Sun Tzu's War Academy available. It also opens the paths to Chivalry and Theology, so it is a strong bridge from early government into specialized medieval military power.

### Flight
<!-- requires: Combustion, Theory of Gravity; unlocks: Fighter; obsoletes: Colossus -->

Flight brings true air warfare to the map through Fighters. Air power can respond rapidly across huge distances and challenge naval or land forces in ways older armies cannot, although discovering Flight also makes the Colossus obsolete.

### Fundamentalism
<!-- requires: Theology, Conscription; unlocks: Fanatics, Fundamentalism government -->

Fundamentalism unlocks a highly militarized government and inexpensive Fanatics. It favors conquest by supporting large armies and strong internal order at the cost of scientific efficiency, making it ideal when the priority is winning wars rather than maximizing research.

### Fusion Power
<!-- requires: Nuclear Power, Superconductors -->

Fusion Power is the culmination of the late nuclear-energy branch. Even without rhYciv's intentionally omitted space-race victory, reaching it demonstrates complete mastery of one of the most demanding scientific paths in the game.

### Genetic Engineering
<!-- requires: Medicine, The Corporation; unlocks: Cure for Cancer -->

Genetic Engineering turns advanced medicine into an empire-wide morale advantage through the Cure for Cancer. It is a late-game happiness technology that can make enormous, crowded civilizations easier to keep productive and stable.

### Guerilla Warfare
<!-- requires: Communism, Tactics; unlocks: Partisan -->

Guerilla Warfare unlocks Partisans, tough irregular troops that can slip through enemy zones of control and fight effectively in contested territory. It is excellent for resistance, raiding, and making an occupied frontier expensive for an enemy to hold.

### Gunpowder
<!-- requires: Invention, Iron Working; unlocks: Musketeers -->

Gunpowder makes ancient armor and formations increasingly obsolete. Musketeers provide durable gunpowder infantry, while the technology leads toward Explosives, Metallurgy, and Leadership, setting up the transition to industrial warfare.

### Horseback Riding
<!-- requires: none, none; unlocks: Horsemen -->

Horseback Riding gives early armies real operational mobility through Horsemen. It also begins the mounted branches leading to the Wheel, Chivalry, and Polytheism, making it a flexible first step for expansionist civilizations.

### Industrialization
<!-- requires: Railroad, Banking; unlocks: Transport, Factory, Women's Suffrage; obsoletes: King Richard's Crusade -->

Industrialization changes the scale of your civilization. Factories multiply production, Transports make overseas warfare practical, and Women's Suffrage helps democratic and republican powers sustain armies; the technology also opens several key modern economic and military branches.

### Invention
<!-- requires: Engineering, Literacy; unlocks: Leonardo's Workshop -->

Invention is a broad technological springboard. Leonardo's Workshop can modernize obsolete forces automatically, while Invention leads toward Gunpowder, Democracy, and Steam Engine, making it one of the most strategically connected advances in the tree.

### Iron Working
<!-- requires: Bronze Working, Warrior Code; unlocks: Legion -->

Iron Working gives you the Legion, one of the first genuinely dangerous assault units. It also leads toward Bridge Building, Gunpowder, and Magnetism, so the same technology that fuels ancient conquest becomes a foundation for later infrastructure and warfare.

### Labor Union
<!-- requires: Mass Production, Guerilla Warfare; unlocks: Mech. Inf. -->

Labor Union unlocks Mechanized Infantry, one of the strongest defensive land units in the game. It gives a modern empire a durable force for holding key cities and fronts against armor, artillery, and other late-game attackers.

### Laser
<!-- requires: Mass Production, Nuclear Power; unlocks: SDI Defense -->

Laser makes SDI Defense possible, giving cities a critical shield against nuclear attack. It also contributes to the Superconductors branch, so it combines direct strategic defense with continued progress toward the most advanced technologies.

### Leadership
<!-- requires: Chivalry, Gunpowder; unlocks: Dragoons -->

Leadership upgrades mounted warfare with Dragoons and leads directly to Tactics. It is especially useful when you want a mobile army to remain dangerous before the arrival of full industrial cavalry and armor.

### Literacy
<!-- requires: Writing, Code of Laws; unlocks: Great Library -->

Literacy makes organized knowledge a strategic resource. The Great Library can pull in discoveries known by rival civilizations, and Literacy opens multiple branches toward Philosophy, Physics, Invention, and the Republic.

### Machine Tools
<!-- requires: Steel, Tactics; unlocks: Artillery -->

Machine Tools brings industrial precision to the battlefield through Artillery. It also leads to Advanced Flight and Miniaturization, making it a key bridge between heavy weapons, aviation, offshore industry, and the computer age.

### Magnetism
<!-- requires: Iron Working, Physics; unlocks: Galleon, Frigate; obsoletes: Lighthouse -->

Magnetism produces stronger oceangoing fleets through Galleons and Frigates and leads toward Electricity. It also makes the Lighthouse obsolete, marking the point where navigation no longer depends on the ancient wonder's advantages.

### Map Making
<!-- requires: Alphabet, none; unlocks: Trireme, Lighthouse -->

Map Making allows the first serious naval exploration with Triremes and enables the Lighthouse. It also begins the Seafaring branch, making it essential for island worlds, coastal expansion, and early contact with distant civilizations.

### Masonry
<!-- requires: none, none; unlocks: Palace, City Walls, Pyramids, Great Wall -->

Masonry is one of the strongest early infrastructure technologies. It unlocks the Palace, City Walls, Pyramids, and Great Wall, giving you tools for administration, defense, growth, and wonder-driven expansion almost immediately.

### Mass Production
<!-- requires: Automobile, The Corporation; unlocks: Mass Transit -->

Mass Production brings modern urban industry to scale. Mass Transit helps control pollution while the technology opens Computers, Labor Union, Laser, Recycling, and Nuclear Fission, making it a central hub of the late-game tree.

### Mathematics
<!-- requires: Alphabet, Masonry; unlocks: Catapult -->

Mathematics gives early armies siege power through the Catapult. It also leads to Astronomy and University, so it serves as both a military technology and a gateway into serious scientific development.

### Medicine
<!-- requires: Philosophy, Trade; unlocks: Shakespeare's Theatre -->

Medicine improves the long-term health and happiness of a growing civilization. Shakespeare's Theatre can completely pacify its home city, and Medicine leads toward Sanitation, Chemistry, and Genetic Engineering.

### Metallurgy
<!-- requires: Gunpowder, University; unlocks: Cannon, Coastal Defense; obsoletes: Great Wall -->

Metallurgy brings Cannon and Coastal Defense into play, improving both offensive siege power and protection against naval bombardment. It also leads toward Conscription and Electricity, though it makes the Great Wall obsolete.

### Miniaturization
<!-- requires: Machine Tools, Electronics; unlocks: Offshore Platform -->

Miniaturization lets coastal cities exploit the sea more aggressively through Offshore Platforms. It also helps unlock Computers, tying advanced industrial engineering directly into the information age.

### Mobile Warfare
<!-- requires: Automobile, Tactics; unlocks: Armor; obsoletes: Sun Tzu's War Academy -->

Mobile Warfare brings Armor to the battlefield, giving land armies the speed and striking power needed for decisive modern offensives. It also leads to Combined Arms and Robotics, although its arrival makes Sun Tzu's War Academy obsolete.

### Monarchy
<!-- requires: Ceremonial Burial, Code of Laws; unlocks: Monarchy government -->

Monarchy replaces the harsh limits of primitive government with a more stable system suited to expansion and warfare. It improves the practicality of larger empires and opens Feudalism, making it a classic early target when Despotism is holding your cities back.

### Monotheism
<!-- requires: Philosophy, Polytheism; unlocks: Crusaders, Cathedral, Michelangelo's Chapel -->

Monotheism is both a civic and military upgrade. Cathedrals and Michelangelo's Chapel can suppress large amounts of unhappiness, while Crusaders provide a fast, hard-hitting offensive unit and Theology becomes available next.

### Mysticism
<!-- requires: Ceremonial Burial, none; unlocks: Oracle -->

Mysticism strengthens early religious institutions and unlocks the Oracle, whose Temple-enhancing effect can stabilize an expanding empire. It also opens the road toward Astronomy and Philosophy.

### Navigation
<!-- requires: Seafaring, Astronomy; unlocks: Caravel, Magellan's Expedition -->

Navigation makes long-distance sea power practical. Caravels and Magellan's Expedition improve exploration and fleet mobility, while the technology leads toward Amphibious Warfare and Physics.

### Nuclear Fission
<!-- requires: Mass Production, Atomic Theory; unlocks: Manhattan Project -->

Nuclear Fission opens the Manhattan Project and starts the nuclear age. Once the necessary global and missile conditions are met, nuclear weapons become a strategic possibility, while Nuclear Power provides a more constructive use for the same scientific breakthrough.

### Nuclear Power
<!-- requires: Nuclear Fission, Electronics; unlocks: Nuclear Plant -->

Nuclear Power gives cities access to Nuclear Plants, allowing enormous industrial output with different tradeoffs from conventional power. It also opens the path to Laser and Fusion Power, placing your civilization firmly in the high-technology era.

### Philosophy
<!-- requires: Mysticism, Literacy -->

Philosophy is a pivotal research technology: the first civilization to discover it is intended to gain a bonus advance, rewarding players who race for it. It also branches into Communism, Medicine, Monotheism, and University, so its strategic value extends far beyond the free discovery.

### Physics
<!-- requires: Literacy, Navigation -->

Physics is the scientific bridge between learned theory and the industrial age. It leads to Magnetism, Atomic Theory, and Steam Engine, putting naval modernization, nuclear science, and rail transport on the horizon.

### Plastics
<!-- requires: Refining, Space Flight -->

Plastics represents advanced materials engineering and the traditional space-age construction branch. In rhYciv it remains part of technological progression even though spaceship components and space-race victory are intentionally not part of the supported game.

### Polytheism
<!-- requires: Horseback Riding, Ceremonial Burial; unlocks: Elephants -->

Polytheism unlocks Elephants, a powerful early mounted attacker, and leads toward Monotheism. It gives religious development an immediate military payoff while preparing the way for much stronger happiness infrastructure.

### Pottery
<!-- requires: none, none; unlocks: Granary, Hanging Gardens -->

Pottery is an excellent growth technology. Granaries let cities retain food after population increases and the Hanging Gardens can improve happiness across the civilization, while Pottery also begins the Seafaring branch.

### Radio
<!-- requires: Flight, Electricity; unlocks: Airport -->

Radio turns aviation into a mature logistical system. Airports allow rapid movement and support of air forces, while Radio leads directly to Advanced Flight and the much heavier aircraft and carriers that follow.

### Railroad
<!-- requires: Steam Engine, Bridge Building; unlocks: Darwin's Voyage; obsoletes: Hanging Gardens -->

Railroad revolutionizes strategic movement by letting you build rail networks that can shift armies and economic resources across the empire at extraordinary speed. It also unlocks Darwin's Voyage and leads to Industrialization, though it ends the Hanging Gardens' effect.

### Recycling
<!-- requires: Mass Production, Democracy; unlocks: Recycling Center -->

Recycling helps an industrial empire manage the pollution created by high production. Recycling Centers reduce the environmental cost of factories and power plants, and the advance leads toward Environmentalism for even cleaner late-game industry.

### Refining
<!-- requires: Chemistry, The Corporation; unlocks: Power Plant -->

Refining unlocks the Power Plant and provides the industrial chemistry needed for Combustion and Plastics. It is the bridge from mature economics and chemistry into engines, advanced materials, and modern heavy industry.

### Refrigeration
<!-- requires: Sanitation, Electricity; unlocks: Supermarket -->

Refrigeration dramatically increases the food potential of developed cities. Supermarkets and advanced farming let fertile regions support very large populations, making it easier to staff specialist-heavy or high-production metropolises.

### Robotics
<!-- requires: Mobile Warfare, Computers; unlocks: Howitzer, Mfg. Plant -->

Robotics combines extreme industrial output with devastating siege power. Manufacturing Plants multiply city production and Howitzers smash fortified targets, while the technology also contributes directly to Stealth.

### Rocketry
<!-- requires: Advanced Flight, Electronics; unlocks: AEGIS Cruiser, Cruise Missile, Nuclear, SAM Battery -->

Rocketry transforms long-range warfare. SAM Batteries defend cities from aircraft, AEGIS Cruisers strengthen fleet air defense, Cruise Missiles deliver massive conventional strikes, and nuclear delivery systems become possible once the Manhattan Project has opened the nuclear era.

### Sanitation
<!-- requires: Engineering, Medicine; unlocks: Sewer System -->

Sanitation removes one of the major limits on urban growth. Sewer Systems allow cities to grow beyond the Aqueduct era and the technology leads to Refrigeration, making it essential for building the largest population centers.

### Seafaring
<!-- requires: Pottery, Map Making; unlocks: Explorer, Harbor -->

Seafaring improves the value of coastal civilization. Harbors strengthen food production from ocean tiles, Explorers help reveal distant territory, and the technology leads to Navigation and truly reliable overseas expansion.

### Space Flight
<!-- requires: Computers, Rocketry; unlocks: Apollo Program -->

Space Flight unlocks the Apollo Program and the final space-age branches. Apollo provides extraordinary global intelligence, while rhYciv intentionally keeps victory focused on conquest rather than spaceship construction or a space-race ending.

### Stealth
<!-- requires: Superconductors, Robotics; unlocks: Stealth Fighter, Stealth Bomber -->

Stealth unlocks the most advanced aircraft in the game. Stealth Fighters and Stealth Bombers combine exceptional speed and firepower with the advantages of low-observable warfare, making them ideal for breaking late-game defenses and controlling huge theaters.

### Steam Engine
<!-- requires: Physics, Invention; unlocks: Ironclad, Eiffel Tower -->

Steam Engine ushers in mechanized industry and armored navies. Ironclads dominate older wooden fleets, the Eiffel Tower becomes available, and the technology leads directly to Railroad and the logistics revolution that follows.

### Steel
<!-- requires: Electricity, Industrialization; unlocks: Cruiser -->

Steel unlocks the Cruiser and provides the materials base for Automobile and Machine Tools. It is a key industrial military technology that links heavy manufacturing to modern fleets, vehicles, and artillery.

### Superconductors
<!-- requires: Nuclear Power, Laser -->

Superconductors is a late-game scientific breakthrough that opens both Fusion Power and Stealth. Its main value is access to the final tier of energy science and air warfare rather than an immediate city improvement of its own.

### Tactics
<!-- requires: Conscription, Leadership; unlocks: Alpine Troops, Cavalry -->

Tactics modernizes the entire land-war doctrine. Alpine Troops cross rough terrain with remarkable speed, Cavalry provides powerful mobile attack, and the technology opens Amphibious Warfare, Guerilla Warfare, Machine Tools, and Mobile Warfare.

### The Corporation
<!-- requires: Economics, Industrialization; unlocks: Freight, Capitalization -->

The Corporation turns a developed economy into a modern commercial system. Freight improves long-distance trade, Capitalization converts unused production into money, and the technology opens major branches toward Electronics, Mass Production, Refining, and Genetic Engineering.

### The Republic
<!-- requires: Code of Laws, Literacy; unlocks: Republic government -->

The Republic is a major economic government upgrade. It rewards developed trade networks and can produce excellent science and income, though deployed military forces require more careful happiness management; it also leads toward Banking.

### The Wheel
<!-- requires: Horseback Riding, none; unlocks: Chariot -->

The Wheel upgrades early mobility with the Chariot and leads toward Engineering. It gives expansionist players a faster attacker while also serving as a necessary step toward stronger infrastructure.

### Theology
<!-- requires: Feudalism, Monotheism; unlocks: J.S. Bach's Cathedral; obsoletes: Oracle -->

Theology unlocks J.S. Bach's Cathedral, providing a major happiness advantage, and leads toward Fundamentalism. Reaching Theology also makes the Oracle obsolete, shifting religious power from ancient Temple bonuses to later institutions and governments.

### Theory of Gravity
<!-- requires: Astronomy, University; unlocks: Isaac Newton's College -->

Theory of Gravity pushes scientific cities into a new era. Isaac Newton's College can massively increase research in its home city, and the advance leads toward both Atomic Theory and Flight.

### Trade
<!-- requires: Currency, Code of Laws; unlocks: Caravan, Marco Polo's Embassy -->

Trade makes commerce an active strategic system. Caravans can establish lucrative trade routes and help construct wonders, while Marco Polo's Embassy opens broad diplomatic contact; the technology also leads to Banking and Medicine.

### University
<!-- requires: Mathematics, Philosophy; unlocks: University -->

University enables the University improvement, giving established science cities another major research multiplier. It also opens Chemistry, Economics, Metallurgy, and Theory of Gravity, making it a crucial midgame research hub.

### Warrior Code
<!-- requires: none, none; unlocks: Archers -->

Warrior Code gives your civilization an early offensive option through Archers and leads to both Iron Working and Feudalism. It is a strong starting technology for players who expect early conflict or want a fast military branch.

### Writing
<!-- requires: Alphabet, none; unlocks: Diplomat, Library -->

Writing turns knowledge and diplomacy into practical tools. Libraries accelerate science, Diplomats open espionage and negotiation tactics, and Writing leads directly to Literacy and the powerful research branches beyond it.

## Units (51)

### Settlers
<!-- land, attack 0 / defence 1, moves 1, hp 2, firepower 1, cost 40, requires none -->

Settlers are the foundation of expansion. They can found new cities and improve the land with roads, irrigation, mines, fortresses, pollution cleanup, and other terrain work, turning empty territory into a productive empire. Their main cost is strategic rather than military: producing one reduces the home city's population, so good timing matters.

### Engineers
<!-- land, attack 0 / defence 2, moves 2, hp 2, firepower 1, cost 40, requires Explosives -->

Engineers are the advanced replacement for basic Settlers as a terrain-development force. They move twice as fast, perform improvement work at double the rate, and can carry out the most advanced terrain engineering, allowing a mature civilization to reshape its economy and military infrastructure much more quickly.

### Warriors
<!-- land, attack 1 / defence 1, moves 1, hp 1, firepower 1, cost 10, requires none -->

Warriors are the cheapest basic fighting unit and require no technology. They are useful for protecting the first cities, escorting early Settlers, exploring dangerous territory, and providing an emergency garrison when production is scarce, but they should not be expected to stand against specialized troops for long.

### Phalanx
<!-- land, attack 1 / defence 2, moves 1, hp 1, firepower 1, cost 20, requires Bronze Working -->

The Phalanx is an inexpensive early defensive unit. Its stronger defense makes it a dependable city garrison and a useful escort for vulnerable civilian units, giving young civilizations a low-cost way to hold territory while more powerful offensive forces are developed.

### Archers
<!-- land, attack 3 / defence 2, moves 1, hp 1, firepower 1, cost 30, requires Warrior Code -->

Archers provide one of the first meaningful offensive upgrades over Warriors. Their solid attack and respectable defense make them flexible early troops for clearing barbarians, attacking weak cities, and reinforcing exposed frontiers without requiring a large investment.

### Legion
<!-- land, attack 4 / defence 2, moves 1, hp 1, firepower 1, cost 40, requires Iron Working -->

The Legion is a powerful ancient-era assault unit built for taking ground. Its high early attack strength makes it excellent for breaking primitive defenses and turning an Iron Working advantage into rapid conquest, though its ordinary movement means it benefits greatly from roads and careful staging.

### Pikemen
<!-- land, attack 1 / defence 2, moves 1, hp 1, firepower 1, cost 20, requires Feudalism, special: enhanced defense against mounted units -->

Pikemen are specialized defenders designed to blunt fast mounted attacks. They are cheap to build and receive a major defensive advantage against mounted opponents, making them especially valuable for protecting cities and narrow fronts when Horsemen, Knights, and similar units dominate the battlefield.

### Musketeers
<!-- land, attack 3 / defence 3, moves 1, hp 2, firepower 1, cost 30, requires Gunpowder -->

Musketeers are durable gunpowder-era infantry with balanced attack and defense and twice the staying power of early troops. They make excellent general-purpose garrisons and can survive battles that would quickly destroy ancient units, giving expanding armies a sturdy force for holding captured territory.

### Fanatics
<!-- land, attack 4 / defence 4, moves 1, hp 2, firepower 1, cost 20, requires Fundamentalism, special: free support under fundamentalism -->

Fanatics are unusually powerful for their low production cost, combining strong attack, strong defense, and good durability. Under Fundamentalism they require no normal unit support, allowing a Fundamentalist civilization to field very large armies without draining city production.

### Partisan
<!-- land, attack 4 / defence 4, moves 1, hp 2, firepower 1, cost 50, requires Guerilla Warfare, special: ignores enemy zones of control -->

Partisans are tough resistance troops that combine respectable attack and defense with the ability to ignore enemy zones of control. That freedom lets them slip through contested fronts, threaten roads and weak rear areas, reinforce resistance around captured territory, and force an invader to defend more than just the main battle line.

### Alpine Troops
<!-- land, attack 5 / defence 5, moves 1, hp 2, firepower 1, cost 50, requires Tactics, special: treats all terrain as roads for movement -->

Alpine Troops are elite infantry built for difficult terrain. They treat terrain as though it were connected by roads for movement, letting them cross mountains, hills, forests, and other rough country with extraordinary efficiency while still bringing strong attack and defense to the fight.

### Riflemen
<!-- land, attack 5 / defence 4, moves 1, hp 2, firepower 1, cost 40, requires Conscription -->

Riflemen are strong industrial-era infantry with good attack, defense, and durability at a moderate cost. They are excellent line troops for defending cities and holding conquered ground, and they remain useful anywhere a player needs dependable strength without paying for the most advanced mechanized units.

### Marines
<!-- land, attack 8 / defence 5, moves 1, hp 2, firepower 1, cost 60, requires Amphibious Warfare, special: can make amphibious assaults directly from ships -->

Marines are powerful assault infantry designed for invasions. Their amphibious capability lets them attack directly from ships, making them ideal for storming coastal cities and opening beachheads without first exposing a landing force on shore; their strong attack also makes them formidable conventional ground troops.

### Paratroopers
<!-- land, attack 6 / defence 4, moves 1, hp 2, firepower 1, cost 60, requires Combined Arms, special: can make paradrops -->

Paratroopers can leap over normal front lines by making paradrops into suitable land territory. Their ability to seize open tiles, cut transportation routes, reinforce distant positions, or appear behind enemy defenses makes them a strategic mobility unit rather than merely another infantry formation.

### Mech. Inf.
<!-- land, attack 6 / defence 6, moves 3, hp 3, firepower 1, cost 50, requires Labor Union -->

Mechanized Infantry combines excellent defense, good attack, exceptional durability, and three-square movement. It is one of the best units for protecting late-game cities, escorting Armor and Howitzers, and rapidly reinforcing threatened sectors without sacrificing combat strength.

### Horsemen
<!-- land, attack 2 / defence 1, moves 2, hp 1, firepower 1, cost 20, requires Horseback Riding -->

Horsemen are the first fast land combat unit, moving twice as far as ordinary infantry. They are useful for scouting, chasing barbarians, striking weak targets, and exploiting gaps before an opponent can react, making mobility their greatest advantage.

### Chariot
<!-- land, attack 3 / defence 1, moves 2, hp 1, firepower 1, cost 30, requires The Wheel -->

The Chariot improves on Horsemen with a stronger attack while retaining two-square movement. It is a useful early raider and city attacker, especially on road networks where its speed allows concentrated attacks and rapid redeployment.

### Elephants
<!-- land, attack 4 / defence 1, moves 2, hp 1, firepower 1, cost 40, requires Polytheism -->

Elephants are powerful mobile ancient troops with a strong attack and two-square movement. They are well suited to aggressive campaigns in which speed matters, providing a hard-hitting alternative to slower infantry before the arrival of medieval cavalry.

### Crusaders
<!-- land, attack 5 / defence 1, moves 2, hp 1, firepower 1, cost 40, requires Monotheism -->

Crusaders are fast medieval shock troops with excellent offensive strength for their era. Their combination of movement and attack makes them ideal for deep raids, rapid sieges, and concentrating force against cities before defenders can be reinforced.

### Knights
<!-- land, attack 4 / defence 2, moves 2, hp 1, firepower 1, cost 40, requires Chivalry -->

Knights trade some of the Crusader's raw offense for better defense while keeping high mobility. They are versatile mounted troops that can attack, survive counterattacks, escort advancing armies, and hold newly captured positions more reliably than lightly protected cavalry.

### Dragoons
<!-- land, attack 5 / defence 2, moves 2, hp 2, firepower 1, cost 50, requires Leadership -->

Dragoons bring mounted warfare into the gunpowder era. Two movement points, solid attack, and improved durability make them effective mobile reserves and raiders, able to reinforce a front quickly or exploit a breakthrough without being as fragile as earlier cavalry.

### Cavalry
<!-- land, attack 8 / defence 3, moves 2, hp 2, firepower 1, cost 60, requires Tactics -->

Cavalry is a fast, hard-hitting offensive unit with enough durability to sustain a campaign. Its eight-point attack and two-square movement make it excellent for breaking weakened defenses, overrunning exposed units, and turning tactical victories into rapid territorial gains.

### Armor
<!-- land, attack 10 / defence 5, moves 3, hp 3, firepower 1, cost 80, requires Mobile Warfare -->

Armor is the premier fast land assault unit of the modern era. With three-square movement, a very high attack value, strong defense, and excellent durability, it can smash through ordinary defenders and then exploit the breach before slower enemy forces can respond.

### Catapult
<!-- land, attack 6 / defence 1, moves 1, hp 1, firepower 1, cost 40, requires Mathematics -->

The Catapult is an early siege weapon with very high attack strength but poor defense. It is most effective when protected by infantry and used to attack cities or fortified enemies, giving ancient armies the concentrated striking power needed to crack positions that ordinary troops struggle to defeat.

### Cannon
<!-- land, attack 8 / defence 1, moves 1, hp 2, firepower 1, cost 40, requires Metallurgy -->

The Cannon is a stronger and more durable successor to the Catapult. Its high attack makes it a potent city-buster and battlefield offensive unit, but its weak defense still means it should advance behind protective troops rather than stand alone on exposed ground.

### Artillery
<!-- land, attack 10 / defence 1, moves 1, hp 2, firepower 2, cost 50, requires Machine Tools -->

Artillery delivers tremendous offensive power and two points of firepower, allowing successful hits to do much more damage than earlier siege weapons. It is devastating against strong defenders when properly screened, and it gives industrial armies an efficient way to reduce tough positions before mobile units advance.

### Howitzer
<!-- land, attack 12 / defence 2, moves 2, hp 3, firepower 2, cost 70, requires Robotics, special: attacks ignore city walls -->

The Howitzer is the ultimate ground siege unit: fast, extremely powerful, durable, and able to ignore the protection of City Walls when attacking. It is designed to break heavily fortified cities that would otherwise absorb enormous losses, making it one of the most important late-game conquest units.

### Fighter
<!-- air, attack 4 / defence 3, moves 10, hp 2, firepower 2, cost 60, air range 1, requires Flight, special: two-space visibility; can attack air units -->

The Fighter is a very fast air-superiority unit with extended visibility and the ability to attack enemy aircraft. It is valuable for intercepting hostile air power, scouting large areas, striking exposed targets, and protecting bombers and cities from enemy planes, though its limited fuel range requires careful basing.

### Bomber
<!-- air, attack 12 / defence 1, moves 8, hp 2, firepower 2, cost 120, air range 2, requires Advanced Flight, special: two-space visibility -->

The Bomber is a long-range heavy attack aircraft with enormous offensive strength and extended visibility. It can reach deep targets and devastate ground or naval units, but its weak defense means it depends on secure airbases, escorts, and careful target selection.

### Helicopter
<!-- air, attack 10 / defence 3, moves 6, hp 2, firepower 2, cost 100, requires Combined Arms, special: two-space visibility -->

The Helicopter is a highly mobile air assault unit with a strong attack and no normal fuel-turn limit. Its intended tradeoff is attrition when it remains away from a city, airbase, or carrier for too long, so it excels at rapid strikes, reaching otherwise inaccessible targets, and supporting fast offensives before returning to safety.

### Stealth Fighter
<!-- air, attack 8 / defence 4, moves 14, hp 2, firepower 2, cost 80, air range 1, requires Stealth, special: two-space visibility; can attack air units -->

The Stealth Fighter is an advanced air-superiority aircraft with exceptional speed, improved combat strength, extended visibility, and the ability to attack other aircraft. It can cover enormous distances in a turn and is ideal for controlling the skies while also delivering precise attacks against vulnerable targets.

### Stealth Bomber
<!-- air, attack 14 / defence 5, moves 12, hp 2, firepower 2, cost 160, air range 2, requires Stealth, special: two-space visibility -->

The Stealth Bomber is the strongest conventional air-strike unit in the roster. Its enormous attack, strong defense for a bomber, long movement, and extended visibility let it project force deep into enemy territory and destroy high-value targets while surviving threats that would endanger older bombers.

### Trireme
<!-- sea, attack 1 / defence 1, moves 3, hp 1, firepower 1, cost 40, carries 2 units, requires Map Making, special: must remain near land -->

The Trireme is the first naval transport and opens coastal exploration and overseas expansion. It can carry two units, but it must remain near land, so early voyages require careful island-hopping and route planning rather than unrestricted ocean travel.

### Caravel
<!-- sea, attack 2 / defence 1, moves 3, hp 1, firepower 1, cost 40, carries 3 units, requires Navigation -->

The Caravel is a small ocean-going transport that carries three units and is no longer restricted to hugging the coast. It is ideal for exploration, colonization, diplomatic missions, and modest invasion forces during the age of sail.

### Galleon
<!-- sea, attack 0 / defence 2, moves 4, hp 2, firepower 1, cost 40, carries 4 units, requires Magnetism -->

The Galleon is a dedicated sailing transport with room for four units and better durability than earlier transports. It lets players move meaningful armies across the sea, making sustained colonization and amphibious campaigns much easier to organize.

### Frigate
<!-- sea, attack 4 / defence 2, moves 4, hp 2, firepower 1, cost 50, carries 2 units, requires Magnetism -->

The Frigate is a fast armed sailing ship that combines respectable naval attack with space for two passengers. It can escort transports, hunt weaker ships, bombard coastal targets, and still carry a small landing or diplomatic force, giving it unusual flexibility.

### Ironclad
<!-- sea, attack 4 / defence 4, moves 4, hp 3, firepower 1, cost 60, requires Steam Engine -->

The Ironclad is an early armored warship with strong defense and greatly improved durability. It is an effective coastal combatant for destroying wooden fleets and protecting sea lanes during the transition from sail to modern naval warfare.

### Destroyer
<!-- sea, attack 4 / defence 4, moves 6, hp 3, firepower 1, cost 60, requires Electricity, special: two-space visibility -->

The Destroyer is the fastest conventional surface warship in the roster and has extended visibility. Its speed makes it excellent for reconnaissance, escort duty, chasing damaged ships, screening invasion fleets, and reacting quickly to naval threats across a wide area.

### Cruiser
<!-- sea, attack 6 / defence 6, moves 5, hp 3, firepower 2, cost 80, requires Steel, special: two-space visibility -->

The Cruiser is a balanced modern warship with strong attack, defense, durability, firepower, and extended visibility. It is a dependable fleet centerpiece that can defeat lighter vessels, protect transports, and operate aggressively without the enormous production cost of a Battleship.

### AEGIS Cruiser
<!-- sea, attack 8 / defence 8, moves 5, hp 3, firepower 2, cost 100, requires Rocketry, special: two-space visibility; can attack air units; enhanced defense against air units -->

The AEGIS Cruiser is an advanced fleet-defense ship with powerful surface combat statistics, extended visibility, the ability to attack air units, and enhanced defense against air attack. It is especially valuable as an escort for Carriers and Transports when enemy aircraft and missiles threaten a fleet.

### Battleship
<!-- sea, attack 12 / defence 12, moves 4, hp 4, firepower 2, cost 160, requires Automobile, special: two-space visibility -->

The Battleship is the heaviest conventional surface combatant in rhYciv. Its massive attack and defense, four hit points, two firepower, and extended visibility make it ideal for winning major naval battles and supporting coastal offensives, although its high cost makes every loss significant.

### Submarine
<!-- sea, attack 10 / defence 2, moves 3, hp 3, firepower 2, cost 60, carries 8 missile payload, requires Combustion, special: two-space visibility; submarine advantages and restrictions -->

The Submarine is a stealth-oriented naval attacker with very high attack, low defense, extended visibility, and the special advantages and restrictions of submarine warfare. Its large hold is intended for carrying missile weapons, allowing it to approach from the sea and deliver devastating surprise strikes while avoiding a conventional surface battle whenever possible.

### Carrier
<!-- sea, attack 1 / defence 9, moves 5, hp 4, firepower 2, cost 160, carries 8 air units, requires Advanced Flight, special: two-space visibility; can carry air units -->

The Carrier is a mobile airbase at sea. It can carry up to eight air units, has excellent durability and extended visibility, and allows Fighters and Bombers to operate far from friendly cities, making it the foundation of long-range naval air power and overseas invasion support.

### Transport
<!-- sea, attack 0 / defence 3, moves 5, hp 3, firepower 1, cost 50, carries 8 units, requires Industrialization, special: two-space visibility -->

The Transport is the main modern troop carrier, able to move up to eight units at five squares per turn while providing extended visibility. Its own combat ability is minimal, so its value comes from delivering large armies across oceans quickly and safely under the protection of escorting warships.

### Cruise Missile
<!-- air, attack 18 / defence 0, moves 12, hp 1, firepower 3, cost 60, air range 1, requires Rocketry, special: destroyed after attacking -->

The Cruise Missile is a one-use precision strike weapon with extreme attack strength, high speed, and heavy firepower. It is destroyed after attacking, so it is best reserved for eliminating particularly dangerous defenders, ships, or other high-value targets that justify trading the missile for the kill.

### Nuclear
<!-- air, attack 99 / defence 0, moves 16, hp 1, firepower 1, cost 160, air range 1, requires Rocketry, special: destroyed after attacking -->

The Nuclear unit is rhYciv's strategic weapon of mass destruction. Once nuclear weapons are enabled by the Manhattan Project and the required technology is available, it can devastate a target area, destroy military forces, cripple cities, and leave serious pollution behind; its extraordinary power is balanced by its one-use nature and severe diplomatic consequences.

### Diplomat
<!-- land, attack 0 / defence 0, moves 2, hp 1, firepower 1, cost 30, requires Writing, special: ignores enemy zones of control -->

Diplomats are non-combat agents that ignore enemy zones of control and perform political operations instead of fighting. Their intended missions include establishing embassies, investigating enemy cities, stealing technology, sabotaging production, inciting revolts, and bribing vulnerable units, making information and subversion alternatives to direct conquest.

### Spy
<!-- land, attack 0 / defence 0, moves 3, hp 1, firepower 1, cost 30, requires Espionage, special: two-space visibility; ignores enemy zones of control -->

The Spy is the faster and more capable successor to the Diplomat. Spies ignore zones of control, have extended visibility, and are intended to perform the full range of espionage missions with better effectiveness and survivability, giving an advanced civilization a powerful tool for intelligence, sabotage, theft, and covert influence.

### Caravan
<!-- land, attack 0 / defence 1, moves 1, hp 1, firepower 1, cost 50, requires Trade, special: ignores enemy zones of control -->

Caravans are economic support units that ignore enemy zones of control. They can establish trade routes for long-term commerce and immediate rewards, deliver valuable commodities to distant cities, and contribute their production toward Wonders, allowing a trade-focused civilization to turn mobility into both wealth and accelerated construction.

### Freight
<!-- land, attack 0 / defence 1, moves 2, hp 1, firepower 1, cost 50, requires The Corporation, special: ignores enemy zones of control -->

Freight is the faster modern successor to the Caravan. It performs the same trade-route, commodity-delivery, and Wonder-support functions but moves twice as fast, making long-distance commerce easier to manage and allowing economic support to reach major projects or foreign markets much sooner.

### Explorer
<!-- land, attack 0 / defence 1, moves 1, hp 1, firepower 1, cost 30, requires Seafaring, special: ignores enemy zones of control -->

The Explorer is a dedicated reconnaissance unit that cannot attack but can ignore enemy zones of control. It is useful for revealing unknown territory, reaching tribal villages and remote objectives, scouting routes for later armies, and slipping through contested regions without being pinned by normal front-line control rules.

## City Improvements (39)

### Nothing  *(placeholder — skip)*

### Palace
<!-- cost 100, upkeep 0, requires Masonry -->

The Palace establishes the city as the capital of your civilization. It eliminates corruption and waste in the capital and acts as the center from which distance-based corruption and waste are measured throughout the empire. Under Democracy it also contributes to citizen happiness. A well-placed Palace can dramatically improve the efficiency of an empire whose productive cities would otherwise be far from the seat of government.

### Barracks
<!-- cost 40, upkeep 1, requires none -->

Barracks turn a city into a dependable military training center. New land combat units produced there begin as Veterans, making them stronger and more likely to survive combat. Barracks also provide the intended rapid-restoration function for land units: a damaged ground unit that spends a full turn resting in a city with Barracks can be restored much more quickly. They are especially valuable in high-production cities that continuously supply the front.

### Granary
<!-- cost 60, upkeep 1, requires Pottery -->

A Granary preserves half of a city's accumulated food whenever the city grows. Instead of beginning each new population level with an empty food box, the city keeps a substantial reserve and can grow again much sooner. Granaries are therefore among the best early investments for cities with strong food production or for settlements you want to turn into major population centers.

### Temple
<!-- cost 40, upkeep 1, requires Ceremonial Burial -->

A Temple reduces local unhappiness, normally turning one unhappy citizen content. After Mysticism its effect becomes stronger, and the Oracle can further amplify it while that Wonder remains active. Temples are inexpensive tools for keeping early cities productive, preventing disorder, and supporting population growth before more powerful happiness improvements become available.

### Marketplace
<!-- cost 80, upkeep 1, requires Currency -->

A Marketplace increases both Tax and Luxury output by 50 percent. The bonus compounds with later financial improvements such as Banks and Stock Exchanges, so a Marketplace is the foundation of a wealthy commercial city. It is especially useful in cities producing substantial trade, where the same improvement can simultaneously increase treasury income and help keep citizens happy through additional Luxuries.

### Library
<!-- cost 80, upkeep 1, requires Writing -->

A Library increases the city's Science output by 50 percent. Its bonus stacks with Universities and Research Labs, allowing a strong trade city to become a major research center. Building Libraries early in your best commercial cities can accelerate the entire technology tree and help create a lasting military and economic advantage.

### Courthouse
<!-- cost 80, upkeep 1, requires Code of Laws -->

A Courthouse reduces corruption and waste, allowing a distant city to keep more of the trade and shields it produces. It also makes the city more resistant to hostile bribery and, under Democracy, contributes an additional happiness benefit. Courthouses are particularly useful in large empires where productive frontier cities lie far from the capital.

### City Walls
<!-- cost 80, upkeep 0, requires Masonry -->

City Walls make a settlement dramatically harder to capture by multiplying the defense of units inside it against conventional ground attacks. They are one of the strongest defensive investments in the game because they have no maintenance cost and can turn even modest garrisons into formidable obstacles. Specialized weapons such as Howitzers are intended to ignore this protection, so Walls are powerful but not absolute.

### Aqueduct
<!-- cost 80, upkeep 2, requires Construction -->

An Aqueduct allows a city to grow beyond size 8. Without one, a well-fed city eventually reaches its population ceiling no matter how much surplus food it produces. Build Aqueducts in cities with enough food and happiness to benefit from continued growth; small or food-poor settlements can usually postpone the expense.

### Bank
<!-- cost 120, upkeep 3, requires Banking -->

A Bank adds another 50 percent to the city's Tax and Luxury output, cumulative with the Marketplace. In a major trade city this can produce a large increase in both government revenue and happiness-generating Luxuries. Banks are most efficient where the underlying trade output is already high enough to justify their greater construction and maintenance costs.

### Cathedral
<!-- cost 120, upkeep 3, requires Monotheism -->

A Cathedral is a major happiness improvement, normally reducing unhappiness by three citizens. Theology increases its effectiveness by one additional citizen, while Communism reduces it by one. Cathedrals are excellent for stabilizing large cities, supporting high tax or science rates with fewer Luxuries, and keeping heavily populated industrial centers out of disorder.

### University
<!-- cost 160, upkeep 3, requires University -->

A University increases Science output by another 50 percent, cumulative with a Library. In a strong research city, the combination turns trade into technology at a much faster rate and helps your civilization reach advanced military, industrial, and economic discoveries before its rivals. Universities are especially effective in cities already producing significant trade.

### Mass Transit
<!-- cost 160, upkeep 4, requires Mass Production -->

Mass Transit eliminates pollution caused by population. This lets very large cities continue growing without their sheer size adding to the pollution problem, leaving only industrial sources to manage. It is particularly valuable in dense late-game metropolitan areas where population pollution would otherwise consume worker time and threaten the environment.

### Colosseum
<!-- cost 100, upkeep 4, requires Construction -->

A Colosseum reduces unhappiness by three citizens, making it one of the strongest general-purpose happiness buildings available before the modern era. After Electronics, its effect improves to four citizens. Its maintenance is substantial, so it is best used in large cities that genuinely need the extra stability rather than automatically built everywhere.

### Factory
<!-- cost 200, upkeep 4, requires Industrialization -->

A Factory increases the city's shield production by 50 percent, greatly accelerating the construction of units, improvements, and Wonders. It is the foundation of the industrial production chain and enables the strongest later production infrastructure. The drawback is heavier industrial pollution, so highly productive factory cities eventually benefit from cleaner power and pollution-control improvements.

### Mfg. Plant
<!-- cost 320, upkeep 6, requires Robotics -->

A Manufacturing Plant adds another major production bonus on top of a Factory, turning an established industrial city into a late-game production powerhouse. It is expensive to build and maintain, but in the right city it can shorten construction times for advanced military units and major projects dramatically. Its value is greatest where strong base shield production already exists.

### SDI Defense
<!-- cost 200, upkeep 4, requires Laser -->

SDI Defense protects the city and its surrounding area from nuclear attack. In a nuclear era, it can preserve a critical production center, capital, or troop concentration that would otherwise be vulnerable to a single devastating strike. Because its benefit is defensive and specialized, it is most important in cities that an enemy is likely to target with nuclear weapons.

### Recycling Center
<!-- cost 200, upkeep 2, requires Recycling -->

A Recycling Center cuts industrial pollution from shield production to roughly one third of its normal level. This allows a city to maintain heavy industrial output while creating far fewer polluted tiles, reducing the amount of Settler or Engineer labor needed for cleanup. It is an efficient environmental upgrade for mature Factory and Manufacturing Plant cities.

### Power Plant
<!-- cost 160, upkeep 4, requires Refining -->

A Power Plant supplies additional power to an industrial city, providing the power-plant production bonus when used with a Factory or Manufacturing Plant. It is the earliest conventional way to push industrial output beyond Factory levels, but unlike cleaner alternatives it does not reduce production pollution. It is useful when raw production matters more than environmental efficiency or before better power plants become available.

### Hydro Plant
<!-- cost 240, upkeep 4, requires Electronics -->

A Hydro Plant provides the same major industrial power bonus as other advanced power sources while cutting pollution from shield production roughly in half. It combines high output with good environmental performance and has no meltdown risk, making it one of the safest ways to power a major industrial city. The Hoover Dam provides the equivalent benefit civilization-wide.

### Nuclear Plant
<!-- cost 160, upkeep 2, requires Nuclear Power -->

A Nuclear Plant provides the industrial power bonus at a relatively low maintenance cost while also reducing production pollution to about half its normal level. Its great drawback is the intended risk of a catastrophic reactor meltdown if the city falls into disorder; discovering Fusion Power removes that danger. Used in stable cities, Nuclear Plants offer an efficient combination of production and pollution control.

### Stock Exchange
<!-- cost 160, upkeep 4, requires Economics -->

A Stock Exchange adds another 50 percent to Tax and Luxury output, stacking with the Marketplace and Bank. In a top-tier commercial city, the complete financial chain can generate enormous revenue or Luxury output from the same underlying trade. Stock Exchanges are expensive, so they are most profitable in cities that already produce abundant trade.

### Sewer System
<!-- cost 120, upkeep 2, requires Sanitation -->

A Sewer System allows a city to grow beyond size 12. It is the late-game counterpart to the Aqueduct and becomes essential for turning prosperous cities into true metropolises. Because larger populations demand more food and happiness management, it is best built where the city has enough resources and infrastructure to support continued growth.

### Supermarket
<!-- cost 120, upkeep 3, requires Refrigeration -->

A Supermarket increases the food gained from improved agricultural land, allowing irrigated and farmland tiles to support much larger populations. It is most valuable in cities with many productive farm tiles, where the extra food can drive metropolitan growth, support more specialists, or free citizens to work high-shield and high-trade terrain instead.

### Super Highways
<!-- cost 160, upkeep 3, requires Automobile -->

Super Highways increase trade from worked land tiles connected by roads or railroads, with the intended classic effect providing 50 percent more trade from those developed squares. They also strengthen the value of major trade connections. In a mature commercial city with a dense road network, Super Highways can produce a large jump in Taxes, Luxuries, and Science all at once.

### Research Lab
<!-- cost 160, upkeep 3, requires Computers -->

A Research Lab adds another 50 percent to Science output, cumulative with Libraries and Universities. It is the final major city-level research multiplier and can turn a well-developed trade city into an exceptional scientific center. Research Labs are ideal when you are racing toward late-game military and strategic technologies.

### SAM Battery
<!-- cost 100, upkeep 2, requires Rocketry -->

A SAM Battery doubles the defensive strength of units in the city against conventional air attacks. It is particularly useful for protecting valuable industrial cities, capitals, and coastal bases from Fighters, Bombers, and Stealth aircraft. Nuclear weapons are handled by SDI Defense instead, so advanced cities may need both forms of protection.

### Coastal Defense
<!-- cost 80, upkeep 1, requires Metallurgy -->

Coastal Defense doubles the defensive strength of units in the city against attacks from naval units. It is inexpensive insurance for important ports and exposed coastal cities, making bombardment and seaborne assaults much more costly for an enemy. Because it requires access to the ocean, inland cities have no need for it.

### Solar Plant
<!-- cost 320, upkeep 4, requires Environmentalism -->

A Solar Plant combines the industrial power bonus with the cleanest production system available. It eliminates pollution generated by industrial shield production and also reduces the population-related pollution pressure that contributes to global environmental damage. Solar Plants are expensive, but they let heavily industrialized late-game cities maintain maximum output with minimal environmental cost.

### Harbor
<!-- cost 60, upkeep 1, requires Seafaring -->

A Harbor adds one Food to each worked ocean tile in the city's radius. This transforms coastal water from a trade-heavy but food-poor resource into a much stronger source of population support. Harbors are often essential for island cities and coastal settlements that work many ocean tiles, allowing them to grow without abandoning their valuable maritime trade.

### Offshore Platform
<!-- cost 160, upkeep 3, requires Miniaturization -->

An Offshore Platform adds one Shield to each worked ocean tile in the city's radius. Coastal cities that previously relied on land for nearly all of their production can turn the sea into a meaningful industrial resource while retaining its trade value. The improvement is especially powerful in island cities with many ocean tiles and little productive land.

### Airport
<!-- cost 160, upkeep 3, requires Radio -->

An Airport turns a city into an air-power and rapid-transport hub. New air units produced there begin as Veterans, damaged aircraft can be completely restored after spending a full turn in the city, and the intended airlift system allows one unit per turn to move rapidly between cities equipped with Airports. Airports are invaluable for reinforcing distant fronts and sustaining large air forces.

### Police Station
<!-- cost 60, upkeep 2, requires Communism -->

A Police Station reduces the unhappiness caused by military units operating away from their home city. Under Republic it can eliminate the remaining field-unit happiness penalty, while under Democracy it cuts the normal penalty substantially. This lets peaceful governments sustain larger overseas or offensive armies without forcing their home cities into disorder.

### Port Facility
<!-- cost 80, upkeep 3, requires Amphibious Warfare -->

A Port Facility turns a coastal city into a first-class naval base. New sea units produced there begin as Veterans, and damaged ships that spend a full turn in the city can be completely repaired. Port Facilities are ideal in forward naval bases and high-production coastal cities that need to build, restore, and redeploy fleets quickly.

### Unused Structural  *(placeholder — skip)*

### Unused Component  *(placeholder — skip)*

### Unused Module  *(placeholder — skip)*

### Capitalization
<!-- cost 999, upkeep 0, requires The Corporation -->

Capitalization is not a permanent building. When selected as a city's production, it converts the city's shield output into additional treasury income instead of accumulating construction progress. It is useful in mature cities that have nothing urgent to build, allowing excess industrial capacity to strengthen the civilization's finances rather than sit idle.

## Wonders of the World (28)

### Pyramids
<!-- cost 200, requires Masonry, obsolete never -->

The Pyramids give every city in your civilization the benefits of a Granary. Cities retain half of their stored food after population growth, allowing them to recover and grow again much faster. This is one of the strongest early expansion wonders because its benefit applies across your entire empire and remains useful for the rest of the game.

### Hanging Gardens
<!-- cost 200, requires Pottery, obsolete railroad -->

The Hanging Gardens make your people dramatically happier during the ancient and medieval eras. The city containing the wonder receives a major happiness boost, while every other city in your civilization receives a smaller one. It is especially valuable for supporting rapid early growth, reducing disorder, and allowing a larger empire to remain productive before modern happiness improvements become widely available.

### Colossus
<!-- cost 200, requires Bronze Working, obsolete flight -->

The Colossus transforms its home city into a commercial powerhouse. Every worked tile in that city that already produces trade generates one additional trade arrow. Because that extra trade can become taxes, luxuries, or science, the Colossus is particularly powerful in a large coastal city with strong trade terrain and research improvements.

### Lighthouse
<!-- cost 200, requires Map Making, obsolete magnetism -->

The Lighthouse gives your civilization a decisive early advantage at sea. Naval units move one extra square each turn, newly built ships are veteran, and primitive vessels can cross dangerous open water without the normal risk of being lost. It can enable exploration, colonization, and surprise naval warfare long before rival civilizations are ready to operate safely across the oceans.

### Great Library
<!-- cost 300, requires Literacy, obsolete electricity -->

The Great Library helps a civilization keep pace with the world's scientific leaders. Whenever at least two other civilizations know a technology that you do not, the Great Library grants that advance to you automatically. It is an excellent wonder for a civilization that is expanding, waging war, or spending less on research, because it can fill important gaps in your technology tree at no research cost.

### Oracle
<!-- cost 300, requires Mysticism, obsolete theology -->

The Oracle doubles the happiness effect of Temples throughout your civilization. This makes inexpensive Temples far more effective at controlling unrest, allowing cities to grow larger and remain productive with less reliance on entertainers or luxury spending. Its value is greatest before Cathedrals and other later happiness tools become common.

### Great Wall
<!-- cost 300, requires Masonry, obsolete metallurgy -->

The Great Wall protects every city in your civilization as though it had City Walls, providing a major defensive advantage against land attacks. It also strengthens your forces against barbarian threats. For an expanding empire surrounded by rivals, the Great Wall can save enormous production that would otherwise be spent fortifying individual cities.

### Sun Tzu's War Academy
<!-- cost 300, requires Feudalism, obsolete mobile warfare -->

Sun Tzu's War Academy makes every newly produced ground combat unit in your civilization a veteran. Veteran units fight more effectively and are more likely to survive long campaigns, making this wonder enormously valuable for conquest. Its empire-wide military benefit effectively gives every city the most important advantage of a Barracks for land forces.

### King Richard's Crusade
<!-- cost 300, requires Engineering, obsolete industrialization -->

King Richard's Crusade turns its home city into an extraordinary production center. Every worked tile in the wonder city produces one additional shield. In a large, well-developed city this can dramatically accelerate the construction of military units, improvements, and later wonders, making the city one of the industrial centers of the world until modern industry renders the effect obsolete.

### Marco Polo's Embassy
<!-- cost 200, requires Trade, obsolete communism -->

Marco Polo's Embassy establishes diplomatic contact and embassies with every other civilization. You gain immediate access to diplomatic relations, intelligence, negotiation, and technology trading without first sending diplomats across the map. Built early, it can reveal the political shape of the world and create opportunities for alliances, trade, tribute, and carefully chosen wars.

### Michelangelo's Chapel
<!-- cost 400, requires Monotheism, obsolete never -->

Michelangelo's Chapel provides the equivalent happiness effect of a Cathedral in every city you control. Large empires benefit enormously because cities can remain orderly and productive without each one having to build and maintain its own Cathedral. The wonder remains useful throughout the game and is especially powerful for civilizations pursuing rapid population growth or sustained warfare.

### Copernicus' Observatory
<!-- cost 300, requires Astronomy, obsolete never -->

Copernicus' Observatory increases science output in its home city by 50 percent. Place it in your strongest research city—ideally one with high trade, a Library, a University, and later a Research Lab—to create a scientific center capable of producing a disproportionate share of your civilization's research.

### Magellan's Expedition
<!-- cost 400, requires Navigation, obsolete never -->

Magellan's Expedition increases the movement of every naval unit in your civilization by two squares per turn. Faster ships explore more efficiently, reinforce distant fronts sooner, escort transports more safely, and give your navy greater tactical reach. For civilizations dependent on overseas conquest or intercontinental transport, this is one of the most powerful strategic mobility wonders in the game.

### Shakespeare's Theatre
<!-- cost 300, requires Medicine, obsolete never -->

Shakespeare's Theatre eliminates unhappiness in its home city. Even a very large metropolis can remain productive without entertainers or heavy luxury spending, making the wonder city an ideal location for intense industrial production, scientific development, or support of a large overseas military under governments that normally suffer war weariness.

### Leonardo's Workshop
<!-- cost 400, requires Invention, obsolete automobile -->

Leonardo's Workshop continuously modernizes your armed forces. As new military technologies become available, obsolete units are upgraded toward newer equivalent unit types without the normal replacement cost. This can preserve the value of a large veteran army and save vast amounts of production during the transition from medieval to industrial warfare.

### J.S. Bach's Cathedral
<!-- cost 400, requires Theology, obsolete never -->

J.S. Bach's Cathedral reduces unhappiness in your cities, making two unhappy citizens content in cities connected to the wonder's regional sphere of influence. It is especially useful for large empires and wartime governments, where military deployments and empire size can otherwise produce widespread unrest.

### Isaac Newton's College
<!-- cost 400, requires Theory of Gravity, obsolete never -->

Isaac Newton's College doubles science output in its home city. Combined with Copernicus' Observatory and normal research improvements, it can create an exceptional scientific capital that races through the technology tree. Building multiple science multipliers in the same high-trade city produces some of the strongest research output available in rhYciv.

### A.Smith's Trading Co.
<!-- cost 400, requires Economics, obsolete never -->

Adam Smith's Trading Company pays the maintenance cost of every city improvement in your civilization that normally costs one gold per turn. Across a large empire this can save a tremendous amount of money each turn, freeing your treasury for rush-building, diplomacy, taxes, or a higher science rate.

### Darwin's Voyage
<!-- cost 300, requires Railroad, obsolete never -->

Darwin's Voyage immediately grants two free technological advances when the wonder is completed. The effect is instantaneous, making careful timing important: building it when expensive or strategically crucial technologies are available can produce a dramatic leap forward and may unlock powerful units, governments, or improvements several turns ahead of your rivals.

### Statue of Liberty
<!-- cost 400, requires Democracy, obsolete never -->

The Statue of Liberty gives your civilization extraordinary political flexibility. You may adopt any form of government regardless of whether you have researched its normal prerequisite, and changes of government can be made with greatly reduced disruption. This allows you to switch between economic, democratic, and wartime governments as circumstances demand.

### Eiffel Tower
<!-- cost 300, requires Steam Engine, obsolete never -->

The Eiffel Tower improves the international reputation and diplomatic standing of your civilization. Rival leaders view your nation more favorably, making peaceful relations, negotiations, and diplomatic recovery easier. It is most valuable to players who rely heavily on alliances, treaties, technology exchange, and manipulation of the balance of power between competing civilizations.

### Hoover Dam
<!-- cost 600, requires Electronics, obsolete never -->

Hoover Dam provides the production benefit of a Hydro Plant to every city in your civilization. Cities with Factories receive a major industrial boost without having to construct individual power plants, and the hydroelectric effect avoids the pollution associated with conventional power generation. In a large industrial empire, Hoover Dam can add an enormous amount of total shield production.

### Women's Suffrage
<!-- cost 600, requires Industrialization, obsolete never -->

Women's Suffrage reduces the unhappiness caused by military units operating away from home under representative governments. Republics and Democracies can therefore wage larger and longer wars without suffering as much civil disorder. It is an especially important wonder for players who want the economic strength of democratic government without abandoning overseas military campaigns.

### Manhattan Project
<!-- cost 600, requires Nuclear Fission, obsolete never -->

The Manhattan Project unlocks the nuclear age. Once completed, civilizations with the necessary technology can build nuclear weapons. The wonder changes the strategic balance of the entire world rather than benefiting only its builder: nuclear deterrence, preemptive strikes, and the threat of catastrophic retaliation immediately become central considerations in diplomacy and warfare.

### United Nations
<!-- cost 600, requires Communism, obsolete never -->

The United Nations gives its owner exceptional diplomatic influence. It makes negotiations and peaceful settlements easier, strengthens your ability to manage international crises, and can help prevent unwanted wars from spiraling out of control. It is particularly valuable to a civilization trying to choose its wars carefully while maintaining favorable relations with the rest of the world.

### Apollo Program
<!-- cost 600, requires Space Flight, obsolete never -->

The Apollo Program reveals the world on a global scale, exposing the geography of the planet and the locations of rival cities. In traditional rules it also opens the path to space-race construction; rhYciv's streamlined conquest-focused game does not rely on spaceship victory, but the intelligence value of global reconnaissance remains strategically important for planning the final stages of a world war.

### SETI Program
<!-- cost 600, requires Computers, obsolete never -->

The SETI Program provides the science benefit of a Research Lab in every city in your civilization. This empire-wide research multiplier can produce a huge increase in total science output, especially in a large and highly developed civilization, and helps propel your technology rate through the late game.

### Cure for Cancer
<!-- cost 600, requires Genetic Engineering, obsolete never -->

The Cure for Cancer makes one additional citizen happy in every city in your civilization. Because the effect is empire-wide and permanent, it is especially valuable in a large late-game empire where population, war weariness, and the sheer number of cities make happiness increasingly difficult to manage.

## Terrain (33)

### Desert
<!-- food 0, shields 1, trade 0 -->

Desert is poor city land in its natural state, but it is far from useless. Irrigation can make it productive enough to support population, while mining can increase its shield output. Desert is particularly valuable when it contains an Oasis or Oil resource. Because movement is inexpensive and defensive protection is modest, deserts also tend to become open maneuver corridors during war.

### Plains
<!-- food 1, shields 1, trade 0 -->

Plains are balanced, dependable settlement terrain. Their combination of food and shields makes them useful immediately, and irrigation turns them into strong population-supporting tiles. A city surrounded by Plains can usually grow while still maintaining respectable production, especially when Buffalo or Wheat is present.

### Grassland
<!-- food 2, shields 0, trade 0 -->

Grassland is one of the best foundations for city growth. Its strong natural food output supports large populations with little preparation, allowing citizens to work less fertile but more productive tiles elsewhere. Irrigation pushes that advantage even further. Grassland is usually excellent territory for early cities and dense settlement networks.

### Forest
<!-- food 1, shields 2, trade 0 -->

Forest trades speed and food for production and protection. It naturally produces two shields and grants a stronger defensive position than open ground, making forested regions useful for both industry and warfare. Clearing or transforming forest into Grassland can support growth later, but productive forest tiles—especially those containing Silk or Pheasant—are often worth preserving.

### Hills
<!-- food 1, shields 0, trade 0 -->

Hills are premier mining territory and strong defensive ground. A mined Hill becomes a major source of shields, making hilly city sites excellent industrial centers once workers have improved the surrounding land. Their high defensive value also makes Hills useful for forts, border positions, and controlling approaches to important cities.

### Mountains
<!-- food 0, shields 1, trade 0 -->

Mountains are difficult to cross but provide the strongest natural defensive terrain on land. Their ordinary economic yield is weak, but Gold and Iron can make individual mountain tiles extremely valuable. Mountains are ideal defensive anchors and natural barriers; controlling mountain passes can compensate for a smaller army by forcing an enemy to attack at a severe disadvantage.

### Tundra
<!-- food 1, shields 0, trade 0 -->

Tundra is marginal land, but it can support useful frontier settlements when strategically located or blessed with special resources. Irrigation improves its food supply, while Game and Furs can make northern territories surprisingly productive. Tundra cities are often valuable for claiming territory, controlling passages, and exploiting nearby resources rather than for raw early growth.

### Glacier
<!-- food 0, shields 0, trade 0 -->

Glacier is among the least productive terrain in its ordinary form. Its main value comes from special resources, strategic geography, and eventual transformation. Ivory can provide valuable trade, while Icy Oil supplies substantial production. A remote Glacier tile may therefore be economically worthwhile despite otherwise hostile surroundings.

### Swamp
<!-- food 1, shields 0, trade 0 -->

Swamp slows movement and provides moderate defensive cover, but its normal economic value is poor. Its special resources are much more attractive: Peat supplies excellent production while Spice combines food and trade. Transforming ordinary Swamp into Plains can dramatically improve a city’s long-term economy.

### Jungle
<!-- food 1, shields 0, trade 0 -->

Jungle is difficult frontier terrain with modest food output and useful defensive cover. Unimproved Jungle is rarely ideal for a city core, but Gems and Fruit can make individual tiles outstanding. Clearing or transforming Jungle can convert a difficult region into productive Plains once sufficient worker capacity is available.

### Ocean
<!-- food 1, shields 0, trade 2 -->

Ocean is the economic foundation of coastal cities. It naturally produces trade and can provide substantial food once a Harbor is built, while an Offshore Platform later adds shield production to worked sea tiles. Fish and Whales make coastal settlements even stronger. Ocean also forms the strategic highway for naval exploration, trade, amphibious warfare, and overseas conquest.

### Oasis
<!-- food 3, shields 1, trade 0 -->

An Oasis turns otherwise harsh Desert into an excellent food tile. Its three Food allow a desert city to grow rapidly without requiring immediate irrigation, while the additional Shield preserves some production. Oasis tiles are prime locations around which to establish settlements in arid regions.

### Buffalo
<!-- food 1, shields 3, trade 0 -->

Buffalo transforms ordinary Plains into a powerful early-production tile. The three Shields make it excellent for rapidly building settlers, military units, improvements, and Wonders while still contributing enough Food to support the citizen working it.

### Resources
<!-- food 2, shields 1, trade 0 -->

A Resources tile preserves Grassland’s strong two-Food output while adding a Shield. This makes it one of the most balanced early-game tiles: it supports population growth without sacrificing production and is especially valuable around a young capital.

### Pheasant
<!-- food 3, shields 2, trade 0 -->

Pheasant is an exceptionally strong Forest resource, combining three Food with two Shields. It supports rapid growth and industry simultaneously, making it one of the best all-around tiles available before major infrastructure is built.

### Coal
<!-- food 1, shields 2, trade 0 -->

Coal provides useful production on already defensible Hills. Once surrounding infrastructure is developed, Coal-rich regions become natural industrial centers and strategic objectives worth protecting from rivals.

### Gold
<!-- food 0, shields 1, trade 6 -->

Gold is one of the strongest commerce resources in the game. Although the tile feeds no population, its enormous six Trade can fuel taxes, science, or luxuries. A city able to support citizens working nearby Gold can gain a major technological or financial advantage.

### Game
<!-- food 3, shields 1, trade 0 -->

Game makes otherwise marginal Tundra highly livable. Three Food and a Shield can sustain frontier population growth and give northern cities a solid economic anchor, often turning territory that would otherwise be ignored into worthwhile settlement land.

### Ivory
<!-- food 1, shields 1, trade 4 -->

Ivory makes frozen territory economically significant. Its combination of Food, Shield, and four Trade gives a Glacier city a valuable high-commerce tile and can justify settlement in remote polar regions.

### Peat
<!-- food 1, shields 4, trade 0 -->

Peat is a major production resource. Four Shields make a Peat tile excellent for military production, infrastructure, and Wonders, particularly in cities that can obtain sufficient food from surrounding terrain to support the worker assigned to it.

### Gems
<!-- food 1, shields 0, trade 4 -->

Gems provide a powerful commerce source in otherwise difficult Jungle terrain. Four Trade can significantly accelerate research or increase tax income, making Gem-bearing Jungle an excellent reason to settle or retain territory that might otherwise be cleared.

### Fish
<!-- food 3, shields 0, trade 2 -->

Fish is one of the strongest resources for coastal growth. Three Food allows coastal cities to support larger populations while retaining Ocean’s valuable Trade output. With Harbor infrastructure, Fish tiles become even more important to maritime cities.

### Oil
<!-- food 0, shields 4, trade 0 -->

Oil turns Desert into a major production center. Four Shields make it ideal for industrial cities and military buildup, although the tile supplies no Food. Cities working Oil therefore benefit from nearby fertile terrain or strong food infrastructure.

### Wheat
<!-- food 3, shields 1, trade 0 -->

Wheat is an outstanding growth resource. It combines three Food with a Shield, allowing Plains cities to expand rapidly without becoming economically passive. Wheat is especially valuable in the early game when population growth determines how quickly a city can exploit surrounding tiles.

### Bonus
<!-- food 2, shields 0, trade 0 -->

Bonus Grassland emphasizes pure population support. Two Food provides a dependable foundation for growth and lets the city assign other citizens to Hills, Forests, Mountains, or special-resource tiles that would otherwise be difficult to sustain.

### Silk
<!-- food 1, shields 2, trade 3 -->

Silk combines respectable Forest production with three Trade, making it a valuable mixed economic tile. It is particularly useful for cities pursuing both industrial growth and rapid scientific or financial development.

### Wine
<!-- food 1, shields 0, trade 4 -->

Wine converts defensible Hills into an important commerce resource. Its four Trade can help fund research and taxation while the underlying Hill remains strategically valuable as high ground.

### Iron
<!-- food 0, shields 4, trade 0 -->

Iron makes Mountains a formidable production resource. Four Shields can support heavy military and industrial construction, and the Mountain’s strong defense makes Iron-rich territory naturally suited to fortified production centers and contested border regions.

### Furs
<!-- food 2, shields 0, trade 3 -->

Furs make northern territory both habitable and commercially valuable. Two Food supports the worker directly, while three Trade helps frontier cities contribute meaningfully to the civilization’s science and treasury.

### Icy Oil
<!-- food 0, shields 4, trade 0 -->

Icy Oil gives frozen wasteland major strategic value. Like Desert Oil, it produces four Shields but no Food. A well-supported polar city can use Icy Oil to become a surprisingly strong production center despite its hostile surroundings.

### Spice
<!-- food 3, shields 0, trade 4 -->

Spice is an exceptional economic resource, combining three Food with four Trade. It can sustain the citizen working it while producing substantial commerce, making Spice-bearing Swamp excellent territory to control even before the surrounding wetlands are transformed.

### Fruit
<!-- food 4, shields 0, trade 1 -->

Fruit is one of the strongest natural food resources in rhYciv. Four Food can drive explosive city growth and support citizens assigned to low-food production or trade tiles. Its extra Trade makes it even more useful to developing Jungle settlements.

### Whales
<!-- food 2, shields 1, trade 2 -->

Whales are a superb all-purpose maritime resource. They provide Food, production, and Trade simultaneously, giving coastal cities a balanced tile that helps them grow, build, and generate commerce at the same time. Harbor and Offshore Platform infrastructure can make Whales especially valuable in mature naval cities.

## Governments (7)

### Anarchy

Anarchy represents the temporary breakdown of organized government during a revolution. Cities suffer the same low-organization resource penalty as Despotism: a worked tile producing three or more Food, Shields, or Trade loses one point of that resource. Corruption and waste are severe, although cities can support a relatively large number of units without shield maintenance and military units stationed in a city can impose martial law. Anarchy's only real strategic benefit is that it is the bridge between governments. It is flexible enough to keep an empire functioning for a short period, but its weak economy makes ending the revolution and establishing a stable government a high priority.

### Despotism

Despotism is the basic early-game government. Its great advantage is cheap military support: each city can support a number of ordinary units equal to its population before shield upkeep begins, and Settlers consume only one additional food. Up to three friendly combat units stationed in a city can also enforce martial law and reduce local unhappiness. The price is economic inefficiency. Any worked tile producing at least three Food, Shields, or Trade loses one point of that resource, and distance-based corruption and waste can be substantial. Despotism is excellent for surviving the opening turns, expanding quickly, and fielding an early army, but productive civilizations should usually replace it once a more advanced government becomes available.

### Monarchy

Monarchy removes the harsh tile-production penalty of Despotism while retaining inexpensive Settlers and useful military support. Each city receives three free supported units, and up to three friendly combat units stationed in a city can reduce unhappiness through martial law. Cities therefore become noticeably more productive without forcing the player to abandon an expansionist or military strategy. Corruption and waste still increase with distance from the capital, but the economy is far healthier than under Despotism. Monarchy is one of the most dependable governments for the early and middle game: it offers a strong balance of growth, warfare, stability, and economic freedom without the military-happiness penalties of the representative governments.

### Communism

Communism is designed for large, centralized empires. In rhYciv's current rules, distance from the capital does not create corruption or waste, so remote cities can remain productive even across a vast empire. Communism also avoids the additional empire-size unhappiness that can afflict other governments. Military control is exceptionally strong. Each city receives three free supported units, and martial law is twice as effective as under Despotism or Monarchy: each of up to three friendly combat units in a city can pacify two unhappy citizens. The trade-off is that Settlers consume two food, and Cathedrals are slightly less effective under Communist rule. Communism is particularly valuable to sprawling civilizations fighting on many fronts. It sacrifices the extra trade of Republic and Democracy, but provides excellent territorial efficiency, predictable city management, and strong domestic control.

### Fundamentalism

Fundamentalism is the most aggressively military government in rhYciv. Unhappy and angry citizens are suppressed, eliminating ordinary civil disorder and allowing a civilization to wage prolonged wars without the domestic instability suffered by Republics and Democracies. Cities can support an enormous military - ten units per city before normal shield support begins - and Fanatics are intended to remain free of ordinary support costs. The cost is scientific progress. Fundamentalism sacrifices half of normal research output and also limits any one Tax, Luxury, or Science rate to 60 percent. Settlers consume two food, and the government does not receive the representative governments' trade bonus. Fundamentalism is therefore a powerful choice when the immediate objective is conquest. A technologically established civilization can switch to it, mobilize huge armies, and ignore most wartime unhappiness, but remaining Fundamentalist for too long can allow more research-focused rivals to pull ahead technologically.

### Republic

The Republic is the first great economic government. Every worked tile that already produces Trade gains one additional Trade, dramatically increasing the raw commerce available for Taxes, Luxuries, and Science. The higher 80 percent rate ceiling lets the player concentrate that wealth where it is most useful, making Republic exceptionally strong for research, treasury growth, and peaceful development. That prosperity comes with military costs. Units receive no free shield support, Settlers consume two food, and martial law no longer works. Military units deployed away from friendly cities can also create unhappiness in their home cities. In rhYciv's current happiness rules, the first qualifying deployed unit supported by a Republican city is exempt from this field-unhappiness penalty, while additional deployed units normally create one unhappy citizen each. Police Stations and Women's Suffrage can reduce this burden. Republic is ideal when a civilization has enough infrastructure to pay for its military and wants to turn a strong trade network into rapid technological and economic growth. It rewards compact, prosperous empires and controlled wars rather than permanent mass mobilization.

### Democracy

Democracy is rhYciv's most powerful pure economic government. Like Republic, it adds one Trade to every worked tile that already produces Trade, but its rate ceilings rise to 90 percent and the current rules eliminate distance-based corruption and waste entirely. This makes even remote cities economically useful and allows a mature civilization to convert an extraordinary share of its commerce into Science, Taxes, or Luxuries. Democracy also gives an additional happiness benefit in cities containing a Palace or Courthouse. Its weakness is warfare. There is no free unit support or martial law, Settlers require two food, and qualifying military units deployed in the field normally create two unhappy citizens in their home city. A Police Station or Women's Suffrage reduces that penalty by one citizen per deployed unit. For a peaceful or technologically dominant civilization, Democracy offers unmatched economic potential. It can finance enormous research programs and highly developed cities, but extended offensive wars require careful happiness management, strong infrastructure, or a deliberate switch to a more militaristic government.

## Concepts (9)

### Roads

Roads create fast overland transport links and improve the economic value of suitable worked terrain. A connected road network lets armies reinforce fronts quickly, Settlers and Engineers move efficiently between projects, and cities function as a unified empire rather than isolated settlements.

### Railroads

Railroads are the mature form of the land transportation network. They dramatically improve strategic mobility and increase the productive value of developed terrain. A dense rail network allows a civilization to shift military forces, workers, and economic activity across its territory with exceptional speed.

### Irrigation

Irrigation increases Food production on terrain capable of supporting it. It is most valuable around growing cities and on tiles where an additional Food point allows the city to sustain workers assigned to high-production or high-trade terrain elsewhere.

### Mines

Mines increase Shield production on compatible terrain. Hills are especially valuable mining targets because their large mining bonus can turn them into industrial powerhouses. Mining is central to cities specializing in military units, Wonders, and expensive late-game improvements.

### Fortresses

Fortresses create prepared defensive positions outside cities. They are useful on borders, mountain passes, invasion routes, and strategic resources, allowing armies to hold key ground more efficiently and giving the player time to concentrate reinforcements.

### Airbases

Airbases extend the operational reach of air power beyond city limits. They let aircraft operate from strategically chosen positions near fronts or remote regions and can turn otherwise empty territory into an important military staging area.

### Pollution Cleanup

Industrial development can create pollution that damages the productivity and safety of surrounding terrain. Settlers and Engineers can clean polluted tiles, restoring their normal economic value and preventing environmental damage from undermining a highly developed empire.

### Terrain Transformation

Advanced Engineers can reshape certain terrain into more useful forms. Transformation is a long-term strategic tool: poor terrain can be converted into land better suited for growth or production, while forests and other landscapes can be deliberately changed to fit a city’s economic role.

### Choosing a Government

Anarchy is a temporary transitional state that you should leave as quickly as practical. Despotism offers strong early unit support but weak tile productivity and heavy corruption. Monarchy is a flexible all-purpose government for expansion and conventional warfare. Communism is excellent for large empires, distant cities, and sustained military control. Fundamentalism is best for massive war mobilisation and conquest, at the cost of research. Republic gives excellent trade and research with moderate wartime restrictions. Democracy offers maximum economic efficiency and research potential, but is the most demanding government for prolonged offensive war.
