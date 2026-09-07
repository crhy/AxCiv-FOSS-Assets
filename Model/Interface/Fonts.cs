using Raylib_CSharp.Fonts;
using Raylib_CSharp.Textures;

namespace Model.Interface;

public static class Fonts
{
    /// <summary>
    /// Times new roman
    /// </summary>
    public static Font Tnr { get; set; } = Font.GetDefault();

    /// <summary>
    /// Bold times new roman font
    /// </summary>
    public static Font TnRbold { get; set; } = Font.GetDefault();

    /// <summary>
    /// Alternative font
    /// </summary>
    public static Font Arial { get; set; } = Font.GetDefault();

    public const int FontSize = 20;

    /// <summary>
    /// Prepares a loaded face for drawing at any size.
    /// <para>
    /// The atlases are rasterised at 96 to 112 pixels so headings stay sharp, but
    /// most text is drawn between 14 and 20. Bilinear filtering takes only a 2x2
    /// sample, so shrinking a glyph by five times sampled a twentieth of the pixels
    /// it was covering and dropped most of the stroke: body text came out with
    /// broken serifs and stems that flickered as the window moved. Mipmaps give the
    /// small sizes something properly downsampled to read from.
    /// </para>
    /// </summary>
    private static Font Prepare(Font font)
    {
        font.Texture.GenMipmaps();
        font.Texture.SetFilter(TextureFilter.Trilinear);
        return font;
    }

    public static void SetTnr(Font font)
    {
        Tnr = Prepare(font);
    }

    public static void SetArial(Font font)
    {
        Arial = Prepare(font);
    }

    public static void SetBold(Font font)
    {
        TnRbold = Prepare(font);
    }
}
