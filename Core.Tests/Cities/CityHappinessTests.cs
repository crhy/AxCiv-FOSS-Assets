using Civ2engine;
using Civ2engine.Enums;
using Model.Core;
using Model.Core.Cities;
using Model.Core.GameRules;
using Model.Core.Mapping;
using Model.Core.Units;
using Moq;

namespace Core.Tests.Cities;

public class CityHappinessTests
{
    [Fact]
    public void NewVillageCitizenIsContent_NotAutomaticallyHappy()
    {
        var (game, city, _) = CreateCity(DifficultyType.Prince, size: 1);

        var mood = city.CalculateHappiness(game.Object);

        Assert.Equal(0, mood.HappyCitizens);
        Assert.Equal(1, mood.ContentCitizens);
        Assert.Equal(0, mood.UnhappyCitizens);
        Assert.Equal([PeopleType.Content], city.GetPeopleTypes(game.Object));
    }

    [Fact]
    public void UndefendedDeityVillageNeedsMartialLawAsItGrows()
    {
        var (game, city, civ) = CreateCity(DifficultyType.Deity, size: 2);

        var undefended = city.CalculateHappiness(game.Object);
        Assert.Equal(1, undefended.ContentCitizens);
        Assert.Equal(1, undefended.UnhappyCitizens);
        Assert.True(undefended.IsInDisorder);

        AddWarrior(civ, city, city.Location);
        var defended = city.CalculateHappiness(game.Object);

        Assert.Equal(0, defended.HappyCitizens);
        Assert.Equal(2, defended.ContentCitizens);
        Assert.Equal(0, defended.UnhappyCitizens);
        Assert.False(defended.IsInDisorder);
    }

    [Theory]
    [InlineData(DifficultyType.Chieftain, 6)]
    [InlineData(DifficultyType.Warlord, 5)]
    [InlineData(DifficultyType.Prince, 4)]
    [InlineData(DifficultyType.King, 3)]
    [InlineData(DifficultyType.Emperor, 2)]
    [InlineData(DifficultyType.Deity, 1)]
    public void DifficultyControlsNumberOfBornContentCitizens(
        DifficultyType difficulty, int bornContent)
    {
        var (game, city, _) = CreateCity(difficulty, bornContent);
        Assert.Equal(0, city.CalculateHappiness(game.Object).UnhappyCitizens);

        city.Size++;
        Assert.Equal(1, city.CalculateHappiness(game.Object).UnhappyCitizens);
    }

    [Fact]
    public void TwoLuxuriesMakeOneContentCitizenHappy()
    {
        var (game, city, civ) = CreateCity(DifficultyType.Chieftain, size: 2);
        civ.TaxRate = 0;
        civ.ScienceRate = 0;
        city.Trade = 2;

        var mood = city.CalculateHappiness(game.Object);

        Assert.Equal(1, mood.HappyCitizens);
        Assert.Equal(1, mood.ContentCitizens);
        Assert.Equal(0, mood.UnhappyCitizens);
    }

    [Fact]
    public void TempleEffectIncreasesAfterMysticism()
    {
        var (game, city, civ) = CreateCity(DifficultyType.Deity, size: 3);
        city.AddImprovement(new Improvement { Type = (int)ImprovementType.Temple });

        Assert.Equal(1, city.CalculateHappiness(game.Object).UnhappyCitizens);

        civ.Advances[(int)AdvanceType.Mysticism] = true;
        Assert.Equal(0, city.CalculateHappiness(game.Object).UnhappyCitizens);
    }

    [Fact]
    public void MartialLawIsCappedAtThreeCombatUnits_AndDoubledByCommunism()
    {
        var (game, city, civ) = CreateCity(DifficultyType.Deity, size: 8);
        for (var i = 0; i < 4; i++) AddWarrior(civ, city, city.Location);

        civ.Government = (int)GovernmentType.Despotism;
        Assert.Equal(4, city.CalculateHappiness(game.Object).UnhappyCitizens);

        civ.Government = (int)GovernmentType.Communism;
        Assert.Equal(1, city.CalculateHappiness(game.Object).UnhappyCitizens);
    }

