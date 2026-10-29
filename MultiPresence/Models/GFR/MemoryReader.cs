using System.Diagnostics;
using System.Text;

namespace MultiPresence.Models.GFR
{
    internal sealed record GameSnapshot
    {
        public bool IsInGame { get; init; }
        public int CharacterId { get; init; }
        public int Level { get; init; }
        public int StageId { get; init; }
        public int Round { get; init; }
        public int Cycle { get; init; }
        public int Copper { get; init; }
        public int SoulEssence { get; init; }
        public double Hp { get; init; }
        public double MaxHp { get; init; }
        public double Shield { get; init; }
        public double MaxShield { get; init; }
    }

    internal static class MemoryReader
    {
        private const string Pattern = "48 8B 05 ?? ?? ?? ?? 8B 54 24 40 48 8B 88 ?? ?? ?? ?? 4C 8B 41 08 49 8B C8 49 8B 40 08 80 78 19 00";
        private static readonly Dictionary<ulong, string> typeNames = new Dictionary<ulong, string>();
        private static int processId, moduleSize;
        private static ulong moduleBase, logicSlot, managerOffset;
        private static DateTime nextScan;

        public static void Reset()
        {
            typeNames.Clear();
            processId = moduleSize = 0;
            moduleBase = logicSlot = managerOffset = 0;
            nextScan = DateTime.MinValue;
        }

        public static GameSnapshot ReadSnapshot()
        {
            ResolveAddresses();
            for (var attempt = 0; ; attempt++)
            {
                try
                {
                    var frame = new FrameRead();
                    var snapshot = ReadFrame(frame);
                    frame.Validate();
                    return snapshot;
                }
                catch (SnapshotChangedException) when (attempt < 2)
                {
                }
            }
        }

