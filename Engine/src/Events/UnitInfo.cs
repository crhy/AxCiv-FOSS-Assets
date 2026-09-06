using System.Collections.Generic;
using RhyCiv.Engine.Enums;
using RhyCiv.Engine.MapObjects;
using RhyCiv.Engine.Units;
using Model.Core;
using Model.Core.Mapping;
using Model.Core.Units;

namespace RhyCiv.Engine.Events
{
    public class UnitInfo : IUnit
    {
        public  Tile CurrentLocation { get; }

        public readonly List<int> Hitpoints;

        public UnitInfo(Unit unit, List<int> hitpoints)
        {
            CurrentLocation = unit.CurrentLocation;
            HitpointsBase = unit.HitpointsBase;
            RemainingHitpoints = unit.RemainingHitpoints;
            Type = unit.Type;
            Order = unit.Order;
            Owner = unit.Owner;
            IsInStack = unit.IsInStack;
            
            Hitpoints = hitpoints;
        }

        public int HitpointsBase { get; }
        public int RemainingHitpoints { get; }
        public int Type { get; set; }
        public int Order { get; set; }
        public Civilization Owner { get; set; }
        public bool IsInStack { get; }
    }
}