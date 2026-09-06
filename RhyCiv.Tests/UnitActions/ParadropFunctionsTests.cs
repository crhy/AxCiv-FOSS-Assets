using RhyCiv.Engine.UnitActions;
using RhyCiv.Engine.Enums;
using Model.Constants;
using Model.Core;
using Model.Core.Cities;
using Model.Core.GameRules;
using Model.Core.Mapping;
using Model.Core.Player;
using Model.Core.Units;
using Moq;

namespace RhyCiv.Tests.UnitActions;

public class ParadropFunctionsTests
{
    private const int ParadropFlag = 8;

    private static Map BuildMap()
    {
        var map = new Map(true, 0) { XDim = 20, YDim = 20, Tile = new Tile[20, 20] };
        for (var x = 0; x < 20; x++)
        {
            for (var y = 0; y < 20; y++)
            {
                map.Tile[x, y] = new Tile(x * 2 + y % 2, y,
                    new Terrain { Name = "Plains", Type = TerrainType.Plains, Specials = [] },
                    0, map, x, new bool[2]);
            }
        }

        return map;
    }

    private static Mock<IGame> BuildGame(Map map, int range = 10)
    {
        var game = new Mock<IGame>();
        var rules = new Rules { Advances = [], Improvements = [], Governments = [] };
        rules.Cosmic.MaxParadropRange = range;
        game.Setup(g => g.Rules).Returns(rules);
        game.Setup(g => g.Maps).Returns(new List<Map> { map });
        game.Setup(g => g.Players).Returns([new Mock<IPlayer>().Object, new Mock<IPlayer>().Object]);
        return game;
    }

    private static Unit Paratrooper(Civilization owner, Tile at, bool canParadrop = true)
    {
        var flags = new bool[15];
        flags[ParadropFlag] = canParadrop;
        var unit = new Unit
        {
            Owner = owner,
            TypeDefinition = new UnitDefinition { Move = 3, Flags = flags, Domain = UnitGas.Ground }
        };
        unit.CurrentLocation = at;
        unit.X = at.X;
        unit.Y = at.Y;
        return unit;
    }

    private static Tile OwnCityTile(Map map, Civilization owner, int x, int y)
    {
        var tile = map.Tile[x, y];
        tile.CityHere = new City { Owner = owner, Location = tile };
        return tile;
    }

    [Fact]
    public void CanParadrop_RequiresTheFlag()
    {
        var map = BuildMap();
        var civ = new Civilization { Id = 0 };
        var ordinary = Paratrooper(civ, OwnCityTile(map, civ, 5, 5), canParadrop: false);

        Assert.False(ParadropFunctions.CanParadrop(ordinary));
    }

    [Fact]
    public void CanParadrop_RequiresACityOrAirbase()
    {
        var map = BuildMap();
        var civ = new Civilization { Id = 0 };

        Assert.False(ParadropFunctions.CanParadrop(Paratrooper(civ, map.Tile[5, 5])));
        Assert.True(ParadropFunctions.CanParadrop(Paratrooper(civ, OwnCityTile(map, civ, 5, 5))));
    }

    [Fact]
    public void CanParadrop_RequiresAFullTurnOfMovement()
    {
        var map = BuildMap();
        var civ = new Civilization { Id = 0 };
        var unit = Paratrooper(civ, OwnCityTile(map, civ, 5, 5));
        unit.MovePointsLost = 1;

        Assert.False(ParadropFunctions.CanParadrop(unit));
    }

    [Fact]
    public void TryParadrop_MovesTheUnitAndSpendsItsTurn()
    {
        var map = BuildMap();
        var civ = new Civilization { Id = 0 };
        var origin = OwnCityTile(map, civ, 5, 5);
        var unit = Paratrooper(civ, origin);
        var game = BuildGame(map);

        var target = map.Tile[7, 7];
        target.SetVisible(civ.Id);

        Assert.True(ParadropFunctions.TryParadrop(game.Object, unit, target));
        Assert.Equal(target, unit.CurrentLocation);
        Assert.Contains(unit, target.UnitsHere);
        Assert.DoesNotContain(unit, origin.UnitsHere);
        Assert.Equal(unit.MaxMovePoints, unit.MovePointsLost);
    }

    [Fact]
    public void TryParadrop_RefusesATargetBeyondRange()
    {
        var map = BuildMap();
        var civ = new Civilization { Id = 0 };
        var origin = OwnCityTile(map, civ, 2, 2);
        var unit = Paratrooper(civ, origin);
        var game = BuildGame(map, range: 2);

        var target = map.Tile[15, 15];
        target.SetVisible(civ.Id);

        Assert.False(ParadropFunctions.TryParadrop(game.Object, unit, target));
        Assert.Equal(origin, unit.CurrentLocation);
    }

    [Fact]
    public void TryParadrop_RefusesUnseenGround()
    {
        var map = BuildMap();
        var civ = new Civilization { Id = 0 };
        var unit = Paratrooper(civ, OwnCityTile(map, civ, 5, 5));
        var game = BuildGame(map);

        // Never revealed to this player.
        Assert.False(ParadropFunctions.TryParadrop(game.Object, unit, map.Tile[7, 7]));
    }

    [Fact]
    public void TryParadrop_RefusesOccupiedAndSeaSquares()
    {
        var map = BuildMap();
        var civ = new Civilization { Id = 0 };
        var enemy = new Civilization { Id = 1 };
        var unit = Paratrooper(civ, OwnCityTile(map, civ, 5, 5));
        var game = BuildGame(map);

        var defended = map.Tile[6, 6];
        defended.SetVisible(civ.Id);
        var defender = new Unit
        {
            Owner = enemy,
            TypeDefinition = new UnitDefinition { Move = 3, Flags = new bool[15] }
        };
        defender.CurrentLocation = defended;
        Assert.False(ParadropFunctions.TryParadrop(game.Object, unit, defended));

        var ocean = map.Tile[7, 7];
        ocean.SetVisible(civ.Id);
        ocean.Terrain = new Terrain { Name = "Ocean", Type = TerrainType.Ocean, Specials = [] };
        Assert.False(ParadropFunctions.TryParadrop(game.Object, unit, ocean));
    }
}