        private static GameSnapshot ReadFrame(FrameRead frame)
        {
            var logic = frame.WatchPointer(logicSlot);
            if (logic == 0)
            {
                return new GameSnapshot();
            }

            var manager = frame.WatchPointer(logic + managerOffset);
            if (manager == 0)
            {
                return new GameSnapshot();
            }

            var head = frame.WatchPointer(manager + 8);
            var root = frame.WatchPointer(head + 8);
            var pending = new Stack<ulong>();
            var visited = new HashSet<ulong>();
            pending.Push(root);
            ulong player = 0;
            while (pending.Count != 0)
            {
                var node = pending.Pop();
                if (node == head || Byte(node + 0x19) != 0)
                {
                    continue;
                }

                if (!visited.Add(node) || visited.Count > 256)
                {
                    throw Invalid("invalid logic object tree");
                }

                pending.Push(Pointer(node));
                pending.Push(Pointer(node + 0x10));
                var native = frame.WatchPointer(node + 0x28);
                if (native == 0 || (UInt(native + 8) & 3) != 3)
                {
                    continue;
                }

                var instance = frame.WatchPointer(native + 0x18);
                if (frame.TypeName(instance) != "CPlayer")
                {
                    continue;
                }

                if (player != 0)
                {
                    throw Invalid("ambiguous local player");
                }

                player = instance;
            }

            if (player == 0)
            {
                return new GameSnapshot();
            }

            var playerData = frame.Attributes(player, "CPlayer");
            var account = frame.Integer(Get(playerData, "m_AccountID"));
            if (account <= 0)
            {
                return new GameSnapshot();
            }

            var local = FindLogicObject(frame, head, account);
            if (local == 0 || frame.TypeName(local) != "CLogicPlayer")
            {
                return new GameSnapshot();
            }

            var localData = frame.Attributes(local, "CLogicPlayer");
            var gameId = frame.Integer(Get(localData, "m_GameID"));
            if (gameId <= 0 || frame.Integer(Get(localData, "m_PlayerID")) != account)
            {
                return new GameSnapshot();
            }

            var game = frame.WeakTarget(Get(localData, "m_Game"));
            var hero = frame.WeakTarget(Get(localData, "m_HeroObj"));
            if (game == 0 || hero == 0)
            {
                return new GameSnapshot();
            }

            var gameData = frame.Attributes(game, "CLFrameGame");
            var heroData = frame.Attributes(hero, "CCtrlHero");
            if (frame.Integer(Get(gameData, "m_ID")) != gameId || frame.Integer(Get(heroData, "m_GameID")) != gameId ||
                frame.Integer(Get(heroData, "m_PlayerID")) != account || frame.Integer(Get(gameData, "m_ReleaseFlag")) != 0 ||
                frame.Integer(Get(heroData, "m_ReleaseFlag")) != 0 || frame.Integer(Get(heroData, "m_OnGame")) != 1)
            {
                return new GameSnapshot();
            }

            var status = frame.Attributes(Get(gameData, "m_Status"), "CGameStatus");
            if (frame.Integer(Get(status, "m_Status")) is not 2 and not 3)
            {
                return new GameSnapshot();
            }

            var war = frame.Attributes(Get(gameData, "m_WarMgr"), "CWarManager");
            var components = frame.Dictionary(Get(war, "m_Component"));
            if (!components.TryGetValue("LevelCtrl", out var levelController))
            {
                return new GameSnapshot();
            }

            var level = frame.Attributes(levelController, "CLevelCtrlElement");
            var currentNode = Get(level, "m_CurNode");
            if (frame.TypeName(currentNode) == "NoneType")
            {
                return new GameSnapshot();
            }

            var nodeData = frame.Attributes(currentNode);
            var attributes = frame.Dictionary(Get(heroData, "m_PrivateAttr"));
            var hp = Math.Max(0, frame.Number(Get(heroData, "m_HP")) / 100);
            var shield = Math.Max(0, frame.Number(Get(heroData, "m_Shield")) / 100);
            var maxHp = ReadMaximum(frame, Get(attributes, "HPMax"));
            var maxShield = ReadMaximum(frame, Get(attributes, "ShieldMax"));
            if (maxHp <= 0 || maxShield < 0 || hp > 10000000 || maxHp > 10000000 || shield > 10000000 || maxShield > 10000000)
            {
                throw Invalid("invalid health values");
            }

            var snapshot = new GameSnapshot()
            {
                IsInGame = true,
                CharacterId = frame.Integer(Get(heroData, "m_SID")),
                Level = frame.Integer(Get(playerData, "m_Grade")),
                StageId = frame.Integer(Get(nodeData, "m_Level")),
                Round = frame.Integer(Get(war, "m_Round")),
                Cycle = frame.Integer(Get(war, "m_Cycle")),
                Copper = frame.Integer(Get(heroData, "m_WarCash")),
                SoulEssence = frame.Integer(Get(heroData, "m_WarGSCash")),
                Hp = hp,
                MaxHp = maxHp,
                Shield = shield,
                MaxShield = maxShield
            };
            if (snapshot.CharacterId <= 0 || snapshot.Level is < 0 or > 10000 || snapshot.StageId <= 0 ||
                snapshot.Round is < 0 or > 100 || snapshot.Cycle is < 0 or > 10000 || snapshot.Copper < 0 || snapshot.SoulEssence < 0)
            {
                throw Invalid("invalid player values");
            }

            return snapshot;
        }

        private static double ReadMaximum(FrameRead frame, ulong instance)
        {
            var attributes = frame.Attributes(instance);
            var forced = Get(attributes, "m_ForceCurValue");
            return frame.Number(frame.TypeName(forced) == "NoneType" ? Get(attributes, "m_CurValue") : forced) / 100;
        }

        private static ulong FindLogicObject(FrameRead frame, ulong head, int id)
        {
            var node = Pointer(head + 8);
            for (var depth = 0; depth < 128; depth++)
            {
                if (node == head || Byte(node + 0x19) != 0)
                {
                    return 0;
                }

                var key = UInt(node + 0x20);
                if (key == (uint)id)
                {
                    var native = frame.WatchPointer(node + 0x28);
                    return native != 0 && (UInt(native + 8) & 3) == 3 ? frame.WatchPointer(native + 0x18) : 0;
                }

                node = Pointer(node + ((uint)id < key ? 0UL : 0x10UL));
            }

            throw Invalid("logic object tree depth");
        }

