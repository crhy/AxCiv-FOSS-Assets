using System.Linq;
using RhyCiv.Engine.MapObjects;
using RhyCiv.Engine.UnitActions;
using Model.Core.Mapping;
using Model.Core.Units;

namespace RhyCiv.Engine.Scripting.UnitActions;

public class InvasionAction(Unit baseUnit, Tile possibleMove, Game game) : TileAction(baseUnit, possibleMove, "Invasion", game)
{
    public override void Execute()
    {
        var unitToMove = BaseUnit.CarriedUnits.FirstOrDefault(u=>u.CanMakeAmphibiousAssaults);
        if(unitToMove != null)
        {
            MovementFunctions.AttackAtTile(unitToMove, game, Tile);
        }
    }
}