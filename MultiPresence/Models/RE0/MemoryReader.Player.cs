namespace MultiPresence.Models.RE0
{
    internal static partial class MemoryReader
    {
        private readonly record struct PlayerData(ulong Manager, ulong Player, ulong Inventory, int CharacterId, string Character,
            int WeaponId, string Weapon, int Ammo, int ReserveAmmo, bool InfiniteAmmo, int Health);

        private static PlayerData ReadPlayer()
        {
            ulong manager = Pointer(addresses["Player"]);
            ulong player = manager == 0 ? 0 : Pointer(manager + 0x2C);
            if (player == 0)
            {
                return new PlayerData(manager, 0, 0, -1, "", -1, "", -1, -1, false, -1);
            }

            ulong vtable = Pointer(player);
            if (!InPlayerModule(vtable, 0x14))
            {
                throw Invalid("invalid player type");
            }
            ulong getter = Pointer(vtable + 0x10);
            if (!InPlayerModule(getter, 6) || ReadByte(getter) != 0xB8 || ReadByte(getter + 5) != 0xC3)
            {
                throw Invalid("invalid character type getter");
            }
            ulong metadata = Pointer(getter + 1);
            if (!InPlayerModule(metadata, 0x20))
            {
                throw Invalid("invalid character metadata");
            }
            uint type = ReadUInt(metadata + 0x1C);
            int characterId = (int)(type & 0xFF);
            if ((type & 0xFFFFFF00) != 0x80000000 || characterId is not (1 or 2 or 3 or 5 or 7))
            {
                throw Invalid("unknown player character");
            }

            string character = characterId is 1 or 2 or 3 ? "Rebecca Chambers" : "Billy Coen";
            int health = ReadInt(player + 0x1030);
            if (health < -100000 || health > 1000000)
            {
                throw Invalid("invalid player health");
            }
            ulong itemManager = Pointer(addresses["Inventory"]);
            if (itemManager == 0)
            {
                return new PlayerData(manager, player, 0, characterId, character, -1, "", -1, -1, false, Math.Max(0, health));
            }

            ulong inventory = itemManager + (characterId is 1 or 2 or 3 ? 0x20UL : 0x60UL);
            int equipped = ReadInt(inventory + 0x3C);
            if (equipped is < -1 or > 5)
            {
                throw Invalid("invalid equipped slot");
            }
            int weaponId = 1;
            int ammo = -1;
            int reserve = -1;
            bool infinite = false;
            if (equipped >= 0)
            {
                ulong slot = inventory + 4 + (ulong)equipped * 8;
                weaponId = ReadInt(slot);
                int count = ReadInt(slot + 4);
                if (weaponId is < 0 or > 24 || count is < 0 or > 65535)
                {
                    throw Invalid("invalid equipped weapon");
                }
                if (weaponId > 2)
                {
                    ammo = count;
                    infinite = HasInfiniteAmmo(weaponId, count);
                    int ammoId = weaponId switch
                    {
                        3 or 4 or 17 or 19 => 32,
                        5 or 6 => 33,
                        7 => 35,
                        8 => 36,
                        9 => 37,
                        10 or 22 => 34,
                        11 => 40,
                        _ => -1
                    };
                    if (ammoId >= 0)
                    {
                        reserve = ReadReserveAmmo(inventory, ammoId);
                    }
                }
                if (ReadInt(slot) != weaponId)
                {
                    throw Invalid("weapon changed during read");
                }
            }

            if (Pointer(addresses["Player"]) != manager || Pointer(manager + 0x2C) != player ||
                Pointer(addresses["Inventory"]) != itemManager || ReadInt(inventory + 0x3C) != equipped)
            {
                throw Invalid("player changed during read");
            }
            return new PlayerData(manager, player, inventory, characterId, character, weaponId, Weapons.GetWeapon(weaponId),
                ammo, reserve, infinite, Math.Max(0, health));
        }

        private static int ReadReserveAmmo(ulong inventory, int ammoId)
        {
            int total = 0;
            for (int index = 0; index < 6; index++)
            {
                ulong slot = inventory + 4 + (ulong)index * 8;
                if (ReadInt(slot) == ammoId)
                {
                    int count = ReadInt(slot + 4);
                    if (count is < 0 or > 65535)
                    {
                        throw Invalid("invalid reserve ammunition");
                    }
                    total += count;
                }
            }
            return total;
        }

        private static bool HasInfiniteAmmo(int weaponId, int count)
        {
            if (count == 65535)
            {
                return true;
            }
            ulong game = Pointer(addresses["GameInfo"]);
            ulong mode = Pointer(addresses["AmmoMode"]);
            if (game == 0 || mode == 0 || ReadInt(game + 0x5C) == 1 || ReadInt(mode + 0x5C) != 0)
            {
                return false;
            }
            uint flags = ReadUInt(game + 0x30);
            return ((flags & 0x80) != 0 && weaponId is 3 or 4 or 17 or 19) ||
                ((flags & 0x40) != 0 && weaponId == 5) || ((flags & 0x10) != 0 && weaponId is >= 0 and <= 24);
        }

        private static bool InPlayerModule(ulong address, ulong length)
        {
            return address >= moduleBase && address < moduleBase + (ulong)moduleSize &&
                length <= moduleBase + (ulong)moduleSize - address;
        }
    }
}
