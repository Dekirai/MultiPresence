using MultiPresence.Runtime;
using MultiPresence.Infrastructure;
using DiscordRPC;
using MultiPresence.Models.RE3;

namespace MultiPresence.Presence
{
    public class RE3R
    {
        private const string PlayerConditionSlotPattern = "48 8B 15 ?? ?? ?? ?? 48 85 D2 75 ?? 45 33 C0 8D 57 38 48 8B CB E8 ?? ?? ?? ?? 48 8B D7 48 8B 43 50 48 39 78 18 75 ?? 48 85 D2 74 ?? 48 39 7A 10 74 ?? 32 C0 EB ?? 48 8B 52 50 EB ?? B0 01 0F B6 C0 85 C0 75 ?? 48 85 D2 75 ?? 45 33 C0 8D 50 38 48 8B CB E8 ?? ?? ?? ?? 33 C0 48 8B 5C 24 30 48 83 C4 20 5F C3 48 8B 8A C0 02 00 00";
        private const string ConditionHealthLayoutPattern = "48 8B 8A C0 02 00 00 48 85 C9 74 ?? 48 39 79 10 74 ?? 32 C0 EB ?? B0 01 0F B6 C0 85 C0 75 ?? 48 85 C9 75 ?? 45 33 C0 8D 50 38 48 8B CB E8 ?? ?? ?? ?? 48 8B 4B 50 8B C7 48 39 79 18 0F 45 C7 48 8B 5C 24 30 48 83 C4 20 5F C3 8B 41 58 48 8B 4B 50 48 39 79 18";
        private const string HealthValueLayoutPattern = "44 89 42 54 48 8B 41 50 48 83 78 18 00 75 ?? 45 3B C8 44 89 44 24 08 48 8D 44 24 18 44 89 4C 24 18 48 8D 4C 24 08 48 0F 4D C1 8B 00 89 42 58 C3";
        private const string CharacterLayoutPattern = "8B 5A 54 48 8B 15 ?? ?? ?? ?? E8 ?? ?? ?? ?? 48 8B D0 48 8B CF 89 58 10 E8 ?? ?? ?? ?? 48 8B 4F 50 48 8B 5C 24 30 48 83 C1 18 48 8B 11 48 85 D2 74 ?? 48 85 D2 8B C6 0F 45 C6 48 39 31 0F 45 C6 48 8B 74 24 40 48 83 C4 20 5F C3 4C 8D 44 24 38 48 8B D0 48 8B CF E8 ?? ?? ?? ?? 48 8B 4F 50 48 83 C1 18 44 0F B6 C0 48 8B 11 48 85 D2 75 ?? 8B 44 24 38 45 85 C0 41 B9 FF FF FF FF 41 0F 44 C1 48 85 D2 0F 45 C6 48 3B D6 0F 45 C6 48 8B 74 24 40 48 83 C4 20 5F C3 48 8B 74 24 40 B8 D0 07 00 00 48 83 C4 20 5F C3 CC 4D 85 C0 75 ??";
        private const string ConditionInventoryLayoutPattern = "48 8B 82 98 01 00 00 C3";
        private const string InventoryOwnerLayoutPattern = "48 8B 82 88 00 00 00 C3 CC CC CC CC CC CC CC CC";
        private const string DifficultyLayoutPattern = "48 83 EC 28 48 8B 82 A0 01 00 00 48 85 C0 75 ?? 45 33 C0 8D 50 38 E8 ?? ?? ?? ?? 33 C0 48 83 C4 28 C3 8B 40 20 48 83 C4 28 C3";
        private const string WeaponLayoutPattern = "48 85 C0 74 ?? 48 8B 40 18 48 85 C0 74 ?? 48 8B 48 10 48 85 C9 74 ?? 8B 41 14 48 83 C4 20 5B C3 B8 FF FF FF FF 48 83 C4 20 5B C3";
        private const string LoadedAmmoLayoutPattern = "48 8B 80 A0 00 00 00 48 85 C0 74 ?? 48 8B 48 18 48 85 C9 74 ?? 48 8B 41 10 48 85 C0 74 ?? 8B 40 20 48 8B 5C 24 30 48 83 C4 20 5F C3";
        private const string ReserveBulletLayoutPattern = "48 8B 48 18 4C 89 74 24 58 48 85 C9 74 ?? 48 8B 41 10 48 85 C0 74 ?? 44 8B 40 1C EB ?? 44 8B C7 48 8B D6 48 8B CB E8 ?? ?? ?? ??";
        private const string InventorySlotsLayoutPattern = "49 8B 9E 98 00 00 00 48 85 DB 74 ?? 4C 8B 0D ?? ?? ?? ?? 4C 8B C7 48 8B 15 ?? ?? ?? ?? 48 8B CE C6 44 24 20 00 E8 ?? ?? ?? ?? 48 8B CE 48 85 DB 75 ?? 45 33 C0 8D 53 38 E8 ?? ?? ?? ?? 33 C0 EB ??";
        private const string SlotListLayoutPattern = "8B 4B 08 48 8B E8 85 C9 79 ?? 48 89 43 10 EB ?? 48 3B 6B 10 74 ?? 48 85 ED 74 ?? 48 8B CD E8 ?? ?? ?? ?? 48 8B 4B 10 48 8B C1 F0 48 0F B1 6B 10 74 ?? 0F 0D 4B 10 0F 1F 84 00 00 00 00 00 48 8B 4B 10 48 8B C1 F0 48 0F B1 6B 10 75 ?? 48 85 C9 74 ?? E8 ?? ?? ?? ?? 48 8B 6C 24 48 C7 43 18 00 00 00 00";
        private const string AmmoItemMatchLayoutPattern = "49 8B 40 18 48 85 C0 74 ?? 48 8B 48 10 48 85 C9 74 ?? 8B 41 10 EB ?? 33 C0 48 85 D2 75 ?? 41 3B 41 10 0F 94 C0 48 83 C4 28 C3";
        private const string AmmoStackCountLayoutPattern = "49 8B 40 18 48 85 C0 74 ?? 48 8B 48 10 48 85 C9 74 ?? 8B 41 20 EB ?? 33 C0 48 85 D2 75 ?? 41 03 C2 41 89 41 10 C3";
        private const string SurplusCountLayoutPattern = "49 8B 40 18 48 85 C0 74 ?? 48 8B 48 20 48 8B 05 ?? ?? ?? ?? 48 89 04 24 48 85 C9 74 ?? 8B 41 10 89 44 24 04 C6 04 24 01";
        private const string SurplusMatchLayoutPattern = "48 8B 48 18 48 85 C9 74 ?? 48 8B 41 10 48 85 C0 74 ?? 8B 78 1C 3B D7 75 ?? 8B 7D 10 48 8B D6 48 8B CB E8 ?? ?? ?? ?? 48 8B 4B 50 48 83 79 18 00 75 ?? 03 C7 89 45 10";
        private const string ForceHealthStatusLayoutPattern = "40 38 77 6E 74 ?? 85 D2 7E ?? B8 02 00 00 00 48 8B 5C 24 30 48 8B 74 24 38 48 83 C4 20 5F C3 40 38 77 6D 74 ?? 85 D2 7E ?? B8 01 00 00 00 48 8B 5C 24 30 48 8B 74 24 38 48 83 C4 20 5F C3";
        private const string DangerThresholdLayoutPattern = "48 8B 86 E0 00 00 00 48 85 C0 75 ?? 45 33 C0 48 8B CB 41 8D 50 38 E8 ?? ?? ?? ?? 8B CF EB ?? 48 8B 48 20 48 85 C9 74 ?? 8B 49 18 EB ?? B9 68 01 00 00 48 8B 43 50 48 39 78 18 75 ?? 3B E9 7E ??";
        private const string CautionThresholdLayoutPattern = "85 C9 74 ?? 48 8B 86 E0 00 00 00 48 85 C0 75 ?? 45 33 C0 48 8B CB 41 8D 50 38 E8 ?? ?? ?? ?? EB ?? 48 8B 40 20 48 85 C0 74 ?? 8B 78 14 EB ?? BF 84 03 00 00 48 8B 43 50 48 83 78 18 00 0F 85 ?? ?? ?? ?? 3B EF 0F 8F ?? ?? ?? ?? B8 01 00 00 00 E9 ?? ?? ?? ?? B8 02 00 00 00";
        private const string SlotInfiniteLayoutPattern = "80 7A 24 00 0F 95 C0 C3";
        private const string ArrayLayoutPattern = "48 8B 15 ?? ?? ?? ?? 48 8B D8 48 8B BA 28 83 01 00 48 85 C0 75 ?? 45 33 C0 8D 50 38 48 8B CE E8 ?? ?? ?? ?? 33 C0 48 8B 5C 24 40 48 8B 74 24 48 48 83 C4 30 5F C3 83 78 1C 00 77 ?? 45 33 C0 48 8B CE 41 8D 50 37 E8 ?? ?? ?? ?? 33 C0 48 8B 5C 24 40 48 8B 74 24 48 48 83 C4 30 5F C3 8B 40 08 85 C0 79 ?? 48 89 7B 20 EB ??";
        private const string ConfigPathLayoutPattern = "48 85 C9 74 ?? 48 8B 41 10 48 85 C0 74 ?? 48 83 C0 08 EB ?? 48 8D 05 ?? ?? ?? ?? 83 78 1C 0C 72 ?? 4C 8B 00 EB ?? 4C 8B C0 8B 50 18 45 33 C9 48 8B CB E8 ?? ?? ?? ?? 33 FF";
        private const string FlowStateLayoutPattern = "8B AD E0 00 00 00 48 8B DF BF 23 00 00 00 80 7C 24 40 00";
        private const string StayRegionLayoutPattern = "48 85 C9 0F 84 ?? ?? ?? ?? 8B 41 54 EB ?? 8B C7 89 05 ?? ?? ?? ??";
        private const string StayRoomLayoutPattern = "48 8B 8E D0 01 00 00 48 85 C9 74 ?? 48 39 79 10 74 ?? 32 C0 EB ?? 8B 49 58 EB ??";
        private const string PurposePattern = "48 8B 15 ?? ?? ?? ?? 48 8B CB 48 85 D2 75 ?? 45 33 C0 41 8D 50 38 E8 ?? ?? ?? ?? 33 C0 48 8B 5C 24 40 48 83 C4 30 5F C3 E8 ?? ?? ?? ??";
        private const string PurposeListLayoutPattern = "48 8B 47 50 48 8B 55 68 48 83 78 18 00 75 ?? 48 85 D2 75 ?? 45 33 C0 48 8B CF 41 8D 50 38 E8 ?? ?? ?? ??";
        private const string PurposeTitleLayoutPattern = "48 8B 55 68 45 33 C0 48 8B CF 44 39 40 1C 0F 8C ?? ?? ?? ?? 48 85 D2 0F 84 ?? ?? ?? ?? E8 ?? ?? ?? ?? 48 8B 4F 50 48 83 79 18 00 0F 85 ?? ?? ?? ?? 48 85 C0 74 ?? 48 8B 70 10 48 85 F6 74 ?? 48 8B 55 68 45 33 C0 48 8B 76 38";
        private const string PurposeDefaultGuidLayoutPattern = "48 8B 40 10 48 85 C0 0F 84 ?? ?? ?? ?? 44 8B 40 10 44 0F B7 48 14 44 0F B7 50 16 48 8B 48 18";
        private const string FlowPattern = "48 8B 15 ?? ?? ?? ?? 48 8B CB 48 85 D2 0F 84 ?? ?? ?? ?? E8 ?? ?? ?? ?? 0F B6 C8 48 8B 43 50 4C 39 70 18 0F 85 ?? ?? ?? ?? 85 C9 0F 85 ?? ?? ?? ?? 48 8B 15 ?? ?? ?? ??";
        private const string StayAreaLayoutPattern = "48 8B 96 D0 01 00 00 48 85 D2 74 ?? 48 39 7A 10 74 ?? 32 C0 EB ?? 8B 49 5C EB ??";
        private static DiscordRpcClient? discord;
        private static DiscordStatusUpdater? updater;
        private static int cachedProcessId, cachedModuleSize;
        private static ulong cachedModuleBase, playerSlot, flowSlot, objectiveSlot;
        private static DateTime nextScan;
        public static void DoAction()
        {
            ResetAddresses();
            GetPID();
            discord = new DiscordRpcClient("1467486061952372879");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/config/Resident Evil 3 Remake.json");
            PresenceRuntime.Start(nameof(RE3R), "re3", RPC);
        }

