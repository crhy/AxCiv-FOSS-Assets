using System;
using System.IO;

namespace RhyCiv.Engine.SaveLoad;

/// <summary>
/// Writes a file by building it beside the target and moving it into place only
/// once it is complete.
/// </summary>
/// <remarks>
/// Saving used to open the destination with <see cref="FileMode.Truncate"/>,
/// emptying an existing save before writing a byte of the new one. Anything that
/// threw part way through serialisation therefore replaced the player's save with
/// a fragment -- which is exactly what happened when a barbarian city made the
/// save serializer throw: a 200-byte stub where a finished game had been.
///
/// A save that fails must leave the save it was replacing untouched.
/// </remarks>
public static class AtomicFile
{
    /// <summary>Extension given to the partial file while it is being written.</summary>
    private const string PendingSuffix = ".writing";

    /// <summary>
    /// Runs <paramref name="write"/> against a temporary file beside
    /// <paramref name="path"/>, then replaces <paramref name="path"/> with it.
    /// If <paramref name="write"/> throws, the partial file is removed and the
    /// original is left as it was; the exception is rethrown.
    /// </summary>
    public static void Write(string path, Action<Stream> write)
    {
        var directory = Path.GetDirectoryName(path);
        var pending = Path.Combine(
            string.IsNullOrEmpty(directory) ? "." : directory,
            Path.GetFileName(path) + PendingSuffix);

        try
        {
            using (var file = File.Create(pending))
            {
                write(file);
            }

            File.Move(pending, path, overwrite: true);
        }
        catch
        {
            try
            {
                File.Delete(pending);
            }
            catch
            {
                // Nothing useful to do if the partial file cannot be removed. What
                // matters is the file it was going to replace, which is untouched.
            }

            throw;
        }
    }
}
