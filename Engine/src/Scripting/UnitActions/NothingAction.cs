using Model.Core.Units;

namespace RhyCiv.Engine.Scripting.UnitActions;

public class NothingAction(Unit baseUnit, Game game) : FullTurnAction(baseUnit, "Nothing", game)
{
    protected override void DoAction()
    {
        
    }
}