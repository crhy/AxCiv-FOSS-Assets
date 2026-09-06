using RhyCiv.Engine.MapObjects;
using RhyCiv.Engine.UnitActions;
using Model.Core.Mapping;
using Model.Core.Units;

namespace RhyCiv.Engine.Scripting.UnitActions;

public class MoveAction(Unit baseUnit, Tile possibleMove, Game game) : TileAction(baseUnit, possibleMove, "Move", game)
{
    public override void Execute()
    {
        MovementFunctions.ExecuteUnitMove(game, BaseUnit, Tile, BaseUnit.CurrentLocation);
    }
}