using MultiPresence.Infrastructure;
using MultiPresence.Steam.Presence;
using Steamworks;
using System.Diagnostics;
using System.Globalization;

namespace MultiPresence.Steam
{
    internal static class SteamHostRuntime
    {
        private static readonly IReadOnlyDictionary<uint, (string ProcessName, Action Start)> Integrations =
            new Dictionary<uint, (string ProcessName, Action Start)>
            {
                { 2344520, ("Diablo IV", D4.DoAction) },
                { 2357570, ("Overwatch", OW.DoAction) },
                { 2510960, ("TemtemSwarm", TTS.DoAction) }
            };

        private static Func<Task>? pollAsync;
        private static Action? cleanup;

        public static bool Supports(uint appId, string processName)
        {
            return Integrations.TryGetValue(appId, out var integration) &&
                string.Equals(integration.ProcessName, processName, StringComparison.OrdinalIgnoreCase);
        }

        public static int Run(string[] args)
        {
            if (!TryParseArguments(args, out var appId, out var parentId, out var parentStarted,
                out var gameId, out var gameStarted))
            {
                AppLog.Warning("Invalid Steam host arguments.");
                return 2;
            }

            bool initialized = false;
            pollAsync = null;
            cleanup = null;
            try
            {
                using var cancellation = EventWaitHandle.OpenExisting(args[6]);
                using var parent = OpenProcess(parentId, parentStarted);
                using var game = OpenProcess(gameId, gameStarted, Integrations[appId].ProcessName);
                if (parent is null || game is null || cancellation.WaitOne(0))
                {
                    return 0;
                }

                if (!SteamAPI.Init())
                {
                    AppLog.Warning("Steam initialization failed.", data: new { SteamAppId = appId });
                    return 1;
                }

                initialized = true;
                if (SteamUtils.GetAppID().m_AppId != appId)
                {
                    AppLog.Warning("Steam initialized with a different application ID.", data: new { SteamAppId = appId });
                    return 1;
                }

                if (!IsRunning(parent) || !IsRunning(game) || cancellation.WaitOne(0))
                {
                    return 0;
                }

                PlaceholderHelper._startTimestamp = DiscordRPC.Timestamps.Now;
                Integrations[appId].Start();
                var poll = pollAsync ?? throw new InvalidOperationException("The Steam presence did not register a polling callback.");
                var timer = Stopwatch.StartNew();
                var nextPoll = TimeSpan.Zero;
                AppLog.Information("Steam presence started.", new { SteamAppId = appId, GameProcessId = gameId });
                while (IsRunning(parent) && IsRunning(game) && !cancellation.WaitOne(0))
                {
                    SteamAPI.RunCallbacks();
                    if (timer.Elapsed >= nextPoll)
                    {
                        poll().GetAwaiter().GetResult();
                        nextPoll = timer.Elapsed + TimeSpan.FromSeconds(3);
                    }

                    if (cancellation.WaitOne(100))
                    {
                        break;
                    }
                }

                return 0;
            }
            catch (Exception exception) when (exception is not OutOfMemoryException and not StackOverflowException and not AccessViolationException)
            {
                AppLog.Warning("Steam presence failed.", exception, new { SteamAppId = appId });
                return 1;
            }
            finally
            {
                try
                {
                    cleanup?.Invoke();
                }
                catch (Exception exception) when (exception is not OutOfMemoryException and not StackOverflowException and not AccessViolationException)
                {
                    AppLog.Warning("Could not release the Steam Discord presence.", exception);
                }

                pollAsync = null;
                cleanup = null;
                if (initialized)
                {
                    try
                    {
                        SteamAPI.Shutdown();
                    }
                    catch (Exception exception) when (exception is not OutOfMemoryException and not StackOverflowException and not AccessViolationException)
                    {
                        AppLog.Warning("Steam shutdown failed.", exception);
                    }
                }
            }
        }

        public static void Start(Func<Task> poll, Action stop)
        {
            ArgumentNullException.ThrowIfNull(poll);
            ArgumentNullException.ThrowIfNull(stop);
            if (pollAsync is not null)
            {
                throw new InvalidOperationException("A Steam presence is already registered.");
            }

            pollAsync = poll;
            cleanup = stop;
        }

        private static bool TryParseArguments(string[] args, out uint appId, out int parentId,
            out long parentStarted, out int gameId, out long gameStarted)
        {
            appId = 0;
            parentId = 0;
            parentStarted = 0;
            gameId = 0;
            gameStarted = 0;
            return args.Length == 7 && args[0] == "--steam-host" &&
                uint.TryParse(args[1], NumberStyles.None, CultureInfo.InvariantCulture, out appId) &&
                Integrations.ContainsKey(appId) &&
                int.TryParse(args[2], NumberStyles.None, CultureInfo.InvariantCulture, out parentId) &&
                parentId > 0 && parentId != Environment.ProcessId &&
                long.TryParse(args[3], NumberStyles.None, CultureInfo.InvariantCulture, out parentStarted) &&
                parentStarted > 0 && parentStarted <= DateTime.MaxValue.Ticks &&
                int.TryParse(args[4], NumberStyles.None, CultureInfo.InvariantCulture, out gameId) &&
                gameId > 0 && gameId != Environment.ProcessId && gameId != parentId &&
                long.TryParse(args[5], NumberStyles.None, CultureInfo.InvariantCulture, out gameStarted) &&
                gameStarted > 0 && gameStarted <= DateTime.MaxValue.Ticks &&
                IsCancellationEventName(args[6]);
        }

        private static bool IsCancellationEventName(string name)
        {
            const string prefix = @"Local\MultiPresence.Steam.";
            return name.StartsWith(prefix, StringComparison.Ordinal) &&
                Guid.TryParseExact(name[prefix.Length..], "N", out _);
        }

        private static Process? OpenProcess(int processId, long started, string? processName = null)
        {
            Process? process = null;
            try
            {
                process = Process.GetProcessById(processId);
                if (process.StartTime.ToUniversalTime().Ticks == started && IsRunning(process) &&
                    (processName is null || string.Equals(process.ProcessName, processName, StringComparison.OrdinalIgnoreCase)))
                {
                    return process;
                }
            }
            catch (Exception exception) when (exception is ArgumentException or InvalidOperationException or System.ComponentModel.Win32Exception)
            {
                AppLog.Warning("The Steam presence process is no longer available.", exception, new { ProcessId = processId });
            }

            process?.Dispose();
            return null;
        }

        private static bool IsRunning(Process process)
        {
            try
            {
                return !process.HasExited;
            }
            catch (Exception exception) when (exception is InvalidOperationException or System.ComponentModel.Win32Exception)
            {
                return false;
            }
        }
    }
}
