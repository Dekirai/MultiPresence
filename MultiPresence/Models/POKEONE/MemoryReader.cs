using System.Diagnostics;
using System.Text;

namespace MultiPresence.Models.POKEONE
{
    internal static partial class MemoryReader
    {
        private static readonly Dictionary<string, ulong> addresses = new Dictionary<string, ulong>();
        private static readonly Dictionary<ulong, Dictionary<string, int>> layouts = new Dictionary<ulong, Dictionary<string, int>>();
        private static int processId, moduleSize;
        private static ulong moduleBase;
        private static DateTime nextScan;

        public static void Reset()
        {
            addresses.Clear();
            layouts.Clear();
            processId = moduleSize = 0;
            moduleBase = 0;
            nextScan = DateTime.MinValue;
        }

        public static GameSnapshot ReadSnapshot()
        {
            ResolveAddresses();
            var memory = new FrameRead();
            var snapshot = ReadFrame(memory);
            memory.Validate();
            return snapshot;
        }

        private static GameSnapshot ReadFrame(FrameRead m)
        {
            var player = Singleton(m, "PlayerHandler", 4,
                ("Username", 0x2C), ("LoggedIn", 0x30), ("Level", 0x48));
            if (player == 0 || !m.Bool(player + 0x30))
            {
                return new GameSnapshot();
            }

            var map = Singleton(m, "MapManager", 0,
                ("Map", 0x10), ("Loading", 0x18), ("LoadingMap", 0x1A0), ("MapLabel", 0x1C8));
            if (map == 0 || m.Bool(map + 0x18) || m.Bool(map + 0x1A0))
            {
                return Waiting();
            }

            var playerName = m.String(m.Pointer(player + 0x2C));
            var level = m.Pointer(player + 0x48);
            var label = m.Pointer(map + 0x1C8);
            var mapData = m.Pointer(map + 0x10);
            if (playerName.Length == 0 || level == 0 || label == 0 || mapData == 0)
            {
                return Waiting();
            }

            m.Object(level, "Level", "PSXAPI.Response", ("<UserLevel>k__BackingField", 8));
            var trainerLevel = m.Int(level + 8);
            if (trainerLevel is < 1 or > 10000)
            {
                throw Invalid("trainer level");
            }

            m.Object(label, "UILabel", "", ("mText", 0x130));
            m.RequiredPointer(label + 8);
            var location = m.String(m.Pointer(label + 0x130));
            m.Object(mapData, "MapDump", "MAPAPI.Response", ("Settings", 0x40));
            var settings = m.Pointer(mapData + 0x40);
            if (settings == 0 || location.Length == 0)
            {
                return Waiting();
            }

            m.Object(settings, "Settings", "MAPAPI.Response", ("MapName", 8));
            var region = RegionCatalog.GetRegion(location, m.String(m.Pointer(settings + 8)));
            var battleHandler = Singleton(m, "BattlePacketHandler", 0, ("BattleStarting", 0x4C), ("StartingBattle", 0xAC));
            if (battleHandler == 0 || m.Bool(battleHandler + 0x4C) || m.Bool(battleHandler + 0xAC))
            {
                return Waiting();
            }

            var battle = Singleton(m, "BattleHandler", 0, ("TeamStats", 0x98));
            var inBattle = battle != 0;
            var team = inBattle ? BattleTeam(m, battle, playerName) : ActiveTeam(m);
            var teamSize = team.Count(pokemon => !pokemon.Egg);
            if (teamSize == 0)
            {
                return Waiting();
            }

            var active = inBattle ? team.Where(pokemon => pokemon.Active).ToArray() : FollowingPokemon(m, player, team);
            var opponents = inBattle ? OpponentPokemon(m, battle) : [];
            var battleType = "";
            if (inBattle)
            {
                m.Object(battleHandler, "BattlePacketHandler", "", ("IsPVP", 0x6C), ("isWild", 0xBC));
                battleType = PokemonCatalog.GetBattleType(m.Bool(battleHandler + 0xBC), m.Bool(battleHandler + 0x6C));
            }

            return new GameSnapshot()
            {
                State = "Ingame",
                PlayerName = playerName,
                TrainerLevel = trainerLevel,
                Region = region,
                Location = location,
                IsInBattle = inBattle,
                HealthyPokemon = team.Count(pokemon => !pokemon.Egg && pokemon.Health > 0),
                TeamSize = teamSize,
                BattleType = battleType,
                Team = team,
                ActivePokemon = active,
                Opponents = opponents,
                Trainer = ReadTrainer(m, player)
            };
        }

