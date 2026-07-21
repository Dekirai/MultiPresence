using MultiPresence.Infrastructure;
using System.Collections.Frozen;
using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace MultiPresence;

/// <summary>
/// Loads and atomically hot-reloads Discord presence templates.
/// The last valid configuration remains active when a file is temporarily invalid while being edited.
/// </summary>
public sealed class DiscordStatusUpdater : IDisposable
{
    private static readonly TimeSpan ReloadDelay = TimeSpan.FromMilliseconds(350);
    private readonly string _configPath;
    private readonly object _reloadLock = new();
    private readonly FileSystemWatcher? _watcher;
    private readonly System.Threading.Timer? _reloadTimer;
    private FrozenDictionary<string, FrozenDictionary<string, PresenceTemplate>> _games = EmptyGames();
    private bool _disposed;

    public DiscordStatusUpdater(string configPath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(configPath);
        _configPath = Path.GetFullPath(configPath, AppContext.BaseDirectory);
        LoadConfiguration();

        var directory = Path.GetDirectoryName(_configPath);
        if (string.IsNullOrWhiteSpace(directory))
        {
            return;
        }

        try
        {
            Directory.CreateDirectory(directory);
            _reloadTimer = new System.Threading.Timer(
                static state => ((DiscordStatusUpdater)state!).LoadConfiguration(),
                this,
                Timeout.InfiniteTimeSpan,
                Timeout.InfiniteTimeSpan);

            _watcher = new FileSystemWatcher(directory, Path.GetFileName(_configPath))
            {
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.Size,
                EnableRaisingEvents = true
            };
            _watcher.Changed += ScheduleReload;
            _watcher.Created += ScheduleReload;
            _watcher.Renamed += ScheduleReload;
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            AppLog.Warning($"Could not watch presence configuration '{_configPath}'.", exception);
        }
    }

    public string UpdateDetails(
        string gameName,
        Dictionary<string, object> placeholders,
        string state = "Default") =>
        Render(gameName, state, placeholders, static template => template.Details);

    public string UpdateState(
        string gameName,
        Dictionary<string, object> placeholders,
        string state = "Default") =>
        Render(gameName, state, placeholders, static template => template.State);

    public string UpdateLargeAsset(
        string gameName,
        Dictionary<string, object> placeholders,
        string state = "Default") =>
        Render(gameName, state, placeholders, static template => template.LargeAsset);

    public string UpdateLargeAssetText(
        string gameName,
        Dictionary<string, object> placeholders,
        string state = "Default") =>
        Render(gameName, state, placeholders, static template => template.LargeAssetText);

    public string UpdateSmallAsset(
        string gameName,
        Dictionary<string, object> placeholders,
        string state = "Default") =>
        Render(gameName, state, placeholders, static template => template.SmallAsset);

    public string UpdateSmallAssetText(
        string gameName,
        Dictionary<string, object> placeholders,
        string state = "Default") =>
        Render(gameName, state, placeholders, static template => template.SmallAssetText);

    public string UpdateButton1Text(
        string gameName,
        Dictionary<string, object> placeholders,
        string state = "Default") =>
        Render(gameName, state, placeholders, static template => template.Button1Text);

    public string UpdateButton1URL(
        string gameName,
        Dictionary<string, object> placeholders,
        string state = "Default") =>
        Render(gameName, state, placeholders, static template => template.Button1Url);

    public string UpdateButton2Text(
        string gameName,
        Dictionary<string, object> placeholders,
        string state = "Default") =>
        Render(gameName, state, placeholders, static template => template.Button2Text);

    public string UpdateButton2URL(
        string gameName,
        Dictionary<string, object> placeholders,
        string state = "Default") =>
        Render(gameName, state, placeholders, static template => template.Button2Url);

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        if (_watcher is not null)
        {
            _watcher.EnableRaisingEvents = false;
            _watcher.Changed -= ScheduleReload;
            _watcher.Created -= ScheduleReload;
            _watcher.Renamed -= ScheduleReload;
            _watcher.Dispose();
        }

