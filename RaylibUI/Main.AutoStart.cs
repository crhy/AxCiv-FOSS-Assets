using System;
using System.Collections.Generic;
using System.Linq;
using Civ2engine.MapObjects;
using Civ2;
using Civ2engine;
using Civ2engine.NewGame;
using CivInit = Civ2.Rules.Initialization;

namespace RaylibUI
{
    public partial class Main
    {
        // Boot straight into a freshly generated game, skipping every new-game
        // dialog, when RHYCIV_AUTOSTART is set. Intended for UI review and
        // screenshot capture of live gameplay without a human at the menu.
        //   RHYCIV_AUTOSTART=1        enable
        //   RHYCIV_AUTOSTART_SEED=N   deterministic map/start (default: time-based)
        //   RHYCIV_AUTOSTART_CIVS=N   number of rival civilisations (default: max)
        //   RHYCIV_AUTOSTART_ZOOM=N   initial map zoom, -7..32 (default: -1)
        //   RHYCIV_AUTOSTART_REVEAL=1 reveal the whole map (terrain review)
        private bool TryAutoStartGame()
        {
            if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("RHYCIV_AUTOSTART")))
            {
                return false;
            }

            if (ActiveInterface is not Civ2Interface civ2)
            {
                Console.WriteLine("autostart: active interface is not the Civ2 interface; skipping");
                return false;
            }

            // RHYCIV_AUTOSTART=quick exercises the menu's Quick start entry rather
            // than this harness's own settings, so that path can be checked without
            // clicking through the menu.
            if (string.Equals(Environment.GetEnvironmentVariable("RHYCIV_AUTOSTART"), "quick",
                    StringComparison.OrdinalIgnoreCase))
            {
                var quickAction = civ2.StartInstantGame();
                if (quickAction is not Model.InterfaceActions.StartGame started)
                {
                    Console.WriteLine("autostart: quick start did not produce a game");
                    return false;
                }

                var quickGame = started.Game;
                Console.WriteLine(
                    $"autostart: quick start, {quickGame.Maps[0].XDim}x{quickGame.Maps[0].YDim} world, " +
                    $"{quickGame.AllCivilizations.Count} civs incl. barbarians, " +
                    $"player '{quickGame.GetPlayerCiv.TribeName}', difficulty {quickGame.DifficultyLevel}, " +
                    $"units {quickGame.GetPlayerCiv.Units.Count}");
                StartGame(quickGame, CivInit.ViewData);
                return true;
            }

            CivInit.LoadGraphicsAssets(civ2);

            var config = CivInit.ConfigObject;
            config.Random = int.TryParse(Environment.GetEnvironmentVariable("RHYCIV_AUTOSTART_SEED"), out var seed)
                ? new FastRandom(seed)
                : new FastRandom();
            config.QuickStart = true;
            config.WorldSize = new[] { 50, 80 };
            config.BarbarianActivity = 1;
            config.DifficultyLevel = 2;

            var maxCivs = civ2.PlayerColours.Length - 1;
            config.NumberOfCivs = int.TryParse(Environment.GetEnvironmentVariable("RHYCIV_AUTOSTART_CIVS"), out var civs)
                ? Math.Clamp(civs + 1, 2, maxCivs)
                : maxCivs;

            config.PlayerCiv = CivInit.MakeCivilization(config, config.Rules.Leaders[0], true, 1);
            config.Gender = config.PlayerCiv.LeaderGender;

            CivInit.CompleteConfig();

            var maps = MapGenerator.GenerateMap(config).GetAwaiter().GetResult();
            var game = NewGameInitialisation.StartNewGame(config, maps, config.Civilizations,
                civ2.MainApp.ActiveRuleSet.Paths);
            CivInit.Start(game);

            var zoom = int.TryParse(Environment.GetEnvironmentVariable("RHYCIV_AUTOSTART_ZOOM"), out var z) ? z : -1;
            CivInit.ViewData = new Dictionary<string, string?> { ["Zoom"] = zoom.ToString() };

