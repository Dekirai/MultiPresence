using System.Text;

namespace MultiPresence.Models.FFVIIR
{
    internal static class MemoryReader
    {
        private const string namesPattern = "48 8D 05 ?? ?? ?? ?? EB ?? 48 8D 0D ?? ?? ?? ?? E8 ?? ?? ?? ?? C6 05 ?? ?? ?? ?? 01 8B 80 78 09 01 00 48 8B CB 0F 28 35 ?? ?? ?? ?? 89 44 24 78 89 7C 24 7C E8 ?? ?? ?? ?? 48 8B 44 24 78 48 8D 0D ?? ?? ?? ?? 48 89 83 30 01 00 00 48 89 0B 48 89 B3 18 01 00 00 0F 11 B3 20 01 00 00 EB ?? 48 8B DF 4C 8B A4 24 30 02 00 00";
        private const string modePattern = "48 8B 0D ?? ?? ?? ?? C7 85 90 04 00 00 00 00 00 00 48 85 C9 74 ?? 48 8B 01 FF 90 40 01 00 00 48 85 C0 74 ??";
        private const string managerPattern = "4C 8B 05 ?? ?? ?? ?? 8B FA 0F B6 D9 4D 85 C0 75 ??";
        private const string chapterPattern = "32 DB E8 ?? ?? ?? ?? 48 8D 4C 24 30 0F B6 90 F5 37 04 00 E8 ?? ?? ?? ?? 48 8B 44 24 30 BA 01 00 00 00 48 39 45 0C 0F B6 CB 0F 44 CA 0F B6 D1 8B 4D 08 E8 ?? ?? ?? ??";
        private const string difficultyPattern = "40 53 48 83 EC 20 E8 ?? ?? ?? ?? 48 8B D8 E8 ?? ?? ?? ?? 80 BB F9 37 04 00 00 4C 8B C0 75 ?? 0F B6 90 A0 69 0B 00 85 D2 74 ?? 83 EA 01";
        private static readonly Dictionary<uint, string> names = new Dictionary<uint, string>();
        private static readonly Dictionary<(ulong, string), int> fields = new Dictionary<(ulong, string), int>();
        private static ulong namesAddress, modeSlot, managerSlot, moduleBase;
        private static int processId, moduleSize;
        private static DateTime nextScan;

