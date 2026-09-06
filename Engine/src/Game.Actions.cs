using System;
using System.Collections.Generic;
using System.Linq;
using RhyCiv.Engine.Advances;
using RhyCiv.Engine.Enums;
using RhyCiv.Engine.Events;
using RhyCiv.Engine.MapObjects;
using RhyCiv.Engine.Scripting.ScriptObjects;
using RhyCiv.Engine.Statistics;
using RhyCiv.Engine.UnitActions;
using RhyCiv.Engine.Units;
using Model.Constants;
using Model.Core;
using Model.Core.Events;
using Model.Core.Mapping;
using Model.Core.Player;
using Model.Core.Units;
using Neo.IronLua;

namespace RhyCiv.Engine
{
    public partial class Game
    {
        public event EventHandler<PlayerEventArgs> OnPlayerEvent;

        private bool _choosingNextCiv;
        private bool _chooseNextCivAgain;

        public void StartNextTurn()
        {
            StartNextTurnCore();
            ChoseNextCiv();
        }

        private void StartNextTurnCore()
        {
            TurnNumber++;

            if (TurnNumber % 2 == 0)
            {
                Power.CalculatePowerRatings(this);
            }

            _activeCivId = -1;
        }

        public void ChoseNextCiv()
        {
            if (_choosingNextCiv)
            {
                _chooseNextCivAgain = true;
                return;
            }

            _choosingNextCiv = true;
            try
            {
                do
                {
                    _chooseNextCivAgain = false;
                    ChooseNextCivilizationOnce();
                } while (_chooseNextCivAgain);
            }
            finally
            {
                _choosingNextCiv = false;
            }
        }

        private void ChooseNextCivilizationOnce()
        {
            var safety = Math.Max(8, AllCivilizations.Count * 4);
            while (safety-- > 0)
            {
                if (_activeCivId >= AllCivilizations.Count - 1)
                {
                    StartNextTurnCore();
                }

                _activeCivId++;
                if (_activeCivId < 0 || _activeCivId >= AllCivilizations.Count)
                {
                    StartNextTurnCore();
                    continue;
                }

                _activeCiv = AllCivilizations[_activeCivId];

                CheckElimination(_activeCiv);

                if (!_activeCiv.Alive)
                {
                    if (!Options.DontRestartIfEliminated)
                    {
                        //Look to restart if possible
                    }
                    continue;
                }

                var activePlayer = Players[_activeCiv.Id];
                TurnBeginning(_activeCiv, activePlayer);

                if (_activeCiv.PlayerType == PlayerType.Barbarians)
                {
                    ProcessBarbarianTurn(activePlayer);
                    continue;
                }

                StartPlayerTurn(activePlayer);
                return;
            }
        }

        /// <summary>
        /// A civilization that holds no cities and no units is out of the game.
        /// Nothing marked this before, so losing everything simply handed the
        /// turn on and play continued without you. Turn 1 is exempt so a civ
        /// still placing its first settlers is never judged.
        /// </summary>
        private void CheckElimination(Civilization civ)
        {
            if (!civ.Alive || civ.PlayerType == PlayerType.Barbarians || TurnNumber <= 1)
            {
                return;
            }

            // Only a living unit counts. A unit killed in combat is marked dead and
            // taken off the map, but it is left in its owner's unit list - only
            // disbanding removes it - so counting the list meant a civilisation that
            // had ever built anything could never be eliminated. Barbarians running
            // down the last settler ended the game in every sense except the one the
            // player sees.
            if (civ.Cities.Count > 0 || civ.Units.Any(unit => !unit.Dead))
            {
                return;
            }

            civ.Alive = false;
            if (civ.Id >= 0 && civ.Id < Players.Length)
            {
                Players[civ.Id].CivilizationDestroyed();
            }

            CheckConquest();
        }

        /// <summary>
        /// Conquest: one civilisation left standing and the world is theirs. The
        /// barbarians hold nothing and never count towards it.
        /// </summary>
        private void CheckConquest()
        {
            var survivors = AllCivilizations
                .Where(c => c.Alive && c.PlayerType != PlayerType.Barbarians)
                .ToList();
            if (survivors.Count != 1)
            {
                return;
            }

            var winner = survivors[0];
            if (winner.Id >= 0 && winner.Id < Players.Length)
            {
                Players[winner.Id].CivilizationVictorious();
            }
        }

