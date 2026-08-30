using Civ2engine;
using Civ2engine.IO;
using Civ2engine.Terrains;
using Model;
using Model.Images;
using Model.ImageSets;
using Raylib_CSharp.Colors;
using Raylib_CSharp.Images;
using Raylib_CSharp.Transformations;
using RaylibUI;
using RaylibUtils;
using System.Numerics;
using Model.Core.GameRules;
using Model.Core.Mapping;

namespace Civ2.ImageLoader
{
    public static class TerrainLoader
    {
        private static readonly string[] FossTerrainNames =
        [
            "desert",
            "plains",
            "grassland",
            "grassland", // The forest connection overlay supplies the trees.
            "hills",
            "mountains",
            "tundra",
            "glacier",
            "swamp",
            "jungle",
            "ocean"
        ];

        public static void LoadTerrain(Ruleset ruleset, IUserInterface active)
        {
            active.TileSets.Clear();
            for (var i = 0; i < active.ExpectedMaps; i++)
            {
                active.TileSets.Add(LoadTerrain(ruleset, i, active));
            }
        }

        private static TerrainSet LoadTerrain(Ruleset ruleset, int index, IUserInterface active)
        {
            // Initialize objects
            var terrain = new TerrainSet(64, 32);

            // Get dither tile before making it transparent
            var ditherTile = Images.ExtractBitmap(MapIndexChange((BitmapStorage)active.PicSources["dither"][0], index, active));

            Color gray;
            unsafe
            {
                // Get the gray colour (it's not always the same in MGE/TOT, unlike magenta)
                var imageColours = ditherTile.LoadColors();
                gray = imageColours[0];
                Image.UnloadColors(imageColours);
            }
            ditherTile.ReplaceColor(Color.Black, Color.White);
            ditherTile.ReplaceColor(new Color(255, 0, 255, 0), Color.Black);
            ditherTile.ReplaceColor(gray, Color.Black);

            terrain.BaseTiles = active.PicSources["base1"].Select(t => MapIndexChange((BitmapStorage)t, index, active)).ToArray();
            ApplyFossTerrainTextures(terrain, index, active);

            terrain.Specials = new[]
            {
                active.PicSources["special1"].Select(s => MapIndexChange((BitmapStorage) s, index, active)).ToArray(),
                active.PicSources["special2"].Select(s => MapIndexChange((BitmapStorage)s, index, active)).ToArray(),
            };

            terrain.Blank = MapIndexChange((BitmapStorage)active.PicSources["blank"][0], index, active);

            // 4 small dither tiles (base mask must be B/W)
            terrain.DitherMask = new[]
            {
                Image.FromImage(ditherTile, new Rectangle(32, 0, 32, 16)),
                Image.FromImage(ditherTile, new Rectangle(32, 16, 32, 16)),
                Image.FromImage(ditherTile, new Rectangle(0, 16, 32, 16)),
                Image.FromImage(ditherTile, new Rectangle(0, 0, 32, 16)),
            };

            terrain.DitherMaps = new[]
            {
                BuildDitherMaps(terrain.DitherMask[0], terrain.BaseTiles, 32, 0, terrain.Blank),
                BuildDitherMaps(terrain.DitherMask[1], terrain.BaseTiles, 32, 16, terrain.Blank),
                BuildDitherMaps(terrain.DitherMask[2], terrain.BaseTiles, 0, 16, terrain.Blank),
                BuildDitherMaps(terrain.DitherMask[3], terrain.BaseTiles, 0, 0, terrain.Blank),
            };

            terrain.River = active.PicSources["river"].Select(r => MapIndexChange((BitmapStorage)r, index, active)).ToArray();
            terrain.Forest = active.PicSources["forest"].Select(r => MapIndexChange((BitmapStorage)r, index, active)).ToArray();
            terrain.Mountains = active.PicSources["mountain"].Select(r => MapIndexChange((BitmapStorage)r, index, active)).ToArray();
            terrain.Hills = active.PicSources["hill"].Select(r => MapIndexChange((BitmapStorage)r, index, active)).ToArray();
            terrain.RiverMouth = active.PicSources["riverMouth"].Select(r => MapIndexChange((BitmapStorage)r, index, active)).ToArray();

            terrain.Coast = new IImageSource[8, 4];
            for (var i = 0; i < 8; i++)
            {
                terrain.Coast[i, 0] = MapIndexChange((BitmapStorage)active.PicSources["coastline"][4 * i + 0], index, active); // N
                terrain.Coast[i, 1] = MapIndexChange((BitmapStorage)active.PicSources["coastline"][4 * i + 1], index, active); // S
                terrain.Coast[i, 2] = MapIndexChange((BitmapStorage)active.PicSources["coastline"][4 * i + 2], index, active); // W
                terrain.Coast[i, 3] = MapIndexChange((BitmapStorage)active.PicSources["coastline"][4 * i + 3], index, active); // E
            }

            // Road & railroad
            terrain.ImprovementsMap = new Dictionary<int, ImprovementGraphic>();

            var roadGraphics = new ImprovementGraphic
            {
                Levels = new IImageSource[2, 9]
            };

            terrain.ImprovementsMap.Add(ImprovementTypes.Road, roadGraphics);

            for (var i = 0; i < 9; i++)
            {
                roadGraphics.Levels[0, i] = MapIndexChange((BitmapStorage)active.PicSources["road"][i], index, active);
                roadGraphics.Levels[1, i] = MapIndexChange((BitmapStorage)active.PicSources["railroad"][i], index, active);
            }

            terrain.ImprovementsMap.Add(ImprovementTypes.Irrigation, new ImprovementGraphic
            {
                Levels = new[,]
                {
                    { MapIndexChange((BitmapStorage)active.PicSources["irrigation"][0], index, active) },
                    { MapIndexChange((BitmapStorage)active.PicSources["farmland"][0], index, active) }
                }
            });

            terrain.ImprovementsMap[ImprovementTypes.Mining] = new ImprovementGraphic
                { Levels = new[,] { { MapIndexChange((BitmapStorage)active.PicSources["mine"][0], index, active) } } };

            terrain.ImprovementsMap[ImprovementTypes.Pollution] = new ImprovementGraphic
                { Levels = new[,] { { MapIndexChange((BitmapStorage)active.PicSources["pollution"][0], index, active) } } };

            //Note airbase and fortress are now loaded directly by the cities loader
            terrain.GrasslandShield = MapIndexChange((BitmapStorage)active.PicSources["shield"][0], index, active);
            terrain.Huts = MapIndexChange((BitmapStorage)active.PicSources["hut"][0], index, active);

            return terrain;
        }

