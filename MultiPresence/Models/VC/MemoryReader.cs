using System.Diagnostics;
using System.Text;

namespace MultiPresence.Models.VC
{
    internal static class MemoryReader
    {
        private const string ModelNamespace = "Nosebleed.Pancake.Models";
        private const string ConfigNamespace = "Nosebleed.Pancake.GameConfig";
        private static readonly Dictionary<string, ulong> addresses = new Dictionary<string, ulong>();
        private static int processId, moduleSize;
        private static ulong moduleBase;
        private static DateTime nextScan;
        public static void Reset()
        {
            addresses.Clear();
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
            var stateManager = Singleton(m, "StateSingleton", "GameStateManager", "Nosebleed.Pancake");
            if (stateManager == 0)
            {
                return Waiting();
            }

            var state = m.Int(stateManager + 0x30);
            if (m.Bool(stateManager + 0x34))
            {
                return Waiting();
            }

            if (state == 1)
            {
                return new GameSnapshot();
            }

            if (state == 2)
            {
                return new GameSnapshot()

                {
                    State = "CharacterSelect",
                    Activity = "In Village"
                };
            }

            if (state is not 3 and not 4)
            {
                return Waiting();
            }

            var game = Singleton(m, "GameSingleton", "GameModel", ModelNamespace);
            if (game == 0)
            {
                return Waiting();
            }

            if (m.Bool(game + 0xA0))
            {
                return new GameSnapshot()

                {
                    State = "Ended",
                    Activity = "Run Ended"
                };
            }

            var player = m.Unity(m.Pointer(game + 0x58), "PlayerModel", ModelNamespace);
            var dungeon = m.Unity(m.Pointer(game + 0x60), "DungeonModel", ModelNamespace);
            var stateMachine = m.Unity(m.Pointer(game + 0x80), "GameStateMachine", "Nosebleed.Pancake.GameLogic");
            var phase = m.Pointer(stateMachine + 0x20);
            if (phase == 0)
            {
                return Waiting();
            }

            var phaseClass = m.ClassOf(phase);
            if (phaseClass.Namespace != "Nosebleed.Pancake.GameLogic.GameStates")
            {
                return Waiting();
            }

            if (phaseClass.Name is "FloorTransitionState" or "RunStartState")
            {
                return Waiting();
            }

            if (phaseClass.Name is "GameOverState" or "DungeonCompleteState" or "PlayerDiedState")
            {
                return new GameSnapshot()

                {
                    State = "Ended",
                    Activity = phaseClass.Name == "DungeonCompleteState" ? "Dungeon Complete" : "Game Over"
                };
            }

            var globals = Globals(m);
            var settings = m.Unity(m.Pointer(globals + 0x20), "PlayerRunSettings", "Nosebleed.Pancake.RuntimeData");
            var dungeonAsset = m.Unity(m.Pointer(globals + 0x38), "DungeonVariableAsset", "Util.Variables");
            var config = m.Unity(m.Pointer(dungeonAsset + 0x18), "DungeonConfig", ConfigNamespace);
            var dungeonId = m.AssetId(config);
            var characters = m.List(m.Pointer(settings + 0x78), 32).Select(address => m.AssetId(m.Unity(address, "FccConfig", ConfigNamespace))).ToArray();
            if (characters.Length == 0)
            {
                return Waiting();
            }

            var stats = m.Object(m.Pointer(player + 0x138), "PlayerStats", "Nosebleed.Pancake.GameLogic");
            var healthStat = m.Object(m.Pointer(stats + 0x18), "SimpleStat", "Nosebleed.Pancake.GameLogic");
            var health = Stat(m, healthStat);
            var maxHealth = FloatVariable(m, m.Pointer(healthStat + 0x20));
            var armor = Stat(m, m.Pointer(stats + 0x20));
            var mana = Stat(m, m.Pointer(stats + 0x60));
            if (maxHealth <= 0 || health > maxHealth + 0.01f)
            {
                throw Invalid("health range");
            }

            var handPile = CardPile(m, player, 0xB0, "HandPileModel", 0x28);
            var hand = m.List(m.Pointer(handPile + 0x38), 4096).Select(address =>
            {
                var card = m.Unity(address, "CardModel", ModelNamespace);
                var cardConfig = m.Pointer(card + 0x38);
                m.Unity(cardConfig, ["CardConfig", "FccConfig"], ConfigNamespace);
                return m.AssetId(cardConfig);
            }).ToArray();
            var draw = CardPile(m, player, 0xA8, "DrawPileModel", 0x68);
            var discard = CardPile(m, player, 0xC0, "DiscardPileModel", 0x80);
            var completedFloors = m.Nonnegative(dungeon + 0xB0);
            var generation = m.Unity(m.Pointer(config + 0x80), "DungeonGenerationConfig", ConfigNamespace);
            var floors = m.ListCount(m.Pointer(generation + 0x58), 10000);
            if (floors == 0)
            {
                throw Invalid("empty dungeon generation");
            }

            return new GameSnapshot()

            {
                State = "Ingame",
                Activity = Activity(phaseClass.Name),
                DungeonId = dungeonId,
                CharacterIds = characters,
                HandIds = hand,
                Floor = completedFloors % floors + 1,
                CompletedFloors = completedFloors,
                Level = IntVariable(m, m.Pointer(player + 0x188), 1),
                Kills = IntAsset(m, m.Pointer(game + 0x48)),
                Coins = IntAsset(m, m.Pointer(player + 0x80)),
                Health = health,
                MaxHealth = maxHealth,
                Armor = armor,
                Mana = mana,
                StartingMana = IntAsset(m, m.Pointer(stats + 0x10)),
                Experience = IntVariable(m, m.Pointer(player + 0x178)),
                NextLevelExperience = IntVariable(m, m.Pointer(player + 0x180)),
                TotalExperience = m.Nonnegative(player + 0x170),
                Turn = IntVariable(m, m.Pointer(player + 0x198)),
                Combo = IntVariable(m, m.Pointer(player + 0x190)),
                DrawCount = m.ListCount(m.Pointer(draw + 0x38), 4096),
                DiscardCount = m.ListCount(m.Pointer(discard + 0x38), 4096),
                PlayerTurn = m.Bool(m.Unity(m.Pointer(player + 0x60), "BoolAsset", "Util.Variables") + 0x18),
                Endless = m.Bool(settings + 0x60),
                HandSizeLimit = m.Bool(settings + 0x61),
                Hurry = m.Bool(settings + 0x62)
            };
        }

