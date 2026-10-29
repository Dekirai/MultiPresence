using MultiPresence.Runtime;
using MultiPresence.Infrastructure;
using DiscordRPC;
using MultiPresence.Models.VS;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class VS
    {
        private const string ProcessName = "VampireSurvivors";
        private const string ModuleName = "GameAssembly.dll";
        private const string ManagerPattern = "48 8B 05 ?? ?? ?? ?? 48 8B 88 B8 00 00 00 80 79 20 00 75 ?? " + "48 8B 05 ?? ?? ?? ?? 48 8B 88 B8 00 00 00 48 8B 01";
        private static DiscordRpcClient? discord;
        private static DiscordStatusUpdater? updater;
        private static int cachedProcessId;
        private static ulong cachedModuleBase, adventureClassSlot, gameManagerClassSlot;
        private static DateTime nextScan;
        public static string adventure = "";
        public static void DoAction()
        {
            ResetAddresses();
            GetPID();
            discord = new DiscordRpcClient("1283783524423438409");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/config/Vampire Survivors.json");
            PresenceRuntime.Start(nameof(VS), ProcessName, RPC);
        }

        private static void GetPID()
        {
            ProcessMonitor.TryAttach(ProcessName);
        }

        private static async Task RPC()
        {
            if (discord == null || updater == null)
            {
                throw new InvalidOperationException("VS: presence is not initialized.");
            }

            if (ProcessMonitor.IsRunning(ProcessName))
            {
                try
                {
                    if (!Hypervisor.IsAttached && !ProcessMonitor.TryAttach(ProcessName))
                    {
                        return;
                    }

                    var snapshot = ReadSnapshot();
                    var placeholders = await PlaceholderHelper.GetPlaceholders(() => GeneratePlaceholders(snapshot));
                    var template = snapshot.IsInGame ? "Ingame" : "Default";
                    if (snapshot.IsAdventure)
                    {
                        template += "_Adventure";
                    }

                    PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Vampire Survivors", placeholders, template);
                }
                catch (Exception exception) when (exception is not OutOfMemoryException and not StackOverflowException)
                {
                    AppLog.Warning("Vampire Survivors memory is not ready; showing the main menu.", exception);
                    discord.SetPresence(new RichPresence()
                    {
                        Details = "In Main Menu",
                        State = "",
                        Assets = new Assets()
                        {
                            LargeImageKey = "logo",
                            LargeImageText = "Vampire Survivors"
                        },
                        Timestamps = PlaceholderHelper._startTimestamp
                    });
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
            adventure = snapshot.IsAdventure ? Stages.GetAdventureName(snapshot.StageId) : "";
            var modes = new List<string>();
            if (snapshot.Hyper)
            {
                modes.Add("Hyper");
            }

            if (snapshot.Hurry)
            {
                modes.Add("Hurry");
            }

            if (snapshot.Arcanas)
            {
                modes.Add("Arcanas");
            }

            if (snapshot.LimitBreak)
            {
                modes.Add("Limit Break");
            }

            if (snapshot.Inverse)
            {
                modes.Add("Inverse");
            }

            if (snapshot.Endless)
            {
                modes.Add("Endless");
            }

            if (snapshot.RandomEvents)
            {
                modes.Add("Random Events");
            }

            if (snapshot.RandomLevelUp)
            {
                modes.Add("Random LevelUp");
            }

            var time = TimeSpan.FromSeconds(float.IsFinite(snapshot.Time) ? Math.Max(0, snapshot.Time) : 0);
            return new Dictionary<string, object>
            {
                { "character", await Characters.GetCharacter(snapshot.CharacterId) },
                { "stage", await Stages.GetStages(snapshot.StageId) },
                { "coins", WholeNumber(snapshot.Coins) },
                { "coinsingame", WholeNumber(snapshot.RunCoins) },
                { "kills", snapshot.Kills },
                { "health", WholeNumber(snapshot.Health) },
                { "level", snapshot.Level },
                { "time", $"{(int)time.TotalMinutes:D2}:{time.Seconds:D2}"},
                { "adventure", adventure },
                { "modes", modes.Count == 0 ? "None" : string.Join(", ", modes) },
                { "hasEggs", snapshot.Eggs },
                { "maxWeapons", snapshot.MaxWeapons },
                { "weapons", FormatEquipment(snapshot.Weapons, false) },
                { "weaponsWithLevels", FormatEquipment(snapshot.Weapons, true) },
                { "weaponIds", string.Join(", ", snapshot.Weapons.Select(item => item.Id)) },
                { "weaponCount", snapshot.Weapons.Length },
                { "passives", FormatEquipment(snapshot.Passives, false) },
                { "passivesWithLevels", FormatEquipment(snapshot.Passives, true) },
                { "passiveIds", string.Join(", ", snapshot.Passives.Select(item => item.Id)) },
                { "passiveCount", snapshot.Passives.Length },
                { "equipment", FormatEquipment(snapshot.Weapons.Concat(snapshot.Passives).ToArray(), true) }
            };
        }

        private sealed record Snapshot(bool IsAdventure, bool IsInGame, int CharacterId, int StageId, float Coins, float RunCoins, int Kills, float Health, int Level, float Time, bool Hyper, bool Hurry, bool Arcanas, bool LimitBreak, bool Inverse, bool Endless, bool Eggs, bool RandomEvents, bool RandomLevelUp, int MaxWeapons, EquippedItem[] Weapons, EquippedItem[] Passives);
        private sealed record EquippedItem(int Id, int Level)
        {
            public string Name => Items.GetEquipmentName(Id);
        }

        private static Snapshot ReadSnapshot()
        {
            ResolveAddresses();
            var adventureClass = Pointer(adventureClassSlot);
            RequireClass(adventureClass, "AdventureManager", "VampireSurvivors.App.Scripts.Framework.Adventures");
            var adventureStatics = Pointer(adventureClass + 0xB8);
            var manager = Pointer(adventureStatics + 8);
            var playerOptions = Pointer(manager + 0x10);
            RequireObject(playerOptions, "PlayerOptions", "VampireSurvivors.Objects");
            var isAdventure = Hypervisor.Read<byte>(adventureStatics + 0x20, true) != 0;
            var config = Pointer(playerOptions + 0x68);
            if (config == 0)
            {
                config = Pointer(playerOptions + 0x58);
            }

            if (config == 0)
            {
                var adventureConfig = Pointer(playerOptions + 0x78);
                config = adventureConfig != 0 && Hypervisor.Read<byte>(adventureConfig + 0x308, true) != 0 ? adventureConfig : Pointer(playerOptions + 0x50);
            }

            RequireObject(config, "PlayerOptionsData", "VampireSurvivors.Data");
            var data = Hypervisor.ReadArray(config, 0xA0, true);
            var gameManager = GetGameManager();
            var isInGame = gameManager != 0 && Hypervisor.Read<byte>(gameManager + 0x209, true) != 0;
            float time = 0, health = 0;
            int level = 0;
            EquippedItem[] weapons = [], passives = [];
            if (isInGame)
            {
                time = Hypervisor.Read<float>(gameManager + 0x468, true);
                var character = GetActiveCharacter(playerOptions);
                if (character != 0)
                {
                    health = Hypervisor.Read<float>(character + 0x240, true);
                    level = Hypervisor.Read<int>(character + 0x244, true);
                    weapons = ReadEquipment(Pointer(character + 0xC0));
                    passives = ReadEquipment(Pointer(character + 0xC8));
                }
            }

            return new Snapshot(isAdventure, isInGame, BitConverter.ToInt32(data, 0x54), BitConverter.ToInt32(data, 0x58), BitConverter.ToSingle(data, 0x94), BitConverter.ToSingle(data, 0x98), BitConverter.ToInt32(data, 0x9C), health, level, time, data[0x5C] != 0, data[0x5E] != 0, data[0x5F] != 0, data[0x60] != 0, data[0x61] != 0, data[0x62] != 0, data[0x63] != 0, data[0x74] != 0, data[0x75] != 0, BitConverter.ToInt32(data, 0x88), weapons, passives);
        }

        private static ulong GetGameManager()
        {
            var gmClass = Pointer(gameManagerClassSlot);
            RequireClass(gmClass, "GM", "VampireSurvivors.Framework");
            var statics = Pointer(gmClass + 0xB8);
            var manager = Pointer(statics);
            if (manager == 0)
            {
                return 0;
            }

            RequireObject(manager, "GameManager", "VampireSurvivors.Framework");
            return Pointer(manager + 0x10) == 0 ? 0 : manager;
        }

        public static void GetHealthAndLevel(out float health, out int level)
        {
            var snapshot = ReadSnapshot();
            health = snapshot.Health;
            level = snapshot.Level;
        }

        private static ulong GetActiveCharacter(ulong playerOptions)
        {
            var session = Pointer(playerOptions + 0x30);
            var character = session == 0 ? 0 : Pointer(session + 0x10);
            return character == 0 || Pointer(character + 0x10) == 0 ? 0 : character;
        }

        private static EquippedItem[] ReadEquipment(ulong manager)
        {
            if (manager == 0 || Pointer(manager + 0x10) == 0)
            {
                return[];
            }

            var list = Pointer(manager + 0x28);
            if (list == 0)
            {
                return[];
            }

            var version = Hypervisor.Read<int>(list + 0x1C, true);
            var count = Hypervisor.Read<int>(list + 0x18, true);
            if (count < 0 || count > 256)
            {
                throw new InvalidDataException("Vampire Survivors: invalid equipment list size.");
            }

            var array = Pointer(list + 0x10);
            if (count == 0)
            {
                return[];
            }

            if (array == 0 || (ulong)count > Hypervisor.Read<ulong>(array + 0x18, true))
            {
                throw new InvalidDataException("Vampire Survivors: equipment list is changing.");
            }

            var entries = Hypervisor.ReadArray(array + 0x20, count * 8, true);
            var result = new List<EquippedItem>(count);
            for (var index = 0; index < count; index++)
            {
                var item = BitConverter.ToUInt64(entries, index * 8);
                if (item == 0 || Pointer(item + 0x10) == 0)
                {
                    continue;
                }

                var id = Hypervisor.Read<int>(item + 0x48, true);
                var itemLevel = Hypervisor.Read<int>(item + 0x4C, true);
                result.Add(new EquippedItem(id, itemLevel));
            }

            if (version != Hypervisor.Read<int>(list + 0x1C, true) || array != Pointer(list + 0x10) || count != Hypervisor.Read<int>(list + 0x18, true))
            {
                throw new InvalidDataException("Vampire Survivors: equipment changed during the read; retry next poll.");
            }

            return result.ToArray();
        }

        private static string FormatEquipment(EquippedItem[] items, bool withLevels)
        {
            return items.Length == 0 ? "None" : string.Join(", ", items.Select(item => withLevels ? $"{item.Name} Lv. {item.Level}" : item.Name));
        }

        private static long WholeNumber(float value)
        {
            return float.IsFinite(value) ? (long)Math.Floor(value) : 0;
        }

        private static ulong Pointer(ulong address)
        {
            return address == 0 ? 0 : Hypervisor.Read<ulong>(address, true);
        }

        private static void RequireObject(ulong address, string name, string ns)
        {
            RequireClass(Pointer(address), name, ns);
        }

        private static void RequireClass(ulong address, string name, string ns)
        {
            if (address == 0 || Hypervisor.ReadString(Pointer(address + 0x10), name.Length + 1, true) != name || Hypervisor.ReadString(Pointer(address + 0x18), ns.Length + 1, true) != ns)
            {
                throw new InvalidDataException($"Vampire Survivors: {name} is not initialized or the layout has changed.");
            }
        }

        private static void ResetAddresses()
        {
            cachedProcessId = 0;
            cachedModuleBase = adventureClassSlot = gameManagerClassSlot = 0;
            nextScan = DateTime.MinValue;
        }

        private static void ResolveAddresses()
        {
            var process = Hypervisor.Process ?? throw new InvalidDataException("VS: game is not attached.");
            var module = process.Modules.Cast<ProcessModule>().FirstOrDefault(candidate => string.Equals(candidate.ModuleName, ModuleName, StringComparison.OrdinalIgnoreCase)) ?? throw new InvalidDataException("Vampire Survivors: GameAssembly.dll is not loaded yet.");
            var moduleBase = (ulong)module.BaseAddress.ToInt64();
            if (cachedProcessId != process.Id || cachedModuleBase != moduleBase)
            {
                ResetAddresses();
                cachedProcessId = process.Id;
                cachedModuleBase = moduleBase;
            }

            if (adventureClassSlot != 0 && gameManagerClassSlot != 0)
            {
                return;
            }

            if (DateTime.UtcNow < nextScan)
            {
                throw new InvalidDataException("Vampire Survivors: waiting to retry the manager pattern scan.");
            }

            nextScan = DateTime.UtcNow.AddSeconds(30);
            var match = FindManagerPattern(moduleBase, module.ModuleMemorySize);
            adventureClassSlot = ResolveRipLoad(match);
            gameManagerClassSlot = ResolveRipLoad(match + 0x14);
            if (adventureClassSlot < moduleBase || adventureClassSlot + 8 > moduleBase + (ulong)module.ModuleMemorySize || gameManagerClassSlot < moduleBase || gameManagerClassSlot + 8 > moduleBase + (ulong)module.ModuleMemorySize)
            {
                adventureClassSlot = gameManagerClassSlot = 0;
                throw new InvalidDataException("Vampire Survivors: pattern targets are outside GameAssembly.dll.");
            }
        }

        private static ulong ResolveRipLoad(ulong instruction)
        {
            return checked((ulong)((long)instruction + 7 + Hypervisor.Read<int>(instruction + 3, true)));
        }

        private static ulong FindManagerPattern(ulong moduleBase, int moduleSize)
        {
            var peOffset = Hypervisor.Read<uint>(moduleBase + 0x3C, true);
            var pe = moduleBase + peOffset;
            if (peOffset > (uint)moduleSize - 24 || Hypervisor.Read<uint>(pe, true) != 0x4550)
            {
                throw new InvalidDataException("Vampire Survivors: invalid PE header.");
            }

            var sectionCount = Hypervisor.Read<ushort>(pe + 6, true);
            var optionalSize = Hypervisor.Read<ushort>(pe + 20, true);
            var sectionTable = pe + 24 + optionalSize;
            var pattern = SignaturePattern.Parse(ManagerPattern);
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
                    throw new InvalidDataException("Vampire Survivors: invalid executable section.");
                }

                for (ulong offset = 0; offset < size; offset += chunkSize)
                {
                    var count = (int)Math.Min((ulong)(chunkSize + pattern.Length - 1), size - offset);
                    var bytes = Hypervisor.ReadArray(moduleBase + rva + offset, count, true);
                    var start = 0;
                    while (start <= bytes.Length - pattern.Length)
                    {
                        var found = pattern.IndexOf(bytes.AsSpan(start));
                        if (found < 0)
                        {
                            break;
                        }

                        var address = moduleBase + rva + offset + (ulong)(start + found);
                        if (match != 0 && match != address)
                        {
                            throw new InvalidDataException("Vampire Survivors: manager pattern is ambiguous.");
                        }

                        match = address;
                        start += found + 1;
                    }
                }
            }

            return match != 0 ? match : throw new InvalidDataException("Vampire Survivors: manager pattern was not found.");
        }

        private static void InitializeDiscord()
        {
            if (discord == null)
            {
                throw new InvalidOperationException("VS: presence is not initialized.");
            }

            discord.Initialize();
        }
    }
}
