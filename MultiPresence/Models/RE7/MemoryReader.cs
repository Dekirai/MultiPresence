using System.Globalization;
using System.Text;

namespace MultiPresence.Models.RE7
{
    internal static partial class MemoryReader
    {
        private static readonly Dictionary<string, ulong> addresses = new Dictionary<string, ulong>();
        private static int processId, moduleSize;
        private static ulong moduleBase;
        private static DateTime nextScan;

        public static void Reset()
        {
            addresses.Clear();
            processId = moduleSize = 0;
            moduleBase = 0;
            nextScan = DateTime.MinValue;
        }

        public static GameSnapshot ReadSnapshot()
        {
            ResolveAddresses();
            var core = ReadCore();
            if (core.State is not "Main Story" and not "DLC Story" and not "DLC Minigame")
            {
                return new GameSnapshot() { State = core.State };
            }

            var location = ReadLocation();
            var objectives = ReadObjectives();
            if (core != ReadCore() || location != ReadLocation())
            {
                throw Invalid("scene changed during read");
            }

            return new GameSnapshot()
            {
                State = core.State,
                Mode = core.Mode,
                Character = core.Character,
                CharacterId = core.CharacterId,
                Health = core.Health,
                MaxHealth = core.MaxHealth,
                HealthStatus = core.HealthStatus,
                DifficultyId = core.DifficultyId,
                Difficulty = core.Difficulty,
                Location = location.Name,
                Area = location.Area,
                Room = location.Room,
                RoomId = location.Id == Guid.Empty ? "" : location.Id.ToString(),
                MapId = location.MapId,
                PrimaryObjective = objectives.Primary,
                SecondaryObjective = objectives.Secondary,
                Objective = objectives.Current,
                ObjectivePath = objectives.Path,
                Objectives = objectives.All,
                PrimaryObjectiveId = objectives.PrimaryId,
                SecondaryObjectiveId = objectives.SecondaryId,
                ObjectiveId = objectives.CurrentId,
                Weapon = core.Weapon,
                WeaponId = core.WeaponId,
                Ammo = core.Ammo,
                ReserveAmmo = core.ReserveAmmo,
                InfiniteAmmo = core.InfiniteAmmo
            };
        }

        private static InvalidDataException Invalid(string value)
        {
            return new InvalidDataException("RE7: " + value + ".");
        }

        private static ulong Pointer(ulong address)
        {
            var value = Hypervisor.Read<ulong>(address, true);
            if (value != 0 && (value < 0x10000 || value > 0x7FFFFFFFFFFF || (value & 7) != 0))
            {
                throw Invalid("invalid pointer");
            }

            return value;
        }

        private static int ReadInt(ulong address)
        {
            return Hypervisor.Read<int>(address, true);
        }

        private static uint ReadUInt(ulong address)
        {
            return Hypervisor.Read<uint>(address, true);
        }

        private static byte ReadByte(ulong address)
        {
            return Hypervisor.Read<byte>(address, true);
        }

        private static float ReadFloat(ulong address)
        {
            return Hypervisor.Read<float>(address, true);
        }

        private static Guid ReadGuid(ulong address)
        {
            return new Guid(Hypervisor.ReadArray(address, 16, true));
        }

        private static string ReadString(ulong address)
        {
            if (address == 0)
            {
                return "";
            }

            var length = ReadInt(address + 0x10);
            if (length is < 0 or > 2048)
            {
                throw Invalid("invalid string length");
            }

            return Encoding.Unicode.GetString(Hypervisor.ReadArray(address + 0x14, length * 2, true));
        }

        private static bool NativeObject(ulong address)
        {
            return address != 0 && Pointer(address + 0x10) != 0;
        }

        private static ulong[] ReadList(ulong list, int maximum)
        {
            if (list == 0)
            {
                return [];
            }

            var items = Pointer(list + 0x10);
            var count = ReadInt(list + 0x18);
            var capacity = items == 0 ? 0 : ReadInt(items + 0x1C);
            if (count < 0 || count > maximum || capacity < count || capacity > 65536)
            {
                throw Invalid("invalid list length");
            }

            var result = new ulong[count];
            for (var index = 0; index < count; index++)
            {
                result[index] = Pointer(items + 0x20 + (ulong)index * 8);
            }

            if (items != Pointer(list + 0x10) || count != ReadInt(list + 0x18))
            {
                throw Invalid("list changed during read");
            }

            return result;
        }

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
            foreach (var entry in CorePatterns.Concat(LocationPatterns).Concat(ObjectivePatterns))
            {
                var match = FindUniquePattern(sections, entry.Pattern);
                var target = entry.Length == 0 ? match : checked((ulong)((long)match + entry.Length + ReadInt(match + (ulong)entry.Operand)));
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
            if (offset > (uint)moduleSize - 24 || ReadUInt(moduleBase + offset) != 0x4550 || Hypervisor.Read<ushort>(moduleBase + offset + 4, true) != 0x8664)
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
