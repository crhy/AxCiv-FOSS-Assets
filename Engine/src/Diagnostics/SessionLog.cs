using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace RhyCiv.Engine.Diagnostics;

/// <summary>
/// A running record of what the game was doing, written to disk as it happens.
/// </summary>
/// <remarks>
/// The crash handler in Program catches managed exceptions, and that is most of
/// them. It cannot catch the rest: a fault inside raylib or the graphics driver,
/// or a stack overflow, kills the process without .NET running any handler at
/// all. Those crashes leave nothing behind, and a report that says only "it
/// crashed while I was playing" cannot be acted on.
///
/// So this does not try to catch anything. It writes a line per notable action,
/// flushed immediately, and holds the file open for the length of the session.
/// A clean exit deletes it. A file still present at the next launch therefore
/// means the previous session died without shutting down, and it is promoted to
/// a crash report carrying the last thing the game managed to do.
///
/// Everything here is best-effort. Diagnostics must never be the reason a player
/// cannot start the game, so every operation swallows its own failures.
/// </remarks>
public static class SessionLog
{
    private const string ActiveFileName = "session-in-progress.log";

    /// <summary>Lines kept in memory for the crash report; the file keeps them all.</summary>
    private const int RecentLines = 40;

    private static readonly object Gate = new();
    private static readonly Queue<string> Recent = new();
    private static StreamWriter? _writer;

    /// <summary>
    /// Where the record is kept. Overridable so the tests can exercise this
    /// without writing into the player's own log folder; nothing else reassigns it.
    /// </summary>
    internal static Func<string> LogFolder { get; set; } = () => Settings.CrashLogFolder;

    private static string ActivePath => Path.Combine(LogFolder(), ActiveFileName);

    /// <summary>
    /// Opens the session record, and returns the previous session's record if it
    /// was left behind — which means that session crashed without a handler
    /// running. The caller decides how to report it.
    /// </summary>
    public static string? Begin(string version)
    {
        string? previous = null;
        lock (Gate)
        {
            try
            {
                previous = PromotePreviousSession();

                _writer = new StreamWriter(
                    new FileStream(ActivePath, FileMode.Create, FileAccess.Write, FileShare.Read))
                {
                    AutoFlush = true,
                };

                Write($"rhYciv {version}");
                Write($"{RuntimeInformation.OSDescription} ({RuntimeInformation.OSArchitecture})");
                Write($".NET {RuntimeInformation.FrameworkDescription}");
                Write($"flatpak={(Environment.GetEnvironmentVariable("FLATPAK_ID") is { } id ? id : "no")}");
            }
            catch (Exception e)
            {
                Console.Error.WriteLine($"rhYciv: session log unavailable: {e.Message}");
                _writer = null;
            }
        }

        return previous;
    }

    /// <summary>Records one notable action. Cheap enough to call from game code.</summary>
    public static void Record(string message)
    {
        lock (Gate)
        {
            Write(message);
        }
    }

    /// <summary>
    /// Marks the session as having ended properly, so the next launch does not
    /// report it as a crash.
    /// </summary>
    public static void End()
    {
        lock (Gate)
        {
            try
            {
                _writer?.Dispose();
                _writer = null;
                if (File.Exists(ActivePath))
                {
                    File.Delete(ActivePath);
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine($"rhYciv: could not close the session log: {e.Message}");
            }
        }
    }

    /// <summary>
    /// The last few recorded actions, for inclusion in a managed crash report.
    /// </summary>
    public static string RecentActivity()
    {
        lock (Gate)
        {
            return Recent.Count == 0
                ? "(nothing recorded)"
                : string.Join(Environment.NewLine, Recent);
        }
    }

    private static void Write(string message)
    {
        var line = $"{DateTime.Now:HH:mm:ss.fff}  {message}";

        Recent.Enqueue(line);
        while (Recent.Count > RecentLines)
        {
            Recent.Dequeue();
        }

        try
        {
            _writer?.WriteLine(line);
        }
        catch
        {
            // A full or read-only disk must not stop play.
        }
    }

    /// <summary>
    /// Renames a session record left by a previous run into a crash report, and
    /// returns what it held.
    /// </summary>
    private static string? PromotePreviousSession()
    {
        if (!File.Exists(ActivePath))
        {
            return null;
        }

        var contents = File.ReadAllText(ActivePath);
        var report = Path.Combine(LogFolder(),
            $"crash-{DateTime.Now:yyyyMMdd-HHmmss}-no-handler.log");

        var header = new StringBuilder()
            .AppendLine("rhYciv: the previous session ended without shutting down.")
            .AppendLine()
            .AppendLine("No managed exception was recorded, so this was most likely a fault in a")
            .AppendLine("native library or the graphics driver, which terminates the process before")
            .AppendLine("any handler can run. What follows is what the game was doing; the last line")
            .AppendLine("is the last thing it completed.")
            .AppendLine()
            .ToString();

        File.WriteAllText(report, header + contents);
        File.Delete(ActivePath);
        Console.Error.WriteLine($"rhYciv: previous session did not exit cleanly; wrote {report}");
        return report;
    }
}
