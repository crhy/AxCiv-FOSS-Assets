using Raylib_CSharp.Textures;

namespace RaylibUI.RunGame.GameControls.CityControls;

/// <summary>
/// The resource medallions ship at a much higher resolution than the 14x14 and
/// 10x10 sheet patches they replaced, so nothing may draw one at its native
/// size any more. Everything asks for the footprint it wants in the city
/// window's own logical pixels and gets back the scale that fills it.
/// </summary>
public static class ResourceIconScale
{
    /// <summary>The footprint the classic large icons occupied.</summary>
    public const float LargeLogicalSize = 14f;

    /// <summary>The footprint the classic small icons occupied.</summary>
    public const float SmallLogicalSize = 10f;

    public static float ToHeight(Texture2D icon, float targetHeight) =>
        icon.Height > 0 ? targetHeight / icon.Height : 1f;
}
