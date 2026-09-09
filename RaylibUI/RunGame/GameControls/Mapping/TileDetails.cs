using RhyCiv.Engine.Enums;
using Model.Images;
using Raylib_CSharp.Images;

namespace RaylibUI.RunGame.GameControls.Mapping;

public class TileDetails
{
    public Image Image { get; set; }
    public ForegroundImprovement? ForegroundElement { get; set; }

    /// <summary>
    /// The tile resampled to the size it is currently drawn at, if that differs
    /// from the size it was composed at.
    /// <para>
    /// Terrain is composed at power-of-two scales so that changing the zoom by one
    /// notch does not rebuild every texture, which means the composed tile is
    /// usually a little larger or smaller than the square it is drawn into.
    /// Resampling it is by far the most expensive part of drawing a tile, and
    /// without this it happened once per tile per redraw -- the same answer, several
    /// hundred times a second. It is kept until the drawn size changes, which is to
    /// say until the zoom does.
    /// </para>
    /// </summary>
    internal Image? Scaled { get; set; }

    internal int ScaledWidth { get; set; }

    internal int ScaledHeight { get; set; }

    /// <summary>
    /// View build this tile was last used by. The cache will not evict a tile
    /// that the view currently being composed has already drawn from.
    /// </summary>
    internal long Generation { get; set; }

    /// <summary>
    /// Releases the tile and any resampled copy of it.
    /// </summary>
    internal void Unload()
    {
        Image.Unload();
        if (Scaled is { } scaled)
        {
            scaled.Unload();
            Scaled = null;
        }
    }
}

public class ForegroundImprovement
{
    public IImageSource Image { get; set; } = null!;
}

public class UnitHidingImprovement : ForegroundImprovement
{
    public IImageSource UnitImage { get; set; } = null!;
    
    public UnitGas UnitDomain { get; set; }
}