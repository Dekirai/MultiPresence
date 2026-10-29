using MultiPresence.Runtime;
using DiscordRPC;
using MultiPresence.Models.KH2;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class KH2
    {
        private static DiscordRpcClient? discord;
        private static DiscordStatusUpdater? updater;
        private static readonly Dictionary<string, (string Pattern, int Operand, int Length)> patterns = new Dictionary<string, (string, int, int)>
        {
            { "Area", ("0F B6 05 ?? ?? ?? ?? 89 44 24 38 8B 44 24 38 83 F8 FF 7E ?? 8B 44 24 38 83 F8 13", 3, 7) },
            { "Loaded", ("80 3D ?? ?? ?? ?? 00 74 ?? E8 ?? ?? ?? ?? 84 C0 75 ?? E8 ?? ?? ?? ?? 83 B8 08 01 00 00 00", 2, 7) },
            { "Title", ("48 8D 0D ?? ?? ?? ?? 48 89 05 ?? ?? ?? ?? E8 ?? ?? ?? ?? 33 D2 48 8B CB E8 ?? ?? ?? ??", 10, 14) },
            { "Save", ("48 8D 15 ?? ?? ?? ?? 44 0F B7 4C 42 10 0F B7 4C 42 12 0F B7 54 42 14", 3, 7) },
            { "Stats", ("48 8D 3D ?? ?? ?? ?? BB 50 00 00 00 48 8B CF E8 ?? ?? ?? ?? 48 81 C7 78 02 00 00 48 83 EB 01 75 ??", 3, 7) },
            { "Battle", ("8B 05 ?? ?? ?? ?? 3B C7 75 ?? 83 FF 01 75 ?? F3 0F 10 05 ?? ?? ?? ?? EB ??", 2, 6) }
        };
        private static readonly Dictionary<string, ulong> addresses = new Dictionary<string, ulong>();
        private static Dictionary<string, object>? lastPlaceholders;
        private static int processId;
        private static ulong moduleBase;
        private static DateTime nextScan;

        public static void DoAction()
        {
            ResetAddresses();
            GetPID();
            discord = new DiscordRpcClient("826145131152408625");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/config/Kingdom Hearts II.json");
            PresenceRuntime.Start(nameof(KH2), "KINGDOM HEARTS II FINAL MIX", RPC);
        }

        private static void GetPID()
        {
            try
            {
                ProcessMonitor.TryAttach("KINGDOM HEARTS II FINAL MIX");
            }
            catch
            {
                // Ignore exception
            }
        }

        private static async Task RPC()
        {
            if (discord == null || updater == null)
            {
                throw new InvalidOperationException("KH2: presence is not initialized.");
            }

            if (ProcessMonitor.IsRunning("KINGDOM HEARTS II FINAL MIX"))
            {
                try
                {
                    if (IsMainMenu())
                    {
                        ShowMainMenu();
                        return;
                    }

                    int battleflag = ReadBattleFlag();
                    if (battleflag != 1 && battleflag != 2)
                    {
                        var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                        PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Kingdom Hearts II Final Mix", placeholders);
                    }
                    else if (battleflag == 1)
                    {
                        var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                        PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Kingdom Hearts II Final Mix", placeholders, "FieldBattle");
                    }
                    else if (battleflag == 2)
                    {
                        var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                        PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Kingdom Hearts II Final Mix", placeholders, "BossBattle");
                    }
                }
                catch
                {
                    if (lastPlaceholders == null)
                    {
                        ShowStatus("Loading");
                    }
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

        private static async Task<Dictionary<string, object>> GeneratePlaceholders()
        {
            if (IsMainMenu())
            {
                lastPlaceholders = null;
                throw new InvalidOperationException("KH2 is not in game.");
            }

            if (Hypervisor.Read<byte>(addresses["Loaded"], true) != 1 && lastPlaceholders == null)
            {
                throw new InvalidOperationException("KH2 is loading.");
            }

            int world_get = Hypervisor.Read<byte>(addresses["Area"], true);
            int room_get = Hypervisor.Read<byte>(addresses["Area"] + 1, true);
            var world = await Worlds.GetWorld(world_get);
            var room = await Rooms.GetRoom(world[0]);
            if (world_get <= 1 || world_get == 255 || room_get >= room.Length)
            {
                return lastPlaceholders ?? throw new InvalidOperationException("KH2 location is unavailable.");
            }

            int difficulty_get = Hypervisor.Read<byte>(addresses["Save"] + 0x2498, true);
            int level = Hypervisor.Read<byte>(addresses["Save"] + 0x24FF, true);
            int hp = Hypervisor.Read<int>(addresses["Stats"] + 0xC308, true);
            int hpmax = Hypervisor.Read<int>(addresses["Stats"] + 0xC30C, true);
            int mp = Hypervisor.Read<int>(addresses["Stats"] + 0xC488, true);
            int mpmax = Hypervisor.Read<int>(addresses["Stats"] + 0xC48C, true);
            int form_get = Hypervisor.Read<byte>(addresses["Save"] + 0x3524, true);

            var difficulty = await Difficulties.GetDifficulty(difficulty_get);
            string form = "";

            if (form_get == 0)
                form = "None";
            else if (form_get == 1)
                form = "Valor";
            else if (form_get == 2)
                form = "Wisdom";
            else if (form_get == 3)
                form = "Limit";
            else if (form_get == 4)
                form = "Master";
            else if (form_get == 5)
                form = "Final";
            else if (form_get == 6)
                form = "Anti";

            lastPlaceholders = new Dictionary<string, object>
            {
                { "level", level },
                { "hp", hp },
                { "hpmax", hpmax },
                { "mp", mp },
                { "mpmax", mpmax },
                { "form", form },
                { "room", room[room_get] },
                { "world", world[0] },
                { "world_icon_name", world[1] },
                { "difficulty", difficulty }
            };
            return lastPlaceholders;
        }

        private static bool IsMainMenu()
        {
            ResolveAddresses();
            return Hypervisor.Read<ulong>(addresses["Title"], true) != 0;
        }

        private static void ShowMainMenu()
        {
            lastPlaceholders = null;
            ShowStatus("In Main Menu");
        }

        private static void ShowStatus(string details)
        {
            if (discord == null)
            {
                throw new InvalidOperationException("KH2: presence is not initialized.");
            }

            discord.SetPresence(new RichPresence()
            {
                Details = details,
                State = "",
                Assets = new Assets()
                {
                    LargeImageKey = "logo",
                    LargeImageText = "Kingdom Hearts II Final Mix"
                },
                Timestamps = PlaceholderHelper._startTimestamp
            });
        }

        private static int ReadBattleFlag()
        {
            ResolveAddresses();
            return Hypervisor.Read<byte>(addresses["Battle"], true);
        }

        private static void ResetAddresses()
        {
            addresses.Clear();
            lastPlaceholders = null;
            processId = 0;
            moduleBase = 0;
            nextScan = DateTime.MinValue;
        }

        private static void ResolveAddresses()
        {
            var process = Hypervisor.Process ?? throw new InvalidDataException("KH2: game is not attached.");
            var currentBase = Hypervisor.PureAddress;
            if (process.Id != processId || currentBase != moduleBase)
            {
                ResetAddresses();
                processId = process.Id;
                moduleBase = currentBase;
            }

            if (addresses.Count == patterns.Count)
            {
                return;
            }

            if (DateTime.UtcNow < nextScan)
            {
                throw new InvalidDataException("KH2 patterns are unavailable.");
            }

            nextScan = DateTime.UtcNow.AddSeconds(30);
            var moduleSize = (ulong)(process.MainModule ?? throw new InvalidDataException("KH2: module is not loaded.")).ModuleMemorySize;
            var peOffset = Hypervisor.Read<uint>(moduleBase + 0x3C, true);
            if (peOffset > moduleSize - 24 || Hypervisor.Read<uint>(moduleBase + peOffset, true) != 0x4550)
            {
                throw new InvalidDataException("Invalid KH2 module header.");
            }

            var pe = moduleBase + peOffset;
            var count = Hypervisor.Read<ushort>(pe + 6, true);
            var table = pe + 24 + Hypervisor.Read<ushort>(pe + 20, true);
            if (count > 96 || table + (ulong)count * 40 > moduleBase + moduleSize)
            {
                throw new InvalidDataException("Invalid KH2 section table.");
            }

            var signatures = patterns.ToDictionary(item => item.Key, item => SignaturePattern.Parse(item.Value.Pattern));
            var matches = new Dictionary<string, ulong>();
            for (var section = 0; section < count; section++)
            {
                var header = table + (ulong)section * 40;
                var size = Hypervisor.Read<uint>(header + 8, true);
                var rva = Hypervisor.Read<uint>(header + 12, true);
                if ((Hypervisor.Read<uint>(header + 36, true) & 0x20000000) == 0 || size == 0)
                {
                    continue;
                }

                if ((ulong)rva + size > moduleSize || size > int.MaxValue)
                {
                    throw new InvalidDataException("Invalid KH2 executable section.");
                }

                var data = Hypervisor.ReadArray(moduleBase + rva, (int)size, true);
                foreach (var signature in signatures)
                {
                    var found = signature.Value.IndexOf(data);
                    if (found < 0)
                    {
                        continue;
                    }

                    if (matches.ContainsKey(signature.Key) || signature.Value.IndexOf(data.AsSpan(found + 1)) >= 0)
                    {
                        throw new InvalidDataException("Ambiguous KH2 pattern: " + signature.Key);
                    }

                    var pattern = patterns[signature.Key];
                    var instruction = moduleBase + rva + (ulong)found;
                    var target = (ulong)((long)instruction + pattern.Length + BitConverter.ToInt32(data, found + pattern.Operand));
                    var length = signature.Key == "Stats" ? 0xC490UL : signature.Key == "Save" ? 0x3525UL : signature.Key == "Title" ? 8UL : 2UL;
                    if (target < moduleBase || target > moduleBase + moduleSize - length)
                    {
                        throw new InvalidDataException("Invalid KH2 pattern target: " + signature.Key);
                    }

                    matches.Add(signature.Key, target);
                }
            }

            if (matches.Count != patterns.Count)
            {
                throw new InvalidDataException("KH2 pattern not found.");
            }

            foreach (var match in matches)
            {
                addresses.Add(match.Key, match.Value);
            }
        }

        private static void InitializeDiscord()
        {
            if (discord == null)
            {
                throw new InvalidOperationException("KH2: presence is not initialized.");
            }

            discord.Initialize();
        }
    }
}
