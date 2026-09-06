using RhyCiv.UI.Classic.Rules;
using RhyCiv.Engine;
using RhyCiv.Engine.IO;
using RhyCiv.Engine.LegacySaves;
using Model;
using Model.Core;
using Model.InterfaceActions;

namespace RhyCiv.UI.Classic.Dialogs.FileDialogs;

public class LoadGame : FileDialogHandler
{
    public const string DialogTitle = "File-LoadGame";

    public LoadGame() : base(DialogTitle, ".sav")
    {
    }

    protected override string InitialDirectory => Settings.SaveGameFolder;

    public override ICivDialogHandler UpdatePopupData(Dictionary<string, PopupBox> popup)
    {
        this.Title = Labels.For(LabelIndex.SelectGameToLoad);
        return this;
    }

    protected override IInterfaceAction HandleFileSelection(string fileName,
        Dictionary<string, ICivDialogHandler> civDialogHandlers, ClassicInterface civ2Interface)
    {
        return RhyCiv.Engine.SaveLoad.LoadGame.LoadFrom(fileName, civ2Interface.MainApp);
    }
}
