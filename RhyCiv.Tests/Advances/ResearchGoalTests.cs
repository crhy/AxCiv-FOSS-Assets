using RhyCiv.Engine.Advances;
using RhyCiv.Tests.Mocks;
using Model.Core;
using Model.Core.Advances;
using Model.Core.GameRules;

namespace RhyCiv.Tests.Advances;

/// <summary>
/// The technology goal: a civilisation names an advance several steps away, and
/// the research chooser offers only the advances that lead towards it. Whether it
/// can say "nothing leads there yet" correctly matters as much as the list, since
/// that is the answer whenever the route is blocked behind something the
/// civilisation cannot start.
/// </summary>
public class ResearchGoalTests
{
    // A small tree:  Alphabet -> Writing -> Literacy,  Bronze (no prerequisites),
    // and Philosophy, which needs both Literacy and Bronze. Mysticism sits behind
    // Ceremonial Burial, which this civilisation is barred from, so it stands for a
    // goal that is real but has no route to it.
    private const int Alphabet = 0;
    private const int Writing = 1;
    private const int Literacy = 2;
    private const int Bronze = 3;
    private const int Philosophy = 4;
    private const int CeremonialBurial = 5;
    private const int Mysticism = 6;

    [Fact]
    public void NeededFor_CollectsTheWholeChain()
    {
        var (game, civ) = Board();

        var needed = AdvanceFunctions.AdvancesNeededFor(game, civ, Philosophy);

        Assert.Equal(new HashSet<int> { Alphabet, Writing, Literacy, Bronze, Philosophy }, needed);
    }

    [Fact]
    public void NeededFor_StopsAtWhatIsAlreadyKnown()
    {
        var (game, civ) = Board();
        Learn(civ, Alphabet, Writing);

        var needed = AdvanceFunctions.AdvancesNeededFor(game, civ, Philosophy);

        Assert.Equal(new HashSet<int> { Literacy, Bronze, Philosophy }, needed);
    }

    [Fact]
    public void NeededFor_IsEmptyForAGoalAlreadyReached()
    {
        var (game, civ) = Board();
        Learn(civ, Philosophy);

        Assert.Empty(AdvanceFunctions.AdvancesNeededFor(game, civ, Philosophy));
    }

    [Fact]
    public void StepsToward_KeepsOnlyTheAdvancesOnTheRoute()
    {
        var (game, civ) = Board();
        var options = AdvanceFunctions.CalculateAvailableResearch(game, civ);

        // Nothing is known, so Alphabet and Bronze are the two that can be begun.
        Assert.Equal(new[] { Alphabet, Bronze }, options.Select(a => a.Index).OrderBy(i => i));

        var steps = AdvanceFunctions.StepsToward(game, civ, Literacy, options);

        Assert.Equal(new[] { Alphabet }, steps.Select(a => a.Index));
    }

    [Fact]
    public void StepsToward_OffersTheGoalItselfOnceItIsWithinReach()
    {
        var (game, civ) = Board();
        Learn(civ, Alphabet, Writing);

        var steps = AdvanceFunctions.StepsToward(game, civ, Literacy,
            AdvanceFunctions.CalculateAvailableResearch(game, civ));

        Assert.Equal(new[] { Literacy }, steps.Select(a => a.Index));
    }

    [Fact]
    public void StepsToward_IsEmptyWhenNothingAvailableLeadsToTheGoal()
    {
        var (game, civ) = Board();

        // Mysticism needs Ceremonial Burial, which this civilisation is barred from,
        // so nothing it can begin now brings Mysticism any nearer. This is the case
        // the chooser has to say out loud rather than show as an empty list.
        var steps = AdvanceFunctions.StepsToward(game, civ, Mysticism,
            AdvanceFunctions.CalculateAvailableResearch(game, civ));

        Assert.Empty(steps);
    }

    [Fact]
    public void NeededFor_StillNamesARouteThatCannotBeTaken()
    {
        var (game, civ) = Board();

        // The advances are still reported: it is the caller's business to notice
        // that none of them can be started.
        Assert.Equal(new HashSet<int> { CeremonialBurial, Mysticism },
            AdvanceFunctions.AdvancesNeededFor(game, civ, Mysticism));
    }

    [Fact]
    public void PossibleGoals_LeavesOutWhatIsAlreadyKnown()
    {
        var (game, civ) = Board();
        Learn(civ, Alphabet);

        var goals = AdvanceFunctions.PossibleResearchGoals(game, civ);

        Assert.DoesNotContain(goals, a => a.Index == Alphabet);
        Assert.Contains(goals, a => a.Index == Philosophy);
    }

    [Fact]
    public void PossibleGoals_LeavesOutWhatCannotBeResearchedAtAll()
    {
        var (game, civ) = Board();

        Assert.DoesNotContain(AdvanceFunctions.PossibleResearchGoals(game, civ),
            a => a.Index == CeremonialBurial);
    }

    private static (MockGame Game, Civilization Civ) Board()
    {
        var advances = new[]
        {
            Advance("Alphabet", Alphabet),
            Advance("Writing", Writing, Alphabet),
            Advance("Literacy", Literacy, Writing),
            Advance("Bronze Working", Bronze),
            Advance("Philosophy", Philosophy, Literacy, Bronze),
            Advance("Ceremonial Burial", CeremonialBurial, group: 1),
            Advance("Mysticism", Mysticism, CeremonialBurial),
        };

        var rules = new Rules { Advances = advances };
        var civ = new Civilization
        {
            Id = 1,
            Advances = new bool[advances.Length],
            AllowedAdvanceGroups = [AdvanceGroupAccess.CanResearch, AdvanceGroupAccess.Prohibited],
        };

        return (new MockGame { Rules = rules }, civ);
    }

    private static void Learn(Civilization civ, params int[] advances)
    {
        foreach (var advance in advances)
        {
            civ.Advances[advance] = true;
        }
    }

    private static Advance Advance(string name, int index, int prereq1 = -1, int prereq2 = -1,
        int group = 0) =>
        new() { Name = name, Index = index, Prereq1 = prereq1, Prereq2 = prereq2, AdvanceGroup = group };
}
