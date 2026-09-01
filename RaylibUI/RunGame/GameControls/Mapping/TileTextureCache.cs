using Civ2engine.IO;
using Civ2engine.MapObjects;
using Model.Core.Mapping;
using Model.ImageSets;
using RaylibUI.RunGame.Commands;

namespace RaylibUI.RunGame.GameControls.Mapping;

public class TileTextureCache
{
    private readonly GameScreen _parentScreen;

    private readonly List<TileDetails?[,]> _mapTileTextures = new();

    /// <summary>
    /// Composed tiles in least-recently-used order, newest first. Tiles are built
    /// at the terrain set's render scale, so at high zoom each one is large; the
    /// cache is bounded by total pixels rather than by tile count.
    /// </summary>
    private readonly LinkedList<CacheKey> _recent = new();
    private readonly Dictionary<CacheKey, LinkedListNode<CacheKey>> _recentNodes = new();
    private long _cachedBytes;
    private long _generation;

    /// <summary>
    /// Roughly a handful of screenfuls at any zoom. Composing a tile is cheap
    /// next to holding every tile a long game has ever revealed.
    /// </summary>
    private const long CachedTileByteBudget = 192L * 1024 * 1024;

    private readonly record struct CacheKey(int MapIndex, int X, int Y);

    private readonly List<int> _seenMaps = new();
    private readonly List<TerrainSet> _tileSets = new();
    private readonly List<MapDimensions> _dimensions = new();
    private readonly Dictionary<(int MapIndex, int Zoom), MapDimensions> _scaledDimensions = new();

    public TileTextureCache(GameScreen parentScreen)
    {
        _parentScreen = parentScreen;
    }

    /// <summary>
    /// Marks the start of composing a view. Tiles drawn during this build are
    /// protected from eviction until the next one begins.
    /// </summary>
    public void BeginViewBuild()
    {
        _generation++;
    }

    public TileDetails GetTileDetails(Tile tile, int civilizationId)
    {
        var mapIndex = _seenMaps.IndexOf(tile.Map.MapIndex);
        if (mapIndex == -1)
        {
            mapIndex = SetupMap(tile.Map);
        }

        var key = new CacheKey(mapIndex, tile.XIndex, tile.Y);
        var cache = _mapTileTextures[mapIndex];
        var details = cache[tile.XIndex, tile.Y];
        if (details != null)
        {
            details.Generation = _generation;
            Touch(key);
            return details;
        }

        details = MapImage.MakeTileGraphic(tile, tile.Map, _tileSets[mapIndex], _parentScreen.Game, civilizationId);
        details.Generation = _generation;
        cache[tile.XIndex, tile.Y] = details;
        Track(key, details);
        TrimToBudget();
        return details;
    }

    private void Touch(CacheKey key)
    {
        if (_recentNodes.TryGetValue(key, out var node))
        {
            _recent.Remove(node);
            _recent.AddFirst(node);
        }
    }

    private void Track(CacheKey key, TileDetails details)
    {
        if (_recentNodes.TryGetValue(key, out var existing))
        {
            _recent.Remove(existing);
            _recentNodes.Remove(key);
        }

        _recentNodes[key] = _recent.AddFirst(key);
        _cachedBytes += ByteSize(details);
    }

    private void Forget(CacheKey key, TileDetails details)
    {
        if (_recentNodes.Remove(key, out var node))
        {
            _recent.Remove(node);
        }

        _cachedBytes -= ByteSize(details);
        if (_cachedBytes < 0)
        {
            _cachedBytes = 0;
        }
    }

    private static long ByteSize(TileDetails details) =>
        Math.Max(0L, (long)details.Image.Width * details.Image.Height * 4);