        private static void GetPID()
        {
            ProcessMonitor.TryAttach("re3");
        }

        private static async Task RPC()
        {
            if (discord == null || updater == null)
            {
                throw new InvalidOperationException("RE3R: presence is not initialized.");
            }

            if (ProcessMonitor.IsRunning("re3"))
            {
                try
                {
                    if (!Hypervisor.IsAttached && !ProcessMonitor.TryAttach("re3"))
                    {
                        return;
                    }

                    var snapshot = ReadSnapshot();
                    if (!snapshot.IsInGame)
                    {
                        ShowStatus("In Main Menu / Loading");
                        return;
                    }

                    var placeholders = await PlaceholderHelper.GetPlaceholders(() => GeneratePlaceholders(snapshot));
                    PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Resident Evil 3", placeholders);
                }
                catch (Exception exception) when (exception is not OutOfMemoryException and not StackOverflowException)
                {
                    AppLog.Warning("Resident Evil 3 memory is not ready.", exception);
                    ShowStatus("Waiting for game data");
                }
            }
            else
            {
                ResetAddresses();
                discord.Deinitialize();
                updater.Dispose();
                PresenceRuntime.RequestDetection();
            }
        }

        private static async Task<Dictionary<string, object>> GeneratePlaceholders(Snapshot snapshot)
        {
            if (!snapshot.IsInGame)
            {
                return new Dictionary<string, object>();
            }

            var region = Locations.GetRegion(snapshot.RegionId, snapshot.RoomId, snapshot.AreaId);
            var room = Locations.GetRoom(snapshot.RoomId, snapshot.AreaId);
            var location = Locations.GetLocation(snapshot.RegionId, snapshot.RoomId, snapshot.AreaId);
            if (location.Length == 0)
            {
                location = $"Unknown area ({snapshot.RegionId}/{snapshot.RoomId}/{snapshot.AreaId})";
            }

            var percentage = (double)snapshot.Health / snapshot.MaxHealth * 100;
            return new Dictionary<string, object>
            {
                { "character", Characters.GetName(snapshot.CharacterId) },
                { "characterid", snapshot.CharacterId },
                { "difficulty", Difficulties.GetName(snapshot.DifficultyId) },
                { "difficultyid", snapshot.DifficultyId },
                { "region", region },
                { "regionid", snapshot.RegionId },
                { "room", room },
                { "roomid", snapshot.RoomId },
                { "areaid", snapshot.AreaId },
                { "mapid", Locations.ResolveMapId(snapshot.RoomId, snapshot.AreaId) },
                { "location", location },
                { "objective", snapshot.Objective },
                { "health", snapshot.Health },
                { "maxhealth", snapshot.MaxHealth },
                { "healthstatus", snapshot.HealthStatus },
                { "healthpercentage", percentage },
                { "healthstatusemoji", snapshot.HealthStatus switch { "Fine" => "🟢", "Caution" => "🟡", _ => "🔴", } },
                { "weapon", snapshot.WeaponId < 0 ? "None" : await Weapons.GetWeapon(snapshot.WeaponId) },
                { "weaponid", snapshot.WeaponId },
                { "ammo", snapshot.InfiniteAmmo ? "∞" : snapshot.Ammo },
                { "loadedammo", snapshot.Ammo },
                { "infiniteammo", snapshot.InfiniteAmmo },
                { "reserveammo", snapshot.ReserveAmmo < 0 ? "" : snapshot.ReserveAmmo },
            };
        }