        public static GameSnapshot ReadSnapshot()
        {
            ResolveAddresses();
            var mode = Pointer(modeSlot);
            if (!IsA(mode, "EndGameMode"))
            {
                return new GameSnapshot();
            }

            var stateAddress = Field(mode, "GameState", 8);
            var state = Pointer(stateAddress);
            if (!IsA(state, "EndGameState"))
            {
                return new GameSnapshot() { State = "Loading" };
            }

            var gameStateAddress = Field(state, "GameState", 1);
            var gameState = ReadByte(gameStateAddress);
            if (gameState is not (6 or 7 or 8 or 10))
            {
                return new GameSnapshot() { State = gameState is 5 or 9 ? "Loading" : "Default" };
            }

            var locationAddress = Field(state, "ActiveLocation", 16);
            var locationId = ReadString(locationAddress);
            if (locationId == "005-TITLB")
            {
                return new GameSnapshot();
            }

            var level = Pointer(mode + 0x20);
            var world = Pointer(level + 0x20);
            if (!IsA(world, "World") || Pointer(Field(world, "AuthorityGameMode", 8)) != mode)
            {
                return new GameSnapshot() { State = "Loading" };
            }

            var instanceAddress = Field(world, "OwningGameInstance", 8);
            var instance = Pointer(instanceAddress);
            if (!IsA(instance, "GameInstance"))
            {
                return new GameSnapshot() { State = "Loading" };
            }

            var playersAddress = Field(instance, "LocalPlayers", 16);
            var players = Pointer(playersAddress);
            var count = ReadInt(playersAddress + 8);
            var capacity = ReadInt(playersAddress + 12);
            if (players == 0 || count < 1 || count > capacity || capacity > 8)
            {
                return new GameSnapshot() { State = "Loading" };
            }

            var player = Pointer(players);
            if (!IsA(player, "LocalPlayer"))
            {
                return new GameSnapshot() { State = "Loading" };
            }

            var controllerAddress = Field(player, "PlayerController", 8);
            var controller = Pointer(controllerAddress);
            if (!IsA(controller, "EndPlayerController"))
            {
                return new GameSnapshot() { State = "Loading" };
            }

            var pawnAddress = Field(controller, "Pawn", 8);
            var pawn = Pointer(pawnAddress);
            var manager = Pointer(managerSlot);
            var native = pawn == 0 ? 0 : Pointer(pawn + 0xCB0);
            if (manager == 0 || native == 0 || !IsA(pawn, "EndCharacter"))
            {
                return new GameSnapshot() { State = "Loading" };
            }

            var characterId = ReadByte(native + 0x58);
            if (characterId > 6)
            {
                throw new InvalidDataException("FFVIIR: invalid character.");
            }

            var snapshot = new GameSnapshot()
            {
                State = "Ingame",
                CharacterId = characterId,
                Character = Characters.GetName(characterId),
                CharacterKey = Characters.GetParameterKey(characterId)
            };
            snapshot = CampaignReader.Read(manager, snapshot);
            if (snapshot.Chapter == 0 || (snapshot.Campaign == "INTERmission") != (characterId >= 5) || !PlayerStats.TryRead(manager, characterId, snapshot, out snapshot))
            {
                return new GameSnapshot() { State = "Loading" };
            }

            LocationReader.TryRead(world, pawn, locationId, out var layerId, out var groupId);
            snapshot = snapshot with
            {
                LocationId = locationId,
                MapId = locationId,
                LayerId = layerId,
                GroupId = groupId,
                Map = string.IsNullOrEmpty(Locations.GetLayer(locationId, layerId)) ? Locations.GetName(locationId) : Locations.GetLayer(locationId, layerId),
                Region = Locations.GetRegion(locationId, layerId),
                Location = Locations.GetLocation(locationId, layerId, groupId),
                Area = Locations.GetArea(locationId, layerId, groupId)
            };

            if (mode != Pointer(modeSlot) || state != Pointer(stateAddress) || gameState != ReadByte(gameStateAddress) || level != Pointer(mode + 0x20) || world != Pointer(level + 0x20) || instance != Pointer(instanceAddress) || players != Pointer(playersAddress) || player != Pointer(players) || controller != Pointer(controllerAddress) || pawn != Pointer(pawnAddress) || manager != Pointer(managerSlot) || native != Pointer(pawn + 0xCB0) || characterId != ReadByte(native + 0x58) || locationId != ReadString(locationAddress) || snapshot.ChapterId != CampaignReader.Read(manager, new GameSnapshot()).ChapterId)
            {
                return new GameSnapshot() { State = "Loading" };
            }

            return snapshot;
        }

        internal static ulong Field(ulong instance, string field, int size)
        {
            var type = Pointer(instance + 0x10);
            var key = (type, field);
            if (fields.TryGetValue(key, out var cached))
            {
                return instance + (ulong)cached;
            }

            for (var depth = 0; type != 0 && depth < 32; depth++, type = Pointer(type + 0x30))
            {
                var property = Pointer(type + 0x38);
                for (var count = 0; property != 0 && count < 2048; count++, property = Pointer(property + 0x28))
                {
                    if (ReadName(Pointer(property + 0x18)) != field)
                    {
                        continue;
                    }

                    var offset = ReadInt(property + 0x44);
                    if (offset < 0x28 || offset > 0x100000 || ReadInt(property + 0x34) != size)
                    {
                        throw new InvalidDataException("FFVIIR: unsupported property: " + field);
                    }

                    fields[key] = offset;
                    return instance + (ulong)offset;
                }
            }

            throw new InvalidDataException("FFVIIR: property not found: " + field);
        }

