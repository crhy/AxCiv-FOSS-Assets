using System.Collections.Generic;
using RhyCiv.Engine.Enums;
using RhyCiv.Engine.Units;
using Model.Core.Units;

namespace RhyCiv.Engine.Events
{
    public class CombatEventArgs : UnitEventArgs
    {
        public List<bool> CombatRoundsAttackerWins;

        public UnitInfo Defender;

        public CombatEventArgs(UnitEventType eventType, Unit attacker, Unit defender, List<bool> combatRoundsAttackerWins,
            List<int> attackerHitpoints, List<int> defenderHitpoints) : base(eventType, new [] { attacker.CurrentLocation, defender.CurrentLocation })
        {
            Attacker = new UnitInfo(attacker, attackerHitpoints);
            Defender = new UnitInfo(defender, defenderHitpoints);
            CombatRoundsAttackerWins = combatRoundsAttackerWins;
            Sound = attacker.AttackSound;
        }

        public UnitInfo Attacker { get; set; }
        public string Sound { get; }
    }
}