        private static void ShowStatus(string text)
        {
            if (discord == null)
            {
                throw new InvalidOperationException("RE3R: presence is not initialized.");
            }

            discord.SetPresence(new RichPresence()
            {
                Details = text,
                State = "",
                Assets = new Assets()
                {
                    LargeImageKey = "logo",
                    LargeImageText = "Resident Evil 3"
                },
                Timestamps = PlaceholderHelper._startTimestamp
            });
        }

        private sealed record Snapshot(bool IsInGame, int CharacterId, int DifficultyId, int RegionId, int RoomId, int AreaId, int Health, int MaxHealth, string HealthStatus, int WeaponId, int Ammo, int ReserveAmmo, bool InfiniteAmmo, string Objective);
        private static Snapshot EmptySnapshot()
        {
            return new Snapshot(false, -1, -1, -1, -1, -1, 0, 0, "", -1, 0, -1, false, "");
        }

        private static Snapshot ReadSnapshot()
        {
            ResolveAddresses();
            var flow = Pointer(flowSlot);
            if (flow == 0)
            {
                return EmptySnapshot();
            }

            var state = ReadInt(flow + 0xE0);
            if (state is not (6 or 7 or 9))
            {
                return EmptySnapshot();
            }

            var header = Pointer(flow + 0x1A0);
            var owner = Pointer(playerSlot);
            var player = owner == 0 ? 0 : Pointer(owner + 0x50);
            if (header == 0 || !NativeObject(player))
            {
                return EmptySnapshot();
            }

            var hp = Pointer(player + 0x2C0);
            var stay = Pointer(player + 0x1D0);
            if (!NativeObject(hp) || !NativeObject(stay))
            {
                return EmptySnapshot();
            }

            var health = ReadInt(hp + 0x58);
            var maxHealth = ReadInt(hp + 0x54);
            if (maxHealth <= 0 || maxHealth > 10000000 || health < 0 || health > maxHealth)
            {
                throw new InvalidDataException("RE3: invalid hit-point controller.");
            }

            var difficulty = ReadInt(header + 0x20);
            if (difficulty < 0 || difficulty > 4)
            {
                throw new InvalidDataException("RE3: unsupported difficulty.");
            }

            var equipment = ReadEquipment(player);
            var objective = ReadObjective();
            var status = ReadHealthStatus(owner, player, health);
            var snapshot = new Snapshot(true, ReadInt(player + 0x54), difficulty, ReadInt(stay + 0x54), ReadInt(stay + 0x58), ReadInt(stay + 0x5C), health, maxHealth, status, equipment.WeaponId, equipment.Ammo, equipment.Reserve, equipment.Infinite, objective);
            if (flow != Pointer(flowSlot) || state != ReadInt(flow + 0xE0) || header != Pointer(flow + 0x1A0) || difficulty != ReadInt(header + 0x20) || owner != Pointer(playerSlot) || player != Pointer(owner + 0x50) || !NativeObject(player) || !NativeObject(hp) || !NativeObject(stay) || hp != Pointer(player + 0x2C0) || stay != Pointer(player + 0x1D0) || snapshot.CharacterId != ReadInt(player + 0x54) || snapshot.RegionId != ReadInt(stay + 0x54) || snapshot.RoomId != ReadInt(stay + 0x58) || snapshot.AreaId != ReadInt(stay + 0x5C) || health != ReadInt(hp + 0x58) || maxHealth != ReadInt(hp + 0x54) || status != ReadHealthStatus(owner, player, health) || equipment != ReadEquipment(player) || objective != ReadObjective())
            {
                throw new InvalidDataException("RE3: scene changed during the read; retry next poll.");
            }

            return snapshot;
        }

