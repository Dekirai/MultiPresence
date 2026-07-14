using MultiPresence.Core;

namespace MultiPresence;

public partial class MainForm
{
    private readonly BlacklistService _blacklistService = new(Path.Combine("Assets", "blacklist.json"));
    private GamePresenceManager? _presenceManager;

    public void EnableRefactoredRuntime()
    {
        gameUpdater.Stop();
        gameUpdater.Elapsed -= gameUpdater_Tick;

        btn_Blacklist.CheckedChanged -= btn_Blacklist_CheckedChanged;
        btn_Blacklist.Click -= btn_Blacklist_Click;
        btn_Blacklist.Click += btn_BlacklistRefactored_Click;

        _presenceManager = new GamePresenceManager(
            gameUpdater,
            _blacklistService,
            UpdateDetectedGame,
            game => RunOnUiThread(() => Balloon(game)));

        FormClosed += (_, _) => _presenceManager?.Dispose();
        _presenceManager.Start();
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
