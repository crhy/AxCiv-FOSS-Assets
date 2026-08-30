using Civ2engine.NewGame;
using Model.Core;
using Model.Core.Mapping;
using Model.Core.Units;

namespace Core.Tests.NewGame;

public class NewGameInitialisationTests
{
    [Fact]
    public void HumanStartsWithTwoSettlersAndAiStartsWithOne()
    {
        var map = new Map(false, 0) { XDim = 2, YDim = 2, Tile = new Tile[2, 2] };
        var plains = new Terrain { Type = TerrainType.Plains, Name = "Plains" };
        var humanStart = new Tile(0, 0, plains, 1, map, 0, new bool[3]);
        var aiStart = new Tile(1, 1, plains, 1, map, 0, new bool[3]);
        var human = new Civilization { Id = 1, PlayerType = PlayerType.Local };
        var ai = new Civilization { Id = 2, PlayerType = PlayerType.Ai };
        var settlerType = new UnitDefinition { Type = 0, Name = "Settlers", Flags = new bool[20] };

        var units = NewGameInitialisation.CreateStartingUnits(
            [(human, humanStart), (ai, aiStart)], settlerType, human.Id);

        Assert.Equal(3, units.Count);
        Assert.Equal(2, units.Count(unit => unit.Owner == human));
        Assert.Single(units, unit => unit.Owner == ai);
        Assert.Equal([0, 1, 2], units.Select(unit => unit.Id));
        Assert.All(units.Where(unit => unit.Owner == human), unit => Assert.Same(humanStart, unit.CurrentLocation));
        Assert.Equal(2, humanStart.UnitsHere.Count);
    }
}
