using System.Numerics;
using Civ2engine.MapObjects;
using Civ2engine.Units;
using Model.Core.Mapping;
using RaylibUI.RunGame.GameControls.Mapping.Views.ViewElements;

namespace RaylibUI.RunGame.GameControls.Mapping.Views;

public class WaitingView : BaseGameView
{
    public WaitingView(GameScreen gameScreen, IGameView? currentView, int viewHeight,
        int viewWidth, bool forceRedraw) : base(gameScreen, gameScreen.Game.ActivePlayer.ActiveTile,
        currentView, viewHeight, viewWidth, true, 200, Array.Empty<Tile>(), forceRedraw)
    {
        var activeInterface = gameScreen.Main.ActiveInterface;

        // The marker art is a full map tile now, not the classic 64x32 sprite, so it
        // has to be told the footprint it stands for. Drawn raw it came out five
        // times the size of its tile and streaked across the map.
        var marker = TextureCache.GetImage(activeInterface.MapImages.ViewPiece);
        var logicalSize = new Vector2(MapImage.TileRec.Width, MapImage.TileRec.Height);
        var renderScale = marker.Width > 0 ? logicalSize.X / marker.Width : 1f;

        SetAnimation(new[]
        {
            new TextureElement(texture: marker,
                location: ActivePos, gameScreen.Game.ActivePlayer.ActiveTile,
                renderScale: renderScale, maxDrawSize: logicalSize)
        });


        SetAnimation(Array.Empty<TextureElement>());
    }
}