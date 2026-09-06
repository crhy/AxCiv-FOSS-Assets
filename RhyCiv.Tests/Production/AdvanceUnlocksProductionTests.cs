using RhyCiv.Engine;
using RhyCiv.Engine.Advances;
using RhyCiv.Engine.Production;
using RhyCiv.Engine.UnitActions;
using RhyCiv.Tests.TestFiles;
using Model.Constants;
using Model.Core;
using Model.Core.Cities;
using Model.Core.Mapping;
using Model.Core.Production;
using Model.Core.Units;

namespace RhyCiv.Tests.Production;

/// <summary>
/// Discovering an advance has to put whatever it unlocks into the city's build
/// list. That list is built once at game start and changed only by
/// AdvanceFunctions, so an advance that failed to update it would leave the
/// player unable to build what they had just researched -- which is what issue
/// #91, "Discovered Ceremonial Burial, can't build temple?", describes.
/// </summary>
public class AdvanceUnlocksProductionTests
{
    [Fact]
    public void DiscoveringAnAdvance_AddsWhatItUnlocksToTheBuildList()
    {
        var (game, _, rules) = CleanRoomGameFactory.CreateGame();
        var civ = game.GetPlayerCiv;
        var city = FoundCity(game, civ);

        // An improvement gated behind an advance this civ does not have, that the
        // city could actually build once it does. Wonders and the Palace are
        // excluded: a city already holding one is refused a second, so they would
        // stay out of the build list for reasons that have nothing to do with the
        // advance being tested.
        var locked = ProductionOrder.GetAll(rules)
            .OfType<BuildingProductionOrder>()
            .First(order => order.RequiredTech >= 0
                            && !Knows(civ, order.RequiredTech)
                            && !order.Improvement.IsWonder
                            && city.Improvements.All(built => built.Name != order.Improvement.Name));

        Assert.DoesNotContain(ProductionPossibilities.GetAllowedProductionOrders(city),
            order => order.Title == locked.Title);

        game.GiveAdvance(locked.RequiredTech, civ);

        Assert.Contains(ProductionPossibilities.GetAllowedProductionOrders(city),
            order => order.Title == locked.Title);
    }

    [Fact]
    public void TheTemple_BecomesBuildableOnceItsAdvanceIsKnown()
    {
        var (game, _, rules) = CleanRoomGameFactory.CreateGame();
        var civ = game.GetPlayerCiv;
        var city = FoundCity(game, civ);

        var temple = ProductionOrder.GetAll(rules)
            .OfType<BuildingProductionOrder>()
            .FirstOrDefault(order => order.Title.Contains("Temple", StringComparison.OrdinalIgnoreCase));
        Assert.NotNull(temple);

        if (!Knows(civ, temple.RequiredTech))
        {
            game.GiveAdvance(temple.RequiredTech, civ);
        }

        Assert.Contains(ProductionPossibilities.GetAllowedProductionOrders(city),
            order => order.Title == temple.Title);
    }

    [Fact]
    public void TheBuildList_KeepsAStableOrderWhenAnAdvanceUnlocksSomething()
    {
        // The list is appended to as advances are discovered, so without an
        // explicit order a newly unlocked item lands at the bottom instead of in
        // its usual place -- which is how a buildable Temple goes unnoticed.
        var (game, _, rules) = CleanRoomGameFactory.CreateGame();
        var civ = game.GetPlayerCiv;
        var city = FoundCity(game, civ);

        var locked = ProductionOrder.GetAll(rules)
            .OfType<BuildingProductionOrder>()
            .First(order => order.RequiredTech >= 0
                            && !Knows(civ, order.RequiredTech)
                            && !order.Improvement.IsWonder
                            && city.Improvements.All(built => built.Name != order.Improvement.Name));

        game.GiveAdvance(locked.RequiredTech, civ);
        var allowed = ProductionPossibilities.GetAllowedProductionOrders(city);

        // Units come before improvements, and each group follows the ruleset.
        var keys = allowed.Select(order => ((int)order.Type, order.ImageIndex)).ToList();
        Assert.Equal(keys.OrderBy(k => k.Item1).ThenBy(k => k.Item2).ToList(), keys);

        // ...and the new item is not simply stuck on the end.
        var index = allowed.ToList().FindIndex(order => order.Title == locked.Title);
        Assert.InRange(index, 0, allowed.Count - 2);
    }

    /// <summary>
    /// A new civilisation starts with an empty Advances array rather than one
    /// entry per advance in the ruleset -- GiveAdvance grows it on first use -- so
    /// an index into it is only meaningful after a bounds check.
    /// </summary>
    private static bool Knows(Civilization civ, int advanceIndex) =>
        advanceIndex >= 0 && advanceIndex < civ.Advances.Length && civ.Advances[advanceIndex];

    /// <summary>
    /// A freshly generated game has settlers but no cities, and the build list is
    /// per-city, so one has to exist before it can be asked what it may build.
    /// </summary>
    private static City FoundCity(Game game, Civilization civ)
    {
        var settler = civ.Units.First(unit =>
            unit.AiRole == AiRoleType.Settle
            && !unit.CurrentLocation.Terrain.Impassable
            && unit.CurrentLocation.Type != TerrainType.Ocean);
        return CityActions.BuildCity(settler, game, "Testopolis");
    }
}
