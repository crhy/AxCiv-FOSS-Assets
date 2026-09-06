using RhyCiv.Engine.Enums;
using RhyCiv.Engine.Events;
using Model.Controls;
using JetBrains.Annotations;
using Model.Input;

namespace RaylibUI.RunGame.Commands.Zoom;

[UsedImplicitly]
public class MaxZoomIn(GameScreen gameScreen) :  AlwaysOnCommand(gameScreen,CommandIds.MaxZoomIn, [new Shortcut(Key.Z, ctrl: true)])
{
    public override void Action()
    {
        if (GameScreen.Zoom < GameScreen.MaximumZoom)
            GameScreen.TriggerMapEvent(new MapEventArgs(MapEventType.ZoomChange) { Zoom = GameScreen.MaximumZoom });
    }
}
