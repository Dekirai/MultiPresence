using MultiPresence;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

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
            { "Default", new GameDetails { Details = "{nickname} (Lv. {level})", State = "Playing as {character}", LargeAsset = "{character_icon_name}", LargeAssetText = "asobiSW", SmallAsset = "", SmallAssetText = "" } }
        }
    },
    { "CRISIS CORE –FINAL FANTASY VII– REUNION", new Dictionary<string, GameDetails>
        {
            { "Default", new GameDetails { Details = "Lv. {level} ({difficulty})", State = "HP: {hp}/{maxhp} | MP: {mp}/{maxmp}", LargeAsset = "logo", LargeAssetText = "CRISIS CORE –FINAL FANTASY VII– REUNION", SmallAsset = "", SmallAssetText = "" } },
            { "Mission", new GameDetails { Details = "[In Mission] Lv. {level} ({difficulty})}", State = "HP: {hp_mission}/{maxhp_mission} | MP: {mp_mission}/{maxmp_mission}", LargeAsset = "logo", LargeAssetText = "CRISIS CORE –FINAL FANTASY VII– REUNION", SmallAsset = "", SmallAssetText = "" } }
        }
    },
    { "Final Fantasy VII Remake", new Dictionary<string, GameDetails>
        {
            { "Default", new GameDetails { Details = "HP: {hp}/{maxhp} | MP: {mp}/{maxmp}", State = "Lv. {level}", LargeAsset = "logo", LargeAssetText = "Final Fantasy VII Remake", SmallAsset = "", SmallAssetText = "" } }
        }
    },
    { "Kingdom Hearts Birth by Sleep Final Mix", new Dictionary<string, GameDetails>
        {
            { "Default", new GameDetails { Details = "Lv. {level} ({difficulty})", State = "{room}", LargeAsset = "{world_icon_name}", LargeAssetText = "{world}", SmallAsset = "{character_icon_name}", SmallAssetText = "Playing as {character}" } }
        }
    },
    { "Kingdom Hearts Dream Drop Distance", new Dictionary<string, GameDetails>
        {
            { "Default", new GameDetails { Details = "Lv. {level} ({difficulty})", State = "{room}", LargeAsset = "{world_icon_name}", LargeAssetText = "{world}", SmallAsset = "{character_icon_name}", SmallAssetText = "Playing as {character}" } }
        }
    },
    { "Kingdom Hearts Final Mix", new Dictionary<string, GameDetails>
        {
            { "Default", new GameDetails { Details = "Lv. {level} ({difficulty})", State = "{room}", LargeAsset = "{world_icon_name}", LargeAssetText = "{world}", SmallAsset = "", SmallAssetText = "" } }
        }
    },
    { "Kingdom Hearts II Final Mix", new Dictionary<string, GameDetails>
        {
            { "Default", new GameDetails { Details = "Lv. {level} ({difficulty})", State = "{room}", LargeAsset = "{world_icon_name}", LargeAssetText = "{world}", SmallAsset = "", SmallAssetText = "" } }
        }
    },
    { "Kingdom Hearts III", new Dictionary<string, GameDetails>
        {
            { "World_Map", new GameDetails { Details = "Playing on {difficulty}", State = "{room}", LargeAsset = "", LargeAssetText = "", SmallAsset = "", SmallAssetText = "" } },
            { "Gummi_Ship", new GameDetails { Details = "Gummi Lv. {gummilevel} ({difficulty})", State = "{room}", LargeAsset = "", LargeAssetText = "", SmallAsset = "", SmallAssetText = "" } },
            { "In_World", new GameDetails { Details = "Lv. {level} ({difficulty})", State = "{room}", LargeAsset = "", LargeAssetText = "", SmallAsset = "", SmallAssetText = "" } }
        }
    },
    { "Marvel's Spider-Man Remastered", new Dictionary<string, GameDetails>
        {
            { "Default", new GameDetails { Details = "Health: {health} (Level {level})", State = "Swinging in {location}", LargeAsset = "logo", LargeAssetText = "Marvel's Spider-Man Remastered", SmallAsset = "", SmallAssetText = "" } }
        }
    },
    { "Marvel's Spider-Man: Miles Morales", new Dictionary<string, GameDetails>
        {
            { "Default", new GameDetails { Details = "Health: {health} (Level {level})", State = "Swinging in {location}", LargeAsset = "logo", LargeAssetText = "Marvel's Spider-Man: Miles Morales", SmallAsset = "", SmallAssetText = "" } }
        }
    },
    { "Mega Man 11", new Dictionary<string, GameDetails>
        {
            { "Default", new GameDetails { Details = "Lives: {lives} ({difficulty})", State = "{stage}", LargeAsset = "{stage_icon_name}", LargeAssetText = "{stage}", SmallAsset = "", SmallAssetText = "" } }
        }
    },
    { "Mega Man Battle Network 6", new Dictionary<string, GameDetails>
        {
            { "Default", new GameDetails { Details = "HP: {hp}/{maxhp}", State = "{location}", LargeAsset = "logo", LargeAssetText = "Mega Man Battle Network 6", SmallAsset = "", SmallAssetText = "" } },
            { "In_Battle", new GameDetails { Details = "HP: {hp_battle}/{maxhp_battle}", State = "{location} (In Battle)", LargeAsset = "logo", LargeAssetText = "Mega Man Battle Network 6", SmallAsset = "", SmallAssetText = "" } }
        }
    },
    { "Overwatch", new Dictionary<string, GameDetails>
        {
            { "Default", new GameDetails { Details = "", State = "", LargeAsset = "logo", LargeAssetText = "Overwatch 2", SmallAsset = "", SmallAssetText = "" } }
        }
    },
    { "Pangya Reborn", new Dictionary<string, GameDetails>
        {
            { "Ingame_Match", new GameDetails { Details = "{nickname} - {level}", State = "{mode} — H{currenthole}/{maxholes}", LargeAsset = "", LargeAssetText = "", SmallAsset = "", SmallAssetText = "" } },
            { "Ingame_Tourney", new GameDetails { Details = "{nickname} - {level}", State = "{mode} — H{currenthole}/{maxholes} — Score: {score}", LargeAsset = "", LargeAssetText = "", SmallAsset = "", SmallAssetText = "" } },
            { "Ingame_Lounge", new GameDetails { Details = "{nickname} - {level}", State = "{mode}", LargeAsset = "", LargeAssetText = "", SmallAsset = "", SmallAssetText = "" } },
            { "Waiting_Room", new GameDetails { Details = "{nickname} - {level}", State = "{mode}", LargeAsset = "", LargeAssetText = "", SmallAsset = "", SmallAssetText = "" } },
            { "Lobby", new GameDetails { Details = "{nickname} - {level}", State = "In Lobby", LargeAsset = "", LargeAssetText = "", SmallAsset = "", SmallAssetText = "" } }
        }
    },
    { "Resident Evil 4", new Dictionary<string, GameDetails>
        {
            { "Title_Screen", new GameDetails { Details = "At the Title Screen", State = "", LargeAsset = "", LargeAssetText = "", SmallAsset = "", SmallAssetText = "" } },
            { "Main", new GameDetails { Details = "Weapon: {weapon}", State = "Chapter {chapter}: {room}", LargeAsset = "", LargeAssetText = "", SmallAsset = "", SmallAssetText = "" } },
            { "Assignment_Ada", new GameDetails { Details = "Weapon: {weapon}", State = "{room}", LargeAsset = "", LargeAssetText = "", SmallAsset = "", SmallAssetText = "" } },
            { "Separate_Ways", new GameDetails { Details = "Weapon: {weapon}", State = "Chapter {chapter}: {room}", LargeAsset = "", LargeAssetText = "", SmallAsset = "", SmallAssetText = "" } },
            { "Mercenaries", new GameDetails { Details = "Score: {score}", State = "Playing as '{character}' on '{room}'", LargeAsset = "", LargeAssetText = "", SmallAsset = "", SmallAssetText = "" } }
        }
    },
    { "Resident Evil 5", new Dictionary<string, GameDetails>
        {
            { "Default", new GameDetails { Details = "{chapter}", State = "{room}", LargeAsset = "", LargeAssetText = "", SmallAsset = "", SmallAssetText = "" } }
        }
    },
    { "Resident Evil 6", new Dictionary<string, GameDetails>
        {
            { "Booting", new GameDetails { Details = "Starting the game...", State = "", LargeAsset = "", LargeAssetText = "", SmallAsset = "", SmallAssetText = "" } },
            { "Ingame", new GameDetails { Details = "{chapter}", State = "{room}", LargeAsset = "", LargeAssetText = "", SmallAsset = "", SmallAssetText = "" } },
            { "Main_Menu", new GameDetails { Details = "In Main Menu", State = "", LargeAsset = "", LargeAssetText = "", SmallAsset = "", SmallAssetText = "" } },
            { "Saving", new GameDetails { Details = "Saving the game...", State = "", LargeAsset = "", LargeAssetText = "", SmallAsset = "", SmallAssetText = "" } },
            { "Cutscene", new GameDetails { Details = "{chapter}", State = "In a cutscene", LargeAsset = "", LargeAssetText = "", SmallAsset = "", SmallAssetText = "" } },
        }
    },
    { "Resident Evil Revelations 2", new Dictionary<string, GameDetails>
        {
            { "Lobby", new GameDetails { Details = "Raid Mode: In Lobby", State = "{character} (Lv. {level})", LargeAsset = "", LargeAssetText = "", SmallAsset = "", SmallAssetText = "" } },
            { "Ingame", new GameDetails { Details = "Raid Mode: Mission {chapter}-0{mission}", State = "{character} (Lv. {level})", LargeAsset = "", LargeAssetText = "", SmallAsset = "", SmallAssetText = "" } }
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
            _config = new JObject();
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

    public string UpdateLargeAsset(string gameName, Dictionary<string, object> placeholders, string largeasset = "Default")
    {
        LoadConfig();

        if (_config["Games"]?[gameName] != null)
        {
            var gameConfig = (JObject)_config["Games"][gameName];

            if (!string.IsNullOrEmpty(largeasset) && gameConfig[largeasset] != null)
            {
                var largeassetConfig = (JObject)gameConfig[largeasset];
                var largeassetTemplate = (string)largeassetConfig["LargeAsset"];
                return ApplyPlaceholders(largeassetTemplate, placeholders);
            }
            else
            {
                var largeassetTemplate = (string)gameConfig["LargeAsset"];
                return ApplyPlaceholders(largeassetTemplate, placeholders);
            }
        }

        return DefaultGameDetails.TryGetValue(gameName, out var details) && details.TryGetValue(largeasset, out var gameDetails) ?
            ApplyPlaceholders(gameDetails.LargeAsset, placeholders) :
            $"LargeAsset not available for {gameName}";
    }

    public string UpdateLargeAssetText(string gameName, Dictionary<string, object> placeholders, string largeassettext = "Default")
    {
        LoadConfig();

        if (_config["Games"]?[gameName] != null)
        {
            var gameConfig = (JObject)_config["Games"][gameName];

            if (!string.IsNullOrEmpty(largeassettext) && gameConfig[largeassettext] != null)
            {
                var largeassettextConfig = (JObject)gameConfig[largeassettext];
                var largeassettextTemplate = (string)largeassettextConfig["LargeAssetText"];
                return ApplyPlaceholders(largeassettextTemplate, placeholders);
            }
            else
            {
                var largeassettextTemplate = (string)gameConfig["LargeAssetText"];
                return ApplyPlaceholders(largeassettextTemplate, placeholders);
            }
        }

        return DefaultGameDetails.TryGetValue(gameName, out var details) && details.TryGetValue(largeassettext, out var gameDetails) ?
            ApplyPlaceholders(gameDetails.LargeAssetText, placeholders) :
            $"LargeAssetText not available for {gameName}";
    }

    public string UpdateSmallAsset(string gameName, Dictionary<string, object> placeholders, string smallasset = "Default")
    {
        LoadConfig();

        if (_config["Games"]?[gameName] != null)
        {
            var gameConfig = (JObject)_config["Games"][gameName];

            if (!string.IsNullOrEmpty(smallasset) && gameConfig[smallasset] != null)
            {
                var smallassetConfig = (JObject)gameConfig[smallasset];
                var smallassetTemplate = (string)smallassetConfig["SmallAsset"];
                return ApplyPlaceholders(smallassetTemplate, placeholders);
            }
            else
            {
                var smallassetTemplate = (string)gameConfig["SmallAsset"];
                return ApplyPlaceholders(smallassetTemplate, placeholders);
            }
        }

        return DefaultGameDetails.TryGetValue(gameName, out var details) && details.TryGetValue(smallasset, out var gameDetails) ?
            ApplyPlaceholders(gameDetails.SmallAsset, placeholders) :
            $"SmallAsset not available for {gameName}";
    }

    public string UpdateSmallAssetText(string gameName, Dictionary<string, object> placeholders, string smallassettext = "Default")
    {
        LoadConfig();

        if (_config["Games"]?[gameName] != null)
        {
            var gameConfig = (JObject)_config["Games"][gameName];

            if (!string.IsNullOrEmpty(smallassettext) && gameConfig[smallassettext] != null)
            {
                var smallassettextConfig = (JObject)gameConfig[smallassettext];
                var smallassettextTemplate = (string)smallassettextConfig["SmallAssetText"];
                return ApplyPlaceholders(smallassettextTemplate, placeholders);
            }
            else
            {
                var smallassettextTemplate = (string)gameConfig["SmallAssetText"];
                return ApplyPlaceholders(smallassettextTemplate, placeholders);
            }
        }

        return DefaultGameDetails.TryGetValue(gameName, out var details) && details.TryGetValue(smallassettext, out var gameDetails) ?
            ApplyPlaceholders(gameDetails.SmallAssetText, placeholders) :
            $"SmallAssetText not available for {gameName}";
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
    public string LargeAsset { get; set; }
    public string SmallAsset { get; set; }
    public string LargeAssetText { get; set; }
    public string SmallAssetText { get; set; }
}
