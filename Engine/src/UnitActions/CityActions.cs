using System;
using System.Collections.Generic;
using System.Linq;
using Civ2engine.Advances;
using Civ2engine.Enums;
using Civ2engine.MapObjects;
using Civ2engine.Production;
using Civ2engine.Units;
using Model.Constants;
using Model.Core;
using Model.Core.Advances;
using Model.Core.Cities;
using Model.Core.Mapping;
using Model.Core.Units;

namespace Civ2engine.UnitActions
{
    public static class CityActions 
    {
        public static string GetCityName(Civilization civ , IGame game)
        {
            var cityCount = game.CitiesBuiltSoFar.GetValueOrDefault(civ, (byte) 0);
            var names = game.CityNames;
            var tribe = civ.TribeName.ToUpperInvariant();
            var civCityList = names.TryGetValue(tribe, out var tribeList) ? tribeList
                : names.TryGetValue("EXTRA", out var extraList) ? extraList
                : null;
            if (civCityList != null && cityCount < civCityList.Count)
            {
                return civCityList[cityCount];
            }
            
            return "Dummy Name";
        }

        /// <summary>
        /// Removes a unit from the game entirely. A unit standing in, or homed
        /// to, a city credits that city's current production with half its
        /// shield cost, as Civ II does when a unit is disbanded in a city;
        /// otherwise it is simply removed.
        /// </summary>
        public static void DisbandUnit(Unit unit, IGame game)
        {
            var city = unit.CurrentLocation?.CityHere ?? unit.HomeCity;
            if (city != null)
            {
                ApplyDisbandProductionCredit(city, unit, game.Rules.Cosmic.RowsShieldBox);
            }

            unit.Dead = true;
            unit.Owner.Units.Remove(unit);
        }

        /// <summary>
        /// Credits half a disbanded unit's shield cost toward the city's item in
        /// production, capped at that item's remaining cost. Only applies when
        /// the unit is standing in, or homed to, the crediting city.
        /// </summary>
        public static void ApplyDisbandProductionCredit(City city, Unit unit, int shieldRows)
        {
            if (unit.HomeCity != city && unit.CurrentLocation != city.Location)
            {
                return;
            }

            var totalCost = Math.Max(1, city.ItemInProduction.Cost);
            var shieldCredit = Math.Max(1, unit.TypeDefinition.Cost / 2);
            city.ShieldsProgress = Math.Min(totalCost, city.ShieldsProgress + shieldCredit);

            var queuedItem = city.ConstructionQueue.Current;
            if (queuedItem != null)
            {
                queuedItem.RemainingCost = Math.Max(0, queuedItem.RemainingCost - shieldCredit);
            }
        }

        public static City BuildCity(Unit unit, IGame game, string name)
        {
            var tile = unit.CurrentLocation;
            var initialProduction = ProductionOrder.GetAll(game.Rules).MinBy(i => i.Cost);
            var city = new City
            {
                Location = tile,
                Name = name,
                X = tile.X,
                Y = tile.Y,
                Owner = unit.Owner,
                Size = 1,
                ItemInProduction = initialProduction!,
                WhoBuiltIt = unit.Owner,
            };
            // A city replaces whatever village/hut marker was on this tile.  Settlers can start
            // on a goody hut, and Civ2 allows founding there; if the hut is not cleared the map
            // renderer continues to draw the hut instead of the newly founded city.
            tile.HasGoodieHut = false;

            tile.WorkedBy = city;
            tile.CityHere = city;
            game.AllCities.Add(tile.CityHere);
            unit.Owner.Cities.Add(tile.CityHere);

            game.SetImprovementsForCity(city);
            
            if (unit.Owner.Cities.Count == 1)
            {
                var capitalImprovement = ProductionPossibilities.FindByEffect(city.Owner.Id, Effects.Capital)
                                         ?? game.Rules.Improvements.Where(i =>
                                             i.Effects.ContainsKey(Effects.Capital) &&
                                             city.Owner.AllowedAdvanceGroups[
                                                 game.Rules.Advances[i.Prerequisite].AdvanceGroup] !=
                                             AdvanceGroupAccess.Prohibited).MinBy(i => i.Cost);
                if (capitalImprovement != null)
                {
                    city.AddImprovement(capitalImprovement);
                }
            }
            game.History.CityBuilt(tile.CityHere);
            int currentCityCount = game.CitiesBuiltSoFar.GetValueOrDefault(city.Owner, 0);
            game.CitiesBuiltSoFar[city.Owner] = currentCityCount + 1;

            city.AutoAddDistributionWorkers(game.Rules);
            city.CalculateOutput(city.Owner.Government, game);

            unit.Dead = true;
            unit.MovePointsLost = unit.MovePoints;

            if (tile.Fertility != -2)
            {
                tile.Map.AdjustFertilityForCity(tile);
            }

            game.UpdateTiles(new List<Tile> {tile});

            return city;
        }
    }
}