        private static string ReadHealthStatus(ulong owner, ulong player, int health)
        {
            if (health <= 0)
            {
                return "Dead";
            }

            if (ReadByte(player + 0x6E) != 0)
            {
                return "Danger";
            }

            if (ReadByte(player + 0x6D) != 0)
            {
                return "Caution";
            }

            var caution = 900;
            var danger = 360;
            var config = Pointer(owner + 0xE0);
            var resource = config == 0 ? 0 : Pointer(config + 0x10);
            var pathLength = resource == 0 ? 0 : ReadInt(resource + 0x20);
            if (pathLength < 0 || pathLength > 4096)
            {
                throw new InvalidDataException("RE3: invalid health resource path.");
            }

            var parameter = pathLength > 0 ? Pointer(config + 0x20) : 0;
            if (parameter != 0)
            {
                caution = ReadInt(parameter + 0x14);
                danger = ReadInt(parameter + 0x18);
                if (danger < 0 || caution < danger || caution > 10000000)
                {
                    throw new InvalidDataException("RE3: invalid health thresholds.");
                }
            }

            return health <= danger ? "Danger" : health <= caution ? "Caution" : "Fine";
        }

        private readonly record struct Equipment(ulong Inventory, ulong Slot, ulong Item, ulong Primitive, int WeaponId, int BulletId, int Ammo, int Reserve, bool Infinite);
        private static Equipment ReadEquipment(ulong player)
        {
            var inventory = Pointer(player + 0x198);
            if (inventory == 0)
            {
                return new Equipment(0, 0, 0, 0, -1, 0, 0, 0, false);
            }

            if (Pointer(inventory + 0x88) != player)
            {
                throw new InvalidDataException("RE3: inventory belongs to another player.");
            }

            var slot = Pointer(inventory + 0xA0);
            var item = slot == 0 ? 0 : Pointer(slot + 0x18);
            var primitive = item == 0 ? 0 : Pointer(item + 0x10);
            if (primitive == 0)
            {
                return new Equipment(inventory, slot, item, 0, -1, 0, 0, 0, false);
            }

            var weaponId = ReadInt(primitive + 0x14);
            var bulletId = ReadInt(primitive + 0x1C);
            var ammo = ReadInt(primitive + 0x20);
            if (weaponId == -1)
            {
                return new Equipment(inventory, slot, item, primitive, -1, 0, 0, 0, false);
            }

            var infinite = ReadByte(slot + 0x24) != 0;
            if (ammo < 0 && bulletId <= 0)
            {
                ammo = 0;
            }

            if (weaponId < -1 || weaponId > 100000 || ammo < 0 || ammo > 10000000)
            {
                throw new InvalidDataException("RE3: invalid equipped weapon.");
            }

            var reserve = 0;
            if (bulletId > 0)
            {
                var list = Pointer(inventory + 0x98);
                if (list != 0)
                {
                    var array = Pointer(list + 0x10);
                    var count = ReadInt(list + 0x18);
                    var capacity = array == 0 ? 0 : ReadInt(array + 0x1C);
                    if (count < 0 || count > capacity || capacity > 256 || (count > 0 && array == 0))
                    {
                        throw new InvalidDataException("RE3: invalid inventory slots.");
                    }

                    for (var index = 0; index < count; index++)
                    {
                        var entry = Pointer(array + 0x20 + (ulong)index * 8);
                        var stackItem = entry == 0 ? 0 : Pointer(entry + 0x18);
                        var stack = stackItem == 0 ? 0 : Pointer(stackItem + 0x10);
                        if (stack == 0 || ReadInt(stack + 0x10) != bulletId)
                        {
                            continue;
                        }

                        var amount = ReadInt(stack + 0x20);
                        if (amount < 0 || amount > 10000000)
                        {
                            throw new InvalidDataException("RE3: invalid ammo stack.");
                        }

                        reserve = checked(reserve + amount);
                    }

                    if (list != Pointer(inventory + 0x98) || array != Pointer(list + 0x10) || count != ReadInt(list + 0x18) || (array != 0 && capacity != ReadInt(array + 0x1C)))
                    {
                        throw new InvalidDataException("RE3: inventory changed during the read.");
                    }
                }

                var surplus = Pointer(item + 0x20);
                if (surplus != 0 && ReadInt(surplus + 0x14) == bulletId)
                {
                    var amount = ReadInt(surplus + 0x10);
                    if (amount < 0 || amount > 10000000)
                    {
                        throw new InvalidDataException("RE3: invalid ammo surplus.");
                    }

                    reserve = checked(reserve + amount);
                }
            }

            return new Equipment(inventory, slot, item, primitive, weaponId, bulletId, ammo, reserve, infinite);
        }

