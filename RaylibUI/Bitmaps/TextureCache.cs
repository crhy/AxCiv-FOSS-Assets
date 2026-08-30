using Civ2engine;
using Model;
using Model.Images;
using Raylib_CSharp.Textures;
using Raylib_CSharp.Colors;
using RaylibUtils;

namespace RaylibUI;

public static class TextureCache
{
    private static readonly Dictionary<string, Texture2D> Textures = new();

    public static Texture2D GetBordered(IUserInterface active, string name, IImageSource source)
    {
        if (!Textures.ContainsKey(name))
        {
            var padding = active.GetPadding(0f, false);
            var copy = Images.ExtractBitmap(source).Copy();
            copy.ResizeCanvas(copy.Width + padding.Left + padding.Right, copy.Height + padding.Left + padding.Right, padding.Left, padding.Top, Color.White);
            ImageUtils.PaintPanelBorders(active, ref copy, copy.Width, copy.Height, padding);
            Textures[name] = Texture2D.LoadFromImage(copy);
        }

        return Textures[name];
    }

    public static Texture2D GetImage(IImageSource source)
    {
        return GetImage(source, null, -1);
    }

    public static Texture2D GetImage(IImageSource source, IUserInterface? activeInterface = null, int civ = -1)
    {
        var key = source.GetKey( civ);
        if (!Textures.ContainsKey(key))
        {
            var img = Images.ExtractBitmapData(source, activeInterface, civ).Image;
            Textures[key] = Texture2D.LoadFromImage(img);
            // Large FOSS sources are intentionally sampled down to Civ II's logical
            // footprint. Bilinear filtering retains their detail; classic sprites
            // retain the configured (normally nearest-neighbour) pixel-art filter.
            var filter = img.Width >= 256 || img.Height >= 256
                ? TextureFilter.Bilinear
                : (TextureFilter)Settings.TextureFilter;
            Textures[key].SetFilter(filter);
        }
        return Textures[key];
    }

    public static void Clear()
    {
        foreach (var texture in Textures.Where(t => !t.Key.StartsWith("Binary")))
        {
            texture.Value.Unload();
            Textures.Remove(texture.Key);
        }
        Images.ClearCache();
    }
}
