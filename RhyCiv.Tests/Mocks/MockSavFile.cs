using RhyCiv.Engine.IO;
using RhyCiv.Engine.SaveLoad.SavFile;
using Model.Core;
using Model.Core.GameRules;

namespace RhyCiv.Tests.Mocks;

internal class MockSavFile : SavFileBase
{
    public override IGame LoadGame(byte[] fileData, Ruleset activeRuleSet, Rules rules)
    {
        return new MockGame();
    }
}
