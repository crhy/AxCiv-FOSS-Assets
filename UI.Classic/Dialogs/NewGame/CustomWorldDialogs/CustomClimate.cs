using RhyCiv.UI.Classic.Rules;
using RhyCiv.Engine;
using Model.Controls;

namespace RhyCiv.UI.Classic.Dialogs.NewGame.CustomWorldDialogs;

public class CustomClimate: SimpleSettingsDialog
{
    public const string Title = "CUSTOMCLIMATE";

    public CustomClimate() : base(Title)
    {
    }

    protected override string SetConfigValue(DialogResult result, DialogElements? dialog)
    {
        Initialization.ConfigObject.Climate = result.SelectedButton == dialog.Button[0]
            ? Initialization.ConfigObject.Random.Next(dialog.Options.Texts.Count)
            : result.SelectedIndex;
        return CustomTemp.Title;
    }
}