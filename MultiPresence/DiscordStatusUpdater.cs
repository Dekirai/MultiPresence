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
    { "asobiSW", new Dictionary<string, GameDetails>
        {
            { "Default", new GameDetails { Details = "{nickname} (Lv. {level})", State = "Playing as {character}" } }
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
    { "Kingdom Hearts III", new Dictionary<string, GameDetails>
        {
            { "World_Map", new GameDetails { Details = "Playing on {difficulty}", State = "{room}" } },
            { "Gummi_Ship", new GameDetails { Details = "Gummi Lv. {gummilevel} ({difficulty})", State = "{room}" } },
            { "In_World", new GameDetails { Details = "Lv. {level} ({difficulty})", State = "{room}" } }
        }
    },
    { "Marvel's Spider-Man Remastered", new Dictionary<string, GameDetails>
        {
            { "Default", new GameDetails { Details = "Health: {health} (Level {level})", State = "Swinging in {location}" } }
        }
    },
    { "Marvel's Spider-Man: Miles Morales", new Dictionary<string, GameDetails>
        {
            { "Default", new GameDetails { Details = "Health: {health} (Level {level})", State = "Swinging in {location}" } }
        }
    },
    { "Mega Man 11", new Dictionary<string, GameDetails>
        {
            { "Default", new GameDetails { Details = "Lives: {lives} ({difficulty})", State = "{stage}" } }
        }
    },
    { "Mega Man Battle Network 6", new Dictionary<string, GameDetails>
        {
            { "Default", new GameDetails { Details = "HP: {hp}/{maxhp}", State = "{location}" } },
            { "In_Battle", new GameDetails { Details = "HP: {hp_battle}/{maxhp_battle}", State = "{location} (In Battle)" } }
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
    { "Resident Evil 5", new Dictionary<string, GameDetails>
        {
            { "Default", new GameDetails { Details = "{chapter}", State = "{room}" } }
        }
    },
    { "Resident Evil 6", new Dictionary<string, GameDetails>
        {
            { "Booting", new GameDetails { Details = "Starting the game...", State = "" } },
            { "Ingame", new GameDetails { Details = "{chapter}", State = "{room}" } },
            { "Main_Menu", new GameDetails { Details = "In Main Menu", State = "" } },
            { "Saving", new GameDetails { Details = "Saving the game...", State = "" } },
            { "Cutscene", new GameDetails { Details = "{chapter}", State = "In a cutscene" } },
        }
    },
    { "Resident Evil Revelations 2", new Dictionary<string, GameDetails>
        {
            { "Lobby", new GameDetails { Details = "Raid Mode: In Lobby", State = "{character} (Lv. {level})" } },
            { "Ingame", new GameDetails { Details = "Raid Mode: Mission {chapter}-0{mission}", State = "{character} (Lv. {level})" } }
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
