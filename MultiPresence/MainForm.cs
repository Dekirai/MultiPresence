using DiscordRPC;
using Microsoft.Win32;
using MultiPresence.Infrastructure;
using MultiPresence.Properties;
using MultiPresence.Runtime;
using System.Diagnostics;
using System.Globalization;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text.Json;

namespace MultiPresence;

public partial class MainForm : Form
{
    private const string CurrentReleaseDate = "21.07.2026";
    private static readonly HttpClient UpdateClient = CreateUpdateClient();
    private readonly System.Windows.Forms.Timer _detectionTimer = new() { Interval = 3000 };
    private readonly SemaphoreSlim _detectionGate = new(1, 1);
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
        lb_Version.Text = $"MultiPresence {GetDisplayVersion()}";

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
        _detectionTimer.Stop();
        _detectionTimer.Tick -= DetectionTimerOnTick;
        PresenceRuntime.DetectionRequested -= PresenceRuntimeOnDetectionRequested;
        PresenceRuntime.StopAll();
        Hypervisor.DetachProcess();
        notify.Visible = false;
        base.OnFormClosed(e);
    }

    private void DetectionTimerOnTick(object? sender, EventArgs e) => _ = DetectAndStartAsync();

    private async Task DetectAndStartAsync()
    {
        if (!await _detectionGate.WaitAsync(0).ConfigureAwait(true))
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

            DeleteStaleSteamAppId();
            PlaceholderHelper._startTimestamp = Timestamps.Now;
            ShowTrackingNotification(detected.GameName);
            AppLog.Information("Starting game integration.", detected);
            await startAsync().ConfigureAwait(true);

            // Steam integrations complete only when the game exits. Memory integrations request
            // detection through PresenceRuntime when their controlled polling loop finishes.
            if (!PresenceRuntime.HasActiveLoops)
            {
                _detectionTimer.Start();
            }
        }
        catch (Exception exception) when (exception is not OutOfMemoryException and not StackOverflowException)
        {
            AppLog.Error("Starting a game integration failed.", exception, new { Game = _activeGame });
            ShowErrorNotification("The game integration failed. See the log for details.");
            _detectionTimer.Start();
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
        try
        {
            using var response = await UpdateClient.GetAsync(
                "https://api.github.com/repos/Dekirai/MultiPresence/releases/latest").ConfigureAwait(true);
            response.EnsureSuccessStatusCode();
            await using var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(true);
            using var document = await JsonDocument.ParseAsync(stream).ConfigureAwait(true);
            var root = document.RootElement;
            var tag = root.GetProperty("tag_name").GetString()?.TrimStart('v');
            var url = root.GetProperty("html_url").GetString();

            if (!TryParseReleaseDate(tag, out var latest) ||
                !TryParseReleaseDate(CurrentReleaseDate, out var current) ||
                latest <= current ||
                !Uri.TryCreate(url, UriKind.Absolute, out var releaseUri))
            {
                return;
            }

            var result = MessageBox.Show(
                this,
                $"MultiPresence {tag} is available. Open the release page?",
                "MultiPresence update",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Information);
            if (result == DialogResult.Yes)
            {
                Process.Start(new ProcessStartInfo(releaseUri.AbsoluteUri) { UseShellExecute = true });
            }
        }
        catch (Exception exception) when (
            exception is HttpRequestException or TaskCanceledException or JsonException or InvalidOperationException)
        {
            AppLog.Warning("The update check failed.", exception);
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

    private static void DeleteStaleSteamAppId()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Assets", "steam_appid.txt");
        try
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
        catch (IOException exception)
        {
            AppLog.Warning("Could not remove the previous Steam application ID.", exception);
        }
    }

    private static string GetDisplayVersion() =>
        Assembly.GetExecutingAssembly().GetName().Version?.ToString(3) ?? CurrentReleaseDate;

    private static bool TryParseReleaseDate(string? value, out DateTime date) =>
        DateTime.TryParseExact(
            value,
            "dd.MM.yyyy",
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out date);

    private static HttpClient CreateUpdateClient()
    {
        var client = new HttpClient { Timeout = TimeSpan.FromSeconds(10) };
        client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("MultiPresence", "12.0"));
        return client;
    }
}
