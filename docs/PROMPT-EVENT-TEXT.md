# rhYciv — Prompt & Event Text Master

**Purpose:** Original, vivid replacement copy for the classic Civilization II-style gameplay prompts used by rhYciv.

> This file does **not** reproduce the original Civilization II prose. It uses legacy prompt identifiers where they are useful for implementation mapping, while all player-facing wording below is newly written for rhYciv.

## Placeholder Convention

Use whichever variable syntax the engine ultimately adopts. The copy below assumes readable placeholders such as `{city}`, `{unit}`, `{tech}`, `{wonder}`, `{item}`, `{leader}`, `{otherLeader}`, `{civ}`, `{otherCiv}`, `{gold}`, `{commodity}`, `{government}`, `{rank}`, and `{year}`.

### Tone Guide

- **Vivid, historical, and human:** the player should feel that events are happening to a living civilization, not a spreadsheet.
- **Concise enough for popups:** most messages are one to three sentences and should fit comfortably in modern dialog layouts.
- **Respect consequence:** war, nuclear weapons, famine, betrayal, and collapse should feel weighty rather than cartoonish.
- **Celebrate progress without becoming sterile:** discoveries and Wonders should feel like real thresholds in human capability.
- **Keep gameplay readable:** the first sentence usually states what happened; the second adds atmosphere or strategic meaning.

## 1. Dawn, Identity & the First Turn

### `rhyciv.opening.beginning` — The First Dawn

Before there were borders, monuments, or chronicles, there was only a people beneath an open sky. They look to you now. From this first campfire, raise a civilization whose memory will outlive the ages.

### `rhyciv.opening.name_leader` — Name the Leader

History is waiting for a name. What shall the generations call the one who first gathered this people beneath a common banner?

### `rhyciv.opening.name_people` — Name the People

Give your people a name worthy of songs, treaties, monuments, and the long memory of the world.

### `@CUSTOMTRIBE` — A People of Your Own

No old banner need bind you. Name the civilization that will rise here and make its place among the nations.

### `@CUSTOMTRIBE2` — The Language of a Nation

Choose the words by which your people will be known: their name, their adjective, and the titles carried by those who rule them.

### `@GOVTNAME` — Title of Rule

Every age gives power a different name. Choose the title by which your authority will be proclaimed.

### `@FIRSTMOVE` — The World Awaits

The map is empty of your story. Move, explore, and choose carefully where the first roots of your civilization will take hold.

### `@FIRSTUNIT1` — Your First Company

These are the first organized hands of your civilization. Where they walk, roads may follow; where they settle, history may begin.

### `@FIRSTUNIT2` — Guardians of the Dawn

Your first warriors stand ready. They are few, but they carry the hopes of a people not yet written into history.

### `@FIRSTPRODUCT` — The First Labor

Your people are ready to build. Choose what this young settlement needs most, and the whole city will bend its effort toward that purpose.

### `@ACCELERATED` — The Pace of History

The ages can move swiftly. In this accelerated world, every decision echoes sooner and every opportunity closes faster.

### `@AUTOMONARCHY` — A Crown in the Vacuum

Order has returned faster than debate. Until a new course is chosen, authority gathers beneath a single crown.

## 2. Founding, Cities & the Life of the People

### `@BUILDCITY` — Found a City

Here roads may meet, homes may rise, and generations may gather behind walls of their own making. Shall this place become a city?

### `@CITYNAME` — Name the City

A city is more than stone and timber; it is a promise to the future. Give this new settlement the name by which history will remember it.

### `@CITYATSEA` — No City Can Stand Here

The sea may carry fleets and fortunes, but it cannot hold streets and foundations. Find solid ground for your city.

### `@ADJACENTCITY` — Too Near Another City

Another city already commands this ground. Give each settlement room to breathe, grow, and draw strength from the land around it.

### `@ONLYSETTLERS` — Only Settlers May Found Cities

Armies can conquer ground, but only settlers can turn wilderness into a lasting home. Bring builders, families, and tools.

### `@FURTHERGROWTH` — The City Strains at Its Bounds

{city} is pressing against the limits of its water, streets, and sanitation. Give it the works it needs, and the city can rise into a greater age.

### `@FOODSHORTAGE` — Hunger in {city}

The granaries of {city} are running thin. Unless more food reaches the people, growth will turn to hardship and the city will begin to shrink.

### `@WATERSUPPLY` — Water for a Growing City

{city} has grown beyond the reach of wells and cisterns. A greater water system is needed before the city can safely expand.

### `@WELOVEKING` — A City in Celebration

Lanterns burn late in {city}. Markets overflow, music fills the streets, and the people celebrate an age of confidence beneath your rule.

### `@WEDONTLOVEKING` — The Celebration Ends

The banners are coming down in {city}. The people return to ordinary life, and the city’s brief season of jubilation has passed.

### `@GOLDENAGE` — A Golden Age Begins

Across the civilization, workshops ring, scholars argue late into the night, and citizens speak of the future with uncommon faith. This is an age later generations may envy.

### `@PLEASECITY` — A City Is Requested

Our people ask for one city to be placed under the terms of this agreement. Choose carefully; a city is land, labor, memory, and power.

### `@PLEASECITIES` — Cities Are Requested

The proposed settlement demands cities, not merely coin. Consider the price carefully: every city ceded redraws the future.

### `@CITYCAPTURE` — {city} Has Fallen

The gates of {city} are open and its standards have been torn down. Your troops now hold the streets; the city’s fate passes into your hands.

### `@CITYCAPTURE2` — A New Banner Over {city}

After smoke, shouting, and shattered gates, {city} wakes beneath a new banner. Its people now wait to learn what your victory will mean.

### `@CITYLOSEALLY` — An Ally Loses {city}

News has reached the capital: allied defenses at {city} have broken. The enemy is advancing, and the balance of the war has shifted.

### `@RANSOMCITY` — The Price of a City

The victors offer to spare {city} from further ruin—for a price. Pay {gold} gold, or refuse and accept what follows.

### `@MOVECAPITAL` — A New Seat of Power

The old center can no longer carry the weight of the realm. Choose the city from which decrees, taxes, armies, and ambassadors will now radiate.

### `@ADMIRECITY` — A City Worth Seeing

Visitors speak of {city} with awe. Its streets, works, and monuments have become a living argument for the strength of your civilization.

### `@ALREADYSOLD` — Already Sold

