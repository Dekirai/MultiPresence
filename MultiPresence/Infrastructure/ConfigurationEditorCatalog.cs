using System.Reflection;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace MultiPresence.Infrastructure;

internal sealed record EditorPlaceholder(string Name, string Description, string Example);
internal sealed record EditorAsset(string Key, string Description);

internal static class ConfigurationEditorCatalog
{
    private sealed record SourceEntry(bool Complete, bool Shared, IReadOnlyList<string> Keys);
    private sealed record Documentation(IReadOnlyList<EditorPlaceholder> Placeholders, IReadOnlyList<EditorAsset> Assets);

    private static readonly Assembly Assembly = typeof(ConfigurationEditorCatalog).Assembly;
    private static readonly Lazy<Dictionary<string, SourceEntry>> Sources = new(LoadSources);
    private static readonly Lazy<Dictionary<string, Documentation>> Documents = new(LoadDocuments);
    private static readonly Dictionary<string, string> DocumentAliases = new(StringComparer.OrdinalIgnoreCase)
    {
        ["CRISIS CORE –FINAL FANTASY VII– REUNION"] = "Crisis Core Final Fantasy VII",
        ["Dark Souls I"] = "Dark Souls Remastered",
        ["Dark Souls II"] = "Dark Souls II SotFS",
        ["Kingdom Hearts I"] = "Kingdom Hearts Final Mix",
        ["Kingdom Hearts II"] = "Kingdom Hearts II Final Mix",
        ["Kingdom Hearts Birth by Sleep"] = "Kingdom Hearts Birth by Sleep Final Mix",
        ["Overwatch"] = "Overwatch 2",
        ["Resident Evil 1"] = "Resident Evil",
        ["Resident Evil 2 Remake"] = "Resident Evil 2",
        ["Resident Evil 3 Remake"] = "Resident Evil 3",
        ["Resident Evil 4 Remake"] = "Resident Evil 4"
    };

    public static IReadOnlyList<EditorPlaceholder> GetPlaceholders(string gameName, string configName)
    {
        var source = GetSource(gameName, configName);
        var document = GetDocument(gameName, configName);
        var descriptions = document?.Placeholders.ToDictionary(item => item.Name, StringComparer.OrdinalIgnoreCase)
            ?? new Dictionary<string, EditorPlaceholder>(StringComparer.OrdinalIgnoreCase);
        IEnumerable<string> names;
        if (source?.Complete == true)
        {
            names = source.Keys;
        }
        else if (document != null && source?.Shared == true)
        {
            names = document.Placeholders.Select(item => item.Name);
        }
        else
        {
            names = (source?.Keys ?? Array.Empty<string>()).Concat(document?.Placeholders.Select(item => item.Name) ?? Enumerable.Empty<string>());
        }

        return names.Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(name => name, StringComparer.OrdinalIgnoreCase)
            .Select(name =>
            {
                string description = descriptions.TryGetValue(name, out var entry) ? entry.Description : "Available in this game's presence.";
                return new EditorPlaceholder(name, description, Example(name, description, gameName));
            })
            .ToArray();
    }

    public static IReadOnlyList<EditorAsset> GetAssets(string gameName, string configName)
    {
        var assets = new Dictionary<string, EditorAsset>(StringComparer.OrdinalIgnoreCase);
        foreach (var asset in GetDocument(gameName, configName)?.Assets ?? Array.Empty<EditorAsset>())
        {
            assets.TryAdd(asset.Key, asset);
        }
        string resource = "MultiPresence.DefaultConfig." + FileName(configName) + ".json";
        using var stream = Assembly.GetManifestResourceStream(resource);
        if (stream != null)
        {
            using var json = JsonDocument.Parse(stream);
            if (json.RootElement.TryGetProperty("Games", out var games) && games.ValueKind == JsonValueKind.Object)
            {
                foreach (var game in games.EnumerateObject())
                {
                    if (!game.Name.Equals(gameName, StringComparison.OrdinalIgnoreCase) || game.Value.ValueKind != JsonValueKind.Object)
                        continue;
                    foreach (var state in game.Value.EnumerateObject())
                    {
                        if (state.Value.ValueKind != JsonValueKind.Object)
                            continue;
                        foreach (string property in new[] { "LargeAsset", "SmallAsset" })
                        {
                            if (state.Value.TryGetProperty(property, out var value) && value.ValueKind == JsonValueKind.String &&
                                value.GetString() is { Length: > 0 } key && !key.Contains('{'))
                            {
                                assets.TryAdd(key, new EditorAsset(key, "Used by the default configuration."));
                            }
                        }
                    }
                }
            }
        }
        return assets.Values.OrderBy(asset => asset.Key, StringComparer.OrdinalIgnoreCase).ToArray();
    }

