using RhyCiv.Engine.MapObjects;
using Model.Core;
using Model.Core.Mapping;

namespace RhyCiv.Engine.Scripting;

public class TerrainApi(Tile tile)
{
    public bool isOcean => tile.Terrain.Type == TerrainType.Ocean;
}