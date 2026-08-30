using Model.Core.Units;

namespace Model.Core.GoodyHuts.Outcomes
{
    public class ScrollsOutcome : GoodyHutOutcome
    {
        public string Name => "Scrolls";
        public string Description => "You have discovered scrolls of ancient wisdom.";

        public override GoodyHutOutcomeResult ApplyOutcome(Unit unit)
        {
            return ApplyOutcome(unit, null, null);
        }

        internal GoodyHutOutcomeResult ApplyOutcome(Unit unit, IReadOnlyList<int>? eligibleAdvanceIndices,
            Random? random)
        {
            var advances = unit.Owner.Advances;
            if (advances == null || advances.Length == 0)
            {
                return new GoodyHutOutcomeResult(Description, false, "Scrolls");
            }

            var candidates = (eligibleAdvanceIndices ?? Enumerable.Range(0, advances.Length))
                .Where(index => index >= 0 && index < advances.Length && !advances[index])
                .Distinct()
                .ToArray();
            if (candidates.Length == 0)
            {
                return new GoodyHutOutcomeResult("Your sages find no new secrets in the ancient scrolls.", false, "Scrolls");
            }

            var advanceIndex = random == null ? candidates[0] : candidates[random.Next(candidates.Length)];
            return new GoodyHutOutcomeResult(Description, true, "Scrolls")
            {
                AdvanceIndex = advanceIndex
            };
        }
    }
}