        public void StartPlayerTurn(IPlayer activePlayer)
        {
            activePlayer.TurnStart(TurnNumber);

            //If there are any units waiting to move goto move them
            if (_activeCiv.Units.Any(u => u is { MovePointsLost: 0, Order: (int)OrderType.NoOrders }))
            {
                ChooseNextUnit();
            }
            else
            {
                activePlayer.WaitingAtEndOfTurn();
            }
        }

        public void SetHumanPlayer(int civId)
        {
            AllCivilizations.ForEach(c => c.PlayerType = PlayerType.Ai);
            AllCivilizations[0].PlayerType = PlayerType.Barbarians;
            AllCivilizations[civId].PlayerType = PlayerType.Local;
        }

/*
        public void AiTurn()
        {
            foreach (var unit in _activeCiv.Units.Where(u => !u.Dead).ToList())
            {
                var currentTile = unit.CurrentLocation;
                switch (unit.AiRole)
                {
                    case AiRoleType.Attack:
                        break;
                    case AiRoleType.Defend:
                        if (currentTile.CityHere != null)
                        {
                            if (currentTile.UnitsHere.Count(u => u != unit && u.AiRole == AiRoleType.Defend) <
                                2 + currentTile.CityHere.Size / 3)
                            {
                                if (unit.Order == (int)OrderType.Fortify || unit.Order == (int)OrderType.Fortified)
                                {
                                    unit.Order = (int)OrderType.Fortified;
                                }
                                else
                                {
                                    unit.Order = (int)OrderType.Fortify;
                                }
                                unit.MovePointsLost = unit.MovePoints;
                            }
                        }
                        else
                        {
                            
                        }
                        break;
                    case AiRoleType.NavalSuperiority:
                        break;
                    case AiRoleType.AirSuperiority:
                        break;
                    case AiRoleType.SeaTransport:
                        break;
                    case AiRoleType.Settle:
                        var cityTile = CurrentMap.CityRadius(currentTile)
                            .FirstOrDefault(t => t.CityHere != null);
                        
                        if (currentTile.Fertility == -2 && cityTile == null && currentTile.Type != TerrainType.Ocean)
                        {
                            CityActions.AiBuildCity(unit, this);
                        }
                        
                        if (cityTile == null && currentTile.Type != TerrainType.Ocean)
                        {
                            var moreFertile = MovementFunctions.GetPossibleMoves(currentTile, unit)
                                .Where(n => n.Fertility > currentTile.Fertility).OrderByDescending(n => n.Fertility)
                                .FirstOrDefault();
                            if (moreFertile == null)
                            {
                                CityActions.AiBuildCity(unit, this);
                            }
                            else
                            {
                                if (MovementFunctions.UnitMoved(this, unit, moreFertile, currentTile))
                                {
                                    currentTile = moreFertile;
                                    if (unit.MovePoints > 0)
                                    {
                                        CityActions.AiBuildCity(unit, this);
                                    }
                                }
                            }
                        }

                        break;
                    case AiRoleType.Diplomacy:
                        break;
                    case AiRoleType.Trade:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                while (unit.MovePoints > 0)
                {
                    var possibleMoves = MovementFunctions.GetPossibleMoves(currentTile, unit).ToList();
                    if (unit.AttackBase == 0)
                    {
                        possibleMoves = possibleMoves
                            .Where(m => m.UnitsHere.Count == 0 || m.UnitsHere[0].Owner == unit.Owner).ToList();
                    }
                    if (possibleMoves.Count == 0)
                    {
                        unit.SkipTurn();
                    }
                    else
                    {
                        var destination = Random.ChooseFrom(possibleMoves);
                        if (destination.UnitsHere.Count > 0 && destination.UnitsHere[0].Owner != unit.Owner)
                        {
                            unit.Order = (int)OrderType.NoOrders;
                            MovementFunctions.AttackAtTile(unit, this, destination);
                        }
                        else if (MovementFunctions.UnitMoved(this, unit, destination, currentTile))
                        {
                            currentTile = destination;
                        }
                    }
                }
            }
            ChoseNextCiv();
        }
*/

