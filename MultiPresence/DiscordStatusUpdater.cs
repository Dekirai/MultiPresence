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
    { "CODE VEIN", new Dictionary<string, GameDetails>
        {
            { "Default", new GameDetails { Details = "Lv. {level} - Haze: {haze}", State = "{hp}/{maxhp} HP - {stamina}/{maxstamina} Stamina", LargeAsset = "logo", LargeAssetText = "CODE VEIN", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } }
        }
    },
    { "Crash Bandicoot 4: It's About Time", new Dictionary<string, GameDetails>
        {
            { "Default", new GameDetails { Details = "Wumpas: {wumpasmodern} - Deaths: {deaths}", State = "Crates: {currentcrates}/{maxcrates}", LargeAsset = "logo", LargeAssetText = "Crash Bandicoot 4: It's About Time", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } }
        }
    },
    { "Crash Bandicoot N. Sane Trilogy", new Dictionary<string, GameDetails>
        {
            { "Level", new GameDetails { Details = "Lives: {lives} - Crates: {currentcrates}/{maxcrates}", State = "Level: {level}", LargeAsset = "{gamelogo}", LargeAssetText = "Playing {game_long}", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } },
            { "Hub", new GameDetails { Details = "Lives: {lives}", State = "Roaming around in the Hub", LargeAsset = "{gamelogo}", LargeAssetText = "Playing {game_long}", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } }
        }
    },
    { "CRISIS CORE –FINAL FANTASY VII– REUNION", new Dictionary<string, GameDetails>
        {
            { "Default", new GameDetails { Details = "Lv. {level} ({difficulty})", State = "HP: {hp}/{maxhp} | MP: {mp}/{maxmp}", LargeAsset = "logo", LargeAssetText = "CRISIS CORE –FINAL FANTASY VII– REUNION", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } },
            { "Mission", new GameDetails { Details = "[In Mission] Lv. {level} ({difficulty})}", State = "HP: {hp_mission}/{maxhp_mission} | MP: {mp_mission}/{maxmp_mission}", LargeAsset = "logo", LargeAssetText = "CRISIS CORE –FINAL FANTASY VII– REUNION", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } }
        }
    },
    { "Dark Souls: Remastered", new Dictionary<string, GameDetails>
        {
            { "Default", new GameDetails { Details = "Lv. {level} - {class}", State = "{hp}/{maxhp} HP - {mp}/{maxmp} MP", LargeAsset = "logo", LargeAssetText = "Dark Souls: Remastered", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } }
        }
    },
    { "Dark Souls II", new Dictionary<string, GameDetails>
        {
            { "Default", new GameDetails { Details = "Lv. {level} - {class}", State = "{hp}/{maxhp} HP", LargeAsset = "logo", LargeAssetText = "Dark Souls II", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } }
        }
    },
    { "Dark Souls III", new Dictionary<string, GameDetails>
        {
            { "Default", new GameDetails { Details = "Lv. {level} - {class}", State = "{hp}/{maxhp} HP - {mp}/{maxmp} MP", LargeAsset = "logo", LargeAssetText = "Dark Souls III", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } }
        }
    },
    { "Death Stranding", new Dictionary<string, GameDetails>
        {
            { "Default", new GameDetails { Details = "Health: {health}/{maxhealth}", State = "Stamina: {stamina}/{maxstamina}", LargeAsset = "logo", LargeAssetText = "Death Stranding", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } }
        }
    },
    { "Devil May Cry", new Dictionary<string, GameDetails>
        {
            { "Default", new GameDetails { Details = "Mission {mission} ({difficulty})", State = "Health: {health}/{maxhealth}", LargeAsset = "logo", LargeAssetText = "Devil May Cry", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } },
            { "Pause Menu", new GameDetails { Details = "Mission {mission} ({difficulty})", State = "In Pause Menu", LargeAsset = "logo", LargeAssetText = "Devil May Cry", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } },
            { "Shop Menu", new GameDetails { Details = "Mission {mission} ({difficulty})", State = "In Shop Menu", LargeAsset = "logo", LargeAssetText = "Devil May Cry", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } },
            { "Mission End", new GameDetails { Details = "Mission {mission} ({difficulty})", State = "Mission is ending", LargeAsset = "logo", LargeAssetText = "Devil May Cry", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } },
            { "Mission Start", new GameDetails { Details = "Mission {mission} ({difficulty})", State = "Starting the next mission", LargeAsset = "logo", LargeAssetText = "Devil May Cry", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } }
        }
    },
    { "Devil May Cry 2", new Dictionary<string, GameDetails>
        {
            { "Default", new GameDetails { Details = "Mission {mission} ({difficulty})", State = "Health: {health}/{maxhealth}", LargeAsset = "logo", LargeAssetText = "Devil May Cry 2", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } },
            { "Bloody Palace", new GameDetails { Details = "Bloody Palace - Level {level}", State = "Health: {health}/{maxhealth}", LargeAsset = "logo", LargeAssetText = "Devil May Cry 2", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } }
        }
    },
    { "Devil May Cry 3", new Dictionary<string, GameDetails>
        {
            { "Default", new GameDetails { Details = "Mission {mission} ({difficulty})", State = "{character} - Health: {health}/{maxhealth}", LargeAsset = "logo", LargeAssetText = "Devil May Cry 3", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } },
            { "Bloody Palace", new GameDetails { Details = "Bloody Palace - Level {level}", State = "{character} - Health: {health}/{maxhealth}", LargeAsset = "logo", LargeAssetText = "Devil May Cry 3", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } },
            { "Cutscene", new GameDetails { Details = "Mission {mission} ({difficulty})", State = "In a cutscene", LargeAsset = "logo", LargeAssetText = "Devil May Cry 3", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } },
            { "Pause Menu", new GameDetails { Details = "Mission {mission} ({difficulty})", State = "In Pause Menu", LargeAsset = "logo", LargeAssetText = "Devil May Cry 3", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } },
            { "Bloody Palace Pause Menu", new GameDetails { Details = "Bloody Palace - Level {level}", State = "In Pause Menu", LargeAsset = "logo", LargeAssetText = "Devil May Cry 3", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } }
        }
    },
    { "Devil May Cry 4", new Dictionary<string, GameDetails>
        {
            { "Default", new GameDetails { Details = "Mission {mission} ({difficulty})", State = "{scenario} - Health: {health}", LargeAsset = "logo", LargeAssetText = "Devil May Cry 4", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } },
            { "Bloody Palace", new GameDetails { Details = "Bloody Palace - Level {level}", State = "{scenario} - Health: {health}/{maxhealth}", LargeAsset = "logo", LargeAssetText = "Devil May Cry 4", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } }
        }
    },
    { "Devil May Cry 5", new Dictionary<string, GameDetails>
        {
            { "Default", new GameDetails { Details = "Mission {mission} ({difficulty})", State = "{character} - Health: {health}/{maxhealth}", LargeAsset = "logo", LargeAssetText = "Devil May Cry 5", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } }
        }
    },
    { "DmC Devil May Cry", new Dictionary<string, GameDetails>
        {
            { "Default", new GameDetails { Details = "Mission {mission} ({difficulty})", State = "Health: {health}/{maxhealth}", LargeAsset = "logo", LargeAssetText = "DmC Devil May Cry", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } },
            { "Bloody Palace", new GameDetails { Details = "Bloody Palace - Level {level}", State = "Health: {health}/{maxhealth}", LargeAsset = "logo", LargeAssetText = "DmC Devil May Cry", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } }
        }
    },
    { "Elden Ring", new Dictionary<string, GameDetails>
        {
            { "Default", new GameDetails { Details = "Lv. {level} - {class}", State = "{hp}/{maxhp} HP - {mp}/{maxmp} MP", LargeAsset = "logo", LargeAssetText = "ELDEN RING", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } }
        }
    },
    { "Final Fantasy VII Remake", new Dictionary<string, GameDetails>
        {
            { "Default", new GameDetails { Details = "HP: {hp}/{maxhp} | MP: {mp}/{maxmp}", State = "Lv. {level} - Chapter {chapter}", LargeAsset = "logo", LargeAssetText = "Final Fantasy VII Remake", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } }
        }
    },
    { "Final Fantasy VII Rebirth", new Dictionary<string, GameDetails>
        {
            { "Default", new GameDetails { Details = "HP: {hp}/{maxhp} | MP: {mp}/{maxmp}", State = "Lv. {level} - Chapter {chapter}", LargeAsset = "logo", LargeAssetText = "Final Fantasy VII Rebirth", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } }
        }
    },
    { "Final Fantasy XV", new Dictionary<string, GameDetails>
        {
            { "Default", new GameDetails { Details = "HP: {hp} (Lv. {level})", State = "{story}", LargeAsset = "logo", LargeAssetText = "Final Fantasy XV", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } }
        }
    },
    { "Final Fantasy XVI", new Dictionary<string, GameDetails>
        {
            { "Default", new GameDetails { Details = "HP: {hp} (Lv. {level})", State = "Gil: {gil}", LargeAsset = "logo", LargeAssetText = "Final Fantasy XVI", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } }
        }
    },
    { "Kingdom Hearts Birth by Sleep Final Mix", new Dictionary<string, GameDetails>
        {
            { "Default", new GameDetails { Details = "Lv. {level} ({difficulty})", State = "{room}", LargeAsset = "{world_icon_name}", LargeAssetText = "{world}", SmallAsset = "{character_icon_name}", SmallAssetText = "Playing as {character}", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } },
            { "Battle", new GameDetails { Details = "Lv. {level} ({difficulty})", State = "HP: {hp}/{hpmax}", LargeAsset = "{world_icon_name}", LargeAssetText = "{world} - {room}", SmallAsset = "battle", SmallAssetText = "In Battle", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } }
        }
    },
    { "Kingdom Hearts Dream Drop Distance", new Dictionary<string, GameDetails>
        {
            { "Default", new GameDetails { Details = "Lv. {level} ({difficulty})", State = "{room}", LargeAsset = "{world_icon_name}", LargeAssetText = "{world}", SmallAsset = "{character_icon_name}", SmallAssetText = "Playing as {character}", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } }
        }
    },
    { "Kingdom Hearts Re:Chain of Memories", new Dictionary<string, GameDetails>
        {
            { "Default", new GameDetails { Details = "Lv. {level} ({difficulty})", State = "{world}", LargeAsset = "{world_icon_name}", LargeAssetText = "{world}", SmallAsset = "{character_icon_name}", SmallAssetText = "Playing as {character}", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } },
            { "Battle", new GameDetails { Details = "Lv. {level} ({difficulty})", State = "HP: {hp}/{hpmax}", LargeAsset = "{world_icon_name}", LargeAssetText = "{world}", SmallAsset = "battle", SmallAssetText = "In Battle", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } }
        }
    },
    { "Kingdom Hearts Final Mix", new Dictionary<string, GameDetails>
        {
            { "Default", new GameDetails { Details = "Lv. {level} ({difficulty})", State = "{room}", LargeAsset = "{world_icon_name}", LargeAssetText = "{world}", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } },
            { "Battle", new GameDetails { Details = "Lv. {level} ({difficulty})", State = "HP: {hp}/{hpmax} - MP: {mp}/{mpmax}", LargeAsset = "{world_icon_name}", LargeAssetText = "{world} - {room}", SmallAsset = "battle", SmallAssetText = "In Battle", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } }
        }
    },
    { "Kingdom Hearts II Final Mix", new Dictionary<string, GameDetails>
        {
            { "Default", new GameDetails { Details = "Lv. {level} ({difficulty})", State = "{room}", LargeAsset = "{world_icon_name}", LargeAssetText = "{world}", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } },
            { "FieldBattle", new GameDetails { Details = "Lv. {level} ({difficulty})", State = "HP: {hp}/{hpmax} - MP: {mp}/{mpmax}", LargeAsset = "{world_icon_name}", LargeAssetText = "{world} - {room}", SmallAsset = "field", SmallAssetText = "Field Battle", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } },
            { "BossBattle", new GameDetails { Details = "Lv. {level} ({difficulty})", State = "HP: {hp}/{hpmax} - MP: {mp}/{mpmax}", LargeAsset = "{world_icon_name}", LargeAssetText = "{world} - {room}", SmallAsset = "boss", SmallAssetText = "Boss Battle", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } }
        }
    },
    { "Kingdom Hearts III", new Dictionary<string, GameDetails>
        {
            { "World_Map", new GameDetails { Details = "Playing on {difficulty}", State = "{room}", LargeAsset = "", LargeAssetText = "", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } },
            { "Gummi_Ship", new GameDetails { Details = "Gummi Lv. {gummilevel} ({difficulty})", State = "{room}", LargeAsset = "", LargeAssetText = "", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } },
            { "In_World", new GameDetails { Details = "Lv. {level} ({difficulty})", State = "{room}", LargeAsset = "", LargeAssetText = "", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } }
        }
    },
    { "Lies of P", new Dictionary<string, GameDetails>
        {
            { "Default", new GameDetails { Details = "Lv. {level} - {deaths} Deaths", State = "{hp}/{maxhp} HP - {fable}/{maxfable} Fable", LargeAsset = "logo", LargeAssetText = "Lies of P", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } }
        }
    },
    { "Marvel's Spider-Man Remastered", new Dictionary<string, GameDetails>
        {
            { "Default", new GameDetails { Details = "Health: {health} (Level {level})", State = "Swinging in {location}", LargeAsset = "logo", LargeAssetText = "Marvel's Spider-Man Remastered", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } }
        }
    },
    { "Marvel's Spider-Man 2", new Dictionary<string, GameDetails>
        {
            { "Default", new GameDetails { Details = "Level {level}", State = "Swinging in {location}", LargeAsset = "logo", LargeAssetText = "Marvel's Spider-Man 2", SmallAsset = "{character_icon}", SmallAssetText = "Playing as {character}", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } }
        }
    },
    { "Marvel's Spider-Man: Miles Morales", new Dictionary<string, GameDetails>
        {
            { "Default", new GameDetails { Details = "Health: {health} (Level {level})", State = "Swinging in {location}", LargeAsset = "logo", LargeAssetText = "Marvel's Spider-Man: Miles Morales", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } }
        }
    },
    { "Mega Man 11", new Dictionary<string, GameDetails>
        {
            { "Default", new GameDetails { Details = "Bolts: {bolts} ({difficulty})", State = "In stage select", LargeAsset = "logo", LargeAssetText = "Mega Man 11", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } },
            { "Ingame", new GameDetails { Details = "Lives: {lives} ({difficulty})", State = "{stage}", LargeAsset = "{stage_icon_name}", LargeAssetText = "{stage}", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } }
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
            { "Ingame_Match", new GameDetails { Details = "{nickname} - {level}", State = "{mode} - H{currenthole}/{maxholes}", LargeAsset = "{map_icon_name}", LargeAssetText = "{map}", SmallAsset = "playing", SmallAssetText = "#{room} - {players}/{playersmax} Players", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } },
            { "Ingame_Tourney", new GameDetails { Details = "{nickname} - {level}", State = "{mode} - H{currenthole}/{maxholes} - Score: {score}", LargeAsset = "{map_icon_name}", LargeAssetText = "{map}", SmallAsset = "playing", SmallAssetText = "#{room} - {players}/{playersmax} Players", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } },
            { "Ingame_Lounge", new GameDetails { Details = "{nickname} - {level}", State = "{mode}", LargeAsset = "{map_icon_name}", LargeAssetText = "{map}", SmallAsset = "playing", SmallAssetText = "#{room} - {players}/{playersmax} Players", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } },
            { "Waiting_Room", new GameDetails { Details = "{nickname} - {level}", State = "{mode}", LargeAsset = "{map_icon_name}", LargeAssetText = "{map}", SmallAsset = "waiting", SmallAssetText = "#{room} - {players}/{playersmax} Players", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } },
            { "Ingame_Match_Private", new GameDetails { Details = "{nickname} - {level}", State = "{mode} - H{currenthole}/{maxholes}", LargeAsset = "{map_icon_name}", LargeAssetText = "{map}", SmallAsset = "playing", SmallAssetText = "Private Room", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } },
            { "Ingame_Tourney_Private", new GameDetails { Details = "{nickname} - {level}", State = "{mode} - H{currenthole}/{maxholes} - Score: {score}", LargeAsset = "{map_icon_name}", LargeAssetText = "{map}", SmallAsset = "playing", SmallAssetText = "Private Room", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } },
            { "Ingame_Lounge_Private", new GameDetails { Details = "{nickname} - {level}", State = "{mode}", LargeAsset = "{map_icon_name}", LargeAssetText = "", SmallAsset = "playing", SmallAssetText = "Private Room", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } },
            { "Waiting_Room_Private", new GameDetails { Details = "{nickname} - {level}", State = "{mode}", LargeAsset = "{map_icon_name}", LargeAssetText = "", SmallAsset = "waiting", SmallAssetText = "Private Room", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } },
            { "Lobby", new GameDetails { Details = "{nickname} - {level}", State = "In Lobby", LargeAsset = "logo", LargeAssetText = "PangYa Reborn", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } }
        }
    },
    { "Persona 4 Golden", new Dictionary<string, GameDetails>
        {
            { "Default", new GameDetails { Details = "Lv. {level}", State = "{currenttime} - {currentday}", LargeAsset = "logo", LargeAssetText = "Persona 4 Golden", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } },
            { "Battle", new GameDetails { Details = "In Battle", State = "Lv. {level} - HP: {hp} - SP: {sp}", LargeAsset = "logo", LargeAssetText = "Persona 4 Golden", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } }
        }
    },
    { "Persona 5 Royal", new Dictionary<string, GameDetails>
        {
            { "Default", new GameDetails { Details = "Lv. {level}", State = "{currenttime} - {currentday}", LargeAsset = "logo", LargeAssetText = "Persona 5 Royal", SmallAsset = "idle", SmallAssetText = "Idle", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } },
            { "Battle", new GameDetails { Details = "Persona: {persona}", State = "Lv. {level} - HP: {hp} - SP: {sp}", LargeAsset = "logo", LargeAssetText = "Persona 5 Royal", SmallAsset = "battle", SmallAssetText = "In Battle", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } }
        }
    },
    { "Persona 5 Strikers", new Dictionary<string, GameDetails>
        {
            { "Default", new GameDetails { Details = "Lv. {level}", State = "Money: {money}", LargeAsset = "logo", LargeAssetText = "Persona 5 Strikers", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } },
            { "Battle", new GameDetails { Details = "In Battle", State = "Lv. {level} - HP: {hp} - SP: {sp}", LargeAsset = "logo", LargeAssetText = "Persona 5 Strikers", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } }
        }
    },
    { "Persona 5: The Phantom X", new Dictionary<string, GameDetails>
        {
            { "Default", new GameDetails { Details = "ID: {uid}", State = "", LargeAsset = "logo", LargeAssetText = "Persona 5: The Phantom X", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } }
        }
    },
    { "Project Diva Mega Mix+", new Dictionary<string, GameDetails>
        {
            { "Default", new GameDetails { Details = "In Menu", State = "", LargeAsset = "logo", LargeAssetText = "Project Diva: Mega Mix+", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } },
            { "Ingame", new GameDetails { Details = "[{difficulty}] Score: {score} - Combo: {currentcombo}", State = "Song: {song}", LargeAsset = "logo", LargeAssetText = "Project Diva: Mega Mix+", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } }
        }
    },
    { "Resident Evil", new Dictionary<string, GameDetails>
        {
            { "Default", new GameDetails { Details = "{floor} - {room}", State = "Status: {healthstatus} - Weapon: {weapon}", LargeAsset = "logo", LargeAssetText = "Resident Evil", SmallAsset = "{character_icon}", SmallAssetText = "Playing as {character}", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } },
        }
    },
    { "Resident Evil 2", new Dictionary<string, GameDetails>
        {
            { "Default", new GameDetails { Details = "Weapon: {weapon} - Ammo: {ammo}", State = "Status: {healthstatus}", LargeAsset = "logo", LargeAssetText = "Resident Evil 2", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } },
        }
    },
    { "Resident Evil 4 (2005)", new Dictionary<string, GameDetails>
        {
            { "Title_Screen", new GameDetails { Details = "At the Title Screen", State = "", LargeAsset = "logo", LargeAssetText = "Resident Evil 4", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } },
            { "Main", new GameDetails { Details = "Weapon: {weapon}", State = "Chapter {chapter}: {room}", LargeAsset = "", LargeAssetText = "", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } },
            { "Assignment_Ada", new GameDetails { Details = "Weapon: {weapon}", State = "{room}", LargeAsset = "", LargeAssetText = "", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } },
            { "Separate_Ways", new GameDetails { Details = "Weapon: {weapon}", State = "Chapter {chapter}: {room}", LargeAsset = "", LargeAssetText = "", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } },
            { "Mercenaries", new GameDetails { Details = "Score: {score}", State = "Playing as '{character}' on '{room}'", LargeAsset = "", LargeAssetText = "", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } }
        }
    },
    { "Resident Evil 4", new Dictionary<string, GameDetails>
        {
            { "Main Menu", new GameDetails { Details = "At the Title Screen", State = "", LargeAsset = "logo", LargeAssetText = "Resident Evil 4", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } },
            { "Main Story", new GameDetails { Details = "{state} - {difficulty}", State = "Health: {healthpercentage}% - Chapter {chapter}", LargeAsset = "logo", LargeAssetText = "Resident Evil 4", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } },
            { "Separate Ways", new GameDetails { Details = "{state} - {difficulty}", State = "Health: {healthpercentage}% - Chapter {chapter}", LargeAsset = "logo", LargeAssetText = "Resident Evil 4", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } },
            { "Mercenaries", new GameDetails { Details = "{state} - {map}", State = "", LargeAsset = "{character} - Score: {score}", LargeAssetText = "", SmallAsset = "logo", SmallAssetText = "Resident Evil 4", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } }
        }
    },
    { "Resident Evil 5", new Dictionary<string, GameDetails>
        {
            { "Default", new GameDetails { Details = "{chapter} - {room}", State = "{item} - {ammo}/{pouch} Ammo", LargeAsset = "logo", LargeAssetText = "Resident Evil 5", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } }
        }
    },
    { "Resident Evil 6", new Dictionary<string, GameDetails>
        {
            { "Booting", new GameDetails { Details = "Starting the game...", State = "", LargeAsset = "logo", LargeAssetText = "Resident Evil 6", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } },
            { "Ingame", new GameDetails { Details = "{chapter}", State = "{room}", LargeAsset = "logo", LargeAssetText = "Resident Evil 6", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } },
            { "Main_Menu", new GameDetails { Details = "In Main Menu", State = "", LargeAsset = "logo", LargeAssetText = "Resident Evil 6", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } },
            { "Saving", new GameDetails { Details = "Saving the game...", State = "", LargeAsset = "logo", LargeAssetText = "Resident Evil 6", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } },
            { "Cutscene", new GameDetails { Details = "{chapter}", State = "In a cutscene", LargeAsset = "logo", LargeAssetText = "Resident Evil 6", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } },
        }
    },
    { "Resident Evil 7", new Dictionary<string, GameDetails>
        {
            { "Main Story", new GameDetails { Details = "{mode}", State = "Health: {healthstatus}", LargeAsset = "logo", LargeAssetText = "Resident Evil 7", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } },
            { "DLC Story", new GameDetails { Details = "{mode}", State = "Health: {healthstatus}", LargeAsset = "logo", LargeAssetText = "Resident Evil 7", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } },
            { "DLC Minigame", new GameDetails { Details = "{mode}", State = "Health: {healthstatus}", LargeAsset = "logo", LargeAssetText = "Resident Evil 7", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } }
        }
    },
    { "Resident Evil 8", new Dictionary<string, GameDetails>
        {
            { "Main Story", new GameDetails { Details = "Playing the Main Story", State = "Health: {healthstatus}", LargeAsset = "logo", LargeAssetText = "Resident Evil Village", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } },
            { "Shadow of Rose", new GameDetails { Details = "Playing Shadow of Rose", State = "Health: {healthstatus}", LargeAsset = "logo", LargeAssetText = "Resident Evil Village", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } },
            { "Mercenaries", new GameDetails { Details = "Mercenaries - {character}", State = "Health: {healthstatus} - Score: {score}", LargeAsset = "logo", LargeAssetText = "Resident Evil Village", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } }
        }
    },
    { "Resident Evil Revelations 2", new Dictionary<string, GameDetails>
        {
            { "Lobby", new GameDetails { Details = "Raid Mode: In Lobby", State = "{character} (Lv. {level})", LargeAsset = "logo", LargeAssetText = "Resident Evil Revelations 2", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } },
            { "Ingame", new GameDetails { Details = "Raid Mode: Mission {chapter}-0{mission}", State = "{character} (Lv. {level})", LargeAsset = "logo", LargeAssetText = "Resident Evil Revelations 2", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } }
        }
    },
    { "Scott Pilgrim vs The World", new Dictionary<string, GameDetails>
        {
            { "Default", new GameDetails { Details = "Lives: {lives} - Money: {money}", State = "Health: {health} - EXP: {totalexperience}", LargeAsset = "logo", LargeAssetText = "Scott Pilgrim vs The World", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } }
        }
    },
    { "Sonic Adventure DX", new Dictionary<string, GameDetails>
        {
            { "Adventure Field", new GameDetails { Details = "In Adventure Field", State = "{level}", LargeAsset = "{character_icon}", LargeAssetText = "Playing as {character}", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } },
            { "Level", new GameDetails { Details = "Rings: {rings} - Lives: {lives}", State = "{level}", LargeAsset = "{character_icon}", LargeAssetText = "Playing as {character}", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } }
        }
    },
    { "Sonic Adventure 2", new Dictionary<string, GameDetails>
        {
            { "Default", new GameDetails { Details = "In Main Menu", State = "", LargeAsset = "logo", LargeAssetText = "Sonic Adventure 2", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } },
            { "Ingame", new GameDetails { Details = "Rings: {rings} - Lives: {lives}", State = "{level}", LargeAsset = "{character_icon}", LargeAssetText = "Playing as {character}", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } }
        }
    },
        { "Stellar Blade", new Dictionary<string, GameDetails>
        {
            { "Default", new GameDetails { Details = "Health: {health}/{maxhealth}", State = "Skill Level {level}", LargeAsset = "logo", LargeAssetText = "Stellar Blade", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } }
        }
    },
    { "The Binding of Isaac: Rebirth", new Dictionary<string, GameDetails>
        {
            { "Default", new GameDetails { Details = "Time: {time} - Score: {score}", State = "🪙{coins}💣{bombs}🔑{keys}", LargeAsset = "logo", LargeAssetText = "", SmallAsset = "{characterasset}", SmallAssetText = "Playing as {character}", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } }
        }
    },
    { "The Witcher 3", new Dictionary<string, GameDetails>
        {
            { "Default", new GameDetails { Details = "Health: {health}/{maxhealth}", State = "Lv. {level} - {xp} XP", LargeAsset = "logo", LargeAssetText = "The Witcher 3: Wild Hunt", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } }
        }
    },
    { "TY The Tasmanian Tiger", new Dictionary<string, GameDetails>
        {
            { "Default", new GameDetails { Details = "Health: {health}", State = "", LargeAsset = "logo", LargeAssetText = "TY The Tasmanian Tiger", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } },
            { "Level", new GameDetails { Details = "Health: {health} - Opals: {opals}/300", State = "", LargeAsset = "logo", LargeAssetText = "TY The Tasmanian Tiger", SmallAsset = "", SmallAssetText = "", Button1Text = "", Button1URL = "", Button2Text = "", Button2URL = "" } },
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