            var reveal = !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("RHYCIV_AUTOSTART_REVEAL"));
            if (reveal)
            {
                foreach (var map in game.Maps)
                {
                    map.MapRevealed = true;
                }
            }

            Console.WriteLine(
                $"autostart: generated {config.WorldSize[0]}x{config.WorldSize[1]} world, " +
                $"{config.Civilizations.Count} civs, player '{game.GetPlayerCiv.TribeName}', zoom {zoom}");

            StartGame(game, CivInit.ViewData);

            if (reveal && _activeScreen is RunGame.GameScreen gameScreen)
            {
                gameScreen.TileCache.Clear();
                gameScreen.MapControl.ForceRedraw = true;
            }

            // RHYCIV_TEST_CITY=N founds N cities and opens the last one's city
            // screen, so crashes on that path can be reproduced without playing to
            // them. N defaults to 1. Each city after the first is founded from a
            // separate settler walked far enough away to clear the adjacency rule,
            // which is the path the second-city crash report describes.
            var testCityValue = Environment.GetEnvironmentVariable("RHYCIV_TEST_CITY");
            if (!string.IsNullOrWhiteSpace(testCityValue)
                && _activeScreen is RunGame.GameScreen cityScreen)
            {
                var wanted = int.TryParse(testCityValue, out var count) ? Math.Max(1, count) : 1;
                RunCityFoundingHarness(game, cityScreen, wanted);
            }

            // RHYCIV_TEST_POPUP=NAME[,NAME...] pops the named GAME.TXT dialog(s)
            // right after start, with placeholder text, so prompt layout can be
            // reviewed without playing to the event that triggers it.
            var testPopups = Environment.GetEnvironmentVariable("RHYCIV_TEST_POPUP");
            if (!string.IsNullOrWhiteSpace(testPopups) && _activeScreen is RunGame.GameScreen screen)
            {
                var fillers = new List<string>
                {
                    game.GetPlayerCiv.TribeName, "Babylon", "the Wonder of the Ages",
                    "an aqueduct", "scholars", "the Hanging Gardens", "Marketplace",
                };
                foreach (var name in testPopups.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
                {
                    screen.ShowPopup(name, replaceStrings: fillers,
                        replaceNumbers: new List<int> { 42, 120, 7 });
                }
            }

            return true;
        }

        /// <summary>
        /// Founds <paramref name="wanted"/> cities for the human player, reporting each
        /// step so an unhandled exception can be pinned to the city it happened on.
        /// </summary>
        private static void RunCityFoundingHarness(Game game, RunGame.GameScreen screen, int wanted)
        {
            var civ = game.GetPlayerCiv;
            Model.Core.Cities.City? last = null;

            for (var founded = 0; founded < wanted; founded++)
            {
                var settler = civ.Units.FirstOrDefault(u =>
                    !u.Dead && u.AiRole == Model.Constants.AiRoleType.Settle);
                if (settler == null)
                {
                    Console.WriteLine($"test-city: no settler left after {founded} cities");
                    break;
                }

                // Walk clear of every existing city: founding adjacent to one is
                // rejected, and the walk itself exercises the movement path.
                var attempts = 0;
                while (attempts++ < 40 && !CanFoundHere(settler.CurrentLocation, civ))
                {
                    var step = settler.CurrentLocation.Neighbours()
                        .FirstOrDefault(t => t.Type != Model.Core.Mapping.TerrainType.Ocean &&
                                             !t.Terrain.Impassable);
                    if (step == null)
                    {
                        break;
                    }

                    settler.X = step.X;
                    settler.Y = step.Y;
                    settler.CurrentLocation = step;
                }

                if (!CanFoundHere(settler.CurrentLocation, civ))
                {
                    Console.WriteLine($"test-city: could not find a legal site for city {founded + 1}");
                    break;
                }

                var name = Civ2engine.UnitActions.CityActions.GetCityName(civ, game);
                Console.WriteLine($"test-city: founding city {founded + 1} '{name}' at " +
                                  $"{settler.CurrentLocation.X},{settler.CurrentLocation.Y}");
                last = Civ2engine.UnitActions.CityActions.BuildCity(settler, game, name);
                Console.WriteLine($"test-city: founded '{last.Name}' size {last.Size}, production " +
                                  (last.ItemInProduction == null ? "<null>" : last.ItemInProduction.ToString()));

                // Everything the real order does after BuildCity.
                last.Location.SetVisible(civ.Id);
                last.Location.UpdatePlayer(civ.Id);
                Console.WriteLine($"test-city: city {founded + 1} tile published");
            }

            if (last != null)
            {
                // RHYCIV_TEST_CITY_SIZE grows the city before its window opens, so the
                // citizen row has something in it to inspect.
                if (int.TryParse(Environment.GetEnvironmentVariable("RHYCIV_TEST_CITY_SIZE"), out var size)
                    && size > last.Size)
                {
                    while (last.Size < size)
                    {
                        last.Size++;
                        last.AutoAddDistributionWorkers(game.Rules);
                    }

                    last.CalculateOutput(last.Owner.Government, game);
                    Console.WriteLine($"test-city: grown to size {last.Size}");
                }

                if (int.TryParse(Environment.GetEnvironmentVariable("RHYCIV_TEST_CITY_SHIELDS"), out var sh))
                {
                    last.ShieldsProgress = sh;
                    Console.WriteLine($"test-city: shields set to {last.ShieldsProgress}/{last.ItemInProduction.Cost}");
                }

                Console.WriteLine($"test-city: units in city {last.UnitsInCity.Count} " +
                                  $"[{string.Join(", ", last.UnitsInCity.Select(u => $"{u.Name} dead={u.Dead} home={(u.HomeCity == null ? "NONE" : u.HomeCity.Name)}"))}]");
                Console.WriteLine($"test-city: supported {last.SupportedUnits.Count} " +
                                  $"[{string.Join(", ", last.SupportedUnits.Select(u => $"{u.Name} dead={u.Dead}"))}]");

                screen.ShowCityWindow(last);
                Console.WriteLine("test-city: city window opened");
            }

            // Founding the last settler's city leaves nobody awaiting orders, so the
            // real order's ChooseNextUnit call runs the first end of turn. That is
            // the step the second-city crash report actually reaches, and skipping
            // it is why this harness could not reproduce the report before.
            Console.WriteLine("test-city: choosing next unit (may end the turn)");
            game.ChooseNextUnit();
            Console.WriteLine($"test-city: next unit chosen, turn {game.TurnNumber}");

            // With the last settler spent nobody is awaiting orders, so the player's
            // next act is to end the turn. RHYCIV_TEST_TURNS=N runs that many.
            var turns = int.TryParse(Environment.GetEnvironmentVariable("RHYCIV_TEST_TURNS"), out var t)
                ? t
                : 0;
            for (var i = 0; i < turns; i++)
            {
                Console.WriteLine($"test-city: ending turn {game.TurnNumber}");
                if (game.ProcessEndOfTurn())
                {
                    game.ChoseNextCiv();
                }

                Console.WriteLine($"test-city: turn is now {game.TurnNumber}");
                foreach (var c in civ.Cities)
                {
                    Console.WriteLine($"test-city:   {c.Name} size {c.Size} shields {c.ShieldsProgress}" +
                                      $"/{c.ItemInProduction.Cost} producing {c.ItemInProduction.GetDescription()}" +
                                      $" (+{c.Production}/turn) disorder={c.CivilDisorder}");
                }

                Console.WriteLine($"test-city:   units {civ.Units.Count(u => !u.Dead)}");
            }
        }

        private static bool CanFoundHere(Model.Core.Mapping.Tile tile, Model.Core.Civilization civ)
        {
            if (tile.Type == Model.Core.Mapping.TerrainType.Ocean || tile.Terrain.Impassable ||
                tile.CityHere != null)
            {
                return false;
            }

            return !tile.Neighbours().Any(t => t.IsCityPresent);
        }

    }
}
