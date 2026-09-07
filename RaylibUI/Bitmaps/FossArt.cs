using RhyCiv.Engine;
using Model.Images;
using Raylib_CSharp.Textures;
using RaylibUtils;

namespace RaylibUI;

/// <summary>
/// Loads a piece of the bundled art set straight from disk, for the few pictures
/// the interface draws that are not part of a ruleset's image sets.
/// <para>
/// The terrain and unit art all arrives through <see cref="IUserInterface"/>'s
/// picture sources, which are keyed to the compatibility sheets. A marker like the
/// fallen-soldier icon has no sheet cell to be keyed to, so rather than inventing
/// one in all three interfaces it is fetched by name from the same search path
/// the terrain textures use.
/// </para>
/// </summary>
public static class FossArt
{
    private static readonly Dictionary<string, IImageSource?> Sources = new();

    /// <summary>
    /// The texture for a file under FOSSart, or null when it is not on disk. A
    /// missing file is remembered, so a build without the art does not stat it
    /// once per frame.
    /// </summary>
    public static Texture2D? GetTexture(string relativePath)
    {
        if (!Sources.TryGetValue(relativePath, out var source))
        {
            var path = Find(relativePath);
            if (path == null)
            {
                source = null;
            }
            else
            {
                var image = Images.LoadImageFromFile(path).Image;
                source = image.Width > 1 && image.Height > 1
                    ? new MemoryStorage(image, $"FossArt-{relativePath}")
                    : null;
            }

            Sources[relativePath] = source;
        }

        return source == null ? null : TextureCache.GetImage(source);
    }

    private static string? Find(string relativePath)
    {
        var roots = Settings.SearchPaths
            .Concat([
                Environment.CurrentDirectory,
                AppContext.BaseDirectory,
                Path.Combine(Environment.CurrentDirectory, "RaylibUI")
            ])
            .Where(root => !string.IsNullOrWhiteSpace(root))
            .Distinct(StringComparer.OrdinalIgnoreCase);

        foreach (var root in roots)
        {
            foreach (var candidate in new[]
                     {
                         Path.Combine(root, relativePath),
                         Path.Combine(root, "FOSSart", relativePath),
                         Path.Combine(root, "RaylibUI", "FOSSart", relativePath)
                     })
            {
                if (File.Exists(candidate))
                {
                    return candidate;
                }
            }
        }

        return null;
    }
}
