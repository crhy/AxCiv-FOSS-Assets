using RhyCiv.Engine.Enums;
using RhyCiv.Engine.Events;
using Model.Controls;
using JetBrains.Annotations;
using Model.Input;

namespace RaylibUI.RunGame.Commands.Zoom;

[UsedImplicitly]
public class MaxZoomOut(GameScreen gameScreen) :  AlwaysOnCommand(gameScreen,CommandIds.MaxZoomOut, [new Shortcut(Key.G, ctrl: true)])
{
    public override void Action()
    {
        if (GameScreen.Zoom > GameScreen.MinimumZoom)
            GameScreen.TriggerMapEvent(new MapEventArgs(MapEventType.ZoomChange) { Zoom = GameScreen.MinimumZoom });
    }
}
