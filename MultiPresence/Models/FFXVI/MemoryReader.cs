using System.Globalization;

namespace MultiPresence.Models.FFXVI
{
    internal static class MemoryReader
    {
        private const string scenePattern = "48 8B 0D ?? ?? ?? ?? 48 8B F2 48 85 C9 0F 84 ?? ?? ?? ?? 48 8B 51 48 E8 ?? ?? ?? ?? 48 8B D8 48 85 D2";
        private const string sceneLayout = "48 8B 49 48 48 85 C9 74 ?? 83 79 08 02 75 ?? 48 8B 89 10 05 00 00 8B 41 10 F7 D8 48 1B C0 48 23 C1 C3 33 C0 C3";
        private const string mapLayout = "48 8B 49 48 48 85 C9 74 ?? 83 79 08 02 75 ?? 48 8B 89 10 05 00 00 8B 41 10 F7 D8 48 1B C0 48 23 C1 74 ?? 48 8B 40 18";
        private const string areaPattern = "48 8B 0D ?? ?? ?? ?? 48 81 C1 38 8D 00 00 E8 ?? ?? ?? ?? E9 ?? ?? ?? ?? 83 F8 14";
        private const string areaLayout = "8B 7B 08 3B 7E 78 74 ?? 48 8B D3 E8 ?? ?? ?? ?? 84 C0 75 ?? 8B D7 E8 ?? ?? ?? ?? 48 8B D3 48 8D 4E F8 E8 ?? ?? ?? ?? 44 88 73 18 EB ?? 44 89 76 78";
        private const string areaNameLayout = "48 89 5C 24 08 48 89 6C 24 10 48 89 74 24 18 57 48 83 EC 20 8B 42 08 48 8B F2 89 81 80 00 00 00";
        private const string questPattern = "48 8B 15 ?? ?? ?? ?? B9 E4 CA 00 00 48 85 D2 BB F0 06 00 00 48 8D 82 0C CB 00 00 48 0F 45 C8 8B 01 89 44 24 30 48 8D 82 18 07 00 00 48 0F 45 D8 44 8B 8B 10 2D 00 00";
        private const string questLayout = "48 8D 93 10 23 00 00 45 33 C0 48 8D 4C 24 30 E8 ?? ?? ?? ?? 48 85 C0 74 ?? 8B 40 04 48 8D 0C 40 48 8D 04 CB 48 85 C0 74 ?? 8B 40 04";
        private static ulong sceneSlot, profileSlot, playerSlot, currencySlot, scenarioSlot, moduleBase;
        private static int processId, moduleSize;
        private static DateTime nextScan;

