using System;
using System.Collections.Generic;
using System.Linq;
using Civ2engine.Enums;
using Civ2engine.MapObjects;
using Model.Constants;
using Model.Core;
using Model.Core.Cities;
using Model.Core.Mapping;

namespace Civ2engine;

/// <summary>
/// The final citizen attitudes produced by Civ II's five happiness passes.
/// UnhappyCitizens includes AngryCitizens, matching the values stored by Civ II.
/// </summary>
public readonly record struct CityHappiness(
    int HappyCitizens,
    int ContentCitizens,
    int UnhappyCitizens,
    int AngryCitizens,
    int Specialists)
{
    public bool IsInDisorder => UnhappyCitizens > HappyCitizens;
    public bool CanCelebrate(int citySize) =>
        citySize >= 3 && UnhappyCitizens == 0 && HappyCitizens * 2 >= citySize;
}

public static class CityHappinessExtensions
{
    /// <summary>
    /// Reproduces the ordering and integer arithmetic of the original Civ II
    /// happiness calculation: base mood, luxuries, improvements, government,
    /// and wonders. Each pass is normalized before the next one is applied.
    /// </summary>
    public static CityHappiness CalculateHappiness(this City city, IGame game)
    {
        var size = Math.Max(0, city.Size);
        var specialists = Math.Clamp(city.NoOfSpecialistsx4 / 4, 0, size);
        var state = new MoodState(size, specialists);

        ApplyBaseMood(city, game, ref state);
        state.Normalize();

        // Entertainers provide two luxuries; taxmen and scientists contribute to
        // their respective city outputs instead.
        state.Happy = city.GetLuxury(city.CountSpecialists(PeopleType.Elvis) * 2) / 2;
        state.Normalize();

        ApplyImprovements(city, ref state);
        state.Normalize();

        ApplyGovernment(city, ref state);
        state.Normalize();

        ApplyWonders(city, ref state);
        state.Normalize();

        var unhappy = Math.Clamp(state.Unhappy, 0, size - specialists);
        var angry = Math.Clamp(state.Angry, 0, unhappy);
        var happy = Math.Clamp(state.Happy, 0, size - specialists - unhappy);
        var content = Math.Max(0, size - specialists - happy - unhappy);
        var result = new CityHappiness(happy, content, unhappy, angry, specialists);

        city.HappyCitizens = result.HappyCitizens;
        city.UnhappyCitizens = result.UnhappyCitizens;
        return result;
    }

    public static PeopleType[] GetPeopleTypes(this City city, IGame game)
    {
        var mood = city.CalculateHappiness(game);
        var people = new List<PeopleType>(city.Size);
        people.AddRange(Enumerable.Repeat(PeopleType.Happy, mood.HappyCitizens));
        people.AddRange(Enumerable.Repeat(PeopleType.Content, mood.ContentCitizens));
        people.AddRange(Enumerable.Repeat(PeopleType.Unhappy,
            mood.UnhappyCitizens - mood.AngryCitizens));
        people.AddRange(Enumerable.Repeat(PeopleType.Angry, mood.AngryCitizens));
        people.AddRange(city.GetSpecialistTypes());
        return people.ToArray();
    }

    public static PeopleType[] GetSpecialistTypes(this City city)
    {
        var count = Math.Clamp(city.NoOfSpecialistsx4 / 4, 0, Math.Max(0, city.Size));
        var normalized = new PeopleType[count];
        for (var i = 0; i < count; i++)
        {
            normalized[i] = i < city.SpecialistTypes.Length &&
                            city.SpecialistTypes[i] is >= (int)PeopleType.Elvis and <= (int)PeopleType.Scientist
                ? (PeopleType)city.SpecialistTypes[i]
                : PeopleType.Elvis;
        }

        city.SpecialistTypes = normalized.Select(type => (int)type).ToArray();
        return normalized;
    }

    public static int CountSpecialists(this City city, PeopleType type) =>
        city.GetSpecialistTypes().Count(specialist => specialist == type);

