using Civ2engine.Enums;
using Model.Images;
using Raylib_CSharp.Images;

namespace RaylibUI.RunGame.GameControls.Mapping;

public class TileDetails
{
    public Image Image { get; set; }
    public ForegroundImprovement? ForegroundElement { get; set; }

    /// <summary>
    /// View build this tile was last used by. The cache will not evict a tile
    /// that the view currently being composed has already drawn from.
    /// </summary>
    internal long Generation { get; set; }
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