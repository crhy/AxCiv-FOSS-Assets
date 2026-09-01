using System.Collections.Generic;
using System.Linq;
using Civ2engine.Enums;
using Civ2engine.MapObjects;
using Model.Constants;
using Model.Core;
using Model.Core.Mapping;
using Model.Core.Units;

namespace Civ2engine.Units;

public static class UnitExtensions
{
    /// <summary>
    /// Defence bonus a city wall provides, matching the bundled ruleset. Used when
    /// a wonder stands in for the building itself.
    /// </summary>
    private const int FreeCityWallEffect = 200;

    public static double AttackFactor(this Unit attackUnit, Unit defendingUnit)
    {
        // Base attack factor from RULES
        double af = attackUnit.AttackBase;

        // Bonus for veteran units
        if (attackUnit.Veteran) af *= 1.5;

        // Partisan bonus against non-combat units
        if (attackUnit.TypeDefinition.Effects.TryGetValue(UnitEffect.Partisan, out var effect) && defendingUnit.AttackBase == 0)
        {
            af *= effect;
        }

        // The Great Wall doubles its owner's attack strength against barbarians.
        if (defendingUnit.Owner.PlayerType == PlayerType.Barbarians &&
            WonderFunctions.DoublesAttackAgainstBarbarians(attackUnit.Owner))
        {
            af *= 2;
        }

        return af;
    }


    public static int DefenseFactor(this Unit defendingUnit, Unit attackingUnit, Tile tile, int groundDefMultiplier)
    {
        //Carried units cannot be the defender
        if (defendingUnit.InShip != null) return 0;

        // Base defense factor from RULES
        decimal df = defendingUnit.DefenseBase;

        // Bonus for veteran units
        if (defendingUnit.Veteran) df *= 1.5m;

        // Pikemen-style bonus. Civ II applies x1.5 -- not x2 -- and only against a
        // land attacker with two movement points, one hit point and one firepower,
        // which is how the rules identify a mounted unit without a dedicated flag.
        if (defendingUnit.X2OnDefenseVersusHorse && IsMountedAttacker(attackingUnit))
        {
            df *= 1.5m;
        }

        // AEGIS-style bonus: x3 against aircraft, x5 against missiles.
        if (defendingUnit.X2OnDefenseVersusAir && attackingUnit.Domain == UnitGas.Air)
        {
            df *= attackingUnit.DestroyedAfterAttacking ? 5m : 3m;
        }

        // City walls bonus (applies only to land units)
        if (defendingUnit.Domain == UnitGas.Ground)
        {
            var bestGroundFactor = 0m;
            // Fortress bonus (Applies only to land units. Unit doesn't have to be fortified. Doesn't count if air unit is attacking.)
            if (groundDefMultiplier != 0 && attackingUnit.Domain != UnitGas.Air)
            {
                bestGroundFactor = df * groundDefMultiplier / 100;
            }

            // Fortified bonus
            if (defendingUnit.Order == (int)OrderType.Fortified)
            {
                var fortifiedFactor = df / 2m;
                if (fortifiedFactor > bestGroundFactor)
                {
                    bestGroundFactor = fortifiedFactor;
                }
            }

            //City walls (Note these are summed)
            if (tile.CityHere != null &&
                defendingUnit.Domain == UnitGas.Ground && !attackingUnit.NegatesCityWalls)
            {
                var wallEffect =
                    tile.CityHere.Improvements.Sum(i => i.Effects.GetValueOrDefault(Effects.Walled, 0));

                // The Great Wall stands in for city walls wherever a city has none.
                if (wallEffect < FreeCityWallEffect &&
                    WonderFunctions.HasFreeCityWalls(tile.CityHere.Owner))
                {
                    wallEffect = FreeCityWallEffect;
                }

                var totalWallDefence = wallEffect / 100m;
                if (totalWallDefence > bestGroundFactor)
                {
                    bestGroundFactor = totalWallDefence;
                }
            }

            df += bestGroundFactor;
        }

        // Helicopters are vulnerable to anti air
        else if (defendingUnit is { Domain: UnitGas.Air, FuelRange: 0 } && attackingUnit.CanAttackAirUnits)
        {
            df /= 2;
        }

        if (tile.CityHere != null)
        {
            if (attackingUnit.Domain == UnitGas.Air)
            {
                if (defendingUnit is { Domain: UnitGas.Air, FuelRange: 1 })
                {
                    // TODO: Message box about fighters scrambling for defence
                    if (attackingUnit.FuelRange != 1)
                    {
                        df *= 4;
                    }
                    else
                    {
                        df *= 2;
                    }
                }
                else
                {
                    int samBonus = 0;
                    int sdiBonus = 0;
                    foreach (var improvement in tile.CityHere.Improvements)
                    {
                        if (improvement.Effects.TryGetValue(Effects.AirDefence, out var sam))
                        {
                            samBonus += sam;
                        }

                        if (improvement.Effects.TryGetValue(Effects.MissileDefence, out var missile))
                        {
                            sdiBonus += missile;
                        }
                    }

                    // Effect of SAM batteries (only when attacked from air)
                    if (samBonus > 0)
                    {
                        //TODO: SAM message?
                        df += df * samBonus / 100m;
                    }

                    if (sdiBonus > 0 &&
                        attackingUnit.TypeDefinition.Effects.TryGetValue(UnitEffect.SDIVulnerable,
                            out var sdimulti) && sdimulti > 0)
                    {
                        df += df * sdiBonus * sdimulti / 100m;
                    }
                }
            }
            else if (attackingUnit.Domain == UnitGas.Sea)
            {
                var seaDefence =
                    tile.CityHere.Improvements.Sum(i => i.Effects.GetValueOrDefault(Effects.SeaDefence, 0));
                if (seaDefence > 0)
                {
                    //TODO: Coastal fortress message
                    df += df * seaDefence / 100m;
                }
            }
        }

        // Effect of terrain
        df *= tile.Defense;

        return (int)df;
    }

    /// <summary>
    /// Civ II has no explicit "is a horse" flag. A mounted attacker is a land unit
    /// with two movement points, a single hit point and a single firepower, which
    /// selects exactly the Horsemen-through-Cavalry line in the standard rules
    /// while still working for custom rulesets.
    /// </summary>
    private static bool IsMountedAttacker(Unit attackingUnit)
    {
        var definition = attackingUnit.TypeDefinition;
        return attackingUnit.Domain == UnitGas.Ground &&
               definition.AttackPerTurn == 2 &&
               definition.Hitp == 10 &&
               definition.Firepwr == 1;
    }
}