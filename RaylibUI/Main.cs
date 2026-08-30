using System.Numerics;
using Civ2engine;
using Civ2engine.IO;
using Civ2engine.MapObjects;
using Model;
using Model.Core.Mapping;
using RaylibUI.Initialization;
using Raylib_CSharp;
using Raylib_CSharp.Windowing;
using Raylib_CSharp.Interact;
using Raylib_CSharp.Audio;
using Raylib_CSharp.Rendering;
using Raylib_CSharp.Colors;
using Raylib_CSharp.Images;
using Raylib_CSharp.Shaders;
using Raylib_CSharp.Textures;
using Raylib_CSharp.Transformations;

namespace RaylibUI
{
    public partial class Main : IMain
    {

        private IScreen _activeScreen = null!;
        private bool _shouldClose;


        private Sound Soundman = null!;
        private IUserInterface _activeInterface = null!;
        private RenderTexture2D _colorTarget;
        private string? _colorCorrectionMessage;
        private double _colorCorrectionMessageUntil;

        public Main()
        {
            var hasCivDir = Settings.LoadConfigSettings();

            //========= RAYLIB WINDOW SETTINGS
            Raylib.SetConfigFlags(ConfigFlags.Msaa4XHint| ConfigFlags.VSyncHint |
                                  ConfigFlags.ResizableWindow);
            Window.Init(1600, 900, "rhYciv");
            var appIcon = Image.Load(AssetPaths.Resolve("FOSSart/rhyciv-app-icon.png"));
            Window.SetIcon(appIcon);
            appIcon.Unload();
            //Raylib.SetTargetFPS(60);
            AudioDevice.Init();

            Input.SetExitKey(KeyboardKey.F12);

            Shaders.Load();
            Helpers.LoadFonts();

            if (hasCivDir)
            {
                _activeScreen = SetupMainScreen();
            }
            else
            {
                _activeScreen = new GameFileLocatorScreen(this,() =>
                {
                    hasCivDir = true;
                    _activeScreen = SetupMainScreen();
                }, ShutdownApp);
            }

            //============ LOAD SOUNDS

            //prep this for a loop( should split that function out between loops and non loops)

        }

        private const float pulseTime = 30;

        private List<TimedEvent> _events = new List<TimedEvent>();

        public void RunLoop()
        {
            var counter = pulseTime;
            var pulse = false;

            while (!Window.ShouldClose() && !_shouldClose)
            {
                var frameTime = Time.GetFrameTime();

                for (int i = 0; i < _events.Count; i++)
                {
                    _events[i].Remaining -= frameTime;
                    if (_events[i].Remaining < 0)
                    {
                        _events[i].Action();
                        _events.RemoveAt(i);
                        i--;
                    }
                }

                HandleColorCorrectionKeys();
                var screenWidth = Window.GetScreenWidth();
                var screenHeight = Window.GetScreenHeight();
                if (ColorCorrectionActive())
                {
                    EnsureColorTarget(screenWidth, screenHeight);
                    Graphics.BeginTextureMode(_colorTarget);
                    DrawScene(pulse, screenHeight);
                    Graphics.EndTextureMode();

                    Graphics.BeginDrawing();
                    Graphics.ClearBackground(Color.Black);
                    Graphics.BeginShaderMode(Shaders.ColorCorrection);
                    Graphics.DrawTextureRec(_colorTarget.Texture,
                        new Rectangle(0, 0, screenWidth, -screenHeight), Vector2.Zero, Color.White);
                    Graphics.EndShaderMode();
                    DrawColorCorrectionMessage();
                    Graphics.EndDrawing();
                }
                else
                {
                    Graphics.BeginDrawing();
                    DrawScene(pulse, screenHeight);
                    DrawColorCorrectionMessage();
                    Graphics.EndDrawing();
                }

                if (counter++ >= 30)
                {
                    pulse = !pulse;
                    counter = 0;
                }
            }

            ShutdownApp();
        }

        private void DrawScene(bool pulse, int screenHeight)
        {
            _activeScreen.Draw(pulse);
            Graphics.DrawText($"{Time.GetFPS()} FPS", 5, screenHeight - 20, 20, Color.Magenta);
        }

        private static bool ColorCorrectionActive() =>
            Math.Abs(Settings.Brightness - 1f) > 0.001f ||
            Math.Abs(Settings.Saturation - 1f) > 0.001f ||
            Math.Abs(Settings.Gamma - 1f) > 0.001f;

        private void EnsureColorTarget(int width, int height)
        {
            if (_colorTarget.IsValid() &&
                _colorTarget.Texture.Width == width && _colorTarget.Texture.Height == height)
            {
                return;
            }

            if (_colorTarget.IsValid())
            {
                _colorTarget.Unload();
            }
            _colorTarget = RenderTexture2D.Load(width, height);
        }

