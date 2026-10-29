using System.Text;

namespace MultiPresence.Models.SXSG
{
    internal static class MemoryReader
    {
        private static readonly Dictionary<ulong, string> types = new Dictionary<ulong, string>();
        private static ulong applicationAddress;
        private static ulong moduleBase;
        private static int moduleSize;
        private static int processId;
        private static DateTime nextScan;
        private static ulong titleMode;
        private static bool shadowSelected;

        public static void Reset()
        {
            types.Clear();
            applicationAddress = 0;
            moduleBase = 0;
            moduleSize = 0;
            processId = 0;
            nextScan = DateTime.MinValue;
            titleMode = 0;
            shadowSelected = false;
        }

        public static bool IsShadowGenerations()
        {
            ResolveAddress();
            var memory = new FrameRead();
            var (_, _, mode) = ReadApplication(memory);
            var state = GetState(memory, mode);
            memory.Validate();
            return state != "OtherGame";
        }

        public static GameSnapshot ReadSnapshot()
        {
            ResolveAddress();
            var memory = new FrameRead();
            var snapshot = ReadFrame(memory);
            memory.Validate();
            return snapshot;
        }

        private static GameSnapshot ReadFrame(FrameRead m)
        {
            var (_, manager, mode) = ReadApplication(m);
            var state = GetState(m, mode);
            if (state != "Ingame")
            {
                return new GameSnapshot() { State = state };
            }

            var level = FindNamed(m, manager + 0x150, 256, 0x38, "LevelInfo", true);
            if (level == 0)
            {
                return new GameSnapshot();
            }

            var stageData = m.Pointer(level + 0x78);
            if (stageData == 0)
            {
                return new GameSnapshot();
            }

            var stageId = m.Text(m.Pointer(stageData + 0x18), 32);
            if (!GameText.IsKnownStage(stageId))
            {
                return new GameSnapshot();
            }

            var rings = 0;
            var player = FindNamed(m, manager + 0x130, 32768, 0xB8, "Shadow", false);
            if (player != 0)
            {
                var component = FindNamed(m, player + 0x58, 128, 0x48, "GOCPlayerBlackboard", true);
                if (component != 0)
                {
                    var blackboard = m.Pointer(component + 0x80);
                    if (blackboard != 0)
                    {
                        var item = FindType(m, blackboard + 0x18, 64, ".?AVBlackboardItem@player@app@@");
                        if (item != 0)
                        {
                            rings = m.Int(item + 0x28);
                        }
                    }
                }
            }

            var time = m.Int(level + 0xD8);
            if (rings is < 0 or > 99999 || time is < 0 or > 599999)
            {
                throw Invalid("rings or time");
            }

            var location = GameText.GetLocation(stageId);
            var act = GameText.GetAct(stageId);
            return new GameSnapshot()
            {
                State = "Ingame",
                StageId = stageId,
                Stage = act.Length == 0 ? location : location + " / " + act,
                Mode = GameText.GetMode(stageId),
                Character = "Shadow",
                Rings = rings,
                Time = time / 100.0
            };
        }

        private static string GetState(FrameRead m, ulong mode)
        {
            var name = GetTypeName(m, mode);
            if (name is ".?AVGameModeWhiteSpace@game@app@@" or ".?AVGameModeStage@game@app@@" or ".?AVGameModeOpening@game@app@@" or ".?AVGameModeStaffRoll@game@app@@")
            {
                m.TitleMode = 0;
                m.ShadowSelected = true;
                return "Ingame";
            }

            if (name != ".?AVGameModeTitle@game@app@@")
            {
                m.TitleMode = 0;
                m.ShadowSelected = false;
                return "OtherGame";
            }

            if (m.TitleMode != 0 && m.TitleMode != mode)
            {
                m.ShadowSelected = false;
            }

            m.TitleMode = mode;
            var hsm = FindType(m, mode + 0xB0, 64, ".?AVGameModeHsmExtension@game@app@@");
            if (hsm != 0)
            {
                foreach (var entry in m.Pointers(hsm + 0x38, 16))
                {
                    var type = GetTypeName(m, entry);
                    if (type == ".?AVTitleTopMenu@State@GameModeTitle@game@app@@")
                    {
                        m.ShadowSelected = false;
                    }
                    else if (type == ".?AVTitleSelectMenu@State@GameModeTitle@game@app@@")
                    {
                        m.ShadowSelected = m.Flag(entry + 0x30);
                    }
                    else if (type == ".?AVOptionMenu@State@GameModeTitle@game@app@@")
                    {
                        m.ShadowSelected = (m.Byte(mode + 0x120) & 9) == 9;
                    }
                }
            }

            return m.ShadowSelected ? "MainMenu" : "OtherGame";
        }

