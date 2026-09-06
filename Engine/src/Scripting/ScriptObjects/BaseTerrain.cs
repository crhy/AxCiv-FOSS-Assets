using Model.Core.Mapping;

namespace RhyCiv.Engine.Scripting;

public class BaseTerrain(Terrain terrain, int map)
{
    private readonly int _map = map;

    public Terrain Terrain { get; } = terrain;

    public bool isOcean => Terrain.Type == TerrainType.Ocean;
}
