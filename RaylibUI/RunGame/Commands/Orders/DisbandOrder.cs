using System.Diagnostics;
using RhyCiv.Engine.UnitActions;
using JetBrains.Annotations;
using Model;
using Model.Controls;
using Model.Core;
using Model.Input;

namespace RaylibUI.RunGame.Commands.Orders;

/// <summary>
/// Removes the active unit from the game. A unit standing in, or homed to, a
/// city credits that city's current production with half its shield cost, the
/// way Civ II handles disbanding a unit in a city.
/// </summary>
[UsedImplicitly]
public class DisbandOrder(GameScreen gameScreen)
    : Order(gameScreen, new Shortcut(Key.D, shift: true), CommandIds.DisbandOrder)
{
    private readonly IGame _game = gameScreen.Game;
    private readonly LocalPlayer _player = gameScreen.Player;

    public override bool Update()
    {
        return SetCommandState(_player.ActiveUnit != null ? CommandStatus.Normal : CommandStatus.Invalid);
    }

    public override void Action()
    {
        var unit = _player.ActiveUnit;
        Debug.Assert(unit != null);

        _player.SetUnitActive(null, false);
        CityActions.DisbandUnit(unit, _game);
        _game.ChooseNextUnit();
    }
}
