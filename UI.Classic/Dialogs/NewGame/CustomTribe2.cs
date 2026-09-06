using RhyCiv.UI.Classic.Rules;
using RhyCiv.Engine;
using RhyCiv.Engine.IO;
using Model.Controls;
using Model.Interface;
using Model.InterfaceActions;

namespace RhyCiv.UI.Classic.Dialogs.NewGame;

public class CustomTribe2 : BaseDialogHandler
{
    public const string Title = "CUSTOMTRIBE2";

    public CustomTribe2() : base(Title, 0, -0.03)
    {
    }

    public override IInterfaceAction Show(ClassicInterface activeInterface)
    {
        var male = Initialization.ConfigObject.Gender == 0;
        Dialog.TextBoxes = Initialization.ConfigObject.Rules.Governments.Select((gov, i) => new TextBoxDefinition
        {
            Index = i,
            Name = gov.Name, InitialValue = male ? gov.TitleMale : gov.TitleFemale, CharLimit = 23, Width = 300
        }).ToList();
        return base.Show(activeInterface);
    }

    public override IInterfaceAction HandleDialogResult(DialogResult result,
        Dictionary<string, ICivDialogHandler> civDialogHandlers, ClassicInterface civ2Interface)
    {
        if (result.SelectedButton == Labels.Cancel)
        {
            return civDialogHandlers[CustomTribe.Title].Show(civ2Interface);
        }

        if (result.TextValues != null)
        {
            Initialization.ConfigObject.PlayerCiv.Titles =
                Dialog.TextBoxes.Select(t => result.TextValues[t.Name]).ToArray();
        }

        return civDialogHandlers[CustomTribe.Title].Show(civ2Interface);
    }
}