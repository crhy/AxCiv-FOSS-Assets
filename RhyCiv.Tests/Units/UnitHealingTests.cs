using RhyCiv.Engine;
using RhyCiv.Engine.Enums;
using Model.Core;
using Model.Core.Mapping;
using Model.Core.Units;

namespace RhyCiv.Tests.Units;

/// <summary>
/// Covers <see cref="Game.HealRestingUnit"/>, and in particular the rule that a
/// unit put to sleep to recover wakes when it is whole again while a unit that
/// was already healthy when told to sleep stays asleep until the player wakes it.
/// </summary>
public class UnitHealingTests
{
    [Fact]
    public void SleepingUnit_WakesWhenFullyHealed()
    {
        var unit = RestingUnit(hitPointsLost: 1, OrderType.Sleep);

        Game.HealRestingUnit(unit);

        Assert.Equal(0, unit.HitPointsLost);
        Assert.Equal((int)OrderType.NoOrders, unit.Order);
    }

    [Fact]
    public void SleepingUnit_StaysAsleepWhileStillDamaged()
    {
        var unit = RestingUnit(hitPointsLost: 5, OrderType.Sleep);

        Game.HealRestingUnit(unit);

        Assert.True(unit.HitPointsLost > 0);
        Assert.Equal((int)OrderType.Sleep, unit.Order);
    }

    [Fact]
    public void HealthySleepingUnit_StaysAsleep()
    {
        // The point of the rule: sleeping on watch is not cancelled by the healing
        // pass, because an undamaged unit never enters it.
        var unit = RestingUnit(hitPointsLost: 0, OrderType.Sleep);

        Game.HealRestingUnit(unit);

        Assert.Equal((int)OrderType.Sleep, unit.Order);
    }

    [Fact]
    public void FortifiedUnit_StaysFortifiedWhenFullyHealed()
    {
        // Only sleep is cancelled. A fortified unit is holding ground on purpose,
        // and waking it would give away a defensive bonus the player chose.
        var unit = RestingUnit(hitPointsLost: 1, OrderType.Fortified);

        Game.HealRestingUnit(unit);

        Assert.Equal(0, unit.HitPointsLost);
        Assert.Equal((int)OrderType.Fortified, unit.Order);
    }

    private static Unit RestingUnit(int hitPointsLost, OrderType order)
    {
        var map = new Map(true, 0) { Tile = new Tile[3, 3], XDim = 3, YDim = 3 };
        var terrain = new Terrain { Type = TerrainType.Plains, Specials = [] };
        var tile = new Tile(1, 1, terrain, 1, map, 1, new bool[2]);
        map.Tile[1, 1] = tile;

        return new Unit
        {
            Owner = new Civilization(),
            TypeDefinition = new UnitDefinition
            {
                Domain = UnitGas.Ground,
                Move = 1,
                Flags = Enumerable.Repeat(false, 13).ToArray(),
                Attack = 1,
                Defense = 1,
            },
            CurrentLocation = tile,
            HitPointsLost = hitPointsLost,
            Order = (int)order,
        };
    }
}
