using System;
using System.Collections.Generic;
using System.Linq;
using RhyCiv.Engine.Units;
using RhyCiv.Engine.Enums;
using RhyCiv.Engine.Events;
using RhyCiv.Engine.MapObjects;
using RhyCiv.Engine.Terrains;
using RhyCiv.Engine.UnitActions;
using Model.Constants;
using Model.Core.Mapping;
using Model.Core.Player;
using Model.Core.Units;

// `Path` here is the engine's unit movement path, not System.IO.Path, which
// ImplicitUsings now brings into scope.
using Path = RhyCiv.Engine.Units.Path;
namespace RhyCiv.Engine
{
    public partial class Game
    {
        /// <summary>
        /// This is now only used for lua script integration for other events raise them on the player version
        /// </summary>
        public event EventHandler<UnitEventArgs> OnUnitEvent;
        internal event EventHandler<CivEventArgs> OnCivEvent;

        private readonly int[] _doNothingOrders = { (int)OrderType.Fortified, (int)OrderType.Sleep };

        // Choose next unit for orders. If all units ended turn, update cities.
        public void ChooseNextUnit()
        {
            var units = _activeCiv.Units.Where(u => !u.Dead).ToList();

            var player = Players[_activeCiv.Id];
            
            //Look for units on this square or neighbours of this square
            
            var nextUnit = NextUnit(player, units);
            
            // End turn if no units awaiting orders
            if (nextUnit == null)
            {
                // Nothing left to move, so nothing should still be selected. The
                // ActiveUnit setter refuses a unit whose turn has ended and leaves
                // the previous one in place, so without this the unit that just
                // spent its last move point stayed active and went on blinking for
                // the rest of the turn.
                player.SetUnitActive(null, false);

                var anyUnitsMoved = units.Any(u => u.MovePointsLost > 0);
                if ((!anyUnitsMoved || Options.AlwaysWaitAtEndOfTurn))
                {
                    Players[_activeCiv.Id].WaitingAtEndOfTurn();
                }
                else
                {
                    if (ProcessEndOfTurn())
                    {
                        ChoseNextCiv();
                    }
                }
            }
            else
            {
                //TODO: determine the true values of these extra props
                OnUnitEvent?.Invoke(this, new ActivationEventArgs(unit: nextUnit, userInitiated: true, reactivation: false));
                player.SetUnitActive(nextUnit, true);
                // If the player immediately moved the unit it might be already dead or moved so choose again
                if (nextUnit.Dead || nextUnit.MovePointsLost == nextUnit.MaxMovePoints)
                {
                    ChooseNextUnit();
                }
            }
        }

        private Unit? NextUnit(IPlayer player, List<Unit> units)
        {
            if (player.WaitingList is { Count: > 0 })
            {
                return
                    ActiveTile.UnitsHere.FirstOrDefault(u => u.AwaitingOrders && !player.WaitingList.Contains(u)) ??
                    ActiveTile
                        .Neighbours()
                        .SelectMany(
                            t => t.UnitsHere.Where(u =>
                                u.Owner == _activeCiv && u.AwaitingOrders && !player.WaitingList.Contains(u)))
                        .FirstOrDefault() ??
                    units.FirstOrDefault(u => u.AwaitingOrders && !player.WaitingList.Contains(u)) ??
                    ResetWaiting(player);

            }

            return ActiveTile.UnitsHere.FirstOrDefault(u => u.AwaitingOrders) ??
                   ActiveTile
                       .Neighbours()
                       .SelectMany(
                           t => t.UnitsHere.Where(u => u.Owner == _activeCiv && u.AwaitingOrders))
                       .FirstOrDefault() ?? units.FirstOrDefault(u => u.AwaitingOrders);

        }

        private Unit ResetWaiting(IPlayer player)
        {
            var unit = player.WaitingList[0];
            player.WaitingList.Clear();
            return unit;
        }

