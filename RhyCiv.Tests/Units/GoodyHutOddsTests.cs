using Model.Core;
using Model.Core.GoodyHuts;
using Model.Core.GoodyHuts.Outcomes;
using Model.Core.Units;

namespace RhyCiv.Tests.Units;

/// <summary>
/// Civ II's five hut outcomes are equally likely, and an empty village is not one
/// of them.
/// <para>
/// This had six outcomes drawn evenly, one of which was the empty village, so a
/// sixth of huts held nothing before any of the others degraded into a
/// consolation of their own. The real distribution is tribes, gold, mercenaries,
/// scrolls and barbarians at one chance in five each, with tribes and barbarians
/// suppressed in favour of mercenaries near a city or before the finder has
/// founded one.
/// </para>
/// <para>
/// Measured in the study of hut outcomes at
/// https://apolyton.net/forum/civilization-series/civilization-i-and-civilization-ii/82184-a-study-of-hut-outcomes
/// </para>
/// </summary>
public class GoodyHutOddsTests
{
    private const int Samples = 20000;

    [Fact]
    public void TheFiveOutcomes_AreEquallyLikely()
    {
        var counts = Sample(nearSettlement: false);

        Assert.Equal(5, counts.Count);
        foreach (var (outcome, count) in counts)
        {
            Assert.InRange(count / (double)Samples, 0.17, 0.23);
        }
    }

    [Fact]
    public void NoHutIsEverSimplyEmpty()
    {
        var counts = Sample(nearSettlement: false);

        // The empty village is a consolation for an outcome that could not be
        // delivered, never a draw in its own right.
        Assert.False(counts.ContainsKey("AbandonedVillage"));
    }

    [Fact]
    public void NearASettlement_TribesAndBarbariansAreWithheld()
    {
        var counts = Sample(nearSettlement: true);

        Assert.False(counts.ContainsKey("AdvancedTribe"));
        Assert.False(counts.ContainsKey("Nomads"));
        Assert.False(counts.ContainsKey("Barbarians"));
    }

    [Fact]
    public void NearASettlement_MercenariesTakeTheirShare()
    {
        var counts = Sample(nearSettlement: true);

        // The ratio becomes 0:1:3:1:0, so three draws in five are mercenaries.
        Assert.InRange(counts.GetValueOrDefault("Mercenaries") / (double)Samples, 0.56, 0.64);
        Assert.InRange(counts.GetValueOrDefault("Gold") / (double)Samples, 0.17, 0.23);
    }

    [Fact]
    public void OutcomesSuppliedByACaller_AreStillDrawnEvenly()
    {
        // A scenario or a test that supplies its own outcomes gets an even draw,
        // because the weights describe the standard five and nothing else.
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

        Assert.InRange(empty / (double)Samples, 0.46, 0.54);
    }

    private static Dictionary<string, int> Sample(bool nearSettlement)
    {
        var hut = new GoodyHut(null, new Random(20260907));
        var counts = new Dictionary<string, int>();
        for (var draw = 0; draw < Samples; draw++)
        {
            var outcome = hut.Trigger(Unit(), eligibleAdvanceIndices: [3, 4, 5], nearSettlement);
            var key = outcome.OutcomeType;
            // A tribe on grassland founds a city and on anything else joins as
            // nomads; both are the same draw.
            counts[key] = counts.GetValueOrDefault(key) + 1;
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
