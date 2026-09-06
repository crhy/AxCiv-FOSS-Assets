using RhyCiv.Engine.MapObjects;
using RhyCiv.Engine.Units;
using Model.Core.Mapping;
using Model.Core.Units;

// `Path` here is the engine's unit movement path, not System.IO.Path, which
// ImplicitUsings now brings into scope.
using Path = RhyCiv.Engine.Units.Path;
namespace RhyCiv.Engine.Scripting.UnitActions;

internal class GotoAction(Unit baseUnit, Tile tile, Game game) : TileAction(baseUnit, tile, "Goto", game)
{
    public override void Execute()
    {
        var path = Path.CalculatePathBetween(game, BaseUnit.CurrentLocation, Tile, BaseUnit.Domain, BaseUnit.MaxMovePoints, BaseUnit.Owner, BaseUnit.Alpine, BaseUnit.IgnoreZonesOfControl, false);
        path?.Follow(game, BaseUnit);
    }
}