This city has already stripped away one improvement this turn. What is dismantled cannot be endlessly sold twice.

### `@DELETEITEM` — Dismantle Improvement

Once removed, this work will cease to serve the city. Recover what value you can, but remember that rebuilding takes time.

## 3. Workers, Terrain & the Shape of the Land

### `@ALREADYROAD` — Road Already Present

A road already binds this ground to the wider world. Send these workers where new paths are still needed.

### `@ALREADYMINING` — Mine Already Worked

The valuable seams here are already being cut and hauled. Further mining orders would only repeat work already done.

### `@ALREADYFARMLAND` — Farmland Already Prepared

This soil has already been shaped for intensive agriculture. Its furrows are as developed as present methods allow.

### `@ALREADYFORT` — Fortifications Already Stand

Earthworks and defenses already command this position. The ground is prepared for soldiers to hold it.

### `@CANTIMPROVE` — The Land Resists This Work

This terrain cannot support that improvement. The land has its own logic; choose a task suited to what stands beneath your feet.

### `@BRIDGEBUILDING` — Bridges Open the Roads

Your engineers have learned to carry roads across rivers without surrendering momentum. Waterways that once divided the map can now become arteries of trade and war.

### `@RAILROADS` — The Iron Road

Steel rails and powerful engines have changed distance itself. Cities once separated by weeks can now exchange people, goods, and armies with astonishing speed.

### `@ENGINEERS` — The Age of Engineers

Your builders have become engineers: surveyors, bridge-makers, excavators, and shapers of whole landscapes. The map itself is now a material of policy.

### `@ODDTERRAIN` — Unusual Ground

This terrain does not behave like ordinary land. Study it before committing workers or armies; unfamiliar ground rewards attention.

### `@HIDDENTERRAIN` — The Land Revealed

What seemed blank on the map has taken form beneath your scouts. Every hill, forest, river, and coast is another choice waiting to be made.

### `@SEAFARING` — Beyond the Familiar Coast

Your people have learned to trust hull, sail, star, and current. The horizon is no longer an edge; it is a road.

### `@NEWXFORM` — The Land Can Be Remade

Engineering has reached the point where terrain itself can be transformed. What nature placed here need not be what future generations inherit.

## 4. Villages, Ruins & Exploration

### `@SURPRISEMERCS` — Warriors From the Wild

A hard-bitten band emerges from the settlement, impressed by your growing power. They offer their weapons and experience to your cause.

### `@SURPRISEBARB` — An Ambush From the Ruins

The silence breaks into horns and shouting. Armed raiders pour from hiding, and your explorers have only moments to form ranks.

### `@SURPRISEMETALS` — A Cache of Wealth

Beneath rotting timbers lies a hoard of worked metal, coin, and trade goods. What another people abandoned will now strengthen your treasury.

### `@SURPRISENOMADS` — Wanderers Join Your People

A wandering people has watched your banners from afar. They ask for a place among your civilization and offer their wagons, livestock, and knowledge of the land.

### `@SURPRISENOTHING` — Only Wind in the Ruins

Cold ashes, fallen walls, and weeds are all that remain. Whoever lived here vanished long ago, leaving no wealth but the warning of impermanence.

### `@SURPRISESCROLLS` — Forgotten Knowledge

Inside a dry chamber, your scouts find preserved tablets and manuscripts. Ideas thought lost to time are carried back to your scholars.

### `@SURPRISETRIBE` — A Settlement Joins Your Realm

The people here have heard of your civilization and choose union over isolation. Their homes, fields, and hopes now become part of your story.

### `@ARCHAEOLOGISTS` — Voices Beneath the Dust

Scholars uncover traces of a vanished age. Broken walls and buried tools reveal that history is deeper than any living memory.

### `@ARCHAEOLOGISTS3` — The Past Speaks Clearly

Years of excavation have assembled fragments into a story. What was once legend can now be read in stone, bone, and earth.

## 5. Production, Construction & Wonders

### `@PRODUCTION` — Choose Production

The workshops of {city} await direction. Decide what the city will devote its labor, timber, metal, and time to creating next.

### `@PRODCHANGE` — Change Production

Changing course now will waste some of the effort already invested. Redirect the city only if the new need outweighs the lost momentum.

### `@AUTOBUILD` — Automated Production

The city may choose its own next project when current work is complete. Leave the workshops to local judgment, or retain direct control.

### `@STARTWONDER` — A Wonder Is Begun

Scaffolds rise in {city} for {wonder}. The work is greater than any ordinary building; if completed, generations will measure themselves against it.

### `@ALMOSTWONDER` — A Rival Nears Completion

Travelers report that {otherCiv} is nearing completion of {wonder}. The race is no longer theoretical; the final stones are being set.

### `@ABANDONWONDER` — Abandon the Great Work

Years of labor have already gone into {wonder}. Abandoning it will free the city for other work, but this unfinished dream may never be reclaimed.

### `@SWITCHWONDER` — Redirect the Great Work

The craftsmen of {city} can turn their accumulated effort toward another Wonder. Choose quickly; history rarely waits for indecision.

### `@STILLWONDER` — The Work Continues

{city} still labors on {wonder}. Stone by stone, beam by beam, the impossible is becoming real.

### `@STILLWONDER1` — A Monument Taking Shape

The outline of {wonder} now dominates {city}. Citizens can see the future landmark rising above their streets.

### `@STILLWONDER2` — The Final Stage

Only the hardest work remains on {wonder}. Complete it, and the achievement will belong to your civilization for as long as memory endures.

### `@LOSTWONDER` — The Wonder Is Lost

Another civilization has completed {wonder} first. The plans in {city} are now relics of a race that has ended.

### `@CAPTUREWONDER` — A Wonder Changes Hands

With the capture of {city}, {wonder} now stands within your realm. You did not raise it, but history has placed its guardianship in your hands.

### `@ADDTOWONDER` — Caravan Aid to a Wonder

The caravan’s cargo can be folded into the vast work on {wonder}. Warehouses empty, crews unload, and the great project leaps forward.

### `@ADDTOTHRONE` — A Gift to the Throne

A new treasure has been added to the seat of power, not merely as decoration but as a visible record of an age growing richer and more confident.

### `@COMPLETE0` — Construction Complete

The final crews step back. {city} has completed {item}, and its benefits now belong to the city and the civilization beyond it.

