using System.Diagnostics;

namespace MultiPresence.Models.BXP
{
    internal static partial class MemoryReader
    {
        private static readonly Dictionary<string, SignaturePattern> patterns = new()
        {
            ["SceneMgr"] = SignaturePattern.Parse("48 8B 05 ?? ?? ?? ?? 48 8B 88 B8 00 00 00 48 8B 01 48 85 C0 74 ?? 8B 40 30"),
            ["InfoDB"] = SignaturePattern.Parse("48 8B 05 ?? ?? ?? ?? 48 8B 88 B8 00 00 00 48 8B 09 48 85 C9 74 ?? 48 8B 89 D8 00 00 00 48 85 C9 74 ?? 48 63 46 10 3B 41 18 73 ?? 48 8B 54 C1 20"),
            ["SaveMgr"] = SignaturePattern.Parse("48 8B 05 ?? ?? ?? ?? 33 ED 89 6C 24 30 48 8B D6 48 8B 88 B8 00 00 00"),
            ["UpgradeMgr"] = SignaturePattern.Parse("48 8B 0D ?? ?? ?? ?? 48 8B 89 B8 00 00 00 E8 ?? ?? ?? ?? 48 8B 0D ?? ?? ?? ?? BA 1F 00 00 00 E8 ?? ?? ?? ?? 48 8D 8B 70 02 00 00")
        };

        private static void ResolveAddresses()
        {
            var process = Hypervisor.Process ?? throw new InvalidOperationException("BALL x PIT is not attached.");
            if (process.Id != processId)
            {
                Reset();
                processId = process.Id;
            }
            if (addresses.Count != 0) return;
            if (DateTime.UtcNow < nextScan) throw new InvalidDataException("Waiting to retry BALL x PIT patterns.");
            nextScan = DateTime.UtcNow.AddSeconds(15);
            var module = process.Modules.Cast<ProcessModule>().First(m => string.Equals(m.ModuleName, "GameAssembly.dll", StringComparison.OrdinalIgnoreCase));
            ulong start = (ulong)module.BaseAddress.ToInt64();
            var header = Hypervisor.ReadArray(start, 4096, true);
            int pe = BitConverter.ToInt32(header, 0x3C);
            if (pe < 64 || pe > header.Length - 24 || BitConverter.ToUInt16(header) != 0x5A4D ||
                BitConverter.ToUInt32(header, pe) != 0x4550) throw new InvalidDataException("Invalid GameAssembly header.");
            int count = BitConverter.ToUInt16(header, pe + 6);
            int table = pe + 24 + BitConverter.ToUInt16(header, pe + 20);
            if (count is < 1 or > 96 || table + count * 40 > header.Length) throw new InvalidDataException("Invalid GameAssembly section table.");

            var found = new Dictionary<string, ulong>();
            int overlap = patterns.Values.Max(p => p.Length) - 1;
            const int blockSize = 1024 * 1024;
            for (int i = 0; i < count; i++)
            {
                int section = table + i * 40;
                if ((BitConverter.ToUInt32(header, section + 36) & 0x20000000) == 0) continue;
                uint length = BitConverter.ToUInt32(header, section + 8);
                uint rva = BitConverter.ToUInt32(header, section + 12);
                if ((ulong)rva + length > (ulong)module.ModuleMemorySize) throw new InvalidDataException("Invalid GameAssembly section size.");
                for (long offset = 0; offset < length; offset += blockSize)
                {
                    ulong block = start + rva + (ulong)offset;
                    byte[] bytes = Hypervisor.ReadArray(block, (int)Math.Min(length - offset, blockSize + overlap), true);
                    foreach (var (name, pattern) in patterns)
                    {
                        ReadOnlySpan<byte> prefix = [0x48, 0x8B, pattern.Bytes[2]!.Value];
                        int pos = 0;
                        while (pos < blockSize && pos <= bytes.Length - pattern.Length)
                        {
                            int next = bytes.AsSpan(pos).IndexOf(prefix);
                            if (next < 0) break;
                            pos += next;
                            if (pos >= blockSize || pos > bytes.Length - pattern.Length) break;
                            if (pattern.IndexOf(bytes.AsSpan(pos, pattern.Length)) == 0)
                            {
                                long target = (long)block + pos + 7 + BitConverter.ToInt32(bytes, pos + 3);
                                if (target < (long)start || target + 8 > (long)start + module.ModuleMemorySize || !found.TryAdd(name, (ulong)target))
                                    throw new InvalidDataException($"Invalid or ambiguous BALL x PIT pattern for {name}.");
                            }
                            pos++;
                        }
                    }
                }
            }
            if (found.Count != patterns.Count) throw new InvalidDataException("Missing BALL x PIT manager pattern.");
            foreach (var item in found) addresses.Add(item.Key, item.Value);
        }
    }
}
