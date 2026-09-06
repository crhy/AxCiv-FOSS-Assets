// ReSharper disable InconsistentNaming

using Model.Core;

namespace RhyCiv.Engine.Scripting.ScriptObjects;

public class Leader(Civilization civ)
{
    public string name => civ.LeaderName;
    public string title => civ.LeaderTitle;
    public int gender => civ.LeaderGender;
}
