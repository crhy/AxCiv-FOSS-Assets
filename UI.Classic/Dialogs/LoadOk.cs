using RhyCiv.UI.Classic.Rules;
using RhyCiv.Engine;
using Model;
using Model.Controls;
using Model.Core;
using Model.InterfaceActions;

namespace RhyCiv.UI.Classic.Dialogs;

public class LoadOk : ICivDialogHandler
{
    public const string Title = "LOADOK";

    public string Name { get; } = Title;
    public ICivDialogHandler UpdatePopupData(Dictionary<string, PopupBox> popups)
    {   
        Dialog = new DialogElements(popups[Name])
        {
            DialogPos = new Point(0,0)
        };
        return this;
    }

    public DialogElements Dialog { get; private set; }

    public IInterfaceAction HandleDialogResult(DialogResult result,
        Dictionary<string, ICivDialogHandler> civDialogHandlers, ClassicInterface civ2Interface)
    {
        return new StartGame(Initialization.GameInstance, Initialization.ViewData);
    }

    public IInterfaceAction Show(ClassicInterface activeInterface)
    {
        var game = Initialization.GameInstance;
        var playerCiv = game.GetPlayerCiv;
    
        Dialog.ReplaceStrings = new List<string>
        {
            playerCiv.LeaderTitle, playerCiv.LeaderName,
            playerCiv.TribeName, game.Date.GameYearString(game.TurnNumber),
            game.Rules.Difficulty[game.DifficultyLevel]
        };
        return new MenuAction(Dialog);
    }
}