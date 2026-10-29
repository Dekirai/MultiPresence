using MultiPresence.Infrastructure;
using System.Diagnostics;
using System.Text;

namespace MultiPresence.Models.HL
{
    internal sealed record GameSnapshot
    {
        public bool IsInGame { get; init; }
        public string Character { get; init; } = "";
        public int Level { get; init; }
        public string Location { get; init; } = "";
        public int Difficulty { get; init; }
        public string QuestId { get; init; } = "";
        public bool? Modded { get; init; }
    }

    internal static class MemoryReader
    {
        private static readonly Dictionary<string, (string Pattern, int Operand, int Length)> Patterns = new Dictionary<string, (string, int, int)>
        {
            { "Names", ("48 8D 15 ?? ?? ?? ?? EB ?? 48 8D 0D ?? ?? ?? ?? E8 ?? ?? ?? ?? 48 8B D0 C6 05 ?? ?? ?? ?? 01 48 8B 44 24 30 48 C1 E8 20 03 C0 48 03 44 DA 10", 3, 7) },
            { "World", ("48 8B 0D ?? ?? ?? ?? 48 85 C9 74 ?? 48 8B 01 FF 90 60 01 00 00 48 8B C8 48 83 C4 28 E9 ?? ?? ?? ??", 3, 7) },
            { "Experience", ("48 8B 05 ?? ?? ?? ?? 48 85 C0 75 ?? E8 ?? ?? ?? ?? 48 8B 05 ?? ?? ?? ?? 8B 40 44 89 87 00 01 00 00", 3, 7) },
            { "Profile", ("48 8B 0D ?? ?? ?? ?? E8 ?? ?? ?? ?? 4C 8B C0 48 05 98 03 00 00 48 63 10 85 D2 78 ?? 48 8B 0D ?? ?? ?? ?? 3B 91 60 08 00 00 7D ?? 48 69 C2 70 03 00 00 49 03 80 08 07 00 00", 3, 7) },
            { "Mission", ("48 8B 05 ?? ?? ?? ?? 48 8B DA 48 85 C0 75 ?? E8 ?? ?? ?? ?? 48 8B 05 ?? ?? ?? ?? 48 8B 88 28 06 00 00 48 85 C9 74 ?? 48 8D 54 24 40 E8 ?? ?? ?? ?? 48 8B 08 B0 01 48 89 0B", 3, 7) },
            { "UI", ("48 8B 0D ?? ?? ?? ?? 48 85 C9 75 0C E8 ?? ?? ?? ?? 48 8B 0D ?? ?? ?? ?? 0F B6 93 84 03 00 00", 3, 7) },
            { "OfficialMods", (ModStatus.OfficialPattern, 7, 11) },
            { "UnofficialMods", (ModStatus.UnofficialPattern, 2, 7) }
        };
        private static readonly Dictionary<string, ulong> addresses = new Dictionary<string, ulong>();
        private static readonly Dictionary<uint, string> names = new Dictionary<uint, string>();
        private static readonly Dictionary<(ulong, string), uint> properties = new Dictionary<(ulong, string), uint>();
        private static int processId, moduleSize;
        private static ulong moduleBase, currentWorld;
        private static DateTime nextScan;

        public static void Reset()
        {
            addresses.Clear();
            names.Clear();
            properties.Clear();
            processId = moduleSize = 0;
            moduleBase = currentWorld = 0;
            nextScan = DateTime.MinValue;
        }

        public static GameSnapshot ReadSnapshot()
        {
            ResolveAddresses();
            for (var attempt = 0; ; attempt++)
            {
                try
                {
                    return ReadFrame();
                }
                catch (SnapshotChangedException) when (attempt < 2)
                {
                }
            }
        }