        private void TurnBeginning(Civilization activeCiv, IPlayer player)
        {
            // Adjust reputation
            
            // Reset turns of all units and let resting wounded units recover.
            // Wonder movement bonuses are re-derived here so they appear and expire
            // with the wonder rather than being baked into a unit when it is built.
            var seaMovementBonus = WonderFunctions.GetSeaMovementBonus(activeCiv) *
                                   Rules.Cosmic.MovementMultiplier;
            foreach (var unit in activeCiv.Units.Where(n => !n.Dead))
            {
                HealRestingUnit(unit);
                unit.BonusMovePoints = unit.Domain == UnitGas.Sea ? seaMovementBonus : 0;
                unit.MovePointsLost = 0;
            }

            ResolveAirFuel(activeCiv, player);
            ResolveShipsLostAtSea(activeCiv, player);
            ResolveGreatLibrary(activeCiv, player);

            // Update all cities
            this.CitiesTurn(player);
        }

        /// <summary>
        /// Civ II's Great Library hands its owner every advance that at least two
        /// other civilisations already know. It runs at the start of the owner's
        /// turn, before research is resolved, and keeps working until Electricity
        /// obsoletes the wonder.
        /// </summary>
        private void ResolveGreatLibrary(Civilization activeCiv, IPlayer player)
        {
            if (!WonderFunctions.OwnsActiveWonder(activeCiv, ImprovementType.GreatLibrary))
            {
                return;
            }

            var granted = WonderFunctions
                .GreatLibraryAdvances(activeCiv, AllCivilizations, Rules.Advances.Length)
                .ToList();

            foreach (var advance in granted)
            {
                this.GiveAdvance(advance, activeCiv);

                // GiveAdvance silently ignores an advance the civ is barred from,
                // so only announce the ones that actually landed.
                if (AdvanceFunctions.HasTech(activeCiv, advance))
                {
                    player.NotifyAdvanceResearched(advance);
                }
            }
        }

        /// <summary>
        /// Civ II air units carry a fuel range: a fighter must land every turn and a
        /// bomber may spend one turn out. A unit that has not reached a city, airbase
        /// or carrier by the time its range is used up crashes.
        /// </summary>
        private void ResolveAirFuel(Civilization activeCiv, IPlayer player)
        {
            var crashed = new List<Unit>();
            foreach (var unit in activeCiv.Units.Where(u => !u.Dead && u.Domain == UnitGas.Air && u.FuelRange > 0).ToList())
            {
                if (CanRefuel(unit))
                {
                    unit.TurnsAirborne = 0;
                    continue;
                }

                unit.TurnsAirborne++;
                if (unit.TurnsAirborne >= unit.FuelRange)
                {
                    unit.Dead = true;
                    crashed.Add(unit);
                }
            }

            foreach (var unit in crashed)
            {
                player.UnitLost(unit, null);
            }
        }

        private static bool CanRefuel(Unit unit)
        {
            if (unit.InShip is { } carrier && carrier.CanCarryAirUnits)
            {
                return true;
            }

            var tile = unit.CurrentLocation;
            if (tile == null)
            {
                return false;
            }

            if (tile.CityHere != null && tile.CityHere.Owner == unit.Owner)
            {
                return true;
            }

            return tile.EffectsList.Any(e => e.Target == ImprovementConstants.Airbase);
        }

        /// <summary>
        /// Triremes and other "must stay near land" ships risk being lost when they
        /// end a turn out of sight of land. Civ II uses a one-in-two chance, improved
        /// to one-in-four by Seafaring and one-in-eight by Navigation, and removed
        /// entirely by the Lighthouse.
        /// </summary>
        private void ResolveShipsLostAtSea(Civilization activeCiv, IPlayer player)
        {
            var atRisk = activeCiv.Units
                .Where(u => !u.Dead && u.ShipMustStayNearLand && u.CurrentLocation != null)
                .ToList();
            if (atRisk.Count == 0)
            {
                return;
            }

            if (WonderFunctions.OwnsActiveWonder(activeCiv, ImprovementType.Lighthouse))
            {
                return;
            }

            var lossChance = 2;
            if (HasAdvance(activeCiv, AdvanceType.Seafaring)) lossChance = 4;
            if (HasAdvance(activeCiv, AdvanceType.Navigation)) lossChance = 8;

            var lost = new List<Unit>();
            foreach (var unit in atRisk)
            {
                var tile = unit.CurrentLocation!;
                if (tile.Type != TerrainType.Ocean)
                {
                    continue;
                }

                if (tile.Map.Neighbours(tile).Any(t => t.Type != TerrainType.Ocean))
                {
                    continue;
                }

                if (Random.Next(lossChance) != 0)
                {
                    continue;
                }

                unit.Dead = true;
                lost.Add(unit);
            }

            foreach (var unit in lost)
            {
                player.UnitLost(unit, null);
            }
        }

