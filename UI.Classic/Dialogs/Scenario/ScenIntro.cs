using RhyCiv.UI.Classic.Dialogs.Scenario;
using RhyCiv.Engine;
using Model;
using Model.Controls;
using Model.Core;
using Model.InterfaceActions;

namespace RhyCiv.UI.Classic.Dialogs;

public class ScenIntro : ICivDialogHandler
{
    public const string Title = "SCENINTRO";

    public string Name { get; } = Title;
    public ICivDialogHandler UpdatePopupData(Dictionary<string, PopupBox> popups)
    {
        Dialog = new DialogElements(popups[Name])
        {
            Name = Title,
            Title = "",
            DialogPos = new Point(0, 0)
        };
        return this;
    }

    public DialogElements Dialog { get; private set; }

    public IInterfaceAction HandleDialogResult(DialogResult result,
        Dictionary<string, ICivDialogHandler> civDialogHandlers, ClassicInterface civ2Interface)
    {
        return civDialogHandlers[ScenChoseCiv.Title].Show(civ2Interface);
    }

    public IInterfaceAction Show(ClassicInterface activeInterface)
    {
        return new MenuAction(Dialog);
    }
}