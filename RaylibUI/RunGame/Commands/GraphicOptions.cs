using Model.Input;
using Civ2engine;
using Civ2engine.IO;
using JetBrains.Annotations;
using Model;
using Model.Controls;
using Model.Core;

namespace RaylibUI.RunGame.Commands;

[UsedImplicitly]
public class GraphicOptions(GameScreen gameScreen) : IGameCommand
{
    private readonly Options _options = gameScreen.Game.Options;

    public string Id => CommandIds.GraphicOptions;
    public Shortcut[] ActivationKeys { get; set; } = { new(Key.P, ctrl: true) };
    public CommandStatus Status => CommandStatus.Normal;

    public bool Update()
    {
        return true;
    }

    public void Action()
    {
        // ReSharper disable once StringLiteralTypo
        // The throne room, animated heralds, the high council and wonder movies are
        // outside this game's scope, so only the two live toggles are offered.
        gameScreen.ShowPopup("GRAPHICOPTIONS", DialogClick,
            checkboxStates: new List<bool>
            {
                _options.DiplomacyScreenGraphics, _options.CivilopediaForAdvances
            });
    }

    private void DialogClick(string button, int _, IList<bool>? checkboxes, IDictionary<string, string>? _2)
    {
        if (button != Labels.Ok) return;
        if (checkboxes is not { Count: >= 2 }) return;

        _options.DiplomacyScreenGraphics = checkboxes[0];
        _options.CivilopediaForAdvances = checkboxes[1];
    }

    public bool Checked => false;
    public MenuCommand? Command { get; set; }
    public string ErrorDialog => string.Empty;
    public DialogImageElements? ErrorImage => null;
    public string? Name => "Graphic Options";
}