using MultiPresence.Runtime;
using MultiPresence.Infrastructure;
using DiscordRPC;
using MultiPresence.Models.RE2;

namespace MultiPresence.Presence
{
    public class RE2R
    {
        private const string PlayerPattern = "48 8B 35 ?? ?? ?? ?? 48 85 F6 75 1A 45 33 C0 8D 56 38 48 8B 74 24 30 48 8B 5C 24 38 48 83 C4 20 5F E9 ?? ?? ?? ?? E8 ?? ?? ?? ??";
        private const string WeaponPattern = "4C 8B 05 ?? ?? ?? ?? 4D 85 C0 74 0A 49 39 48 18 74 04 32 C0 EB 02 B0 01 0F B6 C0 85 C0 75 48 48 8B 47 28 48 85 C0 0F 84 06 FF FF FF";
        private const string InventoryPattern = "48 8B 15 ?? ?? ?? ?? 49 8B F8 48 8B D9 48 83 78 18 00 75 11 4D 85 C0 75 1E";
        private const string FlowPattern = "48 8B 15 ?? ?? ?? ?? 48 8B CF 48 85 D2 0F 84 C1 00 00 00 E8 ?? ?? ?? ??";
        private const string LocationPattern = "8B 05 ?? ?? ?? ?? C3 CC CC CC CC CC CC CC CC CC 48 89 5C 24 08 48 89 74 24 10 57 48 83 EC 20 48 8B 05 ?? ?? ?? ??";
        private const string MapPattern = "8B 05 ?? ?? ?? ?? C3 CC CC CC CC CC CC CC CC CC C6 82 30 01 00 00 00 C3";
        private const string ScenarioLayoutPattern = "48 83 EC 28 48 8B 82 A0 01 00 00 48 85 C0 75 12 45 33 C0 8D 50 38 E8 ?? ?? ?? ?? 33 C0 48 83 C4 28 C3 8B 40 1C";
        private const string StateLayoutPattern = "8B AD DC 00 00 00 48 8B DF BF 23 00 00 00 80 7C 24 40 00 74 16 49 8B D6";
        private static DiscordRpcClient? discord;
        private static DiscordStatusUpdater? updater;
        private static int cachedProcessId;
        private static ulong cachedModuleBase, playerSlot, weaponSlot, inventorySlot, flowSlot, locationAddress, mapAddress;
        private static DateTime nextScan;
        public static void DoAction()
        {
            ResetAddresses();
            GetPID();
            discord = new DiscordRpcClient("1349630099510657079");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/config/Resident Evil 2 Remake.json");
            PresenceRuntime.Start(nameof(RE2R), "re2", RPC);
        }

        private static void GetPID()
        {
            ProcessMonitor.TryAttach("re2");
        }

