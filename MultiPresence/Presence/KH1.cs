using MultiPresence.Runtime;
using DiscordRPC;
using MultiPresence.Models.KH1;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class KH1
    {
        private static DiscordRpcClient? discord;
        private static DiscordStatusUpdater? updater;
        private static readonly Dictionary<string, (string Pattern, int Operand, int Length)> patterns = new Dictionary<string, (string, int, int)>
        {
            { "Area", ("8B 0D ?? ?? ?? ?? 89 48 44 48 8B 0D ?? ?? ?? ?? 8B 05 ?? ?? ?? ?? 89 41 48 E8 ?? ?? ?? ?? 48 8B 0D ?? ?? ?? ?? 0F 10 00", 2, 6) },
            { "Mode", ("48 63 05 ?? ?? ?? ?? 83 F8 12 0F 87 ?? ?? ?? ??", 3, 7) },
            { "Save", ("48 8D 3D ?? ?? ?? ?? 33 C0 B9 80 2D 00 00 F3 48 AB 48 8B 7C 24 08 C3", 3, 7) },
            { "Stats", ("48 8D 05 ?? ?? ?? ?? 45 33 D2 41 B9 01 00 00 00 41 8B D2 41 8B C9 0F 1F 00 41 85 C8 74 ??", 3, 7) },
            { "Battle", ("F6 05 ?? ?? ?? ?? 01 8B DA 48 8B F9 74 ?? 48 85 C9 74 ?? E8 ?? ?? ?? ?? 85 C0 74 ??", 2, 7) }
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
            discord = new DiscordRpcClient("827214883190734889");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/config/Kingdom Hearts I.json");
            PresenceRuntime.Start(nameof(KH1), "KINGDOM HEARTS FINAL MIX", RPC);
        }

        private static void GetPID()
        {
            try
            {
                ProcessMonitor.TryAttach("KINGDOM HEARTS FINAL MIX");
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
                throw new InvalidOperationException("KH1: presence is not initialized.");
            }

            if (ProcessMonitor.IsRunning("KINGDOM HEARTS FINAL MIX"))
            {
                try
                {
                    if (IsMainMenu())
                    {
                        ShowMainMenu();
                        return;
                    }

                    int battleflag = ReadBattleFlag();
                    if (battleflag != 1)
                    {
                        var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                        PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Kingdom Hearts Final Mix", placeholders);
                    }
                    else if (battleflag == 1)
                    {
                        var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                        PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Kingdom Hearts Final Mix", placeholders, "Battle");
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
            ResolveAddresses();
            int mode = Hypervisor.Read<int>(addresses["Mode"], true);
            if (mode == 2 || mode == 3)
            {
                lastPlaceholders = null;
                throw new InvalidOperationException("KH1 is not in game.");
            }

            if (mode != 5 && mode != 7 && mode != 9 && lastPlaceholders == null)
            {
                throw new InvalidOperationException("KH1 is loading.");
            }

            var area = Hypervisor.ReadArray(addresses["Area"], 12, true);
            int world_get = BitConverter.ToInt32(area, 8);
            int room_get = BitConverter.ToInt32(area, 0);
            var world = await Worlds.GetWorld(world_get);
            var room = await Rooms.GetRoom(world[0]);
            if (world_get < 0 || world_get == 255 || room_get < 0 || room_get >= room.Length)
            {
                return lastPlaceholders ?? throw new InvalidOperationException("KH1 location is unavailable.");
            }

            int difficulty_get = Hypervisor.Read<byte>(addresses["Save"] + 0x1642C, true);
            int level = Hypervisor.Read<byte>(addresses["Save"] + 4, true);
            int hp = Hypervisor.Read<int>(addresses["Stats"] + 0x3C, true);
            int hpmax = Hypervisor.Read<int>(addresses["Stats"] + 0x40, true);
            int mp = Hypervisor.Read<int>(addresses["Stats"] + 0x44, true);
            int mpmax = Hypervisor.Read<int>(addresses["Stats"] + 0x48, true);
            var difficulty = await Difficulties.GetDifficulty(difficulty_get);

            lastPlaceholders = new Dictionary<string, object>
            {
                { "level", level },
                { "hp", hp },
                { "hpmax", hpmax },
                { "mp", mp },
                { "mpmax", mpmax },
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
            int mode = Hypervisor.Read<int>(addresses["Mode"], true);
            return mode == 2 || mode == 3;
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
                throw new InvalidOperationException("KH1: presence is not initialized.");
            }

            discord.SetPresence(new RichPresence()
            {
                Details = details,
                State = "",
                Assets = new Assets()
                {
                    LargeImageKey = "logo",
                    LargeImageText = "Kingdom Hearts Final Mix"
                },
                Timestamps = PlaceholderHelper._startTimestamp
            });
        }

        private static int ReadBattleFlag()
        {
            ResolveAddresses();
            return Hypervisor.Read<byte>(addresses["Battle"], true) & 1;
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
            var process = Hypervisor.Process ?? throw new InvalidDataException("KH1: game is not attached.");
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
                throw new InvalidDataException("KH1 patterns are unavailable.");
            }

            nextScan = DateTime.UtcNow.AddSeconds(30);
            var moduleSize = (ulong)(process.MainModule ?? throw new InvalidDataException("KH1: module is not loaded.")).ModuleMemorySize;
            var peOffset = Hypervisor.Read<uint>(moduleBase + 0x3C, true);
            if (peOffset > moduleSize - 24 || Hypervisor.Read<uint>(moduleBase + peOffset, true) != 0x4550)
            {
                throw new InvalidDataException("Invalid KH1 module header.");
            }

            var pe = moduleBase + peOffset;
            var count = Hypervisor.Read<ushort>(pe + 6, true);
            var table = pe + 24 + Hypervisor.Read<ushort>(pe + 20, true);
            if (count > 96 || table + (ulong)count * 40 > moduleBase + moduleSize)
            {
                throw new InvalidDataException("Invalid KH1 section table.");
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
                    throw new InvalidDataException("Invalid KH1 executable section.");
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
                        throw new InvalidDataException("Ambiguous KH1 pattern: " + signature.Key);
                    }

                    var pattern = patterns[signature.Key];
                    var instruction = moduleBase + rva + (ulong)found;
                    var target = (ulong)((long)instruction + pattern.Length + BitConverter.ToInt32(data, found + pattern.Operand));
                    var length = signature.Key == "Stats" ? 0x4CUL : signature.Key == "Save" ? 0x1642DUL : signature.Key == "Area" ? 12UL : 4UL;
                    if (target < moduleBase || target > moduleBase + moduleSize - length)
                    {
                        throw new InvalidDataException("Invalid KH1 pattern target: " + signature.Key);
                    }

                    matches.Add(signature.Key, target);
                }
            }

            if (matches.Count != patterns.Count)
            {
                throw new InvalidDataException("KH1 pattern not found.");
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
                throw new InvalidOperationException("KH1: presence is not initialized.");
            }

            discord.Initialize();
        }
    }
}
