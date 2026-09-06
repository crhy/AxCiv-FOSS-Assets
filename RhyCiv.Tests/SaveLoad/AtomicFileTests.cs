using System.Text;
using RhyCiv.Engine.SaveLoad;

namespace RhyCiv.Tests.SaveLoad;

/// <summary>
/// Saving used to truncate the destination before writing, so a serialiser that
/// threw part way through replaced the player's save with a fragment. These cover
/// the guarantee that replaced it: a failed write leaves the previous file exactly
/// as it was.
/// </summary>
public class AtomicFileTests : IDisposable
{
    private readonly string _directory =
        Directory.CreateTempSubdirectory("rhyciv-atomic-").FullName;

    private string Path(string name) => System.IO.Path.Combine(_directory, name);

    public void Dispose() => Directory.Delete(_directory, recursive: true);

    [Fact]
    public void Write_CreatesTheFile()
    {
        var path = Path("new.sav");

        AtomicFile.Write(path, stream => stream.Write(Encoding.UTF8.GetBytes("finished")));

        Assert.Equal("finished", File.ReadAllText(path));
    }

    [Fact]
    public void Write_ReplacesAnExistingFile()
    {
        var path = Path("existing.sav");
        File.WriteAllText(path, "old");

        AtomicFile.Write(path, stream => stream.Write(Encoding.UTF8.GetBytes("new")));

        Assert.Equal("new", File.ReadAllText(path));
    }

    [Fact]
    public void Write_LeavesTheExistingFileIntact_WhenWritingThrows()
    {
        var path = Path("precious.sav");
        File.WriteAllText(path, "a whole game");

        Assert.Throws<InvalidOperationException>(() =>
            AtomicFile.Write(path, stream =>
            {
                // Write something first: this is the case that used to destroy the
                // save, because the destination had already been truncated.
                stream.Write(Encoding.UTF8.GetBytes("partial"));
                throw new InvalidOperationException("serialiser failed");
            }));

        Assert.Equal("a whole game", File.ReadAllText(path));
    }

    [Fact]
    public void Write_LeavesNoFileBehind_WhenWritingThrowsAndThereWasNoOriginal()
    {
        var path = Path("never-written.sav");

        Assert.Throws<InvalidOperationException>(() =>
            AtomicFile.Write(path, _ => throw new InvalidOperationException("serialiser failed")));

        Assert.False(File.Exists(path));
        Assert.Empty(Directory.GetFiles(_directory));
    }
}