### `@CANTHOCKTHIS` — This Work Cannot Be Rushed

Some achievements cannot be bought whole with gold. Their cost must be paid in sustained labor, planning, and time.

### `@IMPROVEMENTS` — City Improvements

A city becomes powerful through the institutions it builds between its walls: markets, libraries, defenses, temples, factories, and the systems that bind them together.

### `@CONSTRUCTION` — Construction

Your builders now command arches, heavy masonry, and more ambitious works. Cities can reach higher, endure longer, and shelter more complex life.

### `@CATHEDRL` — A Great House of Worship

Vaulted stone and gathering crowds transform the city’s spiritual life. The new cathedral becomes both sanctuary and landmark.

### `@MRKTPLCE` — A Marketplace Opens

Stalls, scales, contracts, and caravans crowd a new center of exchange. Wealth now moves through {city} with greater purpose.

## 6. Science, Knowledge & Breakthroughs

### `@BREAKTHROUGH` — A New Idea Changes the World

Your scholars have mastered {tech}. What yesterday was impossible now becomes a tool, a question, or a doorway to something greater.

### `@CIVADVANCE` — Civilization Advance

Knowledge has crossed a threshold. {tech} is no longer speculation; it is now part of the working inheritance of your civilization.

### `@GIVETECHNOLOGY` — Knowledge Granted

{otherCiv} places the secrets of {tech} in your hands. Ideas ignore borders once they are shared, and your scholars move quickly to absorb them.

### `@TECHGIFT2` — A Gift of Knowledge

The offer is not gold or land, but understanding: {tech}. Accepting it may shorten years of research in a single diplomatic gesture.

### `@STEALNONE` — No Useful Secret Found

The spy searches archives, workshops, and private studies, but finds nothing your civilization does not already know.

### `@STEALSPECIFIC` — Choose a Secret to Steal

Your agent has reached the heart of the rival archives. Choose carefully; there may be time to carry out only one secret.

### `@STEALTHESE` — Secrets Within Reach

These technologies are exposed to your agent. One stolen idea can alter armies, economies, and the balance of an age.

### `@STEALHARD` — The Archives Are Guarded

Security is tight and the mission is perilous. The knowledge is valuable precisely because the enemy knows what it is worth.

### `@FOILEDAGAIN` — The Attempt Fails

The operation collapses before the secret can be secured. Doors close, guards move, and the opportunity vanishes into suspicion.

### `@FUTURETECH` — Beyond the Known Age

Your civilization has reached beyond the established tree of knowledge. Research now pushes into futures no earlier generation could have named.

## 7. Caravans, Trade & the Treasury

### `@CARAVANBUILT` — A Caravan Is Ready

Wagons are loaded, manifests sealed, and guards assembled. This caravan can carry the wealth of {city} across the world.

### `@CARAVANHOME` — Trade With the Home City

The caravan has returned to familiar markets. Useful exchange is possible, but the greatest fortunes are usually found beyond the horizon.

### `@CARAVANOTHER` — A Foreign Market

The caravan has reached {city}. Merchants crowd the gates, prices shift, and a new commercial relationship is within reach.

### `@CARAVANMENU` — Caravan Orders

The caravan stands ready. Establish trade, help a Wonder, or continue onward in search of a richer destination.

### `@CARACONFIRM` — Establish This Trade Route?

Once opened, this route will tie the fortunes of two cities together. Confirm the exchange and let merchants turn distance into wealth.

### `@FOODCARAVAN` — Food on the Road

This caravan carries grain, livestock, and preserved stores instead of luxury goods. Somewhere ahead, full granaries may matter more than gold.

### `@SUPPLYSHOW` — Goods Available

The warehouses of {city} are ready to export these goods. Choose what the wider world is most likely to value.

### `@SUPPLYSEARCH` — Seeking a Market

Merchants compare rumors, prices, and distant demand. The right destination can turn an ordinary cargo into a national windfall.

### `@SUPPLYNONE` — No Export Surplus

{city} has no worthwhile surplus ready for long-distance trade. Let its economy grow before sending another caravan.

### `@MONEYGIFT` — A Gift of Gold

{otherLeader} offers {gold} gold. Coin is never merely coin between nations; it may be gratitude, leverage, apology, or bait.

### `@EXCHANGEGIFT` — An Exchange of Gifts

The meeting turns cordial. Each side offers something of value—not enough to bind the future, but enough to show that trust is still possible.

### `@EXCHANGEGIFT2` — A Generous Exchange

The gifts are substantial, and the gesture will be remembered. Diplomacy often begins with symbols before it becomes policy.

### `@EXCHANGEPETTY` — A Token Offering

The offer is small, almost ceremonial. Accept it for what it is, or make clear that your civilization expects more serious terms.

### `@CASHFORPEACE` — Gold for Peace

They offer {gold} gold to end the fighting. Decide whether the treasury is worth more than the opportunity war has opened.

## 8. Government, Revolution & Public Order

### `@REVOLUTION` — The Old Order Breaks

The machinery of government is coming apart. Offices fall silent, factions gather, and the people wait to see what form of rule will rise from the uncertainty.

### `@OVERTHROWN` — A Government Falls

The old regime can no longer command obedience. Its seals, titles, and decrees have become scraps of paper; a new political order must be chosen.

### `@PICKGOVT` — Choose a New Government

The revolution has opened a rare moment in which institutions can be remade. Choose not only who rules, but how power itself will work.

### `@GOVERNMENTS` — Forms of Government

Every system of rule trades one strength for another: speed, liberty, control, commerce, stability, or war-making power. Choose the structure that fits the age you intend to build.

### `@ALLOWHAWKS` — The Hawks Demand War

Voices in government insist that restraint has become weakness. They urge action before the enemy grows stronger.

### `@CONTINUEHAWKS` — Pressure for War Continues

The war faction has not quieted. Each new insult is being used as proof that patience has failed.

### `@ALLOWAGGRESSOR` — Authority to Strike

The restraints on open aggression have lifted. If you choose war now, the responsibility will be unmistakably yours.

### `@OVERRULECEASE` — The Government Blocks the Attack

Your government refuses to tear up the cease-fire. The guns will remain silent unless political conditions change.

### `@OVERRULEPEACE` — The Government Defends the Treaty

The order to attack is rejected. A signed peace carries weight at home as well as abroad.

### `@UNOVERCEASE` — The Restraint Is Lifted

