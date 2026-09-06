using System;
using RhyCiv.Engine.Events;
using RhyCiv.Engine.Scripting.ScriptObjects;
using RhyCiv.Engine.Units;
using Model.Core.Units;

// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable InconsistentNaming

namespace RhyCiv.Engine.Scripting
{
    public class ScenarioHooks(Game game)
    {
        #region onActivateUnit
        public void onActivateUnit(Func<UnitApi, bool, bool, object> activateHook)
        {
            game.OnUnitEvent += (_, args) =>
            {
                if (args is ActivationEventArgs activationArgs)
                {
                    activateHook(new UnitApi(activationArgs.Unit, game), activationArgs.UserInitiated, activationArgs.Reactivation);
                }
            };
        }
        
        public void onActivateUnit(Func<UnitApi, bool, object> activateHook)
        {
            game.OnUnitEvent += (_, args) =>
            {
                if (args is ActivationEventArgs activationArgs)
                {
                    activateHook(new UnitApi(activationArgs.Unit, game), activationArgs.UserInitiated);
                }
            };
        }
        
        public void onActivateUnit(Func<UnitApi, object> activateHook)
        {
            game.OnUnitEvent += (_, args) =>
            {
                if (args is ActivationEventArgs activationArgs)
                {
                    activateHook(new UnitApi(activationArgs.Unit, game));
                }
            };
        }
        public void onActivateUnit(Func<object> activateHook)
        {
            game.OnUnitEvent += (_, args) =>
            {
                if (args is ActivationEventArgs)
                {
                    activateHook();
                }
            };
        }
        #endregion
    }
}