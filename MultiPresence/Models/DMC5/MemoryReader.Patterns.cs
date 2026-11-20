namespace MultiPresence.Models.DMC5
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
            if (gameSlot != 0 && flowSlot != 0 && orbSlot != 0 && playerSlot != 0) return;
            if (DateTime.UtcNow < nextScan) throw new InvalidDataException("Waiting to retry Devil May Cry 5 patterns.");
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

            var patterns = new[]
            {
                SignaturePattern.Parse(MissionPattern), SignaturePattern.Parse(DifficultyPattern), SignaturePattern.Parse(CharacterPattern),
                SignaturePattern.Parse(ModePattern), SignaturePattern.Parse(PausePattern), SignaturePattern.Parse(FlowPattern),
                SignaturePattern.Parse(OrbsPattern), SignaturePattern.Parse(PlayerPattern)
            };
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
            if (hits.Any(hit => hit == 0)) throw Invalid("missing pattern");

            var slots = new ulong[hits.Length];
            for (int i = 0; i < hits.Length; i++)
            {
                long target = (long)hits[i] + 7 + Hypervisor.Read<int>(hits[i] + 3, true);
                if (target < (long)start || target > (long)start + size - 8) throw Invalid("pattern target");
                slots[i] = (ulong)target;
            }
            // Mission, difficulty, character, mode and pause code all read the same app.GameManager singleton.
            if (slots[1] != slots[0] || slots[2] != slots[0] || slots[3] != slots[0] || slots[4] != slots[0]) throw Invalid("game manager references");

            int[] offsets =
            [
                Hypervisor.Read<int>(hits[0] + 0x12, true), Hypervisor.Read<int>(hits[1] + 0x0E, true), Hypervisor.Read<int>(hits[2] + 0x13, true),
                Hypervisor.Read<int>(hits[3] + 0x0E, true), Hypervisor.Read<int>(hits[4] + 0x0E, true), Hypervisor.Read<sbyte>(hits[5] + 0x0E, true),
                Hypervisor.Read<int>(hits[5] + 0x14, true), Hypervisor.Read<sbyte>(hits[7] + 0x0F, true)
            ];
            if (offsets.Any(offset => offset is <= 0 or > 0x1000)) throw Invalid("manager layout");

            missionOffset = offsets[0];
            difficultyOffset = offsets[1];
            characterOffset = offsets[2];
            modeOffset = offsets[3];
            pauseOffset = offsets[4];
            nowFlowOffset = offsets[5];
            nextFlowOffset = offsets[6];
            manualPlayerOffset = offsets[7];
            gameSlot = slots[0];
            flowSlot = slots[5];
            orbSlot = slots[6];
            playerSlot = slots[7];
        }
    }
}
