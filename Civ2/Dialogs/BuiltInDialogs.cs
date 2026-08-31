using Civ2engine.IO;
using Model.Core;

namespace Civ2.Dialogs;

/// <summary>
/// Clean-room dialog copy used by the standalone ruleset.  Compatibility
/// rulesets may still override any entry through their own GAME.txt file.
/// </summary>
internal static class BuiltInDialogs
{
    public static void AddFallbacks(Dictionary<string, PopupBox> dialogs)
    {
        Add(dialogs, "MAINMENU", "rhYciv", ["New game", "Load map", "Custom world", "Load scenario", "Load game"], ["OK", "Exit"]);
        Add(dialogs, "SIZEOFMAP", "World size", ["Small", "Medium", "Large"], ["Custom", "OK", "Cancel"]);
        Add(dialogs, "CUSTOMSIZE", "Custom world size", ["Width", "Height"], ["OK", "Cancel"]);
        Add(dialogs, "DIFFICULTY", "Difficulty", ["Chieftain", "Warlord", "Prince", "King", "Emperor", "Deity"], ["OK", "Cancel"]);
        Add(dialogs, "ENEMIES", "Rival civilizations", ["8 Civilizations"], ["OK", "Cancel"]);
        Add(dialogs, "BARBARITY", "Barbarian activity", ["Villages only", "Roving bands", "Restless tribes", "Raging hordes"], ["OK", "Cancel"]);
        Add(dialogs, "RULES", "Game rules", ["Standard conquest", "Advanced options"], ["OK", "Cancel"]);
        Add(dialogs, "ADVANCED", "Advanced rules",
            ["Simplified combat", "Flat world", "Choose computer rivals", "Accelerated start", "Conquest-only victory", "Permanent elimination"],
            ["OK", "Cancel"], checkbox: true);
        Add(dialogs, "ACCELERATED", "Starting era", ["4000 BC", "3000 BC", "2000 BC"], ["OK", "Cancel"]);
        Add(dialogs, "GENDER", "Leader", ["Male", "Female"], ["OK", "Cancel"]);
        Add(dialogs, "TRIBE", "Choose a civilization", [], ["OK", "Custom", "Cancel"]);
        Add(dialogs, "NAME", "Name your leader", ["Leader name"], ["OK", "Cancel"]);
        Add(dialogs, "CUSTOMTRIBE", "Customize civilization", ["Leader", "Civilization", "Adjective"], ["OK", "Titles", "Cancel"]);
        Add(dialogs, "CUSTOMTRIBE2", "Government titles", [], ["OK", "Cancel"]);
        Add(dialogs, "CUSTOMCITY", "City style", [], ["OK", "Cancel"]);
        Add(dialogs, "OPPONENT", "Choose a rival", ["Random civilization"], ["OK", "Cancel"]);
        Add(dialogs, "INIT", "The first turn", [], ["Begin"]);

        AddRandom(dialogs, "CUSTOMLAND", "Land coverage", ["Sparse", "Balanced", "Abundant"]);
        AddRandom(dialogs, "CUSTOMFORM", "Land form", ["Archipelagos", "Continents", "Pangaea"]);
        AddRandom(dialogs, "CUSTOMCLIMATE", "Climate", ["Dry", "Temperate", "Wet"]);
        AddRandom(dialogs, "CUSTOMTEMP", "Temperature", ["Cool", "Temperate", "Warm"]);
        AddRandom(dialogs, "CUSTOMAGE", "World age", ["Young", "Mature", "Old"]);
        Add(dialogs, "USESEED", "Map resources", ["Randomize resources", "Keep map resources"], ["OK", "Cancel"]);
        Add(dialogs, "USESTARTLOC", "Starting locations", ["Randomize starts", "Keep map starts"], ["OK", "Cancel"]);
        Add(dialogs, "FAILEDTOLOAD", "Could not load map", [], ["OK"]);
        Add(dialogs, "LOADOK", "Game loaded", [], ["Continue"]);
        Add(dialogs, "SCENCHOSECIV", "Choose a civilization", [], ["OK", "Cancel"]);
        Add(dialogs, "SCENINTRO", "Scenario", [], ["Continue"]);
        Add(dialogs, "SCENCUSTOMINTRO", "Scenario", [], ["OK", "Cancel"]);
        Add(dialogs, "SCENARIOLOADED", "Scenario loaded", [], ["Continue"]);
    }

    public static PopupBox Generic(string name) => new()
    {
        Name = name,
        Width = 440,
        Title = name,
        Button = [Labels.Ok],
        Text = [],
        Options = []
    };

    private static void AddRandom(Dictionary<string, PopupBox> dialogs, string name, string title, IList<string> options) =>
        Add(dialogs, name, title, options, ["Random", "OK", "Cancel"]);

    private static void Add(Dictionary<string, PopupBox> dialogs, string name, string title,
        IList<string> options, IList<string> buttons, bool checkbox = false)
    {
        if (dialogs.ContainsKey(name)) return;
        dialogs[name] = new PopupBox
        {
            Name = name,
            Width = 440,
            Title = title,
            Button = buttons,
            Options = options,
            Checkbox = checkbox,
            Text = []
        };
    }
}
