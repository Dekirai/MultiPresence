using MultiPresence.Core;
using MultiPresence.Properties;

namespace MultiPresence;

public partial class MainForm
{
    private readonly BlacklistService _blacklistService = new(Path.Combine("Assets", "blacklist.json"));
    private readonly CancellationTokenSource _runtimeCancellation = new();
    private GamePresenceManager? _presenceManager;
    private StartupService? _startupService;
    private NotificationService? _notificationService;

    public void EnableRefactoredRuntime(bool updatesDisabled)
    {
        gameUpdater.Stop();
        gameUpdater.Elapsed -= gameUpdater_Tick;

        _startupService = new StartupService(Application.ExecutablePath);
        _notificationService = new NotificationService(notify, () => cb_DisableNotifications.Checked);

        cb_DisableAutoUpdates.Checked = updatesDisabled;
        cb_LaunchWithWindows.Checked = Settings.Default.startup;
        cb_LaunchWithWindowsAdmin.Checked = Settings.Default.startupadmin;

        btn_Blacklist.CheckedChanged -= btn_Blacklist_CheckedChanged;
        btn_Blacklist.Click -= btn_Blacklist_Click;
        btn_Blacklist.Click += btn_BlacklistRefactored_Click;

        cb_LaunchWithWindows.Click -= cb_LaunchWithWindows_Click;
        cb_LaunchWithWindowsAdmin.Click -= cb_LaunchWithWindowsAdmin_Click;
        cb_LaunchWithWindows.Click += cb_LaunchWithWindowsRefactored_Click;
        cb_LaunchWithWindowsAdmin.Click += cb_LaunchWithWindowsAdminRefactored_Click;

        _presenceManager = new GamePresenceManager(
            _blacklistService,
            UpdateDetectedGame,
            game => RunOnUiThread(() => _notificationService.ShowGameTracking(game)));

        FormClosed += MainForm_RuntimeClosed;
        _presenceManager.Start();

        if (!updatesDisabled)
            _ = CheckForVerifiedUpdateAsync(_runtimeCancellation.Token);
    }

    private async Task CheckForVerifiedUpdateAsync(CancellationToken cancellationToken)
    {
        var updateService = new UpdateService();
        await updateService.CheckForUpdateAsync(
            message => RunOnUiThread(() => _notificationService?.ShowUpdateStatus(message)),
            cancellationToken);
    }

    private async void MainForm_RuntimeClosed(object? sender, FormClosedEventArgs e)
    {
        _runtimeCancellation.Cancel();

        if (_presenceManager is not null)
            await _presenceManager.DisposeAsync();

        _runtimeCancellation.Dispose();
    }

    private void UpdateDetectedGame(string game, bool blocked)
    {
        RunOnUiThread(() =>
        {
            var hasGame = !string.IsNullOrWhiteSpace(game);
            lb_ActiveGame.Text = hasGame ? $"Active game: {game}" : "Active game: None";
            btn_Blacklist.Enabled = hasGame;
            btn_Blacklist.Checked = blocked;
            btn_Blacklist.Text = blocked ? "Whitelist current game" : "Blacklist current game";
        });
    }

    private async void btn_BlacklistRefactored_Click(object? sender, EventArgs e)
    {
        try
        {
            var game = GameDetector.GetGame();
            if (string.IsNullOrWhiteSpace(game))
                return;

            var currentlyBlocked = await _blacklistService.ContainsAsync(game);
            await _blacklistService.SetAsync(game, !currentlyBlocked);
            UpdateDetectedGame(game, !currentlyBlocked);
        }
        catch (Exception ex)
        {
            RateLimitedLogger.Error("blacklist-update", ex);
            MessageBox.Show($"Could not update the blacklist: {ex.Message}", "Blacklist Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void cb_LaunchWithWindowsRefactored_Click(object? sender, EventArgs e)
    {
        if (_startupService is null)
            return;

        try
        {
            if (cb_LaunchWithWindows.Checked)
            {
                cb_LaunchWithWindowsAdmin.Checked = false;
                _startupService.Apply(StartupMode.CurrentUser);
                Settings.Default.startup = true;
                Settings.Default.startupadmin = false;
            }
            else
            {
                _startupService.Apply(StartupMode.Disabled);
                Settings.Default.startup = false;
            }

            Settings.Default.Save();
        }
        catch (Exception ex)
        {
            RateLimitedLogger.Error("startup-current-user", ex);
            cb_LaunchWithWindows.Checked = Settings.Default.startup;
            MessageBox.Show($"Could not update Windows startup: {ex.Message}", "Startup Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void cb_LaunchWithWindowsAdminRefactored_Click(object? sender, EventArgs e)
    {
        if (_startupService is null)
            return;

        try
        {
            if (cb_LaunchWithWindowsAdmin.Checked)
            {
                cb_LaunchWithWindows.Checked = false;
                _startupService.Apply(StartupMode.Elevated);
                Settings.Default.startup = false;
                Settings.Default.startupadmin = true;
            }
            else
            {
                _startupService.Apply(StartupMode.Disabled);
                Settings.Default.startupadmin = false;
            }

            Settings.Default.Save();
        }
        catch (Exception ex)
        {
            RateLimitedLogger.Error("startup-elevated", ex);
            cb_LaunchWithWindowsAdmin.Checked = Settings.Default.startupadmin;
            MessageBox.Show($"Could not update elevated Windows startup: {ex.Message}", "Startup Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void RunOnUiThread(Action action)
    {
        if (IsDisposed || Disposing)
            return;

        if (InvokeRequired)
        {
            BeginInvoke(action);
            return;
        }

        action();
    }
}