        private static ulong Get(Dictionary<string, ulong> dictionary, string name)
        {
            return dictionary.TryGetValue(name, out var value) ? value : throw Invalid("missing field: " + name);
        }

        private sealed class FrameRead
        {
            private readonly Dictionary<ulong, Dictionary<string, ulong>> dictionaries = new Dictionary<ulong, Dictionary<string, ulong>>();
            private readonly Dictionary<ulong, byte[]> dictionaryHeaders = new Dictionary<ulong, byte[]>();
            private readonly Dictionary<ulong, ulong> pointers = new Dictionary<ulong, ulong>();

            public ulong WatchPointer(ulong address)
            {
                var value = Pointer(address);
                if (pointers.TryGetValue(address, out var previous) && previous != value)
                {
                    throw new SnapshotChangedException("pointer changed during read");
                }

                pointers[address] = value;
                return value;
            }

            public ulong WeakTarget(ulong instance)
            {
                var type = TypeName(instance);
                if (type == "NoneType")
                {
                    return 0;
                }

                if (type is not "weakproxy" and not "weakcallableproxy")
                {
                    throw Invalid("unexpected weak reference");
                }

                var target = WatchPointer(instance + 0x10);
                return TypeName(target) == "NoneType" ? 0 : target;
            }

            public string TypeName(ulong instance)
            {
                if (instance == 0)
                {
                    return "NULL";
                }

                var type = Pointer(instance + 8);
                if (!typeNames.TryGetValue(type, out var name))
                {
                    var data = Bytes(Pointer(type + 0x18), 128);
                    var end = Array.IndexOf(data, (byte)0);
                    if (end <= 0)
                    {
                        throw Invalid("invalid Python type name");
                    }

                    name = Encoding.UTF8.GetString(data, 0, end);
                    typeNames[type] = name;
                }

                return name;
            }

            public Dictionary<string, ulong> Attributes(ulong instance, string expectedType = "")
            {
                var type = TypeName(instance);
                if (instance == 0 || type == "NoneType" || (expectedType.Length != 0 && type != expectedType))
                {
                    throw Invalid("unexpected Python object: " + type);
                }

                var offset = Hypervisor.Read<long>(Pointer(instance + 8) + 0x120, true);
                if (offset < 0x10 || offset > 0x100000 || (offset & 7) != 0)
                {
                    throw Invalid("invalid Python attribute offset");
                }

                return Dictionary(WatchPointer(instance + (ulong)offset));
            }

            public Dictionary<string, ulong> Dictionary(ulong instance)
            {
                if (dictionaries.TryGetValue(instance, out var cached))
                {
                    return cached;
                }

                if (TypeName(instance) != "dict")
                {
                    throw Invalid("expected Python dictionary");
                }

                var header = Bytes(instance + 0x10, 32);
                var used = BitConverter.ToInt64(header, 0);
                var keys = BitConverter.ToUInt64(header, 16);
                var values = BitConverter.ToUInt64(header, 24);
                var keyHeader = Bytes(keys, 40);
                var size = BitConverter.ToInt64(keyHeader, 8);
                var entries = BitConverter.ToInt64(keyHeader, 32);
                if (used < 0 || used > 32768 || size < 1 || size > 65536 || (size & (size - 1)) != 0 || entries < used || entries > size)
                {
                    throw Invalid("invalid Python dictionary size");
                }

                var indexSize = size <= 255 ? 1 : size <= 65535 ? 2 : 4;
                var data = Bytes(keys + 40 + (ulong)(size * indexSize), checked((int)entries * 24));
                var split = values == 0 ? Array.Empty<byte>() : Bytes(values, checked((int)entries * 8));
                var result = new Dictionary<string, ulong>();
                for (var index = 0; index < entries; index++)
                {
                    var key = BitConverter.ToUInt64(data, index * 24 + 8);
                    var value = values == 0 ? BitConverter.ToUInt64(data, index * 24 + 16) : BitConverter.ToUInt64(split, index * 8);
                    if (key != 0 && value != 0 && TypeName(key) == "str")
                    {
                        result.Add(String(key), value);
                    }
                }

                if (!header.AsSpan().SequenceEqual(Bytes(instance + 0x10, 32)) || !keyHeader.AsSpan().SequenceEqual(Bytes(keys, 40)))
                {
                    throw new SnapshotChangedException("Python dictionary changed during read");
                }

                dictionaries.Add(instance, result);
                dictionaryHeaders.Add(instance, header);
                return result;
            }

