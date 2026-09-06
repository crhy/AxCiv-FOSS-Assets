using RhyCiv.Engine;
using RhyCiv.Engine.IO;
using Model.Controls;
using Model.InterfaceActions;

namespace RhyCiv.UI.Classic.Dialogs.NewGame;

public abstract class SimpleSettingsDialog : BaseDialogHandler
{
    protected SimpleSettingsDialog(string name, double x = 0, double y = 0) : base(name, x, y)
    {
    }

    public override IInterfaceAction HandleDialogResult(DialogResult result,
        Dictionary<string, ICivDialogHandler> civDialogHandlers, ClassicInterface civ2Interface)
    {
        if (result.SelectedButton == Labels.Cancel)
        {
            return civDialogHandlers[MainMenu.Title].Show(civ2Interface);
        }

        //var popupBox = civDialogHandlers[Dialog.Name];
        var next = SetConfigValue(result, Dialog);

        return civDialogHandlers[next].Show(civ2Interface);
    }

    protected abstract string SetConfigValue(DialogResult result, DialogElements? popupBox);

}