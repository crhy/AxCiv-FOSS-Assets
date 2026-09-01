using System.Numerics;
using Civ2engine.MapObjects;
using Model.Core.Mapping;
using Model.ImageSets;
using Raylib_CSharp.Textures;
using RaylibUI.RunGame.GameControls.Mapping.Views.ViewElements;

namespace RaylibUI.RunGame.GameControls.Mapping;

public class CityData : TextureElement
{
    public CityData(PlayerColour color, string name, int size, Vector2 sizeRectLoc, Texture2D texture,
        Vector2 location, Tile tile, Vector2? logicalSize = null, Vector2? offset = null, float renderScale = 1f)
        : base(texture, location, tile, offset: offset, renderScale: renderScale, maxDrawSize: logicalSize)
    {
        Color = color;
        Name = name;
        Size = size;
        SizeRectLoc = sizeRectLoc;
        LogicalSize = logicalSize ?? new Vector2(texture.Width, texture.Height);
    }

    public string Name { get; }
    public PlayerColour Color { get; }
    public int Size { get; }
    public Vector2 SizeRectLoc { get; }

    /// <summary>
    /// Civ2 logical footprint of the city sprite. Labels and markers are placed
    /// against this rather than the source texture, so high-resolution FOSS art
    /// does not displace them.
    /// </summary>
    public Vector2 LogicalSize { get; }
}
