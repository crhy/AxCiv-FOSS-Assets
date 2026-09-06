using RhyCiv.Engine.UnitActions;
using Model.Core.Cities;
using Model.Core.Units;

namespace RhyCiv.Engine.Scripting.UnitActions;

public class CaptureAction(Unit baseUnit, City city, Game game) : TileAction(baseUnit, city.Location, "Capture", game)
{
    public override void Execute()
    {
        MovementFunctions.ExecuteUnitMove(game, BaseUnit, Tile, BaseUnit.CurrentLocation);
    }
}