The political barrier that preserved the cease-fire has fallen away. Military action is once again possible.

### `@UNOVERPEACE` — The Treaty No Longer Shields Them

Domestic opposition to breaking the peace has collapsed. The decision now rests with you.

### `@DEMOCRATS` — The Public Demands a Voice

Citizens are no longer content to be subjects of distant decisions. They demand institutions that answer to the governed.

### `@BARBARITY` — Rule by Fear

Order can be imposed through fear, but fear leaves scars. Every act of brutality purchases obedience at the cost of memory and trust.

## 9. Diplomacy — Greetings, Trust & Attitude

### `@GREETINGS` — An Envoy Arrives

The banners of {otherCiv} appear at the edge of your court. Their envoy requests an audience; what follows may shape decades.

### `@HOWDYALLY` — An Ally at the Door

{otherLeader} greets you as a partner in a shared cause. Between allies, even ordinary words carry the weight of promises already made.

### `@HOWDYPEACE` — A Peaceful Audience

{otherLeader} approaches under the protection of peace. There is caution in the room, but no sword has yet been drawn.

### `@WELCOMEALLY` — Welcome, Ally

Your ally is received with open gates and honored banners. Today’s meeting begins from trust rather than suspicion.

### `@WELCOMEPEACE` — Welcome Under Peace

The envoys are admitted without hostility. Peace has given both sides the luxury of speaking before acting.

### `@ATTITUDEALLY` — Alliance Attitude

An alliance is more than a document; it is a running account of aid, restraint, shared enemies, and remembered betrayals.

### `@ATTITUDEPEACE` — Peace Attitude

Relations are peaceful, but peace has shades: warm, wary, resentful, or nearly broken. Every agreement changes the tone of the next meeting.

### `@ATTITUDEMULTI` — Diplomatic Attitude

Other civilizations judge you by more than your armies. Reputation, power, generosity, threats, and broken promises all arrive before your ambassadors do.

### `@AMBASSADORS` — Ambassadors

Permanent embassies turn rumor into information and accidents into conversations. Diplomacy becomes easier when nations have a door they can knock on.

### `@EMISSARYFORCE` — An Audience Is Demanded

The envoy will not be dismissed quietly. Their ruler insists that the matter is too urgent to postpone.

### `@HERALDWARNING` — A Herald Brings Warning

A mounted herald arrives under truce, carrying words meant to be heard before armies speak instead.

### `@LASTCONTACT` — A Long Silence

It has been years since your courts last exchanged words. Old assumptions may no longer be safe; the world has changed in the silence.

### `@NOTORIOUS` — A Reputation in Ruins

Your promises are now weighed against a history of broken ones. Foreign rulers may bargain with you, but few will do so without suspicion.

### `@NOVIOLATORS` — A Record of Honor

Your agreements have carried weight because you have made them carry weight. Even rivals must account for a reputation built on kept promises.

### `@VIOLATORS` — Treaty Breakers

The diplomatic world remembers who tears up agreements when they become inconvenient. Betrayal is a weapon that dulls every time it is used.

### `@APOLOGIZE` — Offer an Apology

Words cannot undo an injury, but they can decide whether an injury becomes a feud. Offer a formal apology and test whether pride can still yield to reason.

### `@ANNOYALLIED` — An Ally Is Offended

Your ally’s patience is thinning. Shared enemies do not erase every grievance between friends.

### `@ANNOYCEASE` — The Cease-Fire Frays

The guns are quiet, but the diplomacy is not. One more provocation may be enough to turn a pause into renewed war.

### `@ANNOYPEACE` — Peace Under Strain

The treaty still stands, though resentment gathers beneath it. Peace can survive anger, but not indefinitely without repair.

### `@ANNOYVASSAL` — A Subordinate Power Resents You

Obedience without respect breeds quiet resistance. The weaker state is complying, but its resentment is becoming part of the strategic landscape.

### `@ADMIRECITY` — Foreign Admiration

Even rival envoys speak with respect of {city}. Prestige is a form of power that crosses borders without an army.

### `@BONDGLORY` — Bound by Shared Glory

Victory fought side by side has created a bond no treaty clerk could have written. For now, memory itself strengthens the alliance.

### `@PERHAPSSOLIDARITY` — Perhaps We Stand Together

Our interests are not identical, but neither are they opposed. There may be enough common ground here to build something durable.

### `@PERHAPSSECRET` — Perhaps Knowledge Can Open the Door

If trust is uncertain, perhaps an exchange of knowledge can prove that cooperation still has value.

### `@PERHAPSTHROWIN` — Sweeten the Offer

The agreement is close, but not close enough. Add something of value and the balance may finally tip.

### `@PERHAPSTHANKSANYWAY` — No Agreement Today

The terms do not meet our needs. We part without agreement, though the door need not remain closed forever.

### `@PERHAPSBYE` — The Audience Ends

There is nothing more to be gained from words today. The envoys withdraw, leaving the next move to events beyond the chamber.

### `@PERHAPSDIDNTPROVE` — Goodwill Was Not Enough

Your gesture was noticed, but it did not erase the larger dispute. Diplomacy may need more than courtesy to move forward.

### `@TAUNTALLY` — An Ally Speaks Harshly

Even allies can wound with words. Decide whether this insult is noise, warning, or the first crack in something larger.

### `@UPYOURSTOO` — The Insult Is Returned

Courtesy has left the room. What began as negotiation is hardening into pride, and pride has started many unnecessary wars.

### `@UNFORTUNATE` — An Unfortunate Turn

The meeting has gone badly. Neither side leaves satisfied, and the next contact may begin from a colder place.

### `@WITHDRAWN` — The Envoy Withdraws

The delegation rises, gathers its papers, and leaves without ceremony. Whatever chance existed for agreement has passed for now.

### `@WORTHLESS` — Offer Rejected

They dismiss the offer as unworthy of further discussion. If you want movement, the terms will have to change.

## 10. Diplomacy — Peace, Cease-Fire & Alliance

### `@PROPOSECEASE` — Propose a Cease-Fire

Enough blood has been spent for the moment. Offer a pause in the fighting and see whether exhaustion can accomplish what persuasion could not.

### `@CEASEFIRE` — Cease-Fire

The weapons fall silent, but only for a time. A cease-fire is breathing room, not reconciliation.

### `@GRANTCEASE` — Cease-Fire Accepted