        private static string Activity(string phase)
        {
            return phase switch
            {
                "PlayerTurnState" => "Player Turn",
                "EnemyTurnState" => "Enemy Turn",
                "EncounterStartState" or "PrePlayerTurnState" or "PlayerEndTurnState" or "PostPlayerEndTurnState" or "EncounterResetState" => "In Battle",
                "EncounterDefeatedState" => "Encounter Defeated",
                "CutsceneState" => "In Cutscene",
                _ => "Exploring",
            };
        }

        private static GameSnapshot Waiting()
        {
            return new GameSnapshot()

            {
                State = "Loading",
                Activity = "Loading / Waiting for game data"
            };
        }

        private static InvalidDataException Invalid(string field)
        {
            return new InvalidDataException($"Vampire Crawlers: invalid or changing {field}.");
        }

        private static ulong Singleton(FrameRead m, string pattern, string name, string ns)
        {
            var klass = m.Pointer(addresses[pattern]);
            if (klass == 0)
            {
                return 0;
            }

            m.RequireClass(klass, ["SingletonMonoBehaviour`1"], "");
            var statics = m.Pointer(klass + 0xB8);
            if (statics == 0)
            {
                return 0;
            }

            var instance = m.Pointer(statics + 8);
            if (instance == 0)
            {
                return 0;
            }

            m.Object(instance, name, ns);
            return m.Pointer(instance + 0x10) == 0 ? 0 : instance;
        }

