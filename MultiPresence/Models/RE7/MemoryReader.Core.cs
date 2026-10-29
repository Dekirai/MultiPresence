namespace MultiPresence.Models.RE7
{
    internal static partial class MemoryReader
    {
        private sealed record CoreState
        {
            public string State { get; init; } = "Loading";
            public string Mode { get; init; } = "";
            public bool IsGameplay => State is "Main Story" or "DLC Story" or "DLC Minigame";
            public ulong GameManager { get; init; }
            public ulong ObjectManager { get; init; }
            public ulong Player { get; init; }
            public ulong PlayerStatus { get; init; }
            public int Chapter { get; init; } = -1;
            public string Character { get; init; } = "";
            public string CharacterId { get; init; } = "";
            public float Health { get; init; }
            public float MaxHealth { get; init; }
            public string HealthStatus { get; init; } = "";
            public int DifficultyId { get; init; } = -1;
            public string Difficulty { get; init; } = "";
            public string Weapon { get; init; } = "";
            public string WeaponId { get; init; } = "";
            public int Ammo { get; init; } = -1;
            public int ReserveAmmo { get; init; } = -1;
            public bool? InfiniteAmmo { get; init; }
        }

        private static CoreState ReadCore()
        {
            ulong manager = Pointer(addresses["GameManager"]);
            ulong objects = Pointer(addresses["ObjectManager"]);
            var result = new CoreState { GameManager = manager, ObjectManager = objects };
            if (!NativeObject(manager) || !ManagedTypeIs(manager, 0x43FEF87B))
            {
                return result;
            }

            int chapter = ReadInt(manager + 0x54);
            result = result with { Chapter = chapter };
            if (ReadByte(manager + 0x1A0) != 0 || ReadByte(manager + 0x1A1) != 0)
            {
                return result;
            }
            if (chapter is 0 or 1 or 3 or 21 or 28 or 33 || ReadByte(manager + 0x133) != 0 || ReadByte(manager + 0x134) != 0)
            {
                return result with { State = "Main Menu" };
            }
            if (chapter < 0 || chapter > 37 || chapter == 19)
            {
                return result;
            }

            if (!NativeObject(objects) || !ManagedTypeIs(objects, 0x4173EAAC))
            {
                return result;
            }
            ulong player = Pointer(objects + 0x88);
            result = result with { Player = player };
            if (player == 0 || Pointer(player + 0x18) == 0)
            {
                return result;
            }
            ulong status = FindPlayerStatus(player);
            if (status == 0)
            {
                return result;
            }
            result = result with { PlayerStatus = status };
            ulong damage = Pointer(status + 0x68);
            if (!NativeObject(damage) || Pointer(damage + 0x10) != player)
            {
                return result;
            }
            ulong hp = Pointer(damage + 0x70);
            if (hp == 0 || !ManagedTypeIs(hp, 0xF7D467AD))
            {
                return result;
            }
            float health = ReadFloat(hp + 0x14);
            float maxHealth = ReadFloat(hp + 0x10);
            if (!float.IsFinite(health) || !float.IsFinite(maxHealth) || maxHealth <= 0 || maxHealth > 100000 || health < 0 || health > maxHealth * 1.01f)
            {
                return result;
            }

            string pawn = ReadString(Pointer(player + 0x28));
            (string state, string mode) = GetMode(chapter);
            int difficulty = state == "Main Story" ? ReadInt(manager + 0xE8) : -1;
            result = result with
            {
                State = state,
                Mode = mode,
                CharacterId = pawn,
                Character = GetCharacter(pawn),
                Health = health,
                MaxHealth = maxHealth,
                HealthStatus = health <= 0 ? "Dead" : health / maxHealth > 0.75f ? "Fine" : health / maxHealth > 0.25f ? "Caution" : "Danger",
                DifficultyId = difficulty is >= 0 and <= 2 ? difficulty : -1,
                Difficulty = difficulty switch
                {
                    0 => "Easy",
                    1 => "Normal",
                    2 => "Madhouse",
                    _ => ""
                }
            };
            return ReadEquipment(result);
        }

        private static ulong FindPlayerStatus(ulong player)
        {
            var seen = new HashSet<ulong>();
            for (ulong component = Pointer(player + 0x18); component != 0 && seen.Count < 256 && seen.Add(component); component = Pointer(component + 0x18))
            {
                if (Pointer(component + 0x10) != player)
                {
                    break;
                }
                if (ManagedTypeIs(component, 0xEF2C7547))
                {
                    return component;
                }
            }
            return 0;
        }

        private static bool ManagedTypeIs(ulong instance, uint expectedHash)
        {
            if (instance == 0)
            {
                return false;
            }
            ulong info = Pointer(instance);
            if (info == 0)
            {
                return false;
            }
            ulong type = Pointer(info);
            if (type == 0)
            {
                return false;
            }
            int index = (int)(ReadUInt(type) & 0x3FFFF);
            if (index <= 0 || index > 200000 || type < (ulong)index * 0x50)
            {
                return false;
            }
            ulong table = type - (ulong)index * 0x50;
            for (int depth = 0; depth < 16 && index != 0; depth++)
            {
                type = table + (ulong)index * 0x50;
                if (ReadUInt(type + 0x18) == expectedHash)
                {
                    return true;
                }
                uint low = ReadUInt(type);
                uint high = ReadUInt(type + 4);
                index = (int)(((low >> 18) | (high << 14)) & 0x3FFFF);
                if (index > 200000)
                {
                    return false;
                }
            }
            return false;
        }

        private static CoreState ReadEquipment(CoreState core)
        {
            ulong equip = Pointer(core.PlayerStatus + 0x128);
            if (!NativeObject(equip) || Pointer(equip + 0x10) != core.Player)
            {
                return core;
            }
            int id = ReadInt(equip + 0x70);
            string itemId = ReadString(Pointer(equip + 0x60));
            string weaponName = GetWeaponName(id);
            core = core with { Weapon = weaponName, WeaponId = itemId.Length > 0 ? itemId : GetWeaponId(id) };
            ulong weapon = Pointer(equip + 0x58);
            if (!NativeObject(weapon) || !ManagedTypeIs(weapon, 0x0DB02687))
            {
                return core;
            }
            ulong bullet = Pointer(weapon + 0x190);
            if (bullet == 0)
            {
                return core;
            }
            int ammo = ReadInt(bullet + 0x14);
            if (ammo < 0 || ammo > 100000)
            {
                return core;
            }
            ulong parameters = Pointer(weapon + 0x180);
            bool? infinite = parameters == 0 ? null : ReadByte(parameters + 0x24) != 0 || ReadByte(parameters + 0x25) != 0;
            ulong skills = Pointer(core.PlayerStatus + 0x110);
            if (NativeObject(skills) && ReadInt(skills + 0xA0) > 0)
            {
                infinite = true;
            }
            int reserve = ReadReserve(Pointer(weapon + 0x150), ReadInt(bullet + 0x10));
            return core with { Ammo = ammo, ReserveAmmo = reserve, InfiniteAmmo = infinite };
        }

        private static int ReadReserve(ulong inventory, int bulletId)
        {
            string itemId = bulletId switch
            {
                73 => "HandgunBullet", 74 => "HandgunBulletL", 75 => "ShotgunBullet", 76 => "MachineGunBullet",
                77 => "MagnumBullet", 78 => "BurnerBullet", 79 => "FlameBulletS", 80 => "FlameBulletL",
                81 => "AcidBulletS", 82 => "AcidBulletL", 100 => "HotloadBullet", 154 => "AlbertHandgunBullet",
                155 => "AlbertHandgunBulletL", 156 => "AlbertShotgunBullet", _ => ""
            };
            if (itemId.Length == 0 || !NativeObject(inventory))
            {
                return -1;
            }
            ulong list = Pointer(inventory + 0x70);
            if (list == 0)
            {
                return -1;
            }
            int count = ReadInt(list + 0x18);
            ulong array = Pointer(list + 0x10);
            if (count < 0 || count > 256 || array == 0 || count > ReadInt(array + 0x1C))
            {
                return -1;
            }
            int total = 0;
            for (int index = 0; index < count; index++)
            {
                ulong entry = Pointer(array + 0x20 + (ulong)index * 8);
                ulong item = entry == 0 ? 0 : Pointer(entry + 0x18);
                if (!NativeObject(item))
                {
                    continue;
                }
                if (ReadItemId(item) != itemId)
                {
                    continue;
                }
                int stack = ReadInt(item + 0x88);
                if (stack < 0 || stack > 100000)
                {
                    return -1;
                }
                total += stack;
                if (total > 100000)
                {
                    return -1;
                }
            }
            return total;
        }

        private static string ReadItemId(ulong item)
        {
            ulong switcher = Pointer(item + 0xC8);
            if (NativeObject(switcher))
            {
                int index = ReadInt(switcher + 0x58);
                ulong list = Pointer(switcher + 0x50);
                if (list != 0 && index >= 0 && index < ReadInt(list + 0x18) && index < 256)
                {
                    ulong array = Pointer(list + 0x10);
                    if (array != 0 && index < ReadInt(array + 0x1C))
                    {
                        ulong data = Pointer(array + 0x20 + (ulong)index * 8);
                        if (data != 0)
                        {
                            return ReadString(Pointer(data + 0x18));
                        }
                    }
                }
            }
            return ReadString(Pointer(item + 0x80));
        }

        private static (string State, string Mode) GetMode(int chapter)
        {
            return chapter switch
            {
                20 or 22 or 23 or 24 or 25 or 26 => ("DLC Minigame", "Jack's 55th Birthday"),
                29 => ("DLC Minigame", "Banned Footage - Bedroom"),
                30 => ("DLC Minigame", "Banned Footage - 21"),
                31 => ("DLC Minigame", "Banned Footage - Nightmare"),
                32 => ("DLC Minigame", "Banned Footage - Daughters"),
                34 => ("DLC Story", "Ethan Must Die"),
                35 => ("DLC Story", "Not a Hero"),
                37 => ("DLC Story", "End of Zoe"),
                _ => ("Main Story", "Main Story")
            };
        }

        private static string GetCharacter(string pawn)
        {
            return pawn switch
            {
                "Pl0000" or "Pl0000_Chapter1" or "Pl0000_IMD" => "Ethan Winters",
                "Pl1000" => "Chris Redfield",
                "Pl2000" or "Pl2100" or "Pl2000_Birthday" => "Mia Winters",
                "Pl3000" or "Pl3100_Chapter7_1" or "Pl3100_Chapter7_2" or "Pl3100_Chapter7_3" => "Clancy Jarvis",
                "Pl3400" => "Zoe Baker",
                "Pl9000" => "Joe Baker",
                _ => ""
            };
        }

        private static string GetWeaponName(int id)
        {
            return id switch
            {
                0 => "Unarmed", 1 => "Axe", 2 => "Circular Saw", 3 or 48 => "Knife", 5 => "Handgun",
                6 => "M19 Handgun", 7 => "G17 Handgun", 8 => "MPM Handgun", 9 => "Albert-01", 10 => "Shotgun",
                11 or 12 => "M37 Shotgun", 13 => "M21 Shotgun", 14 => "P19 Machine Gun", 15 => "44 MAG",
                16 => "Grenade Launcher", 17 => "Burner", 18 => "Candle", 21 or 22 => "Remote Bomb",
                28 or 42 or 57 => "Knife", 29 => "Chainsaw", 31 => "Flashlight", 32 => "Bolt Cutters",
                35 or 55 => "Lantern", 41 or 47 => "Axe", 44 => "Blaster", 46 => "Albert-01R",
                49 => "Samurai Edge", 50 => "Thor's Hammer", 51 => "Blue Blaster", 52 => "Red Blaster",
                56 => "Lighter", 58 => "Grenade", 59 => "Incendiary Grenade", 60 => "Stun Grenade", _ => ""
            };
        }

        private static string GetWeaponId(int id)
        {
            return id switch
            {
                0 => "Hand", 1 => "HandAxe", 2 => "CircularSaw", 3 => "Knife", 5 => "Handgun",
                6 => "Handgun_M19", 7 => "Handgun_G17", 8 => "Handgun_MPM", 9 => "Handgun_Albert",
                10 => "ShotGun", 11 => "Shotgun_M37", 12 => "Shotgun_M37S", 13 => "Shotgun_DB",
                14 => "MachineGun", 15 => "Magnum", 16 => "GrenadeLauncher", 17 => "Burner",
                18 => "Candle", 21 => "LiquidBomb", 22 => "Timebomb", 28 => "KitchenKnife", 29 => "ChainSaw",
                31 => "HandLight", 32 => "ChainCutter", 35 => "Lantern", 41 => "FireAxe", 42 => "MiaKnife",
                44 => "HyperBlaster", 46 => "Handgun_Albert_Reward", 47 => "FireAxeBreakable", 48 => "CKnife",
                49 => "Handgun_Albert_C", 50 => "Shotgun_Albert", 51 => "BlueBlaster", 52 => "RedBlaster",
                55 => "Lantern_C", 56 => "Lighter_Z", 57 => "GimmickKnife", 58 => "Grenadebomb",
                59 => "Thermatebomb", 60 => "Stangrenadebomb", _ => id is >= 0 and <= 70 ? id.ToString() : ""
            };
        }
    }
}