        private static (ulong Application, ulong Manager, ulong Mode) ReadApplication(FrameRead m)
        {
            var application = m.Pointer(applicationAddress);
            if (application == 0)
            {
                return (0, 0, 0);
            }

            var manager = m.Pointer(application + 0x70);
            var sequence = FindType(m, application + 0x88, 32, ".?AVApplicationSequenceExtension@game@app@@");
            return (application, manager, sequence == 0 ? 0 : m.Pointer(sequence + 0x78));
        }

        private static ulong FindType(FrameRead m, ulong array, int maximum, string type)
        {
            foreach (var item in m.Pointers(array, maximum))
            {
                if (item != 0 && GetTypeName(m, item) == type)
                {
                    return item;
                }
            }

            return 0;
        }

        private static ulong FindNamed(FrameRead m, ulong array, int maximum, ulong offset, string name, bool className)
        {
            foreach (var item in m.Pointers(array, maximum))
            {
                if (item == 0)
                {
                    continue;
                }

                var text = m.Pointer(item + offset);
                if (className && text != 0)
                {
                    text = m.Pointer(text);
                }

                if (text != 0 && m.Text(text, 64) == name)
                {
                    return item;
                }
            }

            return 0;
        }

        private static string GetTypeName(FrameRead m, ulong instance)
        {
            if (instance == 0)
            {
                return "";
            }

            var table = m.Pointer(instance);
            if (table < moduleBase + 8 || table >= moduleBase + (ulong)moduleSize)
            {
                throw Invalid("type table");
            }

            if (!types.TryGetValue(table, out var name))
            {
                var locator = m.Pointer(table - 8);
                if (locator < moduleBase || locator + 24 > moduleBase + (ulong)moduleSize)
                {
                    throw Invalid("type locator");
                }

                var rva = m.Int(locator + 12);
                if (rva < 0 || rva > moduleSize - 144)
                {
                    throw Invalid("type name");
                }

                name = m.Text(moduleBase + (uint)rva + 16, 128);
                types[table] = name;
            }

            return name;
        }

