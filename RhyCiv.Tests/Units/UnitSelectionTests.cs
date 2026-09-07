using RhyCiv.Engine;
using RhyCiv.Engine.Enums;
using RhyCiv.Tests.Mocks;
using RhyCiv.Tests.TestFiles;
using Model.Core;
using Model.Core.Units;

namespace RhyCiv.Tests.Units;

/// <summary>
/// Choosing the next unit must only ever offer one that can actually be given
/// orders.
/// <para>
/// The interface refuses a unit whose turn has ended, so offering one left it with
/// nothing selected — and, because that path also skipped the "nothing left to
/// move" handling, without being told the turn was waiting to be ended either.
/// That is the state behind "troops are blinking but they can't move" and Enter
/// appearing to do nothing.
/// </para>
/// </summary>
public class UnitSelectionTests
{
    [Fact]
    public void AUnitToldToWait_ThatCanNoLongerMove_IsNotOffered()
    {
        var (game, civ, player) = Ready();

        // Everything is spent except one unit, which the player told to wait and
        // then fortified — so by the time the waiting list comes round again it can
        // no longer be given orders.
        var waiting = civ.Units.First(unit => !unit.Dead);
        foreach (var unit in civ.Units)
        {
            unit.MovePointsLost = unit.MaxMovePoints;
        }

        waiting.MovePointsLost = 0;
        waiting.Order = (int)OrderType.Fortified;
        player.WaitingList.Add(waiting);

        // Connecting the player reports whatever was active at the time, which is
        // not what these are about.
        player.Offered.Clear();

        game.ChooseNextUnit();

        Assert.DoesNotContain(waiting, player.Offered);
    }

    [Fact]
    public void WhenNothingCanBeGivenOrders_TheTurnIsMarkedAsWaiting()
    {
        var (game, civ, player) = Ready();

        var waiting = civ.Units.First(unit => !unit.Dead);
        foreach (var unit in civ.Units)
        {
            unit.MovePointsLost = unit.MaxMovePoints;
        }

        waiting.MovePointsLost = 0;
        waiting.Order = (int)OrderType.Fortified;
        player.WaitingList.Add(waiting);

        // Connecting the player reports whatever was active at the time, which is
        // not what these are about.
        player.Offered.Clear();

        game.ChooseNextUnit();

        // One of three things has to happen: a unit is offered, the turn is marked
        // as waiting to be ended, or it ends by itself. What must never happen is
        // none of them, which leaves the player looking at a map that answers
        // nothing.
        Assert.True(player.Offered.Count > 0 || player.WaitedAtEndOfTurn || player.TurnsStarted > 0,
            "nothing was offered, the turn was not marked as waiting, and it did not end");
    }

    [Fact]
    public void AUnitToldToWait_ThatStillCanMove_IsOffered()
    {
        var (game, civ, player) = Ready();

        var waiting = civ.Units.First(unit => !unit.Dead);
        foreach (var unit in civ.Units)
        {
            unit.MovePointsLost = unit.MaxMovePoints;
        }

        waiting.MovePointsLost = 0;
        waiting.Order = (int)OrderType.NoOrders;
        player.WaitingList.Add(waiting);

        // Connecting the player reports whatever was active at the time, which is
        // not what these are about.
        player.Offered.Clear();

        game.ChooseNextUnit();

        Assert.Contains(waiting, player.Offered);
    }

    [Fact]
    public void TheWaitingList_IsEmptiedWhenItComesRound()
    {
        var (game, civ, player) = Ready();

        foreach (var unit in civ.Units)
        {
            unit.MovePointsLost = unit.MaxMovePoints;
            unit.Order = (int)OrderType.Fortified;
            player.WaitingList.Add(unit);
        }

        // Connecting the player reports whatever was active at the time, which is
        // not what these are about.
        player.Offered.Clear();

        game.ChooseNextUnit();

        // Left in place, the same unusable units would be offered again next turn.
        Assert.Empty(player.WaitingList);
    }

    [Fact]
    public void EveryUnitOffered_CanBeGivenOrders()
    {
        var (game, civ, player) = Ready();

        // A mixture: some spent, some fortified, some asleep, one able to move.
        var orders = new[]
        {
            (int)OrderType.Fortified, (int)OrderType.Sleep, (int)OrderType.NoOrders
        };
        for (var index = 0; index < civ.Units.Count; index++)
        {
            var unit = civ.Units[index];
            unit.Order = orders[index % orders.Length];
            unit.MovePointsLost = index % 2 == 0 ? 0 : unit.MaxMovePoints;
            player.WaitingList.Add(unit);
        }

        // Connecting the player reports whatever was active at the time, which is
        // not what these are about.
        player.Offered.Clear();

        game.ChooseNextUnit();

        Assert.All(player.Offered, unit => Assert.True(unit.AwaitingOrders,
            $"offered {unit.Name} which cannot be given orders " +
            $"(order={(OrderType)unit.Order}, moves={unit.MovePoints})"));
    }

    private static (Game Game, Civilization Civ, RecordingPlayer Player) Ready()
    {
        var (game, _, _) = CleanRoomGameFactory.CreateGame();
        var civ = game.GetPlayerCiv;
        var player = new RecordingPlayer(civ);
        game.ConnectPlayer(player);
        return (game, civ, player);
    }

    private sealed class RecordingPlayer(Civilization civ) : MockPlayer(civ)
    {
        public List<Unit> Offered { get; } = new();
        public bool WaitedAtEndOfTurn { get; private set; }
        public int TurnsStarted { get; private set; }

        public override void SetUnitActive(Unit? unit, bool move)
        {
            if (unit != null)
            {
                Offered.Add(unit);
            }

            base.SetUnitActive(unit, move);
        }

        public override void WaitingAtEndOfTurn() => WaitedAtEndOfTurn = true;

        public override void TurnStart(int turnNumber) => TurnsStarted++;
    }
}
