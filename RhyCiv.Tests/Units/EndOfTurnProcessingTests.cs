using RhyCiv.Engine;
using RhyCiv.Engine.Enums;
using RhyCiv.Tests.Mocks;
using RhyCiv.Tests.TestFiles;
using Model.Core.Units;

namespace RhyCiv.Tests.Units;

/// <summary>
/// Ending a turn has to process every unit, not just as far as the first one that
/// wants a decision.
/// <para>
/// It used to return the moment it found one. Two things followed from that. The
/// units after it in the order were never processed, so a unit told to fortify did
/// not become fortified until whatever came before it in the list had been dealt
/// with. And since each press of End Turn got only as far as the next such unit, a
/// player with several of them pressed Enter over and over with nothing appearing
/// to happen — reported as "sometimes you have to press enter more than once to
/// end the turn".
/// </para>
/// </summary>
public class EndOfTurnProcessingTests
{
    [Fact]
    public void WithNothingOutstanding_TheTurnEndsOnTheFirstAsk()
    {
        var (game, civ, _) = Ready();
        foreach (var unit in civ.Units)
        {
            unit.MovePointsLost = unit.MaxMovePoints;
        }

        Assert.True(game.ProcessEndOfTurn());
    }

    [Fact]
    public void AUnitLaterInTheList_IsStillProcessed_WhenAnEarlierOneWantsOrders()
    {
        var (game, civ, player) = Ready();

        // The first unit is left needing a decision: a ground unit ordered to a
        // square out at sea. The destination is a real square, so the order stands,
        // but no route to it exists, which is one of the cases that hands a unit
        // back to the player.
        var stuck = civ.Units[0];
        OrderToTheOpenSea(stuck);

        // A later one is told to fortify, which end-of-turn processing is what
        // promotes to actually being fortified.
        var fortifying = civ.Units[^1];
        Assert.NotSame(stuck, fortifying);
        fortifying.MovePointsLost = 0;
        fortifying.Order = (int)OrderType.Fortify;

        var ended = game.ProcessEndOfTurn();

        // The turn does not end, because the first unit needs the player.
        Assert.False(ended);
        Assert.Same(stuck, player.LastActivated);

        // But the unit behind it in the queue was still dealt with. Before the fix
        // this stayed on Fortify until the stuck unit had been resolved.
        Assert.Equal((int)OrderType.Fortified, fortifying.Order);
    }

    [Fact]
    public void OnlyOneUnit_IsOfferedPerAsk()
    {
        var (game, civ, player) = Ready();

        foreach (var unit in civ.Units)
        {
            OrderToTheOpenSea(unit);
        }

        Assert.False(game.ProcessEndOfTurn());

        // The first of them, and the others have had their orders cleared rather
        // than being left for a later press to rediscover.
        Assert.Same(civ.Units[0], player.LastActivated);
        Assert.All(civ.Units, unit => Assert.Equal((int)OrderType.NoOrders, unit.Order));
    }

    /// <summary>
    /// Gives a ground unit a GoTo it cannot possibly satisfy, so end-of-turn
    /// processing has to hand it back rather than resolve it.
    /// </summary>
    private static void OrderToTheOpenSea(Unit unit)
    {
        var map = unit.CurrentLocation.Map;
        var sea = Enumerable.Range(0, map.XDim)
            .SelectMany(x => Enumerable.Range(0, map.YDim).Select(y => map.Tile[x, y]))
            .First(tile => tile.Type == Model.Core.Mapping.TerrainType.Ocean);

        unit.MovePointsLost = 0;
        unit.Order = (int)OrderType.GoTo;
        unit.GoToX = sea.X;
        unit.GoToY = sea.Y;
    }

    private static (Game Game, Model.Core.Civilization Civ, RecordingPlayer Player) Ready()
    {
        var (game, _, _) = CleanRoomGameFactory.CreateGame();
        var civ = game.GetPlayerCiv;
        var player = new RecordingPlayer(civ);
        game.ConnectPlayer(player);
        return (game, civ, player);
    }

    /// <summary>Remembers which unit was handed back to the player, and when.</summary>
    private sealed class RecordingPlayer(Model.Core.Civilization civ) : MockPlayer(civ)
    {
        public Unit? LastActivated { get; private set; }

        public override void SetUnitActive(Unit? unit, bool move)
        {
            if (unit != null)
            {
                LastActivated = unit;
            }

            base.SetUnitActive(unit, move);
        }
    }
}
