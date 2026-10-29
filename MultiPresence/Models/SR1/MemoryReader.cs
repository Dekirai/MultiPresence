using System.Diagnostics;
using System.Text;

namespace MultiPresence.Models.SR1
{
    internal static class MemoryReader
    {
        private static readonly Dictionary<string, ulong> addresses = new Dictionary<string, ulong>();
        private static readonly Dictionary<string, (ulong Base, int Size)> modules = new Dictionary<string, (ulong Base, int Size)>();
        private static readonly Dictionary<string, DateTime> nextScan = new Dictionary<string, DateTime>();
        private static int processId;

        public static void Reset()
        {
            addresses.Clear();
            modules.Clear();
            nextScan.Clear();
            processId = 0;
        }

        public static bool IsSoulReaver1()
        {
            ResolveAddresses("SRX.exe");
            return Hypervisor.Read<int>(addresses["Selection"], true) == 0 && Hypervisor.Read<int>(addresses["Collection"], true) == 0;
        }

        public static GameSnapshot ReadSnapshot()
        {
            if (!IsSoulReaver1())
            {
                return new GameSnapshot() { State = "OtherGame" };
            }

            ResolveAddresses("sr1.dll");
            var memory = new FrameRead();
            var snapshot = ReadFrame(memory);
            memory.Validate();
            return snapshot;
        }

        private static GameSnapshot ReadFrame(FrameRead m)
        {
            var selection = m.Int(addresses["Selection"]);
            var collection = m.Int(addresses["Collection"]);
            if (selection != 0 || collection != 0)
            {
                return new GameSnapshot() { State = "OtherGame" };
            }

            var state = m.Int(addresses["GameState"]);
            if (state != 1)
            {
                return new GameSnapshot() { State = state is 2 or 3 ? "Loading" : "Default" };
            }

            var player = m.Pointer(addresses["Player"]);
            if (player == 0)
            {
                return new GameSnapshot() { State = "Loading" };
            }

            if (player < 0x10000 || player > 0x00007FFFFFFFFFFF)
            {
                throw Invalid("player");
            }

            var realm = m.Int(addresses["Realm"]);
            var realmName = GameText.GetRealm(realm);
            if (realmName.Length == 0)
            {
                return new GameSnapshot() { State = "Loading" };
            }

            var areaBytes = m.Bytes(addresses["Area"], 16);
            var end = Array.IndexOf(areaBytes, (byte)0);
            if (end <= 0)
            {
                return new GameSnapshot() { State = "Loading" };
            }

            var area = Encoding.ASCII.GetString(areaBytes, 0, end);
            if (area.Any(value => !char.IsAsciiLetterOrDigit(value) && value != '_'))
            {
                throw Invalid("area name");
            }

            var health = m.Bytes(addresses["Health"], 0x20);
            var scale = BitConverter.ToInt16(health);
            var current = BitConverter.ToInt32(health, 4);
            var mana = BitConverter.ToInt16(health, 0x1C);
            var maxMana = BitConverter.ToInt16(health, 0x1E);
            if (scale is < 1 or > 100 || current is < -100000 or > 10100000 || mana < 0 || maxMana < 0 || mana > maxMana || maxMana > 10000)
            {
                throw Invalid("health or glyph energy");
            }

            var maximum = realm == 2 ? 100000 : 100000 * scale;
            var currentHealth = Math.Clamp(realm == 2 ? current : current - 100000, 0, maximum);
            var upgrades = m.Bytes(addresses["Area"] + 0x160, 20);
            if (upgrades.Any(value => value > 1))
            {
                throw Invalid("collectibles");
            }

            return new GameSnapshot()
            {
                State = "Ingame",
                Area = area,
                Location = GameText.GetLocation(area),
                Realm = realmName,
                Objective = GameText.GetObjective(m.Int(addresses["Area"] + 0x148)),
                Health = currentHealth,
                MaxHealth = maximum,
                Mana = mana,
                MaxMana = maxMana,
                HealthUpgrades = upgrades.Take(15).Count(value => value == 1),
                EnergyUpgrades = upgrades.Skip(15).Count(value => value == 1)
            };
        }

        private static InvalidDataException Invalid(string field)
        {
            return new InvalidDataException($"Soul Reaver 1: invalid or changing {field}.");
        }

        private static void ResolveAddresses(string moduleName)
        {
            var process = Hypervisor.Process ?? throw Invalid("process");
            if (processId != process.Id)
            {
                Reset();
                processId = process.Id;
            }

            var module = process.Modules.Cast<ProcessModule>().FirstOrDefault(value => string.Equals(value.ModuleName, moduleName, StringComparison.OrdinalIgnoreCase)) ?? throw Invalid(moduleName + " module");
            var moduleBase = (ulong)module.BaseAddress.ToInt64();
            var moduleSize = module.ModuleMemorySize;
            var definitions = PatternCatalog.All.Where(value => value.Value.Module == moduleName).ToDictionary(value => value.Key, value => value.Value);
            if (!modules.TryGetValue(moduleName, out var previous) || previous != (moduleBase, moduleSize))
            {
                foreach (var name in definitions.Keys)
                {
                    addresses.Remove(name);
                }

                nextScan.Remove(moduleName);
                modules[moduleName] = (moduleBase, moduleSize);
            }

            if (definitions.Keys.All(addresses.ContainsKey))
            {
                return;
            }

            if (nextScan.TryGetValue(moduleName, out var retry) && DateTime.UtcNow < retry)
            {
                throw Invalid("pattern scan (retry pending)");
            }

            nextScan[moduleName] = DateTime.UtcNow.AddSeconds(30);
            var matches = definitions.Keys.ToDictionary(name => name, _ => new HashSet<ulong>());
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

            var longest = definitions.Values.Max(value => value.Pattern.Split(' ').Length);
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
                    foreach (var (name, definition) in definitions)
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
            foreach (var (name, definition) in definitions)
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

            foreach (var pair in resolved)
            {
                addresses[pair.Key] = pair.Value;
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

            public int Int(ulong address)
            {
                return BitConverter.ToInt32(Bytes(address, 4));
            }

            public ulong Pointer(ulong address)
            {
                return BitConverter.ToUInt64(Bytes(address, 8));
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
