namespace MultiPresence.Models.SH2
{
    internal static partial class MemoryReader
    {
        private static void ResolveAddresses()
        {
            var process = Hypervisor.Process ?? throw Invalid("process");
            if (process.Id != processId)
            {
                Reset();
                processId = process.Id;
            }
            if (worldSlot != 0 && namesPool != 0) return;
            if (DateTime.UtcNow < nextScan) throw Invalid("pattern scan (retry pending)");
            nextScan = DateTime.UtcNow.AddSeconds(30);

            var module = process.MainModule ?? throw Invalid("module");
            ulong start = (ulong)module.BaseAddress.ToInt64();
            uint size = checked((uint)module.ModuleMemorySize);
            var header = Hypervisor.ReadArray(start, 4096, true);
            int pe = BitConverter.ToInt32(header, 0x3C);
            if (BitConverter.ToUInt16(header) != 0x5A4D || pe < 64 || pe > header.Length - 24 ||
                BitConverter.ToUInt32(header, pe) != 0x4550 || BitConverter.ToUInt16(header, pe + 4) != 0x8664) throw Invalid("PE header");
            int count = BitConverter.ToUInt16(header, pe + 6);
            int table = pe + 24 + BitConverter.ToUInt16(header, pe + 20);
            if (count is < 1 or > 96 || table + count * 40 > header.Length) throw Invalid("PE section table");

            var patterns = new[] { SignaturePattern.Parse(WorldPattern), SignaturePattern.Parse(NamesPattern) };
            var prefixes = patterns.Select(p => p.Bytes.TakeWhile(b => b.HasValue).Select(b => b.GetValueOrDefault()).ToArray()).ToArray();
            var hits = new ulong[patterns.Length];
            int overlap = patterns.Max(p => p.Length) - 1;
            const int chunk = 1024 * 1024;
            for (int i = 0; i < count; i++)
            {
                int section = table + i * 40;
                if ((BitConverter.ToUInt32(header, section + 36) & 0x20000000) == 0) continue;
                uint length = BitConverter.ToUInt32(header, section + 8);
                uint rva = BitConverter.ToUInt32(header, section + 12);
                if ((ulong)rva + length > size) throw Invalid("executable section");
                for (long offset = 0; offset < length; offset += chunk)
                {
                    ulong address = start + rva + (ulong)offset;
                    byte[] bytes = Hypervisor.ReadArray(address, (int)Math.Min(length - offset, chunk + overlap), true);
                    for (int j = 0; j < patterns.Length; j++)
                    {
                        var pattern = patterns[j];
                        for (int pos = 0; pos <= bytes.Length - pattern.Length; pos++)
                        {
                            int found = bytes.AsSpan(pos).IndexOf(prefixes[j]);
                            if (found < 0) break;
                            pos += found;
                            if (pos > bytes.Length - pattern.Length) break;
                            if (pattern.IndexOf(bytes.AsSpan(pos, pattern.Length)) < 0) continue;
                            ulong match = address + (ulong)pos;
                            if (hits[j] != 0 && hits[j] != match) throw Invalid("ambiguous pattern");
                            hits[j] = match;
                        }
                    }
                }
            }

            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i] == 0) throw Invalid("missing pattern");
                long target = (long)hits[i] + 7 + Hypervisor.Read<int>(hits[i] + 3, true);
                if (target < (long)start || target > (long)start + size - 8) throw Invalid("pattern target");
                hits[i] = (ulong)target;
            }
            worldSlot = hits[0];
            namesPool = hits[1];
        }
    }
}
