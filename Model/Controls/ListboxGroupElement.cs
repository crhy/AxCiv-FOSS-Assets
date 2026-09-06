using Model.Core;
using Model.Core.Units;
using Model.Images;
using Raylib_CSharp.Colors;

namespace Model.Controls;

/// <summary>
/// Texts and images that are formed into groups of elements in listbox.
/// </summary>
public class ListboxGroupElement
{
    public IUnit? Unit { get; set; }
    public IGame? Game { get; set; }
    public string Text { get; set; } = string.Empty;
    public int? TextSizeOverride { get; set; } = null;
    public Color? FrontColorOverride { get; set; } = null;
    public Color? ShadowColorOverride { get; set; } = null;
    public HorizontalAlignment HorizontalAlignment { get; set; } = HorizontalAlignment.Left;
    public VerticalAlignment VerticalAlignment { get; set; } = VerticalAlignment.Top;
    public IImageSource? Icon { get; set; }
    public float ScaleIcon { get; set; } = 1.0f;

    /// <summary>
    /// Scale the icon to fill its cell, up as well as down, ignoring
    /// <see cref="ScaleIcon"/>. Use this where the row has a size it wants the art
    /// to meet, rather than a fixed multiplier tuned to one particular source
    /// resolution: a hard-coded multiplier silently becomes wrong the moment the
    /// art behind it is redrawn at a different size.
    /// </summary>
    public bool FitIconToCell { get; set; }

    /// <summary>
    /// Custom width of control.
    /// </summary>
    public int? Width { get; set; }

    /// <summary>
    /// Custom height of control.
    /// </summary>
    public int? Height { get; set; }

    /// <summary>
    /// Custom x-offset of control. Otherwise it's positioned after previous control.
    /// </summary>
    public int? Xoffset { get; set; }
}