using RhyCiv.UI.Classic.Rules;
using RhyCiv.Engine;
using Model.Controls;

namespace RhyCiv.UI.Classic.Dialogs.NewGame.CustomWorldDialogs;

public class CustomiseLandform : SimpleSettingsDialog
{
    public const string Title = "CUSTOMFORM";

    public CustomiseLandform() : base(Title)
    {
    }

    protected override string SetConfigValue(DialogResult result, DialogElements? dialog)
    {
        Initialization.ConfigObject.Landform = result.SelectedButton == dialog.Button[0]
            ? Initialization.ConfigObject.Random.Next(dialog.Options.Texts.Count)
            : result.SelectedIndex;
        return CustomClimate.Title;
    }
}