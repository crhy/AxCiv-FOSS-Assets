using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Model.Core.GameRules;

namespace Civ2engine.IO
{
    public static class Labels
    {
        private static readonly Lock Lock = new();

        public static string Ok { get; set; } = "Ok";
        public static string Cancel { get; set; } = "Cancel";
        public static string Help { get; set; } = "Help";
        public static string Custom { get; set; } = "Custom";

        public static string[] Items { get; set; } = [];

        public static string For(LabelIndex index)
        {
            if (Items == null)
            {
                throw new InvalidOperationException($"Labels.Items is null when trying to access index {index}. Current path: {_currentPath}");
            }
            return Items[(int)index];
        }
        
        public static string For(LabelIndex index, params string[] strings)
        {
            var label = Items[(int)index];
            if (label == null)
            {
                return "???" + index + "???";
            }
            for (int i = 0; i < strings.Length; i++)
            {
                var rep = "%STRING" + i;
                if (label.Contains(rep))
                {
                    label = label.Replace(rep, strings[i] ?? "null");
                }
            }

            return label.Split("|")[0];
        }

        private static string _currentPath = "";
        
        public static void UpdateLabels(Ruleset? rules)
        {
            var labelPath = rules != null ? Utils.GetFilePath("labels.txt", rules.Paths) : Utils.GetFilePath("labels.txt");
            if (string.IsNullOrWhiteSpace(labelPath))
            {
                UseBuiltInLabels();
                return;
            }

            lock (Lock)
            {
                if (labelPath == _currentPath) return;

                _currentPath = labelPath;
                TextFileParser.ParseFile(labelPath, new LabelLoader());
            }
        }

        private static void UseBuiltInLabels()
        {
            const string builtInPath = "<rhYciv built-in labels>";
            lock (Lock)
            {
                if (_currentPath == builtInPath) return;

                var indexes = Enum.GetValues<LabelIndex>();
                var maximum = indexes.Max(index => (int)index);
                var labels = Enumerable.Range(0, maximum + 1)
                    .Select(index => index == 0 ? string.Empty : $"Label {index}")
                    .ToArray();

                foreach (var index in indexes)
                {
                    labels[(int)index] = Humanize(index.ToString());
                }

                labels[(int)LabelIndex.BC] = "BC";
                labels[(int)LabelIndex.AD] = "AD";
                labels[(int)LabelIndex.OK] = Ok;
                labels[(int)LabelIndex.Cancel] = Cancel;
                labels[(int)LabelIndex.Help] = Help;
                labels[(int)LabelIndex.BronzeAgeMonolith] = "Bronze Age";
                labels[(int)LabelIndex.ClassicalForum] = "Classical";
                labels[(int)LabelIndex.FarEastPavilion] = "East Asian";
                labels[(int)LabelIndex.MedievalCastle] = "Medieval";

                Items = labels;
                _currentPath = builtInPath;
            }
        }

        private static string Humanize(string value)
        {
            value = value.Replace("STRING0", "%STRING0", StringComparison.OrdinalIgnoreCase);
            value = Regex.Replace(value, "([a-z0-9])([A-Z])", "$1 $2");
            value = Regex.Replace(value, "([A-Za-z])([0-9])", "$1 $2");
            return value.Replace('_', ' ').Trim();
        }
    }
}
