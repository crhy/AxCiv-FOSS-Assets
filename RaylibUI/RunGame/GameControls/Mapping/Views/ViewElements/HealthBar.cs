using System.Numerics;
using RhyCiv.Engine.MapObjects;
using Model;
using Model.Core.Mapping;
using Model.ImageSets;
using Raylib_CSharp.Colors;
using Raylib_CSharp.Rendering;

namespace RaylibUI.RunGame.GameControls.Mapping;

public class HealthBar : IViewElement
{
    private readonly float _healthFraction;
    private readonly Vector2 _trackSize;
    private readonly Color _fillColor;

    public HealthBar(Vector2 location, Tile tile, int remainingHitPoints, int hitPointsBase, Vector2 offset, UnitShield shield)
    {
        Location = location;
        Tile = tile;
        Offset = offset;
        BaseHitpoints = hitPointsBase;
        _trackSize = shield.HPbarSize;
        _healthFraction = hitPointsBase <= 0
            ? 0f
            : Math.Clamp(remainingHitPoints / (float)hitPointsBase, 0f, 1f);
        var hpBarX = (int)MathF.Floor(_healthFraction * shield.HPbarSize.X);
        
        if (hpBarX <= shield.HPbarSizeForColours[0])
        {
            _fillColor = shield.HPbarColours[0];
        }
        else if (hpBarX <= shield.HPbarSizeForColours[1])
        {
            _fillColor = shield.HPbarColours[1];
        }
        else
        {
            _fillColor = shield.HPbarColours[2];
        }
    }

    public int BaseHitpoints { get; }
    public Vector2 Location { get; set; }
    public Tile Tile { get; set; }
    public Vector2 Offset { get; }
    public bool IsTerrain => false;
    public bool IsShaded => false;

    public void Draw(Vector2 adjustedLocation, float scale = 1f, bool isShaded = false)
    {
        var origin = adjustedLocation + Offset * scale;
        var x = (int)MathF.Round(origin.X);
        var y = (int)MathF.Round(origin.Y);
        var width = Math.Max(1, (int)MathF.Round(_trackSize.X * scale));
        var height = Math.Max(1, (int)MathF.Round(_trackSize.Y * scale));

        // Civ II's bar is a tiny black track with a saturated traffic-light fill.
        // Drawing it at final screen resolution keeps the edge and empty portion
        // sharp at every zoom level.
        Graphics.DrawRectangle(x, y, width, height, new Color(7, 7, 9, 255));
        // Gold's five-pixel bar has the original inset black frame. Test of
        // Time uses a two-pixel strip, which must remain fully usable as fill.
        var border = _trackSize.Y >= 4 ? Math.Max(1, (int)MathF.Round(scale)) : 0;
        var innerWidth = Math.Max(0, width - border * 2);
        var innerHeight = Math.Max(1, height - border * 2);
        var fillWidth = Math.Clamp((int)MathF.Round(innerWidth * _healthFraction), 0, innerWidth);
        if (fillWidth <= 0)
        {
            return;
        }

        Graphics.DrawRectangle(x + border, y + border, fillWidth, innerHeight, _fillColor);
        if (innerHeight >= 4)
        {
            var highlight = new Color(
                (byte)Math.Min(255, _fillColor.R + 42),
                (byte)Math.Min(255, _fillColor.G + 42),
                (byte)Math.Min(255, _fillColor.B + 42),
                210);
            Graphics.DrawLine(x + border, y + border, x + border + fillWidth - 1, y + border, highlight);
        }
    }

    public IViewElement CloneForLocation(Vector2 newLocation)
    {
        return new HealthBar(newLocation, Tile, _healthFraction, BaseHitpoints, Offset, _trackSize, _fillColor);
    }

    private HealthBar(Vector2 location, Tile tile, float healthFraction, int baseHitpoints, Vector2 offset,
        Vector2 trackSize, Color fillColor)
    {
        Location = location;
        Tile = tile;
        Offset = offset;
        BaseHitpoints = baseHitpoints;
        _healthFraction = healthFraction;
        _trackSize = trackSize;
        _fillColor = fillColor;
    }
}
