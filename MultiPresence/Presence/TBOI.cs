using MultiPresence.Runtime;
using MultiPresence.Infrastructure;
using DiscordRPC;

namespace MultiPresence.Presence
{
    public class TBOI
    {
        private const string ProcessName = "isaac-ng";
        private const string GamePattern = "8B 0D ?? ?? ?? ?? 85 C9 0F 84 ?? ?? ?? ?? 8B 81 AC BA 01 00 2B 81 A8 BA 01 00 C1 F8 02 85 C0";
        private const string SavePattern = "A1 ?? ?? ?? ?? 68 ?? ?? ?? ?? 8B B0 24 03 00 00 8D B8 ?? ?? ?? ?? 05 ?? ?? ?? ?? 50";
        private const string ScorePattern = "A1 ?? ?? ?? ?? FF B0 ?? ?? ?? ?? 8D 45 A8 68 ?? ?? ?? ?? 6A 40 50 E8 ?? ?? ?? ?? 83 C4 10";
        private const string TimePattern = "8B 81 F8 64 02 00 C3";
        private const string NamePattern = "83 B9 D8 13 00 00 10 8D 81 C4 13 00 00 72 02 8B 00 C3";
        private const string HeartsPattern = "8B 81 88 1D 00 00 C3 CC CC CC CC CC CC CC CC CC 8B 81 A4 1D 00 00 C3";
        private static DiscordRpcClient? discord;
        private static DiscordStatusUpdater? updater;
        private static int cachedProcessId;
        private static ulong cachedModuleBase, gameSlot, saveSlot;
        private static uint scoreOffset;
        private static DateTime nextScan;
        public static void DoAction()
        {
            ResetAddresses();
            GetPID();
            discord = new DiscordRpcClient("1378819491902918758");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/config/The Binding of Isaac Rebirth.json");
            PresenceRuntime.Start(nameof(TBOI), ProcessName, RPC);
        }

        private static void GetPID()
        {
            ProcessMonitor.TryAttach(ProcessName);
        }

