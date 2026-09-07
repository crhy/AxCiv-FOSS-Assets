using RhyCiv.Engine;
using RhyCiv.Engine.UnitActions;
using RhyCiv.Engine.SaveLoad;
using RhyCiv.Engine.Advances;
using RhyCiv.Engine.Diagnostics;
using RhyCiv.Engine.Enums;
using RhyCiv.Engine.Events;
using RhyCiv.Engine.IO;
using RhyCiv.Engine.MapObjects;
using Model.Controls;
using Model.Controls.Civilopedia;
using Model.Core;
using Model.Core.Advances;
using Model.Core.Cities;
using Model.Core.GoodyHuts.Outcomes;
using Model.Core.Mapping;
using Model.Core.Units;
using Model.Events;
using Model.Core.Player;
using Model.Core.Production;
using Model.Images;
using RaylibUI.RunGame.GameControls.Civilopedia;

namespace RaylibUI.RunGame;

public class LocalPlayer : IPlayer
{
    private readonly GameScreen _gameScreen;

    public LocalPlayer(GameScreen gameScreen, Civilization civilization)
    {
        _gameScreen = gameScreen;
        Civilization = civilization;
    }

    public Civilization Civilization { get; }

    public Tile ActiveTile { get; set; }

    private Unit? _activeUnit;

    public Unit? ActiveUnit
    {
        get { return _activeUnit; }
        set
        {
            if (value == null)
            {
                _activeUnit = null;
            }
            else if (value is { TurnEnded: false, Dead: false } && value.Owner == Civilization)
            {
                if (value.CurrentLocation != null) ActiveTile = value.CurrentLocation;
                _activeUnit = value;
            }
            else
            {
#if DEBUG
                //     throw new NotSupportedException("Tried to set ended unit to active");
#endif
            }
        }
    }

    public void CivilDisorder(City city)
    {
        _gameScreen.ShowCityDialog("DISORDER", city);
    }

    public void OrderRestored(City city)
    {
        _gameScreen.ShowCityDialog("RESTORED", city);
    }

    public void WeLoveTheKingStarted(City city)
    {
        _gameScreen.ShowCityDialog("WELOVEKING", city);
    }

    public void WeLoveTheKingCanceled(City city)
    {
        _gameScreen.ShowCityDialog("WEDONTLOVEKING", city);
    }

    public void CantMaintain(City city, Improvement cityImprovement)
    {
        _gameScreen.ShowCityDialog("INHOCK", city, [city.Name, cityImprovement.Name],
            [cityImprovement.Cost]);
    }

    public void SelectNewAdvance(List<Advance> researchPossibilities)
    {
        ShowResearchChoice(researchPossibilities, 0);
    }

