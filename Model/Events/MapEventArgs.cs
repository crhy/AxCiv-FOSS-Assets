using System;
using System.Collections.Generic;
using RhyCiv.Engine.Enums;
using Model.Core.Mapping;

namespace RhyCiv.Engine.Events
{
    public class MapEventArgs : EventArgs
    {
        public MapEventType EventType { get; }
        public List<Tile> TilesChanged { get; set; } = [];

        public int[] MapStartXy = [];
        public int[] MapDrawSq = [];
        public int Zoom, Xshift;

        public MapEventArgs(MapEventType eventType)
        {
            EventType = eventType;
        }
    }
}