        private static GameSnapshot ReadFrame()
        {
            var frame = new FrameRead();
            var world = frame.Pointer(addresses["World"]);
            if (world != currentWorld)
            {
                properties.Clear();
                currentWorld = world;
            }

            if (world == 0 || !IsA(world, "World"))
            {
                return new GameSnapshot();
            }

            var instance = frame.Pointer(world + Property(world, "OwningGameInstance"));
            if (instance == 0)
            {
                return new GameSnapshot();
            }

            var playersAddress = instance + Property(instance, "LocalPlayers");
            var players = frame.Pointer(playersAddress);
            var playerCount = frame.Integer(playersAddress + 8);
            if (players == 0 || playerCount == 0)
            {
                return new GameSnapshot();
            }

            if (playerCount != 1)
            {
                throw Invalid("unexpected local player count");
            }

            var player = frame.Pointer(players);
            var controller = frame.Pointer(player + Property(player, "PlayerController"));
            if (controller == 0)
            {
                return new GameSnapshot();
            }

            var pawn = frame.Pointer(controller + Property(controller, "Pawn"));
            if (pawn == 0 || !IsA(pawn, "Biped_Player") ||
                frame.Pointer(pawn + Property(pawn, "Controller")) != controller ||
                Name(frame.Unsigned(controller + Property(controller, "StateName"))) != "Playing")
            {
                return new GameSnapshot();
            }

            var profileManager = frame.Pointer(addresses["Profile"]);
            if (profileManager == 0 || frame.Byte(profileManager + 0x150) != 1)
            {
                return new GameSnapshot();
            }

            var profile = profileManager + 0x150;
            var index = frame.Integer(profile + 0x398);
            var count = frame.Integer(profileManager + 0x860);
            if (index == -1 || count == 0)
            {
                return new GameSnapshot();
            }

            if (index < 0 || count < 1 || count > 1024 || index >= count)
            {
                throw Invalid("invalid character profile");
            }

            var records = frame.Pointer(profile + 0x708);
            var record = records + (ulong)index * 0x370;
            if (records == 0 || frame.Integer(record) < 0)
            {
                throw Invalid("missing character profile");
            }

            var character = frame.String(record + 8);
            var difficulty = frame.Byte(record + 0x5C);
            var experience = frame.Pointer(addresses["Experience"]);
            if (experience == 0 || !IsA(experience, "ExperienceManager"))
            {
                throw Invalid("missing experience manager");
            }

            var level = frame.Integer(experience + 0x44);
            if (character.Length == 0 || level < 1 || level > 10000 || difficulty > 3)
            {
                throw Invalid("invalid player values");
            }

            var missionManager = frame.Pointer(addresses["Mission"]);
            if (missionManager == 0 || !IsA(missionManager, "MissionManager"))
            {
                throw Invalid("missing mission manager");
            }

            var mission = frame.Pointer(missionManager + 0x628);
            var quest = "";
            if (mission != 0)
            {
                var definition = frame.Pointer(mission + 0x160);
                if (definition == 0)
                {
                    throw Invalid("missing mission definition");
                }

                quest = Name(frame.Unsigned(definition + 8));
            }

            var snapshot = new GameSnapshot()
            {
                IsInGame = true,
                Character = character,
                Level = level,
                Location = ReadLocation(frame, pawn),
                Difficulty = difficulty,
                QuestId = quest,
                Modded = ModStatus.Read(addresses["OfficialMods"], addresses["UnofficialMods"])
            };
            frame.Validate();
            return snapshot;
        }

        private static string ReadLocation(FrameRead frame, ulong pawn)
        {
            var ui = frame.Pointer(addresses["UI"]);
            if (ui == 0 || !IsA(ui, "UIManager"))
            {
                return "Unknown Location";
            }

            var map = frame.Pointer(ui + 0x458);
            if (map == 0 || !IsA(map, "MapSubSystem") ||
                frame.Integer(map + 0x49F8) != frame.Integer(pawn + 0xC))
            {
                return "Unknown Location";
            }

            var room = Locations.GetLocation(Name(frame.Unsigned(map + 0xF8)));
            var region = Locations.GetLocation(Name(frame.Unsigned(map + 0x120)));
            if (room == "Unknown Location")
            {
                return region;
            }

            return region == "Unknown Location" || room == region ? room : region + " / " + room;
        }

        private sealed class FrameRead
        {
            private readonly Dictionary<ulong, byte[]> values = new Dictionary<ulong, byte[]>();

