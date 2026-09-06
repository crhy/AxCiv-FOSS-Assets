using RhyCiv.Engine;
using RhyCiv.Tests.Mocks;
using RhyCiv.Tests.TestFiles;
using Model.Core.Units;

namespace RhyCiv.Tests.Units;

/// <summary>
/// Choosing a unit tells the player, and the player changes the interface mode --
/// and a mode is entitled to ask for the next unit when it has none. That is a
/// cycle, and it shipped: after founding a city with the last settler, the game
/// asked for a unit, found none, told the player, and the player asked again,
/// until the stack overflowed. A StackOverflowException cannot be caught, so the
/// process died with no crash report of any kind.
///
/// The interface side is fixed at its source. This covers the engine's guard,
/// which is what stops any future version of the same cycle from being fatal.
/// </summary>
public class ChooseNextUnitReentrancyTests
{
    [Fact]
    public void ChooseNextUnit_DoesNotRecurse_WhenTheAnswerAsksAgain()
    {
        var (game, _, _) = CleanRoomGameFactory.CreateGame();
        var civ = game.GetPlayerCiv;

        var player = new ReentrantPlayer(civ, game);
        game.ConnectPlayer(player);

        // The state that triggered it: nothing left with moves to give.
        foreach (var unit in civ.Units)
        {
            unit.MovePointsLost = unit.MaxMovePoints;
        }

        game.ChooseNextUnit();

        // Without the guard this never returns -- it recurses until the process
        // dies. One re-entrant request is answered by returning, not by recursing.
        Assert.True(player.ReentryAttempts > 0,
            "the test player did not actually re-enter, so this proves nothing");
        Assert.Equal(1, player.OutstandingCalls);
    }

    /// <summary>
    /// Stands in for the real player, whose mode switch calls back into
    /// ChooseNextUnit whenever it is told there is no active unit.
    /// </summary>
    private sealed class ReentrantPlayer(Model.Core.Civilization civ, Game game) : MockPlayer(civ)
    {
        private int _depth;

        public int ReentryAttempts { get; private set; }

        /// <summary>Highest nesting depth reached; 1 means no recursion happened.</summary>
        public int OutstandingCalls { get; private set; }

        public override void SetUnitActive(Unit? unit, bool move)
        {
            _depth++;
            OutstandingCalls = Math.Max(OutstandingCalls, _depth);

            if (unit == null)
            {
                ReentryAttempts++;
                game.ChooseNextUnit();
            }

            _depth--;
        }
    }
}
