using System;
using System.Linq;
using Model.Core;

namespace Civ2engine.Statistics
{
    public static class Power
    {
        /// <summary>
        /// Power rating (also used for power graph)
        /// Calculated as a sum of:
        /// - rating from no of researched techs, including future techs (every 8 techs increase rating by 3)
        /// - rating from population (rating = sum of cities sizes)
        /// - rating from gold (every 256 gold increases rating by 1)
        /// In original limited to max 255. 
        /// Some strange behavior in the original, e.g. score reset to 0 once score from advances reaches 100.
        /// </summary>
        /// <param name="game"></param>
        public static void CalculatePowerRatings(Game game)
        {
            foreach (var civilization in game.AllCivilizations)
            {
                civilization.PowerRating.Add(CalculateRating(civilization));
            }

            AssignPowerRanks(game);
        }

        public static void AssignPowerRanks(Game game)
        {
            foreach (var pair
                in game.AllCivilizations.OrderBy(CalculateRating).ThenBy(c => c.Id)
                    .Select((civilization, i) => new { civilization, i }))
            {
                pair.civilization.PowerRank = pair.i;
            }
        }

        public static int CalculateRating(Civilization civilization)
        {
            var technologies = civilization.Advances.Count(discovered => discovered) +
                               civilization.FutureTechCount;
            var rating = civilization.Cities.Sum(city => city.Size) +
                         civilization.Money / 256 +
                         technologies * 3 / 8;
            return Math.Clamp(rating, 0, 255);
        }
    }
}
