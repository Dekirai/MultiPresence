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
    { "Call of Duty®", new Dictionary<string, GameDetails>
        {
            { "Default", new GameDetails { Details = "", State = "", LargeAsset = "logo", LargeAssetText = "Call of Duty®", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } }
        }
    },
    { "CRISIS CORE –FINAL FANTASY VII– REUNION", new Dictionary<string, GameDetails>
        {
            { "Default", new GameDetails { Details = "Lv. {level} ({difficulty})", State = "HP: {hp}/{maxhp} | MP: {mp}/{maxmp}", LargeAsset = "logo", LargeAssetText = "CRISIS CORE –FINAL FANTASY VII– REUNION", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } },
            { "Mission", new GameDetails { Details = "[In Mission] Lv. {level} ({difficulty})}", State = "HP: {hp_mission}/{maxhp_mission} | MP: {mp_mission}/{maxmp_mission}", LargeAsset = "logo", LargeAssetText = "CRISIS CORE –FINAL FANTASY VII– REUNION", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } }
        }
    },
    { "Final Fantasy VII Remake", new Dictionary<string, GameDetails>
        {
            { "Default", new GameDetails { Details = "HP: {hp}/{maxhp} | MP: {mp}/{maxmp}", State = "Lv. {level}", LargeAsset = "logo", LargeAssetText = "Final Fantasy VII Remake", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } }
        }
    },
    { "Final Fantasy XVI", new Dictionary<string, GameDetails>
        {
            { "Default", new GameDetails { Details = "HP: {hp} (Lv. {level})", State = "Gil: {gil}", LargeAsset = "logo", LargeAssetText = "Final Fantasy XVI", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } }
        }
    },
    { "Hogwarts Legacy", new Dictionary<string, GameDetails>
        {
            { "Default", new GameDetails { Details = "", State = "", LargeAsset = "logo", LargeAssetText = "Hogwarts Legacy", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } }
        }
    },
    { "Kingdom Hearts Birth by Sleep Final Mix", new Dictionary<string, GameDetails>
        {
            { "Default", new GameDetails { Details = "Lv. {level} ({difficulty})", State = "{room}", LargeAsset = "{world_icon_name}", LargeAssetText = "{world}", SmallAsset = "{character_icon_name}", SmallAssetText = "Playing as {character}", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } }
        }
    },
    { "Kingdom Hearts Dream Drop Distance", new Dictionary<string, GameDetails>
        {
            { "Default", new GameDetails { Details = "Lv. {level} ({difficulty})", State = "{room}", LargeAsset = "{world_icon_name}", LargeAssetText = "{world}", SmallAsset = "{character_icon_name}", SmallAssetText = "Playing as {character}", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } }
        }
    },
    { "Kingdom Hearts Final Mix", new Dictionary<string, GameDetails>
        {
            { "Default", new GameDetails { Details = "Lv. {level} ({difficulty})", State = "{room}", LargeAsset = "{world_icon_name}", LargeAssetText = "{world}", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } }
        }
    },
    { "Kingdom Hearts II Final Mix", new Dictionary<string, GameDetails>
        {
            { "Default", new GameDetails { Details = "Lv. {level} ({difficulty})", State = "{room}", LargeAsset = "{world_icon_name}", LargeAssetText = "{world}", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } }
        }
    },
    { "Kingdom Hearts III", new Dictionary<string, GameDetails>
        {
            { "World_Map", new GameDetails { Details = "Playing on {difficulty}", State = "{room}", LargeAsset = "", LargeAssetText = "", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } },
            { "Gummi_Ship", new GameDetails { Details = "Gummi Lv. {gummilevel} ({difficulty})", State = "{room}", LargeAsset = "", LargeAssetText = "", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } },
            { "In_World", new GameDetails { Details = "Lv. {level} ({difficulty})", State = "{room}", LargeAsset = "", LargeAssetText = "", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } }
        }
    },
    { "Marvel's Spider-Man Remastered", new Dictionary<string, GameDetails>
        {
            { "Default", new GameDetails { Details = "Health: {health} (Level {level})", State = "Swinging in {location}", LargeAsset = "logo", LargeAssetText = "Marvel's Spider-Man Remastered", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } }
        }
    },
    { "Marvel's Spider-Man: Miles Morales", new Dictionary<string, GameDetails>
        {
            { "Default", new GameDetails { Details = "Health: {health} (Level {level})", State = "Swinging in {location}", LargeAsset = "logo", LargeAssetText = "Marvel's Spider-Man: Miles Morales", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } }
        }
    },
    { "Mega Man 11", new Dictionary<string, GameDetails>
        {
            { "Default", new GameDetails { Details = "Lives: {lives} ({difficulty})", State = "{stage}", LargeAsset = "{stage_icon_name}", LargeAssetText = "{stage}", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } }
        }
    },
    { "Mega Man Battle Network 6", new Dictionary<string, GameDetails>
        {
            { "Default", new GameDetails { Details = "HP: {hp}/{maxhp}", State = "{location}", LargeAsset = "logo", LargeAssetText = "Mega Man Battle Network 6", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } },
            { "In_Battle", new GameDetails { Details = "HP: {hp_battle}/{maxhp_battle}", State = "{location} (In Battle)", LargeAsset = "logo", LargeAssetText = "Mega Man Battle Network 6", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } }
        }
    },
    { "Pangya Reborn", new Dictionary<string, GameDetails>
        {
            { "Ingame_Match", new GameDetails { Details = "{nickname} - {level}", State = "{mode} — H{currenthole}/{maxholes}", LargeAsset = "", LargeAssetText = "", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } },
            { "Ingame_Tourney", new GameDetails { Details = "{nickname} - {level}", State = "{mode} — H{currenthole}/{maxholes} — Score: {score}", LargeAsset = "", LargeAssetText = "", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } },
            { "Ingame_Lounge", new GameDetails { Details = "{nickname} - {level}", State = "{mode}", LargeAsset = "", LargeAssetText = "", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } },
            { "Waiting_Room", new GameDetails { Details = "{nickname} - {level}", State = "{mode}", LargeAsset = "", LargeAssetText = "", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } },
            { "Lobby", new GameDetails { Details = "{nickname} - {level}", State = "In Lobby", LargeAsset = "", LargeAssetText = "", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } }
        }
    },
    { "Resident Evil", new Dictionary<string, GameDetails>
        {
            { "Default", new GameDetails { Details = "{floor}", State = "{room}", LargeAsset = "", LargeAssetText = "", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } },
        }
    },
    { "Resident Evil 4 (2005)", new Dictionary<string, GameDetails>
        {
            { "Title_Screen", new GameDetails { Details = "At the Title Screen", State = "", LargeAsset = "", LargeAssetText = "", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } },
            { "Main", new GameDetails { Details = "Weapon: {weapon}", State = "Chapter {chapter}: {room}", LargeAsset = "", LargeAssetText = "", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } },
            { "Assignment_Ada", new GameDetails { Details = "Weapon: {weapon}", State = "{room}", LargeAsset = "", LargeAssetText = "", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } },
            { "Separate_Ways", new GameDetails { Details = "Weapon: {weapon}", State = "Chapter {chapter}: {room}", LargeAsset = "", LargeAssetText = "", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } },
            { "Mercenaries", new GameDetails { Details = "Score: {score}", State = "Playing as '{character}' on '{room}'", LargeAsset = "", LargeAssetText = "", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } }
        }
    },
    { "Resident Evil 5", new Dictionary<string, GameDetails>
        {
            { "Default", new GameDetails { Details = "{chapter}", State = "{room}", LargeAsset = "", LargeAssetText = "", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } }
        }
    },
    { "Resident Evil 6", new Dictionary<string, GameDetails>
        {
            { "Booting", new GameDetails { Details = "Starting the game...", State = "", LargeAsset = "", LargeAssetText = "", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } },
            { "Ingame", new GameDetails { Details = "{chapter}", State = "{room}", LargeAsset = "", LargeAssetText = "", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } },
            { "Main_Menu", new GameDetails { Details = "In Main Menu", State = "", LargeAsset = "", LargeAssetText = "", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } },
            { "Saving", new GameDetails { Details = "Saving the game...", State = "", LargeAsset = "", LargeAssetText = "", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } },
            { "Cutscene", new GameDetails { Details = "{chapter}", State = "In a cutscene", LargeAsset = "", LargeAssetText = "", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } },
        }
    },
    { "Resident Evil Revelations 2", new Dictionary<string, GameDetails>
        {
            { "Lobby", new GameDetails { Details = "Raid Mode: In Lobby", State = "{character} (Lv. {level})", LargeAsset = "", LargeAssetText = "", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } },
            { "Ingame", new GameDetails { Details = "Raid Mode: Mission {chapter}-0{mission}", State = "{character} (Lv. {level})", LargeAsset = "", LargeAssetText = "", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } }
        }
    },
    { "Vampire Survivors", new Dictionary<string, GameDetails>
        {
            { "Default", new GameDetails { Details = "In Main Menu", State = "Coins: {coins}", LargeAsset = "logo", LargeAssetText = "Vampire Survivors", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } },
            { "Default_Adventure", new GameDetails { Details = "In Main Menu", State = "{adventure} - Coins: {coinsadventure}", LargeAsset = "logo", LargeAssetText = "Vampire Survivors", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } },
            { "Ingame", new GameDetails { Details = "{character} - Lv. {level} - Coins: {coinsingame}", State = "🕑{time} {stage} - Kills: {kills}", LargeAsset = "logo", LargeAssetText = "Vampire Survivors", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } },
            { "Ingame_Adventure", new GameDetails { Details = "{characteradventure} - Lv. {level} - Coins: {coinsingameadventure}", State = "🕑{time} {stageadventure} - Kills: {killsadventure}", LargeAsset = "logo", LargeAssetText = "Vampire Survivors", SmallAsset = "book", SmallAssetText = "Playing {adventure}", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } },
        }
    },
    { "Visions of Mana", new Dictionary<string, GameDetails>
        {
            { "Default", new GameDetails { Details = "Lv. {level} ({difficulty})", State = "HP: {hp}/{hpmax} | MP: {mp}/{mpmax}", LargeAsset = "logo", LargeAssetText = "Visions of Mana", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } }
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
    public string UpdateButton1Text(string gameName, Dictionary<string, object> placeholders, string button1text = "Default")
    {
        LoadConfig();

        if (_config["Games"]?[gameName] != null)
        {
            var gameConfig = (JObject)_config["Games"][gameName];

            if (!string.IsNullOrEmpty(button1text) && gameConfig[button1text] != null)
            {
                var button1textConfig = (JObject)gameConfig[button1text];
                var button1textTemplate = (string)button1textConfig["Button1Text"];
                return ApplyPlaceholders(button1textTemplate, placeholders);
            }
            else
            {
                var button1textTemplate = (string)gameConfig["Button1Text"];
                return ApplyPlaceholders(button1textTemplate, placeholders);
            }
        }

        return DefaultGameDetails.TryGetValue(gameName, out var details) && details.TryGetValue(button1text, out var gameDetails) ?
            ApplyPlaceholders(gameDetails.Button1Text, placeholders) :
            $"button1Text not available for {gameName}";
    }

    public string UpdateButton2Text(string gameName, Dictionary<string, object> placeholders, string button2text = "Default")
    {
        LoadConfig();

        if (_config["Games"]?[gameName] != null)
        {
            var gameConfig = (JObject)_config["Games"][gameName];

            if (!string.IsNullOrEmpty(button2text) && gameConfig[button2text] != null)
            {
                var button2textConfig = (JObject)gameConfig[button2text];
                var button2textTemplate = (string)button2textConfig["Button2Text"];
                return ApplyPlaceholders(button2textTemplate, placeholders);
            }
            else
            {
                var button2textTemplate = (string)gameConfig["Button2Text"];
                return ApplyPlaceholders(button2textTemplate, placeholders);
            }
        }

        return DefaultGameDetails.TryGetValue(gameName, out var details) && details.TryGetValue(button2text, out var gameDetails) ?
            ApplyPlaceholders(gameDetails.Button2Text, placeholders) :
            $"button2Text not available for {gameName}";
    }

    public string UpdateButton1URL(string gameName, Dictionary<string, object> placeholders, string button1url = "Default")
    {
        LoadConfig();

        if (_config["Games"]?[gameName] != null)
        {
            var gameConfig = (JObject)_config["Games"][gameName];

            if (!string.IsNullOrEmpty(button1url) && gameConfig[button1url] != null)
            {
                var button1urlConfig = (JObject)gameConfig[button1url];
                var button1urlTemplate = (string)button1urlConfig["Button1URL"];
                return ApplyPlaceholders(button1urlTemplate, placeholders);
            }
            else
            {
                var button1urlTemplate = (string)gameConfig["Button1URL"];
                return ApplyPlaceholders(button1urlTemplate, placeholders);
            }
        }

        return DefaultGameDetails.TryGetValue(gameName, out var details) && details.TryGetValue(button1url, out var gameDetails) ?
            ApplyPlaceholders(gameDetails.Button1URL, placeholders) :
            $"button1url not available for {gameName}";
    }

    public string UpdateButton2URL(string gameName, Dictionary<string, object> placeholders, string button2url = "Default")
    {
        LoadConfig();

        if (_config["Games"]?[gameName] != null)
        {
            var gameConfig = (JObject)_config["Games"][gameName];

            if (!string.IsNullOrEmpty(button2url) && gameConfig[button2url] != null)
            {
                var button2urlConfig = (JObject)gameConfig[button2url];
                var button2urlTemplate = (string)button2urlConfig["Button2URL"];
                return ApplyPlaceholders(button2urlTemplate, placeholders);
            }
            else
            {
                var button2urlTemplate = (string)gameConfig["Button2URL"];
                return ApplyPlaceholders(button2urlTemplate, placeholders);
            }
        }

        return DefaultGameDetails.TryGetValue(gameName, out var details) && details.TryGetValue(button2url, out var gameDetails) ?
            ApplyPlaceholders(gameDetails.Button2URL, placeholders) :
            $"button2url not available for {gameName}";
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
    public string? Details { get; set; }
    public string? State { get; set; }
    public string? LargeAsset { get; set; }
    public string? SmallAsset { get; set; }
    public string? LargeAssetText { get; set; }
    public string? SmallAssetText { get; set; }
    public string? Button1Text { get; set; }
    public string? Button1URL { get; set; }
    public string? Button2Text { get; set; }
    public string? Button2URL { get; set; }
}
