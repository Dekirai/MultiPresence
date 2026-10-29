using MultiPresence.Runtime;
using MultiPresence.Infrastructure;
using DiscordRPC;
using MultiPresence.Models.RE9;
using System.Globalization;
using System.Text;

namespace MultiPresence.Presence
{
    public class RE9
    {
        private const string CharacterPattern = "48 8B 05 ?? ?? ?? ?? 48 85 C0 0F 84 ?? ?? ?? ?? 48 8B 98 B0 00 00 00 48 89 F1 48 89 DA 45 31 C0 E8 ?? ?? ?? ??";
        private const string DifficultyPattern = "48 8B 05 ?? ?? ?? ?? 48 85 C0 74 ?? 48 8B 0D ?? ?? ?? ?? 48 85 C9 74 ?? 80 79 36 00 48 8D 0D ?? ?? ?? ?? 48 0F 44 C8 48 8B 59 10 48 89 F1";
        private const string InventoryPattern = "4C 8B 35 ?? ?? ?? ?? 4D 85 F6 74 ?? 4C 8B 40 20 48 8B 05 ?? ?? ?? ?? B9 00 00 00 80 48 8D 54 24 30 48 89 4A 08";
        private const string ObjectivePattern = "48 8B 0D ?? ?? ?? ?? 48 85 C9 0F 84 83 00 00 00 4C 8B 40 10 C5 F8 57 C0 C5 F8 29 44 24 20 48 8B 51 18 48 85 D2 0F 84 93 00 00 00 48 8D 5C 24 20 48 89";
        private const string GuiMapPattern = "48 8B 15 ?? ?? ?? ?? 48 85 D2 0F 84 DB 02 00 00 4C 8B 05 ?? ?? ?? ?? 48 89 F1 45 31 C9 E8 ?? ?? ?? ??";
        private const string MainGameFlowIsMainGamePattern = "48 8B 42 20 48 85 C0 74 ?? 48 83 78 10 00 0F 95 C2 48 8B 41 50 48 83 78 18 00 0F 94 C0 20 D0 C3 31 D2 EB ??";
        private const string CharacterContextHitPointPattern = "48 89 CE 48 8B 52 70 48 85 D2 74 ?? 48 8B 42 10 48 85 C0 74 ?? 80 78 34 00 74 ??";
        private const string HitPointCurrentAndMaximumPattern = "48 8B 42 10 48 85 C0 74 ?? 80 78 34 00 74 ?? 8B 70 28 EB ?? 31 F6 BA 38 00 00 00 45 31 C0 E8 ?? ?? ?? ?? EB ?? CC 56 48 83 EC 20 48 8B 41 50 31 F6 48 83 78 18 00 74 ?? 89 F0 48 83 C4 20 5E C3 48 8B 42 10 48 85 C0 74 ?? 80 78 34 00 74 ?? 8B 70 30 EB ??";
        private const string FlowPhasePattern = "44 89 C7 48 89 D6 83 BA F0 00 00 00 05 74 ?? 89 BE F0 00 00 00 C7 86 F8 00 00 00 00 00 00 00";
        private const string GuiManagerMapAndCurrentFloorPattern = "48 39 5F 48 0F 85 ?? ?? ?? ?? 48 8B 46 50 48 83 78 18 00 75 ?? 48 8B 97 78 01 00 00 48 85 D2 0F 84 ?? ?? ?? ??";
        private const string GuiMapBridgeStaySectionPattern = "4D 89 CE 4C 89 C6 48 89 D7 48 89 CB 4C 8B BA A0 00 00 00 4D 85 C9 0F 88 ?? ?? ?? ?? 4D 89 3E 48 8B 97 A0 00 00 00";
        private const string MainGameFlowCurrentScenePattern = "48 39 BB 80 00 00 00 74 ?? 48 85 FF 74 ?? 48 89 F9 E8 ?? ?? ?? ?? 48 8B 8B 80 00 00 00 48 89 C8 F0 48 0F B1 BB 80 00 00 00 75 ?? 48 85 C9 74 ?? E8 ?? ?? ?? ?? C7 44 24 24 00 00 00 00 48 8B 53 58";
        private const string InventoryLayoutPattern = "56 57 53 48 83 EC 30 48 89 D7 48 89 CE 48 8B 02 48 89 D1 FF 50 68 48 8B 46 50 48 83 78 18 00 74 08 48 83 C4 30 5B 5F 5E C3 48 8B 1D ?? ?? ?? ??";
        private const string EquipmentLayoutPattern = "48 8B 49 50 31 C0 48 83 79 18 00 75 39 48 8B 4A 10 48 85 C9 74 30 48 83 79 10 00 74 2A 48 8B 81 10 03 00 00 48 85 C0 74 1E 48 8B 08 48 8B 09 48 3B 0D ?? ?? ?? ?? 75 0F 48 83 78 10 00 74 08 48 8B 80 60 01 00 00 C3 31 C0 C3";
        private const string InventoryUserPattern = "48 63 44 24 2C 83 F8 03 77 0D 48 8D 0D ?? ?? ?? ?? 48 8B 04 C1";
        private static DiscordRpcClient? discord;
        private static DiscordStatusUpdater? updater;
        private static int cachedProcessId, cachedModuleSize;
        private static ulong cachedModuleBase, characterSlot, difficultySlot, flowSlot, inventorySlot, guiMapSlot, objectiveSlot, difficultyOverride;
        private static DateTime nextScan;
        private static ulong inventoryUserTable, equipmentTypeSlot;
        public static void DoAction()
        {
            ResetAddresses();
            GetPID();
            discord = new DiscordRpcClient("1479128647133560892");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/Config/Resident Evil Requiem.json");
            PresenceRuntime.Start(nameof(RE9), "re9", RPC);
        }