    /// <summary>
    /// The research chooser, with an Info button that opens the Civilopedia page
    /// for whichever advance is highlighted. Reading about a choice puts the
    /// chooser back underneath, so the pedia closes onto the question again.
    /// </summary>
    private void ShowResearchChoice(List<Advance> researchPossibilities, int preselected)
    {
        var activeInterface = _gameScreen.Main.ActiveInterface;
        _gameScreen.ShowPopup("RESEARCH", (s, i, arg3, arg4) =>
            {
                if (researchPossibilities.Count == 0)
                {
                    return;
                }

                var selectedIndex = Math.Clamp(i, 0, researchPossibilities.Count - 1);

                if (s == "Info")
                {
                    // Pushed first so it sits under the pedia page pushed next.
                    ShowResearchChoice(researchPossibilities, selectedIndex);
                    NotifyAdvanceResearched(researchPossibilities[selectedIndex].Index);
                    return;
                }

                Civilization.ReseachingAdvance = researchPossibilities[selectedIndex].Index;
                if (Civilization.ScienceRate <= 0)
                {
                    Civilization.ScienceRate = 60;
                    Civilization.TaxRate = Math.Min(Civilization.TaxRate, 40);
                }
            }, replaceStrings: [activeInterface.GetScientistName(Civilization.Epoch)],
            listBox: new ListboxDefinition
            {
                VerticalScrollbar = false,
                ImageShift = false,
                Rows = Math.Min(10, researchPossibilities.Count),
                Looks = new ListboxLooks
                {
                    Font = activeInterface.Look.DefaultFont,
                    FontSize = 20,
                    TextColorFront = Raylib_CSharp.Colors.Color.Black,
                    TextColorShadow = Raylib_CSharp.Colors.Color.Blank,
                    TextShadowOffset = System.Numerics.Vector2.Zero,
                    SelectedTextFont = activeInterface.Look.DefaultFont,
                    SelectedTextBackgroundColor = new Raylib_CSharp.Colors.Color(107, 107, 107, 255),
                    SelectedTextColorFront = Raylib_CSharp.Colors.Color.White,
                    SelectedTextColorShadow = Raylib_CSharp.Colors.Color.Black
                },
                Groups = researchPossibilities.Select(a => new ListboxGroup
                {
                    Elements = [new() { Icon = GetClassicAdvanceIcon(a), Width = 2 * 36 + 2 },
                                new() { Text = a.Name, VerticalAlignment = VerticalAlignment.Center } ],
                    Height = 36
                }).ToList(),
                SelectedId = Math.Clamp(preselected, 0, Math.Max(0, researchPossibilities.Count - 1))
            });
    }

    /// <summary>
    /// The badge shown beside an advance in the research list, saying what kind of
    /// advance it is. This used to read a 36x20 patch of the ICONS sheet, which the
    /// standalone set fills with plain coloured rectangles, so every advance was a
    /// flat swatch (#48).
    /// </summary>
    private static readonly string[] AdvanceCategoryArt =
        ["academic", "applied", "military", "social"];

    private static IImageSource GetClassicAdvanceIcon(Advance advance)
    {
        var category = advance.KnowledgeCategory;
        if (category < 0 || category >= AdvanceCategoryArt.Length)
        {
            category = 1;
        }

        return new BitmapStorage(
            System.IO.Path.Combine("Icons", "AdvanceCategories", $"{AdvanceCategoryArt[category]}.png"));
    }

    public void CantProduce(City city, IProductionOrder? newItem)
    {
        _gameScreen.ShowCityDialog("BADBUILD", city);
    }

    public void CityProductionComplete(City city)
    {
        _gameScreen.ShowCityDialog("BUILT", city);
    }

    public IInterfaceCommands Ui { get; }
    public List<Unit> WaitingList { get; } = new();

    /// <summary>
    /// Announces a terrain improvement that an advance has just made available.
    /// <para>
    /// This went through <see cref="Ui"/>, which nothing ever assigns -- no type in
    /// the solution implements <see cref="IInterfaceCommands"/> -- so researching an
    /// advance that enables an improvement with a message threw a
    /// NullReferenceException and took the game down. It now uses the same popup
    /// path as the rest of this class.
    /// </para>
    /// </summary>
    public void NotifyImprovementEnabled(TerrainImprovement improvement, int level)
    {
        if (level < 0 || level >= improvement.Levels.Count)
        {
            return;
        }

        var dialogKey = improvement.Levels[level].EnabledMessage;
        if (!string.IsNullOrWhiteSpace(dialogKey))
        {
            _gameScreen.ShowPopup(dialogKey);
        }
    }

    public void MapChanged(List<Tile> tiles)
    {
        // var t = tiles.SelectMany(t => t.Map.DirectNeighbours(t));

        var allTiles = tiles
            .Concat(tiles.SelectMany(t => t.Map.DirectNeighbours(t).Where(n => n.IsVisible(_gameScreen.VisibleCivId))))
            .Distinct();
        foreach (var tile in allTiles)
        {
            _gameScreen.TileCache.Redraw(tile, _gameScreen.VisibleCivId);
        }

        _gameScreen.ForceRedraw();
    }