    public static bool IsComplete(string gameName, string configName) => GetSource(gameName, configName)?.Complete == true;

    private static SourceEntry? GetSource(string gameName, string configName)
    {
        Sources.Value.TryGetValue(Normalize(configName), out var entry);
        if (entry == null) Sources.Value.TryGetValue(Normalize(gameName), out entry);
        return entry;
    }

    private static Documentation? GetDocument(string gameName, string configName)
    {
        string file = FileName(configName);
        if (DocumentAliases.TryGetValue(file, out var alias) && Documents.Value.TryGetValue(Normalize(alias), out var aliased))
            return aliased;
        if (Documents.Value.TryGetValue(Normalize(gameName), out var game))
            return game;
        return Documents.Value.GetValueOrDefault(Normalize(file));
    }

    private static Dictionary<string, SourceEntry> LoadSources()
    {
        var result = new Dictionary<string, SourceEntry>(StringComparer.OrdinalIgnoreCase);
        using var stream = Assembly.GetManifestResourceStream("MultiPresence.EditorCatalog.xml");
        if (stream == null) return result;
        foreach (var element in XDocument.Load(stream).Root?.Elements("Configuration") ?? Enumerable.Empty<XElement>())
        {
            string name = (string?)element.Attribute("Name") ?? "";
            bool complete = bool.TryParse((string?)element.Attribute("Complete"), out var value) && value;
            bool shared = bool.TryParse((string?)element.Attribute("Shared"), out var sharedValue) && sharedValue;
            var keys = element.Elements("Placeholder").Select(item => (string?)item.Attribute("Name") ?? "")
                .Where(key => key.Length != 0).Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
            result[Normalize(name)] = new SourceEntry(complete, shared, keys);
        }
        return result;
    }

    private static Dictionary<string, Documentation> LoadDocuments()
    {
        const string prefix = "MultiPresence.Placeholders.";
        var result = new Dictionary<string, Documentation>(StringComparer.OrdinalIgnoreCase);
        foreach (string resource in Assembly.GetManifestResourceNames().Where(name => name.StartsWith(prefix, StringComparison.Ordinal)))
        {
            using var stream = Assembly.GetManifestResourceStream(resource);
            if (stream == null) continue;
            using var reader = new StreamReader(stream);
            var placeholders = new Dictionary<string, EditorPlaceholder>(StringComparer.OrdinalIgnoreCase);
            var assets = new Dictionary<string, EditorAsset>(StringComparer.OrdinalIgnoreCase);
            while (reader.ReadLine() is { } line)
            {
                var columns = line.Split('|');
                if (columns.Length >= 3)
                {
                    foreach (Match match in Regex.Matches(columns[1], @"\{([^{}]+)\}"))
                    {
                        string name = match.Groups[1].Value.Trim();
                        string description = columns[2].Trim();
                        if (columns.Length >= 4 && columns[3].Trim().Length > 0)
                            description += " (" + columns[3].Trim() + ")";
                        placeholders.TryAdd(name, new EditorPlaceholder(name, description, ""));
                    }
                }
                var asset = Regex.Match(line, @"^\s*([A-Za-z0-9_.-]+)\s*=\s*(.+?)\s*$");
                if (asset.Success)
                {
                    string key = asset.Groups[1].Value;
                    assets.TryAdd(key, new EditorAsset(key, asset.Groups[2].Value));
                }
            }
            result[Normalize(resource[prefix.Length..])] = new Documentation(placeholders.Values.ToArray(), assets.Values.ToArray());
        }
        return result;
    }

    private static string FileName(string value)
    {
        string name = Path.GetFileName(value);
        return name.EndsWith(".json", StringComparison.OrdinalIgnoreCase) ? name[..^5]
            : name.EndsWith(".md", StringComparison.OrdinalIgnoreCase) ? name[..^3] : name;
    }

    private static string Normalize(string value) => string.Concat(FileName(value).Where(char.IsLetterOrDigit)).ToLowerInvariant();