            private byte[] Watch(ulong address, int size)
            {
                var value = Bytes(address, size);
                if (values.TryGetValue(address, out var previous) && !previous.AsSpan().SequenceEqual(value))
                {
                    throw new SnapshotChangedException();
                }

                values[address] = value;
                return value;
            }

            public ulong Pointer(ulong address)
            {
                return BitConverter.ToUInt64(Watch(address, 8));
            }

            public int Integer(ulong address)
            {
                return BitConverter.ToInt32(Watch(address, 4));
            }

            public uint Unsigned(ulong address)
            {
                return BitConverter.ToUInt32(Watch(address, 4));
            }

            public byte Byte(ulong address)
            {
                return Watch(address, 1)[0];
            }

            public string String(ulong address)
            {
                var data = Watch(address, 16);
                var pointer = BitConverter.ToUInt64(data);
                var count = BitConverter.ToInt32(data, 8);
                var capacity = BitConverter.ToInt32(data, 12);
                if (count < 0 || count > 256 || capacity < count || capacity > 65536)
                {
                    throw Invalid("invalid string size");
                }

                if (count == 0)
                {
                    return "";
                }

                var text = Encoding.Unicode.GetString(Watch(pointer, count * 2));
                return text.TrimEnd('\0').Trim();
            }

            public void Validate()
            {
                foreach (var value in values)
                {
                    if (!value.Value.AsSpan().SequenceEqual(Bytes(value.Key, value.Value.Length)))
                    {
                        throw new SnapshotChangedException();
                    }
                }
            }
        }

        private static bool IsA(ulong instance, string expected)
        {
            var type = Pointer(instance + 0x10);
            for (var depth = 0; type != 0 && depth < 32; depth++)
            {
                if (Name(UInt(type + 0x18)) == expected)
                {
                    return true;
                }

                type = Pointer(type + 0x40);
            }

            return false;
        }

        private static uint Property(ulong instance, string name)
        {
            var type = Pointer(instance + 0x10);
            var key = (type, name);
            if (properties.TryGetValue(key, out var cached))
            {
                return cached;
            }

            for (var depth = 0; type != 0 && depth < 32; depth++)
            {
                var field = Pointer(type + 0x50);
                for (var index = 0; field != 0 && index < 4096; index++)
                {
                    if (Name(UInt(field + 0x28)) == name)
                    {
                        var offset = UInt(field + 0x4C);
                        if (offset < 0x28 || offset > 0x100000)
                        {
                            throw Invalid("invalid property offset");
                        }

                        properties.Add(key, offset);
                        return offset;
                    }

                    field = Pointer(field + 0x20);
                }

                type = Pointer(type + 0x40);
            }

            throw Invalid("missing property: " + name);
        }

        private static string Name(uint id)
        {
            if (names.TryGetValue(id, out var cached))
            {
                return cached;
            }

            var pool = addresses["Names"];
            var block = id >> 16;
            if (block > 8191 || block > UInt(pool + 8))
            {
                throw Invalid("invalid name block");
            }

            var entries = Pointer(pool + 0x10 + block * 8);
            var entry = entries + (id & 0xFFFF) * 2;
            var header = BitConverter.ToUInt16(Bytes(entry, 2));
            var length = header >> 6;
            if (entries == 0 || length == 0 || length > 1024)
            {
                throw Invalid("invalid name entry");
            }

            var wide = (header & 1) != 0;
            var value = (wide ? Encoding.Unicode : Encoding.UTF8).GetString(Bytes(entry + 2, length * (wide ? 2 : 1)));
            if (value.Any(char.IsControl))
            {
                throw Invalid("invalid name text");
            }

            names[id] = value;
            return value;
        }

