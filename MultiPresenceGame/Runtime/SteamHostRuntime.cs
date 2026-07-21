using System.Diagnostics;

namespace MultiPresenceGame.Runtime;

internal static class SteamHostRuntime
{
    private static readonly string SteamAppIdPath = Path.Combine(
        AppContext.BaseDirectory,
        "steam_appid.txt");
    private static int _started;

    public static void WriteSteamAppId(string appId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(appId);
        File.WriteAllText(SteamAppIdPath, appId);
    }

    public static void ClearSteamAppId()
    {
        if (File.Exists(SteamAppIdPath))
        {
            File.Delete(SteamAppIdPath);
        }
    }

    public static void Start(Func<Task> runAsync)
    {
        ArgumentNullException.ThrowIfNull(runAsync);
        if (Interlocked.Exchange(ref _started, 1) != 0)
        {
            return;
        }

        _ = ObserveAsync(runAsync);
    }

    public static bool IsProcessRunning(string processName)
    {
        Process[] processes = [];
        try
        {
            processes = Process.GetProcessesByName(processName);
            return processes.Any(static process =>
            {
                try
                {
                    return !process.HasExited;
                }
                catch
                {
                    return false;
                }
            });
        }
        finally
        {
            foreach (var process in processes)
            {
                process.Dispose();
            }
        }
    }

    private static async Task ObserveAsync(Func<Task> runAsync)
    {
        try
        {
            await runAsync().ConfigureAwait(false);
        }
        catch (Exception exception) when (exception is not OutOfMemoryException and not StackOverflowException)
        {
            try
            {
                var logDirectory = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "MultiPresence",
                    "Logs");
                Directory.CreateDirectory(logDirectory);
                await File.AppendAllTextAsync(
                    Path.Combine(logDirectory, "steam-host.log"),
                    $"{DateTimeOffset.Now:O} {exception}{Environment.NewLine}").ConfigureAwait(false);
            }
            catch
            {
                // The host must still terminate if logging is unavailable.
            }

            Environment.ExitCode = 1;
            Application.Exit();
        }
    }
}
