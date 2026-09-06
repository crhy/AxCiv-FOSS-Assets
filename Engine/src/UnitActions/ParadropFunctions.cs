using System.Collections.Generic;
using System.Linq;
using RhyCiv.Engine.Enums;
using RhyCiv.Engine.MapObjects;
using Model.Core;
using Model.Core.Mapping;
using Model.Core.Units;

namespace RhyCiv.Engine.UnitActions;

/// <summary>
/// Paradrops. A Paratrooper that has not moved yet can jump from a friendly city
/// or airbase to any known, unoccupied land square within the ruleset's paradrop
/// range, landing with its turn spent. Range is measured by walking the map's own
/// adjacency rather than by coordinate arithmetic, so it stays correct across the
/// staggered isometric grid and the east-west wrap.
/// </summary>
public static class ParadropFunctions
{
    /// <summary>
    /// Whether the unit is in a position to jump at all, independent of any target.
    /// </summary>
    public static bool CanParadrop(Unit unit)
    {
        if (unit.Dead || !unit.CanMakeParadrops || unit.CurrentLocation == null)
        {
            return false;
        }

        // Civ II wants a full turn's movement in hand: a unit that has already
        // moved, or that is being carried, cannot jump.
        if (unit.MovePointsLost > 0 || unit.InShip != null)
        {
            return false;
        }

        return IsValidOrigin(unit);
    }

    private static bool IsValidOrigin(Unit unit)
    {
        var tile = unit.CurrentLocation;
        if (tile.CityHere != null && tile.CityHere.Owner == unit.Owner)
        {
            return true;
        }

        return tile.EffectsList.Any(e => e.Target == ImprovementConstants.Airbase);
    }

    /// <summary>
    /// Every square the unit could drop onto this turn.
    /// </summary>
    public static IReadOnlyCollection<Tile> ValidTargets(IGame game, Unit unit)
    {
        if (!CanParadrop(unit))
        {
            return [];
        }

        return TilesWithinRange(unit.CurrentLocation, game.Rules.Cosmic.MaxParadropRange)
            .Where(tile => IsValidTarget(unit, tile))
            .ToList();
    }

    public static bool IsValidTarget(Unit unit, Tile target)
    {
        if (target == unit.CurrentLocation || target.Type == TerrainType.Ocean)
        {
            return false;
        }

        // The drop zone has to be somewhere the player has actually seen.
        if (!target.IsVisible(unit.Owner.Id))
        {
            return false;
        }

        if (target.CityHere != null && target.CityHere.Owner != unit.Owner)
        {
            return false;
        }

        return !target.UnitsHere.Any(u => !u.Dead && u.Owner != unit.Owner);
    }

    /// <summary>
    /// Walk outwards from the origin, collecting everything reachable in at most
    /// <paramref name="range"/> steps.
    /// </summary>
    private static IEnumerable<Tile> TilesWithinRange(Tile origin, int range)
    {
        var seen = new HashSet<Tile> { origin };
        var frontier = new List<Tile> { origin };
        for (var step = 0; step < range && frontier.Count > 0; step++)
        {
            var next = new List<Tile>();
            foreach (var neighbour in frontier.SelectMany(tile => tile.Neighbours()))
            {
                if (seen.Add(neighbour))
                {
                    next.Add(neighbour);
                }
            }

            frontier = next;
        }

        seen.Remove(origin);
        return seen;
    }

    /// <summary>
    /// Execute a drop. Returns false and changes nothing when the jump is not legal.
    /// </summary>
    public static bool TryParadrop(IGame game, Unit unit, Tile target)
    {
        if (!CanParadrop(unit) || !IsValidTarget(unit, target))
        {
            return false;
        }

        if (!TilesWithinRange(unit.CurrentLocation, game.Rules.Cosmic.MaxParadropRange).Contains(target))
        {
            return false;
        }

        var tileFrom = unit.CurrentLocation;

        unit.PrevXy = [unit.X, unit.Y];
        unit.X = target.X;
        unit.Y = target.Y;
        // Assigning the location moves the unit between the tiles' unit lists.
        unit.CurrentLocation = target;

        // The jump costs the whole turn however far it went.
        unit.MovePointsLost = unit.MaxMovePoints;
        unit.Order = (int)OrderType.NoOrders;

        var revealed = new List<Tile> { tileFrom, target };
        foreach (var neighbour in target.Neighbours(unit.TwoSpaceVisibility))
        {
            if (!neighbour.IsVisible(unit.Owner.Id))
            {
                neighbour.SetVisible(unit.Owner.Id);
            }

            revealed.Add(neighbour);
        }

        for (var civId = 0; civId < target.Visibility.Length; civId++)
        {
            if (target.Visibility[civId])
            {
                game.Players[civId].UnitMoved(unit, target, tileFrom);
            }
        }

        game.UpdateTiles(revealed.Distinct().ToList());
        return true;
    }
}
