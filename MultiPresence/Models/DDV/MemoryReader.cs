using System.Diagnostics;
using System.Text;

namespace MultiPresence.Models.DDV
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
            if (Singleton(m, "NewStartMenu", "NewStartMenu", "Mdl.Ui") != 0 || Singleton(m, "StartMenu", "StartMenu", "Mdl.Ui") != 0)
            {
                return new GameSnapshot();
            }

            var root = Singleton(m, "SystemRoot", "SystemRoot", "Mdl.Systems");
            if (root == 0)
            {
                return new GameSnapshot() { State = "Loading" };
            }

            var avatar = m.Pointer(root + 0x30);
            var stack = m.Pointer(root + 0x58);
            if (avatar == 0 || stack == 0 || m.Pointer(avatar + 0x10) == 0 || m.Pointer(stack + 0x10) == 0)
            {
                return new GameSnapshot() { State = "Loading" };
            }

            m.Object(avatar, "PlayerAvatar", "Mdl.Avatar");
            m.Object(stack, "SceneStack", "Mdl.Systems");
            var scenes = m.RequiredPointer(stack + 0x40);
            var count = m.ListCount(scenes, 32);
            if (count == 0)
            {
                return new GameSnapshot() { State = "Loading" };
            }

            var scene = m.Pointer(m.RequiredPointer(scenes + 0x10) + 0x20 + (ulong)(count - 1) * 0x38);
            if (scene == 0)
            {
                return new GameSnapshot() { State = "Loading" };
            }

            m.Object(scene, "SceneItemData", "Definitions.Items");
            var location = GameText.GetScene(m.Int(scene + 0x18));
            if (m.Int(scene + 0x40) == 1 && m.Bool(avatar + 0x240))
            {
                var area = GameText.GetVillageArea(m.Int(avatar + 0x244));
                if (area.Length > 0)
                {
                    location = area;
                }
            }

            var client = m.Pointer(root + 0x20);
            var meta = client == 0 ? 0 : m.Pointer(m.Object(client, "Client", "Mdl.Online") + 0x28);
            if (meta == 0 || !m.Bool(m.Object(meta, "Client", "Meta.Online") + 0xD0))
            {
                return new GameSnapshot() { State = "Loading" };
            }

            var profile = m.Object(m.RequiredPointer(meta + 0xB8), "Profile", "Meta");
            var player = m.Object(m.RequiredPointer(profile + 0x28), "ProfilePlayer", "Meta");
            var level = m.Int(player + 0x20);
            var energy = m.Int(player + 0x68);
            if (level is < 1 or > 10000 || energy is < 0 or > 1000000)
            {
                throw Invalid("player values");
            }

            var currencies = ReadCurrencies(m, m.RequiredPointer(player + 0x48));
            var mission = ReadMission(m, profile);
            return new GameSnapshot()
            {
                State = "Ingame",
                Location = location,
                Quest = GameText.GetQuest(mission),
                Objective = ReadObjectives(m, mission),
                PlayerName = m.String(m.Pointer(player + 0x18)),
                Money = currencies.GetValueOrDefault(80000000),
                Dreamlight = currencies.GetValueOrDefault(80300000),
                Level = level,
                Energy = energy,
                MaxEnergy = ReadMaximumEnergy(m, player, level)
            };
        }

        private static Dictionary<int, int> ReadCurrencies(FrameRead m, ulong currency)
        {
            m.Object(currency, "MapField`2", "Google.Protobuf.Collections");
            var list = m.Object(m.RequiredPointer(currency + 0x18), "LinkedList`1", "System.Collections.Generic");
            var count = m.Int(list + 0x18);
            if (count is < 0 or > 512)
            {
                throw Invalid("currency count");
            }

            var first = m.Pointer(list + 0x10);
            var node = first;
            var seen = new HashSet<ulong>();
            var result = new Dictionary<int, int>();
            for (var index = 0; index < count; index++)
            {
                if (node == 0 || !seen.Add(node))
                {
                    throw Invalid("currency list");
                }

                m.Object(node, "LinkedListNode`1", "System.Collections.Generic");
                var id = m.Int(node + 0x28);
                var value = m.Int(node + 0x2C);
                if (value < 0 || !result.TryAdd(id, value) || m.Pointer(node + 0x10) != list)
                {
                    throw Invalid("currency value");
                }

                node = m.Pointer(node + 0x18);
            }

            if (node != first || (count == 0 && first != 0))
            {
                throw Invalid("currency list length");
            }

            return result;
        }

        private static ulong[] Repeated(FrameRead m, ulong address, int maximum)
        {
            m.Object(address, "RepeatedField`1", "Google.Protobuf.Collections");
            return m.List(m.RequiredPointer(address + 0x10), maximum);
        }

        private static int ReadMission(FrameRead m, ulong profile)
        {
            var activities = Repeated(m, m.RequiredPointer(profile + 0x88), 128);
            if (activities.Length == 0)
            {
                return 0;
            }

            var selected = m.Int(profile + 0x90);
            var activity = m.Object(activities[selected >= 0 && selected < activities.Length ? selected : 0], "TrackedActivity", "Meta");
            if (m.Int(activity + 0x20) != 1)
            {
                return 0;
            }

            var mission = m.Pointer(activity + 0x18);
            return mission == 0 ? 0 : m.Int(m.Object(mission, "TrackedMission", "Meta") + 0x18);
        }

        private static int? ReadMaximumEnergy(FrameRead m, ulong player, int level)
        {
            var info = m.Pointer(player + 0x268);
            if (info == 0)
            {
                return null;
            }

            m.Object(info, "PlayerInfo", "Definitions.Player");
            foreach (var value in Repeated(m, m.RequiredPointer(info + 0x18), 256))
            {
                m.Object(value, "PlayerLevel", "Definitions.Player");
                if (m.Int(value + 0x18) != level)
                {
                    continue;
                }

                var maximum = m.Int(value + 0x48);
                if (maximum is < 1 or > 1000000)
                {
                    throw Invalid("maximum energy");
                }

                return maximum;
            }

            return null;
        }

        private static string ReadObjectives(FrameRead m, int mission)
        {
            if (mission == 0)
            {
                return "";
            }

            var ui = Singleton(m, "BaseUiRoot", "BaseUiRoot", "Mdl.Ui", 0, "UiRoot", "Mdl.Ui");
            var menuStack = ui == 0 ? 0 : m.Pointer(ui + 0x2E8);
            if (menuStack == 0)
            {
                return "";
            }

            var result = new List<string>();
            foreach (var menu in m.List(m.Pointer(menuStack + 0x48), 32))
            {
                if (menu == 0 || !m.IsClass(menu, "HudMenu", "Mdl.Ui") || m.Pointer(menu + 0x10) == 0)
                {
                    continue;
                }

                var tracker = m.Pointer(menu + 0x128);
                if (tracker == 0 || m.Pointer(tracker + 0x10) == 0)
                {
                    continue;
                }

                m.Object(tracker, "HudQuestTracker", "Mdl.Ui");
                var slot = m.Pointer(tracker + 0x28);
                if (slot == 0 || m.Int(m.Object(slot, "MissionSlot", "Meta.Missions") + 0x1C) != mission)
                {
                    continue;
                }

                foreach (var description in m.List(m.Pointer(tracker + 0x30), 64))
                {
                    m.Object(description, "DescriptionInfo", "Meta.Missions");
                    var objective = m.Pointer(description + 0x20);
                    if (objective == 0 || !m.Bool(description + 0x19))
                    {
                        continue;
                    }

                    m.Object(objective, "MissionObjective", "Meta.Missions");
                    var data = m.Pointer(objective + 0x70);
                    if (data == 0)
                    {
                        continue;
                    }

                    m.Object(data, "MissionObjectiveData", "Definitions.Items");
                    var text = GameText.GetObjective(m.String(m.Pointer(data + 0x18)));
                    if (text.Length == 0)
                    {
                        continue;
                    }

                    if (m.Bool(description + 0x28) && m.Bool(description + 0x30))
                    {
                        var current = BitConverter.ToSingle(m.Bytes(description + 0x2C, 4));
                        var total = BitConverter.ToSingle(m.Bytes(description + 0x34, 4));
                        if (!float.IsFinite(current) || !float.IsFinite(total) || current < 0 || total < 0 || current > 100000000 || total > 100000000)
                        {
                            throw Invalid("objective progress");
                        }

                        text += FormattableString.Invariant($" ({current:0.#}/{total:0.#})");
                    }
                    else if (m.Bool(description + 0x18))
                    {
                        text += " (Completed)";
                    }

                    result.Add(text);
                }

                break;
            }

            return string.Join(" | ", result);
        }

        private static InvalidDataException Invalid(string field)
        {
            return new InvalidDataException($"Disney Dreamlight Valley: invalid or changing {field}.");
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
