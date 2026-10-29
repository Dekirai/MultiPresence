using System.Text;

namespace MultiPresence.Models.CCFFVII
{
    internal static class MemoryReader
    {
        private const string namesPattern = "48 8D 05 ?? ?? ?? ?? EB ?? 48 8D 0D ?? ?? ?? ?? E8 ?? ?? ?? ?? C6 05 ?? ?? ?? ?? 01";
        private const string scenePattern = "48 8B 05 ?? ?? ?? ?? 48 85 C0 75 ?? C3 48 8B 80 30 0A 00 00 C3";
        private const string mapPattern = "8B 1D ?? ?? ?? ?? E8 ?? ?? ?? ?? 48 8B C8 8B D3 E8 ?? ?? ?? ?? 48 8B F0 45 33 FF 33 D2 4C 89 7D D7 33 C0 48 89 55 DF";
        private const string chapterPattern = "48 8D 05 ?? ?? ?? ?? C3 CC CC CC CC CC CC CC CC 0F BE 05 ?? ?? ?? ?? C3 CC CC CC CC CC CC CC CC 8B 05 ?? ?? ?? ?? C3";
        private const string locationPattern = "8B 15 ?? ?? ?? ?? E8 ?? ?? ?? ?? 48 8B 5C 24 28 48 85 DB 74 ?? 83 6B 08 01 75 ?? 48 8B 03 48 8B CB FF 10";
        private const string missionPattern = "8B 05 ?? ?? ?? ?? C3 CC CC CC CC CC CC CC CC CC 0F BF 05 ?? ?? ?? ?? C3 CC CC CC CC CC CC CC CC 0F BF 05 ?? ?? ?? ?? C3";
        private static readonly Dictionary<uint, string> names = new Dictionary<uint, string>();
        private static ulong namesAddress, sceneSlot, mapAddress, locationAddress, missionAddress, chapterAddress, uiSlot, saveStatus, battleStatus, moduleBase;
        private static int processId, moduleSize;
        private static DateTime nextScan;

        public static GameSnapshot ReadSnapshot()
        {
            ResolveAddresses();
            var scene = Pointer(sceneSlot);
            if (scene == 0)
            {
                return new GameSnapshot();
            }

            var task = Pointer(scene + 0xA30);
            if (task == 0)
            {
                return new GameSnapshot() { State = "Loading" };
            }

            var sceneType = ReadInt(task + 8);
            if (sceneType is not (2 or 3))
            {
                return new GameSnapshot() { State = sceneType == 1 ? "Default" : "Loading" };
            }

            var ui = Pointer(uiSlot);
            if (!IsA(ui, "FairUIManager"))
            {
                return new GameSnapshot() { State = "Loading" };
            }

            var map = ReadInt(mapAddress);
            var location = ReadInt(locationAddress);
            var chapter = Hypervisor.Read<sbyte>(chapterAddress, true);
            var mission = Hypervisor.Read<short>(missionAddress, true);
            if (chapter is < 0 or > 11 || mission is < -1 or > 299)
            {
                return new GameSnapshot() { State = "Loading" };
            }

            var displayedChapter = location >= 95 && location <= 104 && chapter > 0 ? chapter - 1 : location == 405 ? 10 : chapter;
            var snapshot = new GameSnapshot()
            {
                State = "Ingame",
                Mode = mission >= 0 ? "Mission" : "Story",
                Character = "Zack Fair",
                Chapter = displayedChapter,
                ChapterName = Chapters.GetName(displayedChapter),
                Location = Locations.GetLocation(location),
                Region = Locations.GetRegion(location),
                Area = Locations.GetArea(location),
                MapId = Locations.GetMapKey(map),
                MissionId = mission >= 0 ? Missions.GetNumber(mission) : "",
                MissionName = mission >= 0 ? Missions.GetName(mission) : ""
            };
            if (!PlayerStats.TryRead(saveStatus, battleStatus, ui, snapshot, out snapshot))
            {
                return new GameSnapshot() { State = "Loading" };
            }

            if (scene != Pointer(sceneSlot) || task != Pointer(scene + 0xA30) || sceneType != ReadInt(task + 8) || ui != Pointer(uiSlot) || map != ReadInt(mapAddress) || location != ReadInt(locationAddress) || chapter != Hypervisor.Read<sbyte>(chapterAddress, true) || mission != Hypervisor.Read<short>(missionAddress, true))
            {
                return new GameSnapshot() { State = "Loading" };
            }

            return snapshot;
        }

        internal static bool IsA(ulong instance, string expected)
        {
            if (instance == 0)
            {
                return false;
            }

            var type = Pointer(instance + 0x10);
            for (var depth = 0; type != 0 && depth < 32; depth++, type = Pointer(type + 0x40))
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
                throw new InvalidDataException("CCFFVII: invalid name pool.");
            }

            var start = Pointer(namesAddress + 0x10 + block * 8);
            var offset = (index & 0xFFFF) * 2;
            var limit = block == currentBlock ? cursor : 0x20000;
            if (start == 0 || offset + 2 > limit)
            {
                throw new InvalidDataException("CCFFVII: invalid name entry.");
            }

            var header = Hypervisor.Read<ushort>(start + offset, true);
            var length = header >> 6;
            var wide = (header & 1) != 0;
            var byteLength = length * (wide ? 2 : 1);
            if (length is < 1 or > 1023 || offset + 2 + byteLength > limit)
            {
                throw new InvalidDataException("CCFFVII: invalid name length.");
            }

