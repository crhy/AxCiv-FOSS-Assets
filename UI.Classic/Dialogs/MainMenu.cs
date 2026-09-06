using RhyCiv.UI.Classic.Dialogs.FileDialogs;
using RhyCiv.UI.Classic.Dialogs.NewGame;
using RhyCiv.UI.Classic.Rules;
using RhyCiv.Engine;
using Model.Controls;
using Model.InterfaceActions;

namespace RhyCiv.UI.Classic.Dialogs;

public class MainMenu : BaseDialogHandler
{
    public const string Title = "MAINMENU";

    // Centre it like the other new-game wizard dialogs. The old
    // (-0.08, -0.07) anchor pinned it into the bottom-right corner, where a
    // wider inner panel then overflowed the window and the buttons became
    // unclickable.
    public MainMenu() : base(Title) { }

    public override IInterfaceAction HandleDialogResult(DialogResult result,
        Dictionary<string, ICivDialogHandler> civDialogHandlers, ClassicInterface civ2Interface)

    {
        if (result.SelectedButton == Dialog.Button[1])
        {
            return civ2Interface.InitialMenu != Title ? civ2Interface.GetInitialAction() : ExitAction.Exit;
        }

        switch (result.SelectedIndex)
        {
            case 0:
            case 2:
                Initialization.ClearInitializationConfig();
                Initialization.ConfigObject.CustomizeWorld = result.SelectedIndex == 2;
                if (civ2Interface.MainApp.AllRuleSets.Length > 1)
                    return civDialogHandlers[SelectGameVersionHandler.Title].Show(civ2Interface);
                var activeInterface = civ2Interface.MainApp.SetActiveRuleSet(0);
                return activeInterface.InitNewGame(false);
                //If there is only one ruleset then it will match the interface so nothing needed here
            case 1:
                 return civDialogHandlers[LoadMap.DialogTitle].Show(civ2Interface);
            case 3:
                Initialization.ClearInitializationConfig();
                Initialization.ConfigObject.IsScenario = true;
                return civDialogHandlers[LoadScenario.DialogTitle].Show(civ2Interface);
            case 4:
                return civDialogHandlers[LoadGame.DialogTitle].Show(civ2Interface);
            case 5:
                Initialization.ClearInitializationConfig();
                return civ2Interface.MainApp.SetActiveRuleSet(0).StartInstantGame();
        }
        return new MenuAction(Dialog);
    }
}