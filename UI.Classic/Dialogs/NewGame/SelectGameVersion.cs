using RhyCiv.UI.Classic.Rules;
using RhyCiv.Engine;
using RhyCiv.Engine.IO;
using Model.Controls;
using Model.Core;
using Model.InterfaceActions;

namespace RhyCiv.UI.Classic.Dialogs.NewGame;

// ReSharper disable once ClassNeverInstantiated.Global
public class SelectGameVersionHandler : BaseDialogHandler
{
    public const string Title = "AXX-Select-Game";

    public SelectGameVersionHandler() : base(Title) {}
    public override ICivDialogHandler UpdatePopupData(Dictionary<string, PopupBox> popups)
    {
        popups[Name] = new PopupBox
        {
            Name = Title,
            Title = "Select game version", 
            Button = new List<string> {"Quick Start", "OK", Labels.Cancel}
        };
        return base.UpdatePopupData(popups);
    }

    public override IInterfaceAction Show(ClassicInterface activeInterface)
    {
        Dialog.Options = new();
        Dialog.Options.Texts = activeInterface.MainApp.AllRuleSets.Select(r => r.Name).ToList();
        return base.Show(activeInterface);
    }

    public override IInterfaceAction HandleDialogResult(DialogResult result,
        Dictionary<string, ICivDialogHandler> civDialogHandlers, ClassicInterface civ2Interface)
    {                    
        if (result.SelectedButton == Labels.Cancel)
        {
            return civDialogHandlers[MainMenu.Title].Show(civ2Interface);
        }

        var activeInterface = civ2Interface.MainApp.SetActiveRuleSet(result.SelectedIndex);

        return activeInterface.InitNewGame(result.SelectedButton == "Quick Start");
    }
}