        internal static bool IsA(ulong instance, string expected)
        {
            if (instance == 0)
            {
                return false;
            }

            var type = Pointer(instance + 0x10);
            for (var depth = 0; type != 0 && depth < 32; depth++, type = Pointer(type + 0x30))
            {
                if (ReadName(Pointer(type + 0x18)) == expected)
                {
                    return true;
                }
            }

            return false;
        }

        internal static string ReadName(ulong packedName)
        {
            var index = (uint)packedName;
            if (names.TryGetValue(index, out var cached))
            {
                return cached;
            }

            var block = index >> 16;
            var currentBlock = Hypervisor.Read<uint>(namesAddress + 8, true);
            var cursor = Hypervisor.Read<uint>(namesAddress + 12, true);
            if (currentBlock >= 8192 || block > currentBlock || cursor > 0x20000)
            {
                throw new InvalidDataException("FFVIIR: invalid name pool.");
            }

            var start = Pointer(namesAddress + 0x10 + block * 8);
            var offset = (index & 0xFFFF) * 2;
            var limit = block == currentBlock ? cursor : 0x20000;
            if (start == 0 || offset + 2 > limit)
            {
                throw new InvalidDataException("FFVIIR: invalid name entry.");
            }

            var header = Hypervisor.Read<ushort>(start + offset, true);
            var length = header >> 6;
            var wide = (header & 1) != 0;
            var byteLength = length * (wide ? 2 : 1);
            if (length is < 1 or > 1023 || offset + 2 + byteLength > limit)
            {
                throw new InvalidDataException("FFVIIR: invalid name length.");
            }

            var value = (wide ? Encoding.Unicode : Encoding.UTF8).GetString(Hypervisor.ReadArray(start + offset + 2, byteLength, true));
            if (value.Any(char.IsControl))
            {
                throw new InvalidDataException("FFVIIR: invalid name characters.");
            }

            names[index] = value;
            return value;
        }

        private static string ReadString(ulong address)
        {
            var pointer = Pointer(address);
            var count = ReadInt(address + 8);
            var capacity = ReadInt(address + 12);
            if (count == 0)
            {
                return "";
            }

            if (pointer == 0 || count < 1 || count > capacity || capacity > 256)
            {
                throw new InvalidDataException("FFVIIR: invalid location string.");
            }

            return Encoding.Unicode.GetString(Hypervisor.ReadArray(pointer, (count - 1) * 2, true));
        }

        internal static ulong Pointer(ulong address)
        {
            return Hypervisor.Read<ulong>(address, true);
        }

        internal static int ReadInt(ulong address)
        {
            return Hypervisor.Read<int>(address, true);
        }

        internal static byte ReadByte(ulong address)
        {
            return Hypervisor.Read<byte>(address, true);
        }

        public static void Reset()
        {
            processId = moduleSize = 0;
            moduleBase = namesAddress = modeSlot = managerSlot = 0;
            nextScan = DateTime.MinValue;
            names.Clear();
            fields.Clear();
        }

