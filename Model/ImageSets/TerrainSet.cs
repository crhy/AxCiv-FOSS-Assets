using Model.Core;
using Model.Core.Mapping;
using Model.Images;
using Raylib_CSharp.Images;
using RaylibUI;

namespace Model.ImageSets
{
    public class TerrainSet
    {
        public TerrainSet(int tileWidth, int tileHeight, int renderScale = 1)
        {
            TileWidth = tileWidth;
            TileHeight = tileHeight;
            HalfWidth = tileWidth / 2;
            HalfHeight = tileHeight / 2;
            DiagonalCut = HalfHeight * HalfWidth;
            RenderScale = renderScale < 1 ? 1 : renderScale;
        }

        /// <summary>
        /// How many source pixels this set composes per logical Civ2 tile pixel.
        /// Tile graphics are built at <see cref="TileWidth"/> x <see cref="RenderScale"/>
        /// so that a zoomed-in map is composed at the resolution it will be drawn
        /// at, rather than being upscaled from a fixed 64x32 grid.
        /// </summary>
        public int RenderScale { get; }


        public int DiagonalCut { get; }

        public int HalfHeight { get; }

        public int HalfWidth { get; }

        public IImageSource[] BaseTiles { get; set; } = [];
        public IImageSource[][] Specials { get; set; } = [];
        public IImageSource Blank { get; set; } = null!;
        public DitherMap[] DitherMaps { get; set; } = [];
        public IImageSource[] RiverMouth { get; set; } = [];
        public IImageSource[] River { get; set; } = [];
        public IImageSource[] Forest { get; set; } = [];
        public IImageSource[] Mountains { get; set; } = [];
        public IImageSource[] Hills { get; set; } = [];
        public IImageSource[,] Coast { get; set; } = new IImageSource[0, 0];
        public IImageSource Pollution { get; set; } = null!;
        public IImageSource GrasslandShield { get; set; } = null!;

        public IImageSource Huts { get; set; } = null!;
        public Image[] DitherMask { get; set; } = [];
        
        public Dictionary<int, ImprovementGraphic> ImprovementsMap { get; set; } = new();

        public int TileWidth { get; }

        public int TileHeight { get; }

        public IImageSource[] ImagesFor(TerrainType terrain)
        {
            switch (terrain)
            {
                case TerrainType.Forest:
                    return Forest;
                case TerrainType.Hills:
                    return Hills;
                case TerrainType.Mountains:
                    return Mountains;
                case TerrainType.Desert:
                case TerrainType.Plains:
                case TerrainType.Grassland:
                case TerrainType.Tundra:
                case TerrainType.Glacier:
                case TerrainType.Swamp:
                case TerrainType.Jungle:
                case TerrainType.Ocean:
                default:
                    throw new ArgumentOutOfRangeException(nameof(terrain), terrain, null);
            }
        }
    }
}
