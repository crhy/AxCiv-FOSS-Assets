using System;
using System.Linq;
using RhyCiv.Engine.Enums;
using RhyCiv.Engine.MapObjects;
using RhyCiv.Engine.Terrains;
using RhyCiv.Engine.Units;
using Model.Core;
using Model.Core.Mapping;
using Model.Core.Units;

namespace RhyCiv.Engine.UnitActions
{
    public static class UnitFunctions
    {
        public static bool CanFortifyHere(Unit unit, Tile tile)
        {
            return unit.Domain switch
            {
                UnitGas.Ground => tile.Terrain.Type != TerrainType.Ocean,
                UnitGas.Air => tile.CityHere is not null || tile.EffectsList.Any(e => e.Target == ImprovementConstants.Airbase),
                UnitGas.Sea => tile.CityHere is not null,
                UnitGas.Special => true,
                _ => true
            };
        }

        public static bool CanEnter(UnitGas domain, Tile tile)
        {
            return domain switch
            {
                UnitGas.Ground => tile.Terrain.Type != TerrainType.Ocean,
                UnitGas.Air => true,
                UnitGas.Sea => tile.Terrain.Type == TerrainType.Ocean || tile.CityHere is not null,
                UnitGas.Special => true,
            };
        }
    }
}