    public static void SetSpecialistType(this City city, int index, PeopleType type)
    {
        var specialists = city.GetSpecialistTypes();
        if (index < 0 || index >= specialists.Length)
        {
            return;
        }

        specialists[index] = type is >= PeopleType.Elvis and <= PeopleType.Scientist
            ? type
            : PeopleType.Elvis;
        city.SpecialistTypes = specialists.Select(specialist => (int)specialist).ToArray();
    }

    private static void ApplyBaseMood(City city, IGame game, ref MoodState state)
    {
        var cosmic = game.Rules.Cosmic;
        var difficulty = Math.Clamp(game.DifficultyLevel, 0, (int)DifficultyType.Deity);

        // Computer players always use the King-level base population threshold.
        var baseUnhappy = city.Owner.PlayerType == PlayerType.Ai
            ? state.Size - 1 - (cosmic.CitySizeUnhappyChieftain - 5)
            : state.Size - 1 - ((cosmic.CitySizeUnhappyChieftain - difficulty) - 2);

        var empireUnhappy = 0;
        if (city.Owner.PlayerType != PlayerType.Ai &&
            city.Owner.Government != (int)GovernmentType.Communism)
        {
            var largeMapBonus = city.Location.Map.XDim * city.Location.Map.YDim >= 6000 ? 2 : 0;
            var riotFactor = Math.Max(1, cosmic.RiotFactor - 2 * difficulty + largeMapBonus);
            var governmentFactor = city.Owner.Government / 2 + 2;
            var combinedFactor = Math.Max(1, governmentFactor * riotFactor / 2);
            var cityIndex = Math.Max(0, city.Owner.Cities.IndexOf(city));
            empireUnhappy = Math.Max(0,
                (city.Owner.Cities.Count - combinedFactor + cityIndex % combinedFactor) /
                combinedFactor);
        }

        var totalUnhappy = Math.Max(0, baseUnhappy + empireUnhappy);
        state.Unhappy = Math.Min(totalUnhappy, state.Size);
        state.Angry = Math.Max(0, totalUnhappy - state.Size);
    }

    private static void ApplyImprovements(City city, ref MoodState state)
    {
        var civ = city.Owner;
        var government = (GovernmentType)civ.Government;

        if (city.ImprovementExists((int)ImprovementType.Colosseum))
        {
            state.Unhappy -= HasAdvance(civ, AdvanceType.Electronics) ? 4 : 3;
        }

        if (city.ImprovementExists((int)ImprovementType.Cathedral) ||
            OwnsActiveWonder(city, ImprovementType.MichChapel))
        {
            var effect = 3;
            if (government == GovernmentType.Communism) effect--;
            if (HasAdvance(civ, AdvanceType.Theology)) effect++;
            state.Unhappy -= effect;
        }

        if (city.ImprovementExists((int)ImprovementType.Temple))
        {
            var effect = 1 + (HasAdvance(civ, AdvanceType.Mysticism) ? 1 : 0);
            if (OwnsActiveWonder(city, ImprovementType.Oracle)) effect *= 2;
            state.Unhappy -= effect;
        }

        if (government == GovernmentType.Democracy &&
            (city.ImprovementExists((int)ImprovementType.Palace) ||
             city.ImprovementExists((int)ImprovementType.Courthouse)))
        {
            state.Happy++;
        }
    }

    private static void ApplyGovernment(City city, ref MoodState state)
    {
        var government = (GovernmentType)city.Owner.Government;
        if (government == GovernmentType.Fundamentalism)
        {
            state.Angry = 0;
            state.Unhappy = 0;
            return;
        }

        if (government is GovernmentType.Republic or GovernmentType.Democracy)
        {
            var deployedUnits = city.SupportedUnits.Count(UnitCausesFieldUnhappiness);
            if (government == GovernmentType.Republic && deployedUnits > 0) deployedUnits--;

            var penaltyPerUnit = government == GovernmentType.Democracy ? 2 : 1;
            if (city.ImprovementExists((int)ImprovementType.PoliceStat) ||
                OwnsActiveWonder(city, ImprovementType.WomenSuffrage))
            {
                penaltyPerUnit--;
            }

            state.Unhappy += deployedUnits * Math.Max(0, penaltyPerUnit);
            return;
        }

        var martialLawUnits = city.UnitsInCity.Count(unit => unit.Owner == city.Owner && unit.AttackBase > 0);
        var effectPerUnit = government == GovernmentType.Communism ? 2 : 1;
        state.Unhappy -= Math.Min(3, martialLawUnits) * effectPerUnit;
    }

