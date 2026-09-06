using RhyCiv.UI.Classic.Rules;
using RhyCiv.Engine;
using RhyCiv.Engine.IO;
using Model.Controls;
using Model.Core;
using Model.InterfaceActions;

namespace RhyCiv.UI.Classic.Dialogs.NewGame;

public class SelectTribe : BaseDialogHandler
{
    public const string Title = "TRIBE";

    public SelectTribe() : base(Title, 0, -0.03)
    {
    }

    public override ICivDialogHandler UpdatePopupData(Dictionary<string, PopupBox> popups)
    {
        var res = base.UpdatePopupData(popups);

        if (!res.Dialog.Button.Contains(Labels.Cancel))
        {
            res.Dialog.Button.Add(Labels.Cancel);
        }

        res.Dialog.Options = new();
        res.Dialog.Options.Columns = 3;
        return res;
    }

    public override IInterfaceAction Show(ClassicInterface activeInterface)
    {
        Dialog.Options.Texts = Initialization.ConfigObject.Rules.Leaders.Select(l => l.Adjective).ToList();
        return base.Show(activeInterface);
    }

    public override IInterfaceAction HandleDialogResult(DialogResult result,
        Dictionary<string, ICivDialogHandler> civDialogHandlers, ClassicInterface civ2Interface)
    {
        if (result.SelectedButton == Labels.Cancel)
        {
            return civDialogHandlers[SelectGender.Title].Show(civ2Interface);
        }

        // Make player civilization
        var tribe = Initialization.ConfigObject.Rules.Leaders[result.SelectedIndex];
        Initialization.ConfigObject.PlayerCiv =
            Initialization.MakeCivilization(Initialization.ConfigObject, tribe, true, tribe.Color);

        return result.SelectedButton == Labels.Custom
            ? civDialogHandlers[CustomTribe.Title].Show(civ2Interface)
            : civDialogHandlers[EnterName.Title].Show(civ2Interface);
    }
}