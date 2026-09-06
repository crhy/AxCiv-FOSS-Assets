using RhyCiv.Engine;
using Model.Controls;

namespace RhyCiv.UI.Classic.Dialogs.NewGame.PremadeWorld;

public class Failed : SimpleSettingsDialog
{
    public const string Title = "FAILEDTOLOAD";
    
    public Failed() : base(Title){}
    protected override string SetConfigValue(DialogResult result, DialogElements? dialog)
    {
        return MainMenu.Title;
    }
}