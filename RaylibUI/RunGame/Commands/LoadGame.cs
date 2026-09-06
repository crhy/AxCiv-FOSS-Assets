using Model.Input;
using RhyCiv.Engine;
using RhyCiv.Engine.Diagnostics;
using RhyCiv.Engine.IO;
using Model.Controls;
using Raylib_CSharp.Interact;

namespace RaylibUI.RunGame.Commands;

public class LoadGame(GameScreen gameScreen) : AlwaysOnCommand(gameScreen, CommandIds.LoadGame, [new Shortcut(Key.L, ctrl: true)])
{
    private FileDialog _loadDialog = null!;

    public override void Action()
    {
        _loadDialog = new FileDialog(GameScreen.Main, Labels.For(LabelIndex.SelectGameToLoad),
            Settings.SaveGameFolder, IsValidSelectionCallback, OnSelectionCallback);
        GameScreen.ShowDialog(_loadDialog, true);
    }

    private bool OnSelectionCallback(string? arg)
    {
        if (arg == null)
        {
            GameScreen.CloseDialog(_loadDialog);
            return false;
        }
        
        try
        {
            SessionLog.Record($"loading {Path.GetFileName(arg)}");
            RhyCiv.Engine.SaveLoad.LoadGame.LoadFrom(arg, GameScreen.Main);
        }
        catch (Exception e)
        {
            // A save can be unreadable for reasons that are not the player's fault
            // and are not worth ending the session over -- a file truncated by an
            // interrupted write, or one from an older format. Nothing caught this,
            // so picking a bad save in the load dialog took the whole game down.
            Console.Error.WriteLine($"Could not load '{arg}': {e}");
            GameScreen.CloseDialog(_loadDialog);
            GameScreen.ShowPopup("FAILEDTOLOADGAME",
                replaceStrings: [Path.GetFileName(arg), e.Message]);
            return true;
        }

        GameScreen.CloseDialog(_loadDialog);
        return true;
    }

    private bool IsValidSelectionCallback(string path)
    {
        return path.EndsWith(".sav", StringComparison.InvariantCultureIgnoreCase) && File.Exists(path);
    }
}