They agree to halt the fighting. Troops remain where they stand, but commanders receive orders to hold their fire.

### `@CEASEEXPIRE` — The Cease-Fire Expires

The agreed pause has run its course. Unless renewed, every frontier is once again a place where one nervous soldier can begin a war.

### `@BREAKCEASE` — Break the Cease-Fire

To attack now is to end the truce by your own hand. The military gain may be immediate; the diplomatic cost will last longer.

### `@WALLCEASE` — Peace Enforced by Great Walls

The balance of power surrounding the Great Wall makes open aggression politically difficult. The road to war is blocked for now.

### `@WALLOVERCEASE` — The Great Wall Preserves the Truce

Pressure for war collides with a stronger diplomatic reality. The cease-fire survives.

### `@PROPOSEPEACE` — Propose Peace

Wars end when one side is destroyed—or when both decide that a future matters more than another battlefield. Offer formal peace.

### `@SIGNPEACE` — Peace Is Signed

The treaty is sealed. Borders remain, grievances remain, and armies remain—but for the first time in years, tomorrow need not begin with battle.

### `@CANCELPEACE` — Renounce Peace

Breaking this peace will free your armies and stain your word. Once the treaty is torn, every rival will remember who tore it.

### `@CANCELTREATY` — Renounce the Treaty

A treaty is only as strong as the willingness to honor it. End this one, and accept the consequences of making your signature lighter.

### `@PEACENOBETRAY` — Peace Refused — Betrayal Remembered

They have not forgotten your previous betrayals. Until time or extraordinary concessions change the balance, they will not trust another peace.

### `@PEACENODISLIKE` — Peace Refused — Hostility Runs Deep

Their hostility is stronger than their fear of continued war. No treaty will be signed while anger rules the council chamber.

### `@PEACENOPATIENCE` — Peace Refused — Patience Exhausted

They are done listening. Too many meetings have ended without result, and their envoys will not reopen the question now.

### `@PEACENOWINNING` — Peace Refused — They Believe They Are Winning

Their commanders smell victory and see no reason to bargain it away. Peace will become attractive only when the war looks different.

### `@PROPOSEALLIANCE` — Propose an Alliance

Peace says we will not fight. Alliance says we will stand together when someone else does. Offer a bond that changes the strategic map.

### `@SIGNALLIED` — Alliance Forged

The agreement is sealed. From this day, your enemies must reckon not with two separate powers, but with the possibility that each will answer for the other.

### `@CANCELALLIANCE` — End the Alliance

To dissolve an alliance is to turn a shield into a question mark. Former partners may remain friends—or become the rivals who know you best.

### `@CANCELALLY` — Withdraw From the Alliance

Your civilization steps away from the pact. The shared obligations end here, but shared memories do not.

### `@CANCELALLIED` — Alliance Cancelled

The alliance is over. Treaties may vanish in an instant; the strategic consequences rarely do.

### `@ALLIANCENOBETRAY` — Alliance Refused — Trust Is Broken

They will not tie their survival to a ruler whose promises they do not trust. Reputation has become the obstacle.

### `@ALLIANCENODISLIKE` — Alliance Refused — Too Much Hostility

They may tolerate peace, but they will not call you friend. The distance between non-aggression and alliance remains too great.

### `@ALLIANCENOPATIENCE` — Alliance Refused — Talks Exhausted

Their diplomats have heard enough proposals. For now, no further argument will move them.

### `@ALLIANCENOSMALL` — Alliance Refused — You Seem Too Weak

They see little strategic value in binding themselves to a smaller power. Grow stronger, and the same proposal may sound different.

### `@ALLIANCENOTHANKS` — Alliance Refused

They decline without closing every door. Cooperation may still be possible, but not under the obligations of a formal alliance.

### `@ALLIANCENOWINNING` — Alliance Refused — They Need No Partner

Confidence has made them solitary. They believe the future is already leaning their way and see no need to share the advantage.

### `@ETERNALALLIES` — An Alliance Beyond Convenience

Years of shared struggle have made the alliance feel older than the rulers who maintain it. To break it now would shock both civilizations.

### `@ACTIVATEALLY` — Call the Alliance

The treaty is no ornament. War has come, and now you may call upon your ally to honor the promise made in quieter days.

### `@ALLYHELPS` — The Ally Answers

Your ally honors the pact. Their banners turn toward the common enemy, and the war becomes larger overnight.

### `@DIDNTHELP` — The Ally Refuses

When the moment came, the promised help did not. The treaty remains on paper, but trust has taken a wound.

### `@PATIENCEALLY` — An Ally’s Patience

Even friendship has limits. Repeated demands and provocations are testing how much strain this alliance can bear.

## 11. Diplomacy — War, Threats & International Incidents

### `@DECLAREWAR` — Declare War

This command will end diplomacy and begin open war. Once issued, armies, cities, trade, and memory will all be changed by what follows.

### `@PEARLHARBOR` — A Treacherous Attack

Without warning, forces of {otherCiv} strike under the cover of existing peace. The treaty lies in ruins before the first formal declaration is even heard.

### `@MAJORINCIDENT` — A Major International Incident

The crisis can no longer be dismissed as a misunderstanding. Governments are choosing sides, armies are watching borders, and one decision may widen the conflict.

### `@INCIDENTALLIED` — An Incident With an Ally

An allied nation is involved in the crisis. What would be a simple dispute between strangers now tests the meaning of your alliance.

### `@ALLYUNDERATTACK` — Your Ally Is Under Attack

Enemy forces have struck your ally. The treaty now asks a plain question: was the alliance written for ceremony, or for this moment?

### `@ALLYATTACKING` — Your Ally Goes to War

Your ally has opened hostilities. Their war may soon become your problem whether you join it or not.

### `@ALLYMAKESWAR` — An Ally Declares War

The diplomatic map shifts as your ally enters war with {otherCiv}. Every shared border and treaty must now be reconsidered.

### `@ALLYMAKESPEACE` — An Ally Makes Peace

Your ally has ended its war. The front narrows, and you may now find yourself fighting alone.

### `@PRETEXTALLIED` — A Cause for Allied War

Your diplomats argue that the enemy’s actions justify a common response. If the alliance has meaning, they say, it must mean something now.

### `@DEMANDHELP` — Demand Allied Assistance

Call on {otherLeader} to provide material or military help. Allies reveal themselves most clearly when help is expensive.

### `@WALLFORCE` — The Great Wall Checks Aggression