    /// <summary>
    /// Every unit has moved and the engine is waiting for the turn to be ended.
    /// <para>
    /// Civ II says so, in the side panel, and takes Enter for it. Nothing said so
    /// here: the interface simply dropped into viewing-pieces mode, which looks the
    /// same as choosing to look around mid-turn, so there was no way to tell that
    /// the game was waiting rather than that a unit had been missed.
    /// </para>
    /// </summary>
    public bool IsWaitingAtEndOfTurn { get; private set; }

    public void WaitingAtEndOfTurn()
    {
        IsWaitingAtEndOfTurn = true;
        _gameScreen.ActiveMode = _gameScreen.ViewPiece;
    }

    public void NotifyAdvanceResearched(int advance)
    {
        var rules = _gameScreen.Game.Rules;
        if (advance < 0 || advance >= rules.Advances.Length)
        {
            return;
        }

        var discoveredAdvance = rules.Advances[advance];
        var sortedAdvances = rules.Advances.Take(89).OrderBy(a => a.Name).ToList();
        var civilopediaIndex = sortedAdvances.IndexOf(discoveredAdvance);
        if (civilopediaIndex < 0)
        {
            civilopediaIndex = Math.Clamp(advance, 0, Math.Max(0, sortedAdvances.Count - 1));
        }

        _gameScreen.ShowDialog(new CivilopediaWindow(_gameScreen,
            new CivilopediaEntry(CivilopediaInfoType.Advances, CivilopediaWindowType.Info, civilopediaIndex)), stack: true);
    }

    public void FoodShortage(City city)
    {
        _gameScreen.ShowCityDialog("FOODSHORTAGE", city);
    }

    public void CityGrowthHalted(City city)
    {
        _gameScreen.ShowCityDialog("FURTHERGROWTH", city);
    }

    public void CivilizationDestroyed()
    {
        _gameScreen.ShowPopup("CIVDESTROYED");
    }

    public void CivilizationVictorious()
    {
        _gameScreen.ShowPopup("CONQUEST",
            dialogImage: new DialogImageElements(
                [_gameScreen.Main.ActiveInterface.PicSources["victoryConquest"][0]]));
    }

    public void CityDecrease(City city)
    {
        _gameScreen.ShowCityDialog("DECREASE", city);
    }

    /// <summary>
    /// How many autosaves are kept before the oldest is written over. Enough to
    /// step back past a turn that went wrong, few enough not to fill a disk.
    /// </summary>
    private const int AutosaveSlots = 3;

    public void TurnStart(int turnNumber)
    {
        IsWaitingAtEndOfTurn = false;
        _lastBlocked = (null, BlockedReason.NotBlocked);
        Autosave(turnNumber);
        _gameScreen.TurnStarting(turnNumber);
    }

    /// <summary>
    /// Writes the game at the start of the player's turn, if they have asked for it.
    /// <para>
    /// "Autosave each turn" has been a checkbox in Game Options for as long as the
    /// options dialog has existed, and nothing has ever read it. Given that a
    /// session can still end in a fault no handler can catch, an autosave that does
    /// not happen is the worst of the settings to have got wrong.
    /// </para>
    /// <para>
    /// The turn is saved as it begins, before the player has moved anything, so the
    /// newest autosave is always a position that can be picked up cleanly. Failing
    /// to write one must never end a session, so everything here is best-effort:
    /// the player gets a line in the record and carries on playing.
    /// </para>
    /// </summary>
    private void Autosave(int turnNumber)
    {
        var game = _gameScreen.Game;
        if (!game.Options.AutosaveEachTurn || _gameScreen.Main.ActiveRuleSet is not { } ruleset)
        {
            return;
        }

        try
        {
            var slot = Math.Abs(turnNumber) % AutosaveSlots + 1;
            var path = Path.Combine(Settings.SaveGameFolder, $"autosave{slot}.sav");
            var viewData = new Dictionary<string, string> { { "Zoom", _gameScreen.Zoom.ToString() } };

            // AtomicFile so a failure part way through cannot destroy the autosave
            // from three turns ago that it is writing over.
            AtomicFile.Write(path,
                stream => new GameSerializer().Write(stream, game, ruleset, viewData));
            SessionLog.Record($"autosaved turn {turnNumber} to {Path.GetFileName(path)}");
        }
        catch (Exception error)
        {
            SessionLog.Record($"autosave failed: {error.Message}");
            Console.Error.WriteLine($"rhYciv: could not autosave: {error.Message}");
        }
    }

