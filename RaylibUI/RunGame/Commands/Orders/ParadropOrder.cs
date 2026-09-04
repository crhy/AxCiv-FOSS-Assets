using Civ2engine.UnitActions;
using JetBrains.Annotations;
using Model;
using Model.Controls;
using Model.Input;
using RaylibUI.RunGame.GameModes;

namespace RaylibUI.RunGame.Commands.Orders;

/// <summary>
/// Arms a paradrop. The order itself only checks that the active unit could jump;
/// the drop zone is the next square the player clicks, which
/// <see cref="MovingPieces"/> hands to the engine.
/// </summary>
[UsedImplicitly]
public class ParadropOrder(GameScreen gameScreen)
    : Order(gameScreen, new Shortcut(Key.P), CommandIds.ParadropOrder)
{
    public override bool Update()
    {
        var unit = GameScreen.Player.ActiveUnit;

        // The menu entry is omitted entirely for units that cannot paradrop, so it
        // does not sit greyed out above every Settler in the game.
        if (unit is null || !unit.CanMakeParadrops)
        {
            return SetCommandState(CommandStatus.Invalid);
        }

        return SetCommandState(ParadropFunctions.CanParadrop(unit)
            ? CommandStatus.Normal
            : CommandStatus.Disabled);
    }

    public override void Action()
    {
        var unit = GameScreen.Player.ActiveUnit;
        if (unit is null || !ParadropFunctions.CanParadrop(unit))
        {
            return;
        }

        if (GameScreen.Moving is MovingPieces moving)
        {
            moving.AwaitingParadrop = unit;
        }
    }
}
