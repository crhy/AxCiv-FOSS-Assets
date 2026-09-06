using System;
using System.Collections.Generic;

namespace RhyCiv.Engine.Events
{
    public class PopupboxEventArgs : EventArgs
    {
        public string BoxName { get; private set; }
        public List<string> ReplaceStrings { get; private set; }

        public PopupboxEventArgs(string boxName, List<string>? replaceStrings = null)
        {
            BoxName = boxName;
            ReplaceStrings = replaceStrings ?? [];
        }
    }
}