    public void SetUnitActive(Unit? unit, bool move)
    {
        if (unit != null)
        {
            IsWaitingAtEndOfTurn = false;
        }

        ActiveUnit = unit;

        if (_gameScreen.Game.GetActiveCiv != Civilization)
        {
            return;
        }

        // ActiveUnit refuses a unit that has ended its turn, and used to leave the
        // previous one in place. Switching to Moving anyway left the map in
        // unit-moving mode pointing at a unit that was not there, so nothing
        // responded to the keyboard and no unit could be picked. Fall back to the
        // view piece instead, which the player can always move.
        if (unit != null && !ReferenceEquals(_activeUnit, unit))
        {
            _activeUnit = null;
            if (unit.CurrentLocation != null) ActiveTile = unit.CurrentLocation;
            _gameScreen.ActiveMode = _gameScreen.ViewPiece;
            return;
        }

        // No unit to move. The view piece is the only sensible mode here, and the
        // only safe one: MovingPieces.Activate asks the game for the next unit
        // whenever there is no active one, and this is reached from ChooseNextUnit
        // when nothing is awaiting orders. Entering Moving mode from here therefore
        // called back into ChooseNextUnit and recursed until the stack overflowed,
        // which .NET cannot catch -- the process died with no crash report at all.
        if (_activeUnit == null)
        {
            _gameScreen.ActiveMode = _gameScreen.ViewPiece;
            return;
        }

        _gameScreen.ActiveMode = _gameScreen.Moving;
    }

    public void UnitLost(Unit unit, Unit? killedBy)
    {
        UnitsLost([unit], killedBy);
    }

    /// <summary>
    /// Units belonging to this player have died — in combat, or with the city that
    /// supported them when it was captured.
    /// <para>
    /// Both of these were unimplemented. The engine took the units off the map and
    /// nothing told the interface, so the tiles they had stood on were never
    /// repainted and they appeared to survive whatever had killed them. Losing a
    /// city to the barbarians left its supported units apparently still standing.
    /// </para>
    /// </summary>
    public void UnitsLost(List<Unit> deadUnits, Unit? killedBy)
    {
        if (deadUnits.Count == 0)
        {
            return;
        }

        SessionLog.Record($"lost {deadUnits.Count} unit(s)" +
                          (killedBy == null ? "" : $" to {killedBy.Name}"));

        // Nothing should still be selected if it has just died: the map would go on
        // blinking a unit that is no longer there, and the side panel would describe
        // it as though it could still be given orders.
        if (ActiveUnit is { } active && deadUnits.Contains(active))
        {
            SetUnitActive(null, false);
        }

        // Unit.Dead takes a unit off its tile but asks for no redraw, so the tile
        // keeps the last frame it was drawn with.
        var tiles = deadUnits
            .Select(unit => unit.CurrentLocationOrNull)
            .OfType<Tile>()
            .Distinct()
            .ToList();

        if (tiles.Count > 0)
        {
            MapChanged(tiles);
        }
        else
        {
            _gameScreen.ForceRedraw();
        }
    }

    public void UnitMoved(Unit unit, Tile tileTo, Tile tileFrom)
    {
        OnUnitEvent?.Invoke(this, new MovementEventArgs(unit, tileFrom, tileTo));
    }

    public void CombatHappened(CombatEventArgs combatEventArgs)
    {
        OnUnitEvent?.Invoke(this, combatEventArgs);
    }