        private static ulong Globals(FrameRead m)
        {
            var method = m.RequiredPointer(addresses["GlobalsMethod"]);
            var methodClass = m.RequiredPointer(method + 0x20);
            var context = m.RequiredPointer(methodClass + 0xC0);
            var genericClass = m.RequiredPointer(context + 0x10);
            m.RequireClass(genericClass, ["SingletonMonoBehaviour`1"], "Nosebleed.Core.Singletons");
            var statics = m.RequiredPointer(genericClass + 0xB8);
            return m.Unity(m.RequiredPointer(statics), "Globals", "Nosebleed.Pancake.GameConfig.Accessors");
        }

        private static int IntAsset(FrameRead m, ulong address)
        {
            return m.Nonnegative(m.Unity(address, "IntAsset", "Util.Variables") + 0x18);
        }

        private static int IntVariable(FrameRead m, ulong address, int minimum = 0)
        {
            var value = m.Nonnegative(m.Object(address, "IntVariable", "Util.Variables") + 0x10);
            if (value < minimum)
            {
                throw Invalid("integer variable");
            }

            return value;
        }

        private static float FloatVariable(FrameRead m, ulong address)
        {
            return m.Number(m.Object(address, "FloatVariable", "Util.Variables") + 0x10);
        }

        private static float Stat(FrameRead m, ulong stat)
        {
            m.Object(stat, "SimpleStat", "Nosebleed.Pancake.GameLogic");
            return m.Number(m.Unity(m.Pointer(stat + 0x10), "FloatAsset", "Util.Variables") + 0x18);
        }

