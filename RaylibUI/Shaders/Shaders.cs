using Civ2engine;
using Raylib_CSharp.Shaders;

namespace RaylibUI;

public static class Shaders
{
    public static Shader Grayscale;
    public static Shader ColorCorrection;
    private static int _brightnessLocation, _saturationLocation, _gammaLocation;

    public static void Load()
    {
        Grayscale = Shader.Load(
            AssetPaths.Resolve("Shaders", "base.vs"),
            AssetPaths.Resolve("Shaders", "grayscale.fs")
        );
        ColorCorrection = Shader.Load(
            AssetPaths.Resolve("Shaders", "base.vs"),
            AssetPaths.Resolve("Shaders", "color-correction.fs")
        );
        _brightnessLocation = ColorCorrection.GetLocation("brightness");
        _saturationLocation = ColorCorrection.GetLocation("saturation");
        _gammaLocation = ColorCorrection.GetLocation("gamma");
        SetColorCorrection(Settings.Brightness, Settings.Saturation, Settings.Gamma);
    }

    public static void SetColorCorrection(float brightness, float saturation, float gamma)
    {
        ColorCorrection.SetValue(_brightnessLocation, brightness, ShaderUniformDataType.Float);
        ColorCorrection.SetValue(_saturationLocation, saturation, ShaderUniformDataType.Float);
        ColorCorrection.SetValue(_gammaLocation, gamma, ShaderUniformDataType.Float);
    }

    public static void Unload()
    {
        Grayscale.Unload();
        ColorCorrection.Unload();
    }
}
