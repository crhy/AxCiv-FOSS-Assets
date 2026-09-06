using RhyCiv.UI.Classic.Rules;
using RhyCiv.Engine;
using Model.Controls;

namespace RhyCiv.UI.Classic.Dialogs.NewGame.PremadeWorld;

public class StartLoc : SimpleSettingsDialog
{
    public const string StartLocKey = "USESTARTLOC";

    public StartLoc() : base(StartLocKey)
    {
    }
    
    protected override string SetConfigValue(DialogResult result, DialogElements? dialog)
    {
        if (result.SelectedIndex == 0)
        {
            Initialization.ConfigObject.StartPositions = [];
        }

        return DifficultyHandler.Title;
    }
}