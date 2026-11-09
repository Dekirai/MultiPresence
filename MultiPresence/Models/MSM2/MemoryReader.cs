namespace MultiPresence.Models.MSM2
{
    internal static partial class MemoryReader
    {
        private static int processId, moduleSize;
        private static ulong moduleBase, modeAddress, entityHandleAddress, handleTableSlot, handleCountAddress, saveManagerSlot, districtTypeAddress, lobbyViewAddress;
        private static DateTime nextScan;

        public static void Reset()
        {
            processId = moduleSize = 0;
            moduleBase = modeAddress = entityHandleAddress = handleTableSlot = handleCountAddress = saveManagerSlot = districtTypeAddress = lobbyViewAddress = 0;
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
            var mode = Int(modeAddress);
            if (mode is < 0 or > 5)
            {
                throw Invalid("game mode");
            }

            var lobby = ReadLobbyState();
            var snapshot = new GameSnapshot();
            if (lobby != 0 || mode <= 3)
            {
                snapshot.State = lobby == 0 && mode == 3 ? "Loading" : "Default";
                if (Int(modeAddress) != mode || ReadLobbyState() != lobby)
                {
                    throw new SnapshotChangedException();
                }

                return snapshot;
            }

            var handle = UInt(entityHandleAddress);
            var entity = ResolveHandle(handle);
            var actor = entity == 0 ? 0 : Pointer(entity + 0x10);
            var data = entity == 0 ? 0 : Pointer(entity + 0x108);
            if (entity == 0 || actor == 0 || data == 0)
            {
                snapshot.State = "Loading";
            }
            else
            {
                ReadPlayerStats(snapshot, data);
                ReadCharacter(snapshot, data);
                ReadLocation(snapshot, entity);
                ReadDifficulty(snapshot);
                snapshot.State = "Ingame";
            }

            if (Int(modeAddress) != mode || ReadLobbyState() != lobby || UInt(entityHandleAddress) != handle || ResolveHandle(handle) != entity ||
                (entity != 0 && (Pointer(entity + 0x10) != actor || Pointer(entity + 0x108) != data)))
            {
                throw new SnapshotChangedException();
            }

            return snapshot;
        }

        private static ushort ReadLobbyState()
        {
            var flags = Bytes(lobbyViewAddress + 0x95090, 2);
            if (flags[0] > 1 || flags[1] > 1)
            {
                throw Invalid("lobby state");
            }

            return BitConverter.ToUInt16(flags);
        }

        private static ulong ResolveHandle(uint handle)
        {
            var index = handle & 0xFFFFF;
            var generation = handle >> 20;
            if (generation == 0)
            {
                return 0;
            }

            var count = UInt(handleCountAddress);
            if (count > 0x100000)
            {
                throw Invalid("handle table size");
            }

            if (index >= count)
            {
                return 0;
            }

            var table = Pointer(handleTableSlot);
            if (table < 0x10000)
            {
                throw Invalid("handle table");
            }

            var entry = table + index * 16UL;
            if (UInt(entry + 8) != generation)
            {
                return 0;
            }

            var value = Pointer(entry);
            return UInt(entry + 8) == generation ? value : 0;
        }

        private static void ResolveAddresses()
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

            if (modeAddress != 0)
            {
                return;
            }

            if (DateTime.UtcNow < nextScan)
            {
                throw Invalid("waiting to retry pattern scan");
            }

            nextScan = DateTime.UtcNow.AddSeconds(5);
            var patterns = new[]
            {
                PatternCatalog.GameMode,
                PatternCatalog.Player,
                PatternCatalog.HandleTable,
                PatternCatalog.SaveManager,
                PatternCatalog.DistrictType,
                PatternCatalog.LobbyView
            };
            var matches = FindPatterns(patterns);
            var mode = RipAddress(matches[0], 2, 6);
            var player = RipAddress(matches[1] + 8, 3, 7);
            var table = RipAddress(matches[2] + 23, 3, 7);
            var count = RipAddress(matches[2] + 30, 2, 6);
            var saveManager = RipAddress(matches[3], 3, 7);
            var districtType = RipAddress(matches[4] + 40, 3, 7);
            var lobbyView = RipAddress(matches[5], 3, 7);
            modeAddress = mode;
            entityHandleAddress = player;
            handleTableSlot = table;
            handleCountAddress = count;
            saveManagerSlot = saveManager;
            districtTypeAddress = districtType;
            lobbyViewAddress = lobbyView;
        }

        private static ulong[] FindPatterns(string[] patterns)
        {
            var header = Bytes(moduleBase, 4096);
            var pe = BitConverter.ToInt32(header, 0x3C);
            if (BitConverter.ToUInt16(header) != 0x5A4D || pe < 0x40 || pe > 0xE00 || BitConverter.ToUInt32(header, pe) != 0x4550)
            {
                throw Invalid("PE header");
            }

            var sections = BitConverter.ToUInt16(header, pe + 6);
            var sectionTable = pe + 24 + BitConverter.ToUInt16(header, pe + 20);
            if (sections is 0 or > 96 || sectionTable + sections * 40 > header.Length)
            {
                throw Invalid("PE sections");
            }

            var found = patterns.Select(_ => new HashSet<ulong>()).ToArray();
            var lengths = patterns.Select(pattern => pattern.Split(' ').Length).ToArray();
            var overlap = lengths.Max() - 1;
            const int chunkSize = 1024 * 1024;
            for (var section = 0; section < sections; section++)
            {
                var entry = sectionTable + section * 40;
                if ((BitConverter.ToUInt32(header, entry + 36) & 0x20000000) == 0)
                {
                    continue;
                }

                var size = BitConverter.ToUInt32(header, entry + 8);
                var rva = BitConverter.ToUInt32(header, entry + 12);
                if (rva >= moduleSize || size > moduleSize - rva)
                {
                    throw Invalid("code section range");
                }

                for (ulong offset = 0; offset < size; offset += chunkSize)
                {
                    var start = moduleBase + rva + offset;
                    var bytes = Bytes(start, (int)Math.Min((ulong)(chunkSize + overlap), size - offset));
                    for (var pattern = 0; pattern < patterns.Length; pattern++)
                    {
                        var cursor = 0;
                        while (cursor <= bytes.Length - lengths[pattern])
                        {
                            var match = Hypervisor.FindPattern(bytes.AsSpan(cursor), patterns[pattern]);
                            if (match < 0)
                            {
                                break;
                            }

                            cursor += match;
                            found[pattern].Add(start + (ulong)cursor);
                            cursor++;
                        }
                    }
                }
            }

            if (found.Any(matches => matches.Count != 1))
            {
                throw Invalid("missing or ambiguous pattern");
            }

            return found.Select(matches => matches.Single()).ToArray();
        }

        private static ulong RipAddress(ulong address, int displacement, int length)
        {
            var target = checked((ulong)((long)address + length + Int(address + (ulong)displacement)));
            if (target < moduleBase || target + 8 > moduleBase + (ulong)moduleSize)
            {
                throw Invalid("pattern target");
            }

            return target;
        }

        private static byte[] Bytes(ulong address, int count)
        {
            if (count < 0 || count > 2 * 1024 * 1024 || address < 0x10000 || address > 0x7FFFFFFFFFFFUL - (ulong)count)
            {
                throw Invalid("memory range");
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

        private static int Int(ulong address)
        {
            return BitConverter.ToInt32(Bytes(address, 4));
        }

        private static float Float(ulong address)
        {
            var value = BitConverter.ToSingle(Bytes(address, 4));
            return float.IsFinite(value) ? value : throw Invalid("numeric value");
        }

        private static byte Byte(ulong address)
        {
            return Bytes(address, 1)[0];
        }

        private static InvalidDataException Invalid(string field)
        {
            return new InvalidDataException("MSM2: invalid or changing " + field + ".");
        }

        private sealed class SnapshotChangedException : IOException
        {
            public SnapshotChangedException() : base("MSM2: game state changed during read.")
            {
            }
        }
    }
}
