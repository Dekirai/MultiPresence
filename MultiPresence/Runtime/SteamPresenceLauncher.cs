using MultiPresence.Infrastructure;
using System.Diagnostics;

namespace MultiPresence.Runtime;

internal static class SteamPresenceLauncher
{
    private static readonly SemaphoreSlim LaunchGate = new(1, 1);

    public static async Task LaunchAsync(string processName, uint steamAppId)
    {
        await LaunchGate.WaitAsync().ConfigureAwait(false);
        try
        {
            await FileChecker.EnsureFilesExistAsync().ConfigureAwait(false);

            var assetsDirectory = Path.Combine(AppContext.BaseDirectory, "Assets");
            var hostPath = Path.Combine(assetsDirectory, "MultiPresenceGame.exe");
            var steamLibraryPath = Path.Combine(assetsDirectory, "steam_api64.dll");
            if (!File.Exists(hostPath) || !File.Exists(steamLibraryPath))
            {
                AppLog.Warning("Steam presence host is unavailable after dependency verification.");
                PresenceRuntime.RequestDetection();
                return;
            }

            Directory.CreateDirectory(assetsDirectory);
            await File.WriteAllTextAsync(
                    Path.Combine(assetsDirectory, "steam_appid.txt"),
                    steamAppId.ToString(System.Globalization.CultureInfo.InvariantCulture))
                .ConfigureAwait(false);

            using var host = Process.Start(new ProcessStartInfo
            {
                FileName = hostPath,
                WorkingDirectory = assetsDirectory,
                UseShellExecute = false
            });

            if (host is null)
            {
                throw new InvalidOperationException("The Steam presence host could not be started.");
            }

            AppLog.Information("Steam presence host started.", new { Process = processName, SteamAppId = steamAppId });
        }
        catch (Exception exception) when (exception is not OutOfMemoryException and not StackOverflowException)
        {
            AppLog.Warning("Could not start the Steam presence host.", exception);
            PresenceRuntime.RequestDetection();
            return;
        }
        finally
        {
            LaunchGate.Release();
        }

        while (ProcessMonitor.IsRunning(processName))
        {
            await Task.Delay(TimeSpan.FromSeconds(3)).ConfigureAwait(false);
        }

        PresenceRuntime.RequestDetection();
    }
}
