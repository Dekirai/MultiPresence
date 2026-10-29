namespace MultiPresence.Models.RE4
{
    internal static partial class MemoryReader
    {
        private const string WeaponTablePattern = "55 8B EC 8B 15 ?? ?? ?? ?? 33 C0 56 85 D2 7E ?? 66 8B 75 08 B9 ?? ?? ?? ?? 8D A4 24 00 00 00 00 66 3B 31 74 ?? 40 83 C1 16 3B C2 7C ?? B8 FF FF 00 00 5E 5D C3 8B 0D ?? ?? ?? ?? 6B C0 0B 03 45 0C";

        private static readonly (string Name, string Pattern, int Operand)[] CorePatterns =
        [
            ("HealthLayout", "8B 15 ?? ?? ?? ?? 0F B6 82 C8 4F 00 00 83 F8 05 77 ?? FF 24 85 ?? ?? ?? ?? B8 58 02 00 00 EB ?? B8 B0 04 00 00 0F BF C8 56 0F BF B2 B4 4F 00 00", -1),
            ("ItemManager", "80 B9 C0 4F 00 00 10 0F 84 ?? ?? ?? ?? B9 ?? ?? ?? ?? E8 ?? ?? ?? ?? 66 85 C0 0F 84 ?? ?? ?? ?? D9 05 ?? ?? ?? ?? A1", 14),
            ("WeaponTable", WeaponTablePattern, 21),
            ("WeaponTableCount", WeaponTablePattern, 5)
        ];

        private sealed record CoreState
        {
            public int Health { get; init; }
            public int MaxHealth { get; init; }
            public string HealthStatus { get; init; } = "";
            public int WeaponId { get; init; } = -1;
            public int Ammo { get; init; } = -1;
            public int ReserveAmmo { get; init; } = -1;
            public bool? InfiniteAmmo { get; init; }
            public int Money { get; init; } = -1;
        }

        private static CoreState ReadCore(ulong global, ulong player)
        {
            if (global == 0 || player == 0)
            {
                throw new InvalidOperationException("The player state is unavailable.");
            }
            int health = ReadShort(global + 0x4FB4);
            int maxHealth = ReadShort(global + 0x4FB6);
            if (maxHealth <= 0 || health < 0 || health > maxHealth)
            {
                throw new InvalidOperationException("The player health state is unavailable.");
            }

            int money = ReadInt(global + 0x4FA8);
            // The native condition getter uses the character's base health, not upgraded maximum health.
            int conditionBase = ReadByte(global + 0x4FC8) == 1 ? 600 : 1200;
            var core = new CoreState
            {
                Health = health,
                MaxHealth = maxHealth,
                HealthStatus = health == 0 ? "Dead" : health >= conditionBase * 2 / 3 ? "Fine" : health >= conditionBase / 3 ? "Caution" : "Danger",
                Money = money is >= 0 and <= 999999999 ? money : -1
            };
            return ReadEquipment(core);
        }

        private static CoreState ReadEquipment(CoreState core)
        {
            ulong manager = addresses["ItemManager"];
            int id = ReadUShort(manager + 0x10);
            if (id == 0x800)
            {
                return core with { WeaponId = id };
            }

            ulong inventory = Pointer(manager + 0x14);
            int count = ReadInt(manager + 0x1C);
            ulong weapon = Pointer(manager + 0xC);
            int owner = ReadByte(manager + 0x13);
            if (inventory == 0 || count is <= 0 or > 512 || weapon < inventory ||
                weapon - inventory >= (ulong)count * 0xE || (weapon - inventory) % 0xE != 0 ||
                ReadUShort(weapon) != id || (ReadByte(weapon + 4) & 1) == 0 || ReadByte(weapon + 5) != owner)
            {
                return core;
            }

            ulong definition = FindWeaponDefinition(id);
            if (definition == 0)
            {
                return core with { WeaponId = id };
            }

            int ammoId = ReadUShort(definition + 6);
            int capacityLevel = ReadUShort(weapon + 6) >> 12;
            bool? infinite = capacityLevel < 7 ? ReadUShort(definition + 8 + (ulong)capacityLevel * 2) == 0x8000 : null;
            int ammo = ReadUShort(weapon + 8) >> 3;
            int reserve = ammoId < 0xFF ? ReadReserve(inventory, count, owner, ammoId) : -1;

            // Grenades, eggs and launchers hold their count in the item quantity.
            if (id is 1 or 2 or 8 or 9 or 10 or 14 or 23 or 53 or 109)
            {
                ammo = ReadUShort(weapon + 2);
            }
            if (id == 109)
            {
                infinite = true;
            }
            // Both bows consume ammunition directly from the inventory.
            if (id is 16 or 82)
            {
                ammo = reserve;
                reserve = 0;
            }
            if (id == 56)
            {
                ammo = -1;
                reserve = -1;
                infinite = null;
            }

            if (Pointer(manager + 0xC) != weapon || ReadUShort(manager + 0x10) != id ||
                Pointer(manager + 0x14) != inventory || ReadByte(manager + 0x13) != owner)
            {
                return core;
            }
            return core with { WeaponId = id, Ammo = ammo, ReserveAmmo = reserve, InfiniteAmmo = infinite };
        }

        private static ulong FindWeaponDefinition(int id)
        {
            int count = ReadInt(addresses["WeaponTableCount"]);
            if (count is <= 0 or > 128)
            {
                return 0;
            }
            ulong table = addresses["WeaponTable"];
            for (int index = 0; index < count; index++)
            {
                ulong entry = table + (ulong)index * 0x16;
                if (ReadUShort(entry) == id)
                {
                    return entry;
                }
            }
            return 0;
        }

        private static int ReadReserve(ulong inventory, int count, int owner, int ammoId)
        {
            int total = 0;
            for (int index = 0; index < count; index++)
            {
                ulong item = inventory + (ulong)index * 0xE;
                if ((ReadByte(item + 4) & 1) != 0 && ReadByte(item + 5) == owner && ReadUShort(item) == ammoId)
                {
                    total += ReadUShort(item + 2);
                }
            }
            return total;
        }
    }
}
