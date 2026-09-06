using RhyCiv.UI.Classic.Dialogs.NewGame;
using RhyCiv.UI.Classic.Dialogs.Scenario;
using RhyCiv.UI.Classic.Rules;
using RhyCiv.Engine;
using RhyCiv.Engine.IO;
using Model;
using Model.InterfaceActions;
using System.Text.RegularExpressions;
using RhyCiv.Engine.LegacySaves;
using Model.Core;

namespace RhyCiv.UI.Classic.Dialogs.FileDialogs;

public class LoadScenario : FileDialogHandler
{
    public const string DialogTitle = "File-LoadScenario";

    public LoadScenario() : base(DialogTitle, ".scn")
    {
    }

    public override ICivDialogHandler UpdatePopupData(Dictionary<string, PopupBox> popup)
    {
        this.Title = Labels.For(LabelIndex.SelectScenarioToLoad);
        return this;
    }

    protected override IInterfaceAction HandleFileSelection(string fileName, Dictionary<string, ICivDialogHandler> civDialogHandlers, ClassicInterface civ2Interface)
    {
        return RhyCiv.Engine.SaveLoad.LoadGame.LoadFrom(fileName, civ2Interface.MainApp);
    }
}