    private static string Example(string name, string description, string gameName)
    {
        string key = string.Concat(name.Where(char.IsLetterOrDigit)).ToLowerInvariant();
        string game = Normalize(gameName);
        if (game == "tekken8")
        {
            string? example = key switch
            {
                "mode" => "Ranked Match",
                "session" => "Online",
                "activity" => "Ranked Match | Online",
                "stage" => "Ortiz Farm",
                "stageid" => "27",
                "character" or "rightcharacter" => "Devil Jin",
                "opponent" or "leftcharacter" => "Lee",
                "rank" => "Beginner",
                "rankid" => "0",
                "side" => "Right",
                "score" => "2-1",
                "roundswon" or "rightrounds" => "2",
                "opponentrounds" or "leftrounds" => "1",
                "round" => "4",
                "roundstowin" => "3",
                "bestof" => "5",
                "matchup" => "Devil Jin (Beginner) vs Lee | 2-1",
                "matchdetails" => "Ranked Match | Online | Ortiz Farm",
                _ => null
            };
            if (example != null) return example;
        }
        if (game == "silenthill2")
        {
            switch (key)
            {
                case "area": return "Eastern South Vale";
                case "areaid": return "10";
                case "location": return "Eastern South Vale / Sanders St.";
                case "room": return "Sanders St.";
                case "combatdifficulty":
                case "puzzledifficulty": return "Standard";
                case "healthstatus": return "Healthy";
                case "weaponammo": return "Handgun (15/30)";
                case "weapon": return "Handgun";
            }
        }
        if (game == "silenthillf")
        {
            switch (key)
            {
                case "location": return "Shrine Path";
                case "areaid": return "Torii";
                case "objective": return "Find the correct ema (1/3)";
                case "objectivename": return "Find the correct ema";
                case "objectivecount": return "1";
                case "objectivemaxcount": return "3";
            }
        }
        if (description.StartsWith("Unavailable", StringComparison.OrdinalIgnoreCase)) return "";
        if (key.Contains("image") || key.Contains("icon") || key.Contains("logo") || key.EndsWith("asset")) return "logo";
        if (key == "level" && (game.StartsWith("crashbandicoot") || game.StartsWith("sonicadventure") || game is "croc2" or "tythetasmaniantiger")) return "Sample level";
        if (key == "players" && game == "labyrinthine") return " | 2/4 Players";
        if (key == "players" && game == "cryo") return " | 2 Players";
        if (key == "currenttime" && game.StartsWith("persona")) return "After School";
        if (key == "currentday") return "04/15 (Fri)";
        if (game == "legacyofkaindefianceremastered")
        {
            if (key == "reaverlevel") return "2";
            if (key == "reaverxp") return "250";
            if (key == "reaver") return "Spectral Reaver";
            if (key == "character") return "Raziel";
            if (key == "realm") return "Spectral Realm";
        }
        if (key == "time" && game is "sonicgenerations" or "shadowgenerations") return "00:46.22";
        if (key == "time" && game == "vampiresurvivors") return "12:34";
        if (key == "floor" && game is "residentevil" or "residentevil1" or "residentevil0") return "1F";
        if (game.Contains("bindingofisaac") && key == "floor") return "Basement I";
        if (game.Contains("bindingofisaac") && key == "difficulty") return "1";
        if (key == "exp" && game.StartsWith("ysi")) return "Level 10 (EXP: 250/1000)";
        if (key is "mission" or "missionid" && game.Contains("crisiscore")) return "M6-1-1";
        if (key == "chapter" && game == "residentevil42005") return "1-1";
        if (key == "weaponammo") return "Handgun | 15 / 30";
        if (key == "moneydisplay") return "Credits: 1250";
        if (key == "mods") return "Mods Enabled";
        if (key == "healthstatus") return "Fine";
        if (key == "healthstatusemoji") return "🟢";
        if (key == "battlestatus") return "In Battle";
        if (key == "session") return "Multiplayer (2/4)";
        if (key == "newgameplus") return "New Game Plus";
        if (key == "worldtime") return "Present Day";
        if (key is "progress") return "1/4";
        if (key == "objectivecount") return "1";
        if (key == "objectivemaxcount") return "4";
        if (key.EndsWith("percent") || key.EndsWith("percentage")) return key.StartsWith("exp") ? "25" : "75";
        if (key == "infiniteammo") return "False";
        if (key is "inbattle" or "isoverworld" or "endless" or "hurry" or "haseggs" or "playerturn" or "handsizelimit" ||
            Regex.IsMatch(key, @"^(pokemon[1-6]?|opponent)(active|egg|shiny)$")) return "True";
        if (key.EndsWith("guid")) return "00000000-0000-0000-0000-000000000001";
        if (key.EndsWith("ids")) return "1, 2, 3";
        if (key.EndsWith("id") || key.EndsWith("index")) return "1";
        if (key is "character" or "charactername" or "playername" or "trainername" or "nickname" or "name" or "mainchar") return "Player";
        if (key == "difficulty") return "Normal";
        if (key is "location" or "area" or "room" or "region" or "world" or "stage" or "map" or "subarea") return "Sample " + key;
        if (key is "weapon" or "currentweapon" or "item") return "Handgun";
        if (key is "time" or "playtime" or "currenttime") return "00:12:34";
        if (key is "state" or "gamestate") return "Ingame";
        if (key is "mode" or "gamemode" or "gameplaymode") return "Story";

        string metric = Regex.Replace(key, @"(battle|mission)$", "");
        metric = Regex.Replace(metric, @"^(pokemon[1-6]?|opponent|chris|sheva|current)", "");
        bool maximum = metric.StartsWith("max") || metric.EndsWith("max") || metric.Contains("maxcount");
        if (metric.StartsWith("max")) metric = metric[3..];
        if (metric.EndsWith("max")) metric = metric[..^3];
        if (metric is "hp" or "health" or "life") return game == "croc2" ? maximum ? "3" : "2" : maximum ? "100" : "75";
        if (metric is "mp" or "mana" or "ap" or "shield" or "stamina" or "energy" or "inchor") return maximum ? "100" : "75";
        if (metric is "beta" or "burst" or "tachy" or "limitbreak") return maximum ? "4" : "2";
        if (metric == "limitbreakraw") return "200";
        if (metric is "ep" or "exp" or "xp" or "experience" or "sp" or "wp" or "spexperience") return maximum ? "1000" : "250";
        if (metric is "exprequired" or "experiencerequired") return "1000";
        if (metric == "nextexp") return "750";
        if (metric is "totalexp" or "totalxp" or "totalexperience" or "experiencetotal") return "4250";
        if (metric is "ammo" or "loadedammo" or "ammoclip") return maximum ? "30" : "15";
        if (metric is "reserveammo" or "pouch") return "30";
        if (metric is "players" or "playercount") return maximum ? "4" : "2";
        if (metric is "crates") return maximum ? "104" : "24";
        if (metric is "holes" or "hole") return maximum ? "18" : "3";
        if (metric == "weapons" && maximum) return "6";
        if (metric.EndsWith("level") || metric is "level" or "slv") return "10";
        if (metric is "itempower") return "23";
        if (metric is "armor" or "def" or "defense" or "str" or "strength" or "speed" or "willpower" || metric.EndsWith("attackpower")) return "25";
        if (metric is "skillpoints" or "abilitypoints") return "5";
        if (metric is "money" or "gil" or "zenny" or "copper" or "gold" or "lei" or "souls" or "haze" or "dreamlight" or "redorbs" || metric.Contains("coin")) return "1250";
        if (metric.EndsWith("score") || metric is "score" or "battlepoints") return "4500";
        if (metric is "teamsize") return "6";
        if (metric is "remaining") return "5";
        if (metric is "fainted") return "1";
        if (metric is "pokedexseen") return "21";
        if (metric is "pokedexcaught") return "17";
        if (metric is "lives") return "5";
        if (metric is "redhearts" or "readhearts") return maximum ? "6" : "4";
        if (metric.EndsWith("count") || metric.EndsWith("combo") || metric.EndsWith("hearts") || metric.EndsWith("orbs") ||
            metric.EndsWith("tokens") || metric.EndsWith("upgrades") || metric.EndsWith("gobbos") || metric is "kills" or "deaths" or "keys" or "bombs" or "turn" or "round" or "winstreak") return maximum ? "6" : "3";
        if (metric.Contains("crystal") || metric.Contains("wumpa") || metric is "rings" or "soulessence" or "eridium" or "metals" or "opals" or "glory" or "tinks") return "25";
        if (metric is "chapter" or "floor" or "floornumber" or "stagetype" or "position") return "2";
        if (key == "startingmana") return "100";
        if (key.Contains("quest") || key.Contains("objective")) return "Sample objective";
        if (Regex.IsMatch(description, @"\b(amount|count|counter|number|points|gauge|bitmask)\b", RegexOptions.IgnoreCase) &&
            !Regex.IsMatch(description, @"\b(name|names|summary|status|key|keys|ID|IDs|type)\b", RegexOptions.IgnoreCase)) return maximum ? "100" : "25";
        return "Sample " + name;
    }
}
