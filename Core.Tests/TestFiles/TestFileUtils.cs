namespace Core.Tests.TestFiles;


internal static class TestFileUtils
{
    internal static string GetTestFileDirectory()
    {
        return CleanRoomGameFactory.StandaloneDirectory;
    }

    internal static string GetTestFilePath(string fileName)
    {
        return Path.Combine(GetTestFileDirectory(), fileName);
    }
}