        private static void ResolveAddresses()
        {
            var process = Hypervisor.Process ?? throw Invalid("process not attached");
            var module = process.MainModule ?? throw Invalid("main module not loaded");
            var address = (ulong)module.BaseAddress.ToInt64();
            if (processId != process.Id || moduleBase != address || moduleSize != module.ModuleMemorySize)
            {
                Reset();
                processId = process.Id;
                moduleBase = address;
                moduleSize = module.ModuleMemorySize;
            }

            if (addresses.Count == Patterns.Count)
            {
                return;
            }

            if (DateTime.UtcNow < nextScan)
            {
                throw Invalid("waiting to retry patterns");
            }

            nextScan = DateTime.UtcNow.AddSeconds(30);
            addresses.Clear();
            try
            {
                var matches = FindPatterns();
                foreach (var pattern in Patterns)
                {
                    var match = matches[pattern.Key];
                    var target = checked((ulong)((long)match + pattern.Value.Length + Hypervisor.Read<int>(match + (ulong)pattern.Value.Operand, true)));
                    if (target < moduleBase || target + 8 > moduleBase + (ulong)moduleSize)
                    {
                        throw Invalid("pattern target outside module");
                    }

                    addresses.Add(pattern.Key, target);
                }

                if (Name(0) != "None")
                {
                    throw Invalid("name layout changed");
                }
            }
            catch
            {
                addresses.Clear();
                names.Clear();
                properties.Clear();
                throw;
            }
        }

        private static Dictionary<string, ulong> FindPatterns()
        {
            var patterns = Patterns.ToDictionary(item => item.Key, item => SignaturePattern.Parse(item.Value.Pattern));
            var matches = new Dictionary<string, ulong>();
            var overlap = Patterns.Values.Max(item => item.Pattern.Split(' ').Length) - 1;
            var peOffset = UInt(moduleBase + 0x3C);
            if (peOffset > moduleSize - 24 || UInt(moduleBase + peOffset) != 0x4550)
            {
                throw Invalid("invalid module header");
            }

            var pe = moduleBase + peOffset;
            var count = BitConverter.ToUInt16(Bytes(pe + 6, 2));
            var table = pe + 24 + BitConverter.ToUInt16(Bytes(pe + 20, 2));
            if (count > 96 || table + (ulong)count * 40 > moduleBase + (ulong)moduleSize)
            {
                throw Invalid("invalid section table");
            }

            const int chunkSize = 1024 * 1024;
            for (var section = 0; section < count; section++)
            {
                var header = table + (ulong)section * 40;
                var size = UInt(header + 8);
                var rva = UInt(header + 12);
                if ((UInt(header + 36) & 0x20000000) == 0 || size == 0)
                {
                    continue;
                }

                if ((ulong)rva + size > (ulong)moduleSize)
                {
                    throw Invalid("invalid executable section");
                }

                for (ulong offset = 0; offset < size; offset += chunkSize)
                {
                    var data = Bytes(moduleBase + rva + offset, (int)Math.Min((ulong)(chunkSize + overlap), size - offset));
                    foreach (var pattern in patterns)
                    {
                        var start = 0;
                        while (start < data.Length)
                        {
                            var found = pattern.Value.IndexOf(data.AsSpan(start));
                            if (found < 0)
                            {
                                break;
                            }

                            var candidate = start + found;
                            var address = moduleBase + rva + offset + (ulong)candidate;
                            if (matches.TryGetValue(pattern.Key, out var previous) && previous != address)
                            {
                                throw Invalid("ambiguous pattern: " + pattern.Key);
                            }

                            matches[pattern.Key] = address;
                            start = candidate + 1;
                        }
                    }
                }
            }

            foreach (var pattern in patterns.Keys)
            {
                if (!matches.ContainsKey(pattern))
                {
                    throw Invalid("pattern not found: " + pattern);
                }
            }

            return matches;
        }

        private static byte[] Bytes(ulong address, int size)
        {
            if (size < 0 || size > 2 * 1024 * 1024 || address < 0x10000 || address > 0x7FFFFFFFFFFFUL - (ulong)size)
            {
                throw Invalid("invalid read range");
            }

            return size == 0 ? Array.Empty<byte>() : Hypervisor.ReadArray(address, size, true);
        }

        private static ulong Pointer(ulong address)
        {
            return BitConverter.ToUInt64(Bytes(address, 8));
        }

        private static uint UInt(ulong address)
        {
            return BitConverter.ToUInt32(Bytes(address, 4));
        }

        private static InvalidDataException Invalid(string message)
        {
            return new InvalidDataException("HL: " + message + ".");
        }

        private sealed class SnapshotChangedException : IOException
        {
            public SnapshotChangedException() : base("HL: game changed during read.")
            {
            }
        }
    }
}
