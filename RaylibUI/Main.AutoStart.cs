using System;
using System.Collections.Generic;
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
    }
}
