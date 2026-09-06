using System;
using System.Collections.Generic;
using RhyCiv.Engine.Enums;
using RhyCiv.Engine.Units;
using Model.Core.Mapping;
using Model.Core.Units;

namespace RhyCiv.Engine.Events
{
    public abstract class UnitEventArgs : EventArgs
    {
        public IList<Tile> Location { get; }
        public UnitEventType EventType { get; }
        
        protected UnitEventArgs(UnitEventType eventType, IList<Tile> locations)
        {
            Location = locations;
            EventType = eventType;
        }
    }

    public class ActivationEventArgs : UnitEventArgs
    {
        public bool UserInitiated { get; }
        public bool Reactivation { get; }

        public ActivationEventArgs(Unit unit, bool userInitiated, bool reactivation) : base(UnitEventType.NewUnitActivated, new[] { unit.CurrentLocation })
        {
            UserInitiated = userInitiated;
            Reactivation = reactivation;
            Unit = unit;
        }

        public Unit Unit { get; }  
    }
}
