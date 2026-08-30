using Model;
using Model.Images;
using Raylib_CSharp.Colors;
using Raylib_CSharp.Rendering;
using Raylib_CSharp.Textures;

namespace RaylibUI.Controls;

public class ImagePanel
{
    private readonly Texture2D _texture;

    public ImagePanel(IUserInterface active, string name, IImageSource imageSource, Point location)
    {
        Key = name;
        Location = location;

        _texture = TextureCache.GetBordered(active, name, imageSource);
    }

    public string Key { get; }
    public Point Location { get; set; }

    public void Draw()
    {
        int x, y;

        // Panel position on screen
        if (Location.X < 0) // offset from right
        {
            x = (int)((1 + Location.X) * DisplayScale.Width) - _texture.Width;
        }
        else if (Location.X > 0)
        {
            x = (int)(Location.X * DisplayScale.Width);
        }
        else // =0 (center on screen)
        {
            x = (int)(DisplayScale.Width * 0.5 - _texture.Width * 0.5);
        }

        if (Location.Y < 0)
        {
            y = (int)((1 + Location.Y) * DisplayScale.Height) - _texture.Height;
        }
        else if (Location.Y > 0)
        {
            y = (int)(Location.Y * DisplayScale.Height);
        }
        else
        {
            y = (int)(DisplayScale.Height * 0.5 - _texture.Height * 0.5);
        }

        Graphics.DrawTexture(_texture, x, y, Color.White);
    }
}
