using Civ2engine;
using Model.Core;
using Model.Core.GameRules;
using Model.Core.Mapping;

namespace Core.Tests.MapGeneration;

public class Civ2WorldGeneratorTests
{
    [Fact]
    public void SameSeedAndSettingsProduceSameWorld()
    {
        var first = Civ2WorldGenerator.Generate(Config(24680), 50, 80);
        var second = Civ2WorldGenerator.Generate(Config(24680), 50, 80);

        for (var y = 0; y < 80; y++)
        for (var x = 0; x < 50; x++)
        {
            Assert.Equal(first.Terrain[x, y], second.Terrain[x, y]);
            Assert.Equal(first.Rivers[x, y], second.Rivers[x, y]);
        }
    }

    [Fact]
    public void LandMassSettingChangesWorldCoverageInTheExpectedOrder()
    {
        var small = Civ2WorldGenerator.Generate(Config(1024, propLand: 0), 50, 80);
        var normal = Civ2WorldGenerator.Generate(Config(1024, propLand: 1), 50, 80);
        var large = Civ2WorldGenerator.Generate(Config(1024, propLand: 2), 50, 80);

        Assert.True(CountLand(small) < CountLand(normal));
        Assert.True(CountLand(normal) < CountLand(large));
        Assert.InRange(CountLand(normal) / 4000d, 0.44, 0.54);
    }

    [Fact]
    public void WetClimateCreatesMoreConnectedRiverTilesThanAridClimate()
    {
        var arid = Civ2WorldGenerator.Generate(Config(777, climate: 0), 50, 80);
        var wet = Civ2WorldGenerator.Generate(Config(777, climate: 2), 50, 80);

        Assert.True(CountRivers(wet) > CountRivers(arid));
        Assert.All(RiverCells(wet), cell => Assert.Contains(
            Neighbours(cell.X, cell.Y, 50, 80),
            neighbour => wet.Rivers[neighbour.X, neighbour.Y] ||
                         wet.Terrain[neighbour.X, neighbour.Y] == TerrainType.Ocean));
    }

    [Fact]
    public void YoungWorldIsMoreRuggedThanOldWorld()
    {
        var young = Civ2WorldGenerator.Generate(Config(9001, age: 0), 50, 80);
        var old = Civ2WorldGenerator.Generate(Config(9001, age: 2), 50, 80);

        Assert.True(Count(young, TerrainType.Hills, TerrainType.Mountains) >
                    Count(old, TerrainType.Hills, TerrainType.Mountains));
    }

    [Fact]
    public void ArchipelagoCreatesMoreSeparateLandRegionsThanContinents()
    {
        var archipelagoConfig = Config(31415);
        archipelagoConfig.FlatWorld = true;
        archipelagoConfig.Landform = 0;
        var continentsConfig = Config(31415);
        continentsConfig.FlatWorld = true;
        continentsConfig.Landform = 2;

        var archipelago = Civ2WorldGenerator.Generate(archipelagoConfig, 50, 80);
        var continents = Civ2WorldGenerator.Generate(continentsConfig, 50, 80);

        Assert.True(CountLandRegions(archipelago, wrapX: false) > CountLandRegions(continents, wrapX: false));
    }

    [Fact]
    public async Task MapGeneratorBuildsAPlayableMapFromGeneratedWorld()
    {
        var terrains = Enum.GetValues<TerrainType>()
            .Select(type => new Terrain { Type = type, Name = type.ToString(), Food = type == TerrainType.Ocean ? 1 : 2 })
            .ToArray();
        var config = Config(8181);
        config.WorldSize = [40, 50];
        config.NumberOfCivs = 7;
        config.Rules = new Rules { Terrains = [terrains], Maps = [] };

        var maps = await MapGenerator.GenerateMap(config);

        var map = Assert.Single(maps);
        Assert.Equal(40, map.Tile.GetLength(0));
        Assert.Equal(50, map.Tile.GetLength(1));
        Assert.NotEmpty(map.Islands);
        Assert.All(map.Tile.Cast<Tile>().Where(tile => tile.River),
            tile => Assert.NotEqual(TerrainType.Ocean, tile.Type));
    }

    private static GameInitializationConfig Config(int seed, int propLand = 1, int climate = 1, int age = 1) =>
        new()
        {
            Random = new FastRandom(seed),
            PropLand = propLand,
            Landform = 1,
            Climate = climate,
            Temperature = 1,
            Age = age
        };

    private static int CountLand(GeneratedWorld world) =>
        world.Terrain.Cast<TerrainType>().Count(type => type != TerrainType.Ocean);

    private static int CountRivers(GeneratedWorld world) => world.Rivers.Cast<bool>().Count(value => value);

    private static int Count(GeneratedWorld world, params TerrainType[] types) =>
        world.Terrain.Cast<TerrainType>().Count(types.Contains);

    private static int CountLandRegions(GeneratedWorld world, bool wrapX)
    {
        var width = world.Terrain.GetLength(0);
        var height = world.Terrain.GetLength(1);
        var remaining = new HashSet<(int X, int Y)>();
        for (var y = 0; y < height; y++)
        for (var x = 0; x < width; x++)
            if (world.Terrain[x, y] != TerrainType.Ocean) remaining.Add((x, y));

        var regions = 0;
        while (remaining.Count > 0)
        {
            regions++;
            var queue = new Queue<(int X, int Y)>();
            var first = remaining.First();
            remaining.Remove(first);
            queue.Enqueue(first);
            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                foreach (var neighbour in Neighbours(current.X, current.Y, width, height, wrapX))
                {
                    if (!remaining.Remove(neighbour)) continue;
                    queue.Enqueue(neighbour);
                }
            }
        }
        return regions;
    }

    private static IEnumerable<(int X, int Y)> RiverCells(GeneratedWorld world)
    {
        for (var y = 0; y < world.Rivers.GetLength(1); y++)
        for (var x = 0; x < world.Rivers.GetLength(0); x++)
            if (world.Rivers[x, y]) yield return (x, y);
    }

    private static IEnumerable<(int X, int Y)> Neighbours(int x, int y, int width, int height,
        bool wrapX = true)
    {
        var odd = y & 1;
        int[][] offsets =
        [
            [odd, -1], [1, 0], [odd, 1], [0, 2],
            [-1 + odd, 1], [-1, 0], [-1 + odd, -1], [0, -2]
        ];
        foreach (var offset in offsets)
        {
            var nx = x + offset[0];
            var ny = y + offset[1];
            if (wrapX) nx = (nx + width) % width;
            if (nx >= 0 && nx < width && ny >= 0 && ny < height) yield return (nx, ny);
        }
    }
}
