using Model.Core;
using Model.Core.GoodyHuts;
using Model.Core.GoodyHuts.Outcomes;
using Model.Core.Units;

namespace RhyCiv.Tests.Units;

/// <summary>
/// An empty village should be the exception, not the commonest thing a hut can
/// hold.
/// <para>
/// The six standard outcomes used to be drawn evenly, one chance in six each. On
/// top of that, several of the others degrade into a consolation of their own —
/// scrolls find nothing when there is no advance left to give, an advanced tribe
/// pays gold instead of founding a city when one is already near — so in play a
/// good third of huts came up with nothing in them, which is what "too many
/// empty goody huts" was.
/// </para>
/// </summary>
public class GoodyHutOddsTests
{
    private const int Samples = 6000;

    [Fact]
    public void TheEmptyVillage_IsUncommon()
    {
        var counts = Sample();
        var empty = counts.GetValueOrDefault("AbandonedVillage");

        // Weighted at 8 of 100. Well clear of the 1-in-6 it used to be, with room
        // for the sampling noise a few thousand draws still carry.
        Assert.InRange(empty / (double)Samples, 0.05, 0.12);
    }

    [Fact]
    public void EveryOutcome_StillHappens()
    {
        var counts = Sample();

        // Weighting must not quietly retire an outcome: a hut should still be able
        // to hold any of the six things it ever could.
        Assert.Equal(6, counts.Count);
        Assert.All(counts.Values, count => Assert.True(count > 0));
    }

    [Fact]
    public void GoldAndAdvances_AreTheUsualFind()
    {
        var counts = Sample();
        var worthwhile = counts.GetValueOrDefault("Gold") + counts.GetValueOrDefault("Scrolls");

        Assert.True(worthwhile > counts.GetValueOrDefault("AbandonedVillage") * 3,
            "Gold and scrolls together should dominate an empty village by a wide margin.");
    }

    [Fact]
    public void OutcomesSuppliedByACaller_AreStillDrawnEvenly()
    {
        // A scenario or a test that supplies its own outcomes gets an even draw,
        // because the weights describe the standard six and nothing else.
        var hut = new GoodyHut([new GoldOutcome(10), new AbandonedVillageOutcome()],
            new Random(7));

        var empty = 0;
        for (var draw = 0; draw < Samples; draw++)
        {
            if (hut.Trigger(Unit()).OutcomeType == "AbandonedVillage")
            {
                empty++;
            }
        }

        Assert.InRange(empty / (double)Samples, 0.44, 0.56);
    }

    private static Dictionary<string, int> Sample()
    {
        var hut = new GoodyHut(null, new Random(20260906));
        var counts = new Dictionary<string, int>();
        for (var draw = 0; draw < Samples; draw++)
        {
            var outcome = hut.Trigger(Unit(), eligibleAdvanceIndices: [3, 4, 5]);
            counts[outcome.OutcomeType] = counts.GetValueOrDefault(outcome.OutcomeType) + 1;
        }

        return counts;
    }

    /// <summary>
    /// A unit standing on grassland, so the tribe outcome takes its advanced-tribe
    /// branch and the scrolls outcome has advances left to give.
    /// </summary>
    private static Unit Unit()
    {
        var map = new Model.Core.Mapping.Map(true, 0)
        {
            Tile = new Model.Core.Mapping.Tile[1, 1], XDim = 1, YDim = 1
        };
        var terrain = new Model.Core.Mapping.Terrain
        {
            Type = Model.Core.Mapping.TerrainType.Grassland, Specials = []
        };
        var tile = new Model.Core.Mapping.Tile(0, 0, terrain, 1, map, 0, new bool[1]);
        map.Tile[0, 0] = tile;

        var owner = new Civilization { Id = 0, Advances = new bool[20] };
        return new Unit
        {
            Owner = owner,
            CurrentLocation = tile,
            TypeDefinition = new UnitDefinition { Flags = Enumerable.Repeat(false, 13).ToArray() }
        };
    }
}
