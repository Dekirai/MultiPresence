using System.Diagnostics;

namespace MultiPresence.Models.RE
{
    internal static class MemoryReader
    {
        private const string RoomPattern = "A1 ?? ?? ?? ?? 8B 88 ?? ?? ?? ?? C1 E1 08 0B 88 ?? ?? ?? ?? 81 F9 02 03 00 00";
        private const string CameraPattern = "8B 0D ?? ?? ?? ?? 8B 81 ?? ?? ?? ?? EB ?? 8B 0D ?? ?? ?? ?? 8B 81 ?? ?? ?? ?? 83 F8 FF 75 ?? 8B 81 ?? ?? ?? ?? 8D 91";
        private const string CharacterPattern = "A1 ?? ?? ?? ?? 8B CB 8B B0 ?? ?? ?? ?? E8 ?? ?? ?? ?? 3B F0 74 ??";
        private const string WeaponPattern = "8B 0D ?? ?? ?? ?? 8B 81 ?? ?? ?? ?? 8B 89 ?? ?? ?? ?? 89 87 ?? ?? ?? ?? 33 C0 89 8F ?? ?? ?? ??";
        private const string PlayerPattern = "8B 0D ?? ?? ?? ?? 6A 00 6A 00 50 6A 1C E8 ?? ?? ?? ?? FF 86 B4 00 00 00";
        private sealed record Addresses(uint Game, uint Player, uint Floor, uint Room, uint Character, uint Weapon,
            (uint Override, uint Current)? Camera);
        private static Addresses? addresses;
        private static int processId;
        private static DateTime nextScan;

        public static void Reset()
        {
            addresses = null;
            processId = 0;
            nextScan = DateTime.MinValue;
        }

        public static GameSnapshot ReadSnapshot()
        {
            var found = ResolveAddresses();
            ulong game = Hypervisor.Read<uint>(found.Game, true);
            if (game == 0) return new GameSnapshot();
            int floor = Hypervisor.Read<int>(game + found.Floor, true);
            if (floor <= 0) return new GameSnapshot();

            int room = Hypervisor.Read<int>(game + found.Room, true);
            int? camera = ReadCamera(game, found);
            byte character = Hypervisor.Read<byte>(game + found.Character, true);
            int weapon = Hypervisor.Read<int>(game + found.Weapon, true);
            ulong player = Hypervisor.Read<uint>(found.Player, true);
            if (player == 0) return new GameSnapshot();
            // The title screen retains the last room but releases the playable actor.
            ulong actor = Hypervisor.Read<uint>(player + 0x23C, true);
            if (actor == 0) return new GameSnapshot();
            ulong stats = Hypervisor.Read<uint>(actor + 0x154, true);
            if (stats == 0) return new GameSnapshot();
            ulong health = Hypervisor.Read<uint>(stats + 0x2BC, true);
            if (health == 0) return new GameSnapshot();
            int currentHealth = Hypervisor.Read<int>(health + 0x3BC, true);
            int maxHealth = Hypervisor.Read<int>(health + 0x3C0, true);
            byte ammoClip = 0, maxAmmoClip = 0;
            if (weapon is >= 3 and <= 15)
            {
                ulong equipped = Hypervisor.Read<uint>(player + 0x1C4, true);
                if (equipped == 0) return new GameSnapshot();
                ammoClip = Hypervisor.Read<byte>(equipped + 0xFFC, true);
                maxAmmoClip = Hypervisor.Read<byte>(equipped + 0x1000, true);
            }

            if (Hypervisor.Read<uint>(found.Game, true) != game || Hypervisor.Read<uint>(found.Player, true) != player ||
                Hypervisor.Read<uint>(player + 0x23C, true) != actor ||
                Hypervisor.Read<int>(game + found.Floor, true) != floor || Hypervisor.Read<int>(game + found.Room, true) != room ||
                Hypervisor.Read<byte>(game + found.Character, true) != character || Hypervisor.Read<int>(game + found.Weapon, true) != weapon)
                return new GameSnapshot();

            return new GameSnapshot
            {
                State = "Ingame",
                Floor = floor,
                Room = room,
                Camera = camera.HasValue && ReadCamera(game, found) == camera ? camera : null,
                Character = character,
                Weapon = weapon,
                Health = currentHealth,
                MaxHealth = maxHealth,
                AmmoClip = ammoClip,
                MaxAmmoClip = maxAmmoClip
            };
        }

