using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Civ2engine;

namespace RaylibUI
{
    class Program
    {
        static int Main(string[] args)
        {
            // A hard crash used to leave nothing behind but a window that vanished,
            // which is why the reports we get name the action and not the line. Write
            // the stack trace somewhere the player can find it and say where it went.
            AppDomain.CurrentDomain.UnhandledException += (_, e) =>
                Report(e.ExceptionObject as Exception, "unhandled exception");
            TaskScheduler.UnobservedTaskException += (_, e) =>
                Report(e.Exception, "unobserved task exception");

            try
            {
                var main = new Main();

                main.RunLoop();
                return 0;
            }
            catch (Exception ex)
            {
                Report(ex, "crash");
                return 1;
            }
        }

        private static void Report(Exception? exception, string kind)
        {
            if (exception == null)
            {
                return;
            }

            var report = new StringBuilder()
                .AppendLine($"rhYciv {kind} at {DateTime.Now:yyyy-MM-dd HH:mm:ss}")
                .AppendLine(exception.ToString())
                .ToString();

            // Always to stderr, so a terminal run shows it even if the file cannot
            // be written.
            Console.Error.WriteLine(report);

            try
            {
                var path = Path.Combine(Settings.CrashLogFolder,
                    $"crash-{DateTime.Now:yyyyMMdd-HHmmss}.log");
                File.WriteAllText(path, report);
                Console.Error.WriteLine($"rhYciv: crash report written to {path}");
            }
            catch (Exception writeFailure)
            {
                Console.Error.WriteLine($"rhYciv: could not write a crash report: {writeFailure.Message}");
            }
        }
    }
}