    [Fact]
    public void FundamentalismEliminatesUnhappyCitizens()
    {
        var (game, city, civ) = CreateCity(DifficultyType.Deity, size: 8);
        civ.Government = (int)GovernmentType.Fundamentalism;

        var mood = city.CalculateHappiness(game.Object);

        Assert.Equal(0, mood.UnhappyCitizens);
        Assert.Equal(8, mood.ContentCitizens);
    }

    [Fact]
    public void RepublicOnlyPenalizesTheSecondDeployedCombatUnit()
    {
        var (game, city, civ) = CreateCity(DifficultyType.Chieftain, size: 3);
        civ.Government = (int)GovernmentType.Republic;
        var fieldTile = city.Location.Map.Tile[0, 0];
        AddWarrior(civ, city, fieldTile);

        Assert.Equal(0, city.CalculateHappiness(game.Object).UnhappyCitizens);

        AddWarrior(civ, city, fieldTile);
        Assert.Equal(1, city.CalculateHappiness(game.Object).UnhappyCitizens);

        city.AddImprovement(new Improvement { Type = (int)ImprovementType.PoliceStat });
        Assert.Equal(0, city.CalculateHappiness(game.Object).UnhappyCitizens);
    }

    [Fact]
    public void CelebrationRequiresAtLeastThreeCitizens()
    {
        Assert.False(new CityHappiness(1, 0, 0, 0, 0).CanCelebrate(1));
        Assert.True(new CityHappiness(2, 1, 0, 0, 0).CanCelebrate(3));
        Assert.False(new CityHappiness(1, 1, 1, 0, 0).CanCelebrate(3));
    }

    [Fact]
    public void RiotFactorAddsStaggeredEmpireSizeUnhappiness()
    {
        var (game, city, civ) = CreateCity(DifficultyType.Deity, size: 1);
        for (var i = 1; i < 12; i++)
        {
            civ.Cities.Add(new City
            {
                Owner = civ,
                Location = city.Location,
                Size = 1,
                Name = $"City {i}"
            });
        }

        var mood = city.CalculateHappiness(game.Object);

        Assert.Equal(1, mood.UnhappyCitizens);
        Assert.Equal(1, mood.AngryCitizens);
    }

    private static (Mock<IGame> Game, City City, Civilization Civ) CreateCity(
        DifficultyType difficulty, int size)
    {
        var rules = new Rules
        {
            Governments = Enumerable.Range(0, 7).Select(_ => new Government()).ToArray()
        };
        rules.Cosmic.CitySizeUnhappyChieftain = 7;
        rules.Cosmic.RiotFactor = 14;

        var map = new Map(flat: false, index: 0)
        {
            XDim = 10,
            YDim = 10,
            Tile = new Tile[10, 10]
        };
        var terrain = new Terrain
        {
            Name = "Plains",
            Type = TerrainType.Plains,
            Defense = 100,
            Specials = []
        };
        for (var y = 0; y < map.YDim; y++)
        {
            for (var x = 0; x < map.XDim; x++)
            {
                map.Tile[x, y] = new Tile(2 * x + y % 2, y, terrain, 0, map, x, new bool[2]);
            }
        }

        var civ = new Civilization
        {
            Id = 1,
            Government = (int)GovernmentType.Despotism,
            PlayerType = PlayerType.Local,
            Advances = new bool[100],
            TaxRate = 40,
            ScienceRate = 60
        };
        var city = new City
        {
            Owner = civ,
            WhoBuiltIt = civ,
            Location = map.Tile[4, 4],
            Size = size,
            Name = "Test City"
        };
        city.Location.CityHere = city;
        civ.Cities.Add(city);

        var game = new Mock<IGame>();
        game.SetupGet(g => g.Rules).Returns(rules);
        game.SetupGet(g => g.DifficultyLevel).Returns((int)difficulty);
        game.SetupGet(g => g.Maps).Returns([map]);
        return (game, city, civ);
    }

    private static Unit AddWarrior(Civilization civ, City homeCity, Tile location)
    {
        var unit = new Unit
        {
            Owner = civ,
            HomeCity = homeCity,
            TypeDefinition = new UnitDefinition
            {
                Attack = 1,
                Defense = 1,
                Flags = new bool[20]
            },
            CurrentLocation = location
        };
        civ.Units.Add(unit);
        return unit;
    }
}
