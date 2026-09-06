using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using Model.Utils;

namespace RhyCiv.Engine
{
    public class Settings
    {
        private static string SettingsFilePath => Path.Combine(ApplicationDataFolder, SettingsFileName);

        /// <summary>
        /// The directory this build reads and writes. Before the project was
        /// deforked the same data lived under <see cref="LegacyDataFolderName"/>;
        /// <see cref="MigrateLegacyDataFolder"/> moves it across once.
        /// </summary>
        private const string DataFolderName = "rhYciv";

        /// <summary>Pre-defork name of <see cref="DataFolderName"/>.</summary>
        private const string LegacyDataFolderName = "AxxCiv";

        private static string ApplicationDataFolder => Path.Combine(GetLocalAppDataFolder(), DataFolderName);

        private static string LegacyApplicationDataFolder =>
            Path.Combine(GetLocalAppDataFolder(), LegacyDataFolderName);

        /// <summary>
        /// Moves saves, logs and settings written by a pre-defork build into the
        /// current data directory, once, on first launch. It runs before anything
        /// reads the folder and is deliberately best-effort: a player whose old
        /// directory cannot be moved gets a fresh one rather than a failed start.
        /// The legacy directory is left in place so an older build still runs.
        /// </summary>
        public static void MigrateLegacyDataFolder()
        {
            try
            {
                if (Directory.Exists(ApplicationDataFolder)) return;
                if (!Directory.Exists(LegacyApplicationDataFolder)) return;

                CopyDirectory(LegacyApplicationDataFolder, ApplicationDataFolder);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(
                    $"Could not migrate the previous data directory '{LegacyApplicationDataFolder}': {e.Message}");
            }
        }

        private static void CopyDirectory(string source, string destination)
        {
            Directory.CreateDirectory(destination);
            foreach (var file in Directory.EnumerateFiles(source))
            {
                File.Copy(file, Path.Combine(destination, Path.GetFileName(file)), overwrite: false);
            }

            foreach (var directory in Directory.EnumerateDirectories(source))
            {
                CopyDirectory(directory, Path.Combine(destination, Path.GetFileName(directory)));
            }
        }

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
        
        /// <summary>
        /// Writable per-user storage for crash reports, beside the saves and for the
        /// same reason: the bundled ruleset directory is read-only under Flatpak.
        /// </summary>
        public static string CrashLogFolder
        {
            get
            {
                var path = Path.Combine(ApplicationDataFolder, "Logs");
                Directory.CreateDirectory(path);
                return path;
            }
        }

        private const string SettingsFileName = "appsettings.json";

        /// <summary>Pre-defork name of the <see cref="GameDataPath"/> settings key.</summary>
        private const string LegacyGameDataPathKey = "Civ2Path";

        // Game settings from App.config
        public static string GameDataPath { get; private set; } = string.Empty;
        
        public static string[] SearchPaths { get; internal set; } = BuiltInSearchPaths;

        public static int TextureFilter { get; private set; }
        public static float Brightness { get; private set; } = 1f;
        public static float Saturation { get; private set; } = 1f;
        public static float Gamma { get; private set; } = 1f;

        public static bool LoadConfigSettings()
        {
            MigrateLegacyDataFolder();

            if (File.Exists(SettingsFilePath))
            {
                LoadSettings(SettingsFilePath);
                if (HasStandaloneData)
                {
                    SelectStandaloneRootIfNeeded();
                    return true;
                }
                if (!string.IsNullOrWhiteSpace(GameDataPath) && IsValidRoot(GameDataPath))
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

            return !string.IsNullOrWhiteSpace(GameDataPath) && IsValidRoot(GameDataPath);
        }

        private static void SelectStandaloneRootIfNeeded()
        {
            if (!string.IsNullOrWhiteSpace(GameDataPath) && IsValidRoot(GameDataPath)) return;

            GameDataPath = BuiltInSearchPaths.First(path =>
                FileUtilities.GetFile(path, RulesFile) != null && FileUtilities.GetFile(path, "game.txt") != null);
        }

        public static string BasePath => AppDomain.CurrentDomain.BaseDirectory;

        private static void LoadSettings(string? settingsFilePath)
        {
            if (!File.Exists(settingsFilePath)) return;

            var contents = File.ReadAllText(settingsFilePath, Encoding.UTF8);

            var settingsDoc = JsonDocument.Parse(contents);

            var root = settingsDoc.RootElement;

            // "Civ2Path" is what this key was called before the defork; settings
            // files written by an older build still use it.
            if (root.TryGetProperty(nameof(GameDataPath), out var gameDataPathElement) ||
                root.TryGetProperty(LegacyGameDataPathKey, out gameDataPathElement))
            {
                var gameDataPath = gameDataPathElement.GetString();
                if (IsValidRoot(gameDataPath))
                {
                    GameDataPath = gameDataPath!;
                }
            }

            if (root.TryGetProperty(nameof(SearchPaths), out var searchPathsElement))
            {
                var searchPaths = BuiltInSearchPaths.Concat(searchPathsElement.EnumerateArray()
                        .Select(e => e.GetString()).Where(IsValidRoot).OfType<string>())
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToArray();
                if (!string.IsNullOrWhiteSpace(GameDataPath))
                {
                    SearchPaths = !searchPaths.Contains(GameDataPath, StringComparer.OrdinalIgnoreCase)
                        ? BuiltInSearchPaths.Concat([GameDataPath]).Concat(searchPaths)
                            .Distinct(StringComparer.OrdinalIgnoreCase).ToArray()
                        : searchPaths;
                }
                else if(searchPaths.Length > 0)
                {
                    GameDataPath = searchPaths[0];
                    SearchPaths = searchPaths;
                }
                
            }else if (!string.IsNullOrWhiteSpace(GameDataPath))
            {
                SearchPaths = [..BuiltInSearchPaths, GameDataPath];
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

        public static bool IsValidRoot(string? gameDataPath)
        {
            try
            {
                return !string.IsNullOrWhiteSpace(gameDataPath) && Directory.Exists(gameDataPath) && FileUtilities.GetFile(gameDataPath, RulesFile) != null;
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

            if (string.IsNullOrWhiteSpace(GameDataPath) || !IsValidRoot(GameDataPath))
            {
                GameDataPath = path;
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
            writer.WriteString(nameof(GameDataPath),GameDataPath);
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
