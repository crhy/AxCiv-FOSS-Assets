using RhyCiv.Engine.MapObjects;
using RhyCiv.Engine.UnitActions;
using Model.Core.Mapping;
using Model.Core.Units;

namespace RhyCiv.Engine.Scripting.UnitActions;

public class AttackAction(Unit baseUnit, Tile tile, Game game) : TileAction(baseUnit, tile, "Attack", game)
{
    public override void Execute()
    {
        MovementFunctions.AttackAtTile(BaseUnit, game, Tile);
    }
}