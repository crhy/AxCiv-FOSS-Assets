using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using Model.Utils;

namespace Civ2engine
{
    public class Settings
    {
        private static string SettingsFilePath => Path.Combine(ApplicationDataFolder, SettingsFileName);
        
        private static string ApplicationDataFolder => Path.Combine(GetLocalAppDataFolder(), "AxxCiv");

        /// <summary>
        /// Writable per-user storage for standalone saves. This deliberately
        /// avoids writing beside the bundled ruleset, which is read-only in a
        /// Flatpak installation.
        /// </summary>
        public static string SaveGameFolder
        {
            get
            {
                var path = Path.Combine(ApplicationDataFolder, "Saves");
                Directory.CreateDirectory(path);
                return path;
            }
        }
        
        private const string SettingsFileName = "appsettings.json";

        // Game settings from App.config
        public static string Civ2Path { get; private set; } = string.Empty;
        
        public static string[] SearchPaths { get; internal set; } = BuiltInSearchPaths;

        public static int TextureFilter { get; private set; }
        public static float Brightness { get; private set; } = 1f;
        public static float Saturation { get; private set; } = 1f;
        public static float Gamma { get; private set; } = 1f;

        public static bool LoadConfigSettings()
        {
            if (File.Exists(SettingsFilePath))
            {
                LoadSettings(SettingsFilePath);
                if (HasStandaloneData)
                {
                    SelectStandaloneRootIfNeeded();
                    return true;
                }
                if (!string.IsNullOrWhiteSpace(Civ2Path) && IsValidRoot(Civ2Path))
                {
                    return true;
                }
            }
            var alternativePath = Path.Combine(BasePath, SettingsFileName);

            LoadSettings(alternativePath);

            if (HasStandaloneData)
            {
                SelectStandaloneRootIfNeeded();
                return true;
            }

            return !string.IsNullOrWhiteSpace(Civ2Path) && IsValidRoot(Civ2Path);
        }

        private static void SelectStandaloneRootIfNeeded()
        {
            if (!string.IsNullOrWhiteSpace(Civ2Path) && IsValidRoot(Civ2Path)) return;

            Civ2Path = BuiltInSearchPaths.First(path =>
                FileUtilities.GetFile(path, RulesFile) != null && FileUtilities.GetFile(path, "game.txt") != null);
        }

        public static string BasePath => AppDomain.CurrentDomain.BaseDirectory;

        private static void LoadSettings(string? settingsFilePath)
        {
            if (!File.Exists(settingsFilePath)) return;

            var contents = File.ReadAllText(settingsFilePath, Encoding.UTF8);

            var settingsDoc = JsonDocument.Parse(contents);

            var root = settingsDoc.RootElement;

            if (root.TryGetProperty(nameof(Civ2Path), out var civ2PathElement))
            {
                var civ2Path = civ2PathElement.GetString();
                if (IsValidRoot(civ2Path))
                {
                    Civ2Path = civ2Path!;
                }
            }

            if (root.TryGetProperty(nameof(SearchPaths), out var searchPathsElement))
            {
                var searchPaths = BuiltInSearchPaths.Concat(searchPathsElement.EnumerateArray()
                        .Select(e => e.GetString()).Where(IsValidRoot).OfType<string>())
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToArray();
                if (!string.IsNullOrWhiteSpace(Civ2Path))
                {
                    SearchPaths = !searchPaths.Contains(Civ2Path, StringComparer.OrdinalIgnoreCase)
                        ? BuiltInSearchPaths.Concat([Civ2Path]).Concat(searchPaths)
                            .Distinct(StringComparer.OrdinalIgnoreCase).ToArray()
                        : searchPaths;
                }
                else if(searchPaths.Length > 0)
                {
                    Civ2Path = searchPaths[0];
                    SearchPaths = searchPaths;
                }
                
            }else if (!string.IsNullOrWhiteSpace(Civ2Path))
            {
                SearchPaths = [..BuiltInSearchPaths, Civ2Path];
            }

            TextureFilter = root.TryGetProperty(nameof(TextureFilter), out var textureFilter) ? textureFilter.GetInt32() : 0;
            Brightness = ReadCorrection(root, nameof(Brightness), 1f, 0.5f, 1.5f);
            Saturation = ReadCorrection(root, nameof(Saturation), 1f, 0f, 2f);
            Gamma = ReadCorrection(root, nameof(Gamma), 1f, 0.5f, 2f);
        }

