using MultiPresence.Infrastructure;
using MultiPresence.Steam;
using System.Diagnostics;
using System.Globalization;

namespace MultiPresence.Runtime
{
    internal static class SteamPresenceLauncher
    {
        private static readonly SemaphoreSlim launchGate = new SemaphoreSlim(1, 1);
        private static readonly object sync = new object();
        private static CancellationTokenSource? activeCancellation;
        private static TaskCompletionSource? activeCompletion;

        public static void Stop(bool waitForExit = false)
        {
            Task? completion;
            lock (sync)
            {
                activeCancellation?.Cancel();
                completion = activeCompletion?.Task;
            }
            if (waitForExit)
            {
                completion?.Wait(TimeSpan.FromSeconds(12));
            }
        }

        public static async Task LaunchAsync(string processName, uint steamAppId)
        {
            if (!await launchGate.WaitAsync(0).ConfigureAwait(false))
            {
                return;
            }

            using var cancellation = new CancellationTokenSource();
            var completion = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            Process? host = null;
            EventWaitHandle? stopEvent = null;
            string? sessionDirectory = null;
            lock (sync)
            {
                activeCancellation = cancellation;
                activeCompletion = completion;
            }

            try
            {
                if (!SteamHostRuntime.Supports(steamAppId, processName))
                {
                    throw new ArgumentException("The requested Steam integration is not supported.");
                }

                using var game = FindGame(processName);
                if (game == null)
                {
                    return;
                }

                using var parent = Process.GetCurrentProcess();
                var eventName = "Local\\MultiPresence.Steam." + Guid.NewGuid().ToString("N");
                stopEvent = new EventWaitHandle(false, EventResetMode.ManualReset, eventName);
                sessionDirectory = Path.Combine(Path.GetTempPath(), "MultiPresence", "Steam", Guid.NewGuid().ToString("N"));
                Directory.CreateDirectory(sessionDirectory);
                await File.WriteAllTextAsync(Path.Combine(sessionDirectory, "steam_appid.txt"),
                    steamAppId.ToString(CultureInfo.InvariantCulture), cancellation.Token).ConfigureAwait(false);

                var startInfo = CreateStartInfo(steamAppId, parent.Id, parent.StartTime.ToUniversalTime().Ticks,
                    game.Id, game.StartTime.ToUniversalTime().Ticks, eventName, sessionDirectory);
                cancellation.Token.ThrowIfCancellationRequested();
                host = Process.Start(startInfo) ?? throw new InvalidOperationException("The Steam presence host could not be started.");
                AppLog.Information("Built-in Steam presence started.", new { Process = processName, SteamAppId = steamAppId, HostPid = host.Id });

                while (!host.HasExited && !game.HasExited)
                {
                    await Task.Delay(1000, cancellation.Token).ConfigureAwait(false);
                }

                if (host.HasExited && !game.HasExited)
                {
                    AppLog.Warning("The Steam presence host stopped while the game was running.",
                        data: new { Process = processName, ExitCode = host.ExitCode });
                    await Task.Delay(TimeSpan.FromSeconds(15), cancellation.Token).ConfigureAwait(false);
                }
            }
            catch (OperationCanceledException) when (cancellation.IsCancellationRequested)
            {
            }
            catch (Exception exception) when (exception is not OutOfMemoryException and not StackOverflowException and not AccessViolationException)
            {
                AppLog.Warning("Could not run the built-in Steam presence.", exception);
            }
            finally
            {
                stopEvent?.Set();
                await StopHostAsync(host).ConfigureAwait(false);
                host?.Dispose();
                stopEvent?.Dispose();
                DeleteSessionDirectory(sessionDirectory);
                lock (sync)
                {
                    activeCancellation = null;
                    activeCompletion = null;
                }
                launchGate.Release();
                completion.TrySetResult();
                PresenceRuntime.RequestDetection();
            }
        }

        internal static ProcessStartInfo CreateStartInfo(uint steamAppId, int parentId, long parentStart,
            int gameId, long gameStart, string eventName, string sessionDirectory)
        {
            var executable = Environment.ProcessPath ?? throw new InvalidOperationException("The executable path is unavailable.");
            var startInfo = new ProcessStartInfo()
            {
                FileName = executable,
                WorkingDirectory = sessionDirectory,
                UseShellExecute = false,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden
            };
            if (Path.GetFileNameWithoutExtension(executable).Equals("dotnet", StringComparison.OrdinalIgnoreCase))
            {
                startInfo.ArgumentList.Add(Path.Combine(AppContext.BaseDirectory, "MultiPresence.dll"));
            }

            startInfo.ArgumentList.Add("--steam-host");
            startInfo.ArgumentList.Add(steamAppId.ToString(CultureInfo.InvariantCulture));
            startInfo.ArgumentList.Add(parentId.ToString(CultureInfo.InvariantCulture));
            startInfo.ArgumentList.Add(parentStart.ToString(CultureInfo.InvariantCulture));
            startInfo.ArgumentList.Add(gameId.ToString(CultureInfo.InvariantCulture));
            startInfo.ArgumentList.Add(gameStart.ToString(CultureInfo.InvariantCulture));
            startInfo.ArgumentList.Add(eventName);
            startInfo.Environment["SteamAppId"] = steamAppId.ToString(CultureInfo.InvariantCulture);
            startInfo.Environment["SteamGameId"] = steamAppId.ToString(CultureInfo.InvariantCulture);
            return startInfo;
        }

        private static Process? FindGame(string processName)
        {
            Process? selected = null;
            var processes = Process.GetProcessesByName(processName);
            try
            {
                foreach (var process in processes)
                {
                    if (!process.HasExited)
                    {
                        selected = process;
                        break;
                    }
                }
                return selected;
            }
            finally
            {
                foreach (var process in processes)
                {
                    if (process != selected)
                    {
                        process.Dispose();
                    }
                }
            }
        }

        private static async Task StopHostAsync(Process? host)
        {
            if (host == null)
            {
                return;
            }

            try
            {
                try
                {
                    await host.WaitForExitAsync().WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
                }
                catch (TimeoutException)
                {
                    host.Kill();
                    await host.WaitForExitAsync().WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
                }
            }
            catch (Exception exception) when (exception is InvalidOperationException or System.ComponentModel.Win32Exception or TimeoutException)
            {
                AppLog.Warning("Could not stop the Steam presence host.", exception);
            }
        }

        private static void DeleteSessionDirectory(string? directory)
        {
            if (directory == null)
            {
                return;
            }

            try
            {
                File.Delete(Path.Combine(directory, "steam_appid.txt"));
                Directory.Delete(directory);
            }
            catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
            {
                AppLog.Warning("Could not remove the temporary Steam presence directory.", exception);
            }
        }
    }
}