The enemy’s position is protected by a diplomatic order built around the Great Wall. Force alone cannot easily break the political barrier.

### `@OVERABARREL` — Negotiating From Weakness

They know the pressure is on you. Every concession they ask reflects the belief that you have fewer alternatives than they do.

### `@OUTAHEREALLY` — Relations Collapse

The meeting ends in fury. Whatever friendship existed has been pushed aside by threat, pride, and the expectation of conflict.

### `@HELLNOWEWONTGO` — Defiance

They reject the demand outright. Fear will not move them, and the next argument may have to be made outside the council chamber.

### `@ACCURSEDUN` — The United Nations Intervenes

The wider community of nations has entered the dispute. What you do next will be judged far beyond the immediate battlefield.

### `@ACCURSEDWALL` — The Great Wall Frustrates War

Old fortifications have become more than stone: they are a symbol around which diplomacy hardens against aggression.

## 12. Diplomats, Spies & Covert Operations

### `@SPYOPTIONS` — Covert Mission

Your agent has reached {city}. Choose the mission carefully; information, sabotage, revolt, and theft each carry different risks and consequences.

### `@ENEMYEMBASSY` — Establish an Embassy

A permanent diplomatic mission will give you a clearer window into this civilization. Information is often worth more than a single act of sabotage.

### `@ENEMYINVESTIGATE` — Investigate {city}

Your operative can slip into the city’s administrative life and return with a picture of its defenses, production, and resources.

### `@SABOTAGEOPTIONS` — Choose a Sabotage Target

The agent has access to vulnerable systems inside {city}. Choose what to cripple before the window closes.

### `@SABOTAGESPECIFIC` — Sabotage a Specific Improvement

Name the target. A single destroyed facility can matter more than a dozen burned warehouses if chosen well.

### `@SABOTAGEONE` — Sabotage Succeeds

The charge is placed and the target is destroyed. By the time alarms spread through {city}, your agent is already moving toward escape.

### `@SABOTAGETWO` — Sabotage Causes Heavy Damage

The operation tears through more than expected. Production stalls, officials panic, and the city will feel the damage for some time.

### `@SABOTAGENO` — Sabotage Fails

Security closes around the target before the operation can be completed. The mission is lost, and suspicion spreads through the city.

### `@SABOTAGEHARD` — A Difficult Sabotage Mission

The target is heavily guarded and failure may cost the agent. Success, however, could cripple a vital part of the enemy city.

### `@PLANTEDNUKE` — A Nuclear Device Is Planted

The agent has crossed the final line. A hidden nuclear device now waits inside {city}; if detonated, the consequences will reach far beyond the mission.

### `@PLANTEDNUKE2` — The Device Is Armed

There is no ordinary sabotage left here. The weapon is in place, the city is unaware, and history is seconds from changing.

### `@WATERSUPPLY` — Poison the Water Supply

The agent has access to the city’s water system. This attack would strike civilians as surely as soldiers and will be remembered accordingly.

### `@BRIBEDUNIT` — An Enemy Unit Changes Sides

Coin, promises, and self-interest prove stronger than loyalty. The unit lowers its old banner and accepts your command.

### `@CANESCAPE` — Escape Route Open

The mission is complete and a narrow route remains out of the city. Move now, before the guards understand what has happened.

### `@MERCBETRAY` — Mercenaries Betray Their Employer

The hired blades have decided that loyalty is worth less than survival or better pay. Their allegiance changes at the worst possible moment.

## 13. Combat, Armies & the Cost of War

### `@CASUALTIES` — The Cost of Battle

The field is won or lost, but the dead do not care which banner remains standing. Every battle spends lives that no treasury can replace.

### `@DESTROYED` — Unit Destroyed

{unit} has been destroyed. The formation disappears from the map, but the consequences of its loss may remain for many turns.

### `@UNITKILLED` — A Unit Falls

{unit} is gone. Survivors scatter, standards vanish, and the line must close around an empty place.

### `@BLEWITUP` — Target Destroyed

The attack lands with devastating force. Smoke and wreckage are all that remain where the target stood.

### `@BOATSINK` — A Ship Is Lost

The vessel rolls beneath the waves, taking cargo, crew, and certainty with it. The sea closes quickly over even the proudest hull.

### `@JETCRASH` — Aircraft Lost

Fuel, weather, damage, or distance has won. The aircraft fails to return, and the sky keeps no monument.

### `@AIRCOMBT` — Air Combat

Aircraft meet at speed above the battlefield. Seconds of maneuver and fire will decide which pilot returns home.

### `@ENEMYFIGHTERS` — Enemy Fighters Intercept

Hostile aircraft rise to meet the mission. The target is no longer the only danger in the sky.

### `@PARTISANS` — Partisans Rise

The city may have fallen, but resistance has not. Armed partisans emerge from streets, hills, and villages to continue the fight under their own initiative.

### `@INTRUDERS` — Enemy Forces Detected

Foreign troops have crossed into territory you consider your own. Whether mistake, test, or invasion, their presence demands a response.

### `@BARBARIANS` — Barbarian Raiders

Raiders with no interest in treaties or borders have appeared. They will take what they can and burn what they cannot carry.

### `@BARBARIANSLAND` — Raiders Come Ashore

Boats grind onto the coast and armed raiders spill onto land. Settlements near the shore are suddenly on the front line.

### `@ALLIEDREPAIR` — Repairs in Allied Territory

Friendly ports and workshops open their doors to your damaged forces. An alliance is sometimes measured in spare parts and safe harbors.

### `@AMPHIBMOTIZE` — Amphibious Assault

Troops prepare to strike from sea onto defended land. There will be no easy retreat once the first boats touch shore.

### `@SCRAMBLE` — Scramble Fighters

Radar and lookouts report incoming danger. Fighters can launch now to contest the skies before the attack reaches its target.

### `@SWORDFGT` — Close Combat

The distance collapses. Formation, nerve, steel, and exhaustion decide what maps and speeches cannot.

### `@HELICPTR` — Helicopter Operations

Rotors lift troops over roads, rivers, and broken ground. Mobility has become a weapon in its own right.

### `@YOURNUKES` — Your Nuclear Forces

You possess weapons capable of ending cities in moments. Their existence is power; their use will be something else entirely.

## 14. Airlift, Paradrops & Logistics

### `@AIRLIFTSELECT` — Choose an Airlift Destination

