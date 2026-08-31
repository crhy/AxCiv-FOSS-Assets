using System.Numerics;
using Civ2engine.MapObjects;
using Model.Core.Mapping;
using Model.Interface;
using Raylib_CSharp.Fonts;
using Raylib_CSharp.Rendering;
using Raylib_CSharp.Colors;

namespace RaylibUI.RunGame.GameControls.Mapping;

public class TextElement : IViewElement
{
    private readonly string _text;
    private readonly int _height;
    private readonly Color _color;
    private readonly Color? _background;


    public TextElement(string text, Vector2 loc, int height, Tile tile, Vector2 offset,
        Color? color = null, Color? background = null)
    {
        _text = text;
        _height = height;
        Location = loc;
        Tile = tile;
        Offset = offset;
        _color = color ?? Color.Black;
        _background = background;
    }

    public Vector2 Offset { get; set; }

    public Vector2 Location { get; set; }
    public Tile Tile { get; set; }
    public bool IsTerrain => false;
    public bool IsShaded => false;

    public void Draw(Vector2 adjustedLocation, float scale = 1f, bool isShaded = false)
    {
        var loc = adjustedLocation + Offset * scale;
        var fontSize = Math.Max(1, (int)MathF.Round(_height * scale));
        var size = TextRendering.Measure(Fonts.Arial, _text, fontSize, 0);
        var textPosition = new Vector2(
            MathF.Round(loc.X - size.X / 2f),
            MathF.Round(loc.Y - size.Y / 2f));

        if (_background is { } background)
        {
            Graphics.DrawRectangle((int)textPosition.X - 2, (int)textPosition.Y - 1,
                (int)size.X + 4, (int)size.Y + 2, background);
        }
        global::RaylibUI.TextRendering.Draw(Fonts.Arial, _text, textPosition, fontSize, 0, _color);
    }

    public IViewElement CloneForLocation(Vector2 newLocation)
    {
        return new TextElement(_text, newLocation, _height, Tile, Offset, _color, _background);
    }
}
