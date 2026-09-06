using System;
using System.Collections.Generic;
using System.Linq;
using Model.Core;

namespace RhyCiv.Engine.IO
{
    public class PopupBoxReader(Dictionary<string, PopupBox> boxes) : IFileHandler
    {
        // Read popupbox data from the txt file
        public static Dictionary<string, PopupBox> LoadPopupBoxes(string[] paths, string fileName)
        {
            var boxes = new Dictionary<string, PopupBox>();
            var filePath = Utils.GetFilePath(fileName, paths);
            if (filePath != null)
            {
                TextFileParser.ParseFile(filePath, new PopupBoxReader(boxes: boxes), true);
            }
            return boxes;
        }

        public Dictionary<string, PopupBox> Boxes { get; init; } = boxes;

        public void ProcessSection(string section, List<string>? contents)
        {
            if (contents == null) return;
            
            var popupBox = new PopupBox {Name = section, Checkbox = false};

            var contentHandler = TextHandler;

            var optionsHandler = new Action<string>((line) =>
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    contentHandler = TextHandler;
                }
                else
                {
                    (popupBox.Options ??= new List<string>()).Add(line);
                }
            });

            foreach (var line in contents)
            {
                if (line.StartsWith('@'))
                {
                    var parts = line.Split(['@', '='], 2,
                        StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == 0)
                    {
                        continue;
                    }

                    switch (parts[0])
                    {
                        case "width":
                            popupBox.Width = int.Parse(parts[1]);
                            break;
                        case "title":
                            popupBox.Title = parts[1];
                            break;
                        case "default":
                            popupBox.Default = int.Parse(parts[1]);
                            break;
                        case "x":
                            popupBox.X = int.Parse(parts[1]);
                            break;
                        case "y":
                            popupBox.Y = int.Parse(parts[1]);
                            break;
                        case "options":
                            contentHandler = optionsHandler;
                            break;
                        case "checkbox":
                            popupBox.Checkbox = true;
                            break;
                        case "listbox":
                            popupBox.Listbox = true;
                            popupBox.ListboxLines = parts.Length > 1 ? int.Parse(parts[1]) : 16;
                            break;
                        case "button":
                            (popupBox.Button ??= new List<string>()).Add(parts[1]);
                            break;
                    }
                }

                else
                {
                    contentHandler(line);
                }
            }

            // Only synthesise the default OK button when the section declared no
            // @button lines of its own; otherwise its explicit buttons stand.
            if (popupBox.Button is not { Count: > 0 })
            {
                popupBox.Button = new List<string> { "OK" };
            }

            // Add a Cancel button for choice dialogs that did not list one.
            if (popupBox.Options != null &&
                !popupBox.Button.Contains("Cancel", StringComparer.OrdinalIgnoreCase))
            {
                popupBox.Button.Add("Cancel");
            }


            Boxes[popupBox.Name] = popupBox;
            return;

            void TextHandler(string line)
            {
                if (string.IsNullOrWhiteSpace(line) && popupBox.Text?.Count > 0 && popupBox.Options == null && section != "SCENARIO")   // No options in scenario intro text
                {
                    contentHandler = (item) =>
                    {
                        if (string.IsNullOrWhiteSpace(item)) return;
                        (popupBox.Options ??= new List<string>()).Add(item);
                    };
                    return;
                }

                popupBox.AddText(line);
            }
        }
    }
}
