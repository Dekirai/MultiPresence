using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using MultiPresence.Infrastructure;

namespace MultiPresence.Updater
{
    internal static class Program
    {
        [STAThread]
        private static int Main(string[] args)
        {
            PreparedUpdate? update = null;
            var installed = false;
            try
            {
                if (args.Length is not 3 and not 5)
                {
                    throw new ArgumentException("Start this updater through MultiPresence.");
                }

                var zipPath = Path.GetFullPath(args[0]);
                var executablePath = Path.GetFullPath(args[1]);
                var updaterPath = Path.GetFullPath(args[2]);
                var currentPath = Environment.ProcessPath ?? throw new InvalidOperationException("The updater path is unavailable.");
                if (!updaterPath.Equals(currentPath, StringComparison.OrdinalIgnoreCase))
                {
                    throw new ArgumentException("The updater path does not match the running program.");
                }

                var workDirectory = Path.Combine(Path.GetDirectoryName(zipPath) ?? Path.GetTempPath(), "install-" + Guid.NewGuid().ToString("N"));
                update = UpdateInstaller.Prepare(zipPath, executablePath, workDirectory);
                WaitForApplication(executablePath, args);
                update.Install();
                installed = true;
                using var application = Process.Start(new ProcessStartInfo()
                {
                    FileName = executablePath,
                    WorkingDirectory = Path.GetDirectoryName(executablePath),
                    UseShellExecute = false
                }) ?? throw new InvalidOperationException("MultiPresence could not be restarted.");
                update.Complete();
                DeleteDownload(zipPath);
                AppLog.Information("MultiPresence was updated successfully.");
                return 0;
            }
            catch (Exception exception) when (exception is not OutOfMemoryException and not StackOverflowException)
            {
                if (installed && update != null)
                {
                    try
                    {
                        update.Rollback();
                    }
                    catch (Exception rollbackError) when (rollbackError is not OutOfMemoryException and not StackOverflowException)
                    {
                        exception = new AggregateException(exception, rollbackError);
                    }
                }

                AppLog.Error("The MultiPresence update failed.", exception);
                var recovery = update == null ? "" : "\n\nUpdate files and backups: " + update.WorkDirectory;
                MessageBox.Show("MultiPresence could not be updated.\n\n" + exception.Message + recovery,
                    "MultiPresence update", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 1;
            }
        }

        private static void WaitForApplication(string executablePath, string[] args)
        {
            var deadline = DateTime.UtcNow.AddSeconds(60);
            if (args.Length == 5)
            {
                if (!int.TryParse(args[3], NumberStyles.None, CultureInfo.InvariantCulture, out var processId) || processId <= 0 ||
                    !long.TryParse(args[4], NumberStyles.None, CultureInfo.InvariantCulture, out var startTime) || startTime <= 0)
                {
                    throw new ArgumentException("The application process is invalid.");
                }

                Process? parent = null;
                try
                {
                    parent = Process.GetProcessById(processId);
                    if (!parent.HasExited && parent.StartTime.ToUniversalTime().Ticks == startTime)
                    {
                        if (!string.Equals(parent.MainModule?.FileName, executablePath, StringComparison.OrdinalIgnoreCase))
                        {
                            throw new InvalidOperationException("The application process has a different executable.");
                        }
                        WaitForExit(parent, deadline);
                    }
                }
                catch (ArgumentException)
                {
                }
                catch (Exception exception) when (exception is InvalidOperationException or Win32Exception && parent != null && parent.HasExited)
                {
                }
                finally
                {
                    parent?.Dispose();
                }
            }

            var processes = Process.GetProcessesByName(Path.GetFileNameWithoutExtension(executablePath));
            try
            {
                foreach (var process in processes)
                {
                    if (IsApplication(process, executablePath))
                    {
                        WaitForExit(process, deadline);
                    }
                }
            }
            finally
            {
                foreach (var process in processes)
                {
                    process.Dispose();
                }
            }
        }

        private static bool IsApplication(Process process, string executablePath)
        {
            try
            {
                return !process.HasExited && string.Equals(process.MainModule?.FileName, executablePath, StringComparison.OrdinalIgnoreCase);
            }
            catch (Exception exception) when (exception is Win32Exception or InvalidOperationException)
            {
                return false;
            }
        }

        private static void WaitForExit(Process process, DateTime deadline)
        {
            var remaining = deadline - DateTime.UtcNow;
            if (!process.HasExited && (remaining <= TimeSpan.Zero || !process.WaitForExit((int)remaining.TotalMilliseconds)))
            {
                throw new TimeoutException("MultiPresence is still running. Close it before trying the update again.");
            }
        }

        private static void DeleteDownload(string zipPath)
        {
            try
            {
                File.Delete(zipPath);
            }
            catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
            {
                AppLog.Warning("Could not remove the downloaded update archive.", exception);
            }
        }
    }
}
