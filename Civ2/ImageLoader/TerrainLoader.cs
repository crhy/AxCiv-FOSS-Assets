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
        /// <summary>
        /// Composition scale used when nothing better is known. A 2x map tile
        /// matches the physical pixel density of a 3840x2160 display while
        /// retaining Civ II's original 64x32 logical tile geometry.
        /// </summary>
        public const int DefaultTerrainRenderScale = 2;

        /// <summary>
        /// Upper bound on composition scale. A tile is composed at
        /// 64 x 32 x scale, so this caps a single tile at 512x256 pixels.
        /// </summary>
        public const int MaximumTerrainRenderScale = 8;
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

        /// <summary>
        /// Terrain sets already built for this interface, keyed by composition
        /// scale. Building one resizes every base texture and recomposes every
        /// overlay, and the images end up in the shared image cache either way, so
        /// a scale is built once and then reused whenever the zoom returns to it.
        /// </summary>
        private static readonly Dictionary<IUserInterface, Dictionary<int, List<TerrainSet>>> BuiltSets = new();

        /// <summary>
        /// Loads terrain for a newly selected ruleset, discarding anything built
        /// for the previous one.
        /// </summary>
        public static void LoadTerrain(Ruleset ruleset, IUserInterface active,
            int renderScale = DefaultTerrainRenderScale)
        {
            BuiltSets.Remove(active);
            UseTerrainScale(ruleset, active, renderScale);
        }

        /// <summary>
        /// Switches the active terrain to a given composition scale, building it
        /// the first time that scale is asked for.
        /// </summary>
        public static void UseTerrainScale(Ruleset ruleset, IUserInterface active, int renderScale)
        {
            renderScale = Math.Clamp(renderScale, 1, MaximumTerrainRenderScale);

            if (!BuiltSets.TryGetValue(active, out var byScale))
            {
                byScale = new Dictionary<int, List<TerrainSet>>();
                BuiltSets[active] = byScale;
            }

            if (!byScale.TryGetValue(renderScale, out var sets))
            {
                sets = new List<TerrainSet>();
                for (var i = 0; i < active.ExpectedMaps; i++)
                {
                    sets.Add(LoadTerrain(ruleset, i, active, renderScale));
                }

                byScale[renderScale] = sets;
            }

            active.TileSets.Clear();
            foreach (var set in sets)
            {
                active.TileSets.Add(set);
            }
        }

        private static TerrainSet LoadTerrain(Ruleset ruleset, int index, IUserInterface active, int renderScale)
        {
            // Initialize objects
            var terrain = new TerrainSet(64, 32, renderScale);

            // Get dither tile before making it transparent.
            // Threshold every pixel to pure black or white so AlphaMask produces
            // only fully-transparent or fully-opaque regions (no blue crosshatching).
            var ditherTile = Images.ExtractBitmap(MapIndexChange((BitmapStorage)active.PicSources["dither"][0], index, active));
            unsafe
            {
                var pixels = ditherTile.LoadColors();
                for (var i = 0; i < pixels.Length; i++)
                {
                    var c = pixels[i];
                    var lum = (c.R + c.G + c.B) / 3;
                    var bw = lum < 128 ? Color.Black : Color.White;
                    var x = i % ditherTile.Width;
                    var y = i / ditherTile.Width;
                    ditherTile.DrawPixel(x, y, bw);
                }
                Image.UnloadColors(pixels);
            }

            terrain.BaseTiles = active.PicSources["base1"].Select(t => MapIndexChange((BitmapStorage)t, index, active)).ToArray();
            var fossTerrainApplied = ApplyFossTerrainTextures(terrain, index, active);
            terrain.HighResBaseTiles = fossTerrainApplied;

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
                BuildDitherMaps(terrain.DitherMask[0], terrain.BaseTiles, 32, 0, terrain.Blank, fossTerrainApplied),
                BuildDitherMaps(terrain.DitherMask[1], terrain.BaseTiles, 32, 16, terrain.Blank, fossTerrainApplied),
                BuildDitherMaps(terrain.DitherMask[2], terrain.BaseTiles, 0, 16, terrain.Blank, fossTerrainApplied),
                BuildDitherMaps(terrain.DitherMask[3], terrain.BaseTiles, 0, 0, terrain.Blank, fossTerrainApplied),
            };

            terrain.River = active.PicSources["river"].Select(r => MapIndexChange((BitmapStorage)r, index, active)).ToArray();
            terrain.Forest = active.PicSources["forest"].Select(r => MapIndexChange((BitmapStorage)r, index, active)).ToArray();
            terrain.Mountains = active.PicSources["mountain"].Select(r => MapIndexChange((BitmapStorage)r, index, active)).ToArray();
            terrain.Hills = active.PicSources["hill"].Select(r => MapIndexChange((BitmapStorage)r, index, active)).ToArray();
            terrain.RiverMouth = active.PicSources["riverMouth"].Select(r => MapIndexChange((BitmapStorage)r, index, active)).ToArray();

            if (fossTerrainApplied)
            {
                ApplyFossOverlayArt(terrain);
            }

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

            if (fossTerrainApplied)
            {
                var edgeWidth = terrain.TileWidth * terrain.RenderScale;
                var edgeHeight = terrain.TileHeight * terrain.RenderScale;
                terrain.ShallowEdge = new[]
                {
                    BuildShallowEdge(edgeWidth, edgeHeight, 0),
                    BuildShallowEdge(edgeWidth, edgeHeight, 1),
                    BuildShallowEdge(edgeWidth, edgeHeight, 2),
                    BuildShallowEdge(edgeWidth, edgeHeight, 3),
                };
            }

            return terrain;
        }

        /// <summary>
        /// Swaps the base terrain diamonds for the bundled FOSS textures, composed
        /// at the set's own render scale times Civ2's logical tile size.
        /// Returns true when the higher-resolution tiles were installed, so the
        /// connection overlays know they can be composed to match.
        /// </summary>
        private static bool ApplyFossTerrainTextures(TerrainSet terrain, int mapIndex, IUserInterface active)
        {
            // The bundled textures depict the classic Earth terrain set. Other Test of Time maps
            // retain their scenario-specific art until equivalent FOSS sets are available.
            if (mapIndex != 0)
            {
                return false;
            }

            var applied = false;
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

                // The bundled terrain diamonds are rendered as turf slabs with a
                // dark soil rim around the edge. Zoom a little past that rim
                // before fitting the art to the tile, otherwise every tile shows
                // its own border and the map reads as a grid of separate slabs.
                const float keep = 0.82f;
                replacement.Crop(new Rectangle(
                    replacement.Width * (1f - keep) / 2f,
                    replacement.Height * (1f - keep) / 2f,
                    replacement.Width * keep,
                    replacement.Height * keep));

                replacement.Resize(terrain.TileWidth * terrain.RenderScale,
                    terrain.TileHeight * terrain.RenderScale);
                ApplyDiamondAlpha(replacement);
                terrain.BaseTiles[terrainIndex] = new MemoryStorage(replacement,
                    $"FossTerrain-{terrainIndex}-{terrain.RenderScale}-{artPath}");
                applied = true;
            }

            return applied;
        }

        /// <summary>
        /// Connection-overlay sets bundled as native 300x300 art. Each set holds
        /// eight variants; the sixteen neighbour-connection indices cycle through
        /// them, matching how the compatibility sheet is generated.
        /// </summary>
        private static readonly (string Directory, string Stem)[] FossOverlaySets =
        [
            ("Rivers", "river"),
            ("Forest", "forest"),
            ("Mountains", "mountain"),
            ("Hills", "hill")
        ];

        private const int FossOverlayVariants = 8;

        /// <summary>
        /// Replaces the forest/hills/mountains/river connection overlays with the
        /// native high-resolution art, composed once at the working tile size.
        /// Routing them through the legacy 64x32 sheet cell and letting the tile
        /// compositor upscale it throws away half the detail the source carries.
        /// </summary>
        private static void ApplyFossOverlayArt(TerrainSet terrain)
        {
            foreach (var (directory, stem) in FossOverlaySets)
            {
                var variants = LoadFossOverlayVariants(terrain, directory, stem);
                if (variants == null)
                {
                    continue;
                }

                var target = stem switch
                {
                    "river" => terrain.River,
                    "forest" => terrain.Forest,
                    "mountain" => terrain.Mountains,
                    "hill" => terrain.Hills,
                    _ => null
                };

                if (target == null || target.Length == 0)
                {
                    continue;
                }

                for (var i = 0; i < target.Length; i++)
                {
                    target[i] = variants[i % FossOverlayVariants];
                }
            }
        }

        private static IImageSource[]? LoadFossOverlayVariants(TerrainSet terrain, string directory, string stem)
        {
            var variants = new IImageSource[FossOverlayVariants];
            for (var variant = 0; variant < FossOverlayVariants; variant++)
            {
                var path = FindFossOverlayPath(directory, $"{stem}_{variant + 1:00}.png");
                if (path == null)
                {
                    return null;
                }

                var composed = ComposeOverlayTile(terrain, path);
                if (composed == null)
                {
                    return null;
                }

                variants[variant] = new MemoryStorage(composed.Value,
                    $"FossOverlay-{stem}-{variant}-{terrain.RenderScale}");
            }

            return variants;
        }

        /// <summary>
        /// Fits square overlay art into a tile-sized transparent canvas, centred
        /// horizontally and resting on the bottom edge, so it lands where the
        /// classic sheet cell used to sit.
        /// </summary>
        private static Image? ComposeOverlayTile(TerrainSet terrain, string path)
        {
            var art = Images.LoadImageFromFile(path).Image;
            if (art.Width <= 1 || art.Height <= 1)
            {
                return null;
            }

            var targetWidth = terrain.TileWidth * terrain.RenderScale;
            var targetHeight = terrain.TileHeight * terrain.RenderScale;

            var scale = MathF.Min((float)targetWidth / art.Width, (float)targetHeight / art.Height);
            var drawWidth = Math.Max(1, (int)MathF.Round(art.Width * scale));
            var drawHeight = Math.Max(1, (int)MathF.Round(art.Height * scale));
            art.Resize(drawWidth, drawHeight);

            var canvas = Image.GenColor(targetWidth, targetHeight, Color.Blank);
            canvas.Draw(art,
                new Rectangle(0, 0, drawWidth, drawHeight),
                new Rectangle((targetWidth - drawWidth) / 2f, targetHeight - drawHeight, drawWidth, drawHeight),
                Color.White);
            art.Unload();
            return canvas;
        }

        private static string? FindFossOverlayPath(string directory, string fileName)
        {
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
                foreach (var candidate in new[]
                         {
                             Path.Combine(root, "Terrain", "Overlays", directory),
                             Path.Combine(root, "FOSSart", "Terrain", "Overlays", directory),
                             Path.Combine(root, "RaylibUI", "FOSSart", "Terrain", "Overlays", directory)
                         })
                {
                    var path = Path.Combine(candidate, fileName);
                    if (File.Exists(path))
                    {
                        return path;
                    }
                }
            }

            return null;
        }

        private static string? FindFossTerrainPath(string terrainName)
        {
            // PNG first: the newer terrain diamonds carry their own alpha, so they
            // do not depend on the compatibility sheet's mask to cut the corners.
            var fileNames = new[] { $"{terrainName}.png", $"{terrainName}.jpg" };
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
                    foreach (var fileName in fileNames)
                    {
                        var path = Path.Combine(directory, fileName);
                        if (File.Exists(path))
                        {
                            return path;
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Cuts the diamond out of a rectangular replacement texture with a hard
        /// edge, kept a pixel or two oversized so neighbouring tiles overlap
        /// along the join instead of leaving the view canvas showing through.
        /// A soft edge here bled the darker photo pixels just outside the
        /// diamond into a visible outline once the tile was drawn.
        /// </summary>
        private static void ApplyDiamondAlpha(Image image)
        {
            var width = image.Width;
            var height = image.Height;

            // ~1.5px of outward dilation in the taxicab distance field.
            var dilate = 3.0 / height;

            for (var y = 0; y < height; y++)
            {
                var ny = (y + 0.5) / height * 2.0 - 1.0;
                for (var x = 0; x < width; x++)
                {
                    var nx = (x + 0.5) / width * 2.0 - 1.0;

                    // Taxicab distance from tile centre: 1.0 on the diamond edge.
                    var d = Math.Abs(nx) + Math.Abs(ny);
                    if (d <= 1.0 + dilate)
                    {
                        continue;
                    }

                    var src = image.GetColor(x, y);
                    if (src.A != 0)
                    {
                        image.DrawPixel(x, y, new Color(src.R, src.G, src.B, (byte)0));
                    }
                }
            }
        }

        /// <summary>
        /// One shallow-water rim: a full tile-sized transparent image with a soft
        /// pale band fading in along a single diagonal edge. <paramref name="quadrant"/>
        /// is 0=NE, 1=SE, 2=SW, 3=NW, matching the dither quadrant order.
        /// </summary>
        private static Image BuildShallowEdge(int width, int height, int quadrant)
        {
            var eastHalf = quadrant is 0 or 1;
            var southHalf = quadrant is 1 or 2;

            // How far in from the edge the band reaches, and its peak opacity.
            const double band = 0.5;
            const double maxAlpha = 0.5;

            var shallow = Image.GenColor(width, height, Color.Blank);
            for (var y = 0; y < height; y++)
            {
                var ny = (y + 0.5) / height * 2.0 - 1.0;
                for (var x = 0; x < width; x++)
                {
                    var nx = (x + 0.5) / width * 2.0 - 1.0;

                    var inQuadrant = (eastHalf ? nx >= 0.0 : nx <= 0.0)
                                     && (southHalf ? ny >= 0.0 : ny <= 0.0);
                    if (!inQuadrant)
                    {
                        continue;
                    }

                    var d = Math.Abs(nx) + Math.Abs(ny);
                    if (d >= 1.0)
                    {
                        continue;
                    }

                    var f = Math.Clamp((d - (1.0 - band)) / band, 0.0, 1.0);
                    var alpha = (byte)Math.Clamp((int)Math.Round(f * f * maxAlpha * 255.0), 0, 255);
                    shallow.DrawPixel(x, y, new Color((byte)168, (byte)216, (byte)230, alpha));
                }
            }

            return shallow;
        }

        private static DitherMap BuildDitherMaps(Image mask, IImageSource[] baseTiles, int offsetX, int offsetY,
            IImageSource terrainBlank, bool feather)
        {
            var totalTiles = baseTiles.Length + 1;
            var ditherMaps = new Image[totalTiles];
            for (var i = 0; i < baseTiles.Length; i++)
            {
                var baseImage = Images.ExtractBitmap(baseTiles[i]);
                var scaleX = baseImage.Width / 64f;
                var scaleY = baseImage.Height / 32f;
                var scaledSampleRect = new Rectangle(offsetX * scaleX, offsetY * scaleY,
                    32 * scaleX, 16 * scaleY);
                ditherMaps[i] = Image.FromImage(baseImage, scaledSampleRect);

                if (feather)
                {
                    // Multiply the quadrant's own alpha by a soft ramp that is
                    // strongest along the shared diamond edge and gone by
                    // roughly half way to the centre, so the neighbouring
                    // terrain blends across the join instead of the classic
                    // hard checkerboard stipple. Multiplying (rather than
                    // AlphaMask, which replaces) keeps the diamond cut, so the
                    // darker pixels just outside the neighbour's diamond are
                    // not resurrected into an outline.
                    const double band = 0.55;
                    const double maxStrength = 0.72;
                    var mw = ditherMaps[i].Width;
                    var mh = ditherMaps[i].Height;
                    for (var py = 0; py < mh; py++)
                    {
                        var tileY = offsetY + (py + 0.5) / mh * 16.0;
                        var ny = tileY / 16.0 - 1.0;
                        for (var px = 0; px < mw; px++)
                        {
                            var tileX = offsetX + (px + 0.5) / mw * 32.0;
                            var nx = tileX / 32.0 - 1.0;

                            var d = Math.Abs(nx) + Math.Abs(ny);
                            var ramp = 0.0;
                            if (d < 1.0)
                            {
                                var t = Math.Clamp((d - (1.0 - band)) / band, 0.0, 1.0);
                                ramp = t * t * maxStrength;
                            }

                            var src = ditherMaps[i].GetColor(px, py);
                            var a = (byte)Math.Clamp((int)Math.Round(src.A * ramp), 0, 255);
                            ditherMaps[i].DrawPixel(px, py, new Color(src.R, src.G, src.B, a));
                        }
                    }
                }
                else
                {
                    var scaledMask = mask.Copy();
                    if (scaledMask.Width != ditherMaps[i].Width || scaledMask.Height != ditherMaps[i].Height)
                    {
                        scaledMask.ResizeNN(ditherMaps[i].Width, ditherMaps[i].Height);
                    }
                    ditherMaps[i].AlphaMask(scaledMask);
                    scaledMask.Unload();
                }
            }

            var sampleRect = new Rectangle(offsetX, offsetY, 32, 16);
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