Select the city that will receive this unit. Airlift turns geography into logistics, but only where runways and capacity allow.

### `@ALREADYAIR` — Already Moved by Air

This unit has already used its air movement opportunity. Even fast transport has limits within a single turn.

### `@ALREADYAIRLIFT` — Airlift Capacity Used

This city has already committed its airlift capacity. More troops must wait for the next operational window.

### `@PARADROPRULES1` — Paradrop Range

Airborne troops can leap across terrain that would delay an ordinary army, but only within the reach of aircraft and planning.

### `@PARADROPRULES2` — Paradrop Risk

A paradrop begins with surprise and ends with isolation. Once committed, the unit may have to survive without immediate support.

### `@PARADROPTARGET` — Choose Drop Zone

Select the ground where the airborne force will descend. Open terrain, enemy positions, and nearby support all matter.

### `@PARADROPTARGET1` — Target Beyond Range

That drop zone lies beyond operational reach. Choose ground the aircraft can actually deliver troops to.

### `@PARADROPTARGET2` — Unsafe Drop Zone

The selected ground cannot receive this paradrop under current conditions. Find another approach.

### `@ALREADYCHOSEN` — Order Already Given

This unit has already been committed to that action. Choose a different order or let the turn advance.

### `@WAITPRODUCTION` — Awaiting Production

The unit or project is not ready yet. Logistics has its own clock, and the battlefield must sometimes wait for the workshops.

## 15. Nuclear Weapons, Pollution & Planetary Consequences

### `@NUCLEARWEAPONS` — The Nuclear Age

Humanity has learned how to release energies once confined to stars and deep earth. From this moment on, war carries the possibility of destruction on an entirely new scale.

### `@MANHATTAN` — The Bomb Becomes Possible

The scientific barrier has been crossed. Nuclear weapons are no longer theory; every great power must now think differently about war, deterrence, and survival.

### `@USEWEAPONS` — Use Nuclear Weapons?

This attack will not be an ordinary act of war. A city, its people, and the land around it may be transformed in a single flash. Confirm only if you accept the world that follows.

### `@NUKEXPLO` — Nuclear Detonation

For an instant the horizon becomes brighter than day. Buildings vanish, fires race outward, and the political world changes before the smoke has even risen.

### `@CHERNOBYL` — Nuclear Catastrophe

A reactor disaster has torn through {city}. Evacuation, contamination, and fear spread beyond the plant itself, leaving a scar that will outlive the emergency.

### `@GLOBALWARMING` — The Climate Is Shifting

Smoke, industry, and pollution have accumulated beyond the planet’s ability to quietly absorb them. Coastlines, rainfall, and fertile land now begin to change.

### `@POLLUTION` — Pollution Near {city}

The wealth of industry has left a dark mark on the land. Clean it before local damage becomes part of a planetary problem.

### `@EVENTERRAIN` — The Land Has Changed

Forces larger than any city have altered the terrain. The map is not fixed; climate, disaster, and human action can redraw it.

## 16. Diplomatic Conversation & Negotiation Flow

### `@DIPLOMACY` — Diplomacy

Across the table sits another civilization with fears, ambitions, pride, and memory. Every word is a move on the same board as armies and cities.

### `@DIPLOMACYMENU` — What Will You Discuss?

Choose the matter to place before {otherLeader}: peace, alliance, exchange, tribute, threats, or farewell.

### `@PARLEYREQUEST` — Request Negotiations

Send word that you are willing to talk. The answer may reveal as much as the meeting itself.

### `@PARLEYWAITING` — Awaiting Their Reply

The message has been delivered. For now, diplomacy is a matter of waiting to see whether the other side opens the door.

### `@PARLEYACCEPT` — Negotiations Accepted

They agree to meet. The room is prepared, the guards remain outside, and the next moves will be made with words.

### `@PARLEYACCEPT2` — Offer Accepted

The terms are accepted. What was only language a moment ago now becomes policy between nations.

### `@PARLEYCOUNTEROFFER` — Counteroffer

They reject the exact terms but not the negotiation. A revised proposal is placed on the table.

### `@PARLEYNOTHANKS` — Offer Declined

They decline the proposal without ending the audience. Another arrangement may still be possible.

### `@PARLEYCANCEL` — Negotiation Cancelled

The discussion is ended before agreement. No treaty changes hands, and both sides return to the strategic reality outside the room.

### `@PARLEYGOAWAY` — Audience Refused

They will not meet under present conditions. Whatever message you intended to deliver must wait—or be delivered by other means.

### `@PARLEYBUSY` — No Audience Available

Their court is occupied with other matters and refuses immediate talks. Try again when events have shifted.

### `@PARLEYOK` — Proceed

The other side is listening. State the terms clearly; ambiguity is expensive between nations.

## 17. Spaceflight & the Road to Alpha Centauri

### `@ASTRONAUTS` — A New Kind of Explorer

For most of history, the sky was a ceiling. Your astronauts have turned it into a frontier.

### `@COMPONENT` — Spaceship Component

Another critical system is ready for the interstellar vessel. Every component turns a national dream into an engineering fact.

### `@LAUNCHED` — The Spaceship Launches

Engines ignite beneath a machine built by an entire civilization. The ship leaves Earth carrying more than passengers: it carries your claim on the future.

### `@CENTAURI` — Voyage to Alpha Centauri

The ship is committed to the dark between stars. Nothing your civilization has ever built has traveled so far from home.

### `@CENTAURI3` — Approaching a New Sun

After years in the void, another star grows bright ahead. The destination that once existed only in equations now fills the forward windows.

### `@CENTAURI_BEATEN` — Another Civilization Reaches the Stars

A rival civilization has reached Alpha Centauri first. Humanity has entered a new chapter, but another banner stands on its first page.

### `rhyciv.space.structural` — Structural Integrity

The ship’s frame determines whether the vessel can endure the long acceleration and years between stars. Build enough structure to carry the dream safely.

### `rhyciv.space.propulsion` — Propulsion

Engines decide how quickly the ship can turn distance into arrival. More propulsion shortens the years between launch and a new world.

### `rhyciv.space.habitation` — Habitation

A starship is not only a machine. It must keep human beings alive, sane, fed, and hopeful across a journey longer than many lives once lasted.

### `rhyciv.space.success` — A New World

The long voyage is over. Beneath another sun, your people step onto a world untouched by human history—and begin history again.