        private static Guid[] ReadObjectiveIds()
        {
            var manager = Pointer(objectiveSlot);
            if (manager == 0)
            {
                return[];
            }

            var list = Pointer(manager + 0x68);
            if (list == 0)
            {
                return[];
            }

            var checks = new List<Action>();
            void CheckPointer(ulong address, ulong expected)
            {
                checks.Add(() =>
                {
                    if (Pointer(address) != expected)
                    {
                        throw new InvalidDataException("RE3: objective changed during read.");
                    }
                });
            }

            void CheckInt(ulong address, int expected)
            {
                checks.Add(() =>
                {
                    if (ReadInt(address) != expected)
                    {
                        throw new InvalidDataException("RE3: objective changed during read.");
                    }
                });
            }

            (ulong Items, int Count) ReadList(ulong owner, int maximum)
            {
                var items = Pointer(owner + 0x10);
                var count = ReadInt(owner + 0x18);
                var capacity = items == 0 ? 0 : ReadInt(items + 0x1C);
                if (count < 0 || count > maximum || capacity < count || capacity > 4096 || count > 0 && items == 0)
                {
                    throw new InvalidDataException("RE3: invalid objective list.");
                }

                CheckPointer(owner + 0x10, items);
                CheckInt(owner + 0x18, count);
                if (items != 0)
                {
                    CheckInt(items + 0x1C, capacity);
                }

                return (items, count);
            }

            CheckPointer(objectiveSlot, manager);
            CheckPointer(manager + 0x68, list);
            var(items, count) = ReadList(list, 128);
            var result = new List<Guid>();
            for (var index = 0; index < count; index++)
            {
                var entryAddress = items + 0x20 + (ulong)index * 8;
                var current = Pointer(entryAddress);
                if (current == 0)
                {
                    throw new InvalidDataException("RE3: missing current objective.");
                }

                CheckPointer(entryAddress, current);
                var data = Pointer(current + 0x10);
                var titleIndex = ReadInt(current + 0x1C);
                if (data == 0)
                {
                    throw new InvalidDataException("RE3: missing objective data.");
                }

                CheckPointer(current + 0x10, data);
                CheckInt(current + 0x1C, titleIndex);
                var guidAddress = data + 0x10;
                if (titleIndex >= 0)
                {
                    var variants = Pointer(data + 0x38);
                    if (variants == 0)
                    {
                        throw new InvalidDataException("RE3: missing objective title variants.");
                    }

                    CheckPointer(data + 0x38, variants);
                    var(variantItems, variantCount) = ReadList(variants, 128);
                    if (titleIndex >= variantCount)
                    {
                        throw new InvalidDataException("RE3: invalid objective title index.");
                    }

                    var variantAddress = variantItems + 0x20 + (ulong)titleIndex * 8;
                    var variant = Pointer(variantAddress);
                    if (variant == 0)
                    {
                        throw new InvalidDataException("RE3: missing objective title variant.");
                    }

                    CheckPointer(variantAddress, variant);
                    guidAddress = variant + 0x10;
                }

                var bytes = Hypervisor.ReadArray(guidAddress, 16, true);
                var address = guidAddress;
                checks.Add(() =>
                {
                    if (!bytes.AsSpan().SequenceEqual(Hypervisor.ReadArray(address, 16, true)))
                    {
                        throw new InvalidDataException("RE3: objective text changed during read.");
                    }
                });
                var id = new Guid(bytes);
                if (id != Guid.Empty && !result.Contains(id))
                {
                    result.Add(id);
                }
            }

            foreach (var check in checks)
            {
                check();
            }

            return result.ToArray();
        }