        private static int? ReadCamera(ulong game, Addresses found)
        {
            if (found.Camera is not { } camera) return null;
            if (Hypervisor.TryRead<int>(game + camera.Override, out int angle, true) && angle >= 0) return angle;
            return Hypervisor.TryRead<int>(game + camera.Current, out angle, true) && angle >= 0 ? angle : null;
        }

        private static Addresses ResolveAddresses()
        {
            var process = Hypervisor.Process ?? throw new InvalidOperationException("Resident Evil is not attached.");
            if (process.Id != processId)
            {
                Reset();
                processId = process.Id;
            }
            if (addresses != null) return addresses;
            if (DateTime.UtcNow < nextScan) throw new InvalidDataException("Waiting to retry Resident Evil patterns.");
            nextScan = DateTime.UtcNow.AddSeconds(10);

            var module = process.MainModule ?? throw new InvalidDataException("Missing Resident Evil module.");
            var code = ReadCode(module);
            int room = Find(code, RoomPattern);
            int character = Find(code, CharacterPattern);
            int weapon = Find(code, WeaponPattern);
            int player = Find(code, PlayerPattern);
            uint gameAddress = BitConverter.ToUInt32(code, room + 1);
            uint playerAddress = BitConverter.ToUInt32(code, player + 2);
            ulong start = (ulong)module.BaseAddress.ToInt64();
            ulong end = start + (ulong)module.ModuleMemorySize;
            if (gameAddress < start || gameAddress + 4UL > end || playerAddress < start || playerAddress + 4UL > end ||
                gameAddress == playerAddress || BitConverter.ToUInt32(code, character + 1) != gameAddress ||
                BitConverter.ToUInt32(code, weapon + 2) != gameAddress)
                throw new InvalidDataException("Invalid Resident Evil game addresses.");

            int camera = FindUnique(code, CameraPattern);
            (uint, uint)? cameraOffsets = null;
            if (camera >= 0 && BitConverter.ToUInt32(code, camera + 2) == gameAddress &&
                BitConverter.ToUInt32(code, camera + 16) == gameAddress &&
                BitConverter.ToUInt32(code, camera + 22) == BitConverter.ToUInt32(code, camera + 33))
                cameraOffsets = (BitConverter.ToUInt32(code, camera + 8), BitConverter.ToUInt32(code, camera + 22));

            return addresses = new Addresses(gameAddress, playerAddress, BitConverter.ToUInt32(code, room + 7),
                BitConverter.ToUInt32(code, room + 16), BitConverter.ToUInt32(code, character + 9),
                BitConverter.ToUInt32(code, weapon + 8), cameraOffsets);
        }

        private static byte[] ReadCode(ProcessModule module)
        {
            ulong start = (ulong)module.BaseAddress.ToInt64();
            var header = Hypervisor.ReadArray(start, 4096, true);
            int pe = BitConverter.ToInt32(header, 0x3C);
            if (pe < 64 || pe > header.Length - 64 || BitConverter.ToUInt32(header, pe) != 0x4550 ||
                BitConverter.ToUInt16(header, pe + 4) != 0x14C || BitConverter.ToUInt16(header, pe + 24) != 0x10B)
                throw new InvalidDataException("Invalid Resident Evil PE header.");
            int count = BitConverter.ToUInt16(header, pe + 6);
            int table = pe + 24 + BitConverter.ToUInt16(header, pe + 20);
            if (count is < 1 or > 96 || table + count * 40 > header.Length) throw new InvalidDataException("Invalid section table.");
            for (int i = 0; i < count; i++)
            {
                int section = table + i * 40;
                if (!header.AsSpan(section, 8).SequenceEqual(".text\0\0\0"u8)) continue;
                int length = BitConverter.ToInt32(header, section + 8);
                int rva = BitConverter.ToInt32(header, section + 12);
                if (rva <= 0 || length <= 0 || (long)rva + length > module.ModuleMemorySize ||
                    (BitConverter.ToUInt32(header, section + 36) & 0x20000000) == 0) break;
                return Hypervisor.ReadArray(start + (ulong)rva, length, true);
            }
            throw new InvalidDataException("Missing Resident Evil code section.");
        }

        private static int Find(byte[] code, string signature)
        {
            int match = FindUnique(code, signature);
            if (match < 0) throw new InvalidDataException("Missing or ambiguous Resident Evil signature.");
            return match;
        }

        private static int FindUnique(byte[] code, string signature)
        {
            var pattern = SignaturePattern.Parse(signature);
            int match = pattern.IndexOf(code);
            return match >= 0 && pattern.IndexOf(code.AsSpan(match + 1)) < 0 ? match : -1;
        }
    }
}
