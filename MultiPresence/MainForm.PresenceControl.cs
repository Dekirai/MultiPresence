using DiscordRPC;
using MultiPresence.Infrastructure;
using MultiPresence.Properties;
using MultiPresence.Runtime;

namespace MultiPresence;

public partial class MainForm
{
    private readonly ToolStripMenuItem _selectGame = new("Select game");
    private readonly ToolStripMenuItem _preferredGame = new("Preferred game...");
    private readonly ToolStripMenuItem _pausePresence = new("Pause presence") { CheckOnClick = true };
    private readonly ToolStripMenuItem _restartDetection = new("Restart detection");
    private IReadOnlyList<DetectedGame> _availableGames = Array.Empty<DetectedGame>();
    private DetectedGame? _activeDetectedGame;
    private Task? _integrationTask;
    private ConfigurationEditorForm? _configurationEditor;
    private string? _manualGame;
    private bool _paused;

    private void InitializePresenceControls()
    {
        var editor = new ToolStripMenuItem("Edit presence...");
        editor.Click += (_, _) => OpenConfigurationEditor();
        cms.Items.Insert(cms.Items.IndexOf(btn_Config), editor);
        var position = cms.Items.IndexOf(btn_Blacklist);
        cms.Items.Insert(position++, _selectGame);
        cms.Items.Insert(position++, _preferredGame);
        cms.Items.Insert(position++, _pausePresence);
        cms.Items.Insert(position, _restartDetection);
        _selectGame.DropDownOpening += SelectGameOnOpening;
        _preferredGame.Click += PreferredGameOnClick;
        _pausePresence.Click += PausePresenceOnClick;
        _restartDetection.Click += RestartDetectionOnClick;
        UpdateSelectionMenu();
    }

    private bool IntegrationIsRunning => PresenceRuntime.HasActiveLoops || SteamPresenceLauncher.IsRunning ||
        _integrationTask is { IsCompleted: false };

    private async Task UpdatePresenceAsync()
    {
        if (_exiting || _paused || IsDisposed || !await _detectionGate.WaitAsync(0))
        {
            return;
        }

        try
        {
            _availableGames = GameDetector.DetectAll(IntegrationIsRunning ? _activeDetectedGame : null);
            if (_manualGame != null && !_availableGames.Any(game => game.GameName.Equals(_manualGame, StringComparison.OrdinalIgnoreCase)))
            {
                _manualGame = null;
            }
            var selected = GameSelection.Choose(_availableGames, _manualGame, Settings.Default.PreferredGame,
                _activeDetectedGame?.GameName, _blacklist.Contains);
            UpdateSelectionMenu();
            if (selected == _activeDetectedGame && IntegrationIsRunning)
            {
                return;
            }

            await StopCurrentPresenceAsync();
            if (_exiting || _paused || selected == null)
            {
                return;
            }
            if (!GameIntegrationRegistry.TryGet(selected.GameName, out var startAsync) || startAsync == null)
            {
                AppLog.Warning($"No integration is registered for detected game '{selected.GameName}'.");
                return;
            }

            _activeDetectedGame = selected;
            PlaceholderHelper._startTimestamp = Timestamps.Now;
            AppLog.Information("Starting game integration.", selected);
            _integrationTask = ObserveIntegrationAsync(startAsync(), selected);
            ShowTrackingNotification(selected.GameName);
        }
        catch (Exception exception) when (exception is not OutOfMemoryException and not StackOverflowException)
        {
            AppLog.Error("Starting a game integration failed.", exception, new { Game = _activeGame });
            await StopCurrentPresenceAsync();
            if (!_exiting && !IsDisposed)
            {
                ShowErrorNotification("The game integration failed. See the log for details.");
            }
        }
        finally
        {
            _detectionGate.Release();
            if (!_exiting && !_paused && !IsDisposed)
            {
                _detectionTimer.Start();
            }
        }
    }

    private async Task ObserveIntegrationAsync(Task integration, DetectedGame game)
    {
        try
        {
            await integration;
        }
        catch (Exception exception) when (exception is not OutOfMemoryException and not StackOverflowException)
        {
            AppLog.Error("The game integration stopped unexpectedly.", exception, game);
            if (!_exiting && !IsDisposed)
            {
                ShowErrorNotification("The game integration failed. See the log for details.");
            }
        }
    }

    private async Task StopCurrentPresenceAsync()
    {
        await Task.WhenAll(PresenceRuntime.StopAllAsync(), SteamPresenceLauncher.StopAsync());
        if (_integrationTask != null)
        {
            await _integrationTask;
            await Task.WhenAll(PresenceRuntime.StopAllAsync(), SteamPresenceLauncher.StopAsync());
        }
        _integrationTask = null;
        _activeDetectedGame = null;
        PresencePreviewStore.Clear();
        UpdateDetectedGame(null);
    }

