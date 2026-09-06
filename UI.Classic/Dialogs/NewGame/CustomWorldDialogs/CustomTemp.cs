using RhyCiv.UI.Classic.Rules;
using RhyCiv.Engine;
using Model.Controls;

namespace RhyCiv.UI.Classic.Dialogs.NewGame.CustomWorldDialogs;

public class CustomTemp: SimpleSettingsDialog
{
    public const string Title = "CUSTOMTEMP";

    public CustomTemp() : base(Title)
    {
    }

    protected override string SetConfigValue(DialogResult result, DialogElements? dialog)
    {
        Initialization.ConfigObject.Temperature = result.SelectedButton == dialog.Button[0]
            ? Initialization.ConfigObject.Random.Next(dialog.Options.Texts.Count)
            : result.SelectedIndex;
        return CustomAge.Title;
    }
}