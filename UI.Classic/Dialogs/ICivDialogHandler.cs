using RhyCiv.Engine;
using Model.Controls;
using Model.Core;
using Model.InterfaceActions;

namespace RhyCiv.UI.Classic.Dialogs;

public interface ICivDialogHandler
{
    string Name { get; }
    ICivDialogHandler UpdatePopupData(Dictionary<string, PopupBox> popup);
    DialogElements Dialog { get; }
    IInterfaceAction HandleDialogResult(DialogResult result, Dictionary<string, ICivDialogHandler> civDialogHandlers,
        ClassicInterface activeInterface);
    IInterfaceAction Show(ClassicInterface activeInterface);
}