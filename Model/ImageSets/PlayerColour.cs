using System.Numerics;
using Model.Images;
using Raylib_CSharp.Colors;

namespace Model.ImageSets;

public class PlayerColour
{
    public IImageSource Image { get; set; } = null!;

    /// <summary>
    /// The high-resolution flag, when the bundled art set has one.
    /// <para>
    /// The classic flag is a sprite a dozen pixels across. Drawn beside a city on
    /// a map composed at several times Civ II's tile size it is enlarged by the
    /// same factor, which is why the flag over a city was a blur while everything
    /// around it was sharp. When this is set the map draws it instead, fitted to
    /// <see cref="LogicalSize"/> so nothing moves.
    /// </para>
    /// </summary>
    public IImageSource? MapImage { get; set; }

    /// <summary>
    /// The footprint the classic flag occupies, which the high-resolution art is
    /// fitted into so city layout and the flag's anchor are unchanged.
    /// </summary>
    public Vector2 LogicalSize { get; set; }

    public Color DarkColour { get; set; }
    public Color TextColour { get; set; }
    public Color LightColour { get; set; }
}
