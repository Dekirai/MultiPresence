using System.Globalization;

namespace MultiPresence.Models.RE4
{
    internal static partial class MemoryReader
    {
        private static void ResolveAddresses()
        {
            var process = Hypervisor.Process ?? throw Invalid("process not attached");
            var module = process.MainModule ?? throw Invalid("module not loaded");
            var currentBase = (ulong)module.BaseAddress.ToInt64();
            if (processId != process.Id || moduleBase != currentBase || moduleSize != module.ModuleMemorySize)
            {
                Reset();
                processId = process.Id;
                moduleBase = currentBase;
                moduleSize = module.ModuleMemorySize;
            }

            if (addresses.Count != 0)
            {
                return;
            }

            if (DateTime.UtcNow < nextScan)
            {
                throw Invalid("waiting to retry patterns");
            }

            nextScan = DateTime.UtcNow.AddSeconds(30);
            var sections = ReadExecutableSections();
            var resolved = new Dictionary<string, ulong>();
            foreach (var entry in ContextPatterns.Concat(CorePatterns))
            {
                var match = FindUniquePattern(sections, entry.Pattern);
                var target = entry.Operand < 0 ? match : ReadUInt(match + (ulong)entry.Operand);
                if (target < moduleBase || target + 8 > moduleBase + (ulong)moduleSize)
                {
                    throw Invalid("pattern target outside module");
                }

                resolved.Add(entry.Name, target);
            }

            foreach (var entry in resolved)
            {
                addresses.Add(entry.Key, entry.Value);
            }
        }

        private static List<(ulong Address, byte[] Data)> ReadExecutableSections()
        {
            if (moduleSize < 64)
            {
                throw Invalid("invalid module size");
            }

            var offset = ReadUInt(moduleBase + 0x3C);
            if (offset > (uint)moduleSize - 24 || ReadUInt(moduleBase + offset) != 0x4550 || Hypervisor.Read<ushort>(moduleBase + offset + 4, true) != 0x14C)
            {
                throw Invalid("invalid PE header");
            }

            var count = Hypervisor.Read<ushort>(moduleBase + offset + 6, true);
            var optionalSize = Hypervisor.Read<ushort>(moduleBase + offset + 20, true);
            var table = moduleBase + offset + 24 + optionalSize;
            if (count is < 1 or > 96 || table + (ulong)count * 40 > moduleBase + (ulong)moduleSize)
            {
                throw Invalid("invalid section table");
            }

            var result = new List<(ulong, byte[])>();
            for (var index = 0; index < count; index++)
            {
                var header = table + (ulong)index * 40;
                var size = ReadUInt(header + 8);
                var rva = ReadUInt(header + 12);
                if ((ReadUInt(header + 36) & 0x20000000) == 0 || size == 0)
                {
                    continue;
                }

                if ((ulong)rva + size > (ulong)moduleSize || size > int.MaxValue)
                {
                    throw Invalid("invalid executable section");
                }

                result.Add((moduleBase + rva, Hypervisor.ReadArray(moduleBase + rva, (int)size, true)));
            }

            return result;
        }

        private static ulong FindUniquePattern(List<(ulong Address, byte[] Data)> sections, string signature)
        {
            var pattern = signature.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(value => value == "??" ? (byte?)null : byte.Parse(value, NumberStyles.HexNumber, CultureInfo.InvariantCulture)).ToArray();
            var prefix = pattern.TakeWhile(value => value.HasValue).Select(value => value!.Value).ToArray();
            if (prefix.Length == 0)
            {
                throw Invalid("empty pattern prefix");
            }

            ulong match = 0;
            foreach (var section in sections)
            {
                var offset = 0;
                while (offset <= section.Data.Length - pattern.Length)
                {
                    var found = section.Data.AsSpan(offset).IndexOf(prefix);
                    if (found < 0)
                    {
                        break;
                    }

                    var candidate = offset + found;
                    offset = candidate + 1;
                    if (candidate + pattern.Length > section.Data.Length)
                    {
                        break;
                    }

                    var matches = true;
                    for (var index = prefix.Length; index < pattern.Length; index++)
                    {
                        if (pattern[index].HasValue && section.Data[candidate + index] != pattern[index])
                        {
                            matches = false;
                            break;
                        }
                    }

                    if (!matches)
                    {
                        continue;
                    }

                    if (match != 0)
                    {
                        throw Invalid("ambiguous pattern");
                    }

                    match = section.Address + (ulong)candidate;
                }
            }

            return match == 0 ? throw Invalid("pattern not found") : match;
        }
    }
}