        public static GameSnapshot ReadSnapshot()
        {
            ResolveAddresses();
            var scene = Pointer(sceneSlot);
            if (scene == 0)
            {
                return new GameSnapshot() { State = "Loading" };
            }

            var task = Pointer(scene + 0x48);
            if (task == 0)
            {
                return new GameSnapshot() { State = "Loading" };
            }

            var sceneType = ReadInt(task + 8);
            if (sceneType != 2)
            {
                return new GameSnapshot() { State = sceneType == 1 ? "Default" : "Loading" };
            }

            var descriptor = Pointer(task + 0x510);
            if (descriptor == 0 || ReadInt(descriptor + 0x10) == 0)
            {
                return new GameSnapshot() { State = "Loading" };
            }

            var profile = Pointer(profileSlot);
            var player = Pointer(playerSlot);
            var currency = Pointer(currencySlot);
            var scenario = Pointer(scenarioSlot);
            if (profile == 0 || player == 0 || currency == 0 || scenario == 0)
            {
                return new GameSnapshot() { State = "Loading" };
            }

            var mapId = ReadInt(descriptor + 0x18);
            var subId = ReadInt(descriptor + 0x1C);
            var placeId = ReadInt(player + 0x8DB8);
            var map = Locations.GetMapName(mapId, subId);
            var area = Locations.GetName(placeId);
            var region = Locations.GetMapRegion(mapId, subId);
            if (string.IsNullOrEmpty(area))
            {
                area = map;
            }

            if (string.IsNullOrEmpty(region) && map != area)
            {
                region = map;
            }

            var quest = ReadQuest(profile);
            var objective = Quests.GetVisibleSequenceId(quest.Id, quest.Sequence);
            var snapshot = new GameSnapshot()
            {
                State = "Ingame",
                Region = region,
                Area = area,
                Location = string.Join(" / ", new[] { region, map, area }.Where(value => !string.IsNullOrEmpty(value)).Distinct()),
                MapId = mapId.ToString(CultureInfo.InvariantCulture) + ":" + subId.ToString(CultureInfo.InvariantCulture),
                Quest = Quests.GetQuest(quest.Id, objective > 0 ? objective : quest.Sequence),
                QuestId = quest.Id > 0 ? quest.Id.ToString(CultureInfo.InvariantCulture) : "",
                QuestObjective = Quests.GetObjective(quest.Id, objective),
                QuestObjectiveId = objective > 0 ? objective.ToString(CultureInfo.InvariantCulture) : ""
            };
            if (!PlayerStats.TryRead(profile, player, currency, scenario, snapshot, out snapshot))
            {
                return new GameSnapshot() { State = "Loading" };
            }

            if (scene != Pointer(sceneSlot) || task != Pointer(scene + 0x48) || sceneType != ReadInt(task + 8) || descriptor != Pointer(task + 0x510) || ReadInt(descriptor + 0x10) == 0 || mapId != ReadInt(descriptor + 0x18) || subId != ReadInt(descriptor + 0x1C) || profile != Pointer(profileSlot) || player != Pointer(playerSlot) || currency != Pointer(currencySlot) || scenario != Pointer(scenarioSlot) || quest != ReadQuest(profile) || placeId != ReadInt(player + 0x8DB8))
            {
                return new GameSnapshot() { State = "Loading" };
            }

            return snapshot;
        }

        private static (int Id, int Sequence) ReadQuest(ulong profile)
        {
            var questId = ReadInt(profile + 0xCB0C);
            var records = profile + 0x718;
            var count = ReadInt(records + 0x2D10);
            if (count < 0 || count > 320)
            {
                throw new InvalidDataException("FFXVI: invalid quest count.");
            }

            if (questId <= 0 || count == 0)
            {
                return (0, 0);
            }

            var entries = Hypervisor.ReadArray(records + 0x2310, count * 8, true);
            for (var index = 0; index < count; index++)
            {
                if (BitConverter.ToInt32(entries, index * 8) != questId)
                {
                    continue;
                }

                var record = BitConverter.ToInt32(entries, index * 8 + 4);
                if (record < 0 || record >= 374)
                {
                    throw new InvalidDataException("FFXVI: invalid quest index.");
                }

                return (questId, ReadInt(records + (ulong)record * 24 + 4));
            }

            return (questId, 0);
        }

        internal static ulong Pointer(ulong address)
        {
            return Hypervisor.Read<ulong>(address, true);
        }

        internal static int ReadInt(ulong address)
        {
            return Hypervisor.Read<int>(address, true);
        }

        public static void Reset()
        {
            processId = moduleSize = 0;
            moduleBase = sceneSlot = profileSlot = playerSlot = currencySlot = scenarioSlot = 0;
            nextScan = DateTime.MinValue;
        }