    private async Task RestartPresenceAsync()
    {
        _detectionTimer.Stop();
        _restartDetection.Enabled = false;
        await _detectionGate.WaitAsync();
        try
        {
            await StopCurrentPresenceAsync();
        }
        catch (Exception exception) when (exception is not OutOfMemoryException and not StackOverflowException)
        {
            AppLog.Warning("Could not stop the current presence.", exception);
            ShowErrorNotification("The presence could not be restarted. See the log for details.");
        }
        finally
        {
            _detectionGate.Release();
            _restartDetection.Enabled = true;
        }
        if (!_exiting && !_paused)
        {
            await UpdatePresenceAsync();
        }
    }

    private async void RestartDetectionOnClick(object? sender, EventArgs e)
    {
        await RestartPresenceAsync();
    }

    private async void PausePresenceOnClick(object? sender, EventArgs e)
    {
        _paused = _pausePresence.Checked;
        _pausePresence.Text = _paused ? "Resume presence" : "Pause presence";
        await RestartPresenceAsync();
        UpdateDetectedGame(_activeDetectedGame?.GameName);
    }

    private async void SelectGameOnOpening(object? sender, EventArgs e)
    {
        if (_exiting || !await _detectionGate.WaitAsync(0))
        {
            return;
        }
        try
        {
            _availableGames = GameDetector.DetectAll(IntegrationIsRunning ? _activeDetectedGame : null);
            UpdateSelectionMenu(true);
        }
        finally
        {
            _detectionGate.Release();
        }
    }

    private void UpdateSelectionMenu(bool force = false)
    {
        if (_selectGame.DropDown.Visible && !force)
        {
            return;
        }
        foreach (var item in _selectGame.DropDownItems.Cast<ToolStripItem>().ToArray())
        {
            item.Dispose();
        }
        _selectGame.DropDownItems.Clear();
        var automatic = new ToolStripMenuItem("Automatic") { Checked = _manualGame == null };
        automatic.Click += async (_, _) =>
        {
            _manualGame = null;
            await RestartPresenceAsync();
            UpdateSelectionMenu();
        };
        _selectGame.DropDownItems.Add(automatic);
        _selectGame.DropDownItems.Add(new ToolStripSeparator());
        if (_availableGames.Count == 0)
        {
            _selectGame.DropDownItems.Add(new ToolStripMenuItem("No supported games running") { Enabled = false });
        }
        foreach (var game in _availableGames.OrderBy(game => game.GameName, StringComparer.OrdinalIgnoreCase))
        {
            var blocked = _blacklist.Contains(game.GameName);
            var item = new ToolStripMenuItem(game.GameName + (blocked ? " (blacklisted)" : ""))
            {
                Checked = string.Equals(_manualGame, game.GameName, StringComparison.OrdinalIgnoreCase)
            };
            var selection = item;
            if (blocked)
            {
                selection = new ToolStripMenuItem("Whitelist and select");
                item.DropDownItems.Add(selection);
            }
            selection.Click += async (_, _) =>
            {
                if (blocked)
                {
                    try
                    {
                        _blacklist.SetBlocked(game.GameName, false);
                    }
                    catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
                    {
                        AppLog.Warning("Could not update the game blacklist.", exception);
                        ShowErrorNotification("The blacklist could not be saved.");
                        return;
                    }
                }
                _manualGame = game.GameName;
                await RestartPresenceAsync();
                UpdateSelectionMenu();
            };
            _selectGame.DropDownItems.Add(item);
        }
        _selectGame.Text = _manualGame == null ? "Select game: Automatic" : "Select game: " + _manualGame;
        _preferredGame.Text = string.IsNullOrEmpty(Settings.Default.PreferredGame)
            ? "Preferred game..." : "Preferred game: " + Settings.Default.PreferredGame;
    }

    private async void PreferredGameOnClick(object? sender, EventArgs e)
    {
        using var dialog = new PreferredGameForm(GameIntegrationRegistry.SupportedGames, Settings.Default.PreferredGame);
        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }
        Settings.Default.PreferredGame = dialog.SelectedGame;
        Settings.Default.Save();
        _manualGame = null;
        UpdateSelectionMenu();
        await RestartPresenceAsync();
    }

    private void OpenConfigurationEditor()
    {
        if (_configurationEditor == null || _configurationEditor.IsDisposed)
        {
            _configurationEditor = new ConfigurationEditorForm(_activeGame);
            _configurationEditor.Show(this);
        }
        else
        {
            _configurationEditor.WindowState = FormWindowState.Normal;
            _configurationEditor.Activate();
        }
    }

    private bool CloseConfigurationEditor()
    {
        if (_configurationEditor == null || _configurationEditor.IsDisposed)
        {
            return true;
        }
        _configurationEditor.Close();
        return _configurationEditor.IsDisposed;
    }
}
