using System;

namespace Model.Controls;

/// <summary>
/// How the shields for an item are arranged in the city window's production box.
/// </summary>
public static class ShieldBoxLayout
{
    /// <summary>
    /// Chooses how many rows of shields to draw, and how many go in each row.
    /// <para>
    /// Every row holds the same number, so the rows are only ever a whole division
    /// of the cost. Filling each row to the width of the panel and letting the last
    /// row take the remainder left a thirty-shield item as two full rows and then a
    /// single shield on its own, which reads as a mistake rather than as a total.
    /// </para>
    /// <para>
    /// Among the divisions that fit, the one nearest square wins, and the block
    /// does not have to span the width of the box: a cheap item should be a small
    /// square rather than a thin line stretched across the panel.
    /// </para>
    /// <para>
    /// A cost with no useful divisor -- a prime, or one whose factors are all too
    /// wide for the box -- has to leave a short last row, so an uneven division is
    /// accepted rather than nothing being drawn.
    /// </para>
    /// </summary>
    /// <param name="cost">Shields the item costs.</param>
    /// <param name="maxRows">Rows there is room for.</param>
    /// <param name="maxPerRow">Shields that fit across the box.</param>
    /// <param name="shieldWidth">Drawn width of one shield.</param>
    /// <param name="shieldHeight">Drawn height of one shield.</param>
    public static (int Rows, int PerRow) Choose(int cost, int maxRows, int maxPerRow,
        float shieldWidth, float shieldHeight)
    {
        cost = Math.Max(1, cost);
        maxRows = Math.Max(1, maxRows);
        maxPerRow = Math.Max(1, maxPerRow);

        return Squarest(cost, maxRows, maxPerRow, shieldWidth, shieldHeight, evenOnly: true)
               ?? Squarest(cost, maxRows, maxPerRow, shieldWidth, shieldHeight, evenOnly: false)
               ?? (1, Math.Min(cost, maxPerRow));
    }

    /// <summary>
    /// The arrangement closest to square among those that fit, or null if none do.
    /// </summary>
    private static (int Rows, int PerRow)? Squarest(int cost, int maxRows, int maxPerRow,
        float shieldWidth, float shieldHeight, bool evenOnly)
    {
        (int Rows, int PerRow)? best = null;
        var bestScore = double.MaxValue;

        for (var rows = 1; rows <= maxRows; rows++)
        {
            if (evenOnly && cost % rows != 0)
            {
                continue;
            }

            var perRow = (int)Math.Ceiling(cost / (double)rows);
            if (perRow > maxPerRow)
            {
                continue;
            }

            // How far from square the block is, measured in drawn pixels so that a
            // shield being wider than it is tall is accounted for.
            var width = perRow * shieldWidth;
            var height = rows * shieldHeight;
            double score = Math.Abs(width - height) / Math.Max(1f, Math.Max(width, height));

            if (score < bestScore)
            {
                bestScore = score;
                best = (rows, perRow);
            }
        }

        return best;
    }
}
