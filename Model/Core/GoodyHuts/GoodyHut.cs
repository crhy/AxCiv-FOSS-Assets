using Model.Core.GoodyHuts.Outcomes;
using Model.Core.Units;

namespace Model.Core.GoodyHuts
{
    public class GoodyHut
    {
        private readonly List<GoodyHutOutcome> _outcomes;
        private readonly IReadOnlyList<int> _weights;
        private readonly Random _random;

        /// <summary>
        /// How often each of the six standard outcomes comes up, in the order they
        /// are listed in the default set.
        /// <para>
        /// They used to be drawn uniformly, one chance in six each, which made an
        /// empty village the single most common thing a hut could be: every other
        /// outcome can degrade into a consolation as well -- scrolls find nothing
        /// when there is no advance left to give, an advanced tribe pays gold
        /// instead of founding a city when one is already near, a barbarian horde
        /// that cannot reach the village leaves only tracks -- so in practice
        /// closer to a third of huts came up empty. Weighting the draw makes the
        /// empty village the exception the study behind these outcomes describes,
        /// rather than the rule.
        /// </para>
        /// </summary>
        private static readonly int[] StandardWeights = [25, 8, 20, 15, 20, 12];

        public GoodyHut() : this(null, null)
        {
        }

        public GoodyHut(IEnumerable<GoodyHutOutcome>? outcomes, Random? random = null)
        {
            var supplied = outcomes?.ToList();
            _outcomes = supplied ?? new List<GoodyHutOutcome>
            {
                new GoldOutcome(50),
                new AbandonedVillageOutcome(),
                new ScrollsOutcome(),
                new MercenariesOutcome(),
                new TribeOutcome(),
                new BarbariansOutcome()
            };
            // A caller that supplies its own outcomes -- a scenario, or a test --
            // gets them drawn evenly, because nothing here knows what they mean.
            _weights = supplied == null
                ? StandardWeights
                : Enumerable.Repeat(1, _outcomes.Count).ToArray();
            _random = random ?? new Random();
        }

        /// <summary>
        /// Picks an outcome with the weights above. Falls back to an even draw if
        /// the weights and the outcomes have got out of step.
        /// </summary>
        private GoodyHutOutcome Draw()
        {
            if (_weights.Count != _outcomes.Count)
            {
                return _outcomes[_random.Next(0, _outcomes.Count)];
            }

            var total = _weights.Sum();
            if (total <= 0)
            {
                return _outcomes[_random.Next(0, _outcomes.Count)];
            }

            var roll = _random.Next(0, total);
            for (var index = 0; index < _outcomes.Count; index++)
            {
                roll -= _weights[index];
                if (roll < 0)
                {
                    return _outcomes[index];
                }
            }

            return _outcomes[^1];
        }

        public GoodyHutOutcomeResult Trigger(Unit unit, IReadOnlyList<int>? eligibleAdvanceIndices = null)
        {
            if (_outcomes.Count == 0)
            {
                return new GoodyHutOutcomeResult("The village is empty.", true, "AbandonedVillage");
            }

            // https://apolyton.net/forum/civilization-series/civilization-i-and-civilization-ii/82184-a-study-of-hut-outcomes
            var outcome = Draw();
            return outcome is ScrollsOutcome scrolls
                ? scrolls.ApplyOutcome(unit, eligibleAdvanceIndices, _random)
                : outcome.ApplyOutcome(unit);
        }
    }
}
