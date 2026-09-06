using RhyCiv.Engine.Enums;
using RhyCiv.Engine.Units;
using Model.Core.Units;

namespace RhyCiv.Engine.Events
{
    public class MovementBlockedEventArgs(IUnit subjectUnit, BlockedReason reason)
        : UnitEventArgs(UnitEventType.MovementBlocked, [subjectUnit.CurrentLocation])
    {
        public BlockedReason Reason { get; set; } = reason;
    }
}