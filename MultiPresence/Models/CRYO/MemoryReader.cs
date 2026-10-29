using System.Diagnostics;
using System.Text;

namespace MultiPresence.Models.CRYO
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
            var network = Singleton(m, "NetworkManager", "NetworkManager", "Mirror", 0x10, "ValkoNetworkManager", "ValkoGames.Core.Network");
            var networkMode = network == 0 ? 0 : m.Int(network + 0x7C);
            if (networkMode == 0)
            {
                return new GameSnapshot();
            }

            if (networkMode is < 0 or > 3)
            {
                throw Invalid("network mode");
            }

            var global = Singleton(m, "Global", "Global", "ValkoGames.Core", 0, "GlobalAntarctic", "ValkoGames.Antarctic");
            var loader = global == 0 ? 0 : m.Pointer(global + 0x28);
            if (loader == 0)
            {
                return new GameSnapshot() { State = "Loading" };
            }

            m.Object(loader, "SceneLoader", "ValkoGames.Core");
            if (m.Pointer(loader + 0x10) == 0 || m.Bool(loader + 0x38))
            {
                return new GameSnapshot() { State = "Loading" };
            }

            var lobby = Singleton(m, "LobbyManager", "LobbyManager", "ValkoGames.Core");
            var game = Singleton(m, "LevelManager", "LevelManager", "ValkoGames.Core", 0, "LevelManagerAntarctic", "ValkoGames.Antarctic");
            if (lobby == 0 && game == 0)
            {
                return new GameSnapshot() { State = "Loading" };
            }

            var client = Singleton(m, "ClientData", "ClientData", "ValkoGames.Core.Players");
            var players = ReadPlayerCount(m, client);
            var maximum = networkMode == 2 ? 0 : m.Int(network + 0x48);
            if (maximum < 0 || maximum > 16 || (networkMode != 2 && (maximum == 0 || players > maximum)))
            {
                throw Invalid("player count");
            }

            var globals = Statics(m, "GlobalValues", "GlobalValues", "ValkoGames.Core");
            var online = globals != 0 && m.Pointer(globals) != 0;
            var snapshot = new GameSnapshot()
            {
                State = "Lobby",
                Online = online,
                PlayerCount = players,
                MaxPlayers = maximum
            };
            if (lobby != 0)
            {
                return snapshot;
            }

            var scene = m.String(m.Pointer(loader + 0x40));
            var map = GameText.GetMap(scene);
            var values = Singleton(m, "LobbyValuesContainer", "LobbyValuesContainer", "ValkoGames.Core.Lobby");
            if (map.Length == 0 || values == 0)
            {
                return new GameSnapshot() { State = "Loading" };
            }

            var difficulty = GameText.GetDifficulty(m.Int(values + 0x6C));
            if (difficulty.Length == 0)
            {
                throw Invalid("difficulty");
            }

            var objectives = ReadObjectives(m, client);
            return snapshot with
            {
                State = "Ingame",
                Map = map,
                Difficulty = difficulty,
                Objective = string.Join(" | ", objectives.Select(value => value.Text)),
                Progress = string.Join(" | ", objectives.Select(value => value.Progress))
            };
        }

        private static int ReadPlayerCount(FrameRead m, ulong client)
        {
            if (client == 0)
            {
                return 1;
            }

            var clients = m.Pointer(client + 0xD0);
            if (clients == 0)
            {
                return 1;
            }

            var count = 0;
            foreach (var entry in m.Array(clients, 16))
            {
                if (entry != 0)
                {
                    m.Object(entry, "ClientData", "ValkoGames.Core.Players");
                    if (m.Pointer(entry + 0x10) != 0)
                    {
                        count++;
                    }
                }
            }

            return count;
        }

        private static List<(string Text, string Progress)> ReadObjectives(FrameRead m, ulong client)
        {
            var result = new List<(string Text, string Progress)>();
            var manager = Singleton(m, "ObjectiveManager", "ObjectiveManager", "ValkoGames.Antarctic.Objectives");
            if (manager == 0 || !m.Bool(manager + 0xA0))
            {
                return result;
            }

            var objectives = m.List(m.Pointer(manager + 0x78), 128);
            var active = new List<ulong>();
            foreach (var objective in objectives)
            {
                if (objective == 0)
                {
                    continue;
                }

                if (!m.IsClass(objective, "ObjectiveDescription", "ValkoGames.Antarctic.Objectives", true))
                {
                    throw Invalid("objective class");
                }
                if (m.Pointer(objective + 0x10) != 0 && m.Bool(objective + 0x59))
                {
                    active.Add(objective);
                }
            }

            var selected = SelectedObjective(m, client);
            if (!active.Contains(selected))
            {
                selected = active.FirstOrDefault(value => m.Int(value + 0x38) == 0 && !m.Bool(value + 0x58));
                if (selected == 0)
                {
                    selected = active.FirstOrDefault(value => !m.Bool(value + 0x58));
                }
            }

            if (selected == 0)
            {
                return result;
            }

            foreach (var step in m.Array(m.Pointer(selected + 0x40), 128))
            {
                if (step == 0)
                {
                    continue;
                }

                m.Object(step, "MainStep", "");
                if (!m.Bool(step + 0x24))
                {
                    continue;
                }

                AddStep(m, step, result);
                foreach (var child in m.Array(m.Pointer(step + 0x30), 128))
                {
                    if (child != 0)
                    {
                        m.Object(child, "Step", "");
                        if (m.Bool(child + 0x24))
                        {
                            AddStep(m, child, result);
                        }
                    }
                }
            }

            return result;
        }

        private static void AddStep(FrameRead m, ulong step, List<(string Text, string Progress)> result)
        {
            var maximum = m.Int(step + 0x20);
            var current = m.Int(step + 0x28);
            if (current is < 0 or > 100000 || maximum is < 1 or > 100000)
            {
                throw Invalid("objective progress");
            }

            var text = GameText.GetText(m.String(m.Pointer(step + 0x18)));
            if (text.Length == 0)
            {
                return;
            }

            var progress = $"{current}/{maximum}";
            if (maximum > 1)
            {
                text += $" ({progress})";
            }
            else if (current >= maximum)
            {
                text += " (Completed)";
            }

            result.Add((text, progress));
        }

        private static ulong SelectedObjective(FrameRead m, ulong client)
        {
            var player = client == 0 ? 0 : m.Pointer(client + 0xB8);
            if (player == 0 || m.Pointer(player + 0x10) == 0)
            {
                return 0;
            }

            m.Object(player, "PlayerAntarctic", "ValkoGames.Antarctic.Players");
            var controller = m.Pointer(player + 0x238);
            if (controller == 0 || m.Pointer(controller + 0x10) == 0)
            {
                return 0;
            }

            m.Object(controller, "PlayerObjectiveTabletController", "ValkoGames.Antarctic.Players.Tablet");
            var tablet = m.Pointer(controller + 0x68);
            if (tablet == 0 || m.Pointer(tablet + 0x10) == 0)
            {
                return 0;
            }

            m.Object(tablet, "PlayerObjectiveTablet", "ValkoGames.Antarctic.Players.Tablet");
            foreach (var component in m.Array(m.Pointer(tablet + 0x58), 32))
            {
                if (component == 0 || !m.IsClass(component, "PlayerObjectiveTabletObjectivesTab", "ValkoGames.Antarctic.Players.Tablet.Objectives") || m.Pointer(component + 0x10) == 0)
                {
                    continue;
                }

                var entries = m.List(m.Pointer(component + 0x60), 128);
                var selected = m.Int(component + 0x70);
                if (selected >= 0 && selected < entries.Length && entries[selected] != 0)
                {
                    var ui = m.Object(entries[selected], "ObjectiveTabUIObject", "ValkoGames.Antarctic.Players.Tablet.Objectives");
                    if (m.Pointer(ui + 0x10) != 0 && m.Bool(ui + 0x80))
                    {
                        return m.Pointer(ui + 0x70);
                    }
                }
            }

            return 0;
        }

        private static InvalidDataException Invalid(string field)
        {
            return new InvalidDataException($"Cryo: invalid or changing {field}.");
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

        private static ulong Singleton(FrameRead m, string pattern, string name, string ns = "", ulong field = 0, string instanceName = "", string instanceNamespace = "")
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

            m.Object(instance, instanceName.Length == 0 ? name : instanceName, instanceName.Length == 0 ? ns : instanceNamespace);
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
                    var end = System.Array.IndexOf(chunk, (byte)0);
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

            public bool IsClass(ulong address, string name, string ns, bool inherited = false)
            {
                var klass = RequiredPointer(address);
                for (var depth = 0; depth < 16 && klass != 0; depth++)
                {
                    if (NativeString(RequiredPointer(klass + 0x10)) == name && NativeString(RequiredPointer(klass + 0x18)) == ns)
                    {
                        return true;
                    }

                    if (!inherited)
                    {
                        return false;
                    }

                    klass = Pointer(klass + 0x58);
                }

                return false;
            }

            public ulong[] Array(ulong address, int maximum)
            {
                if (address == 0)
                {
                    return [];
                }

                var count = Pointer(address + 0x18);
                if (count > (ulong)maximum)
                {
                    throw Invalid("array length");
                }

                var result = new ulong[(int)count];
                if (count > 0)
                {
                    var bytes = Bytes(address + 0x20, (int)count * 8);
                    for (var index = 0; index < result.Length; index++)
                    {
                        result[index] = BitConverter.ToUInt64(bytes, index * 8);
                    }
                }

                return result;
            }

            public ulong[] List(ulong address, int maximum)
            {
                if (address == 0)
                {
                    return [];
                }

                var count = ListCount(address, maximum);
                var items = Array(RequiredPointer(address + 0x10), maximum * 2);
                return items[..count];
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
