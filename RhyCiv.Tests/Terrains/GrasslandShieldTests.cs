using RhyCiv.Engine.MapObjects;
using Model.Core.Mapping;

namespace RhyCiv.Tests.Terrains;

/// <summary>
/// The grassland shield marker says the square yields a shield. It did not: the
/// ruleset gives grassland zero shields, and a shielded square simply read that
/// same figure, so the marker promised production the tile never produced.
/// Whether a square carries a shield is fixed by its coordinates, so these walk
/// the map and use whichever tiles actually have one.
/// </summary>
public class GrasslandShieldTests
{
    [Fact]
    public void ShieldedGrassland_YieldsAShield()
    {
        var (shielded, _) = GrasslandPair();

        Assert.Equal(1, shielded.GetShields(false));
    }

    [Fact]
    public void PlainGrassland_YieldsNothing()
    {
        var (_, plain) = GrasslandPair();

        Assert.Equal(0, plain.GetShields(false));
    }

    /// <summary>One grassland tile with a shield and one without.</summary>
    private static (Tile Shielded, Tile Plain) GrasslandPair()
    {
        var map = new Map(true, 0) { Tile = new Tile[16, 16], XDim = 16, YDim = 16 };
        var grassland = new Terrain
        {
            Type = TerrainType.Grassland,
            Specials = [],
            Shields = 0,
            Food = 2,
        };

        Tile? shielded = null, plain = null;
        for (var y = 0; y < 16 && (shielded == null || plain == null); y++)
        {
            for (var x = 0; x < 16 && (shielded == null || plain == null); x++)
            {
                var tile = new Tile(x, y, grassland, 1, map, x, new bool[2]);
                map.Tile[x, y] = tile;
                if (tile.HasShield) shielded ??= tile;
                else plain ??= tile;
            }
        }

        Assert.NotNull(shielded);
        Assert.NotNull(plain);
        return (shielded, plain);
    }
}