        private static void ResolveAddress()
        {
            var process = Hypervisor.Process ?? throw Invalid("process");
            var module = process.MainModule ?? throw Invalid("module");
            var address = (ulong)module.BaseAddress.ToInt64();
            if (processId != process.Id || moduleBase != address || moduleSize != module.ModuleMemorySize)
            {
                Reset();
                processId = process.Id;
                moduleBase = address;
                moduleSize = module.ModuleMemorySize;
            }

            if (applicationAddress != 0)
            {
                return;
            }

            if (DateTime.UtcNow < nextScan)
            {
                throw Invalid("pattern scan (retry pending)");
            }

            nextScan = DateTime.UtcNow.AddSeconds(30);
            var header = Hypervisor.ReadArray(moduleBase, 0x1000, true);
            var pe = BitConverter.ToInt32(header, 0x3C);
            if (BitConverter.ToUInt16(header) != 0x5A4D || pe < 0x40 || pe > 0xE00 || BitConverter.ToUInt32(header, pe) != 0x4550)
            {
                throw Invalid("PE header");
            }

            var sections = BitConverter.ToUInt16(header, pe + 6);
            var start = pe + 24 + BitConverter.ToUInt16(header, pe + 20);
            if (sections is 0 or > 96 || start + sections * 40 > header.Length)
            {
                throw Invalid("PE sections");
            }

            var matches = new HashSet<ulong>();
            const int chunk = 1024 * 1024;
            var overlap = PatternCatalog.Application.Split(' ').Length - 1;
            for (var section = 0; section < sections; section++)
            {
                var offset = start + section * 40;
                if ((BitConverter.ToUInt32(header, offset + 36) & 0x20000000) == 0)
                {
                    continue;
                }

                var size = BitConverter.ToUInt32(header, offset + 8);
                var rva = BitConverter.ToUInt32(header, offset + 12);
                if (rva >= moduleSize || size > moduleSize - rva)
                {
                    throw Invalid("section range");
                }

                for (long position = 0; position < size; position += chunk)
                {
                    var count = (int)Math.Min(chunk + overlap, size - position);
                    var bytes = Hypervisor.ReadArray(moduleBase + rva + (ulong)position, count, true);
                    var search = 0;
                    while (search < bytes.Length)
                    {
                        var found = Hypervisor.FindPattern(bytes.AsSpan(search), PatternCatalog.Application);
                        if (found < 0)
                        {
                            break;
                        }

                        search += found;
                        matches.Add(moduleBase + rva + (ulong)position + (ulong)search);
                        if (matches.Count > 1)
                        {
                            throw Invalid("ambiguous application pattern");
                        }

                        search++;
                    }
                }
            }

            if (matches.Count != 1)
            {
                throw Invalid("missing application pattern");
            }

            var match = matches.Single();
            var target = (ulong)((long)match + 7 + Hypervisor.Read<int>(match + 3, true));
            if (target < moduleBase || target > moduleBase + (ulong)moduleSize - 8)
            {
                throw Invalid("application address");
            }

            applicationAddress = target;
        }

        private static InvalidDataException Invalid(string field)
        {
            return new InvalidDataException($"Shadow Generations: invalid or changing {field}.");
        }

        private sealed class FrameRead
        {
            private readonly Dictionary<ulong, ulong> pointers = new Dictionary<ulong, ulong>();
            private readonly Dictionary<ulong, byte> flags = new Dictionary<ulong, byte>();
            public ulong TitleMode { get; set; } = titleMode;
            public bool ShadowSelected { get; set; } = shadowSelected;

            public byte[] Bytes(ulong address, int size)
            {
                if (address < 0x10000 || address > 0x00007FFFFFFFFFFF - (ulong)size || size < 0 || size > 262144)
                {
                    throw Invalid("memory range");
                }

                return Hypervisor.ReadArray(address, size, true);
            }

            public int Int(ulong address)
            {
                return BitConverter.ToInt32(Bytes(address, 4));
            }

            public bool Flag(ulong address)
            {
                var value = Byte(address);
                if (value > 1)
                {
                    throw Invalid("selection flag");
                }

                return value == 1;
            }

            public byte Byte(ulong address)
            {
                var value = Bytes(address, 1)[0];
                flags[address] = value;
                return value;
            }

            public ulong Pointer(ulong address)
            {
                if (!pointers.TryGetValue(address, out var value))
                {
                    pointers[address] = value = BitConverter.ToUInt64(Bytes(address, 8));
                }

                return value;
            }

            public string Text(ulong address, int length)
            {
                var data = Bytes(address, length);
                var end = Array.IndexOf(data, (byte)0);
                return end < 0 ? "" : Encoding.ASCII.GetString(data, 0, end);
            }

            public IEnumerable<ulong> Pointers(ulong address, int maximum)
            {
                var data = Pointer(address);
                var count = Pointer(address + 8);
                if (count > (ulong)maximum || count != 0 && data == 0)
                {
                    throw Invalid("array");
                }

                for (ulong index = 0; index < count; index++)
                {
                    yield return Pointer(data + index * 8);
                }
            }

            public void Validate()
            {
                foreach (var (address, expected) in flags)
                {
                    if (Hypervisor.Read<byte>(address, true) != expected)
                    {
                        throw Invalid("selection");
                    }
                }

                foreach (var (address, expected) in pointers)
                {
                    if (Hypervisor.Read<ulong>(address, true) != expected)
                    {
                        throw Invalid("frame");
                    }
                }

                titleMode = TitleMode;
                shadowSelected = ShadowSelected;
            }
        }
    }
}
