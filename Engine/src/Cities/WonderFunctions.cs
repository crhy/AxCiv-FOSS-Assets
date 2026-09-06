using System.Collections.Generic;
using System.Linq;
using RhyCiv.Engine.Enums;
using Model.Constants;
using Model.Core;
using Model.Core.Cities;

namespace RhyCiv.Engine;

/// <summary>
/// Shared wonder lookups for the systems that apply wonder effects.
/// A wonder stops working for its owner once the advance that obsoletes it has
/// been researched, which is how Civ II retires the early wonders.
/// </summary>
public static class WonderFunctions
{
    /// <summary>
    /// The city holding a still-active copy of <paramref name="wonder"/> for this
    /// civilisation, or null when it is unbuilt or has been obsoleted.
    /// </summary>
    public static City? FindActiveWonder(Civilization civilization, ImprovementType wonder)
    {
        // A city that is not yet attached to a civilisation (freshly constructed
        // or mid-transfer) owns no wonders.
        if (civilization?.Cities == null)
        {
            return null;
        }

        var wonderCity = civilization.Cities.FirstOrDefault(c => c.ImprovementExists((int)wonder));
        if (wonderCity == null)
        {
            return null;
        }

        var improvement = wonderCity.Improvements.First(i => i.Type == (int)wonder);
        return improvement.ExpiresAt >= 0 && HasAdvance(civilization, improvement.ExpiresAt)
            ? null
            : wonderCity;
    }

    public static bool OwnsActiveWonder(Civilization civilization, ImprovementType wonder) =>
        FindActiveWonder(civilization, wonder) != null;

    /// <summary>
    /// True when this specific city holds a still-active copy of the wonder.
    /// </summary>
    public static bool CityHasActiveWonder(City city, ImprovementType wonder) =>
        FindActiveWonder(city.Owner, wonder) == city;

    /// <summary>
    /// Percentage points a city's wonders contribute to one of the multiplier
    /// effects, on top of its own improvements.
    /// </summary>
    public static int GetMultiplierBonus(City city, Effects effect)
    {
        if (effect == Effects.ProductionMultiplier)
        {
            // Hoover Dam counts as a Hydro Plant in every city its owner holds.
            return OwnsActiveWonder(city.Owner, ImprovementType.HooverDam) ? 25 : 0;
        }

        if (effect != Effects.ScienceMultiplier)
        {
            return 0;
        }

        var bonus = 0;

        // Copernicus' Observatory raises science in its own city by half.
        if (CityHasActiveWonder(city, ImprovementType.CoperObserv))
        {
            bonus += 50;
        }

        // Isaac Newton's College doubles science in its own city.
        if (CityHasActiveWonder(city, ImprovementType.InCollege))
        {
            bonus += 100;
        }

        // The SETI Program counts as a research lab in every city.
        if (OwnsActiveWonder(city.Owner, ImprovementType.SetiProgr))
        {
            bonus += 50;
        }

        return bonus;
    }

    /// <summary>
    /// Extra shields King Richard's Crusade adds to each worked tile of its city.
    /// </summary>
    public static int GetWorkedTileShieldBonus(City city) =>
        CityHasActiveWonder(city, ImprovementType.KrCrusade) ? 1 : 0;

    /// <summary>
    /// Extra trade the Colossus adds to each worked tile of its city that already
    /// produces trade.
    /// </summary>
    public static int GetProducingTileTradeBonus(City city) =>
        CityHasActiveWonder(city, ImprovementType.Colossus) ? 1 : 0;

    /// <summary>
    /// Food kept on growth granted by wonders. The Pyramids act as a granary in
    /// every city their owner holds.
    /// </summary>
    public static int GetFoodStorageBonus(City city) =>
        OwnsActiveWonder(city.Owner, ImprovementType.Pyramids) ? 50 : 0;

    /// <summary>
    /// Extra whole movement points wonders grant to a sea unit: one from the
    /// Lighthouse and two from Magellan's Expedition.
    /// </summary>
    public static int GetSeaMovementBonus(Civilization civilization)
    {
        var bonus = 0;
        if (OwnsActiveWonder(civilization, ImprovementType.Lighthouse))
        {
            bonus += 1;
        }

        if (OwnsActiveWonder(civilization, ImprovementType.MagellExped))
        {
            bonus += 2;
        }

        return bonus;
    }

    /// <summary>
    /// The Great Wall acts as city walls in every city its owner holds.
    /// </summary>
    public static bool HasFreeCityWalls(Civilization civilization) =>
        OwnsActiveWonder(civilization, ImprovementType.GreatWall);

    /// <summary>
    /// The Great Wall also doubles its owner's attack strength against barbarians.
    /// </summary>
    public static bool DoublesAttackAgainstBarbarians(Civilization civilization) =>
        OwnsActiveWonder(civilization, ImprovementType.GreatWall);

    /// <summary>
    /// Sun Tzu's War Academy makes every ground unit its owner builds a veteran.
    /// </summary>
    public static bool ProducesVeterans(Civilization civilization, UnitGas domain) =>
        domain == UnitGas.Ground && OwnsActiveWonder(civilization, ImprovementType.WarAcademy);

    /// <summary>
    /// A. Smith's Trading Co. pays the upkeep of every building that costs one.
    /// </summary>
    public static bool PaysUpkeepFor(Civilization civilization, int upkeep) =>
        upkeep == 1 && OwnsActiveWonder(civilization, ImprovementType.TradingCompany);

    /// <summary>
    /// Advances the Great Library hands its owner: every advance already known to
    /// at least two other civilisations that the owner does not yet have. Civ II
    /// keeps the owner level with the field this way until Electricity obsoletes
    /// the wonder, which <see cref="FindActiveWonder"/> already accounts for.
    /// Barbarians are not counted as a civilisation for this purpose.
    /// </summary>
    public static IEnumerable<int> GreatLibraryAdvances(Civilization owner,
        IEnumerable<Civilization> allCivilizations, int advanceCount)
    {
        var others = allCivilizations
            .Where(c => c != owner && c.PlayerType != PlayerType.Barbarians)
            .ToList();

        for (var advance = 0; advance < advanceCount; advance++)
        {
            if (HasAdvance(owner, advance))
            {
                continue;
            }

            if (others.Count(c => HasAdvance(c, advance)) >= 2)
            {
                yield return advance;
            }
        }
    }

    private static bool HasAdvance(Civilization civilization, int advance) =>
        advance >= 0 && advance < civilization.Advances.Length && civilization.Advances[advance];
}