            var value = (wide ? Encoding.Unicode : Encoding.UTF8).GetString(Hypervisor.ReadArray(start + offset + 2, byteLength, true));
            if (value.Any(char.IsControl))
            {
                throw new InvalidDataException("CCFFVII: invalid name characters.");
            }

            names[index] = value;
            return value;
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
            moduleBase = namesAddress = sceneSlot = mapAddress = locationAddress = missionAddress = chapterAddress = uiSlot = saveStatus = battleStatus = 0;
            nextScan = DateTime.MinValue;
            names.Clear();
        }

        private static void ResolveAddresses()
        {
            var process = Hypervisor.Process ?? throw new InvalidDataException("CCFFVII: game is not running.");
            var module = process.MainModule ?? throw new InvalidDataException("CCFFVII: module not loaded.");
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
                throw new InvalidDataException("CCFFVII: waiting to retry patterns.");
            }

            nextScan = DateTime.UtcNow.AddSeconds(30);
            var foundNames = Relative(FindUniquePattern(moduleBase, moduleSize, namesPattern), 3, 7);
            var foundScene = Relative(FindUniquePattern(moduleBase, moduleSize, scenePattern), 3, 7);
            var foundLocation = Relative(FindUniquePattern(moduleBase, moduleSize, locationPattern), 2, 6);
            var foundMission = Relative(FindUniquePattern(moduleBase, moduleSize, missionPattern) + 0x10, 3, 7);
            var foundMap = Relative(FindUniquePattern(moduleBase, moduleSize, mapPattern), 2, 6);
            var foundSave = Relative(FindUniquePattern(moduleBase, moduleSize, PatternCatalog.SaveStatus), 10, 14);
            var foundBattle = Relative(FindUniquePattern(moduleBase, moduleSize, PatternCatalog.BattleStatus), 11, 15);
            var foundChapter = Relative(FindUniquePattern(moduleBase, moduleSize, chapterPattern) + 0x10, 3, 7);
            var uiAccessor = Relative(FindUniquePattern(moduleBase, moduleSize, PatternCatalog.UiManager) + 9, 1, 5);
            Verify(uiAccessor + 0x8F, PatternCatalog.UiAccessor);
            var uiCache = Relative(uiAccessor + 0x8F, 1, 5);
            Verify(uiCache + 0xA, PatternCatalog.UiCache);
            var foundUi = Relative(uiCache + 0x14, 3, 7) + 8;
            foreach (var pattern in PatternCatalog.Layouts)
            {
                FindUniquePattern(moduleBase, moduleSize, pattern);
            }

            sceneSlot = foundScene;
            mapAddress = foundMap;
            locationAddress = foundLocation;
            missionAddress = foundMission;
            chapterAddress = foundChapter;
            saveStatus = foundSave;
            battleStatus = foundBattle;
            uiSlot = foundUi;
            namesAddress = foundNames;
        }

        private static ulong Relative(ulong instruction, int displacement, int length)
        {
            var target = checked((ulong)((long)instruction + length + ReadInt(instruction + (ulong)displacement)));
            if (target < moduleBase || target + 8 > moduleBase + (ulong)moduleSize)
            {
                throw new InvalidDataException("CCFFVII: pattern target is outside the module.");
            }

            return target;
        }

        private static void Verify(ulong address, string signature)
        {
            var pattern = SignaturePattern.Parse(signature);
            var bytes = Hypervisor.ReadArray(address, pattern.Length, true);
            for (var index = 0; index < pattern.Length; index++)
            {
                if (pattern.Bytes[index] is byte expected && bytes[index] != expected)
                {
                    throw new InvalidDataException("CCFFVII: unsupported native layout.");
                }
            }
        }

        private static ulong FindUniquePattern(ulong moduleBase, int moduleSize, string signature)
        {
            if (moduleSize < 64)
            {
                throw new InvalidDataException("CCFFVII: invalid module size.");
            }

            var peOffset = Hypervisor.Read<uint>(moduleBase + 0x3C, true);
            if (peOffset > (uint)moduleSize - 24)
            {
                throw new InvalidDataException("CCFFVII: invalid PE offset.");
            }

            var pe = moduleBase + peOffset;
            if (peOffset > (uint)moduleSize - 24 || Hypervisor.Read<uint>(pe, true) != 0x4550 || Hypervisor.Read<ushort>(pe + 4, true) != 0x8664)
            {
                throw new InvalidDataException("CCFFVII: invalid PE header.");
            }

            var sectionCount = Hypervisor.Read<ushort>(pe + 6, true);
            var optionalSize = Hypervisor.Read<ushort>(pe + 20, true);
            var sectionTable = pe + 24 + optionalSize;
            if (sectionTable + (ulong)sectionCount * 40 > moduleBase + (ulong)moduleSize)
            {
                throw new InvalidDataException("CCFFVII: invalid section table.");
            }

            var pattern = SignaturePattern.Parse(signature);
            var prefix = pattern.Bytes.TakeWhile(value => value.HasValue).Select(value => value.GetValueOrDefault()).ToArray();
            if (prefix.Length == 0)
            {
                throw new InvalidDataException("CCFFVII: pattern needs a fixed prefix.");
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
                    throw new InvalidDataException("CCFFVII: invalid executable section.");
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
                            throw new InvalidDataException("CCFFVII: manager pattern is ambiguous.");
                        }

                        match = address;
                    }
                }
            }

            return match != 0 ? match : throw new InvalidDataException("CCFFVII: manager pattern was not found.");
        }

    }
}
