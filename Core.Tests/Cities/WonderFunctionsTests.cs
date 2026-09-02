using System.Linq;
using Civ2engine;
using Model.Core;

namespace Core.Tests.Cities;

public class WonderFunctionsTests
{
    private static Civilization Civ(PlayerType type, params bool[] advances) =>
        new() { PlayerType = type, Advances = advances };

    [Fact]
    public void GreatLibraryAdvances_GrantsAdvanceKnownToTwoOtherCivs()
    {
        var owner = Civ(PlayerType.Local, false, false, false);
        var rivalA = Civ(PlayerType.Ai, false, true, false);
        var rivalB = Civ(PlayerType.Ai, false, true, false);

        var granted = WonderFunctions
            .GreatLibraryAdvances(owner, new[] { owner, rivalA, rivalB }, 3)
            .ToList();

        Assert.Equal(new[] { 1 }, granted);
    }

    [Fact]
    public void GreatLibraryAdvances_IgnoresAdvanceKnownToOnlyOneOtherCiv()
    {
        var owner = Civ(PlayerType.Local, false, false);
        var rivalA = Civ(PlayerType.Ai, false, true);
        var rivalB = Civ(PlayerType.Ai, false, false);

        var granted = WonderFunctions
            .GreatLibraryAdvances(owner, new[] { owner, rivalA, rivalB }, 2)
            .ToList();

        Assert.Empty(granted);
    }

    [Fact]
    public void GreatLibraryAdvances_SkipsAdvancesTheOwnerAlreadyHas()
    {
        var owner = Civ(PlayerType.Local, false, true);
        var rivalA = Civ(PlayerType.Ai, false, true);
        var rivalB = Civ(PlayerType.Ai, false, true);

        var granted = WonderFunctions
            .GreatLibraryAdvances(owner, new[] { owner, rivalA, rivalB }, 2)
            .ToList();

        Assert.Empty(granted);
    }

    [Fact]
    public void GreatLibraryAdvances_DoesNotCountBarbariansAsACivilisation()
    {
        var owner = Civ(PlayerType.Local, false, false);
        var rival = Civ(PlayerType.Ai, false, true);
        var barbarians = Civ(PlayerType.Barbarians, false, true);

        var granted = WonderFunctions
            .GreatLibraryAdvances(owner, new[] { owner, rival, barbarians }, 2)
            .ToList();

        Assert.Empty(granted);
    }

    [Fact]
    public void GreatLibraryAdvances_ReturnsEveryQualifyingAdvance()
    {
        var owner = Civ(PlayerType.Local, false, false, false, true);
        var rivalA = Civ(PlayerType.Ai, true, false, true, true);
        var rivalB = Civ(PlayerType.Ai, true, false, true, true);

        var granted = WonderFunctions
            .GreatLibraryAdvances(owner, new[] { owner, rivalA, rivalB }, 4)
            .ToList();

        // 0 and 2 are known to both rivals and missing from the owner; 1 is known
        // to nobody and 3 the owner already holds.
        Assert.Equal(new[] { 0, 2 }, granted);
    }
}
