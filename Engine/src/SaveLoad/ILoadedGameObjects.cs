using System.Collections.Generic;
using RhyCiv.Engine.MapObjects;
using RhyCiv.Engine.Units;
using Model;
using Model.Core;
using Model.Core.Cities;
using Model.Core.Mapping;
using Model.Core.Units;

namespace RhyCiv.Engine.LegacySaves;

public interface ILoadedGameObjects
{
    Unit ActiveUnit { get; }
    Scenario Scenario { get; }
    List<City> Cities { get; set; }
    List<Transporter> Transporters { get; set; }
    List<Civilization> Civilizations { get; set; }
    List<Map> Maps { get; set; }
    IGameData GameData { get; set; }
    Options Options { get; set; }
}