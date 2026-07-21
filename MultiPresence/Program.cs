using MultiPresence.Infrastructure;

namespace MultiPresence;

internal static class Program
{
    private const string InstanceMutexName = @"Local\MultiPresence.5F864D79-3411-4DC7-84D5-4052C5436F23";

    [STAThread]
    private static void Main()
    {
        using var instanceMutex = new Mutex(initiallyOwned: true, InstanceMutexName, out var isFirstInstance);
        if (!isFirstInstance)
        {
            MessageBox.Show(
                "MultiPresence is already running in the notification area.",
                "MultiPresence",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            return;
        }

        ApplicationConfiguration.Initialize();
        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
        Application.ThreadException += (_, eventArgs) =>
            HandleUnhandledException("Windows Forms thread", eventArgs.Exception);
        AppDomain.CurrentDomain.UnhandledException += (_, eventArgs) =>
            HandleUnhandledException(
                "Application domain",
                eventArgs.ExceptionObject as Exception ?? new InvalidOperationException("Unknown fatal error."));
        TaskScheduler.UnobservedTaskException += (_, eventArgs) =>
        {
            AppLog.Error("An unobserved background task failed.", eventArgs.Exception);
            eventArgs.SetObserved();
        };

        try
        {
            Application.Run(new MainForm());
        }
        catch (Exception exception)
        {
            HandleUnhandledException("Application startup", exception);
        }
        finally
        {
            instanceMutex.ReleaseMutex();
        }
    }

    private static void HandleUnhandledException(string source, Exception exception)
    {
        AppLog.Error($"Unhandled exception in {source}.", exception);
        MessageBox.Show(
            "MultiPresence encountered an unexpected error. Details were written to the log.",
            "MultiPresence",
            MessageBoxButtons.OK,
            MessageBoxIcon.Error);
    }
}
