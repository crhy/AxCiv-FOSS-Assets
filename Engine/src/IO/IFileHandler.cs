using System.Collections.Generic;

namespace RhyCiv.Engine;

public interface IFileHandler
{
    void ProcessSection(string section, List<string>? contents);
}