        private static void GetPID()
        {
            ProcessMonitor.TryAttach("re9");
        }

        private static async Task RPC()
        {
            if (discord == null || updater == null)
            {
                throw new InvalidOperationException("RE9: presence is not initialized.");
            }

            if (ProcessMonitor.IsRunning("re9"))
            {
                try
                {
                    if (!Hypervisor.IsAttached && !ProcessMonitor.TryAttach("re9"))
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
                    PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Resident Evil Requiem", placeholders);
                }
                catch (Exception exception) when (exception is not OutOfMemoryException and not StackOverflowException)
                {
                    AppLog.Warning("Resident Evil Requiem memory is not ready.", exception);
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

        private static Task<Dictionary<string, object>> GeneratePlaceholders(Snapshot snapshot)
        {
            if (!snapshot.IsInGame)
            {
                return Task.FromResult(new Dictionary<string, object> { { "state", "Main Menu" } });
            }

            var character = snapshot.CharacterId switch
            {
                "cp_A000" => "Leon",
                "cp_A100" or "cp_A110" => "Grace",
                "cp_A200" => "Chloe",
                _ => snapshot.CharacterId,
            };
            var difficulty = GetDifficulty(snapshot.DifficultyId);
            var location = Locations.GetMap(snapshot.MapSectionId, snapshot.MapFloorId);
            if (location.Length == 0)
            {
                location = "Unknown location";
            }

            var region = Locations.GetMapArea(snapshot.MapFloorId);
            var room = region.Length > 0 && snapshot.MapSectionId.StartsWith(snapshot.MapFloorId + "_", StringComparison.Ordinal) ? Locations.GetMapRoom(snapshot.MapSectionId) : "";
            var weapon = snapshot.ItemId.Length == 0 ? (snapshot.WeaponId.Length == 0 ? "None" : snapshot.WeaponId) : GameText.GetItemName(snapshot.ItemId);
            var objective = GameText.Get(snapshot.ObjectiveGuid);
            if (objective.Length > 0)
            {
                objective = objective.Replace("{0}", snapshot.ObjectiveCount.ToString(CultureInfo.InvariantCulture)).Replace("{1}", snapshot.ObjectiveMaxCount.ToString(CultureInfo.InvariantCulture));
            }

            var percentage = (double)snapshot.Health / snapshot.MaxHealth * 100;
            return Task.FromResult(new Dictionary<string, object> { { "state", "In Game" }, { "character", character }, { "characterid", snapshot.CharacterId }, { "health", snapshot.Health }, { "maxhealth", snapshot.MaxHealth }, { "healthpercentage", percentage }, { "difficulty", difficulty }, { "difficultyid", snapshot.DifficultyId }, { "location", location }, { "region", region }, { "room", room }, { "mapsectionid", snapshot.MapSectionId }, { "mapfloorid", snapshot.MapFloorId }, { "placeid", snapshot.MapSectionId }, { "scenename", snapshot.SceneName }, { "currentweapon", weapon }, { "weapon", weapon }, { "weaponid", snapshot.WeaponId }, { "itemid", snapshot.ItemId }, { "ammo", snapshot.Ammo }, { "reserveammo", snapshot.ReserveAmmo }, { "money", snapshot.CharacterId == "cp_A000" ? snapshot.Money : "" }, { "moneydisplay", snapshot.CharacterId == "cp_A000" ? $"Credits: {snapshot.Money}" : "" }, { "objective", objective }, { "objectivecount", snapshot.ObjectiveCount }, { "objectivemaxcount", snapshot.ObjectiveMaxCount }, { "objectiveguid", snapshot.ObjectiveGuid.ToString() } });
        }

        private static void ShowStatus(string text)
        {
            if (discord == null)
            {
                throw new InvalidOperationException("RE9: presence is not initialized.");
            }

            discord.SetPresence(new RichPresence()
            {
                Details = text,
                State = "",
                Assets = new Assets()
                {
                    LargeImageKey = "logo",
                    LargeImageText = "Resident Evil Requiem"
                },
                Timestamps = PlaceholderHelper._startTimestamp
            });
        }

        private sealed record Snapshot(bool IsInGame, string CharacterId, int Health, int MaxHealth, string DifficultyId, string MapSectionId, string WeaponId, string ItemId, int Ammo, int ReserveAmmo, int Money, Guid ObjectiveGuid, int ObjectiveCount, int ObjectiveMaxCount, string MapFloorId = "", string SceneName = "");
        private static Snapshot EmptySnapshot()
        {
            return new Snapshot(false, "", 0, 0, "", "", "", "", 0, 0, 0, Guid.Empty, 0, 0);
        }

        private static Snapshot ReadSnapshot()
        {
            ResolveAddresses();
            var flow = Pointer(flowSlot);
            if (flow == 0)
            {
                return EmptySnapshot();
            }

            var controller = Pointer(flow + 0x20);
            if (controller == 0 || Pointer(controller + 0x10) == 0 || ReadInt(controller + 0xF0) != 8)
            {
                return EmptySnapshot();
            }

            var manager = Pointer(characterSlot);
            var player = manager == 0 ? 0 : Pointer(manager + 0xB0);
            if (player == 0)
            {
                return EmptySnapshot();
            }

            var kind = Pointer(player + 0x40);
            var hpContext = Pointer(player + 0x70);
            var hp = hpContext == 0 ? 0 : Pointer(hpContext + 0x10);
            if (kind == 0 || hp == 0 || ReadByte(hp + 0x34) == 0)
            {
                return EmptySnapshot();
            }

            var health = ReadInt(hp + 0x28);
            var maxHealth = ReadInt(hp + 0x30);
            if (maxHealth is <= 0 or > 10000000 || health < 0 || health > maxHealth)
            {
                throw new InvalidDataException("RE9: invalid player health.");
            }

            var character = ReadId(kind);
            var sceneName = Pointer(controller + 0x80);
            var guiMap = Pointer(guiMapSlot);
            var bridge = guiMap == 0 ? 0 : Pointer(guiMap + 0x178);
            var floor = guiMap == 0 ? 0 : Pointer(guiMap + 0x48);
            var section = bridge == 0 ? 0 : Pointer(bridge + 0xA0);
            var difficultyManager = Pointer(difficultySlot);
            if (difficultyManager == 0)
            {
                return EmptySnapshot();
            }

            var overrideMode = ReadByte(flow + 0x36) != 0;
            var difficulty = Pointer((overrideMode ? difficultyOverride : difficultyManager) + 0x10);
            var equipment = ReadEquipment(player, kind, character == "cp_A000");
            var objective = ReadObjective();
            var snapshot = new Snapshot(true, character, health, maxHealth, ReadId(difficulty), ReadId(section), equipment.WeaponId, equipment.ItemId, equipment.Ammo, equipment.ReserveAmmo, equipment.Money, objective.Guid, objective.Count, objective.Maximum, ReadId(floor), ReadManagedString(sceneName));
            if (flow != Pointer(flowSlot) || controller != Pointer(flow + 0x20) || Pointer(controller + 0x10) == 0 || ReadInt(controller + 0xF0) != 8 || manager != Pointer(characterSlot) || player != Pointer(manager + 0xB0) || kind != Pointer(player + 0x40) || hpContext != Pointer(player + 0x70) || hp != Pointer(hpContext + 0x10) || sceneName != Pointer(controller + 0x80) || guiMap != Pointer(guiMapSlot) || bridge != (guiMap == 0 ? 0 : Pointer(guiMap + 0x178)) || floor != (guiMap == 0 ? 0 : Pointer(guiMap + 0x48)) || section != (bridge == 0 ? 0 : Pointer(bridge + 0xA0)) || difficultyManager != Pointer(difficultySlot) || overrideMode != (ReadByte(flow + 0x36) != 0) || difficulty != Pointer((overrideMode ? difficultyOverride : difficultyManager) + 0x10))
            {
                throw new InvalidDataException("RE9: scene changed during the read; retry next poll.");
            }

            return snapshot;
        }

        private static string GetDifficulty(string id)
        {
            return id switch
            {
                "ID0010" => "Casual",
                "ID0020" => "Standard (Modern)",
                "ID0030" => "Standard (Classic)",
                "ID0040" => "Insanity",
                _ => id,
            };
        }

        private sealed record Objective(Guid Guid, int Count, int Maximum);
        private static Objective ReadObjective()
        {
            var manager = Pointer(objectiveSlot);
            var controller = manager == 0 ? 0 : Pointer(manager + 0x40);
            var list = controller == 0 ? 0 : Pointer(controller + 0x10);
            if (list == 0)
            {
                return new Objective(Guid.Empty, 0, 0);
            }

            var items = Pointer(list + 0x10);
            var count = ReadInt(list + 0x18);
            var capacity = items == 0 ? 0 : ReadInt(items + 0x1C);
            if (count < 0 || count > capacity || capacity > 256)
            {
                throw new InvalidDataException("RE9: invalid objective list.");
            }

            if (count == 0)
            {
                return new Objective(Guid.Empty, 0, 0);
            }

            var latest = Pointer(controller + 0x18);
            ulong selected = 0;
            for (var j = 0; j < count; j++)
            {
                var item = Pointer(items + 0x20 + (ulong)j * 8);
                if (item == 0)
                {
                    throw new InvalidDataException("RE9: null objective.");
                }

                if (latest == 0 || Pointer(item + 0x10) == latest)
                {
                    selected = item;
                }
            }

            if (selected == 0)
            {
                selected = Pointer(items + 0x20 + (ulong)(count - 1) * 8);
            }

            var id = Pointer(selected + 0x10);
            var maximum = 0;
            var settings = Pointer(manager + 0x30);
            foreach (var(key, value)in DictionaryEntries(settings))
                if (key == id && value != 0)
                {
                    maximum = ReadInt(value + 0x20);
                    break;
                }

            var current = ReadInt(selected + 0x38);
            if (current < 0 || maximum < 0 || current > 1000000 || maximum > 1000000)
            {
                throw new InvalidDataException("RE9: invalid objective progress.");
            }

            var guid = new Guid(Hypervisor.ReadArray(selected + 0x28, 16, true));
            if (manager != Pointer(objectiveSlot) || controller != Pointer(manager + 0x40) || list != Pointer(controller + 0x10) || items != Pointer(list + 0x10) || count != ReadInt(list + 0x18) || latest != Pointer(controller + 0x18))
            {
                throw new InvalidDataException("RE9: objective changed during the read.");
            }

            return new Objective(guid, current, maximum);
        }

        private static List<(ulong Key, ulong Value)> DictionaryEntries(ulong dictionary)
        {
            var result = new List<(ulong Key, ulong Value)>();
            if (dictionary == 0)
            {
                return result;
            }

            var entries = Pointer(dictionary + 0x18);
            var version = ReadInt(dictionary + 0x38);
            var count = ReadInt(dictionary + 0x3C);
            var capacity = entries == 0 ? 0 : ReadInt(entries + 0x1C);
            if (count < 0 || count > capacity || capacity > 4096)
            {
                throw new InvalidDataException("RE9: invalid dictionary.");
            }

            for (var j = 0; j < count; j++)
            {
                var entry = entries + 0x20 + (ulong)j * 0x18;
                if (ReadInt(entry + 4) < -1)
                {
                    continue;
                }

                var key = Pointer(entry + 8);
                if (key != 0)
                {
                    result.Add((key, Pointer(entry + 0x10)));
                }
            }

            if (entries != Pointer(dictionary + 0x18) || count != ReadInt(dictionary + 0x3C) || version != ReadInt(dictionary + 0x38))
            {
                throw new InvalidDataException("RE9: dictionary changed during the read.");
            }

            return result;
        }

        private static string ReadId(ulong id)
        {
            return id == 0 ? "" : ReadManagedString(Pointer(id + 0x10));
        }

        private static string ReadManagedString(ulong value)
        {
            if (value == 0)
            {
                return "";
            }

            var length = ReadInt(value + 0x10);
            if (length is < 0 or > 256)
            {
                throw new InvalidDataException("RE9: invalid managed string length.");
            }

            return length == 0 ? "" : Encoding.Unicode.GetString(Hypervisor.ReadArray(value + 0x14, length * 2, true));
        }

        private static int ReadInt(ulong address)
        {
            return Hypervisor.Read<int>(address, true);
        }

        private static byte ReadByte(ulong address)
        {
            return Hypervisor.Read<byte>(address, true);
        }

        private static ulong Pointer(ulong address)
        {
            return Hypervisor.Read<ulong>(address, true);
        }

        private sealed record Equipment(string WeaponId, string ItemId, int Ammo, int ReserveAmmo, int Money);
        private static Equipment ReadEquipment(ulong player, ulong kind, bool isLeon)
        {
            var manager = Pointer(inventorySlot);
            if (manager == 0)
            {
                throw new InvalidDataException("RE9: inventory manager unavailable.");
            }

            if (ReadByte(kind + 0x20) == 0)
            {
                throw new InvalidDataException("RE9: character inventory user cache is not ready.");
            }

            var userIndex = ReadInt(kind + 0x24);
            if (userIndex is < 0 or > 3)
            {
                throw new InvalidDataException("RE9: invalid inventory user index.");
            }

            var user = Pointer(Pointer(inventoryUserTable + (ulong)userIndex * 8));
            if (user == 0)
            {
                throw new InvalidDataException("RE9: inventory user unavailable.");
            }

            var normalInventories = Pointer(manager + 0x58);
            ulong mainInventory = 0;
            foreach (var(key, value)in DictionaryEntries(normalInventories))
            {
                if (Pointer(key + 0x10) != user || ReadInt(key + 0x18) != 0 || ReadByte(key + 0x1C) != 0)
                {
                    continue;
                }

                if (value == 0 || mainInventory != 0)
                {
                    throw new InvalidDataException("RE9: ambiguous hand inventory.");
                }

                mainInventory = value;
            }

            if (mainInventory == 0)
            {
                throw new InvalidDataException("RE9: hand inventory unavailable.");
            }

            var money = isLeon ? ReadInt(mainInventory + 0x90) : 0;
            if (money is < 0 or > 1000000000)
            {
                throw new InvalidDataException("RE9: invalid credits.");
            }

            var common = Pointer(player + 0xF8);
            var owner = common == 0 ? 0 : Pointer(common + 0x10);
            var ownerNative = owner == 0 ? 0 : Pointer(owner + 0x10);
            var weapon = ownerNative == 0 ? 0 : Pointer(owner + 0x310);
            var weaponNative = weapon == 0 ? 0 : Pointer(weapon + 0x10);
            var weaponId = weaponNative == 0 ? "" : ReadId(Pointer(weapon + 0x50));
            if (weaponId == "Invalid")
            {
                weaponId = "";
            }

            var panel = weaponNative == 0 || Pointer(Pointer(weapon)) != Pointer(equipmentTypeSlot) ? 0 : Pointer(weapon + 0x160);
            Equipment Finish(Equipment result)
            {
                if (manager != Pointer(inventorySlot) || normalInventories != Pointer(manager + 0x58) || common != Pointer(player + 0xF8) || (common != 0 && owner != Pointer(common + 0x10)) || (owner != 0 && ownerNative != Pointer(owner + 0x10)) || (ownerNative != 0 && weapon != Pointer(owner + 0x310)) || (weapon != 0 && weaponNative != Pointer(weapon + 0x10)))
                {
                    throw new InvalidDataException("RE9: equipment changed during the read.");
                }

                return result;
            }

            if (panel == 0)
            {
                return Finish(new(weaponId, "", 0, 0, money));
            }

            var itemId = ReadId(Pointer(panel + 0x20));
            var loading = Pointer(panel + 0x68);
            var count = loading == 0 ? 0 : ReadInt(loading + 0x1C);
            if (count is < 0 or > 32)
            {
                throw new InvalidDataException("RE9: invalid loaded item count.");
            }

            if (count == 0)
            {
                return Finish(new(weaponId, itemId, 0, 0, money));
            }

            var loaded = int.MaxValue;
            var leadAmmoId = "";
            for (var j = 0; j < count; j++)
            {
                var contain = Pointer(loading + 0x20 + (ulong)j * 8);
                var amount = contain == 0 ? 0 : Pointer(contain + 0x18);
                if (amount == 0 || Pointer(contain + 0x10) != 0)
                {
                    throw new InvalidDataException("RE9: unsupported ammunition stock conversion.");
                }

                var getter = Pointer(Pointer(amount) + 0x60);
                if (getter == 0 || !Hypervisor.ReadArray(getter, 4, true).SequenceEqual(new byte[] { 0x8B, 0x42, 0x20, 0xC3 }))
                {
                    throw new InvalidDataException("RE9: unsupported ammunition amount type.");
                }

                var ammoId = ReadManagedString(Pointer(amount + 0x18));
                if (ammoId.Length == 0)
                {
                    throw new InvalidDataException("RE9: ammunition ID unavailable.");
                }

                if (j == 0)
                {
                    leadAmmoId = ammoId;
                }

                var stock = ReadInt(amount + 0x20);
                if (stock is < 0 or > 1000000)
                {
                    throw new InvalidDataException("RE9: invalid loaded ammunition.");
                }

                loaded = Math.Min(loaded, stock);
            }

            var panelKey = Pointer(panel + 0x10);
            if (panelKey == 0 || Pointer(panelKey + 0x10) != user || ReadInt(panelKey + 0x18) != 0)
            {
                throw new InvalidDataException("RE9: weapon belongs to a different inventory.");
            }

            var temporal = ReadByte(panelKey + 0x1C);
            if (temporal > 1)
            {
                throw new InvalidDataException("RE9: invalid temporal inventory flag.");
            }

            ulong weaponInventory = 0;
            foreach (var(key, value)in DictionaryEntries(Pointer(manager + 0x58 + (ulong)temporal * 8)))
                if (Pointer(key + 0x10) == user && ReadInt(key + 0x18) == 0 && ReadByte(key + 0x1C) == temporal)
                {
                    if (weaponInventory != 0 || value == 0)
                    {
                        throw new InvalidDataException("RE9: ambiguous weapon inventory.");
                    }

                    weaponInventory = value;
                }

            if (weaponInventory == 0)
            {
                throw new InvalidDataException("RE9: weapon inventory unavailable.");
            }

            var reserve = 0;
            foreach (var offset in new ulong[]
            {
                0x48,
                0x50,
                0x58
            }

            )
            {
                foreach (var(_, info)in DictionaryEntries(Pointer(weaponInventory + offset)))
                {
                    var detail = info == 0 ? 0 : Pointer(info + 0x28);
                    if (detail == 0)
                    {
                        throw new InvalidDataException("RE9: inventory detail unavailable.");
                    }

                    if (ReadId(Pointer(detail + 0x10)) != leadAmmoId)
                    {
                        continue;
                    }

                    var stock = ReadInt(info + 0x74);
                    if (stock is < 0 or > 1000000 || reserve > 1000000 - stock)
                    {
                        throw new InvalidDataException("RE9: invalid reserve ammunition.");
                    }

                    reserve += stock;
                }
            }

            if (manager != Pointer(inventorySlot) || normalInventories != Pointer(manager + 0x58) || common != Pointer(player + 0xF8) || owner != Pointer(common + 0x10) || weapon != Pointer(owner + 0x310) || panel != Pointer(weapon + 0x160) || loading != Pointer(panel + 0x68) || count != ReadInt(loading + 0x1C))
            {
                throw new InvalidDataException("RE9: equipment changed during the read.");
            }

            return Finish(new(weaponId, itemId, loaded, reserve, money));
        }

        private static void ResetAddresses()
        {
            cachedProcessId = cachedModuleSize = 0;
            cachedModuleBase = characterSlot = difficultySlot = flowSlot = inventorySlot = guiMapSlot = objectiveSlot = difficultyOverride = 0;
            nextScan = DateTime.MinValue;
            inventoryUserTable = equipmentTypeSlot = 0;
        }

        private static void ResolveAddresses()
        {
            var process = Hypervisor.Process ?? throw new InvalidDataException("RE9: game is not attached.");
            var module = process.MainModule ?? throw new InvalidDataException("RE9: module not loaded.");
            var moduleBase = (ulong)module.BaseAddress.ToInt64();
            if (cachedProcessId != process.Id || cachedModuleBase != moduleBase || cachedModuleSize != module.ModuleMemorySize)
            {
                ResetAddresses();
                cachedProcessId = process.Id;
                cachedModuleBase = moduleBase;
                cachedModuleSize = module.ModuleMemorySize;
            }

            if (characterSlot != 0)
            {
                return;
            }

            if (DateTime.UtcNow < nextScan)
            {
                throw new InvalidDataException("RE9: waiting to retry patterns.");
            }

            nextScan = DateTime.UtcNow.AddSeconds(30);
            var found = new Dictionary<string, ulong>();
            ulong Resolve(string pattern, int operand = 3, int length = 7)
            {
                if (!found.TryGetValue(pattern, out var match))
                {
                    found[pattern] = match = FindUniquePattern(moduleBase, module.ModuleMemorySize, pattern);
                }

                var target = checked((ulong)((long)match + length + ReadInt(match + (ulong)operand)));
                if (target < moduleBase || target + 8 > moduleBase + (ulong)module.ModuleMemorySize)
                {
                    throw new InvalidDataException("RE9: RIP-relative target is outside the module.");
                }

                return target;
            }

            var character = Resolve(CharacterPattern);
            var difficulty = Resolve(DifficultyPattern);
            var flow = Resolve(DifficultyPattern, 15, 19);
            var modeOverride = Resolve(DifficultyPattern, 31, 35);
            var inventory = Resolve(InventoryPattern);
            var objective = Resolve(ObjectivePattern);
            var guiMap = Resolve(GuiMapPattern);
            FindUniquePattern(moduleBase, module.ModuleMemorySize, MainGameFlowIsMainGamePattern);
            FindUniquePattern(moduleBase, module.ModuleMemorySize, CharacterContextHitPointPattern);
            FindUniquePattern(moduleBase, module.ModuleMemorySize, HitPointCurrentAndMaximumPattern);
            FindUniquePattern(moduleBase, module.ModuleMemorySize, FlowPhasePattern);
            FindUniquePattern(moduleBase, module.ModuleMemorySize, GuiManagerMapAndCurrentFloorPattern);
            FindUniquePattern(moduleBase, module.ModuleMemorySize, GuiMapBridgeStaySectionPattern);
            FindUniquePattern(moduleBase, module.ModuleMemorySize, MainGameFlowCurrentScenePattern);
            FindUniquePattern(moduleBase, module.ModuleMemorySize, InventoryLayoutPattern);
            var userTable = Resolve(InventoryUserPattern, 13, 17);
            var equipmentType = Resolve(EquipmentLayoutPattern, 50, 54);
            inventoryUserTable = userTable;
            equipmentTypeSlot = equipmentType;
            difficultySlot = difficulty;
            flowSlot = flow;
            difficultyOverride = modeOverride;
            inventorySlot = inventory;
            objectiveSlot = objective;
            guiMapSlot = guiMap;
            characterSlot = character;
        }

        private static ulong FindUniquePattern(ulong moduleBase, int moduleSize, string signature)
        {
            if (moduleSize < 64)
            {
                throw new InvalidDataException("RE9: invalid module size.");
            }

            var peOffset = Hypervisor.Read<uint>(moduleBase + 0x3C, true);
            if (peOffset > (uint)moduleSize - 24)
            {
                throw new InvalidDataException("RE9: invalid PE offset.");
            }

            var pe = moduleBase + peOffset;
            if (peOffset > (uint)moduleSize - 24 || Hypervisor.Read<uint>(pe, true) != 0x4550 || Hypervisor.Read<ushort>(pe + 4, true) != 0x8664)
            {
                throw new InvalidDataException("RE9: invalid PE header.");
            }

            var sectionCount = Hypervisor.Read<ushort>(pe + 6, true);
            var optionalSize = Hypervisor.Read<ushort>(pe + 20, true);
            var sectionTable = pe + 24 + optionalSize;
            if (sectionTable + (ulong)sectionCount * 40 > moduleBase + (ulong)moduleSize)
            {
                throw new InvalidDataException("RE9: invalid section table.");
            }

            var pattern = SignaturePattern.Parse(signature);
            var prefix = pattern.Bytes.TakeWhile(value => value.HasValue).Select(value => value.GetValueOrDefault()).ToArray();
            if (prefix.Length == 0)
            {
                throw new InvalidDataException("RE9: pattern needs a fixed prefix.");
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
                    throw new InvalidDataException("RE9: invalid executable section.");
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
                            throw new InvalidDataException("RE9: manager pattern is ambiguous.");
                        }

                        match = address;
                    }
                }
            }

            return match != 0 ? match : throw new InvalidDataException("RE9: manager pattern was not found.");
        }

        private static void InitializeDiscord()
        {
            if (discord == null)
            {
                throw new InvalidOperationException("RE9: presence is not initialized.");
            }

            discord.Initialize();
        }
    }
}
