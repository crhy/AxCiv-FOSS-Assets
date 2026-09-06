using System.Collections.Generic;
using RhyCiv.Engine.MapObjects;
using RhyCiv.Engine.LegacySaves;
using RhyCiv.Engine.Units;
using Model;
using Model.Core;
using Model.Core.Cities;
using Model.Core.Mapping;
using Model.Core.Units;

namespace RhyCiv.Engine.SaveLoad;

public class JsonSaveObjects : ILoadedGameObjects
{
    public Unit ActiveUnit { get; set; } = null!;
    public Scenario Scenario { get; } = null!;
    public List<City> Cities { get; set; } = [];
    
    public List<Transporter> Transporters { get; set; } = [];
    public List<Civilization> Civilizations { get; set; } = [];
    public List<Map> Maps { get; set; } = [];
    public IGameData GameData { get; set; } = null!;
    public Options Options { get; set; } = null!;
}
