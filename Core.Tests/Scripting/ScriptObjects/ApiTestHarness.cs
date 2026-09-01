using Civ2engine;
using Civ2engine.MapObjects;
using Civ2engine.Scripting.ScriptObjects;
using Core.Tests.TestFiles;
using Model.Core;
using Model.Core.Cities;
using Model.Core.Mapping;
using Model.Core.Units;

namespace Core.Tests.Scripting.ScriptObjects;

public static class ApiTestHarness
{
    public static (Game game, AiPlayer aiPlayer, Civilization civ) CreateGameAndAi()
    {
        var (game, _, _) = CleanRoomGameFactory.CreateGame();

        var civ = game.AllCivilizations.First(c => c.PlayerType != PlayerType.Barbarians);

        // Ensure the player is an AiPlayer
        if (game.Players[civ.Id] is not AiPlayer aiPlayer)
        {
            var aiInterface = new AiInterface(game, civ, 0, game.Script);
            aiPlayer = new AiPlayer(0, civ, game.Maps[0].Tile[0, 0], game, aiInterface);
            game.Players[civ.Id] = aiPlayer;
        }

        return (game, aiPlayer, civ);
    }

    public static Tile FindEmptyTile(Game game)
    {
        foreach (var tile in game.Maps[0].Tile)
        {
            if (tile.CityHere == null && tile.UnitsHere.Count == 0)
            {
                return tile;
            }
        }

        throw new InvalidOperationException("No empty tile found for tests.");
    }

    public static Unit CreateUnit(Civilization civ, UnitDefinition type, Tile tile, bool veteran = false)
    {
        var unit = new Unit
        {
            Id = civ.Units.Count,
            Owner = civ,
            TypeDefinition = type,
            CurrentLocation = tile,
            X = tile.X,
            Y = tile.Y,
            MapIndex = tile.Z,
            Veteran = veteran
        };

        civ.Units.Add(unit);
        // tile.UnitsHere.Add(unit); // Already added in CreateUnit? Check if necessary
        return unit;
    }

    public static City CreateCity(Game game, Civilization civ, Tile tile, string name = "Testopolis", int size = 1)
    {
        var city = new City
        {
            Owner = civ,
            WhoBuiltIt = civ,
            Name = name,
            Size = size,
            Location = tile,
            X = tile.X,
            Y = tile.Y,
            MapIndex = tile.Z
        };

        tile.CityHere = city;
        civ.Cities.Add(city);
        game.AllCities.Add(city);

        return city;
    }

}
