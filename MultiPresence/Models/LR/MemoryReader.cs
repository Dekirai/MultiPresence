using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace MultiPresence.Models.LR
{
    internal static class MemoryReader
    {
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
            var network = Singleton(m, "NetworkManager", "NetworkManager", "Mirror", 0x10, "ValkoNetworkManager");
            var networkMode = network == 0 ? 0 : m.Int(network + 0xA4);
            if (networkMode == 0)
            {
                return new GameSnapshot();
            }

            var loader = Singleton(m, "SceneLoader", "SceneLoader");
            if (loader != 0)
            {
                var loadingUI = m.Pointer(loader + 0x28);
                if (loadingUI != 0 && m.Pointer(loadingUI + 0x10) != 0)
                {
                    return new GameSnapshot() { State = "Loading" };
                }
            }

            var lobby = Singleton(m, "LobbyManager", "LobbyManager");
            var game = Singleton(m, "GameManager", "GameManager");
            if (lobby == 0 && game == 0)
            {
                return new GameSnapshot() { State = "Loading" };
            }

            var values = Singleton(m, "GameValues", "GameValues");
            if (values == 0)
            {
                return new GameSnapshot() { State = "Loading" };
            }

            var save = Statics(m, "PlayerSave", "PlayerSave", "ValkoGames.Labyrinthine.Saves");
            if (save == 0)
            {
                return new GameSnapshot() { State = "Loading" };
            }

            var experience = m.Bytes(save, 12);
            if (experience[8] != 1)
            {
                return new GameSnapshot() { State = "Loading" };
            }

            var points = BitConverter.ToInt32(experience) ^ BitConverter.ToInt32(experience, 4);
            if (points < 0)
            {
                throw Invalid("experience");
            }

            var level = 1;
            while (level < 9999 && (points -= (level + 5) * 10) > 0)
            {
                level++;
            }

            var players = m.ListCount(m.RequiredPointer((lobby != 0 ? lobby + 0x128 : game + 0x40)), 8);
            var maximum = networkMode == 2 ? 0 : m.Int(network + 0x58);
            if (maximum < 0 || maximum > 8 || (networkMode != 2 && (maximum == 0 || players > maximum)))
            {
                throw Invalid("player count");
            }

            var lobbyId = m.Pointer(values + 0x58);
            var online = false;
            if (lobbyId != 0)
            {
                m.Object(lobbyId, "NetworkID", "ValkoGames.Labyrinthine");
                online = m.Pointer(lobbyId + 0x18) != 0;
            }

            var snapshot = new GameSnapshot()
            {
                State = "Lobby",
                Level = level,
                Online = online,
                PlayerCount = players,
                MaxPlayers = maximum
            };
            if (lobby != 0)
            {
                return snapshot;
            }

            var scene = m.String(m.Pointer(values + 0x28));
            var chapter = GameText.GetChapter(scene);
            var contract = m.Pointer(values + 0x38);
            if (chapter > 0)
            {
                snapshot = snapshot with { State = "Story", Chapter = chapter };
            }
            else if ((scene.StartsWith("Rand_Maze_", StringComparison.OrdinalIgnoreCase) || scene is "DLC_Egypt_A" or "DLC_Egypt_B") && contract != 0)
            {
                m.Object(contract, "Contract", "RandomGeneration.Contracts");
                var type = m.Bytes(contract + 0x30, 1)[0];
                var difficulty = m.Bytes(contract + 0x53, 1)[0];
                if (type > 20 || difficulty > 4)
                {
                    throw Invalid("case type or difficulty");
                }

                snapshot = snapshot with
                {
                    State = "CaseFile",
                    CaseName = GameText.GetCaseName(type),
                    CaseAsset = GameText.GetCaseAsset(type),
                    Difficulty = GameText.GetDifficulty(difficulty),
                    CaseType = GameText.GetCaseType(m.Int(contract + 0x40)),
                    MapSize = GameText.GetMapSize(m.Bytes(contract + 0x44, 1)[0]),
                    Lives = m.Bytes(contract + 0x50, 1)[0]
                };
            }
            else
            {
                return snapshot with { State = "Loading" };
            }

            var objectives = ReadObjectives(m);
            return snapshot with
            {
                Objective = string.Join(" | ", objectives.Select(value => value.Text)),
                Progress = string.Join(" | ", objectives.Select(value => value.Progress))
            };
        }

        private static List<(string Text, string Progress)> ReadObjectives(FrameRead m)
        {
            var result = new List<(string Text, string Progress)>();
            var manager = Singleton(m, "ObjectiveManager", "ObjectiveManager", "Objectives");
            if (manager == 0)
            {
                return result;
            }

            var dictionary = m.RequiredPointer(manager + 0x50);
            m.Object(dictionary, "Dictionary`2", "System.Collections.Generic");
            var count = m.Int(dictionary + 0x20);
            _ = m.Int(dictionary + 0x2C);
            if (count is < 0 or > 128)
            {
                throw Invalid("objective count");
            }

            if (count == 0)
            {
                return result;
            }

            var entries = m.RequiredPointer(dictionary + 0x18);
            if (m.Pointer(entries + 0x18) < (ulong)count)
            {
                throw Invalid("objective array");
            }

            var bytes = m.Bytes(entries + 0x20, count * 24);
            for (var index = 0; index < count; index++)
            {
                var offset = index * 24;
                var ui = BitConverter.ToUInt64(bytes, offset + 16);
                if (BitConverter.ToInt32(bytes, offset) < 0 || ui == 0 || BitConverter.ToUInt64(bytes, offset + 8) == 0)
                {
                    continue;
                }

                m.Object(ui, "ObjectiveUI", "Objectives");
                if (m.Pointer(ui + 0x10) == 0 || !m.Bool(ui + 0x40))
                {
                    continue;
                }

                var objective = m.Object(m.RequiredPointer(ui + 0x38), "Objective", "Objectives");
                var maximum = m.Int(objective + 0x20);
                var current = m.Int(objective + 0x24);
                if (current < 0 || maximum is < 1 or > 100000 || current > maximum)
                {
                    throw Invalid("objective progress");
                }

                if (current == maximum)
                {
                    continue;
                }

                var localized = m.RequiredPointer(objective + 0x18);
                var key = m.String(m.Pointer(localized + 0x10));
                var text = GameText.GetObjective(key);
                if (text.Length == 0)
                {
                    text = m.String(m.Pointer(localized + 0x18));
                }

                text = text.Replace("#", maximum.ToString(CultureInfo.InvariantCulture)).Trim();
                if (text.Length == 0)
                {
                    continue;
                }

                var progress = $"{current}/{maximum}";
                result.Add((maximum > 1 ? $"{text} ({progress})" : text, progress));
            }

            return result;
        }

        private static InvalidDataException Invalid(string field)
        {
            return new InvalidDataException($"Labyrinthine: invalid or changing {field}.");
        }

        private static ulong Statics(FrameRead m, string pattern, string name, string ns)
        {
            var klass = m.Pointer(addresses[pattern]);
            if (klass == 0 || (klass & 1) != 0)
            {
                return 0;
            }

            m.RequireClass(klass, [name], ns);
            return m.Pointer(klass + 0xB8);
        }

        private static ulong Singleton(FrameRead m, string pattern, string name, string ns = "", ulong field = 0, string instanceName = "")
        {
            var statics = Statics(m, pattern, name, ns);
            if (statics == 0)
            {
                return 0;
            }

            var instance = m.Pointer(statics + field);
            if (instance == 0)
            {
                return 0;
            }

            m.Object(instance, instanceName.Length == 0 ? name : instanceName, instanceName.Length == 0 ? ns : "");
            return m.Pointer(instance + 0x10) == 0 ? 0 : instance;
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
                    foreach (var (name, definition) in PatternCatalog.All)
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
            foreach (var (name, definition) in PatternCatalog.All)
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

            public bool Bool(ulong address)
            {
                return Bytes(address, 1)[0] switch
                {
                    0 => false,
                    1 => true,
                    _ => throw Invalid("boolean"),
                };
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

            public string String(ulong address)
            {
                if (address == 0)
                {
                    return "";
                }

                Object(address, "String", "System");
                var length = Int(address + 0x10);
                if (length is < 0 or > 4096)
                {
                    throw Invalid("string length");
                }

                return Encoding.Unicode.GetString(Bytes(address + 0x14, length * 2));
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

            public void Validate()
            {
                foreach (var (range, expected) in reads)
                {
                    if (!Hypervisor.ReadArray(range.Address, range.Size, true).AsSpan().SequenceEqual(expected))
                    {
                        throw Invalid("frame");
                    }
                }
            }
        }
    }
}
