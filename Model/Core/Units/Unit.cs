using Civ2engine;
using Civ2engine.Enums;
using Civ2engine.Units;
using Model.Constants;
using Model.Core.Cities;
using Model.Core.Mapping;

namespace Model.Core.Units
{
    public class Unit : IUnit
    {
        private Tile? _currentLocation;
        private bool _dead;

        // From RULES.TXT
        public string Name => TypeDefinition.Name;

        public bool Dead
        {
            get => _dead;
            set
            {
                if (value)
                {
                    _currentLocation?.UnitsHere.Remove(this);
                }
                _dead = value;
            }
        }

        public int UntilTech => TypeDefinition.Until;
        public UnitGas Domain => TypeDefinition.Domain;
        public int MaxMovePoints => TypeDefinition.Move + BonusMovePoints;

        /// <summary>
        /// Extra movement granted by wonders, recalculated at the start of each of
        /// the owner's turns. Derived state, so it is not saved.
        /// </summary>
        public int BonusMovePoints { get; set; }
        public int FuelRange => TypeDefinition.Range;
        public int AttackBase => TypeDefinition.Attack;
        public int DefenseBase => TypeDefinition.Defense;
        
        public UnitDefinition TypeDefinition { get; set; } = new();

        public int FirepowerBase => TypeDefinition.Firepwr;

        public int Cost => TypeDefinition.Cost;
        public int ShipHold => TypeDefinition.Hold;
        public AiRoleType AiRole => TypeDefinition.AIrole;
        /// <summary>
        /// Reads one of the rules' unit flags. The flag field is a variable-length
        /// bit string, so a ruleset that defines fewer flags than the engine knows
        /// about leaves the remainder unset rather than throwing.
        /// </summary>
        private bool Flag(int index)
        {
            var flags = TypeDefinition.Flags;
            return flags.Length > index && flags[index];
        }

        public bool TwoSpaceVisibility => Flag(0);
        public bool IgnoreZonesOfControl => Flag(1) || Domain == UnitGas.Air || Domain == UnitGas.Sea;
        public bool CanMakeAmphibiousAssaults => Flag(2);
        public bool SubmarineAdvantagesDisadvantages => Flag(3);
        public bool CanAttackAirUnits => Flag(4);    // fighter
        public bool ShipMustStayNearLand => Flag(5);  // trireme
        public bool NegatesCityWalls => Flag(6);  // howitzer
        public bool CanCarryAirUnits => Flag(7);  // carrier
        public bool CanMakeParadrops => Flag(8);
        public bool Alpine => Flag(9);    // treats all squares as road
        public bool X2OnDefenseVersusHorse => Flag(10);    // pikemen
        public bool FreeSupportForFundamentalism => Flag(11);    // fanatics
        public bool DestroyedAfterAttacking => Flag(12);    // missiles
        public bool X2OnDefenseVersusAir => Flag(13);    // AEGIS
        public bool UnitCanSpotSubmarines => Flag(4);


        public int Id { get; set; }

        public int MovePoints => MaxMovePoints - MovePointsLost;

        public int MovePointsLost { get; set; }
        public int HitpointsBase => TypeDefinition.Hitp;
        public int RemainingHitpoints => HitpointsBase - HitPointsLost;
        public int HitPointsLost { get; set; }

        public int Type => TypeDefinition.Type;

        public int Order { get; set; }
        public bool MadeFirstMove { get; set; }
        public bool Veteran { get; set; }
        public bool WaitOrder { get; set; }
        public Civilization Owner { get; set; } = new();
        public int CaravanCommodity { get; set; }
        public City? HomeCity { get; set; }
        public int GoToX { get; set; }
        public int GoToY { get; set; }
        public int GoToMapIndex { get; set; }
        public int LinkOtherUnitsOnTop { get; set; }
        public int LinkOtherUnitsUnder { get; set; }
        public int Counter { get; set; }

        /// <summary>
        /// Consecutive turns an air unit has spent away from a city, airbase or
        /// carrier. Civ II crashes an air unit once this reaches its fuel range.
        /// </summary>
        public int TurnsAirborne { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int MapIndex { get; set; }
        public bool[] VisibilityByCiv { get; set; } = [];
        public int Animation { get; set; }
        public int Orientation { get; set; }

        public int[] PrevXy { get; set; } = [];   // XY position of unit before it moved

        public bool TurnEnded => MovePoints <= 0 ||
                                 Order is (int)OrderType.Fortified or (int)OrderType.Transform or (int)OrderType.Fortify or
                                     (int)OrderType.BuildIrrigation or (int)OrderType.BuildRoad or (int)OrderType.BuildAirbase or
                                     (int)OrderType
                                         .BuildFortress or (int)OrderType.BuildMine;
    

        public bool AwaitingOrders => !TurnEnded && !Dead && (Order is (int)OrderType.NoOrders);

        public void SkipTurn()
        {
            MovePointsLost = MaxMovePoints;
            PrevXy = [X, Y];
        }

        public void Sleep()
        {
            Order = (int)OrderType.Sleep;
        }

        public bool IsInStack => CurrentLocation is { UnitsHere.Count: > 1 };

        public Unit? InShip { get; set; }

        public string AttackSound => TypeDefinition.AttackSound;
        public List<Unit> CarriedUnits { get; } = new();

        public Tile CurrentLocation
        {
            get => _currentLocation ?? throw new InvalidOperationException("Unit has no current location.");
            set
            {
                if(_dead) return; //dead units can't move
                if(_currentLocation == value) return;
                _currentLocation?.UnitsHere.RemoveAll(u=> u== this);
                if (value != null && !value.UnitsHere.Contains(this))
                {
                    value.UnitsHere.Add(this);
                }
                _currentLocation = value;
            }
        }

        public bool FreeSupport(int[] typesWithFreeSupport)
        {
            return AiRole is AiRoleType.Diplomacy or AiRoleType.Trade || (typesWithFreeSupport.Contains(Type));
        }

        public bool NeedsSupport { get; set; } = true;

        public void ProcessOrder()
        {
            Counter += TypeDefinition.WorkRate;
            MovePointsLost = MovePoints;
        }

        public void Build(TerrainImprovement improvement)
        {
            Building = improvement.Id;
            ProcessOrder();
            // This is a cludge but it will work for now
            Order = improvement.Id;
        }

        public int Building { get; set; }
        public int AttacksSpent { get; set; }

        public Dictionary<string, string> ExtendedData { get;} = new();
    }
}