        private static string ReadObjective()
        {
            return GameText.Get(ReadObjectiveIds().FirstOrDefault());
        }

        private static int ReadInt(ulong address)
        {
            return Hypervisor.Read<int>(address, true);
        }

        private static ulong Pointer(ulong address)
        {
            return Hypervisor.Read<ulong>(address, true);
        }

        private static byte ReadByte(ulong address)
        {
            return Hypervisor.Read<byte>(address, true);
        }

        private static ulong Follow(ulong slot, params uint[] offsets)
        {
            var pointer = Pointer(slot);
            foreach (var offset in offsets)
            {
                if (pointer == 0)
                {
                    return 0;
                }

                pointer = Pointer(pointer + offset);
            }

            return pointer;
        }

        private static bool NativeObject(ulong address)
        {
            return address != 0 && Pointer(address + 0x10) != 0;
        }

        private static void ResetAddresses()
        {
            cachedProcessId = cachedModuleSize = 0;
            cachedModuleBase = playerSlot = flowSlot = objectiveSlot = 0;
            nextScan = DateTime.MinValue;
        }

        private static void ResolveAddresses()
        {
            var process = Hypervisor.Process ?? throw new InvalidDataException("RE3R: game is not attached.");
            var module = process.MainModule ?? throw new InvalidDataException("RE3: module not loaded.");
            var moduleBase = (ulong)module.BaseAddress.ToInt64();
            if (cachedProcessId != process.Id || cachedModuleBase != moduleBase || cachedModuleSize != module.ModuleMemorySize)
            {
                ResetAddresses();
                cachedProcessId = process.Id;
                cachedModuleBase = moduleBase;
                cachedModuleSize = module.ModuleMemorySize;
            }

            if (flowSlot != 0)
            {
                return;
            }

            if (DateTime.UtcNow < nextScan)
            {
                throw new InvalidDataException("RE3: waiting to retry patterns.");
            }

            nextScan = DateTime.UtcNow.AddSeconds(30);
            ulong Resolve(string pattern, int operand, int length)
            {
                var match = FindUniquePattern(moduleBase, module.ModuleMemorySize, pattern);
                var target = checked((ulong)((long)match + length + ReadInt(match + (ulong)operand)));
                if (target < moduleBase || target + 8 > moduleBase + (ulong)module.ModuleMemorySize)
                {
                    throw new InvalidDataException("RE3: RIP-relative target is outside the module.");
                }

                return target;
            }

            var playerSlotValue = Resolve(PlayerConditionSlotPattern, 3, 7);
            FindUniquePattern(moduleBase, module.ModuleMemorySize, ConditionHealthLayoutPattern);
            FindUniquePattern(moduleBase, module.ModuleMemorySize, HealthValueLayoutPattern);
            FindUniquePattern(moduleBase, module.ModuleMemorySize, CharacterLayoutPattern);
            FindUniquePattern(moduleBase, module.ModuleMemorySize, ConditionInventoryLayoutPattern);
            FindUniquePattern(moduleBase, module.ModuleMemorySize, InventoryOwnerLayoutPattern);
            FindUniquePattern(moduleBase, module.ModuleMemorySize, DifficultyLayoutPattern);
            FindUniquePattern(moduleBase, module.ModuleMemorySize, WeaponLayoutPattern);
            FindUniquePattern(moduleBase, module.ModuleMemorySize, LoadedAmmoLayoutPattern);
            FindUniquePattern(moduleBase, module.ModuleMemorySize, ReserveBulletLayoutPattern);
            FindUniquePattern(moduleBase, module.ModuleMemorySize, InventorySlotsLayoutPattern);
            FindUniquePattern(moduleBase, module.ModuleMemorySize, SlotListLayoutPattern);
            FindUniquePattern(moduleBase, module.ModuleMemorySize, AmmoItemMatchLayoutPattern);
            FindUniquePattern(moduleBase, module.ModuleMemorySize, AmmoStackCountLayoutPattern);
            FindUniquePattern(moduleBase, module.ModuleMemorySize, SurplusCountLayoutPattern);
            FindUniquePattern(moduleBase, module.ModuleMemorySize, SurplusMatchLayoutPattern);
            FindUniquePattern(moduleBase, module.ModuleMemorySize, ForceHealthStatusLayoutPattern);
            FindUniquePattern(moduleBase, module.ModuleMemorySize, DangerThresholdLayoutPattern);
            FindUniquePattern(moduleBase, module.ModuleMemorySize, CautionThresholdLayoutPattern);
            FindUniquePattern(moduleBase, module.ModuleMemorySize, SlotInfiniteLayoutPattern);
            FindUniquePattern(moduleBase, module.ModuleMemorySize, ArrayLayoutPattern);
            FindUniquePattern(moduleBase, module.ModuleMemorySize, ConfigPathLayoutPattern);
            FindUniquePattern(moduleBase, module.ModuleMemorySize, FlowStateLayoutPattern);
            FindUniquePattern(moduleBase, module.ModuleMemorySize, StayRegionLayoutPattern);
            FindUniquePattern(moduleBase, module.ModuleMemorySize, StayRoomLayoutPattern);
            var objectiveSlotValue = Resolve(PurposePattern, 3, 7);
            FindUniquePattern(moduleBase, module.ModuleMemorySize, PurposeListLayoutPattern);
            FindUniquePattern(moduleBase, module.ModuleMemorySize, PurposeTitleLayoutPattern);
            FindUniquePattern(moduleBase, module.ModuleMemorySize, PurposeDefaultGuidLayoutPattern);
            var flowSlotValue = Resolve(FlowPattern, 3, 7);
            FindUniquePattern(moduleBase, module.ModuleMemorySize, StayAreaLayoutPattern);
            playerSlot = playerSlotValue;
            flowSlot = flowSlotValue;
            objectiveSlot = objectiveSlotValue;
        }

