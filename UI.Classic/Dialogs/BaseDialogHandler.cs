using RhyCiv.Engine;
using Model;
using Model.Controls;
using Model.Core;
using Model.InterfaceActions;

namespace RhyCiv.UI.Classic.Dialogs;

public abstract class BaseDialogHandler : ICivDialogHandler
{
    protected BaseDialogHandler(string name, double x = 0, double y = 0)
    {
        Name = name;
        DialogPos = new Point(x, y);
    }

    private Point DialogPos { get; }

    public string Name { get; }
    public virtual ICivDialogHandler UpdatePopupData(Dictionary<string, PopupBox> popups)
    {
        if (popups.TryGetValue(Name, out var popup))
        {
            Dialog = new DialogElements(popups[Name])
            {
                DialogPos = DialogPos
            };
        }

        return this;
    }
    
    public DialogElements Dialog { get; private set; }

    public abstract IInterfaceAction HandleDialogResult(DialogResult result,
        Dictionary<string, ICivDialogHandler> civDialogHandlers, ClassicInterface civ2Interface);

    public virtual IInterfaceAction Show(ClassicInterface activeInterface)
    {
        return new MenuAction(Dialog);
    }
}