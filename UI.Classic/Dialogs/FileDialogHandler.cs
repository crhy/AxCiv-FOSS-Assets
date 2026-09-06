using RhyCiv.Engine;
using Model.Controls;
using Model.Core;
using Model.InterfaceActions;

namespace RhyCiv.UI.Classic.Dialogs;

public abstract class FileDialogHandler : ICivDialogHandler
{
    private readonly string _extension;
    public string Name { get; }

    protected FileDialogHandler(string name, string extension)
    {
        _extension = extension;
        Name = name;
    }

    public abstract ICivDialogHandler UpdatePopupData(Dictionary<string, PopupBox> popup);

    public DialogElements Dialog { get; }
    public IInterfaceAction HandleDialogResult(DialogResult result,
        Dictionary<string, ICivDialogHandler> civDialogHandlers, ClassicInterface civ2Interface)
    {
        if (result.SelectedIndex == 0)
        {
            var fileName = result.TextValues?["FileName"];
            if (!string.IsNullOrWhiteSpace(fileName))
            {
                return HandleFileSelection(fileName, civDialogHandlers, civ2Interface);
            }
        }

        return civDialogHandlers[MainMenu.Title].Show(civ2Interface);
    }

    protected abstract IInterfaceAction HandleFileSelection(string fileName,
        Dictionary<string, ICivDialogHandler> civDialogHandlers, ClassicInterface active);

    public IInterfaceAction Show(ClassicInterface activeInterface)
    {
        return new FileAction(new OpenFileInfo
        {
            Title = Title,
            InitialDirectory = InitialDirectory,
            Filters = new List<FileFilter> { new(_extension) }
        }, Name);
    }

    protected virtual string InitialDirectory => Settings.GameDataPath;

    public string Title { get; protected set;  }
}
