using Civ2engine.Advances;

namespace Core.Tests.Advances;

public class AdvanceFunctionsTests
{
    [Theory]
    [InlineData(0, 100, 0)]
    [InlineData(24, 100, 0)]
    [InlineData(25, 100, 1)]
    [InlineData(50, 100, 2)]
    [InlineData(75, 100, 3)]
    [InlineData(100, 100, 3)]
    [InlineData(50, 0, 0)]
    [InlineData(-10, 100, 0)]
    public void CalculateResearchProgressQuarter_ReturnsCiv2IconBand(
        int progress, int cost, int expected)
    {
        Assert.Equal(expected,
            AdvanceFunctions.CalculateResearchProgressQuarter(progress, cost));
    }
}
