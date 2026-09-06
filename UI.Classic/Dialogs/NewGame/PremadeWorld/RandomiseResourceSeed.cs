using RhyCiv.UI.Classic.Rules;
using RhyCiv.Engine;
using Model.Controls;

namespace RhyCiv.UI.Classic.Dialogs.NewGame.PremadeWorld;

public class RandomiseResourceSeed : SimpleSettingsDialog
{
    public const string Title = "USESEED";
    public RandomiseResourceSeed() : base(Title)
    {
    }

    protected override string SetConfigValue(DialogResult result, DialogElements? dialog)
    {
        if (result.SelectedIndex == 0)
        {
            Initialization.ConfigObject.ResourceSeed = 0;
        }

        return Initialization.ConfigObject.StartPositions is { Length: > 0 }
            ? StartLoc.StartLocKey
            : DifficultyHandler.Title;
    }
}