        private static PokemonSnapshot[] ActiveTeam(FrameRead m)
        {
            var handler = Singleton(m, "PokemonHandler", 0, ("CachedPokemon", 0x18), ("ActivePokemon", 0x1C));
            if (handler == 0)
            {
                return [];
            }

            var active = m.ListBytes(m.Pointer(handler + 0x1C), 6, 16);
            var ids = new Dictionary<Guid, int>();
            for (var offset = 0; offset < active.Length; offset += 16)
            {
                var id = new Guid(active.AsSpan(offset, 16));
                if (id == Guid.Empty || !ids.TryAdd(id, offset / 16 + 1))
                {
                    throw Invalid("active team identity");
                }
            }

            var team = new List<PokemonSnapshot>();
            var found = new HashSet<Guid>();
            var cached = m.ListBytes(m.Pointer(handler + 0x18), 16384, 4);
            for (var offset = 0; offset < cached.Length; offset += 4)
            {
                var pokemon = (ulong)BitConverter.ToUInt32(cached, offset);
                m.Object(pokemon, "cPkmn", "", ("<UniqueID>k__BackingField", 8),
                    ("<Health>k__BackingField", 0x40), ("<MaxHealth>k__BackingField", 0x44), ("EggState", 0x90));
                var id = new Guid(m.Bytes(pokemon + 8, 16));
                if (!ids.TryGetValue(id, out var position))
                {
                    continue;
                }

                if (!found.Add(id))
                {
                    throw Invalid("duplicate cached team member");
                }

                var egg = m.Int(pokemon + 0x90);
                if (egg is < 0 or > 4)
                {
                    throw Invalid("egg state");
                }

                m.Object(pokemon, "cPkmn", "", ("<PokemonID>k__BackingField", 0x24), ("<Level>k__BackingField", 0x30),
                    ("<Status>k__BackingField", 0x48), ("<Personality>k__BackingField", 0x5C), ("<Shiny>k__BackingField", 0x64), ("<Form>k__BackingField", 0x9C));
                var health = m.Int(pokemon + 0x40);
                var maxHealth = m.Int(pokemon + 0x44);
                if (egg == 0)
                {
                    _ = Healthy(health, maxHealth);
                }

                var species = m.Int(pokemon + 0x24);
                var form = m.String(m.Pointer(pokemon + 0x9C));
                var level = m.Int(pokemon + 0x30);
                ValidatePokemon(species, level, egg != 0);
                team.Add(new PokemonSnapshot()
                {
                    Id = species,
                    Name = egg != 0 ? "Egg" : PokemonCatalog.GetName(species, form),
                    Form = form,
                    Personality = m.Int(pokemon + 0x5C),
                    Level = level,
                    Health = egg != 0 ? null : health,
                    MaxHealth = egg != 0 ? null : maxHealth,
                    Status = egg != 0 ? "Egg" : health == 0 ? "Fainted" : PokemonCatalog.GetStatus(m.Int(pokemon + 0x48)),
                    Shiny = m.Bool(pokemon + 0x64),
                    Position = position,
                    Egg = egg != 0
                });
            }

            if (found.Count != ids.Count)
            {
                throw Invalid("incomplete active team");
            }

            return team.OrderBy(pokemon => pokemon.Position).ToArray();
        }