        public bool ProcessEndOfTurn()
        {
            var player = Players[_activeCiv.Id];
            // Snapshot: following a GoTo can pop a goody hut that hands the player new
            // units, and combat can kill them, both of which modify Units mid-loop.
            foreach (var unit in _activeCiv.Units.ToList())
            {
                if (unit.Dead)
                {
                    continue;
                }

                if (unit is { MovePoints: > 0, CurrentLocation: not null } && !_doNothingOrders.Contains(unit.Order))
                {
                    switch ((OrderType)unit.Order)
                    {
                        case OrderType.Fortify:
                            unit.Order = (int)OrderType.Fortified;
                            unit.MovePointsLost = unit.MovePoints;
                            break;
                        case OrderType.GoTo:
                            if (!unit.CurrentLocation.Map.IsValidTileC2(unit.GoToX, unit.GoToY))
                            {
                                ClearGotoOrder(unit);
                                break;
                            }

                            var destination = unit.CurrentLocation.Map.TileC2(unit.GoToX, unit.GoToY);
                            if (destination == unit.CurrentLocation)
                            {
                                ClearGotoOrder(unit);
                                break;
                            }

                            var path = Path.CalculatePathBetween(this, unit.CurrentLocation, destination, unit.Domain, unit.MaxMovePoints, unit.Owner, unit.Alpine, unit.IgnoreZonesOfControl);
                            if (path == null)
                            {
                                ClearGotoOrder(unit);
                                player.SetUnitActive(unit, true);
                                return false;
                            }

                            var startedAt = unit.CurrentLocation;
                            path.Follow(this, unit);

                            if (unit.Dead)
                            {
                                break;
                            }

                            if (unit.CurrentLocation == destination)
                            {
                                ClearGotoOrder(unit);
                                break;
                            }

                            if (unit.CurrentLocation == startedAt)
                            {
                                // The path exists but the next square is blocked - an enemy
                                // in the way, or a zone of control. Keeping the GoTo order
                                // handed the unit straight back to the player, who could
                                // neither move it nor end the turn, because ending the turn
                                // came back here and offered the same stuck unit again.
                                // Drop the order so it is genuinely awaiting orders.
                                ClearGotoOrder(unit);
                                player.SetUnitActive(unit, true);
                                return false;
                            }

                            if (unit.MovePoints > 0)
                            {
                                player.SetUnitActive(unit, true);
                                return false;
                            }

                            break;
                        case OrderType.Automate:
                            ProcessAutomatedSettler(unit);
                            break;
                        default:
                        {
                            unit.ProcessOrder();

                            if (TerrainImprovements.TryGetValue(unit.Building, out var improvement))
                            {
                                var completedUnits = this.CheckConstruction(unit.CurrentLocation, improvement);
                                foreach (var completedUnit in completedUnits.Where(u => u.WaitOrder && u.AiRole == AiRoleType.Settle))
                                {
                                    completedUnit.Order = (int)OrderType.Automate;
                                    completedUnit.Building = 0;
                                    completedUnit.SkipTurn();
                                }

                                var activeUnit = completedUnits.FirstOrDefault(u => u.MovePoints > 0 && !u.WaitOrder);
                                if (activeUnit != null)
                                {
                                    player.SetUnitActive(activeUnit, true);
                                    return false;
                                }
                            }

                            break;
                        }
                    }
                }
            }

            return true;
        }

        private static void ClearGotoOrder(Unit unit)
        {
            unit.Order = (int)OrderType.NoOrders;
            unit.GoToX = unit.X;
            unit.GoToY = unit.Y;
            unit.GoToMapIndex = unit.MapIndex;
        }

        private void ProcessAutomatedSettler(Unit unit)
        {
            if (unit.AiRole != AiRoleType.Settle || unit.CurrentLocation == null)
            {
                unit.SkipTurn();
                return;
            }

            var currentTile = unit.CurrentLocation;
            if (TryAssignAutomatedSettlerJob(unit, currentTile))
            {
                return;
            }

            var nearbyCity = currentTile.CityRadius().FirstOrDefault(t => t.CityHere != null)?.CityHere;
            var workTile = FindAutomatedSettlerWorkTile(unit, currentTile, nearbyCity?.Location);
            if (workTile != null)
            {
                StepAutomatedSettler(unit, currentTile, workTile);
                return;
            }

            if (nearbyCity == null && currentTile.Type != TerrainType.Ocean)
            {
                var moreFertile = MovementFunctions.GetPossibleMoves(currentTile, unit)
                    .Where(t => t.Fertility > currentTile.Fertility && t.Type != TerrainType.Ocean)
                    .OrderByDescending(t => t.Fertility)
                    .FirstOrDefault();

                if (moreFertile != null)
                {
                    StepAutomatedSettler(unit, currentTile, moreFertile);
                }
                else
                {
                    CityActions.BuildCity(unit, this, CityActions.GetCityName(unit.Owner, this));
                }
                return;
            }

            unit.SkipTurn();
        }

