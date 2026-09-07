using RhyCiv.Engine.Enums;
using RhyCiv.Engine.UnitActions;
using Model.Constants;
using Model.Core;
using Model.Core.GameRules;
using Model.Core.Mapping;
using Model.Core.Units;

namespace RhyCiv.Tests.Units;

/// <summary>
/// A road costs a third of a movement point to follow, and it costs that for
/// every unit.
/// <para>
/// It did not. Ground movement carried a rule that a unit whose whole allowance
/// was one movement point spent all of it on any move costing less than a full
/// point — which is every move along a road. So a Settlers, Warriors, Phalanx or
/// Musketeers got nothing at all from a road and walked it at one square a turn,
/// while a Horsemen alongside covered three. Reported as a settler on a road not
/// using a third of its movement.
/// </para>
/// </summary>
public class RoadMovementTests
{
    // Civ II's standard cosmic movement: a point is three fragments, a road is
    // one of them, and a railway is free.
    private const int Multiplier = 3;
    private const int RoadCost = 1;

    [Fact]
    public void AOneMoveUnitOnARoad_SpendsAThirdOfItsAllowance()
    {
        var (from, to) = RoadedPair();

        var cost = MovementFunctions.GroundMoveCost(OneMoveUnit(), to, from, Cosmic());

        Assert.Equal(RoadCost, cost);
    }

    [Fact]
    public void AOneMoveUnitOnARoad_CanMakeThreeMoves()
    {
        var (from, to) = RoadedPair();
        var unit = OneMoveUnit();
        var cosmic = Cosmic();

        var moves = 0;
        while (unit.MovePoints > 0 && moves < 10)
        {
            unit.MovePointsLost += MovementFunctions.GroundMoveCost(unit, to, from, cosmic);
            moves++;
        }

        Assert.Equal(3, moves);
    }

    [Fact]
    public void AFasterUnitOnARoad_IsUnaffectedByTheFix()
    {
        var (from, to) = RoadedPair();

        var cost = MovementFunctions.GroundMoveCost(Unit(moves: 2), to, from, Cosmic());

        Assert.Equal(RoadCost, cost);
    }

    [Fact]
    public void OffTheRoad_AMoveStillCostsAFullPoint()
    {
        var from = Tile(roaded: false);
        var to = Tile(roaded: false);

        var cost = MovementFunctions.GroundMoveCost(OneMoveUnit(), to, from, Cosmic());

        Assert.Equal(Multiplier, cost);
    }

    private static CosmicRules Cosmic() => new()
    {
        MovementMultiplier = Multiplier,
        RoadMovement = RoadCost,
        RailroadMovement = 0,
        RiverMovement = RoadCost,
        // Nothing here is alpine, so this must never be the cheapest option.
        AlpineMovement = Multiplier
    };

    private static Unit OneMoveUnit() => Unit(moves: 1);

    private static Unit Unit(int moves) => new()
    {
        Owner = new Civilization { Id = 0 },
        Dead = false,
        TypeDefinition = new UnitDefinition
        {
            Domain = UnitGas.Ground,
            Move = Multiplier * moves,
            Flags = Enumerable.Repeat(false, 13).ToArray()
        }
    };

    /// <summary>Two adjacent squares, both carrying a road.</summary>
    private static (Tile From, Tile To) RoadedPair() => (Tile(roaded: true), Tile(roaded: true));

    private static Tile Tile(bool roaded)
    {
        var map = new Map(true, 0) { Tile = new Tile[1, 1], XDim = 1, YDim = 1 };
        var terrain = new Terrain { Type = TerrainType.Grassland, MoveCost = 1, Specials = [] };
        var tile = new Tile(0, 0, terrain, 1, map, 0, new bool[1]);
        map.Tile[0, 0] = tile;

        if (roaded)
        {
            tile.EffectsList.Add(new ActiveEffect(
                new TerrainImprovementAction
                {
                    Target = ImprovementConstants.Movement,
                    Action = ImprovementActions.Set,
                    Value = RoadCost
                },
                source: ImprovementTypes.Road));
        }

        return tile;
    }
}
