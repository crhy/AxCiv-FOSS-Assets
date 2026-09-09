using System.Reflection;
using System.Runtime.Serialization;
using RhyCiv.Engine;
using RhyCiv.Tests.Mocks;
using Model.Core;
using Model.Constants;
using Model.Core.GameRules;
using Model.Core.Player;
using Model.Core.Units;

namespace RhyCiv.Tests;

/// <summary>
/// Ending a turn has to bring the turn back round to the player.
/// <para>
/// It did not. Starting any civilisation's turn returned to the caller and
/// waited, which is right for the player and wrong for everybody else: a
/// computer civilisation has nothing that will come back later and ask it to
/// carry on. So one press of End Turn advanced the world by one civilisation
/// rather than by one turn, and with eight rivals on the board the player had to
/// press Enter eight times, apparently into a game that was ignoring them, before
/// they could move again. Reported as 0.1.4 being "very laggy and buggy, with the
/// player having to hit the enter key numerous times".
/// </para>
/// </summary>
public class GameTurnAdvanceTests
{
    [Fact]
    public void EndingATurn_ComesBackToThePlayer()
    {
        var (game, players) = TwoRivalsAndAPlayer();

        game.ChoseNextCiv();

        Assert.Equal(PlayerType.Local, game.GetActiveCiv.PlayerType);
        Assert.Same(players[1].Civilization, game.GetActiveCiv);
    }

    [Fact]
    public void EndingATurn_PlaysEveryRivalOnTheWay()
    {
        var (game, players) = TwoRivalsAndAPlayer();

        game.ChoseNextCiv();

        // Both computer civilisations were given their turn, and told when it was
        // over -- which is where a scripted AI gets its end-of-turn event.
        Assert.Equal(1, players[2].TurnStartCount);
        Assert.Equal(1, players[2].WaitingAtEndOfTurnCount);
        Assert.Equal(1, players[3].TurnStartCount);
        Assert.Equal(1, players[3].WaitingAtEndOfTurnCount);
    }

    [Fact]
    public void EndingATurn_AdvancesTheTurnNumberExactlyOnce()
    {
        var (game, _) = TwoRivalsAndAPlayer();
        var before = game.TurnNumber;

        game.ChoseNextCiv();

        Assert.Equal(before + 1, game.TurnNumber);
    }

    /// <summary>
    /// A board with the barbarians, the player, and two computer civilisations,
    /// with the player's turn just finished. Each of them holds one settler and
    /// nothing else: enough not to be counted as defeated, and little enough that
    /// the start-of-turn bookkeeping stays out of the way of what is being tested.
    /// </summary>
    private static (Game Game, RecordingPlayer[] Players) TwoRivalsAndAPlayer()
    {
        var game = (Game)FormatterServices.GetUninitializedObject(typeof(Game));

        Set(game, "_rules", new Rules());
        Set(game, "_options", new Options());
        Set(game, "_scenarioData", new Scenario());

        var civilizations = new[]
        {
            new Civilization { Id = 0, TribeName = "Barbarians", PlayerType = PlayerType.Barbarians, Alive = true },
            new Civilization { Id = 1, TribeName = "Americans", PlayerType = PlayerType.Local, Alive = true },
            new Civilization { Id = 2, TribeName = "Romans", PlayerType = PlayerType.Ai, Alive = true },
            new Civilization { Id = 3, TribeName = "Greeks", PlayerType = PlayerType.Ai, Alive = true },
        };
        foreach (var civ in civilizations.Where(c => c.PlayerType != PlayerType.Barbarians))
        {
            var settler = new Unit
            {
                Owner = civ,
                TypeDefinition = new UnitDefinition
                {
                    AIrole = AiRoleType.Settle,
                    Flags = Enumerable.Repeat(false, 13).ToArray(),
                },
            };
            civ.Units.Add(settler);
        }

        SetBacking(game, nameof(Game.AllCivilizations), civilizations.ToList());

        var players = civilizations.Select(civ => new RecordingPlayer(civ)).ToArray();
        SetBacking(game, nameof(Game.Players), players.Cast<IPlayer>().ToArray());

        // The player has just finished their turn, so the two computer
        // civilisations and then the barbarians come next, and the turn has to
        // survive being wrapped round the end of the list on the way back.
        Set(game, "_activeCivId", 1);
        Set(game, "_activeCiv", civilizations[1]);
        SetBacking(game, nameof(Game.TurnNumber), 5);

        return (game, players);
    }

    private static void Set(Game game, string field, object? value) =>
        typeof(Game)
            .GetField(field, BindingFlags.NonPublic | BindingFlags.Instance)!
            .SetValue(game, value);

    // GetUninitializedObject runs no field initialisers, so read-only auto
    // properties come back null and have to be filled in behind their backs.
    private static void SetBacking(Game game, string property, object? value) =>
        Set(game, $"<{property}>k__BackingField", value);

    internal sealed class RecordingPlayer(Civilization civilization) : MockPlayer(civilization)
    {
        public int TurnStartCount { get; private set; }
        public int WaitingAtEndOfTurnCount { get; private set; }

        public override void TurnStart(int turnNumber)
        {
            TurnStartCount++;
            base.TurnStart(turnNumber);
        }

        public override void WaitingAtEndOfTurn()
        {
            WaitingAtEndOfTurnCount++;
            base.WaitingAtEndOfTurn();
        }
    }
}