            private string String(ulong instance)
            {
                var length = Hypervisor.Read<long>(instance + 0x10, true);
                var state = UInt(instance + 0x20);
                var kind = (int)((state >> 2) & 7);
                var compact = (state & 0x20) != 0;
                var ascii = (state & 0x40) != 0;
                if (length < 0 || length > 512 || kind is not 1 and not 2 and not 4 || !compact)
                {
                    throw Invalid("invalid Python field name");
                }

                var data = Bytes(instance + (ascii ? 48UL : 72UL), checked((int)length * kind));
                return (kind == 1 ? Encoding.Latin1 : kind == 2 ? Encoding.Unicode : Encoding.UTF32).GetString(data);
            }

            public int Integer(ulong instance)
            {
                var value = Number(instance);
                if (value < int.MinValue || value > int.MaxValue || value != Math.Truncate(value))
                {
                    throw Invalid("integer outside range");
                }

                return (int)value;
            }

            public double Number(ulong instance)
            {
                var type = TypeName(instance);
                double value;
                if (type == "float")
                {
                    value = Hypervisor.Read<double>(instance + 0x10, true);
                }
                else if (type is "int" or "bool")
                {
                    var size = Hypervisor.Read<long>(instance + 0x10, true);
                    if (size is < -2 or > 2)
                    {
                        throw Invalid("Python integer too large");
                    }

                    value = 0;
                    for (var index = 0; index < Math.Abs(size); index++)
                    {
                        var digit = UInt(instance + 0x18 + (ulong)index * 4);
                        if (digit >= (1U << 30))
                        {
                            throw Invalid("invalid Python integer digit");
                        }

                        value += digit * Math.Pow(2, index * 30);
                    }

                    if (size < 0)
                    {
                        value = -value;
                    }
                }
                else
                {
                    throw Invalid("expected Python number: " + type);
                }

                return double.IsFinite(value) ? value : throw Invalid("invalid numeric value");
            }

            public void Validate()
            {
                foreach (var pointer in pointers)
                {
                    if (Pointer(pointer.Key) != pointer.Value)
                    {
                        throw new SnapshotChangedException("game changed during read");
                    }
                }

                foreach (var dictionary in dictionaryHeaders)
                {
                    if (!dictionary.Value.AsSpan().SequenceEqual(Bytes(dictionary.Key + 0x10, 32)))
                    {
                        throw new SnapshotChangedException("game values changed during read");
                    }
                }
            }
        }

        private static void ResolveAddresses()
        {
            var process = Hypervisor.Process ?? throw Invalid("process not attached");
            var module = process.Modules.Cast<ProcessModule>().FirstOrDefault(item => item.ModuleName.Equals("m1logic4.dll", StringComparison.OrdinalIgnoreCase)) ?? throw Invalid("logic module not loaded");
            var address = (ulong)module.BaseAddress.ToInt64();
            if (processId != process.Id || moduleBase != address || moduleSize != module.ModuleMemorySize)
            {
                Reset();
                processId = process.Id;
                moduleBase = address;
                moduleSize = module.ModuleMemorySize;
            }

            if (logicSlot != 0)
            {
                return;
            }

            if (DateTime.UtcNow < nextScan)
            {
                throw Invalid("waiting to retry pattern");
            }

            nextScan = DateTime.UtcNow.AddSeconds(30);
            var match = FindPattern();
            var slot = checked((ulong)((long)match + 7 + Hypervisor.Read<int>(match + 3, true)));
            var offset = UInt(match + 14);
            if (slot < moduleBase || slot + 8 > moduleBase + (ulong)moduleSize || offset is < 8 or > 0x10000 || (offset & 7) != 0)
            {
                throw Invalid("unexpected native object layout");
            }

            managerOffset = offset;
            logicSlot = slot;
        }

