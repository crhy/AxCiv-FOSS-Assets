using RhyCiv.Engine.SaveLoad;
using RhyCiv.Tests.Mocks;
using RhyCiv.Tests.TestFiles;

namespace RhyCiv.Tests.SaveLoad;

/// <summary>
/// The Save Game dialog offers a name built from the leader's initials and the
/// year. It built it with Substring(0, 2), which throws on a leader whose name is
/// shorter than two characters — so a one-letter leader, or a custom civilisation
/// saved with the name left blank, took the game down on opening the dialog,
/// before there was anything on screen to say why.
/// </summary>
public class SaveFileNameTests
{
    [Theory]
    [InlineData("Caesar")]
    [InlineData("Xi")]
    [InlineData("R")]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData("Ö")]
    [InlineData("!!!")]
    public void AnyLeaderName_ProducesAUsableFileName(string leaderName)
    {
        var name = Suggest(leaderName);

        Assert.EndsWith(".sav", name);
        Assert.NotEmpty(Path.GetFileNameWithoutExtension(name));
        Assert.DoesNotContain(name, character => Path.GetInvalidFileNameChars().Contains(character));
        Assert.Equal(name, Path.GetFileName(name));
    }

    [Fact]
    public void TheLeadersInitials_AreUsedWhenThereAreSome()
    {
        Assert.StartsWith("ca", Suggest("Caesar"));
    }

    [Fact]
    public void ANameWithNothingUsable_FallsBackRatherThanFailing()
    {
        var name = Suggest("!!!");

        Assert.StartsWith("rhy", name);
    }

    [Fact]
    public void TheNameCarriesNoSpaces()
    {
        // The year reads like "4000 BC", and a space in a suggested file name is an
        // avoidable nuisance on every platform.
        Assert.DoesNotContain(' ', Suggest("Caesar"));
    }

    private static string Suggest(string leaderName)
    {
        var (game, _, _) = CleanRoomGameFactory.CreateGame();
        game.ConnectPlayer(new MockPlayer(game.GetPlayerCiv));
        game.ActivePlayer.Civilization.LeaderName = leaderName;
        return SaveFileNames.Suggest(game);
    }
}
