using System.Numerics;
using Model;
using Model.Interface;
using Raylib_CSharp;
using Raylib_CSharp.Fonts;
using Raylib_CSharp.Colors;
using Raylib_CSharp.Rendering;
using Model.Controls;

namespace RaylibUI.BasicTypes.Controls;

public class LabelControl : BaseControl
{
    private readonly int _minWidth;
    private readonly int _defaultHeight;
    private readonly float _spacing;
    private readonly Color[]? _switchColors;
    private readonly int _switchTime;

    public LabelControl(IControlLayout controller,
        string text,
        bool eventTransparent,
        int minWidth = -1,
        Padding padding = default,
        HorizontalAlignment horizontalAlignment = HorizontalAlignment.Left,
        VerticalAlignment verticalAlignment = VerticalAlignment.Top,
        int defaultHeight = 32,
        Font? font = null,
        int fontSize = 20,
        float spacing = 0f,
        Color? colorFront = null,
        Color? colorShadow = null,
        Vector2? shadowOffset = null,
        Color[]? switchColors = null,
        Color? colorBack = null,
        int switchTime = 0) : base(controller,
        eventTransparent: eventTransparent)
    {
        Padding = padding;
        _text = text;
        HorizontalAlignment = horizontalAlignment;
        VerticalAlignment = verticalAlignment;
        _minWidth = minWidth;
        _defaultHeight = defaultHeight;
        _fontSize = Math.Max(1, fontSize);
        _spacing = spacing;
        _font = font ?? controller.MainWindow.ActiveInterface?.Look.LabelFont ?? Fonts.Tnr;
        ColorFront = colorFront ?? TextRendering.StrongBlack;
        ColorShadow = colorShadow ?? Color.Blank;
        ShadowOffset = shadowOffset ?? Vector2.Zero;

        _switchColors = switchColors;
        _switchTime = switchTime;
        BackgroundColor = colorBack;
        _textSize = TextRendering.Measure(_font, _text, _fontSize, _spacing);
    }

    private Vector2 _textSize;
    public Vector2 TextSize => _textSize;

    private string _text;
    public string Text
    {
        get => _text;
        set
        {
            _text = value;
            _textSize = TextRendering.Measure(_font, _text, _fontSize, _spacing);
        }
    }

    private Font _font;
    public Font Font
    {
        get => _font;
        set
        {
            _font = value;
            _textSize = TextRendering.Measure(_font, _text, _fontSize, _spacing);
        }
    }

    private int _fontSize;
    public int FontSize
    {
        get => _fontSize;
        set
        {
            _fontSize = value;
            _textSize = TextRendering.Measure(_font, _text, _fontSize, _spacing);
        }
    }

    public HorizontalAlignment HorizontalAlignment { get; set; }
    public VerticalAlignment VerticalAlignment { get; set; }

    public Color? BackgroundColor { get; set; }
    public Color ColorFront { get; set; }
    public Color ColorShadow { get; set; }
    public Vector2 ShadowOffset { get; set; }

    public Padding Padding { get; set; }

    private int _width;
    public override int Width
    {
        get => _width == 0 ? GetPreferredWidth() : _width;
        set { _width = value; }
    }

    private int _height;
    public override int Height
    {
        get => _height == 0 ? GetPreferredHeight() : _height;
        set { _height = value; }
    }

    public override int GetPreferredWidth()
    {
        return Math.Max(_minWidth, (int)_textSize.X + Padding.Left + Padding.Right + (HorizontalAlignment == HorizontalAlignment.Center ? 10 : 0));
    }

    public override int GetPreferredHeight()
    {
        return Math.Max(_defaultHeight, (int)MathF.Ceiling(_textSize.Y) + Padding.Top + Padding.Bottom);
    }

    public override void Draw(bool pulse)
    {
        if (!Visible) return;

        if (BackgroundColor != null)
        {
            Graphics.DrawRectangleRec(Bounds, BackgroundColor.Value);
        }

        var availableWidth = Width - Padding.Left - Padding.Right;
        var availableHeight = Height - Padding.Top - Padding.Bottom;
        var fontSize = TextRendering.FitFontSize(_font, _text, _fontSize,
            Math.Max(1, availableWidth), Math.Max(1, availableHeight), _spacing);
        var textSize = fontSize == _fontSize
            ? _textSize
            : TextRendering.Measure(_font, _text, fontSize, _spacing);

        var textPosition = new Vector2(Bounds.X + Padding.Left, Bounds.Y + Padding.Top);

        if (HorizontalAlignment == HorizontalAlignment.Center)
        {
            textPosition.X += availableWidth / 2f - textSize.X / 2f;
        }
        else if (HorizontalAlignment == HorizontalAlignment.Right)
        {
            textPosition.X += availableWidth - textSize.X;
        }

        if (VerticalAlignment == VerticalAlignment.Center)
        {
            textPosition.Y += (Height - Padding.Top - Padding.Bottom) / 2f - textSize.Y / 2f;
        }
        else if (VerticalAlignment == VerticalAlignment.Bottom)
        {
            textPosition.Y += Height - Padding.Top - Padding.Bottom - textSize.Y;
        }

        Color colorFront;
        Color colorShadow;
        if (_switchColors is not null)
        {
            var switchIndex = _switchTime > 0
                ? (int)(Time.GetTime() * 1000 / _switchTime) % _switchColors.Length
                : 0;
            colorFront = _switchColors[switchIndex];
            colorShadow = Color.Blank;
        }
        else
        {
            colorFront = ColorFront;
            colorShadow = ColorShadow;
        }

        textPosition = new Vector2(MathF.Round(textPosition.X), MathF.Round(textPosition.Y));

        if (ShadowOffset != Vector2.Zero && colorShadow.A > 0)
        {
            TextRendering.DrawWithShadow(_font, _text, textPosition, fontSize, _spacing, colorFront, colorShadow, ShadowOffset);
        }
        else
        {
            TextRendering.Draw(_font, _text, textPosition, fontSize, _spacing, colorFront);
        }

        base.Draw(pulse);
    }
}