        private static void ApplyFossTerrainTextures(TerrainSet terrain, int mapIndex, IUserInterface active)
        {
            // The bundled textures depict the classic Earth terrain set. Other Test of Time maps
            // retain their scenario-specific art until equivalent FOSS sets are available.
            if (mapIndex != 0)
            {
                return;
            }

            for (var terrainIndex = 0;
                 terrainIndex < terrain.BaseTiles.Length && terrainIndex < FossTerrainNames.Length;
                 terrainIndex++)
            {
                var artPath = FindFossTerrainPath(FossTerrainNames[terrainIndex]);
                if (artPath == null)
                {
                    continue;
                }

                var replacement = Images.LoadImageFromFile(artPath).Image;
                if (replacement.Width <= 1 || replacement.Height <= 1)
                {
                    continue;
                }

                replacement.Resize(terrain.TileWidth, terrain.TileHeight);
                ApplyOriginalTileTransparency(replacement,
                    Images.ExtractBitmap(terrain.BaseTiles[terrainIndex], active));
                terrain.BaseTiles[terrainIndex] = new MemoryStorage(replacement,
                    $"FossTerrain-{terrainIndex}-{artPath}");
            }
        }

        private static string? FindFossTerrainPath(string terrainName)
        {
            var fileName = $"{terrainName}.jpg";
            var roots = Settings.SearchPaths
                .Concat([
                    Environment.CurrentDirectory,
                    AppContext.BaseDirectory,
                    Path.Combine(Environment.CurrentDirectory, "RaylibUI")
                ])
                .Where(root => !string.IsNullOrWhiteSpace(root))
                .Distinct(StringComparer.OrdinalIgnoreCase);

            foreach (var root in roots)
            {
                foreach (var directory in new[]
                         {
                             Path.Combine(root, "Terrain"),
                             Path.Combine(root, "FOSSart", "Terrain"),
                             Path.Combine(root, "RaylibUI", "FOSSart", "Terrain")
                         })
                {
                    var path = Path.Combine(directory, fileName);
                    if (File.Exists(path))
                    {
                        return path;
                    }
                }
            }

            return null;
        }

        private static void ApplyOriginalTileTransparency(Image replacement, Image originalTile)
        {
            for (var y = 0; y < replacement.Height; y++)
            {
                for (var x = 0; x < replacement.Width; x++)
                {
                    if (originalTile.GetColor(x, y).A == 0)
                    {
                        replacement.DrawPixel(x, y, Color.Blank);
                    }
                }
            }
        }

        private static DitherMap BuildDitherMaps(Image mask, IImageSource[] baseTiles, int offsetX, int offsetY,
            IImageSource terrainBlank)
        {
            var sampleRect = new Rectangle(offsetX, offsetY, 32, 16);
            var totalTiles = baseTiles.Length + 1;
            var ditherMaps = new Image[totalTiles];
            for (var i = 0; i < baseTiles.Length; i++)
            {
                ditherMaps[i] = Image.FromImage(Images.ExtractBitmap(baseTiles[i]), sampleRect);
                ditherMaps[i].AlphaMask(mask);
            }

            ditherMaps[^1] = Image.FromImage(Images.ExtractBitmap(terrainBlank), sampleRect);
            ditherMaps[^1].AlphaMask(mask);

            return new DitherMap { X = offsetX, Y = offsetY, Images = ditherMaps };
        }

        /// <summary>
        /// For TERRAIN3, 4, 5, etc.
        /// </summary>
        private static IImageSource MapIndexChange(BitmapStorage storage, int mapIndex, IUserInterface active)
        {
            var file = storage.Filename;

            if (mapIndex != 0)
            {
                int.TryParse(file[^1].ToString(), out int currentIndex);
                int newIndex = mapIndex * 2 + currentIndex;
                file = $"{file.Remove(file.Length - 1, 1)}{newIndex}";
            }

            var img = new BitmapStorage(file, storage.Location, storage.TransparencyPixel, storage.SearchFlagLoc);
            Images.ExtractBitmap(img, active);
            return img;
        }
    }
}