        private static PokemonSnapshot[] BattleTeam(FrameRead m, ulong battle, string playerName)
        {
            var array = m.Pointer(battle + 0x98);
            if (array == 0)
            {
                return [];
            }

            var team = new List<PokemonSnapshot>();
            var seen = new HashSet<ulong>();
            foreach (var pokemon in m.Array(array, 36))
            {
                if (!seen.Add(pokemon))
                {
                    throw Invalid("duplicate battle team member");
                }

                m.Object(pokemon, "PokemonInfo", "", ("Health", 0x14), ("MaxHealth", 0x10), ("Trainer", 0x3C));
                var trainer = m.String(m.Pointer(pokemon + 0x3C));
                if (!string.Equals(trainer, playerName, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                team.Add(BattlePokemon(m, pokemon, team.Count + 1));
            }

            if (team.Count > 6)
            {
                throw Invalid("battle team size");
            }

            return team.ToArray();
        }

        private static PokemonSnapshot BattlePokemon(FrameRead m, ulong pokemon, int position, bool readHealth = true)
        {
            m.Object(pokemon, "PokemonInfo", "", ("ID", 8), ("Health", 0x14), ("MaxHealth", 0x10), ("Level", 0x1C),
                ("Shiny", 0x20), ("Status", 0x2C), ("isActive", 0x38), ("Personality", 0x40), ("Forme", 0x4C));
            var id = m.Int(pokemon + 8);
            var level = m.Int(pokemon + 0x1C);
            var health = m.Int(pokemon + 0x14);
            var maxHealth = m.Int(pokemon + 0x10);
            var status = m.String(m.Pointer(pokemon + 0x2C));
            var form = m.String(m.Pointer(pokemon + 0x4C));
            ValidatePokemon(id, level, false);
            if (readHealth)
            {
                _ = Healthy(health, maxHealth);
            }
            return new PokemonSnapshot()
            {
                Id = id,
                Name = PokemonCatalog.GetName(id, form),
                Form = form,
                Personality = m.Int(pokemon + 0x40),
                Level = level,
                Health = readHealth ? health : null,
                MaxHealth = !readHealth || health == 0 && status == "fnt" ? null : maxHealth,
                Status = readHealth && health == 0 ? "Fainted" : PokemonCatalog.GetStatus(status),
                Shiny = m.Bool(pokemon + 0x20),
                Active = m.Bool(pokemon + 0x38),
                Position = position
            };
        }

        private static PokemonSnapshot[] FollowingPokemon(FrameRead m, ulong player, PokemonSnapshot[] team)
        {
            m.Object(player, "PlayerHandler", "", ("_FollowPokemon", 0x20), ("Character", 0x5C));
            var follower = m.Pointer(player + 0x20);
            if (follower == 0)
            {
                return [];
            }

            m.Object(follower, "FollowPokemon", "", ("Owner", 0xC), ("Personality", 0x24));
            if (m.Pointer(follower + 8) == 0 || m.Pointer(follower + 0xC) != m.RequiredPointer(player + 0x5C))
            {
                return [];
            }

            var personality = m.Int(follower + 0x24);
            var found = team.Where(pokemon => !pokemon.Egg && pokemon.Personality == personality).ToArray();
            if (found.Length != 1)
            {
                return [];
            }

            var position = Array.IndexOf(team, found[0]);
            team[position] = found[0] with { Active = true };
            return [team[position]];
        }

        private static PokemonSnapshot[] OpponentPokemon(FrameRead m, ulong battle)
        {
            m.Object(battle, "BattleHandler", "", ("FoeBattlePoke", 0x94));
            var array = m.Pointer(battle + 0x94);
            if (array == 0)
            {
                return [];
            }

            var opponents = new List<PokemonSnapshot>();
            foreach (var pokemon in m.Array(array, 6))
            {
                if (pokemon == 0)
                {
                    continue;
                }

                m.Object(pokemon, "BattlePokeHandler", "", ("Stats", 0x18));
                if (m.Pointer(pokemon + 8) == 0)
                {
                    continue;
                }

                var stats = m.Pointer(pokemon + 0x18);
                if (stats != 0)
                {
                    opponents.Add(BattlePokemon(m, stats, opponents.Count + 1, false));
                }
            }

            return opponents.ToArray();
        }

        private static void ValidatePokemon(int id, int level, bool egg)
        {
            if (id < 0 || id > 100000 || level < (egg ? 0 : 1) || level > 1000)
            {
                throw Invalid("Pokemon identity or level");
            }
        }

        private static bool Healthy(int health, int maxHealth)
        {
            if (maxHealth is < 1 or > 1000000 || health < 0 || health > maxHealth)
            {
                throw Invalid("Pokemon health");
            }

            return health > 0;
        }

        private static GameSnapshot Waiting()
        {
            return new GameSnapshot()
            {
                State = "Loading"
            };
        }

        private static InvalidDataException Invalid(string field)
        {
            return new InvalidDataException($"PokeOne: invalid or changing {field}.");
        }

        private static ulong Statics(FrameRead m, string name, params (string Name, int Offset)[] fields)
        {
            var klass = m.Pointer(addresses[name]);
            if (klass == 0)
            {
                return 0;
            }

            m.RequireClass(klass, name, "", fields);
            return m.Pointer(klass + 0x5C);
        }

        private static ulong Singleton(FrameRead m, string name, ulong instanceOffset, params (string Name, int Offset)[] fields)
        {
            var statics = Statics(m, name, ("Instance", (int)instanceOffset));
            if (statics == 0)
            {
                return 0;
            }

            var instance = m.Pointer(statics + instanceOffset);
            if (instance == 0)
            {
                return 0;
            }

            m.Object(instance, name, "", fields);
            return m.Pointer(instance + 8) == 0 ? 0 : instance;
        }

        private static void ResolveAddresses()
        {
            var process = Hypervisor.Process ?? throw Invalid("process");
            var module = process.Modules.Cast<ProcessModule>().FirstOrDefault(value => string.Equals(value.ModuleName, "GameAssembly.dll", StringComparison.OrdinalIgnoreCase)) ?? throw Invalid("GameAssembly module");
            var currentBase = (ulong)module.BaseAddress.ToInt64();
            if (processId != process.Id || moduleBase != currentBase || moduleSize != module.ModuleMemorySize)
            {
                Reset();
                processId = process.Id;
                moduleBase = currentBase;
                moduleSize = module.ModuleMemorySize;
            }

            var requiredReady = PatternCatalog.All.Keys.All(addresses.ContainsKey);
            if (requiredReady && (TrainerPatterns.Keys.All(addresses.ContainsKey) || DateTime.UtcNow < nextScan))
            {
                return;
            }

            if (!requiredReady && DateTime.UtcNow < nextScan)
            {
                throw Invalid("pattern scan (retry pending)");
            }

            nextScan = DateTime.UtcNow.AddSeconds(30);
            var patterns = PatternCatalog.All.Concat(TrainerPatterns).ToDictionary(pair => pair.Key, pair => pair.Value);
            var matches = patterns.Keys.ToDictionary(name => name, _ => new HashSet<ulong>());
            var header = Hypervisor.ReadArray(moduleBase, 0x1000, true);
            var pe = BitConverter.ToInt32(header, 0x3C);
            if (BitConverter.ToUInt16(header) != 0x5A4D || pe < 0x40 || pe > 0xE00 || BitConverter.ToUInt32(header, pe) != 0x4550 || BitConverter.ToUInt16(header, pe + 4) != 0x14C)
            {
                throw Invalid("x86 PE header");
            }

            var sectionCount = BitConverter.ToUInt16(header, pe + 6);
            var sectionOffset = pe + 24 + BitConverter.ToUInt16(header, pe + 20);
            if (sectionCount is 0 or > 96 || sectionOffset + sectionCount * 40 > header.Length)
            {
                throw Invalid("PE sections");
            }

            var longest = patterns.Values.Max(value => value.Pattern.Split(' ').Length);
            const int chunkSize = 1024 * 1024;
            for (var section = 0; section < sectionCount; section++)
            {
                var offset = sectionOffset + section * 40;
                if ((BitConverter.ToUInt32(header, offset + 36) & 0x20000000) == 0)
                {
                    continue;
                }

                var size = BitConverter.ToUInt32(header, offset + 8);
                var rva = BitConverter.ToUInt32(header, offset + 12);
                if (rva >= moduleSize || size > moduleSize - rva)
                {
                    throw Invalid("executable section range");
                }

                for (long position = 0; position < size; position += chunkSize)
                {
                    var count = (int)Math.Min(chunkSize + longest - 1L, size - position);
                    var start = moduleBase + rva + (ulong)position;
                    var bytes = Hypervisor.ReadArray(start, count, true);
                    foreach (var (name, definition) in patterns)
                    {
                        var searchOffset = 0;
                        while (searchOffset < bytes.Length)
                        {
                            var found = Hypervisor.FindPattern(bytes.AsSpan(searchOffset), definition.Pattern);
                            if (found < 0)
                            {
                                break;
                            }

                            searchOffset += found;
                            matches[name].Add(start + (ulong)searchOffset);
                            if (matches[name].Count > 1 && PatternCatalog.All.ContainsKey(name))
                            {
                                throw Invalid($"ambiguous {name} pattern");
                            }

                            searchOffset++;
                        }
                    }
                }
            }

            var resolved = new Dictionary<string, ulong>();
            foreach (var (name, definition) in patterns)
            {
                if (matches[name].Count != 1)
                {
                    if (PatternCatalog.All.ContainsKey(name))
                    {
                        throw Invalid($"missing {name} pattern");
                    }

                    continue;
                }

                var match = matches[name].Single();
                var address = (ulong)Hypervisor.Read<uint>(match + (ulong)definition.Operand, true);
                if (address < moduleBase || address - moduleBase > (ulong)moduleSize - 4)
                {
                    if (PatternCatalog.All.ContainsKey(name))
                    {
                        throw Invalid("absolute pattern target");
                    }

                    continue;
                }

                resolved[name] = address;
            }

            addresses.Clear();
            foreach (var pair in resolved)
            {
                addresses.Add(pair.Key, pair.Value);
            }
        }

        private sealed class FrameRead
        {
            private readonly Dictionary<(ulong Address, int Size), byte[]> reads = new Dictionary<(ulong Address, int Size), byte[]>();

            public byte[] Bytes(ulong address, int size)
            {
                if (address < 0x10000 || address > uint.MaxValue || size < 0 || size > 65536 || address + (ulong)size > uint.MaxValue)
                {
                    throw Invalid("memory range");
                }

                var key = (address, size);
                if (!reads.TryGetValue(key, out var value))
                {
                    reads[key] = value = size == 0 ? [] : Hypervisor.ReadArray(address, size, true);
                }

                return value;
            }

            public ulong Pointer(ulong address)
            {
                return BitConverter.ToUInt32(Bytes(address, 4));
            }

            public ulong RequiredPointer(ulong address)
            {
                var pointer = Pointer(address);
                return pointer >= 0x10000 ? pointer : throw Invalid("pointer");
            }

            public int Int(ulong address)
            {
                return BitConverter.ToInt32(Bytes(address, 4));
            }

            public bool Bool(ulong address)
            {
                return Bytes(address, 1)[0] switch
                {
                    0 => false,
                    1 => true,
                    _ => throw Invalid("boolean")
                };
            }

            private string NativeString(ulong address)
            {
                var bytes = new List<byte>();
                for (var offset = 0; offset < 256; offset += 16)
                {
                    var chunk = Bytes(address + (ulong)offset, 16);
                    var end = System.Array.IndexOf(chunk, (byte)0);
                    bytes.AddRange(end < 0 ? chunk : chunk[..end]);
                    if (end >= 0)
                    {
                        return Encoding.UTF8.GetString(bytes.ToArray());
                    }
                }

                throw Invalid("metadata name");
            }

            public void RequireClass(ulong klass, string name, string ns, params (string Name, int Offset)[] fields)
            {
                if (NativeString(RequiredPointer(klass + 8)) != name || NativeString(RequiredPointer(klass + 12)) != ns)
                {
                    throw Invalid("object class " + name);
                }

                if (fields.Length == 0)
                {
                    return;
                }

                if (!layouts.TryGetValue(klass, out var layout))
                {
                    var count = BitConverter.ToUInt16(Bytes(klass + 0xAC, 2));
                    var table = RequiredPointer(klass + 0x40);
                    if (count is 0 or > 2048)
                    {
                        throw Invalid("class field count");
                    }

                    layout = new Dictionary<string, int>();
                    for (var index = 0; index < count; index++)
                    {
                        var field = table + (ulong)index * 20;
                        var fieldName = NativeString(RequiredPointer(field));
                        if (Pointer(field + 8) != klass || !layout.TryAdd(fieldName, Int(field + 12)))
                        {
                            throw Invalid("class field metadata");
                        }
                    }

                    layouts.Add(klass, layout);
                }

                foreach (var field in fields)
                {
                    if (!layout.TryGetValue(field.Name, out var offset) || offset != field.Offset)
                    {
                        throw Invalid(name + "." + field.Name + " layout");
                    }
                }
            }

            public ulong Object(ulong address, string name, string ns, params (string Name, int Offset)[] fields)
            {
                RequireClass(RequiredPointer(address), name, ns, fields);
                return address;
            }

            public string String(ulong address)
            {
                if (address == 0)
                {
                    return "";
                }

                Object(address, "String", "System");
                var length = Int(address + 8);
                if (length is < 0 or > 512)
                {
                    throw Invalid("string length");
                }

                var value = Encoding.Unicode.GetString(Bytes(address + 12, length * 2));
                return value.Any(char.IsControl) ? throw Invalid("string content") : value.Trim();
            }

            public byte[] ListBytes(ulong address, int maximum, int stride)
            {
                Object(address, "List`1", "System.Collections.Generic");
                var count = Int(address + 12);
                _ = Int(address + 16);
                var array = RequiredPointer(address + 8);
                var capacity = Int(array + 12);
                if (count < 0 || count > maximum || capacity < count || capacity > 1000000)
                {
                    throw Invalid("list size");
                }

                return Bytes(array + 16, count * stride);
            }

            public ulong[] Array(ulong address, int maximum)
            {
                var count = Int(address + 12);
                if (count < 0 || count > maximum)
                {
                    throw Invalid("array size");
                }

                var bytes = Bytes(address + 16, count * 4);
                return Enumerable.Range(0, count).Select(index => (ulong)BitConverter.ToUInt32(bytes, index * 4)).ToArray();
            }

            public void Validate()
            {
                foreach (var (range, expected) in reads)
                {
                    if (range.Size > 0 && !Hypervisor.ReadArray(range.Address, range.Size, true).AsSpan().SequenceEqual(expected))
                    {
                        throw Invalid("frame");
                    }
                }
            }
        }
    }
}
