using Microsoft.Win32;
using Microsoft.Win32.TaskScheduler;

namespace MultiPresence.Core;

public enum StartupMode
{
    Disabled,
    CurrentUser,
    Elevated
}

public sealed class StartupService
{
    private const string AppName = "MultiPresence";
    private readonly string _executablePath;

    public StartupService(string executablePath)
    {
        _executablePath = Path.GetFullPath(executablePath);
    }

    public void Apply(StartupMode mode)
    {
        switch (mode)
        {
            case StartupMode.Disabled:
                SetCurrentUserStartup(false);
                SetElevatedStartup(false);
                break;
            case StartupMode.CurrentUser:
                SetElevatedStartup(false);
                SetCurrentUserStartup(true);
                break;
            case StartupMode.Elevated:
                SetCurrentUserStartup(false);
                SetElevatedStartup(true);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
        }
    }

    private void SetCurrentUserStartup(bool enabled)
    {
        using var registryKey = Registry.CurrentUser.OpenSubKey(
            @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run",
            writable: true)
            ?? throw new InvalidOperationException("Could not open the current-user startup registry key.");

        if (enabled)
            registryKey.SetValue(AppName, $"\"{_executablePath}\"");
        else
            registryKey.DeleteValue(AppName, throwOnMissingValue: false);
    }

    private void SetElevatedStartup(bool enabled)
    {
        using var taskService = new TaskService();
        if (!enabled)
        {
            taskService.RootFolder.DeleteTask(AppName, false);
            return;
        }

        var task = taskService.NewTask();
        task.RegistrationInfo.Description = "Starts MultiPresence with elevated privileges at logon.";
        task.Principal.LogonType = TaskLogonType.InteractiveToken;
        task.Principal.RunLevel = TaskRunLevel.Highest;
        task.Triggers.Add(new LogonTrigger());
        task.Actions.Add(new ExecAction(_executablePath, null, Path.GetDirectoryName(_executablePath)));
        taskService.RootFolder.RegisterTaskDefinition(AppName, task);
    }
}
