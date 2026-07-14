using MultiPresence.Core;

namespace MultiPresence;

public partial class MainForm
{
    private readonly BlacklistService _blacklistService = new(Path.Combine("Assets", "blacklist.json"));
    private readonly CancellationTokenSource _runtimeCancellation = new();
    private GamePresenceManager? _presenceManager;

    public void EnableRefactoredRuntime(bool updatesDisabled)
    {
        gameUpdater.Stop();
        gameUpdater.Elapsed -= gameUpdater_Tick;

        cb_DisableAutoUpdates.Checked = updatesDisabled;

        btn_Blacklist.CheckedChanged -= btn_Blacklist_CheckedChanged;
        btn_Blacklist.Click -= btn_Blacklist_Click;
        btn_Blacklist.Click += btn_BlacklistRefactored_Click;

        _presenceManager = new GamePresenceManager(
            _blacklistService,
            UpdateDetectedGame,
            game => RunOnUiThread(() => Balloon(game)));

        FormClosed += MainForm_RuntimeClosed;
        _presenceManager.Start();

        if (!updatesDisabled)
            _ = CheckForVerifiedUpdateAsync(_runtimeCancellation.Token);
    }

    private async Task CheckForVerifiedUpdateAsync(CancellationToken cancellationToken)
    {
        var updateService = new UpdateService();
        await updateService.CheckForUpdateAsync(
            message => RunOnUiThread(() => BalloonUpdate(message)),
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
