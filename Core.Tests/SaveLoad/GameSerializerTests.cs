using Civ2engine;
using Civ2engine.Enums;
using Civ2engine.IO;
using Civ2engine.MapObjects;
using Civ2engine.SaveLoad;
using Civ2engine.Terrains;
using Model.Core;
using Model.Core.Player;
using Model.Core.Units;
using Model.Core.Cities;
using Model.Core.Advances;
using Model.Core.Mapping;
using Moq;
using System.Text.Json;
using Model.Core.GameRules;

namespace Core.Tests.SaveLoad;

public class GameSerializerTests
{
    [Fact]
    public void WriteAndRead_RoundTrip_Works()
    {
        // Create temp dir for scripts
        var tempPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(tempPath);
        File.WriteAllText(Path.Combine(tempPath, "game_setup.lua"), "");
        File.WriteAllText(Path.Combine(tempPath, "tile_improvements.lua"), "");
        File.WriteAllText(Path.Combine(tempPath, "improvements.lua"), "");
        File.WriteAllText(Path.Combine(tempPath, "advances.lua"), "");
        File.WriteAllText(Path.Combine(tempPath, "units.lua"), "");

        // Arrange
        Labels.Items = new string[1000];
        for (int i = 0; i < 1000; i++) Labels.Items[i] = "Label" + i;

        var terrain = new Terrain { 
            Name = "Plains", 
            Type = TerrainType.Plains, 
            Specials = new[] { 
                new Special { Name = "Wheat" }, 
                new Special { Name = "Buffalo" } 
            } 
        };
        var rules = new Rules
        {
            UnitTypes = new[] { 
                new UnitDefinition { Name = "None", Type = 0, Flags = new bool[20] },
                new UnitDefinition { Name = "Settler", Type = 1, Flags = new bool[20] } 
            },
            Leaders = new[] { new LeaderDefaults { NameMale = "Cesar", Plural = "Romans", Adjective = "Roman", Titles = Array.Empty<LeaderTitle>() } },
            Governments = new[] { new Government { Name = "Despotism", Level = 0 } },
            Improvements = new[] { 
                new Improvement { Name = "None", Type = 0 }, 
                new Improvement { Name = "Palace", Type = 1 } 
            },
            CaravanCommoditie = new[] { new Commodity { Name = "Silk" } },
            Terrains = new List<Terrain[]> { Enumerable.Repeat(terrain, 11).ToArray() },
            Advances = Array.Empty<Advance>()
        };
        var ruleset = new Ruleset("test", new Dictionary<string, string> { ["Author"] = "Junie" }, tempPath);
        
        var relation = new Relation { Contact = true, Peace = true, Embassy = true };
        var civ = new Civilization
        {
            Id = 1,
            TribeId = 0,
            Alive = true,
            PlayerType = PlayerType.Local,
            LeaderGender = 1,
            Advances = new bool[10],
            Science = 17,
            ReseachingAdvance = -1,
            FutureTechCount = 3,
            Patience = 6,
            Betrayals = 2,
            CasualtiesPerUnitType = [0, 4],
            Attitude = [0, 42],
            Reputation = [0, 7],
            Relations = [null, relation],
            PowerRating = [12, 15],
            ThroneRoom = new ThroneRoom { Floor = 3, DecorStatues = true },
            GlobalEffects = { }
        };
        var game = new Mock<IGame>();
        game.Setup(g => g.AllCivilizations).Returns(new List<Civilization> { Barbarians.Civilization, civ });
        game.Setup(g => g.GetPlayerCiv).Returns(civ);
        game.Setup(g => g.Rules).Returns(rules);
        game.Setup(g => g.Options).Returns(new Options());
        
        var map = new Map(true, 0) { XDim = 10, YDim = 10 };
        map.Tile = new Tile[10, 10];
        for (int y = 0; y < 10; y++)
        {
            for (int x_idx = 0; x_idx < 10; x_idx++)
            {
                int x = x_idx * 2 + y % 2;
                map.Tile[x_idx, y] = new Tile(x, y, terrain, 0, map, x_idx + y * 10, new bool[2]);
            }
        }
        game.Setup(g => g.Maps).Returns(new List<Map> { map });
        var island = new IslandDetails();
        island.Id = 0;
        foreach (var t in map.Tile) island.Tiles.Add(t);
        map.Islands.Add(island);
        
        var unitTile = map.Tile[2, 2];
        var unit = new Unit { 
            Owner = civ, 
            TypeDefinition = rules.UnitTypes[1], 
            Order = (int)OrderType.NoOrders, 
            MapIndex = 0, 
            X = unitTile.X, 
            Y = unitTile.Y, 
            CurrentLocation = unitTile 
        };
        civ.Units.Add(unit);

        var cityTile = map.Tile[3, 3];
        var city = new City { 
            Owner = civ, 
            WhoBuiltIt = civ, 
            Location = cityTile, 
            X = cityTile.X, 
            Y = cityTile.Y, 
            MapIndex = 0, 
            Name = "Rome", 
            Size = 3,
            NoOfSpecialistsx4 = 8,
            SpecialistTypes = [(int)PeopleType.Taxman, (int)PeopleType.Scientist]
        };
        civ.Cities.Add(city);
        game.Setup(g => g.AllCities).Returns(new List<City> { city });
        
        var gameDate = new Mock<IGameDate>();
        gameDate.Setup(d => d.StartingYear).Returns(-4000);
        gameDate.Setup(d => d.TurnYearIncrement).Returns(50);
        game.Setup(g => g.Date).Returns(gameDate.Object);
        game.Setup(g => g.CitiesBuiltSoFar).Returns(new Dictionary<Civilization, int>());
        
        var encoder = new ImprovementEncoder(new Dictionary<int, TerrainImprovement>());
        game.Setup(g => g.ImprovementEncoder).Returns(encoder);
        
        var activePlayer = new Mock<IPlayer>();
        activePlayer.Setup(p => p.ActiveUnit).Returns(unit);
        game.Setup(g => g.ActivePlayer).Returns(activePlayer.Object);
        
        var viewData = new Dictionary<string, string> { ["Zoom"] = "1.0" };
        
        using var ms = new MemoryStream();
        var serializer = new GameSerializer();
        
        // Act - Write
        serializer.Write(ms, game.Object, ruleset, viewData);
        ms.Position = 0;
        
        // Act - Read
        var jsonDoc = JsonDocument.Parse(ms);
        var gameElement = jsonDoc.RootElement.GetProperty("game");
        var loadedGame = GameSerializer.Read(gameElement, ruleset, rules);
        
        // Assert
        Assert.NotNull(loadedGame);
        Assert.Equal(2, loadedGame.AllCivilizations.Count);
        Assert.Equal(civ.TribeId, loadedGame.AllCivilizations[1].TribeId);
        Assert.Equal(civ.LeaderGender, loadedGame.AllCivilizations[1].LeaderGender);
        Assert.Equal(civ.Science, loadedGame.AllCivilizations[1].Science);
        Assert.Equal(civ.ReseachingAdvance, loadedGame.AllCivilizations[1].ReseachingAdvance);
        Assert.Equal(civ.FutureTechCount, loadedGame.AllCivilizations[1].FutureTechCount);
        Assert.Equal(civ.Patience, loadedGame.AllCivilizations[1].Patience);
        Assert.Equal(civ.Betrayals, loadedGame.AllCivilizations[1].Betrayals);
        Assert.Equal(civ.CasualtiesPerUnitType, loadedGame.AllCivilizations[1].CasualtiesPerUnitType);
        Assert.Equal(civ.Attitude, loadedGame.AllCivilizations[1].Attitude);
        Assert.Equal(civ.Reputation, loadedGame.AllCivilizations[1].Reputation);
        Assert.Equal(relation.Summary, loadedGame.AllCivilizations[1].Relations[1]!.Summary);
        Assert.Equal(civ.PowerRating, loadedGame.AllCivilizations[1].PowerRating);
        Assert.Equal(3, loadedGame.AllCivilizations[1].ThroneRoom.Floor);
        Assert.True(loadedGame.AllCivilizations[1].ThroneRoom.DecorStatues);
        Assert.Equal(map.XDim, loadedGame.Maps[0].XDim);
        Assert.Equal(map.YDim, loadedGame.Maps[0].YDim);
        Assert.Single(loadedGame.AllCivilizations[1].Units);
        Assert.Single(loadedGame.AllCities);
        Assert.Equal(8, loadedGame.AllCities[0].NoOfSpecialistsx4);
        Assert.Equal(city.SpecialistTypes, loadedGame.AllCities[0].SpecialistTypes);
    }
}
