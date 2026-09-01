using System;
using System.Linq;
using System.Security.AccessControl;
using Model.Constants;
using Model.Core.Cities;
using Civ2engine.Enums;

namespace Civ2engine;

public static class CityResourcesExtensions
{
    private static decimal GetMultiplier(this City city, Effects effect)
    {
        var percent = city.Improvements
            .Where(i => i.Effects.ContainsKey(effect))
            .Select(b => b.Effects[effect]).Sum();

        // Wonders that behave like a multiplier building are not held in the
        // city's improvement list when they belong to another city, or when they
        // apply civilisation-wide, so they are added here.
        percent += WonderFunctions.GetMultiplierBonus(city, effect);

        return (100 + percent) / 100m;
    }

    private static int GetBaseScience(this City city)
    {
        var science = city.Trade * city.Owner.ScienceRate / 100;
        return science == 0 && city.Trade > 0 && city.Owner.ScienceRate > 0 ? 1 : science;
    }

    public static int GetScience(this City city)
    {
        var specialistScience = 3 * city.CountSpecialists(PeopleType.Scientist);
        return (int)((city.GetBaseScience() + specialistScience) * city.GetMultiplier(Effects.ScienceMultiplier));
    }

    private static int GetBaseLuxury(this City city)
    {
        return city.Trade * city.Owner.LuxRate / 100;
    }

    public static int GetLuxury(this City city)
    {
        return city.GetLuxury(0);
    }

    public static int GetLuxury(this City city, int extraBaseLuxury)
    {
        return (int)((city.GetBaseLuxury() + extraBaseLuxury) *
                     city.GetMultiplier(Effects.LuxMultiplier));
    }

    /// <summary>
    /// Formula should always round excess into tax
    /// </summary>
    public static int GetTax(this City city)
    {
        var specialistTax = 3 * city.CountSpecialists(PeopleType.Taxman);
        return (int)((city.Trade - GetBaseLuxury(city) - GetBaseScience(city) + specialistTax) *
                     GetMultiplier(city, Effects.TaxMultiplier));
    }

    public static int GetResourceValues(this City city, string name)
    {
        return name switch
        {
            "Science" => city.GetScience(),
            "Lux" => city.GetLuxury(),
            "Tax" => city.GetTax(),
            "Shields" => city.Production,
            "Food" => city.SurplusHunger,
            _ => throw new NotSupportedException()
        };
    }
    
    public static ResourceValues GetConsumableResourceValues(this City city, string resourceName)
    {
        switch (resourceName)
        {
            case "Food":
                return city.SurplusHunger > 0
                    ? new ResourceValues(consumption: city.Food, surplus: city.SurplusHunger, loss: 0)
                    : new ResourceValues(consumption: city.Food, surplus: 0, loss: -city.SurplusHunger);
            case "Shields":
                return new ResourceValues(consumption: city.Support, surplus: city.Production, loss: city.Waste);
            case "Trade":
                return new ResourceValues(consumption: city.Trade, surplus: 0, loss: city.Corruption);
            default:
                throw new NotImplementedException();
        }
    }
}
