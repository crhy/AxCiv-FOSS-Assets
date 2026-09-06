using RhyCiv.Engine.SaveLoad;
using Model.Constants;
using Model.Core;
using Moq;

namespace RhyCiv.Tests.SaveLoad;

/// <summary>
/// The saved per-tribe city counter is a fixed-width array indexed by TribeId.
/// Not every civilisation has a slot in it, so writing one has to be guarded --
/// the barbarians carry TribeId -1, and CityActions.BuildCity records a count for
/// whichever civilisation founded a city, barbarians included. Saving after the
/// barbarians took their first city threw IndexOutOfRangeException and lost the
/// game.
/// </summary>
public class JsonGameDataTests
{
    [Fact]
    public void Save_DoesNotThrow_WhenTheBarbariansHaveFoundedCities()
    {
        var barbarians = new Civilization
        {
            Id = 0, TribeId = -1, PlayerType = PlayerType.Barbarians
        };
        var player = new Civilization { Id = 1, TribeId = 3 };

        var data = new JsonGameData(GameWith(new Dictionary<Civilization, int>
        {
            [barbarians] = 2,
            [player] = 5,
        }));

        // The player's count is kept in its own slot; the barbarians are dropped,
        // exactly as Game.LoadGame already skips them when reading back.
        Assert.Equal(5, data.CitiesBuiltSoFar[3]);
        Assert.DoesNotContain(2, data.CitiesBuiltSoFar);
    }

    [Fact]
    public void Save_DropsATribeWithNoSlotInTheFormat()
    {
        var beyond = new Civilization { Id = 1, TribeId = 500 };

        var data = new JsonGameData(GameWith(new Dictionary<Civilization, int> { [beyond] = 7 }));

        Assert.DoesNotContain(7, data.CitiesBuiltSoFar);
    }

    private static IGame GameWith(Dictionary<Civilization, int> citiesBuiltSoFar)
    {
        var date = new Mock<IGameDate>();
        date.Setup(d => d.StartingYear).Returns(-4000);
        date.Setup(d => d.TurnYearIncrement).Returns(50);

        var game = new Mock<IGame>();
        game.Setup(g => g.Date).Returns(date.Object);
        game.Setup(g => g.CitiesBuiltSoFar).Returns(citiesBuiltSoFar);
        return game.Object;
    }
}
