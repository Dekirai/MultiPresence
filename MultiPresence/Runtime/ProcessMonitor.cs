using MultiPresence.Infrastructure;
using System.Diagnostics;

namespace MultiPresence.Runtime;

public static class ProcessMonitor
{
    public static bool IsRunning(string processName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(processName);
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
            DisposeAll(processes);
        }
    }

    public static bool TryAttach(string processName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(processName);
        Process[] processes = [];
        try
        {
            processes = Process.GetProcessesByName(processName);
            var process = processes.FirstOrDefault(static candidate =>
            {
                try
                {
                    return !candidate.HasExited;
                }
                catch
                {
                    return false;
                }
            });

            if (process is null)
            {
                return false;
            }

            Hypervisor.AttachProcess(process);
            return true;
        }
        catch (Exception exception) when (exception is not OutOfMemoryException and not StackOverflowException)
        {
            AppLog.Warning($"Could not attach Hypervisor to '{processName}'.", exception);
            return false;
        }
        finally
        {
            DisposeAll(processes);
        }
    }

    private static void DisposeAll(IEnumerable<Process> processes)
    {
        foreach (var process in processes)
        {
            process.Dispose();
        }
    }
}
