using RhyCiv.UI.Classic.Dialogs.Scenario;
using RhyCiv.UI.Classic.Rules;
using RhyCiv.Engine;
using RhyCiv.Engine.Enums;
using Model;
using Model.Controls;
using Model.Core;
using Model.InterfaceActions;

namespace RhyCiv.UI.Classic.Dialogs;

public class ScenarioLoadedDialog : ICivDialogHandler
{
    public const string Title = "SCENARIOLOADED";

    public string Name { get; } = Title;
    public ICivDialogHandler UpdatePopupData(Dictionary<string, PopupBox> popups)
    {
        Dialog = new DialogElements(popups[Name])
        {
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
        var config = Initialization.ConfigObject;
        var date = new Date(config.StartingYear, config.TurnYearIncrement, config.DifficultyLevel);

        Dialog.ReplaceNumbers = new List<int> { config.TechParadigm };
        Dialog.ReplaceStrings = new List<string>
        {
            config.ScenarioName, date.GameYearString(1),
            date.GameYearString(config.MaxTurns),
        };
        
        return new MenuAction(Dialog);
    }
}