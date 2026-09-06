using RhyCiv.Engine.MapObjects;
using Model.Core.Mapping;
using Model.Core.Units;

namespace RhyCiv.Engine.Scripting.UnitActions;

public abstract class TileAction(Unit baseUnit, Tile tile, string type, Game game) : UnitAction(baseUnit, type, game, tile)
{
    public Tile Tile { get; } = tile;
}