    private static void ApplyWonders(City city, ref MoodState state)
    {
        var hangingGardens = FindActiveWonder(city, ImprovementType.HangingGardens);
        if (hangingGardens != null)
        {
            state.Happy += hangingGardens == city ? 3 : 1;
        }

        if (OwnsActiveWonder(city, ImprovementType.CureCancer)) state.Happy++;
        if (city.ImprovementExists((int)ImprovementType.ShakespTheat)) state.Unhappy = 0;

        var bach = FindActiveWonder(city, ImprovementType.JsbCathedral);
        if (bach != null && bach.Location.Map == city.Location.Map &&
            bach.Location.Island == city.Location.Island)
        {
            state.Unhappy -= 2;
        }
    }

    private static bool UnitCausesFieldUnhappiness(Model.Core.Units.Unit unit)
    {
        if (unit.AttackBase <= 0 || unit.AiRole is AiRoleType.Diplomacy or AiRoleType.Trade)
            return false;

        var location = unit.CurrentLocation;
        if (location.CityHere?.Owner == unit.Owner) return false;

        // Air-superiority units are the fighter exception in Civ II.
        if (unit.Domain == UnitGas.Air && unit.AiRole == AiRoleType.AirSuperiority) return false;

        if (location.Improvements.Any(i => i.Improvement == ImprovementTypes.Fortress) &&
            unit.Owner.Cities.Any(c => IsWithinThreeTiles(location, c.Location)))
        {
            return false;
        }

        return true;
    }

    private static bool IsWithinThreeTiles(Tile start, Tile destination)
    {
        if (start.Map != destination.Map) return false;
        if (start == destination) return true;

        var visited = new HashSet<Tile> { start };
        var frontier = new HashSet<Tile> { start };
        for (var distance = 1; distance <= 3; distance++)
        {
            frontier = frontier.SelectMany(t => t.Neighbours()).Where(visited.Add).ToHashSet();
            if (frontier.Contains(destination)) return true;
        }

        return false;
    }

    private static bool HasAdvance(Model.Core.Civilization civilization, AdvanceType advance) =>
        (int)advance < civilization.Advances.Length && civilization.Advances[(int)advance];

    private static bool OwnsActiveWonder(City city, ImprovementType wonder) =>
        FindActiveWonder(city, wonder) != null;

    private static City? FindActiveWonder(City city, ImprovementType wonder)
    {
        var wonderCity = city.Owner.Cities.FirstOrDefault(c => c.ImprovementExists((int)wonder));
        if (wonderCity == null) return null;

        var improvement = wonderCity.Improvements.First(i => i.Type == (int)wonder);
        return improvement.ExpiresAt >= 0 && HasAdvance(city.Owner, (AdvanceType)improvement.ExpiresAt)
            ? null
            : wonderCity;
    }

    private struct MoodState(int size, int specialists)
    {
        public int Size { get; } = size;
        public int Specialists { get; } = specialists;
        public int Happy;
        public int Unhappy;
        public int Angry;

        public void Normalize()
        {
            Happy = Math.Clamp(Happy, 0, Size);

            while (Angry != 0 && Angry > Unhappy)
            {
                Angry--;
                Unhappy++;
            }

            var availableCitizens = Math.Clamp(Size - Specialists, 0, 99);
            Unhappy = Math.Clamp(Unhappy, 0, Size);
            while (availableCitizens < Happy + Unhappy)
            {
                if (Angry != 0)
                {
                    Angry--;
                }
                else
                {
                    Happy = Math.Clamp(Happy - 1, 0, Size);
                }

                Unhappy = Math.Clamp(Unhappy - 1, 0, Size);
            }

            while (Angry != 0 && Size - Specialists > Happy + Unhappy)
            {
                Angry--;
                Unhappy++;
            }
        }
    }
}