        private static ulong FindPattern()
        {
            var parts = Pattern.Split(' ').Select(part => part == "??" ? (byte?)null : Convert.ToByte(part, 16)).ToArray();
            var prefix = parts.TakeWhile(part => part.HasValue).Select(part => part!.Value).ToArray();
            var peOffset = UInt(moduleBase + 0x3C);
            if (peOffset > moduleSize - 24 || UInt(moduleBase + peOffset) != 0x4550)
            {
                throw Invalid("invalid module header");
            }

            var pe = moduleBase + peOffset;
            var count = Hypervisor.Read<ushort>(pe + 6, true);
            var table = pe + 24 + Hypervisor.Read<ushort>(pe + 20, true);
            if (count > 96 || table + (ulong)count * 40 > moduleBase + (ulong)moduleSize)
            {
                throw Invalid("invalid section table");
            }

            ulong match = 0;
            const int chunkSize = 1024 * 1024;
            for (var section = 0; section < count; section++)
            {
                var header = table + (ulong)section * 40;
                var size = UInt(header + 8);
                var rva = UInt(header + 12);
                if ((UInt(header + 36) & 0x20000000) == 0 || size < parts.Length)
                {
                    continue;
                }

                if ((ulong)rva + size > (ulong)moduleSize)
                {
                    throw Invalid("invalid executable section");
                }

                for (ulong offset = 0; offset < size; offset += chunkSize)
                {
                    var data = Bytes(moduleBase + rva + offset, (int)Math.Min((ulong)(chunkSize + parts.Length - 1), size - offset));
                    for (var start = 0; start <= data.Length - parts.Length;)
                    {
                        var found = data.AsSpan(start).IndexOf(prefix);
                        if (found < 0)
                        {
                            break;
                        }

                        var candidate = start + found;
                        start = candidate + 1;
                        if (candidate + parts.Length > data.Length || parts.Where((part, index) => part.HasValue && part.Value != data[candidate + index]).Any())
                        {
                            continue;
                        }

                        var address = moduleBase + rva + offset + (ulong)candidate;
                        if (match != 0 && match != address)
                        {
                            throw Invalid("ambiguous logic pattern");
                        }

                        match = address;
                    }
                }
            }

            return match != 0 ? match : throw Invalid("logic pattern not found");
        }

        private static byte[] Bytes(ulong address, int count)
        {
            if (count < 0 || count > 2 * 1024 * 1024 || address < 0x10000 || address > 0x7FFFFFFFFFFFUL - (ulong)count)
            {
                throw Invalid("invalid read range");
            }

            return count == 0 ? Array.Empty<byte>() : Hypervisor.ReadArray(address, count, true);
        }

        private static ulong Pointer(ulong address)
        {
            return BitConverter.ToUInt64(Bytes(address, 8));
        }

        private static uint UInt(ulong address)
        {
            return BitConverter.ToUInt32(Bytes(address, 4));
        }

        private static byte Byte(ulong address)
        {
            return Bytes(address, 1)[0];
        }

        private static InvalidDataException Invalid(string message)
        {
            return new InvalidDataException("GFR: " + message + ".");
        }

        private sealed class SnapshotChangedException : IOException
        {
            public SnapshotChangedException(string message) : base("GFR: " + message + ".")
            {
            }
        }
    }
}
