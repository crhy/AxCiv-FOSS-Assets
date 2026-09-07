using Model.Core.GoodyHuts.Outcomes;
using Model.Core.Units;

namespace Model.Core.GoodyHuts
{
    public class GoodyHut
    {
        private readonly List<GoodyHutOutcome> _outcomes;
        private readonly bool _standardSet;
        private readonly Random _random;

        /// <summary>
        /// Civ II's five hut outcomes are equally likely: tribes, gold,
        /// mercenaries, scrolls and barbarians, one chance in five each.
        /// <para>
        /// There is no "empty village" among them. It only ever appears as a
        /// consolation when one of the five cannot be delivered -- scrolls with no
        /// advance left to give, an advanced tribe with a city already next door.
        /// This implementation had it as a sixth outcome drawn as often as the
        /// rest, which is why so many huts came up with nothing in them.
        /// </para>
        /// <para>
        /// Measured from the study of hut outcomes linked below, which sampled
        /// version 2.4.2 by both reload and continuous-play testing.
        /// </para>
        /// </summary>
        private static readonly int[] OpenCountryWeights = [1, 1, 1, 1, 1];

        /// <summary>
        /// Near a city, or before a civilisation has founded one, the game will not
        /// hand out a wandering tribe or set barbarians on a player who could not
        /// survive them: both are suppressed and their share goes to mercenaries.
        /// The study calls these the No Cities Rule -- no cities and before turn 50
        /// -- and the Near City Rule, within four squares of the nearest city.
        /// </summary>
        private static readonly int[] SettledWeights = [0, 1, 3, 1, 0];

        public GoodyHut() : this(null, null)
        {
        }

        public GoodyHut(IEnumerable<GoodyHutOutcome>? outcomes, Random? random = null)
        {
            var supplied = outcomes?.ToList();
            _standardSet = supplied == null;
            _outcomes = supplied ?? new List<GoodyHutOutcome>
            {
                // The order the weights above are written in.
                new TribeOutcome(),
                new GoldOutcome(50),
                new MercenariesOutcome(),
                new ScrollsOutcome(),
                new BarbariansOutcome()
            };
            _random = random ?? new Random();
        }

        /// <summary>
        /// Picks an outcome. A caller that supplied its own set -- a scenario, or a
        /// test -- gets an even draw, because nothing here knows what those mean.
        /// </summary>
        private GoodyHutOutcome Draw(bool nearSettlement)
        {
            if (!_standardSet)
            {
                return _outcomes[_random.Next(0, _outcomes.Count)];
            }

            var weights = nearSettlement ? SettledWeights : OpenCountryWeights;
            var total = 0;
            for (var index = 0; index < _outcomes.Count && index < weights.Length; index++)
            {
                total += weights[index];
            }

            if (total <= 0)
            {
                return _outcomes[_random.Next(0, _outcomes.Count)];
            }

            var roll = _random.Next(0, total);
            for (var index = 0; index < _outcomes.Count && index < weights.Length; index++)
            {
                roll -= weights[index];
                if (roll < 0)
                {
                    return _outcomes[index];
                }
            }

            return _outcomes[^1];
        }

        /// <param name="nearSettlement">
        /// Whether the No Cities Rule or the Near City Rule applies: the finder has
        /// founded nothing yet and it is still early, or there is a city within four
        /// squares. Either suppresses tribes and barbarians.
        /// </param>
        public GoodyHutOutcomeResult Trigger(Unit unit,
            IReadOnlyList<int>? eligibleAdvanceIndices = null, bool nearSettlement = false)
        {
            if (_outcomes.Count == 0)
            {
                return new GoodyHutOutcomeResult("The village is empty.", true, "AbandonedVillage");
            }

            // https://apolyton.net/forum/civilization-series/civilization-i-and-civilization-ii/82184-a-study-of-hut-outcomes
            var outcome = Draw(nearSettlement);
            return outcome is ScrollsOutcome scrolls
                ? scrolls.ApplyOutcome(unit, eligibleAdvanceIndices, _random)
                : outcome.ApplyOutcome(unit);
        }
    }
}
