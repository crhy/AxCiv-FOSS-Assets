using Civ2engine.Enums;
using JetBrains.Annotations;
using Model;
using Model.Controls;
using Model.Input;

namespace RaylibUI.RunGame.Commands.Orders;

[UsedImplicitly]
public class UnloadOrder(GameScreen gameScreen) : Order(gameScreen, new Shortcut(Key.U), CommandIds.UnloadOrder)
{
    public override bool Update()
    {
        var activeUnit = GameScreen.Player.ActiveUnit;
        if (activeUnit == null)
        {
            return SetCommandState(CommandStatus.Invalid);
        }

        var groundUnits = activeUnit.CurrentLocation.UnitsHere.Where(unit =>
            !unit.Dead && unit.Owner == activeUnit.Owner && unit.Domain == UnitGas.Ground);
        var canToggleStack = groundUnits.Any(unit =>
            unit.Order == (int)OrderType.Sleep || unit.AwaitingOrders && unit.MovePoints > 0);

        return SetCommandState(activeUnit.CarriedUnits.Any(unit => unit.MovePoints > 0) || canToggleStack
            ? CommandStatus.Normal
            : CommandStatus.Disabled);

    }

    public override void Action()
    {
        var player = GameScreen.Player;
        var activeUnit = player.ActiveUnit;
        if (activeUnit == null)
        {
            return;
        }

        var groundUnits = activeUnit.CurrentLocation.UnitsHere
            .Where(unit => !unit.Dead && unit.Owner == activeUnit.Owner && unit.Domain == UnitGas.Ground)
            .ToList();
        var guardedUnits = groundUnits.Where(unit => unit.Order == (int)OrderType.Sleep).ToList();
        if (guardedUnits.Count > 0)
        {
            foreach (var unit in guardedUnits)
            {
                unit.InShip?.CarriedUnits.Remove(unit);
                unit.InShip = null;
                unit.Order = (int)OrderType.NoOrders;
            }

            player.ActiveUnit = guardedUnits.LastOrDefault(unit => unit.AwaitingOrders);
            if (player.ActiveUnit == null)
            {
                GameScreen.Game.ChooseNextUnit();
            }
            return;
        }

        var unitsToGuard = groundUnits.Where(unit => unit.AwaitingOrders && unit.MovePoints > 0).ToList();
        foreach (var unit in unitsToGuard)
        {
            unit.Order = (int)OrderType.Sleep;
        }

        if (activeUnit.Domain == UnitGas.Sea && activeUnit.ShipHold > 0)
        {
            foreach (var unit in unitsToGuard.Where(unit => unit.InShip == null)
                         .Take(Math.Max(0, activeUnit.ShipHold - activeUnit.CarriedUnits.Count)))
            {
                unit.InShip = activeUnit;
                activeUnit.CarriedUnits.Add(unit);
            }
        }

        GameScreen.Game.ChooseNextUnit();
    }
}