    /// <summary>
    /// Drops the coldest tiles until the cache is inside its budget, never
    /// touching a tile the view currently being composed has used.
    /// </summary>
    private void TrimToBudget()
    {
        var node = _recent.Last;
        while (_cachedBytes > CachedTileByteBudget && node != null)
        {
            var previous = node.Previous;
            var key = node.Value;
            var cached = _mapTileTextures[key.MapIndex][key.X, key.Y];
            if (cached != null && cached.Generation != _generation)
            {
                _mapTileTextures[key.MapIndex][key.X, key.Y] = null;
                Forget(key, cached);
                cached.Image.Unload();
            }
            else if (cached == null)
            {
                _recent.Remove(node);
                _recentNodes.Remove(key);
            }

            node = previous;
        }
    }

    private int SetupMap(Map map)
    {
        int mapIndex;
        mapIndex = _seenMaps.Count;
        _seenMaps.Add(map.MapIndex);
        _mapTileTextures.Add(new TileDetails?[map.XDim, map.YDim]);

        var tileSet = _parentScreen.Main.ActiveInterface.TileSets[map.MapIndex];
        _tileSets.Add(tileSet);
        _dimensions.Add(new MapDimensions
        {
            TotalWidth = map.Tile.GetLength(0) * tileSet.TileWidth,
            TotalHeight = map.Tile.GetLength(1) * tileSet.HalfHeight + tileSet.HalfHeight,
            HalfHeight = tileSet.HalfHeight,
            TileHeight = tileSet.TileHeight,
            TileWidth = tileSet.TileWidth,
            HalfWidth = tileSet.HalfWidth,
            DiagonalCut = tileSet.DiagonalCut,

        });
        return mapIndex;
    }

    public MapDimensions GetDimensions(Map map, int zoom)
    {
        if (_scaledDimensions.TryGetValue((map.MapIndex, zoom), out var scaled))
        {
            return scaled;
        }

        var cacheIndex = _seenMaps.IndexOf(map.MapIndex);
        if (cacheIndex == -1)
        {
            cacheIndex = SetupMap(map);
        }

        //return _dimensions[mapIndex];
        scaled = new MapDimensions
        {
            TotalWidth = _dimensions[cacheIndex].TotalWidth.ZoomScale(zoom),
            TotalHeight = _dimensions[cacheIndex].TotalHeight.ZoomScale(zoom),
            HalfHeight = _dimensions[cacheIndex].HalfHeight.ZoomScale(zoom),
            TileHeight = _dimensions[cacheIndex].TileHeight.ZoomScale(zoom),
            TileWidth = _dimensions[cacheIndex].TileWidth.ZoomScale(zoom),
            HalfWidth = _dimensions[cacheIndex].HalfWidth.ZoomScale(zoom),
            DiagonalCut = _dimensions[cacheIndex].DiagonalCut.ZoomScale(zoom).ZoomScale(zoom),
        };
        _scaledDimensions[(map.MapIndex, zoom)] = scaled;
        return scaled;
    }

    public void Redraw(Tile tile, int civilizationId)
    {
        var mapIndex = _seenMaps.IndexOf(tile.Map.MapIndex);
        if (mapIndex == -1)
        {
            mapIndex = SetupMap(tile.Map);
        }

        var key = new CacheKey(mapIndex, tile.XIndex, tile.Y);
        var cache = _mapTileTextures[mapIndex];
        var previous = cache[tile.XIndex, tile.Y];
        if (previous != null)
        {
            Forget(key, previous);
            previous.Image.Unload();
        }

        var replacement = MapImage.MakeTileGraphic(tile, tile.Map, _tileSets[mapIndex], _parentScreen.Game, civilizationId);
        replacement.Generation = _generation;
        cache[tile.XIndex, tile.Y] = replacement;
        Track(key, replacement);
    }

    public void Clear()
    {
        foreach (var mapTextures in _mapTileTextures)
        {
            foreach (var details in mapTextures)
            {
                details?.Image.Unload();
            }
        }
        _seenMaps.Clear();
        _mapTileTextures.Clear();
        _dimensions.Clear();
        _scaledDimensions.Clear();
        _tileSets.Clear();
        _recent.Clear();
        _recentNodes.Clear();
        _cachedBytes = 0;
    }
}
