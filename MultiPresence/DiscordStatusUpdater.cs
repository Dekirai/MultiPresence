using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using System.Threading;
using MultiPresence;

public class DiscordStatusUpdater
{
    private readonly string _configPath;
    private readonly string _absoluteConfigPath;
    private JObject _config;
    private FileSystemWatcher _fileWatcher;
    private DateTime _lastReloadTime = DateTime.MinValue;
    private static readonly TimeSpan ReloadDebounce = TimeSpan.FromSeconds(1);

    private static readonly Dictionary<string, Dictionary<string, GameDetails>> DefaultGameDetails = new Dictionary<string, Dictionary<string, GameDetails>>
{
    { "Marvel's Spider-Man: Miles Morales", new Dictionary<string, GameDetails>
        {
            { "Default", new GameDetails { Details = "Health: {health} (Level {level})", State = "Swinging in {location}" } }
        }
    },
    { "Resident Evil 4", new Dictionary<string, GameDetails>
        {
            { "Title_Screen", new GameDetails { Details = "At the Title Screen", State = "" } },
            { "Main", new GameDetails { Details = "Weapon: {weapon}", State = "Chapter {chapter}: {room}" } },
            { "Assignment_Ada", new GameDetails { Details = "Weapon: {weapon}", State = "{room}" } },
            { "Separate_Ways", new GameDetails { Details = "Weapon: {weapon}", State = "Chapter {chapter}: {room}" } },
            { "Mercenaries", new GameDetails { Details = "Score: {score}", State = "Playing as '{character}' on '{room}'" } }
        }
    },
    { "Kingdom Hearts Birth by Sleep Final Mix", new Dictionary<string, GameDetails>
        {
            { "Default", new GameDetails { Details = "Lv. {level} ({difficulty})", State = "{room}" } }
        }
    },
    { "Kingdom Hearts Dream Drop Distance", new Dictionary<string, GameDetails>
        {
            { "Default", new GameDetails { Details = "Lv. {level} ({difficulty})", State = "{room}" } }
        }
    },
    { "Kingdom Hearts Final Mix", new Dictionary<string, GameDetails>
        {
            { "Default", new GameDetails { Details = "Lv. {level} ({difficulty})", State = "{room}" } }
        }
    },
    { "Kingdom Hearts II Final Mix", new Dictionary<string, GameDetails>
        {
            { "Default", new GameDetails { Details = "Lv. {level} ({difficulty})", State = "{room}" } }
        }
    },
    { "Pangya Reborn", new Dictionary<string, GameDetails>
        {
            { "Ingame_Match", new GameDetails { Details = "{nickname} - {level}", State = "{mode} — H{currenthole}/{maxholes}" } },
            { "Ingame_Tourney", new GameDetails { Details = "{nickname} - {level}", State = "{mode} — H{currenthole}/{maxholes} — Score: {score}" } },
            { "Ingame_Lounge", new GameDetails { Details = "{nickname} - {level}", State = "{mode}" } },
            { "Waiting_Room", new GameDetails { Details = "{nickname} - {level}", State = "{mode}" } },
            { "Lobby", new GameDetails { Details = "{nickname} - {level}", State = "In Lobby" } }
        }
    }
};

    public DiscordStatusUpdater(string configPath)
    {
        _configPath = configPath;
        _absoluteConfigPath = Path.GetFullPath(configPath);
        ValidateConfigPath();
        LoadConfig();
        InitializeFileWatcher();
    }

    private void ValidateConfigPath()
    {
        if (string.IsNullOrWhiteSpace(_absoluteConfigPath))
        {
            throw new ArgumentException("Config path cannot be null or whitespace.");
        }

        if (!File.Exists(_absoluteConfigPath))
        {
            throw new FileNotFoundException($"Config file not found: {_absoluteConfigPath}");
        }

        var directory = Path.GetDirectoryName(_absoluteConfigPath);
        if (string.IsNullOrEmpty(directory) || !Directory.Exists(directory))
        {
            throw new DirectoryNotFoundException($"Directory for config path not found: {directory}");
        }
    }

    private void LoadConfig()
    {
        try
        {
            if (File.Exists(_absoluteConfigPath))
            {
                var configLoader = new ConfigLoader(_absoluteConfigPath);
                _config = configLoader.GetConfig();
            }
            else
            {
                throw new FileNotFoundException($"Config file not found: {_absoluteConfigPath}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading config: {ex.Message}");
        }
    }

    private void InitializeFileWatcher()
    {
        var directory = Path.GetDirectoryName(_absoluteConfigPath);
        if (string.IsNullOrEmpty(directory) || !Directory.Exists(directory))
        {
            throw new DirectoryNotFoundException($"Directory for config path not found: {directory}");
        }

        _fileWatcher = new FileSystemWatcher(directory)
        {
            Filter = Path.GetFileName(_absoluteConfigPath),
            NotifyFilter = NotifyFilters.LastWrite
        };
        _fileWatcher.Changed += OnConfigFileChanged;
        _fileWatcher.EnableRaisingEvents = true;
    }

    private void OnConfigFileChanged(object sender, FileSystemEventArgs e)
    {
        if (DateTime.Now - _lastReloadTime > ReloadDebounce)
        {
            _lastReloadTime = DateTime.Now;
            LoadConfig();
        }
    }

    public string UpdateDetails(string gameName, Dictionary<string, object> placeholders, string state = "Default")
    {
        LoadConfig();

        if (_config["Games"]?[gameName] != null)
        {
            var gameConfig = (JObject)_config["Games"][gameName];

            if (!string.IsNullOrEmpty(state) && gameConfig[state] != null)
            {
                var stateConfig = (JObject)gameConfig[state];
                var detailsTemplate = (string)stateConfig["Details"];
                return ApplyPlaceholders(detailsTemplate, placeholders);
            }
            else
            {
                var detailsTemplate = (string)gameConfig["Details"];
                return ApplyPlaceholders(detailsTemplate, placeholders);
            }
        }

        return DefaultGameDetails.TryGetValue(gameName, out var details) && details.TryGetValue(state, out var gameDetails) ?
            ApplyPlaceholders(gameDetails.Details, placeholders) :
            $"Details not available for {gameName}";
    }

    public string UpdateState(string gameName, Dictionary<string, object> placeholders, string state = "Default")
    {
        LoadConfig();

        if (_config["Games"]?[gameName] != null)
        {
            var gameConfig = (JObject)_config["Games"][gameName];

            if (!string.IsNullOrEmpty(state) && gameConfig[state] != null)
            {
                var stateConfig = (JObject)gameConfig[state];
                var stateTemplate = (string)stateConfig["State"];
                return ApplyPlaceholders(stateTemplate, placeholders);
            }
            else
            {
                var stateTemplate = (string)gameConfig["State"];
                return ApplyPlaceholders(stateTemplate, placeholders);
            }
        }

        return DefaultGameDetails.TryGetValue(gameName, out var details) && details.TryGetValue(state, out var gameDetails) ?
            ApplyPlaceholders(gameDetails.State, placeholders) :
            $"State not available for {gameName}";
    }

    private string ApplyPlaceholders(string template, Dictionary<string, object> placeholders)
    {
        if (template == null) return string.Empty;

        foreach (var placeholder in placeholders)
        {
            template = Regex.Replace(template, $@"\{{\s*{placeholder.Key}\s*\}}", placeholder.Value.ToString());
        }
        return template;
    }
}

public class GameDetails
{
    public string Details { get; set; }
    public string State { get; set; }
}
