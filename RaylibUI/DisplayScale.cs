using System.Numerics;
using Raylib_CSharp.Camera.Cam2D;
using Raylib_CSharp.Interact;
using Raylib_CSharp.Windowing;

namespace RaylibUI;

/// <summary>
/// Keeps UI layout in a stable logical coordinate system while rendering it at
/// the native resolution of high-density displays.
/// </summary>
public static class DisplayScale
{
    private const float ReferenceWidth = 1920f;
    private const float ReferenceHeight = 1080f;
    private const float MaximumScale = 2.5f;

    public static float Factor { get; private set; } = 1f;
    public static int Width => Math.Max(1, (int)MathF.Floor(Window.GetScreenWidth() / Factor));
    public static int Height => Math.Max(1, (int)MathF.Floor(Window.GetScreenHeight() / Factor));
    public static Camera2D Camera => new(Vector2.Zero, Vector2.Zero, 0f, Factor);

    /// <summary>True for the frame in which the display's logical scale changed.</summary>
    public static bool Changed { get; private set; }

    public static void Update()
    {
        var widthScale = Window.GetScreenWidth() / ReferenceWidth;
        var heightScale = Window.GetScreenHeight() / ReferenceHeight;
        var next = Math.Clamp(Math.Min(widthScale, heightScale), 1f, MaximumScale);

        // Quantizing avoids needless map texture rebuilds while resizing a window.
        next = MathF.Round(next * 4f) / 4f;
        Changed = Math.Abs(next - Factor) > 0.001f;
        Factor = next;

        // Raylib applies this transform before UI hit testing sees the pointer.
        Input.SetMouseScale(1f / Factor, 1f / Factor);
    }
}
