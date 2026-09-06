using RhyCiv.UI.Classic.Rules;
using RhyCiv.Engine;
using Model.Controls;

namespace RhyCiv.UI.Classic.Dialogs.NewGame;

public class Barbarity : SimpleSettingsDialog
{
    public const string Title = "BARBARITY";
    
    public Barbarity() : base(Title, 0.085, -0.03)
    {
    }

    protected override string SetConfigValue(DialogResult result, DialogElements? dialog)
    {
        Initialization.ConfigObject.BarbarianActivity = result.SelectedIndex;
        return SelectRules.Title;
    }
}