        private static async Task RPC()
        {
            if (discord == null || updater == null)
            {
                throw new InvalidOperationException("TBOI: presence is not initialized.");
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
                    if (!snapshot.IsInGame)
                    {
                        ShowMenu();
                        return;
                    }

                    var placeholders = await PlaceholderHelper.GetPlaceholders(() => GeneratePlaceholders(snapshot));
                    PlaceholderHelper.UpdateDiscordStatus(discord, updater, "The Binding of Isaac: Rebirth", placeholders);
                }
                catch (Exception exception) when (exception is not OutOfMemoryException and not StackOverflowException)
                {
                    AppLog.Warning("Binding of Isaac memory is not ready; showing the main menu.", exception);
                    ShowMenu();
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
            var character = snapshot.CharacterName;
            var characterAsset = $"char_{character.ToLowerInvariant().Replace(" ", "")}";
            if (snapshot.CharacterId is 19 or 20)
            {
                character = "Jacob & Esau";
            }
            else if (snapshot.CharacterId == 17)
            {
                character = "The Forgotten";
            }

            if (snapshot.CharacterId is >= 21 and <= 40 && !character.StartsWith("Tainted ", StringComparison.Ordinal))
            {
                character = "Tainted " + character;
            }

            var floor = DescribeFloor(snapshot.Floor, snapshot.StageType, snapshot.Difficulty, snapshot.Curses);
            var time = TimeSpan.FromSeconds(Math.Max(0, snapshot.Ticks) / 30);
            return Task.FromResult(new Dictionary<string, object> { { "floor", floor.Name }, { "floortype", floor.Type }, { "floortypeasset", floor.Asset }, { "floornumber", snapshot.Floor }, { "stagetype", snapshot.StageType }, { "difficulty", snapshot.Difficulty }, { "time", $"{(int)time.TotalHours:D2}:{time.Minutes:D2}:{time.Seconds:D2}" }, { "score", snapshot.Score }, { "character", character }, { "characterid", snapshot.CharacterId }, { "characterasset", characterAsset }, { "keys", snapshot.Keys }, { "bombs", snapshot.Bombs }, { "coins", snapshot.Coins }, { "maxredhearts", snapshot.MaxRedHearts }, { "maxreadhearts", snapshot.MaxRedHearts }, { "redhearts", snapshot.RedHearts }, { "whitehearts", snapshot.WhiteHearts }, { "bluehearts", snapshot.BlueHearts }, { "blackheartsmask", snapshot.BlackHeartsMask }, { "bonehearts", snapshot.BoneHearts }, { "boneheartsmask", snapshot.BoneHeartsMask }, { "rottenhearts", snapshot.RottenHearts }, { "goldenhearts", snapshot.GoldenHearts }, { "winstreak", snapshot.WinStreak }, { "edentokens", snapshot.EdenTokens } });
        }

        private static void ShowMenu()
        {
            if (discord == null)
            {
                throw new InvalidOperationException("TBOI: presence is not initialized.");
            }

            discord.SetPresence(new RichPresence()
            {
                Details = "In Main Menu",
                State = "",
                Assets = new Assets()
                {
                    LargeImageKey = "logo",
                    LargeImageText = "The Binding of Isaac: Rebirth"
                },
                Timestamps = PlaceholderHelper._startTimestamp
            });
        }

        private sealed record Snapshot(bool IsInGame, int Floor, int StageType, int Curses, int Difficulty, int Ticks, int Score, int CharacterId, string CharacterName, int Keys, int Bombs, int Coins, int MaxRedHearts, int RedHearts, int WhiteHearts, int BlueHearts, uint BlackHeartsMask, int BoneHearts, uint BoneHeartsMask, int RottenHearts, int GoldenHearts, int WinStreak, int EdenTokens);
        private static Snapshot MenuSnapshot(int streak, int tokens)
        {
            return new Snapshot(false, 0, 0, 0, 0, 0, 0, -1, "", 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, streak, tokens);
        }

        private static Snapshot ReadSnapshot()
        {
            ResolveAddresses();
            var save = Pointer(saveSlot);
            var streak = save == 0 ? 0 : ReadInt(save + 0x328);
            var tokens = save == 0 ? 0 : ReadInt(save + 0x324);
            var game = Pointer(gameSlot);
            if (game == 0)
            {
                return MenuSnapshot(streak, tokens);
            }

            var floor = ReadInt(game);
            var begin = Pointer(game + 0x1BAA8);
            var end = Pointer(game + 0x1BAAC);
            if (floor == 0 || begin == end)
            {
                return MenuSnapshot(streak, tokens);
            }

            if (floor < 0 || floor > 13 || begin == 0 || end < begin || (end - begin) % 4 != 0 || end - begin > 256 * 4)
            {
                throw new InvalidDataException("Binding of Isaac: invalid stage or player vector.");
            }

            var player = Pointer(begin);
            if (player == 0)
            {
                return MenuSnapshot(streak, tokens);
            }

            var stats = Hypervisor.ReadArray(player + 0x1340, 0x98, true);
            int Stat(int offset)
            {
                return BitConverter.ToInt32(stats, offset);
            }

            var snapshot = new Snapshot(true, floor, ReadInt(game + 4), ReadInt(game + 0xC), ReadInt(game + 0x269C8), ReadInt(game + 0x264F8), ReadInt(game + scoreOffset), Stat(0x80), ReadPlayerName(player), Stat(0x1C), Stat(0x24), Stat(0x28), Stat(0), Stat(4), Stat(8), Stat(0xC), BitConverter.ToUInt32(stats, 0x10), ReadInt(player + 0x1D88), Hypervisor.Read<uint>(player + 0x1D8C, true), ReadInt(player + 0x1DA4), ReadInt(player + 0x194C), streak, tokens);
            if (game != Pointer(gameSlot) || begin != Pointer(game + 0x1BAA8) || end != Pointer(game + 0x1BAAC) || player != Pointer(begin) || floor != ReadInt(game))
            {
                throw new InvalidDataException("Binding of Isaac: scene changed during the read; retry next poll.");
            }

            return snapshot;
        }

        private static string ReadPlayerName(ulong player)
        {
            var text = player + 0x13C4;
            var length = Hypervisor.Read<uint>(text + 0x10, true);
            var capacity = Hypervisor.Read<uint>(text + 0x14, true);
            if (length == 0 || length > 256 || capacity < length)
            {
                throw new InvalidDataException("Binding of Isaac: invalid player name.");
            }

            var address = capacity >= 16 ? Pointer(text) : text;
            if (address == 0)
            {
                throw new InvalidDataException("Binding of Isaac: missing player name.");
            }

            return Hypervisor.ReadString(address, (int)length + 1, true);
        }

        private static (string Name, string Type, string Asset) DescribeFloor(int stage, int type, int difficulty, int curses)
        {
            var greed = difficulty is 2 or 3;
            if (greed && stage >= 5)
            {
                var special = stage switch
                {
                    5 => ("Sheol", "floor_sheol"),
                    6 => ("The Shop", "logo"),
                    7 => ("Ultra Greed", "logo"),
                    _ => ($"Stage {stage}", "logo"),
                };
                return (special.Item1, special.Item1, special.Item2);
            }

            var normalStage = greed ? stage * 2 - 1 : stage;
            var variant = type switch
            {
                0 => 0,
                1 => 1,
                2 => 2,
                4 => 3,
                5 => 4,
                _ => -1,
            };
            string name, asset;
            if (normalStage is >= 1 and <= 8 && variant >= 0)
            {
                string[][] names = [["Basement", "Cellar", "Burning Basement", "Downpour", "Dross"], ["Caves", "Catacombs", "Flooded Caves", "Mines", "Ashpit"], ["Depths", "Necropolis", "Dank Depths", "Mausoleum", "Gehenna"], ["Womb", "Utero", "Scarred Womb", "Corpse", "Corpse"]];
                string[][] assets = [["floor_basement", "floor_cellar", "floor_burningbasement", "floor_downpour", "floor_dross"], ["floor_caves", "floor_caves", "floor_caves", "logo", "logo"], ["floor_depths", "floor_necropolis", "floor_dankdepths", "floor_mausoleum", "floor_gehenna"], ["floor_womb", "floor_womb", "floor_scarredwomb", "floor_mother", "floor_mother"]];
                var chapter = (normalStage - 1) / 2;
                name = names[chapter][variant];
                asset = assets[chapter][variant];
                var suffix = greed ? "" : (curses & 2) != 0 ? " XL" : normalStage % 2 == 1 ? " I" : " II";
                return (name + suffix, name, asset);
            }

            (name, asset) = normalStage switch
            {
                9 => ("Blue Womb", "floor_hush"),
                10 when type == 1 => ("Cathedral", "floor_cathedral"),
                10 => ("Sheol", "floor_sheol"),
                11 when type == 1 => ("Chest", "floor_chest"),
                11 => ("Dark Room", "floor_darkroom"),
                12 => ("The Void", "floor_void"),
                13 => ("Home", "floor_home"),
                _ => ($"Stage {stage} (type {type})", "logo"),
            };
            return (name, name, asset);
        }

        private static int ReadInt(ulong address)
        {
            return Hypervisor.Read<int>(address, true);
        }

        private static ulong Pointer(ulong address)
        {
            return Hypervisor.Read<uint>(address, true);
        }

        private static void ResetAddresses()
        {
            cachedProcessId = 0;
            cachedModuleBase = gameSlot = saveSlot = 0;
            scoreOffset = 0;
            nextScan = DateTime.MinValue;
        }

        private static void ResolveAddresses()
        {
            var process = Hypervisor.Process ?? throw new InvalidDataException("TBOI: game is not attached.");
            var module = process.MainModule ?? throw new InvalidDataException("Binding of Isaac: module not loaded.");
            var moduleBase = (ulong)module.BaseAddress.ToInt64();
            if (cachedProcessId != process.Id || cachedModuleBase != moduleBase)
            {
                ResetAddresses();
                cachedProcessId = process.Id;
                cachedModuleBase = moduleBase;
            }

            if (gameSlot != 0 && saveSlot != 0)
            {
                return;
            }

            if (DateTime.UtcNow < nextScan)
            {
                throw new InvalidDataException("Binding of Isaac: waiting to retry patterns.");
            }

            nextScan = DateTime.UtcNow.AddSeconds(30);
            var gameMatch = FindUniquePattern(moduleBase, module.ModuleMemorySize, GamePattern);
            var saveMatch = FindUniquePattern(moduleBase, module.ModuleMemorySize, SavePattern);
            var scoreMatch = FindUniquePattern(moduleBase, module.ModuleMemorySize, ScorePattern);
            FindUniquePattern(moduleBase, module.ModuleMemorySize, TimePattern);
            FindUniquePattern(moduleBase, module.ModuleMemorySize, NamePattern);
            FindUniquePattern(moduleBase, module.ModuleMemorySize, HeartsPattern);
            var game = Pointer(gameMatch + 2);
            var save = Pointer(saveMatch + 1);
            var scoreGame = Pointer(scoreMatch + 1);
            var offset = Hypervisor.Read<uint>(scoreMatch + 7, true);
            if (game != scoreGame || game < moduleBase || save < moduleBase || game + 4 > moduleBase + (ulong)module.ModuleMemorySize || save + 4 > moduleBase + (ulong)module.ModuleMemorySize || offset != 0x26718)
            {
                throw new InvalidDataException("Binding of Isaac: pattern targets or object layout changed.");
            }

            gameSlot = game;
            saveSlot = save;
            scoreOffset = offset;
        }

        private static ulong FindUniquePattern(ulong moduleBase, int moduleSize, string signature)
        {
            if (moduleSize < 64)
            {
                throw new InvalidDataException("Binding of Isaac: invalid module size.");
            }

            var peOffset = Hypervisor.Read<uint>(moduleBase + 0x3C, true);
            if (peOffset > (uint)moduleSize - 24)
            {
                throw new InvalidDataException("Binding of Isaac: invalid PE offset.");
            }

            var pe = moduleBase + peOffset;
            if (peOffset > (uint)moduleSize - 24 || Hypervisor.Read<uint>(pe, true) != 0x4550 || Hypervisor.Read<ushort>(pe + 4, true) != 0x14C)
            {
                throw new InvalidDataException("Binding of Isaac: invalid PE header.");
            }

            var sectionCount = Hypervisor.Read<ushort>(pe + 6, true);
            var optionalSize = Hypervisor.Read<ushort>(pe + 20, true);
            var sectionTable = pe + 24 + optionalSize;
            if (sectionTable + (ulong)sectionCount * 40 > moduleBase + (ulong)moduleSize)
            {
                throw new InvalidDataException("Binding of Isaac: invalid section table.");
            }

            var pattern = SignaturePattern.Parse(signature);
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
                    throw new InvalidDataException("Binding of Isaac: invalid executable section.");
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
                            throw new InvalidDataException("Binding of Isaac: manager pattern is ambiguous.");
                        }

                        match = address;
                        start += found + 1;
                    }
                }
            }

            return match != 0 ? match : throw new InvalidDataException("Binding of Isaac: manager pattern was not found.");
        }

        private static void InitializeDiscord()
        {
            if (discord == null)
            {
                throw new InvalidOperationException("TBOI: presence is not initialized.");
            }

            discord.Initialize();
        }
    }
}
