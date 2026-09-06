using RhyCiv.UI.Classic.Rules;
using RhyCiv.Engine;
using Model.Controls;
using Model.InterfaceActions;

namespace RhyCiv.UI.Classic.Dialogs.NewGame;

public class NoOfCivs : SimpleSettingsDialog
{
    public const string Title = "ENEMIES";
    
    public NoOfCivs() : base(Title, -0.085, -0.03)
    {
    }

    public override IInterfaceAction Show(ClassicInterface activeInterface)
    {
        var possibleCivs = activeInterface.PlayerColours.Length - 1;
        if(Dialog.Options == null || Dialog.Options.Texts.Count +2 != possibleCivs)
        {
            var suffix = Dialog.Options?.Texts[0].Split(" ", 2,
                StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)[1] ?? "Civilizations";
            Dialog.Options.Texts = Enumerable.Range(0, possibleCivs - 2)
                .Select(v => $"{(possibleCivs - v)} {suffix}").ToArray();
        }
        return base.Show(activeInterface);
    }

    protected override string SetConfigValue(DialogResult result, DialogElements? dialog)
    {
        Initialization.ConfigObject.NumberOfCivs = dialog.Options.Texts.Count + 2 - result.SelectedIndex;
        return Barbarity.Title;
    }
}