using MultiPresence.Infrastructure;
using System.Diagnostics;
using System.Reflection;
using DiscordRPC;

namespace MultiPresence.Runtime;

/// <summary>
/// Owns all long-running presence loops. It prevents duplicate loops, contains failures,
/// and guarantees that process detection resumes after a game exits.
/// </summary>
public static class PresenceRuntime
{
    private static readonly object Sync = new();
    private static readonly Dictionary<string, PresenceLoop> ActiveLoops =
        new(StringComparer.OrdinalIgnoreCase);

    public static event EventHandler? DetectionRequested;

    public static TimeSpan PollInterval { get; set; } = TimeSpan.FromSeconds(3);

    public static bool HasActiveLoops
    {
        get
        {
            lock (Sync)
            {
                return ActiveLoops.Count != 0;
            }
        }
    }

    public static void Start(string integrationName, string processName, Func<Task> pollAsync)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(integrationName);
        ArgumentException.ThrowIfNullOrWhiteSpace(processName);
        ArgumentNullException.ThrowIfNull(pollAsync);

        var loop = new PresenceLoop(pollAsync.Method.DeclaringType);
        lock (Sync)
        {
            if (ActiveLoops.ContainsKey(integrationName))
            {
                loop.DisposeCancellation();
                AppLog.Warning($"Presence loop '{integrationName}' is already active.");
                return;
            }
            ActiveLoops.Add(integrationName, loop);
        }

        loop.CaptureResources();
        _ = RunAsync(integrationName, processName, pollAsync, loop);
    }

    public static void StopAll()
    {
        foreach (var loop in GetActiveLoops())
        {
            loop.Cancel();
        }
    }

    private static PresenceLoop[] GetActiveLoops()
    {
        lock (Sync)
        {
            return ActiveLoops.Values.ToArray();
        }
    }

    public static Task StopAllAsync()
    {
        var loops = GetActiveLoops();
        foreach (var loop in loops)
        {
            loop.Cancel();
        }
        return Task.WhenAll(loops.Select(loop => loop.Completion.Task));
    }

    public static bool IsRunning(string integrationName)
    {
        lock (Sync)
        {
            return ActiveLoops.ContainsKey(integrationName);
        }
    }

    public static void RequestDetection()
    {
        StopAll();
        if (!HasActiveLoops)
        {
            DetectionRequested?.Invoke(null, EventArgs.Empty);
        }
    }

    private static async Task RunAsync(
        string integrationName,
        string processName,
        Func<Task> pollAsync,
        PresenceLoop loop)
    {
        var cancellation = loop.Cancellation;
        var consecutiveFailures = 0;
        AppLog.Information($"Presence loop '{integrationName}' started.", new { Process = processName });

        try
        {
            while (!cancellation.IsCancellationRequested)
            {
                var processExists = IsProcessRunning(processName);
                try
                {
                    // Poll once after process exit so legacy integration cleanup can run.
                    await pollAsync().ConfigureAwait(false);
                    loop.CaptureResources();
                    consecutiveFailures = 0;
                }
                catch (Exception exception) when (!IsFatal(exception))
                {
                    consecutiveFailures++;
                    AppLog.Warning(
                        $"Presence loop '{integrationName}' failed; it will be retried.",
                        exception,
                        new { ConsecutiveFailures = consecutiveFailures });
                }

                if (!processExists || !IsProcessRunning(processName))
                {
                    break;
                }

                var delay = consecutiveFailures >= 3
                    ? TimeSpan.FromSeconds(Math.Min(30, consecutiveFailures * 2))
                    : PollInterval;

                await Task.Delay(delay, cancellation.Token).ConfigureAwait(false);
            }
        }
        catch (OperationCanceledException) when (cancellation.IsCancellationRequested)
        {
            // Expected during application shutdown.
        }
        finally
        {
            loop.DisposeResources();
            Hypervisor.DetachProcess();
            lock (Sync)
            {
                if (ActiveLoops.TryGetValue(integrationName, out var current) && ReferenceEquals(current, loop))
                {
                    ActiveLoops.Remove(integrationName);
                }
                loop.DisposeCancellation();
                loop.Completion.TrySetResult();
            }
            AppLog.Information($"Presence loop '{integrationName}' stopped.");
            if (!HasActiveLoops)
            {
                DetectionRequested?.Invoke(null, EventArgs.Empty);
            }
        }
    }

    private sealed class PresenceLoop(Type? owner)
    {
        private readonly Dictionary<FieldInfo, IDisposable> resources = new();
        private readonly object cancellationSync = new();
        private int cancelling;
        private bool cancellationDisposed;
        public CancellationTokenSource Cancellation { get; } = new();
        public TaskCompletionSource Completion { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);

        public void Cancel()
        {
            lock (cancellationSync)
            {
                if (cancellationDisposed)
                {
                    return;
                }
                cancelling++;
            }

            try
            {
                Cancellation.Cancel();
            }
            finally
            {
                lock (cancellationSync)
                {
                    cancelling--;
                    if (cancellationDisposed && cancelling == 0)
                    {
                        Cancellation.Dispose();
                    }
                }
            }
        }

        public void DisposeCancellation()
        {
            lock (cancellationSync)
            {
                if (cancellationDisposed)
                {
                    return;
                }
                cancellationDisposed = true;
                if (cancelling == 0)
                {
                    Cancellation.Dispose();
                }
            }
        }

        public void CaptureResources()
        {
            if (owner == null)
            {
                return;
            }

            foreach (var field in owner.GetFields(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public))
            {
                if ((field.FieldType == typeof(DiscordRpcClient) || field.FieldType == typeof(DiscordStatusUpdater)) &&
                    field.GetValue(null) is IDisposable resource)
                {
                    resources.TryAdd(field, resource);
                }
            }
        }

        public void DisposeResources()
        {
            foreach (var (field, resource) in resources)
            {
                if (resource is DiscordRpcClient client && client.IsInitialized && !client.IsDisposed)
                {
                    try
                    {
                        client.ClearPresence();
                    }
                    catch (Exception exception) when (!IsFatal(exception))
                    {
                        AppLog.Warning("Could not clear a stopped Discord presence.", exception);
                    }
                }

                try
                {
                    resource.Dispose();
                    if (!field.IsInitOnly && ReferenceEquals(field.GetValue(null), resource))
                    {
                        field.SetValue(null, null);
                    }
                }
                catch (Exception exception) when (!IsFatal(exception))
                {
                    AppLog.Warning("Could not dispose a stopped presence resource.", exception);
                }
            }
        }
    }

    private static bool IsProcessRunning(string processName)
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

    private static bool IsFatal(Exception exception) =>
        exception is OutOfMemoryException or StackOverflowException or AccessViolationException;
}