        private static ulong FindUniquePattern(ulong moduleBase, int moduleSize, string signature)
        {
            if (moduleSize < 64)
            {
                throw new InvalidDataException("RE3: invalid module size.");
            }

            var peOffset = Hypervisor.Read<uint>(moduleBase + 0x3C, true);
            if (peOffset > (uint)moduleSize - 24)
            {
                throw new InvalidDataException("RE3: invalid PE offset.");
            }

            var pe = moduleBase + peOffset;
            if (peOffset > (uint)moduleSize - 24 || Hypervisor.Read<uint>(pe, true) != 0x4550 || Hypervisor.Read<ushort>(pe + 4, true) != 0x8664)
            {
                throw new InvalidDataException("RE3: invalid PE header.");
            }

            var sectionCount = Hypervisor.Read<ushort>(pe + 6, true);
            var optionalSize = Hypervisor.Read<ushort>(pe + 20, true);
            var sectionTable = pe + 24 + optionalSize;
            if (sectionTable + (ulong)sectionCount * 40 > moduleBase + (ulong)moduleSize)
            {
                throw new InvalidDataException("RE3: invalid section table.");
            }

            var pattern = SignaturePattern.Parse(signature);
            var prefix = pattern.Bytes.TakeWhile(value => value.HasValue).Select(value => value.GetValueOrDefault()).ToArray();
            if (prefix.Length == 0)
            {
                throw new InvalidDataException("RE3: pattern needs a fixed prefix.");
            }

            const int chunkSize = 1024 * 1024;
            ulong match = 0;
            for (var section = 0; section < sectionCount; section++)
            {
                var header = sectionTable + (ulong)section * 40;
                var size = Hypervisor.Read<uint>(header + 8, true);
                var rva = Hypervisor.Read<uint>(header + 12, true);
                var flags = Hypervisor.Read<uint>(header + 36, true);
                if ((flags & 0x20000000) == 0 || size < pattern.Length)
                {
                    continue;
                }

                if ((ulong)rva + size > (ulong)moduleSize)
                {
                    throw new InvalidDataException("RE3: invalid executable section.");
                }

                for (ulong offset = 0; offset < size; offset += chunkSize)
                {
                    var count = (int)Math.Min((ulong)(chunkSize + pattern.Length - 1), size - offset);
                    var bytes = Hypervisor.ReadArray(moduleBase + rva + offset, count, true);
                    var start = 0;
                    while (start <= bytes.Length - pattern.Length)
                    {
                        var found = bytes.AsSpan(start).IndexOf(prefix);
                        if (found < 0)
                        {
                            break;
                        }

                        var candidate = start + found;
                        start = candidate + 1;
                        if (candidate + pattern.Length > bytes.Length)
                        {
                            break;
                        }

                        var matches = true;
                        for (var part = 0; part < pattern.Length; part++)
                        {
                            if (pattern.Bytes[part] is byte expected && bytes[candidate + part] != expected)
                            {
                                matches = false;
                                break;
                            }
                        }

                        if (!matches)
                        {
                            continue;
                        }

                        var address = moduleBase + rva + offset + (ulong)candidate;
                        if (match != 0 && match != address)
                        {
                            throw new InvalidDataException("RE3: manager pattern is ambiguous.");
                        }

                        match = address;
                    }
                }
            }

            return match != 0 ? match : throw new InvalidDataException("RE3: manager pattern was not found.");
        }

        private static void InitializeDiscord()
        {
            if (discord == null)
            {
                throw new InvalidOperationException("RE3R: presence is not initialized.");
            }

            discord.Initialize();
        }
    }
}
