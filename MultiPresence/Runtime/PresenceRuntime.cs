using MultiPresence.Infrastructure;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace MultiPresence.Runtime;

/// <summary>
/// Owns all long-running presence loops. It prevents duplicate loops, contains failures,
/// and guarantees that process detection resumes after a game exits.
/// </summary>
public static class PresenceRuntime
{
    private static readonly ConcurrentDictionary<string, CancellationTokenSource> ActiveLoops =
        new(StringComparer.OrdinalIgnoreCase);

    public static event EventHandler? DetectionRequested;

    public static TimeSpan PollInterval { get; set; } = TimeSpan.FromSeconds(3);

    public static bool HasActiveLoops => !ActiveLoops.IsEmpty;

    public static void Start(string integrationName, string processName, Func<Task> pollAsync)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(integrationName);
        ArgumentException.ThrowIfNullOrWhiteSpace(processName);
        ArgumentNullException.ThrowIfNull(pollAsync);

        var cancellation = new CancellationTokenSource();
        if (!ActiveLoops.TryAdd(integrationName, cancellation))
        {
            cancellation.Dispose();
            AppLog.Warning($"Presence loop '{integrationName}' is already active.");
            return;
        }

        _ = RunAsync(integrationName, processName, pollAsync, cancellation);
    }

    public static void StopAll()
    {
        foreach (var cancellation in ActiveLoops.Values)
        {
            cancellation.Cancel();
        }
    }

    public static bool IsRunning(string integrationName) => ActiveLoops.ContainsKey(integrationName);

    public static void RequestDetection()
    {
        StopAll();
        DetectionRequested?.Invoke(null, EventArgs.Empty);
    }

    private static async Task RunAsync(
        string integrationName,
        string processName,
        Func<Task> pollAsync,
        CancellationTokenSource cancellation)
    {
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
            ActiveLoops.TryRemove(integrationName, out _);
            cancellation.Dispose();
            Hypervisor.DetachProcess();
            AppLog.Information($"Presence loop '{integrationName}' stopped.");
            RequestDetection();
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
