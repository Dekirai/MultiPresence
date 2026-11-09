namespace MultiPresence.Models.CROC2
{
    internal static partial class MemoryReader
    {
        private static int processId, moduleSize;
        private static ulong moduleBase, modeAddress, locationAddress, statsAddress, levelCrystalsAddress;
        private static DateTime nextScan;

        public static void Reset()
        {
            processId = moduleSize = 0;
            moduleBase = modeAddress = locationAddress = statsAddress = levelCrystalsAddress = 0;
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
            if (mode is < 0 or > 20)
            {
                throw Invalid("game state");
            }

            var snapshot = new GameSnapshot();
            if (mode is 10 or 13 or 16 or 19)
            {
                snapshot.State = "Loading";
            }
            else if (mode == 11)
            {
                snapshot.State = "GameOver";
            }
            else if (mode == 4)
            {
                var location = Bytes(locationAddress, 16);
                var world = BitConverter.ToInt32(location, 0);
                var level = BitConverter.ToInt32(location, 4);
                var map = BitConverter.ToInt32(location, 8);
                var type = BitConverter.ToInt32(location, 12);
                if (world is < 0 or > 5 || level is < 0 or > 99 || map is < 0 or > 999 || type is < 0 or > 3)
                {
                    throw Invalid("location");
                }

                if (world != 0 || level != 0 || type != 3 || map != 0)
                {
                    snapshot.MapId = Locations.GetMapId(world, level, type, map);
                    snapshot.World = Locations.GetWorld(world, level);
                    snapshot.Level = Locations.GetName(world, level, type, map);
                    snapshot.IsOverworld = Locations.IsOverworld(world, level, type, map);
                    snapshot.AreaType = snapshot.IsOverworld ? "Overworld" : Locations.IsShop(world, level, type, map) ? "Shop" : type == 1 ? "Boss" : type == 2 ? "Secret Level" : type == 3 ? "Cutscene" : "Level";
                    snapshot.Location = snapshot.IsOverworld ? snapshot.Level + " / Overworld" : snapshot.World.Length != 0 && snapshot.World != snapshot.Level ? snapshot.World + " / " + snapshot.Level : snapshot.Level;
                    ReadPlayerStats(snapshot);
                    snapshot.CurrentCrystals = snapshot.IsOverworld || snapshot.AreaType == "Shop" ? snapshot.Crystals : snapshot.LevelCrystals;
                    snapshot.State = "Ingame";
                }

                if (!Bytes(locationAddress, 16).SequenceEqual(location))
                {
                    throw new SnapshotChangedException();
                }
            }

            if (Int(modeAddress) != mode)
            {
                throw new SnapshotChangedException();
            }

            return snapshot;
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
            var matches = FindPatterns(new[] { PatternCatalog.GameState, PatternCatalog.Location, PatternCatalog.PlayerStats, PatternCatalog.LevelCrystals });
            var mode = RipAddress(matches[0], 2, 6);
            var location = RipAddress(matches[1] + 12, 2, 7);
            var stats = RipAddress(matches[2], 3, 7);
            var crystals = RipAddress(matches[3], 2, 6);
            modeAddress = mode;
            locationAddress = location;
            statsAddress = stats;
            levelCrystalsAddress = crystals;
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

        private static int Int(ulong address)
        {
            return BitConverter.ToInt32(Bytes(address, 4));
        }

        private static InvalidDataException Invalid(string field)
        {
            return new InvalidDataException("CROC2: invalid or changing " + field + ".");
        }

        private sealed class SnapshotChangedException : IOException
        {
            public SnapshotChangedException() : base("CROC2: game state changed during read.")
            {
            }
        }
    }
}