        private bool TryAssignAutomatedSettlerJob(Unit unit, Tile currentTile)
        {
            var preferredImprovements = new[]
            {
                ImprovementTypes.Road,
                ImprovementTypes.Irrigation
            };

            foreach (var improvementId in preferredImprovements)
            {
                if (!CanAutomatedSettlerBuild(unit, currentTile, improvementId, out var improvement))
                {
                    continue;
                }

                unit.WaitOrder = true;
                unit.Build(improvement);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Move an automated settler one square, and hand it back to the player if it
        /// could not go.
        ///
        /// A move can be refused - an enemy zone of control, or a settler with no
        /// attack meeting the unit standing in its way. The order used to be restored
        /// regardless and no movement spent, so the settler chose the same blocked
        /// square every turn for the rest of the game. Automated units are not
        /// awaiting orders, so it was never offered for selection either: it simply
        /// sat there, unmoving and unreachable, which is what being chased by
        /// barbarians into a corner looked like.
        /// </summary>
        private void StepAutomatedSettler(Unit unit, Tile from, Tile to)
        {
            MovementFunctions.MoveC2(this, unit, to.X - from.X, to.Y - from.Y);
            if (unit.Dead)
            {
                return;
            }

            if (unit.CurrentLocation != from)
            {
                unit.Order = (int)OrderType.Automate;
                return;
            }

            // It could not move. Give it back rather than automating it into the same
            // wall next turn.
            unit.Order = (int)OrderType.NoOrders;
            unit.WaitOrder = false;
        }

        private Tile? FindAutomatedSettlerWorkTile(Unit unit, Tile currentTile, Tile? nearbyCityTile)
        {
            var cityRadius = nearbyCityTile?.CityRadius().ToHashSet();
            return MovementFunctions.GetPossibleMoves(currentTile, unit)
                .Where(t => t.Type != TerrainType.Ocean && t.CityHere == null)
                .Where(t => cityRadius == null || cityRadius.Contains(t))
                .Where(t => CanAutomatedSettlerImprove(unit, t))
                .OrderByDescending(t => AutomatedSettlerWorkScore(unit, t))
                .FirstOrDefault();
        }

        private bool CanAutomatedSettlerImprove(Unit unit, Tile tile)
        {
            return CanAutomatedSettlerBuild(unit, tile, ImprovementTypes.Road, out _) ||
                   CanAutomatedSettlerBuild(unit, tile, ImprovementTypes.Irrigation, out _);
        }

        private int AutomatedSettlerWorkScore(Unit unit, Tile tile)
        {
            var score = (int)tile.Fertility;
            if (tile.WorkedBy != null)
            {
                score += 200;
            }

            if (CanAutomatedSettlerBuild(unit, tile, ImprovementTypes.Road, out _))
            {
                score += 100;
            }

            if (CanAutomatedSettlerBuild(unit, tile, ImprovementTypes.Irrigation, out _))
            {
                score += 80;
            }

            return score;
        }

        private bool CanAutomatedSettlerBuild(Unit unit, Tile tile, int improvementId, out TerrainImprovement improvement)
        {
            if (!TerrainImprovements.TryGetValue(improvementId, out var foundImprovement))
            {
                improvement = null!;
                return false;
            }

            improvement = foundImprovement;

            var selectedImprovement = improvement;

            if (selectedImprovement.ExclusiveGroup > 0 &&
                tile.Improvements.Any(i => i.Group == selectedImprovement.ExclusiveGroup && i.Improvement != selectedImprovement.Id))
            {
                return false;
            }

            return TerrainImprovementFunctions.CanImprovementBeBuiltHere(tile, selectedImprovement, unit.Owner).Enabled;
        }
    }
}
