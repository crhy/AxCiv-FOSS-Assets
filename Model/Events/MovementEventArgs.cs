using RhyCiv.Engine.Enums;
using RhyCiv.Engine.Units;
using Model.Core.Mapping;
using Model.Core.Units;

namespace RhyCiv.Engine.Events
{
    public class MovementEventArgs : UnitEventArgs
    {
        public MovementEventArgs(Unit unit, Tile tileFrom, Tile tileTo) : base(UnitEventType.MoveCommand, new [] { tileFrom, tileTo })
        {
            Unit = unit;
        }

        public Unit Unit { get; }  
    }
}