using Civ2engine.SaveLoad.SerializationUtils;
using Model.Core;
using Model.Core.GameRules;
using System.Linq;

namespace Civ2engine.SaveLoad;

public class JsonCivData
{
    public JsonCivData()
    {
        
    }
    public JsonCivData(Civilization civilization, Rules gameRules)
    {
        var tribe = gameRules.Leaders[civilization.TribeId];
        Alive = civilization.Alive;
        TribeId = civilization.TribeId;
        Gender = civilization.LeaderGender;
        GovernmentId = civilization.Government;
        Money = civilization.Money;
        Science = civilization.Science;
        ResearchingAdvance = civilization.ReseachingAdvance;
        Advances = civilization.Advances.Clamp();
        SciRate = civilization.ScienceRate;
        TaxRate = civilization.TaxRate;
        PlayerType = civilization.PlayerType;
        FutureTechCount = civilization.FutureTechCount;
        Patience = civilization.Patience;
        Betrayals = civilization.Betrayals;
        CasualtiesPerUnitType = civilization.CasualtiesPerUnitType.Clamp();
        Attitude = civilization.Attitude.Clamp();
        Reputation = civilization.Reputation.Clamp();
        Relations = civilization.Relations.Select(relation => relation?.Summary ?? 0).ToArray().Clamp();
        PowerRating = civilization.PowerRating.ToArray().Clamp();
        ThroneRoom = civilization.ThroneRoom.Clone();

        if ((Gender == 1 && civilization.LeaderName != tribe.NameFemale) ||
            (civilization.LeaderName != tribe.NameMale && Gender == 0))
        {
            LeaderName = civilization.LeaderName;
        }

        if (tribe.Plural != civilization.TribeName)
        {
            TribeName = civilization.TribeName;
        }
        if (tribe.Adjective != civilization.Adjective)
        {
            Adjective = civilization.Adjective;
        }

        if (civilization.PlayerType == PlayerType.Local)
        {
            CityStyle = civilization.CityStyle + 1;
        }
    }

    public PlayerType PlayerType { get; set; }

    public int TaxRate { get; set; }

    public string? Adjective { get; set; }

    public bool Alive { get; set; }
    public int CityStyle { get; set; }
    public int TribeId { get; set; }
    public int Gender { get; set; }
    public string? LeaderName { get; set; }
    public int GovernmentId { get; set; }
    public string? TribeName { get; set; }
    public int Money { get; set; }
    public int Science { get; set; }
    public int ResearchingAdvance { get; set; }
    public bool[]? Advances { get; set; }
    public int SciRate { get; set; }
    public int FutureTechCount { get; set; }
    public int Patience { get; set; }
    public int Betrayals { get; set; }
    public int[]? CasualtiesPerUnitType { get; set; }
    public int[]? Attitude { get; set; }
    public int[]? Reputation { get; set; }
    public int[]? Relations { get; set; }
    public int[]? PowerRating { get; set; }
    public ThroneRoom? ThroneRoom { get; set; }
}
