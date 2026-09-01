using Civ2engine;
using Civ2engine.IO;
using Civ2engine.SaveLoad;
using Core.Tests.Mocks;
using Core.Tests.TestFiles;
using Model;
using Model.Controls;
using Model.Core;
using Model.Core.Advances;
using Model.Images;
using Model.ImageSets;
using Model.InterfaceActions;
using Raylib_CSharp.Images;
using Raylib_CSharp.Textures;
using Raylib_CSharp.Transformations;

namespace Core.Tests.SaveLoad;

public class LoadGameTests
{
    private readonly IMain _mockMainApp;
    private readonly MockInterface _mockUi;

    public LoadGameTests()
    {
        // We need to hard code the SearchPaths here for the LoadFrom() method to work properly under test.
        _mockUi = new MockInterface();
        _mockMainApp = _mockUi.MainApp;
        var testFileDirectory = CleanRoomGameFactory.StandaloneDirectory;
        Settings.SearchPaths = [testFileDirectory, testFileDirectory];

        // This is also needed so that the Barbarians civ can be initialised in the GameSerializer.
        // JSON save file loading fails if this isn't pre-populated.
        Labels.UpdateLabels(_mockMainApp.ActiveRuleSet);
    }

    [Fact]
    public void TestLoadFromThrowsExceptionIfPathNotFound()
    {
        // Arrange
        var saveFilePath = TestFileUtils.GetTestFilePath("pathtonowhere.sav");

        // Act
        // Assert
        Assert.Throws<FileNotFoundException>(() => LoadGame.LoadFrom(saveFilePath, _mockMainApp));

    }

    [Fact]
    public void TestLoadJsonGameGivesValue()
    {
        var path = Path.Combine(Path.GetTempPath(), $"rhyciv-clean-room-{Guid.NewGuid():N}.sav");
        File.WriteAllBytes(path, CleanRoomGameFactory.CreateJsonSave());

        try
        {
            LoadGame.LoadFrom(path, _mockMainApp);
            var result = (Game)_mockUi.LoadedGame!;

            Assert.NotNull(result);
            Assert.True(result.Options.Bloodlust);
            Assert.Equal(3, result.AllCivilizations.Count);
            Assert.Equal(PlayerType.Barbarians, result.AllCivilizations[0].PlayerType);
            Assert.Equal(PlayerType.Local, result.AllCivilizations[1].PlayerType);
        }
        finally
        {
            File.Delete(path);
        }
    }
}