## 18. Historians, Retirement, Victory & Defeat

### `@HISTORIANS` — The Historians Convene

The chroniclers pause to compare the great powers of the age. Armies, cities, discoveries, wealth, and wonders are weighed against one another.

### `@HISTORIES` — How the Age Remembers You

History is not a single verdict. Some remember conquerors, some builders, some liberators, some tyrants—and time has a way of changing the labels.

### `@HISTORYRANK` — Your Place in the Age

For now, the historians rank your civilization {rank}. The judgment is temporary; the game of history is still being played.

### `@PLANRETIRE` — Retire From Rule

To retire is to close the book before the last page. Your civilization will be judged as it stands now, with every unfinished dream left unfinished.

### `@RETIREDIE` — End Your Reign?

This decision ends your rule and sends the civilization to the historians. There is no next turn after this choice.

### `@KEEPPLAYING` — Continue Beyond the Verdict

The formal contest may be over, but the world still turns. Continue building, exploring, and reshaping the map for as long as you wish.

### `rhyciv.victory.conquest` — One Civilization Remains

The last rival banner has fallen. From coast to coast, no power remains capable of contesting your rule. The world has been conquered—now comes the harder question of what you build from victory.

### `rhyciv.victory.space` — Humanity Reaches Another Star

Your civilization has carried human life across the gulf between suns. Whatever happens on Earth from this day forward, your people have ensured that the human story is no longer bound to one world.

### `rhyciv.defeat.destroyed` — Your Civilization Has Fallen

The last city is gone and the last organized resistance has ended. The civilization you guided now passes from politics into memory.

### `rhyciv.defeat.time` — The Age Is Judged

The allotted span of history has run its course. The world remains unfinished, but historians must judge what you built with the time you were given.

## 19. Limits, Warnings & Confirmations

### `@TOOMANYCITIES` — The World Is Full of Cities

No more cities can be supported by the current world limits. Expand by developing, joining, or conquering what already exists.

### `@TOOMANYUNITS` — Too Many Units

The world has reached its supported unit limit. New forces cannot be created until others leave the field.

### `@REALLYQUIT` — Leave This World?

Quit the current game? Any unsaved decisions, battles, and discoveries since your last save will be lost.

### `@SAVEERROR` — The Chronicle Could Not Be Saved

The game state could not be written successfully. Choose another location or resolve the storage problem before relying on this save.

### `@LOADBADSAVE` — Damaged Chronicle

This saved game cannot be read safely. The record may be incomplete or corrupted.

### `@LOADOLDSAVE` — Older Save Format

This chronicle comes from an older version. It may be converted, but some details may not survive unchanged.

### `@LOADNEWSAVE` — Newer Save Format

This save was created by a newer version of the game and may contain information this build does not understand.

### `@WRONGVERSION` — Version Mismatch

This file belongs to a different game version. Update the game or use a compatible save.

### `@LOWMEMORY` — Resources Running Low

The game is running short of available memory. Save your progress before continuing into a larger battle or longer session.

## 20. Extra rhYciv Advisor-Style Prompts

### `rhyciv.advice.expansion` — Room to Grow

Unused land is opportunity, but expansion without roads, defense, and productive cities can turn strength into sprawl. Grow where the land can support what you intend to build.

### `rhyciv.advice.defense` — Guard What You Build

A prosperous city without defenders is an invitation. Walls, roads, veteran units, and strategic depth are cheaper than reconquest.

### `rhyciv.advice.science` — Ideas Compound

A discovery is not only a reward; it unlocks more questions. Civilizations that invest steadily in knowledge often find the future arriving on their side first.

### `rhyciv.advice.trade` — Distance Can Be Wealth

A road to another city is more than movement. Trade converts differences between places into shared prosperity—and sometimes into diplomatic influence.

### `rhyciv.advice.happiness` — Prosperity Must Be Felt

Citizens do not live inside your score. Luxury, temples, marketplaces, wonders, and wise government matter because a civilization is strongest when its people believe it is worth sustaining.

### `rhyciv.advice.production` — Every Shield Is a Choice

Production stored in one project cannot simultaneously build another. The art of rule is deciding what the future will need before the crisis arrives.

### `rhyciv.advice.reputation` — Your Word Is a Strategic Resource

Gold can be earned again and armies rebuilt. A reputation for keeping agreements takes far longer to recover once spent.

### `rhyciv.advice.navy` — The Sea Connects More Than It Divides

A strong navy protects trade, projects power, reveals coastlines, and makes distant wars possible. Ignore the sea only if your rivals do too.

### `rhyciv.advice.infrastructure` — Roads Are Invisible Power

The map favors the civilization that can move food, trade, workers, and armies where they are needed. Infrastructure turns territory into a functioning realm.

### `rhyciv.advice.wonders` — Great Works Are Strategic Bets

A Wonder concentrates years of production into one unique prize. Build it when its effect fits your civilization—not merely because the race exists.

### `rhyciv.advice.diplomacy` — Not Every Rival Must Be an Enemy

A civilization spared today can become a market, buffer, research partner, or ally tomorrow. Sometimes the strongest move is the war you do not need to fight.

### `rhyciv.advice.endgame` — Build for the World That Is Coming

The ancient game is expansion. The middle game is systems. The late game is leverage: technology, mobility, alliances, industry, and the ability to turn one advantage into several.

---

## Implementation Notes

1. **Keep legacy identifiers as aliases where convenient.** Many classic prompt names are terse but useful for mapping old gameplay states to the new localization layer.
2. **Prefer stable semantic keys for new code.** For new rhYciv-only events, use names such as `rhyciv.victory.conquest` or `rhyciv.advice.infrastructure` rather than engine-specific numeric IDs.
3. **Separate title from body in the localization data.** The headings above can become popup titles while the paragraph becomes the message body.
4. **Allow civilization-specific flavor later.** A future localization layer could override diplomacy tone by leader personality without changing the underlying event key.
5. **Do not hard-code grammatical gender or government titles into sentences.** Pass those values through localized variables so custom civilizations remain clean.
6. **Treat nuclear and civilian-harm events with gravity.** The text is intentionally serious so high-impact mechanics feel consequential.

**Total replacement prompt entries in this master draft: 295.**

This is intended as the master writing source. The next implementation step can split it into JSON, YAML, RESX, C# localization resources, or the project’s eventual translation format without rewriting the prose.