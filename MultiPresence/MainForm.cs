using DiscordRPC;
using Microsoft.Win32;
using MultiPresence.Infrastructure;
using MultiPresence.Properties;
using MultiPresence.Runtime;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text.Json;

namespace MultiPresence;

public partial class MainForm : Form
{
    private const string CurrentReleaseDate = "29.10.2026";
    private static readonly HttpClient UpdateClient = CreateUpdateClient();
    private readonly System.Windows.Forms.Timer _detectionTimer = new() { Interval = 3000 };
    private readonly SemaphoreSlim _detectionGate = new(1, 1);
    private readonly CancellationTokenSource _updateCancellation = new();
    private readonly BlacklistStore _blacklist = new(Path.Combine("Assets", "blacklist.txt"));
    private string? _activeGame;
    private bool _updatingMenu;
    private bool _exiting;

    public MainForm()
    {
        InitializeComponent();
        Directory.SetCurrentDirectory(AppContext.BaseDirectory);

        cb_DisableNotifications.Checked = Settings.Default.Notifications;
        cb_LaunchWithWindows.Checked = Settings.Default.startup;
        cb_LaunchWithWindowsAdmin.Checked = Settings.Default.startupadmin;
        cb_DisableAutoUpdates.Checked = Settings.Default.autoupdate;
        lb_Version.Text = $"MultiPresence {CurrentReleaseDate}";

        _detectionTimer.Tick += DetectionTimerOnTick;
        PresenceRuntime.DetectionRequested += PresenceRuntimeOnDetectionRequested;
        _detectionTimer.Start();

        if (!cb_DisableAutoUpdates.Checked)
        {
            _ = CheckForUpdatesAsync();
        }
    }

    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);
        Hide();
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        _exiting = true;
        _updateCancellation.Cancel();
        _detectionTimer.Stop();
        _detectionTimer.Tick -= DetectionTimerOnTick;
        PresenceRuntime.DetectionRequested -= PresenceRuntimeOnDetectionRequested;
        PresenceRuntime.StopAll();
        SteamPresenceLauncher.Stop(waitForExit: true);
        Hypervisor.DetachProcess();
        notify.Visible = false;
        base.OnFormClosed(e);
    }

    private void DetectionTimerOnTick(object? sender, EventArgs e) => _ = DetectAndStartAsync();

    private async Task DetectAndStartAsync()
    {
        if (_exiting || IsDisposed || !await _detectionGate.WaitAsync(0).ConfigureAwait(true))
        {
            return;
        }

        _detectionTimer.Stop();
        try
        {
            var detected = GameDetector.Detect();
            UpdateDetectedGame(detected?.GameName);
            if (detected is null)
            {
                _detectionTimer.Start();
                return;
            }

            if (_blacklist.Contains(detected.GameName))
            {
                _detectionTimer.Start();
                return;
            }

            if (!GameIntegrationRegistry.TryGet(detected.GameName, out var startAsync) || startAsync is null)
            {
                AppLog.Warning($"No integration is registered for detected game '{detected.GameName}'.");
                _detectionTimer.Start();
                return;
            }

            PlaceholderHelper._startTimestamp = Timestamps.Now;
            ShowTrackingNotification(detected.GameName);
            AppLog.Information("Starting game integration.", detected);
            await startAsync().ConfigureAwait(true);

            // Steam integrations complete only when the game exits. Memory integrations request
            // detection through PresenceRuntime when their controlled polling loop finishes.
            if (!_exiting && !IsDisposed && !PresenceRuntime.HasActiveLoops)
            {
                _detectionTimer.Start();
            }
        }
        catch (Exception exception) when (exception is not OutOfMemoryException and not StackOverflowException)
        {
            AppLog.Error("Starting a game integration failed.", exception, new { Game = _activeGame });
            if (!_exiting && !IsDisposed)
            {
                ShowErrorNotification("The game integration failed. See the log for details.");
                _detectionTimer.Start();
            }
        }
        finally
        {
            _detectionGate.Release();
        }
    }

    private void PresenceRuntimeOnDetectionRequested(object? sender, EventArgs e)
    {
        if (_exiting || IsDisposed || !IsHandleCreated)
        {
            return;
        }

        BeginInvoke(() =>
        {
            if (!_exiting)
            {
                _detectionTimer.Start();
            }
        });
    }

    private void UpdateDetectedGame(string? gameName)
    {
        _activeGame = string.IsNullOrWhiteSpace(gameName) ? null : gameName;
        _updatingMenu = true;
        try
        {
            lb_ActiveGame.Text = _activeGame is null
                ? "Active game: None"
                : $"Active game: {_activeGame}";
            btn_Blacklist.Enabled = _activeGame is not null;
            btn_Blacklist.Checked = _activeGame is not null && _blacklist.Contains(_activeGame);
            btn_Blacklist.Text = btn_Blacklist.Checked
                ? "Whitelist current game"
                : "Blacklist current game";
        }
        finally
        {
            _updatingMenu = false;
        }
    }

    private void btn_Exit_Click(object? sender, EventArgs e)
    {
        _exiting = true;
        SaveSettings();
        Close();
    }

    private void btn_Config_Click(object? sender, EventArgs e)
    {
        try
        {
            var configDirectory = Path.Combine(AppContext.BaseDirectory, "Assets", "Config");
            Directory.CreateDirectory(configDirectory);
            Process.Start(new ProcessStartInfo
            {
                FileName = configDirectory,
                UseShellExecute = true
            });
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException or InvalidOperationException)
        {
            AppLog.Warning("Could not open the configuration directory.", exception);
            MessageBox.Show(
                this,
                exception.Message,
                "MultiPresence",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

    private void btn_Blacklist_CheckedChanged(object? sender, EventArgs e)
    {
        if (_updatingMenu || _activeGame is null)
        {
            return;
        }

        try
        {
            _blacklist.SetBlocked(_activeGame, btn_Blacklist.Checked);
            btn_Blacklist.Text = btn_Blacklist.Checked
                ? "Whitelist current game"
                : "Blacklist current game";

            if (btn_Blacklist.Checked)
            {
                PresenceRuntime.StopAll();
                SteamPresenceLauncher.Stop();
            }
            else
            {
                _detectionTimer.Start();
            }
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            AppLog.Warning("Could not update the game blacklist.", exception);
            ShowErrorNotification("The blacklist could not be saved.");
        }
    }

    private void cb_DisableNotifications_CheckedChanged(object? sender, EventArgs e) => SaveSettings();

    private void cb_DisableAutoUpdates_CheckedChanged(object? sender, EventArgs e) => SaveSettings();

    private void cb_LaunchWithWindows_Click(object? sender, EventArgs e)
    {
        try
        {
            if (cb_LaunchWithWindows.Checked)
            {
                cb_LaunchWithWindowsAdmin.Checked = false;
                SetStartupTask(false);
            }

            SetRegistryStartup(cb_LaunchWithWindows.Checked);
            SaveSettings();
        }
        catch (Exception exception) when (exception is UnauthorizedAccessException or IOException)
        {
            cb_LaunchWithWindows.Checked = !cb_LaunchWithWindows.Checked;
            AppLog.Warning("Could not update normal startup registration.", exception);
            ShowErrorNotification("Windows startup could not be changed.");
        }
    }

    private void cb_LaunchWithWindowsAdmin_Click(object? sender, EventArgs e)
    {
        try
        {
            if (cb_LaunchWithWindowsAdmin.Checked)
            {
                cb_LaunchWithWindows.Checked = false;
                SetRegistryStartup(false);
            }

            SetStartupTask(cb_LaunchWithWindowsAdmin.Checked);
            SaveSettings();
        }
        catch (Exception exception) when (exception is UnauthorizedAccessException or IOException)
        {
            cb_LaunchWithWindowsAdmin.Checked = !cb_LaunchWithWindowsAdmin.Checked;
            AppLog.Warning("Could not update elevated startup registration.", exception);
            ShowErrorNotification("Administrator startup could not be changed.");
        }
    }

    private void SetRegistryStartup(bool enabled)
    {
        using var key = Registry.CurrentUser.OpenSubKey(
            @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run",
            writable: true) ?? throw new IOException("The Windows startup registry key is unavailable.");

        if (enabled)
        {
            key.SetValue("MultiPresence", $"\"{Application.ExecutablePath}\"");
        }
        else
        {
            key.DeleteValue("MultiPresence", throwOnMissingValue: false);
        }
    }

    private static void SetStartupTask(bool enabled)
    {
        const string taskName = "MultiPresence";
        using var taskService = new Microsoft.Win32.TaskScheduler.TaskService();
        if (!enabled)
        {
            taskService.RootFolder.DeleteTask(taskName, exceptionOnNotExists: false);
            return;
        }

        var definition = taskService.NewTask();
        definition.RegistrationInfo.Description = "Starts MultiPresence after interactive logon.";
        definition.Principal.LogonType = Microsoft.Win32.TaskScheduler.TaskLogonType.InteractiveToken;
        definition.Principal.RunLevel = Microsoft.Win32.TaskScheduler.TaskRunLevel.Highest;
        definition.Triggers.Add(new Microsoft.Win32.TaskScheduler.LogonTrigger());
        definition.Actions.Add(new Microsoft.Win32.TaskScheduler.ExecAction(Application.ExecutablePath));
        taskService.RootFolder.RegisterTaskDefinition(taskName, definition);
    }

    private async Task CheckForUpdatesAsync()
    {
        var installRequested = false;
        var cancellationToken = _updateCancellation.Token;
        try
        {
            using var response = await UpdateClient.GetAsync(
                "https://api.github.com/repos/Dekirai/MultiPresence/releases/latest", cancellationToken).ConfigureAwait(true);
            response.EnsureSuccessStatusCode();
            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(true);
            using var document = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken).ConfigureAwait(true);
            var release = GitHubUpdater.GetRelease(document.RootElement, CurrentReleaseDate);
            if (release == null || _exiting || IsDisposed || cb_DisableAutoUpdates.Checked)
            {
                return;
            }

            if (await GitHubUpdater.IsInstalledAsync(release, Application.ExecutablePath, cancellationToken).ConfigureAwait(true))
            {
                AppLog.Information("The published MultiPresence update is already installed.", new { Version = release.Tag });
                return;
            }
            if (_exiting || IsDisposed || cb_DisableAutoUpdates.Checked)
            {
                return;
            }

            AppLog.Information("A newer MultiPresence release is available.", new { CurrentVersion = CurrentReleaseDate, Version = release.Tag });
            var result = MessageBox.Show(
                this,
                $"MultiPresence {release.Tag} is available. Download and install the update?",
                "MultiPresence update",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Information);
            if (result == DialogResult.Yes)
            {
                installRequested = true;
                lb_Version.Text = $"Downloading MultiPresence {release.Tag}...";
                using var downloadClient = CreateUpdateClient(TimeSpan.FromMinutes(5));
                using var update = await GitHubUpdater.DownloadAsync(downloadClient, release, cancellationToken).ConfigureAwait(true);
                if (_exiting || IsDisposed)
                {
                    return;
                }

                SaveSettings();
                update.Launch(Application.ExecutablePath);
                AppLog.Information("Update downloaded. Restarting through Updater.exe.", new { Version = release.Tag });
                _exiting = true;
                Close();
            }
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
        }
        catch (Exception exception) when (
            exception is HttpRequestException or OperationCanceledException or JsonException or InvalidOperationException or
                IOException or InvalidDataException or UnauthorizedAccessException or System.ComponentModel.Win32Exception or KeyNotFoundException)
        {
            AppLog.Warning("The update could not be completed.", exception);
            if (installRequested && !_exiting && !IsDisposed)
            {
                MessageBox.Show(this, "The update could not be downloaded or started. MultiPresence will keep running.\n\n" + exception.Message,
                    "MultiPresence update", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (exception is InvalidDataException && !_exiting && !IsDisposed)
            {
                ShowErrorNotification("The update release could not be used. " + exception.Message);
            }
        }
        finally
        {
            if (!_exiting && !IsDisposed)
            {
                lb_Version.Text = $"MultiPresence {CurrentReleaseDate}";
            }
        }
    }

    private void SaveSettings()
    {
        Settings.Default.Notifications = cb_DisableNotifications.Checked;
        Settings.Default.autoupdate = cb_DisableAutoUpdates.Checked;
        Settings.Default.startup = cb_LaunchWithWindows.Checked;
        Settings.Default.startupadmin = cb_LaunchWithWindowsAdmin.Checked;
        Settings.Default.Save();
    }

    private void ShowTrackingNotification(string gameName)
    {
        UpdateDetectedGame(gameName);
        if (cb_DisableNotifications.Checked)
        {
            return;
        }

        notify.BalloonTipTitle = "MultiPresence";
        notify.BalloonTipText = $"Tracking {gameName}.";
        notify.ShowBalloonTip(3000);
    }

    private void ShowErrorNotification(string message)
    {
        if (cb_DisableNotifications.Checked)
        {
            return;
        }

        notify.BalloonTipTitle = "MultiPresence";
        notify.BalloonTipText = message;
        notify.ShowBalloonTip(5000);
    }

    private static HttpClient CreateUpdateClient(TimeSpan? timeout = null)
    {
        var client = new HttpClient { Timeout = timeout ?? TimeSpan.FromSeconds(10) };
        client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("MultiPresence", "12.0"));
        return client;
    }
}
