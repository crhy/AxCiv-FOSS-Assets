using Civ2engine.Statistics;
using Model.Core;
using Model.Core.Cities;

namespace Core.Tests.Statistics;

public class PowerTests
{
    [Fact]
    public void CalculateRating_CountsFutureTechsPopulationAndGold()
    {
        var civilization = new Civilization
        {
            Advances = [true, true, true, true, false],
            FutureTechCount = 4,
            Money = 512
        };
        civilization.Cities.Add(new City { Size = 5 });

        Assert.Equal(10, Power.CalculateRating(civilization));
    }

    [Fact]
    public void CalculateRating_ClampsToOriginalByteRange()
    {
        var civilization = new Civilization
        {
            Advances = Enumerable.Repeat(true, 1000).ToArray(),
            FutureTechCount = 1000,
            Money = 100000
        };

        Assert.Equal(255, Power.CalculateRating(civilization));
    }
}
