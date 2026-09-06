using RhyCiv.UI.Classic;
using RhyCiv.UI.Classic.Dialogs;
using RhyCiv.UI.Classic.Dialogs.NewGame;
using RhyCiv.UI.Classic.Rules;
using Model.Controls;
using Model.InterfaceActions;

namespace RhyCiv.UI.CompatAlternate.Dialogs;

public class StartMenuHandler() : BaseDialogHandler(Title)
{
    private const string Title = "STARTMENU";

    public override IInterfaceAction HandleDialogResult(DialogResult result, Dictionary<string, ICivDialogHandler> civDialogHandlers, ClassicInterface civ2Interface)
    {   
        if (result.SelectedButton == Dialog.Button[1])
        {
            return ExitAction.Exit;
        }

        switch (result.SelectedIndex)
        {
            case 0:
                return civDialogHandlers[MainMenu.Title].Show(civ2Interface);
            default:
                //TODO: Additional cases
                break;
        }

        return new MenuAction(Dialog);
    }
}