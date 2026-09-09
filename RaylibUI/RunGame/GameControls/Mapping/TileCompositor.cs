using Raylib_CSharp.Images;
using Raylib_CSharp.Textures;

namespace RaylibUI.RunGame.GameControls.Mapping;

/// <summary>
/// Copies a composed map tile into the picture of the map being built.
/// <para>
/// This is the innermost loop of drawing the world: a screenful of map is a
/// thousand tiles or more, and the whole picture is composed again whenever the
/// view moves or anything on it changes. raylib's own <c>ImageDraw</c> was
/// costing around forty microseconds a tile even when no scaling was involved,
/// which put a hundred milliseconds between clicking the map and seeing the
/// result -- long enough that the frame swallowed the next click, which is what
/// made the game feel as though it were ignoring the mouse.
/// </para>
/// <para>
/// Tiles and the picture they are composed into are both 32-bit RGBA, laid out
/// top row first, so the copy is a straight run over rows: opaque pixels are
/// stored, clear ones skipped, and only the fringe of a tile is actually
/// blended. Anything not in that format falls back to raylib.
/// </para>
/// </summary>
internal static class TileCompositor
{
    public static bool CanCompose(Image image) =>
        image.Format == PixelFormat.UncompressedR8G8B8A8 && image.Data != nint.Zero &&
        image.Width > 0 && image.Height > 0;

    /// <summary>
    /// Draws <paramref name="source"/> over <paramref name="destination"/> with its
    /// top-left corner at (<paramref name="left"/>, <paramref name="top"/>), clipped
    /// to the destination. Both images must be 32-bit RGBA; see
    /// <see cref="CanCompose"/>.
    /// </summary>
    public static unsafe void Blend(Image destination, Image source, int left, int top)
    {
        var firstColumn = Math.Max(0, -left);
        var firstRow = Math.Max(0, -top);
        var lastColumn = Math.Min(source.Width, destination.Width - left);
        var lastRow = Math.Min(source.Height, destination.Height - top);

        if (firstColumn >= lastColumn || firstRow >= lastRow)
        {
            return;
        }

        var sourcePixels = (byte*)source.Data;
        var destinationPixels = (byte*)destination.Data;

        for (var row = firstRow; row < lastRow; row++)
        {
            var sourceRow = sourcePixels + (long)row * source.Width * 4;
            var destinationRow = destinationPixels +
                                 ((long)(top + row) * destination.Width + left) * 4;

            for (var column = firstColumn; column < lastColumn; column++)
            {
                var from = sourceRow + (long)column * 4;
                var alpha = from[3];
                if (alpha == 0)
                {
                    continue;
                }

                var to = destinationRow + (long)column * 4;
                if (alpha == 255)
                {
                    *(uint*)to = *(uint*)from;
                    continue;
                }

                // Source-over, rounded rather than truncated so a long run of
                // part-transparent overlays does not drift darker.
                var inverse = 255 - alpha;
                to[0] = (byte)((from[0] * alpha + to[0] * inverse + 127) / 255);
                to[1] = (byte)((from[1] * alpha + to[1] * inverse + 127) / 255);
                to[2] = (byte)((from[2] * alpha + to[2] * inverse + 127) / 255);
                to[3] = (byte)(alpha + to[3] * inverse / 255);
            }
        }
    }
}