        private static void ResolveAddresses()
        {
            var process = Hypervisor.Process ?? throw new InvalidDataException("FFVIIR: game is not running.");
            var module = process.MainModule ?? throw new InvalidDataException("FFVIIR: module not loaded.");
            var address = (ulong)module.BaseAddress.ToInt64();
            if (processId != process.Id || moduleBase != address || moduleSize != module.ModuleMemorySize)
            {
                Reset();
                processId = process.Id;
                moduleBase = address;
                moduleSize = module.ModuleMemorySize;
            }

            if (namesAddress != 0)
            {
                return;
            }

            if (DateTime.UtcNow < nextScan)
            {
                throw new InvalidDataException("FFVIIR: waiting to retry patterns.");
            }

            nextScan = DateTime.UtcNow.AddSeconds(30);
            ulong Resolve(string pattern)
            {
                var match = FindUniquePattern(moduleBase, moduleSize, pattern);
                var target = checked((ulong)((long)match + 7 + ReadInt(match + 3)));
                if (target < moduleBase || target + 8 > moduleBase + (ulong)moduleSize)
                {
                    throw new InvalidDataException("FFVIIR: pattern target is outside the module.");
                }

                return target;
            }

            var foundNames = Resolve(namesPattern);
            var foundMode = Resolve(modePattern);
            var foundManager = Resolve(managerPattern);
            FindUniquePattern(moduleBase, moduleSize, chapterPattern);
            FindUniquePattern(moduleBase, moduleSize, difficultyPattern);
            foreach (var pattern in PatternCatalog.Layouts)
            {
                FindUniquePattern(moduleBase, moduleSize, pattern);
            }

            modeSlot = foundMode;
            managerSlot = foundManager;
            namesAddress = foundNames;
        }
        private static ulong FindUniquePattern(ulong moduleBase, int moduleSize, string signature)
        {
            if (moduleSize < 64)
            {
                throw new InvalidDataException("FFVIIR: invalid module size.");
            }

            var peOffset = Hypervisor.Read<uint>(moduleBase + 0x3C, true);
            if (peOffset > (uint)moduleSize - 24)
            {
                throw new InvalidDataException("FFVIIR: invalid PE offset.");
            }

            var pe = moduleBase + peOffset;
            if (peOffset > (uint)moduleSize - 24 || Hypervisor.Read<uint>(pe, true) != 0x4550 || Hypervisor.Read<ushort>(pe + 4, true) != 0x8664)
            {
                throw new InvalidDataException("FFVIIR: invalid PE header.");
            }

            var sectionCount = Hypervisor.Read<ushort>(pe + 6, true);
            var optionalSize = Hypervisor.Read<ushort>(pe + 20, true);
            var sectionTable = pe + 24 + optionalSize;
            if (sectionTable + (ulong)sectionCount * 40 > moduleBase + (ulong)moduleSize)
            {
                throw new InvalidDataException("FFVIIR: invalid section table.");
            }

            var pattern = SignaturePattern.Parse(signature);
            var prefix = pattern.Bytes.TakeWhile(value => value.HasValue).Select(value => value.GetValueOrDefault()).ToArray();
            if (prefix.Length == 0)
            {
                throw new InvalidDataException("FFVIIR: pattern needs a fixed prefix.");
            }

            const int chunkSize = 1024 * 1024;
            ulong match = 0;
            for (var section = 0; section < sectionCount; section++)
            {
                var header = sectionTable + (ulong)section * 40;
                var size = Hypervisor.Read<uint>(header + 8, true);
                var rva = Hypervisor.Read<uint>(header + 12, true);
                var flags = Hypervisor.Read<uint>(header + 36, true);
                if ((flags & 0x20000000) == 0 || size < pattern.Length)
                {
                    continue;
                }

                if ((ulong)rva + size > (ulong)moduleSize)
                {
                    throw new InvalidDataException("FFVIIR: invalid executable section.");
                }

                for (ulong offset = 0; offset < size; offset += chunkSize)
                {
                    var count = (int)Math.Min((ulong)(chunkSize + pattern.Length - 1), size - offset);
                    var bytes = Hypervisor.ReadArray(moduleBase + rva + offset, count, true);
                    var start = 0;
                    while (start <= bytes.Length - pattern.Length)
                    {
                        var found = bytes.AsSpan(start).IndexOf(prefix);
                        if (found < 0)
                        {
                            break;
                        }

                        var candidate = start + found;
                        start = candidate + 1;
                        if (candidate + pattern.Length > bytes.Length)
                        {
                            break;
                        }

                        var matches = true;
                        for (var part = 0; part < pattern.Length; part++)
                        {
                            if (pattern.Bytes[part] is byte expected && bytes[candidate + part] != expected)
                            {
                                matches = false;
                                break;
                            }
                        }

                        if (!matches)
                        {
                            continue;
                        }

                        var address = moduleBase + rva + offset + (ulong)candidate;
                        if (match != 0 && match != address)
                        {
                            throw new InvalidDataException("FFVIIR: manager pattern is ambiguous.");
                        }

                        match = address;
                    }
                }
            }

            return match != 0 ? match : throw new InvalidDataException("FFVIIR: manager pattern was not found.");
        }

    }
}
