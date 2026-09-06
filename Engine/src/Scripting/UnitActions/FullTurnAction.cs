using Model.Core.Units;

namespace RhyCiv.Engine.Scripting.UnitActions;

public abstract class FullTurnAction(Unit baseUnit, string type, Game game) : UnitAction(baseUnit, type, game)
{
    public override void Execute()
    {
        DoAction();
        BaseUnit.MovePointsLost = BaseUnit.MaxMovePoints;
    }

    protected abstract void DoAction();
}