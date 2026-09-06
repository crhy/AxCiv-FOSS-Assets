using Model.Core;

namespace RhyCiv.Engine.SaveLoad;

/// <summary>
/// Encapsulate and map the fields of the game object that need to be serialized
/// </summary>
public class JsonGameData : IGameData
{
    /// <summary>
    /// Slots in the saved per-tribe city counter. Fixed by the save format, and
    /// Game.LoadGame reads the array's own length back, so it is only ever the
    /// width written here.
    /// </summary>
    private const int TribeSlots = 21;

    /// <summary>
    /// This constructor is to deserialize data
    /// </summary>
    public JsonGameData()
    {
        
    }

    /// <summary>
    /// This constructor provides mapping from the game object for serialization
    /// </summary>
    /// <param name="game"></param>
    public JsonGameData(IGame game)
    {
        DifficultyLevel = game.DifficultyLevel;
        TurnNumber = game.TurnNumber;
        StartingYear = game.Date.StartingYear == -4000 ? 0 : game.Date.StartingYear;
        TurnYearIncrement = game.Date.TurnYearIncrement;
        BarbarianActivity = game.BarbarianActivity;
        NoPollutionSkulls = game.PollutionSkulls;
        GlobalTempRiseOccured = game.GlobalTempRiseOccured;
        NoOfTurnsOfPeace = game.NoOfTurnsOfPeace;

        // Indexed by TribeId, which is how Game.LoadGame reads it back. The
        // barbarians carry TribeId -1 and the load side already skips them
        // explicitly, but nothing skipped them here: CityActions.BuildCity records a
        // count for whichever civilisation founded a city, barbarians included, so
        // the first barbarian city made this index the array with -1 and every save
        // from then on threw IndexOutOfRangeException. Anything outside the array is
        // dropped rather than trusted, which also covers a ruleset with more tribes
        // than the format has slots.
        CitiesBuiltSoFar = new int[TribeSlots];
        foreach (var civAndCityCount in game.CitiesBuiltSoFar)
        {
            int tribeId = civAndCityCount.Key.TribeId;
            if (tribeId < 0 || tribeId >= CitiesBuiltSoFar.Length)
            {
                continue;
            }

            CitiesBuiltSoFar[tribeId] = civAndCityCount.Value;
        }
    }
    public int DifficultyLevel { set; get; }
    public int TurnNumber { set; get; }
    public int StartingYear { set; get; }
    public int TurnYearIncrement { set; get; }
    public int BarbarianActivity { set; get; }
    public int NoPollutionSkulls { set; get; }
    public int GlobalTempRiseOccured { set; get; }
    public int NoOfTurnsOfPeace { set; get; }
    public int[] CitiesBuiltSoFar { set; get; }
}