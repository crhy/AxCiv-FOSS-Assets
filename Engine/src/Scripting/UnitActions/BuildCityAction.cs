using RhyCiv.Engine.UnitActions;
using Model.Core.Units;

namespace RhyCiv.Engine.Scripting.UnitActions;

public class BuildCityAction(Unit baseUnit, Game game, string? name = null) : FullTurnAction(baseUnit, "BuildCity", game)
{
    protected override void DoAction()
    {
        CityActions.BuildCity(BaseUnit, game, name ?? CityActions.GetCityName(BaseUnit.Owner, game));
    }
}