        private void HandleColorCorrectionKeys()
        {
            var modifiers = (Input.IsKeyDown(KeyboardKey.LeftControl) || Input.IsKeyDown(KeyboardKey.RightControl)) &&
                            (Input.IsKeyDown(KeyboardKey.LeftAlt) || Input.IsKeyDown(KeyboardKey.RightAlt));
            if (!modifiers)
            {
                return;
            }

            var brightness = Settings.Brightness;
            var saturation = Settings.Saturation;
            var gamma = Settings.Gamma;
            if (Input.IsKeyPressed(KeyboardKey.Up)) brightness += 0.05f;
            else if (Input.IsKeyPressed(KeyboardKey.Down)) brightness -= 0.05f;
            else if (Input.IsKeyPressed(KeyboardKey.Right)) saturation += 0.1f;
            else if (Input.IsKeyPressed(KeyboardKey.Left)) saturation -= 0.1f;
            else if (Input.IsKeyPressed(KeyboardKey.PageUp)) gamma += 0.05f;
            else if (Input.IsKeyPressed(KeyboardKey.PageDown)) gamma -= 0.05f;
            else if (Input.IsKeyPressed(KeyboardKey.Home)) brightness = saturation = gamma = 1f;
            else return;

            Settings.SetColorCorrection(brightness, saturation, gamma);
            Shaders.SetColorCorrection(Settings.Brightness, Settings.Saturation, Settings.Gamma);
            _colorCorrectionMessage =
                $"Brightness {Settings.Brightness:0.00}  Saturation {Settings.Saturation:0.00}  Gamma {Settings.Gamma:0.00}";
            _colorCorrectionMessageUntil = Time.GetTime() + 2.5;
        }

        private void DrawColorCorrectionMessage()
        {
            if (_colorCorrectionMessage == null || Time.GetTime() > _colorCorrectionMessageUntil)
            {
                return;
            }

            var width = _colorCorrectionMessage.Length * 10 + 20;
            Graphics.DrawRectangle(8, 8, width, 28, new Color(0, 0, 0, 210));
            Graphics.DrawText(_colorCorrectionMessage, 18, 13, 18, Color.White);
        }

        private MainMenu SetupMainScreen()
        {
            //Helpers.LoadFonts();
            Interfaces = Helpers.LoadInterfaces(this);
            AllRuleSets =  Interfaces.SelectMany((userInterface, idx) =>
                {
                    userInterface.InterfaceIndex = idx;
                    var sets = userInterface.FindRuleSets(Settings.SearchPaths);

                    foreach (var ruleset in sets)
                    {
                        ruleset.InterfaceIndex = idx;
                    }
                    return sets;
                })
                .ToArray();
            ActiveInterface = Helpers.GetInterface(Settings.Civ2Path, Interfaces, AllRuleSets);
            return new MainMenu(this,() => _shouldClose= true, StartGame, Soundman);
        }



        public IUserInterface ActiveInterface
        {
            get => _activeInterface;
            private set
            {
                if(value == _activeInterface) return;

                _activeInterface = value;

                ActiveRuleSet ??= AllRuleSets.First(r => r.InterfaceIndex == _activeInterface.InterfaceIndex);

                _activeInterface.Initialize();
                TextureCache.Clear();
                Labels.UpdateLabels(ActiveRuleSet);
                ImageUtils.SetLook(_activeInterface);
                Soundman?.Dispose();
                Soundman = new Sound(_activeInterface.Title);
                _activeScreen?.InterfaceChanged(Soundman);
            }
        }

        public IList<IUserInterface> Interfaces { get; set; } = [];

        void ShutdownApp()
        {
            if (_colorTarget.IsValid())
            {
                _colorTarget.Unload();
            }
            Shaders.Unload();
            Soundman?.Dispose();
            Window.Close();
            AudioDevice.Close();
        }

        public void ReloadMain()
        {
            ActiveRuleSet = AllRuleSets.First(r => r.InterfaceIndex == _activeInterface.InterfaceIndex);
            TextureCache.Clear();
            ImageUtils.SetLook(_activeInterface);
            _activeScreen = new MainMenu(this,() => _shouldClose= true, StartGame, Soundman);
        }

        public void Schedule(string eventName, TimeSpan delay, Action action)
        {
            for (int i = 0; i < _events.Count; i++)
            {
                if (_events[i].Name == eventName)
                {
                    _events[i] = new TimedEvent(name: eventName, remaining: (float)delay.TotalSeconds, action: action);
                    return;
                }
            }
            _events.Add(new TimedEvent(name: eventName, remaining: (float)delay.TotalSeconds, action: action));
        }

        public void ClearSchedule(string eventName)
        {
            _events.RemoveAll(e => e.Name == eventName);
        }
    }

    internal class TimedEvent(string name, float remaining, Action action)
    {
        public float Remaining { get; set; } = remaining;
        public string Name { get; set; } = name;
        public Action Action { get; set; } = action;
    }
}