        private static bool HasAdvance(Civilization civilization, AdvanceType advance) =>
            (int)advance < civilization.Advances.Length && civilization.Advances[(int)advance];

        private static void HealRestingUnit(Unit unit)
        {
            if (unit.HitPointsLost <= 0)
            {
                return;
            }

            var city = unit.CurrentLocation.CityHere;
            var inFriendlyCity = city?.Owner == unit.Owner;
            var resting = unit.Order is (int)OrderType.Sleep or (int)OrderType.Fortify or (int)OrderType.Fortified;
            if (!resting && !inFriendlyCity)
            {
                return;
            }

            // A city with the support building for the unit's domain -- Barracks for
            // land, Port Facility for sea, Airport for air -- restores it outright
            // rather than a couple of hit points a turn. They are recognised by the
            // same domain-matched Veteran effect that decides veteran production.
            if (inFriendlyCity && city!.Improvements.Any(i =>
                    i.Effects.TryGetValue(Effects.Veteran, out var domain) &&
                    domain == (int)unit.Domain))
            {
                unit.HitPointsLost = 0;
                return;
            }

            var healed = inFriendlyCity ? 2 : 1;
            unit.HitPointsLost = Math.Max(0, unit.HitPointsLost - healed);
        }

        private void ProcessBarbarianTurn(IPlayer activePlayer)
        {
            activePlayer.TurnStart(TurnNumber);

            foreach (var unit in _activeCiv.Units.Where(u => !u.Dead).ToList())
            {
                if (unit.AttackBase <= 0)
                {
                    unit.SkipTurn();
                    continue;
                }

                while (!unit.Dead && unit.MovePoints > 0)
                {
                    var currentTile = unit.CurrentLocation;
                    var adjacentEnemy = FindAdjacentBarbarianTarget(unit, currentTile);
                    if (adjacentEnemy != null)
                    {
                        MovementFunctions.AttackAtTile(unit, this, adjacentEnemy);
                        if (!unit.Dead && unit.MovePoints <= 0)
                        {
                            break;
                        }

                        if (unit.Dead)
                        {
                            break;
                        }
                    }
                    else
                    {
                        var target = FindNearestBarbarianTarget(unit, currentTile);
                        if (target == null)
                        {
                            unit.SkipTurn();
                            break;
                        }

                        var destination = MovementFunctions.GetPossibleMoves(currentTile, unit)
                            .Where(t => !t.Terrain.Impassable)
                            .OrderBy(t => BarbarianDistance(t, target))
                            .FirstOrDefault();

                        if (destination == null || destination == currentTile)
                        {
                            unit.SkipTurn();
                            break;
                        }

                        if (IsEnemyTileForBarbarian(unit, destination))
                        {
                            MovementFunctions.AttackAtTile(unit, this, destination);
                        }
                        else
                        {
                            MovementFunctions.MoveC2(this, unit, destination.X - currentTile.X, destination.Y - currentTile.Y);
                        }
                    }
                }
            }
        }

        private static Tile? FindAdjacentBarbarianTarget(Unit unit, Tile currentTile)
        {
            return currentTile.Neighbours()
                .Where(tile => IsEnemyTileForBarbarian(unit, tile))
                .OrderBy(tile => tile.CityHere == null ? 1 : 0)
                .ThenBy(tile => BarbarianDistance(currentTile, tile))
                .FirstOrDefault();
        }

        private Tile? FindNearestBarbarianTarget(Unit unit, Tile currentTile)
        {
            Tile? best = null;
            var bestDistance = int.MaxValue;
            foreach (var tile in currentTile.Map.Tile)
            {
                if (!IsEnemyTileForBarbarian(unit, tile))
                {
                    continue;
                }

                var distance = BarbarianDistance(currentTile, tile);
                if (distance < bestDistance)
                {
                    best = tile;
                    bestDistance = distance;
                }
            }

            return best;
        }

        private static bool IsEnemyTileForBarbarian(Unit unit, Tile tile)
        {
            if (tile.CityHere is { } city && city.Owner != unit.Owner)
            {
                return true;
            }

            return tile.UnitsHere.Any(other => !other.Dead && other.Owner != unit.Owner && other.InShip == null);
        }

        private static int BarbarianDistance(Tile from, Tile to)
        {
            return Math.Abs(from.X - to.X) + Math.Abs(from.Y - to.Y);
        }
    }
}