        private static float ReadCorrection(JsonElement root, string property, float fallback, float minimum, float maximum) =>
            root.TryGetProperty(property, out var element) && element.TryGetSingle(out var value)
                ? Math.Clamp(value, minimum, maximum)
                : fallback;

        public static void SetColorCorrection(float brightness, float saturation, float gamma)
        {
            Brightness = Math.Clamp(brightness, 0.5f, 1.5f);
            Saturation = Math.Clamp(saturation, 0f, 2f);
            Gamma = Math.Clamp(gamma, 0.5f, 2f);
            Save();
        }

        public static bool IsValidRoot(string? civ2Path)
        {
            try
            {
                return !string.IsNullOrWhiteSpace(civ2Path) && Directory.Exists(civ2Path) && FileUtilities.GetFile(civ2Path, RulesFile) != null;
            }
            catch
            {
                return false;
            }
        }

        private const string RulesFile = "rules.txt";

        public static bool AddPath(string path)
        {
            if (!IsValidRoot(path))
            {
                var dir = Path.GetDirectoryName(path);
                if (dir is null || !IsValidRoot(dir)) return false;
                path = dir;
            }

            if (string.IsNullOrWhiteSpace(Civ2Path) || !IsValidRoot(Civ2Path))
            {
                Civ2Path = path;
                SearchPaths = [..BuiltInSearchPaths, path];
            }
            else
            {
                SearchPaths = SearchPaths.Append(path).ToArray();
            }
            Save();// This overwrites the appsettings.
            return true;
        }

        private static string[] BuiltInSearchPaths =>
        [
            Path.Combine(BasePath, "FOSSart", "Standalone"),
            Path.Combine(BasePath, "RaylibUI", "FOSSart", "Standalone"),
            Path.Combine(BasePath, "FOSSart"),
            Path.Combine(BasePath, "RaylibUI", "FOSSart"),
            BasePath
        ];

        private static bool HasStandaloneData => BuiltInSearchPaths.Any(path =>
            FileUtilities.GetFile(path, RulesFile) != null && FileUtilities.GetFile(path, "game.txt") != null);

        public static void Save()
        {
            if (!Directory.Exists(ApplicationDataFolder))
            {
                Directory.CreateDirectory(ApplicationDataFolder);
            }
            using var writer = new Utf8JsonWriter(File.OpenWrite(SettingsFilePath));
            writer.WriteStartObject();
            writer.WriteString(nameof(Civ2Path),Civ2Path);
            writer.WriteStartArray(nameof(SearchPaths));
            foreach (var searchPath in SearchPaths)
            {
                writer.WriteStringValue(searchPath);
            }
            writer.WriteEndArray();
            writer.WriteNumber(nameof(TextureFilter), TextureFilter);
            writer.WriteNumber(nameof(Brightness), Brightness);
            writer.WriteNumber(nameof(Saturation), Saturation);
            writer.WriteNumber(nameof(Gamma), Gamma);
            writer.WriteEndObject();
            writer.Flush();
        }
        
        private static string GetLocalAppDataFolder() {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return Environment.GetEnvironmentVariable("LOCALAPPDATA") ?? BasePath;
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return Environment.GetEnvironmentVariable("XDG_DATA_HOME")
                    ?? Path.Combine(Environment.GetEnvironmentVariable("HOME") ?? BasePath, ".local", "share");
            } 
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return Path.Combine(Environment.GetEnvironmentVariable("HOME") ?? BasePath, "Library", "Application Support");
            }
            throw new NotImplementedException("Unknown OS Platform");
        }
    }
}
