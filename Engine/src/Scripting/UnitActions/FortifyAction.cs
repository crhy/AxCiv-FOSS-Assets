using RhyCiv.Engine.Enums;
using Model.Core.Units;

namespace RhyCiv.Engine.Scripting.UnitActions;

public class FortifyAction(Unit baseUnit, Game game) : FullTurnAction(baseUnit, "Fortify", game)
{
    protected override void DoAction()
    {
        BaseUnit.Order = (int)OrderType.Fortify;
    }
}