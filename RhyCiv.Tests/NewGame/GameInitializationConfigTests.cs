using Model.Core;

namespace RhyCiv.Tests.NewGame;

public class GameInitializationConfigTests
{
    [Fact]
    public void NewGamesDefaultToConquestRules()
    {
        var config = new GameInitializationConfig();

        Assert.True(config.Bloodlust);
        Assert.True(config.DontRestartEliminatedPlayers);
    }
}