    private (Unit? Unit, BlockedReason Reason) _lastBlocked;

    public void MoveBlocked(Unit unit, BlockedReason blockedReason)
    {
        OnUnitEvent?.Invoke(this, new MovementBlockedEventArgs(unit, blockedReason));
        ReportBlockedMove(unit, blockedReason);
    }

    /// <summary>
    /// Say why a move was refused. The engine has always raised this, but nothing
    /// listened, so a unit that could not move simply did nothing and looked stuck.
    /// Repeats are suppressed - a unit bumping the same zone of control every turn
    /// should not stack up popups.
    /// </summary>
    private void ReportBlockedMove(Unit unit, BlockedReason blockedReason)
    {
        var message = blockedReason switch
        {
            BlockedReason.Zoc =>
                $"Your {unit.Name} cannot move directly between two squares that are both " +
                "next to an enemy unit. Move to a square you already occupy, or attack.",
            BlockedReason.ZeroAttackStrength =>
                $"Your {unit.Name} has no attack strength and cannot move onto an enemy unit.",
            BlockedReason.CannotAttackAirUnits =>
                $"Your {unit.Name} cannot attack air units.",
            BlockedReason.NotAmphibious =>
                $"Your {unit.Name} cannot make an amphibious assault from aboard ship.",
            BlockedReason.EdgeOfMap =>
                $"Your {unit.Name} has reached the edge of the world.",
            _ => null
        };

        if (message == null || (_lastBlocked.Unit == unit && _lastBlocked.Reason == blockedReason))
        {
            return;
        }

        _lastBlocked = (unit, blockedReason);

        var elements = new DialogElements
        {
            Name = "MOVEBLOCKED_DYNAMIC",
            Title = "Blocked",
            Width = 420,
            Button = [Labels.Ok],
            Text = [message],
            LineStyles = [TextStyles.Left]
        };

        CivDialog? dialog = null;
        dialog = new CivDialog(_gameScreen.Main, elements, (_, _, _, _) => _gameScreen.CloseDialog(dialog));
        _gameScreen.ShowDialog(dialog, stack: true);
    }

    public event EventHandler<UnitEventArgs> OnUnitEvent;

    public void GoodyHutTriggered(Unit unit, GoodyHutOutcomeResult outcome)
    {
        var args = new GoodyHutOutcomeEventArgs(unit, outcome);
        OnUnitEvent?.Invoke(this, args);

        _gameScreen.ForceRedraw();
        ShowGoodyHutResult(outcome);
    }

    private void ShowGoodyHutResult(GoodyHutOutcomeResult outcome)
    {
        var title = outcome.OutcomeType switch
        {
            "Gold" => "Village Gold",
            "Scrolls" => "Ancient Scrolls",
            "Nomads" => "Wandering Nomads",
            "AdvancedTribe" => "Advanced Tribe",
            "Barbarians" => "Barbarian Horde",
            "AbandonedVillage" => "Abandoned Village",
            "Mercenaries" => "Mercenaries",
            _ => "Village"
        };

        var message = outcome.Message;
        if (outcome.AdvanceIndex is { } advanceIndex &&
            advanceIndex >= 0 && advanceIndex < _gameScreen.Game.Rules.Advances.Length)
        {
            message += $" You have gained { _gameScreen.Game.Rules.Advances[advanceIndex].Name }.";
        }

        var dialogElements = new DialogElements
        {
            Name = "GOODYHUT_DYNAMIC",
            Title = title,
            Width = 420,
            Button = [Labels.Ok],
            Text = [message],
            LineStyles = [TextStyles.Left]
        };

        CivDialog? dialog = null;
        dialog = new CivDialog(_gameScreen.Main, dialogElements, (_, _, _, _) => _gameScreen.CloseDialog(dialog));
        _gameScreen.ShowDialog(dialog, stack: true);
    }

