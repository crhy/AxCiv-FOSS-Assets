using Model.Controls;

namespace RhyCiv.Tests.Cities;

/// <summary>
/// The shields in the city window's production box are a count of what the item
/// costs, and a count reads best as a block.
/// <para>
/// It used to fill each row to the width of the panel and let the last row hold
/// whatever was left, so a thirty-shield item came out as two full rows and then a
/// single shield sitting on its own, and a ten-shield item as one thin line
/// stretched across the box. Reported twice.
/// </para>
/// </summary>
public class ShieldGridTests
{
    // A shield is a little wider than it is tall, which is what makes the squarest
    // block have fewer rows than columns.
    private const float ShieldWidth = 12f;
    private const float ShieldHeight = 10f;

    [Theory]
    [InlineData(10)]
    [InlineData(20)]
    [InlineData(30)]
    [InlineData(40)]
    [InlineData(60)]
    public void EveryRowHoldsTheSameNumber(int cost)
    {
        var (rows, perRow) = Grid(cost);

        Assert.Equal(0, cost % rows);
        Assert.Equal(cost, rows * perRow);
    }

    [Fact]
    public void TheBlockIsRoughlySquare()
    {
        // Forty shields in a box ten wide: a 4x10 line and a 40x1 column both fit,
        // and neither is what should be drawn.
        var (rows, perRow) = Grid(40);

        var width = perRow * ShieldWidth;
        var height = rows * ShieldHeight;
        var ratio = Math.Max(width, height) / Math.Min(width, height);

        Assert.True(ratio < 2.0,
            $"40 shields came out {rows}x{perRow}, which is {ratio:F1} times longer than it is deep");
    }

    [Fact]
    public void ACheapItem_StaysNarrow()
    {
        // Ten shields must not be stretched across a box that could hold thirty.
        var (_, perRow) = Grid(10, maxPerRow: 30);

        Assert.True(perRow <= 5, $"ten shields spread to {perRow} across");
    }

    [Fact]
    public void AnAwkwardCost_StillFits()
    {
        // A prime cost cannot divide evenly into more than one row, so it has to
        // fall back to a single row rather than leaving a stub.
        var (rows, perRow) = Grid(17, maxPerRow: 20);

        Assert.True(rows * perRow >= 17);
        Assert.True(perRow <= 20);
    }

    [Fact]
    public void ACostTallerThanTheBox_IsSpreadAcross()
    {
        // Ninety shields with room for only ten rows: it has to widen.
        var (rows, perRow) = Grid(90, maxRows: 10, maxPerRow: 30);

        Assert.True(rows <= 10);
        Assert.True(rows * perRow >= 90);
    }

    [Fact]
    public void NothingIsEverZero()
    {
        var (rows, perRow) = Grid(0);

        Assert.True(rows >= 1);
        Assert.True(perRow >= 1);
    }

    private static (int Rows, int PerRow) Grid(int cost, int maxRows = 10, int maxPerRow = 20) =>
        ShieldBoxLayout.Choose(cost, maxRows, maxPerRow, ShieldWidth, ShieldHeight);
}