        private static async Task RPC()
        {
            if (discord == null || updater == null)
            {
                throw new InvalidOperationException("RE2R: presence is not initialized.");
            }

            if (ProcessMonitor.IsRunning("re2"))
            {
                try
                {
                    if (!Hypervisor.IsAttached && !ProcessMonitor.TryAttach("re2"))
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
                    PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Resident Evil 2", placeholders);
                }
                catch (Exception exception) when (exception is not OutOfMemoryException and not StackOverflowException)
                {
                    AppLog.Warning("Resident Evil 2 memory is not ready.", exception);
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
            var percentage = snapshot.MaxHealth > 0 ? (double)snapshot.Health / snapshot.MaxHealth * 100 : 0;
            var healthStatus = snapshot.Health <= 0 ? "Dead" : snapshot.Poisoned ? "Poison" : percentage >= 66 ? "Fine" : percentage >= 33 ? "Caution" : "Danger";
            var region = Locations.GetRegion(snapshot.RegionId);
            var room = Locations.GetRoom(snapshot.RoomId);
            return new Dictionary<string, object>
            {
                { "character", Characters.GetName(snapshot.CharacterId) },
                { "characterid", snapshot.CharacterId },
                { "scenario", Characters.GetScenario(snapshot.ScenarioId) },
                { "scenarioid", snapshot.ScenarioId },
                { "region", region },
                { "regionid", snapshot.RegionId },
                { "room", room },
                { "roomid", snapshot.RoomId },
                { "location", $"{region} / {room}"},
                { "health", snapshot.Health },
                { "maxhealth", snapshot.MaxHealth },
                { "healthstatus", healthStatus },
                { "healthpercentage", percentage },
                { "weapon", snapshot.WeaponId < 0 ? "None" : await Weapons.GetWeapon(snapshot.WeaponId) },
                { "weaponid", snapshot.WeaponId },
                { "ammo", snapshot.Ammo }
            };
        }

        private static void ShowStatus(string text)
        {
            if (discord == null)
            {
                throw new InvalidOperationException("RE2R: presence is not initialized.");
            }

            discord.SetPresence(new RichPresence()
            {
                Details = text,
                State = "",
                Assets = new Assets()
                {
                    LargeImageKey = "logo",
                    LargeImageText = "Resident Evil 2"
                },
                Timestamps = PlaceholderHelper._startTimestamp
            });
        }

        private sealed record Snapshot(bool IsInGame, int CharacterId, int ScenarioId, int RegionId, int RoomId, int Health, int MaxHealth, bool Poisoned, int WeaponId, int Ammo);
        private static Snapshot EmptySnapshot()
        {
            return new Snapshot(false, -1, -1, -1, -1, 0, 0, false, -1, 0);
        }

        private static Snapshot ReadSnapshot()
        {
            ResolveAddresses();
            var flow = Pointer(flowSlot);
            if (flow == 0)
            {
                return EmptySnapshot();
            }

            var state = ReadInt(flow + 0xDC);
            if (state is not (6 or 7 or 9))
            {
                return EmptySnapshot();
            }

            var header = Pointer(flow + 0x1A0);
            var player = Follow(playerSlot, 0x50, 0x10, 0x20);
            if (header == 0 || player == 0)
            {
                return EmptySnapshot();
            }

            var hp = Pointer(player + 0x230);
            if (hp == 0)
            {
                return EmptySnapshot();
            }

            var maxHealth = ReadInt(hp + 0x54);
            var health = ReadInt(hp + 0x58);
            if (maxHealth <= 0 || maxHealth > 10000000 || health < 0 || health > maxHealth)
            {
                throw new InvalidDataException("RE2: invalid hit-point controller.");
            }

            var weapon = Follow(weaponSlot, 0x188);
            var ammunition = Follow(inventorySlot, 0x50, 0xA0, 0x18, 0x10);
            var snapshot = new Snapshot(true, ReadInt(player + 0x54), ReadInt(header + 0x1C), ReadInt(locationAddress), ReadInt(mapAddress), health, maxHealth, Hypervisor.Read<byte>(player + 0x258, true) != 0, weapon == 0 ? -1 : Hypervisor.Read<byte>(weapon + 0x2D0, true), ammunition == 0 ? 0 : ReadInt(ammunition + 0x20));
            if (flow != Pointer(flowSlot) || state != ReadInt(flow + 0xDC) || header != Pointer(flow + 0x1A0) || player != Follow(playerSlot, 0x50, 0x10, 0x20) || hp != Pointer(player + 0x230) || snapshot.ScenarioId != ReadInt(header + 0x1C) || snapshot.RegionId != ReadInt(locationAddress) || snapshot.RoomId != ReadInt(mapAddress))
            {
                throw new InvalidDataException("RE2: scene changed during the read; retry next poll.");
            }

            return snapshot;
        }

        private static int ReadInt(ulong address)
        {
            return Hypervisor.Read<int>(address, true);
        }

        private static ulong Pointer(ulong address)
        {
            return Hypervisor.Read<ulong>(address, true);
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

        private static void ResetAddresses()
        {
            cachedProcessId = 0;
            cachedModuleBase = playerSlot = weaponSlot = inventorySlot = flowSlot = locationAddress = mapAddress = 0;
            nextScan = DateTime.MinValue;
        }

        private static void ResolveAddresses()
        {
            var process = Hypervisor.Process ?? throw new InvalidDataException("RE2R: game is not attached.");
            var module = process.MainModule ?? throw new InvalidDataException("RE2: module not loaded.");
            var moduleBase = (ulong)module.BaseAddress.ToInt64();
            if (cachedProcessId != process.Id || cachedModuleBase != moduleBase)
            {
                ResetAddresses();
                cachedProcessId = process.Id;
                cachedModuleBase = moduleBase;
            }

            if (flowSlot != 0)
            {
                return;
            }

            if (DateTime.UtcNow < nextScan)
            {
                throw new InvalidDataException("RE2: waiting to retry patterns.");
            }

            nextScan = DateTime.UtcNow.AddSeconds(30);
            ulong Resolve(string pattern, int operand, int length)
            {
                var match = FindUniquePattern(moduleBase, module.ModuleMemorySize, pattern);
                var target = checked((ulong)((long)match + length + ReadInt(match + (ulong)operand)));
                if (target < moduleBase || target + 8 > moduleBase + (ulong)module.ModuleMemorySize)
                {
                    throw new InvalidDataException("RE2: RIP-relative target is outside the module.");
                }

                return target;
            }

            var player = Resolve(PlayerPattern, 3, 7);
            var weapon = Resolve(WeaponPattern, 3, 7);
            var inventory = Resolve(InventoryPattern, 3, 7);
            var flow = Resolve(FlowPattern, 3, 7);
            var location = Resolve(LocationPattern, 2, 6);
            var map = Resolve(MapPattern, 2, 6);
            FindUniquePattern(moduleBase, module.ModuleMemorySize, ScenarioLayoutPattern);
            FindUniquePattern(moduleBase, module.ModuleMemorySize, StateLayoutPattern);
            if (map != location + 4)
            {
                throw new InvalidDataException("RE2: location layout changed.");
            }

            playerSlot = player;
            weaponSlot = weapon;
            inventorySlot = inventory;
            locationAddress = location;
            mapAddress = map;
            flowSlot = flow;
        }

        private static ulong FindUniquePattern(ulong moduleBase, int moduleSize, string signature)
        {
            if (moduleSize < 64)
            {
                throw new InvalidDataException("RE2: invalid module size.");
            }

            var peOffset = Hypervisor.Read<uint>(moduleBase + 0x3C, true);
            if (peOffset > (uint)moduleSize - 24)
            {
                throw new InvalidDataException("RE2: invalid PE offset.");
            }

            var pe = moduleBase + peOffset;
            if (peOffset > (uint)moduleSize - 24 || Hypervisor.Read<uint>(pe, true) != 0x4550 || Hypervisor.Read<ushort>(pe + 4, true) != 0x8664)
            {
                throw new InvalidDataException("RE2: invalid PE header.");
            }

            var sectionCount = Hypervisor.Read<ushort>(pe + 6, true);
            var optionalSize = Hypervisor.Read<ushort>(pe + 20, true);
            var sectionTable = pe + 24 + optionalSize;
            if (sectionTable + (ulong)sectionCount * 40 > moduleBase + (ulong)moduleSize)
            {
                throw new InvalidDataException("RE2: invalid section table.");
            }

            var pattern = SignaturePattern.Parse(signature);
            var prefix = pattern.Bytes.TakeWhile(value => value.HasValue).Select(value => value.GetValueOrDefault()).ToArray();
            if (prefix.Length == 0)
            {
                throw new InvalidDataException("RE2: pattern needs a fixed prefix.");
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
                    throw new InvalidDataException("RE2: invalid executable section.");
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
                            throw new InvalidDataException("RE2: manager pattern is ambiguous.");
                        }

                        match = address;
                    }
                }
            }

            return match != 0 ? match : throw new InvalidDataException("RE2: manager pattern was not found.");
        }

        private static void InitializeDiscord()
        {
            if (discord == null)
            {
                throw new InvalidOperationException("RE2R: presence is not initialized.");
            }

            discord.Initialize();
        }
    }
}