    public void SelectTechFromConquest(List<Advance> techs)
    {
        var advance = _gameScreen.Game.Random.ChooseFrom(techs);
        _gameScreen.Game.GiveAdvance(advance.Index, Civilization);
        
        //TODO: Show popup
    }

    public void CityLost(City city)
    {
        //TODO: Show info ? is game over?
    }

    /// <summary>
    /// A Diplomat has reached somebody else's unit or city. Offer what can be done
    /// with it, and how much it costs.
    /// <para>
    /// A Diplomat has no attack strength, so before this the engine simply refused
    /// the move and said so. The unit could be built and walked across the map and
    /// then did nothing at all.
    /// </para>
    /// </summary>
    public void DiplomatArrived(Unit diplomat, Tile target)
    {
        var game = _gameScreen.Game;
        var city = DiplomatActions.EnemyCityAt(diplomat, target);
        var unit = DiplomatActions.BribableUnitAt(diplomat, target);

        SessionLog.Record($"diplomat at {target.X},{target.Y} " +
                          $"(city={city?.Name ?? "none"}, unit={unit?.Name ?? "none"})");

        if (city != null && unit != null)
        {
            // Both are possible, so let the player say which.
            _gameScreen.ShowPopup("DIPLOMATACTION", handleButtonClick: (button, selection, _, _) =>
            {
                if (button != Labels.Ok)
                {
                    return;
                }

                if (selection == 0)
                {
                    OfferToIncite(diplomat, city);
                }
                else
                {
                    OfferToBribe(diplomat, unit);
                }
            });
            return;
        }

        if (city != null)
        {
            OfferToIncite(diplomat, city);
            return;
        }

        if (unit != null)
        {
            OfferToBribe(diplomat, unit);
            return;
        }

        // Something is here, but not something that can be bought: a stack keeping
        // an eye on each other, or a garrison that has to be taken with the city.
        _gameScreen.ShowPopup("CANNOTBRIBE");
    }

    private void OfferToBribe(Unit diplomat, Unit target)
    {
        var cost = DiplomatActions.BribeCost(_gameScreen.Game, target);
        var treasury = diplomat.Owner.Money;
        if (cost > treasury)
        {
            _gameScreen.ShowPopup("NODIPLOMATGOLD", replaceNumbers: [cost, treasury]);
            return;
        }

        _gameScreen.ShowPopup("BRIBEUNIT", handleButtonClick: (button, _, _, _) =>
        {
            if (button != Labels.Ok || diplomat.Dead || target.Dead)
            {
                return;
            }

            if (DiplomatActions.BribeUnit(_gameScreen.Game, diplomat, target))
            {
                _gameScreen.ForceRedraw();
                _gameScreen.Game.ChooseNextUnit();
            }
        }, replaceNumbers: [cost, treasury], replaceStrings: [target.Name]);
    }

    private void OfferToIncite(Unit diplomat, City city)
    {
        if (!DiplomatActions.CanIncite(city))
        {
            _gameScreen.ShowPopup("CANNOTINCITE", replaceStrings: [city.Name]);
            return;
        }

        var cost = DiplomatActions.InciteCost(_gameScreen.Game, city);
        var treasury = diplomat.Owner.Money;
        if (cost > treasury)
        {
            _gameScreen.ShowPopup("NODIPLOMATGOLD", replaceNumbers: [cost, treasury]);
            return;
        }

        _gameScreen.ShowPopup("INCITEREVOLT", handleButtonClick: (button, _, _, _) =>
        {
            if (button != Labels.Ok || diplomat.Dead)
            {
                return;
            }

            if (DiplomatActions.InciteRevolt(_gameScreen.Game, diplomat, city))
            {
                _gameScreen.ForceRedraw();
                _gameScreen.Game.ChooseNextUnit();
            }
        }, replaceNumbers: [cost, treasury],
           replaceStrings: [city.Name, city.Owner.TribeName]);
    }

    public void CityCaptured(City city)
    {
       //TODO: Show popup?? what does the game do here? 
    }
}