        _reloadTimer?.Dispose();
    }

    internal static string ApplyPlaceholders(
        string? template,
        IReadOnlyDictionary<string, object> placeholders) =>
        TemplateRenderer.Render(template, placeholders);

    private string Render(
        string gameName,
        string state,
        IReadOnlyDictionary<string, object> placeholders,
        Func<PresenceTemplate, string?> selector)
    {
        if (!_games.TryGetValue(gameName, out var states))
        {
            return string.Empty;
        }

        if (!states.TryGetValue(state, out var template) &&
            !states.TryGetValue("Default", out template))
        {
            return string.Empty;
        }

        return TemplateRenderer.Render(selector(template), placeholders);
    }

    private void ScheduleReload(object sender, FileSystemEventArgs eventArgs) =>
        _reloadTimer?.Change(ReloadDelay, Timeout.InfiniteTimeSpan);

    private void LoadConfiguration()
    {
        if (_disposed || !File.Exists(_configPath))
        {
            return;
        }

        lock (_reloadLock)
        {
            try
            {
                using var stream = new FileStream(
                    _configPath,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.ReadWrite | FileShare.Delete);
                using var document = JsonDocument.Parse(stream, new JsonDocumentOptions
                {
                    AllowTrailingCommas = true,
                    CommentHandling = JsonCommentHandling.Skip
                });

                if (!document.RootElement.TryGetProperty("Games", out var gamesElement) ||
                    gamesElement.ValueKind != JsonValueKind.Object)
                {
                    throw new InvalidDataException("The configuration must contain a 'Games' object.");
                }

                var games = new Dictionary<string, FrozenDictionary<string, PresenceTemplate>>(
                    StringComparer.OrdinalIgnoreCase);

                foreach (var gameProperty in gamesElement.EnumerateObject())
                {
                    if (gameProperty.Value.ValueKind != JsonValueKind.Object)
                    {
                        continue;
                    }

                    var states = new Dictionary<string, PresenceTemplate>(StringComparer.OrdinalIgnoreCase);
                    foreach (var stateProperty in gameProperty.Value.EnumerateObject())
                    {
                        if (stateProperty.Value.ValueKind != JsonValueKind.Object)
                        {
                            continue;
                        }

                        states[stateProperty.Name] = PresenceTemplate.FromJson(stateProperty.Value);
                    }

                    games[gameProperty.Name] = states.ToFrozenDictionary(StringComparer.OrdinalIgnoreCase);
                }

                _games = games.ToFrozenDictionary(StringComparer.OrdinalIgnoreCase);
                AppLog.Information($"Loaded presence configuration '{_configPath}'.");
            }
            catch (Exception exception) when (exception is IOException or JsonException or InvalidDataException)
            {
                AppLog.Warning(
                    $"Presence configuration '{_configPath}' is invalid; the last valid version remains active.",
                    exception);
            }
        }
    }

    private static FrozenDictionary<string, FrozenDictionary<string, PresenceTemplate>> EmptyGames() =>
        new Dictionary<string, FrozenDictionary<string, PresenceTemplate>>(StringComparer.OrdinalIgnoreCase)
            .ToFrozenDictionary(StringComparer.OrdinalIgnoreCase);
}

internal sealed record PresenceTemplate(
    string? Details,
    string? State,
    string? LargeAsset,
    string? SmallAsset,
    string? LargeAssetText,
    string? SmallAssetText,
    string? Button1Text,
    string? Button1Url,
    string? Button2Text,
    string? Button2Url)
{
    public static PresenceTemplate FromJson(JsonElement element) => new(
        GetString(element, "Details"),
        GetString(element, "State"),
        GetString(element, "LargeAsset"),
        GetString(element, "SmallAsset"),
        GetString(element, "LargeAssetText"),
        GetString(element, "SmallAssetText"),
        GetString(element, "Button1Text"),
        GetString(element, "Button1URL"),
        GetString(element, "Button2Text"),
        GetString(element, "Button2URL"));

    private static string? GetString(JsonElement element, string name) =>
        element.TryGetProperty(name, out var value) && value.ValueKind == JsonValueKind.String
            ? value.GetString()
            : null;
}

internal static partial class TemplateRenderer
{
    [GeneratedRegex(@"\{\s*(?<name>[^{}]+?)\s*\}", RegexOptions.CultureInvariant)]
    private static partial Regex PlaceholderRegex();

    public static string Render(string? template, IReadOnlyDictionary<string, object> placeholders)
    {
        if (string.IsNullOrEmpty(template))
        {
            return string.Empty;
        }

        var normalized = placeholders is Dictionary<string, object> dictionary &&
                         dictionary.Comparer.Equals(StringComparer.OrdinalIgnoreCase)
            ? dictionary
            : new Dictionary<string, object>(placeholders, StringComparer.OrdinalIgnoreCase);

        return PlaceholderRegex().Replace(template, match =>
        {
            var name = match.Groups["name"].Value.Trim();
            return normalized.TryGetValue(name, out var value)
                ? Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty
                : match.Value;
        });
    }
}