        private static void ResolveAddresses()
        {
            var process = Hypervisor.Process ?? throw new InvalidDataException("FFXVI: game is not running.");
            var module = process.MainModule ?? throw new InvalidDataException("FFXVI: module not loaded.");
            var address = (ulong)module.BaseAddress.ToInt64();
            if (processId != process.Id || moduleBase != address || moduleSize != module.ModuleMemorySize)
            {
                Reset();
                processId = process.Id;
                moduleBase = address;
                moduleSize = module.ModuleMemorySize;
            }

            if (sceneSlot != 0)
            {
                return;
            }

            if (DateTime.UtcNow < nextScan)
            {
                throw new InvalidDataException("FFXVI: waiting to retry patterns.");
            }

            nextScan = DateTime.UtcNow.AddSeconds(30);
            var sceneMatch = FindUniquePattern(moduleBase, moduleSize, scenePattern);
            var foundScene = Relative(sceneMatch, 3, 7);
            Verify(Relative(sceneMatch + 23, 1, 5), sceneLayout);
            var foundProfile = Relative(FindUniquePattern(moduleBase, moduleSize, PatternCatalog.Profile), 3, 7);
            var foundPlayer = Relative(FindUniquePattern(moduleBase, moduleSize, PatternCatalog.Player), 3, 7);
            var foundCurrency = Relative(FindUniquePattern(moduleBase, moduleSize, PatternCatalog.Currency), 3, 7);
            var foundScenario = Relative(FindUniquePattern(moduleBase, moduleSize, PatternCatalog.Scenario), 3, 7);
            if (foundProfile != Relative(FindUniquePattern(moduleBase, moduleSize, questPattern), 3, 7))
            {
                throw new InvalidDataException("FFXVI: quest and player profiles do not match.");
            }

            if (foundPlayer != Relative(FindUniquePattern(moduleBase, moduleSize, areaPattern), 3, 7))
            {
                throw new InvalidDataException("FFXVI: area and player managers do not match.");
            }

            var areaMatch = FindUniquePattern(moduleBase, moduleSize, areaLayout);
            Verify(Relative(areaMatch + 34, 1, 5), areaNameLayout);

            FindUniquePattern(moduleBase, moduleSize, questLayout);
            FindUniquePattern(moduleBase, moduleSize, mapLayout);
            foreach (var pattern in PatternCatalog.Layouts)
            {
                FindUniquePattern(moduleBase, moduleSize, pattern);
            }

            profileSlot = foundProfile;
            playerSlot = foundPlayer;
            currencySlot = foundCurrency;
            scenarioSlot = foundScenario;
            sceneSlot = foundScene;
        }

        private static ulong Relative(ulong instruction, int displacement, int length)
        {
            var target = checked((ulong)((long)instruction + length + ReadInt(instruction + (ulong)displacement)));
            if (target < moduleBase || target + 8 > moduleBase + (ulong)moduleSize)
            {
                throw new InvalidDataException("FFXVI: pattern target is outside the module.");
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
                    throw new InvalidDataException("FFXVI: unsupported native layout.");
                }
            }
        }

        private static ulong FindUniquePattern(ulong moduleBase, int moduleSize, string signature)
        {
            if (moduleSize < 64)
            {
                throw new InvalidDataException("FFXVI: invalid module size.");
            }

            var peOffset = Hypervisor.Read<uint>(moduleBase + 0x3C, true);
            if (peOffset > (uint)moduleSize - 24)
            {
                throw new InvalidDataException("FFXVI: invalid PE offset.");
            }

            var pe = moduleBase + peOffset;
            if (peOffset > (uint)moduleSize - 24 || Hypervisor.Read<uint>(pe, true) != 0x4550 || Hypervisor.Read<ushort>(pe + 4, true) != 0x8664)
            {
                throw new InvalidDataException("FFXVI: invalid PE header.");
            }

            var sectionCount = Hypervisor.Read<ushort>(pe + 6, true);
            var optionalSize = Hypervisor.Read<ushort>(pe + 20, true);
            var sectionTable = pe + 24 + optionalSize;
            if (sectionTable + (ulong)sectionCount * 40 > moduleBase + (ulong)moduleSize)
            {
                throw new InvalidDataException("FFXVI: invalid section table.");
            }

            var pattern = SignaturePattern.Parse(signature);
            var prefix = pattern.Bytes.TakeWhile(value => value.HasValue).Select(value => value.GetValueOrDefault()).ToArray();
            if (prefix.Length == 0)
            {
                throw new InvalidDataException("FFXVI: pattern needs a fixed prefix.");
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
                    throw new InvalidDataException("FFXVI: invalid executable section.");
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
                            throw new InvalidDataException("FFXVI: manager pattern is ambiguous.");
                        }

                        match = address;
                    }
                }
            }

            return match != 0 ? match : throw new InvalidDataException("FFXVI: manager pattern was not found.");
        }

    }
}