        private static ulong CardPile(FrameRead m, ulong player, ulong field, string name, ulong pileField)
        {
            var wrapper = m.Unity(m.Pointer(player + field), name, ModelNamespace);
            var pile = m.Unity(m.Pointer(wrapper + pileField), "CardPileModel", ModelNamespace);
            if (m.Pointer(pile + 0x40) != player)
            {
                throw Invalid("card-pile owner");
            }

            return pile;
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

            if (addresses.Count == PatternCatalog.All.Count)
            {
                return;
            }

            if (DateTime.UtcNow < nextScan)
            {
                throw Invalid("pattern scan (retry pending)");
            }

            nextScan = DateTime.UtcNow.AddSeconds(30);
            var matches = PatternCatalog.All.Keys.ToDictionary(name => name, _ => new HashSet<ulong>());
            var header = Hypervisor.ReadArray(moduleBase, 0x1000, true);
            var pe = BitConverter.ToInt32(header, 0x3C);
            if (BitConverter.ToUInt16(header) != 0x5A4D || pe < 0x40 || pe > 0xE00 || BitConverter.ToUInt32(header, pe) != 0x4550)
            {
                throw Invalid("PE header");
            }

            var sectionCount = BitConverter.ToUInt16(header, pe + 6);
            var sectionOffset = pe + 24 + BitConverter.ToUInt16(header, pe + 20);
            if (sectionCount is 0 or > 96 || sectionOffset + sectionCount * 40 > header.Length)
            {
                throw Invalid("PE sections");
            }

            var longest = PatternCatalog.All.Values.Max(value => value.Pattern.Split(' ').Length);
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
                    foreach (var(name, definition)in PatternCatalog.All)
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
                            if (matches[name].Count > 1)
                            {
                                throw Invalid($"ambiguous {name} pattern");
                            }

                            searchOffset++;
                        }
                    }
                }
            }

            var resolved = new Dictionary<string, ulong>();
            foreach (var(name, definition)in PatternCatalog.All)
            {
                if (matches[name].Count != 1)
                {
                    throw Invalid($"missing {name} pattern");
                }

                var match = matches[name].Single();
                var address = definition.Length == 0 ? match : (ulong)((long)match + definition.Length + Hypervisor.Read<int>(match + (ulong)definition.Operand, true));
                if (address < moduleBase || address - moduleBase > (ulong)moduleSize - 8)
                {
                    throw Invalid("RIP target");
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
                if (address < 0x10000 || address > 0x00007FFFFFFFFFFF || size < 0 || size > 65536)
                {
                    throw Invalid("memory range");
                }

                var key = (address, size);
                if (!reads.TryGetValue(key, out var value))
                {
                    reads[key] = value = Hypervisor.ReadArray(address, size, true);
                }

                return value;
            }

            public ulong Pointer(ulong address)
            {
                return BitConverter.ToUInt64(Bytes(address, 8));
            }

            public ulong RequiredPointer(ulong address)
            {
                var pointer = Pointer(address);
                if (pointer < 0x10000 || pointer > 0x00007FFFFFFFFFFF)
                {
                    throw Invalid("pointer");
                }

                return pointer;
            }

            public int Int(ulong address)
            {
                return BitConverter.ToInt32(Bytes(address, 4));
            }

            public int Nonnegative(ulong address)
            {
                var value = Int(address);
                return value >= 0 ? value : throw Invalid("negative counter");
            }

            public bool Bool(ulong address)
            {
                return Bytes(address, 1)[0] switch
                {
                    0 => false,
                    1 => true,
                    _ => throw Invalid("boolean"),
                };
            }

            public float Number(ulong address)
            {
                var value = BitConverter.ToSingle(Bytes(address, 4));
                return float.IsFinite(value) && value is >= 0 and <= 10000000 ? value : throw Invalid("statistic");
            }

            private string NativeString(ulong address)
            {
                var bytes = new List<byte>();
                for (var offset = 0; offset < 256; offset += 16)
                {
                    var chunk = Bytes(address + (ulong)offset, 16);
                    var end = Array.IndexOf(chunk, (byte)0);
                    bytes.AddRange(end < 0 ? chunk : chunk[..end]);
                    if (end >= 0)
                    {
                        return Encoding.UTF8.GetString(bytes.ToArray());
                    }
                }

                throw Invalid("class name");
            }

            public (string Name, string Namespace) ClassOf(ulong address)
            {
                var klass = RequiredPointer(address);
                return (NativeString(RequiredPointer(klass + 0x10)), NativeString(RequiredPointer(klass + 0x18)));
            }

            public void RequireClass(ulong klass, string[] names, string ns)
            {
                var name = NativeString(RequiredPointer(klass + 0x10));
                if (!names.Contains(name, StringComparer.Ordinal) || NativeString(RequiredPointer(klass + 0x18)) != ns)
                {
                    throw Invalid("object class " + string.Join("/", names));
                }
            }

            public ulong Object(ulong address, string name, string ns)
            {
                RequireClass(RequiredPointer(address), [name], ns);
                return address;
            }

            public ulong Unity(ulong address, string name, string ns)
            {
                return Unity(address, [name], ns);
            }

            public ulong Unity(ulong address, string[] names, string ns)
            {
                RequireClass(RequiredPointer(address), names, ns);
                RequiredPointer(address + 0x10);
                return address;
            }

            public string AssetId(ulong address)
            {
                var text = RequiredPointer(address + 0x58);
                var length = Int(text + 0x10);
                if (length is <= 0 or > 256)
                {
                    throw Invalid("asset ID length");
                }

                var result = Encoding.Unicode.GetString(Bytes(text + 0x14, length * 2));
                if (result.Any(char.IsControl))
                {
                    throw Invalid("asset ID content");
                }

                return result;
            }

            public int ListCount(ulong address, int maximum)
            {
                Object(address, "List`1", "System.Collections.Generic");
                var count = Int(address + 0x18);
                _ = Int(address + 0x1C);
                var array = RequiredPointer(address + 0x10);
                var capacity = Pointer(array + 0x18);
                if (count < 0 || count > maximum || capacity > 1000000 || (ulong)count > capacity)
                {
                    throw Invalid("list size");
                }

                return count;
            }

            public ulong[] List(ulong address, int maximum)
            {
                var count = ListCount(address, maximum);
                if (count == 0)
                {
                    return[];
                }

                var values = Bytes(RequiredPointer(address + 0x10) + 0x20, count * 8);
                return Enumerable.Range(0, count).Select(index => BitConverter.ToUInt64(values, index * 8)).ToArray();
            }

            public void Validate()
            {
                foreach (var(range, expected)in reads)
                    if (!Hypervisor.ReadArray(range.Address, range.Size, true).AsSpan().SequenceEqual(expected))
                    {
                        throw Invalid("frame");
                    }
            }
        }
    }
}
