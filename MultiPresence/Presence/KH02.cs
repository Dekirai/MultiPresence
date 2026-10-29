using MultiPresence.Runtime;
using DiscordRPC;
using MultiPresence.Models.KH02;
using MultiPresence.Infrastructure;
using System.Globalization;
using System.Text;

namespace MultiPresence.Presence
{
    public class KH02
    {
        private static DiscordRpcClient? discord;
        private static DiscordStatusUpdater? updater;
        private const string ProcessName = "KINGDOM HEARTS 0.2 Birth by Sleep";
        private const string GameName = "Kingdom Hearts 0.2 Birth by Sleep";
        private static readonly Dictionary<string, (string Pattern, int Operand, int Length)> Patterns = new Dictionary<string, (string Pattern, int Operand, int Length)>
        {
            { "World", ("48 8B 05 ?? ?? ?? ?? 48 0F 45 C1", 3, 7) },
            { "Names", ("48 8B 05 ?? ?? ?? ?? 48 85 C0 75 ?? B9 08 04 00 00", 3, 7) },
            { "NameEntryLayout", ("F6 01 01 48 8D 71 10 48 8B FA 74 ?? 48 C7 C3 FF FF FF FF 48 FF C3 66 83 3C 5E 00 75 ??", 0, 0) },
            { "NameIndexLayout", ("8B 03 99 81 E2 FF 3F 00 00 03 C2 44 8B C0 25 FF 3F 00 00 2B C2 48 63 C8 41 C1 F8 0E 49 63 C0 49 8B 04 C1 48 8B 04 C8", 0, 0) },
            { "WorldInstanceLayout", ("48 8B 81 58 01 00 00 C3", 0, 0) },
            { "LocalPlayersLayout", ("48 8B 48 50 48 63 40 58 48 8D 14 C1 48 3B CA 74 ?? 0F 1F 80 00 00 00 00 48 8B 01 48 39 78 48 74 ??", 0, 0) },
            { "AcknowledgedPawnLayout", ("48 8B 81 58 04 00 00 C3", 0, 0) },
            { "ControllerPawnLayout", ("48 8B 81 E8 03 00 00 C3", 0, 0) },
            { "HealthGetter", ("8B 81 DC 05 00 00 C3", 0, 0) },
            { "MagicGetter", ("8B 81 E4 05 00 00 C3", 0, 0) },
            { "LevelGetter", ("48 8B 81 C8 04 00 00 48 85 C0 74 ?? 8B 40 40 C3 C3", 0, 0) },
            { "MaxHealthGetter", ("48 8B 81 C8 04 00 00 48 85 C0 74 ?? 8B 40 50 C3 C3", 0, 0) },
            { "MaxMagicGetter", ("48 8B 81 C8 04 00 00 48 85 C0 74 ?? 8B 40 54 C3 C3", 0, 0) },
            { "CurrentAreaLayout", ("48 8D 81 38 03 00 00 C3 CC CC CC CC CC CC CC CC", 0, 0) },
            { "CurrentAreaWriteLayout", ("E8 ?? ?? ?? ?? 49 8B 8D 30 04 00 00 48 89 08 48 8B CD E8 ?? ?? ?? ?? 8B 4C 24 70 89 48 08", 0, 0) },
            { "DifficultyLayout", ("8B 81 38 05 00 00 48 8B FA 89 02 48 8B E9 E8 ?? ?? ?? ?? F3 0F 11 47 04", 0, 0) },
            { "DifficultySelectionLayout", ("E8 ?? ?? ?? ?? 48 8B 9F 90 04 00 00 8B 88 00 02 00 00 85 C9 0F 84 ?? ?? ?? ??", 0, 0) },
            { "LevelTransitionLayout", ("40 53 48 83 EC 40 80 B9 B2 02 00 00 00 48 8B D9 74 ?? B0 01 48 83 C4 40", 0, 0) }
        };
        private static Dictionary<string, ulong> addresses = new Dictionary<string, ulong>();
        private static int cachedProcessId, cachedModuleSize;
        private static ulong cachedModuleBase;
        private static DateTime nextScan;
        public static Task DoAction()
        {
            ResetAddresses();
            GetPID();
            discord = new DiscordRpcClient("1562380887138631770");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/Config/Kingdom Hearts 0.2 Birth by Sleep.json");
            PresenceRuntime.Start(nameof(KH02), ProcessName, RPC);
            return Task.CompletedTask;
        }

        private static void GetPID()
        {
            ProcessMonitor.TryAttach(ProcessName);
        }

        private static async Task RPC()
        {
            if (discord == null || updater == null)
            {
                throw new InvalidOperationException("KH02: presence is not initialized.");
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
                        ShowStatus("In Main Menu / Loading");
                        return;
                    }

                    var placeholders = await PlaceholderHelper.GetPlaceholders(() => GeneratePlaceholders(snapshot));
                    PlaceholderHelper.UpdateDiscordStatus(discord, updater, GameName, placeholders, "In_World");
                }
                catch (Exception exception) when (exception is not OutOfMemoryException and not StackOverflowException)
                {
                    AppLog.Warning("Kingdom Hearts 0.2 memory is not ready.", exception);
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
                return Task.FromResult(new Dictionary<string, object>());
            }

            var token = WorldToken(snapshot.MapKey);
            var matches = Locations.IsCurrentArea(snapshot.AreaKey, snapshot.MapKey);
            var location = Locations.GetLocation(snapshot.AreaKey, snapshot.MapKey);
            return Task.FromResult(new Dictionary<string, object> { { "state", "In Game" }, { "character", "Aqua" }, { "level", snapshot.Level }, { "hp", snapshot.Hp }, { "maxhp", snapshot.MaxHp }, { "mp", snapshot.Mp }, { "maxmp", snapshot.MaxMp }, { "world", Locations.GetWorld(token) }, { "world_icon_name", Locations.GetWorldIcon(token) }, { "location", location }, { "room", location }, { "map", Locations.GetMapForArea(snapshot.AreaKey, snapshot.MapKey) }, { "area", matches ? Locations.GetArea(snapshot.AreaKey) : "" }, { "subarea", matches ? Locations.GetSubArea(snapshot.AreaKey) : "" }, { "worldid", token }, { "mapid", snapshot.MapKey }, { "areaid", matches ? snapshot.AreaKey : "" }, { "difficulty", Difficulties.GetName(snapshot.Difficulty) }, { "difficultyid", snapshot.Difficulty } });
        }

        private static void ShowStatus(string text)
        {
            if (discord == null)
            {
                throw new InvalidOperationException("KH02: presence is not initialized.");
            }

            discord.SetPresence(new RichPresence()
            {
                Details = text,
                State = "",
                Assets = new Assets()
                {
                    LargeImageKey = "logo",
                    LargeImageText = GameName
                },
                Timestamps = PlaceholderHelper._startTimestamp
            });
        }

        private sealed record Snapshot(bool IsInGame, string MapKey, string AreaKey, int Difficulty, int Level, int Hp, int MaxHp, int Mp, int MaxMp);
        private static Snapshot EmptySnapshot()
        {
            return new Snapshot(false, "", "", 0, 0, 0, 0, 0, 0);
        }

        private static Snapshot ReadSnapshot()
        {
            ResolveAddresses();
            var world = Pointer(addresses["World"]);
            var names = Pointer(addresses["Names"]);
            if (world == 0 || names == 0)
            {
                return EmptySnapshot();
            }

            var mapPacked = Pointer(world + 0x20);
            var map = ReadName(names, mapPacked);
            if (WorldToken(map).Length == 0)
            {
                return EmptySnapshot();
            }

            var instance = Pointer(world + 0x158);
            if (instance == 0 || Hypervisor.Read<byte>(instance + 0x2B2, true) != 0)
            {
                return EmptySnapshot();
            }

            var player = ReadPlayer(world, names);
            if (player == null || player.Instance != instance)
            {
                return EmptySnapshot();
            }

            var areaPacked = Pointer(instance + 0x338);
            var area = ReadName(names, areaPacked);
            var difficulty = ReadInt(instance + 0x538);
            if (difficulty is < 0 or > 3)
            {
                throw new InvalidDataException("KH0.2: invalid difficulty.");
            }

            if (world != Pointer(addresses["World"]) || names != Pointer(addresses["Names"]) || mapPacked != Pointer(world + 0x20) || instance != Pointer(world + 0x158) || Hypervisor.Read<byte>(instance + 0x2B2, true) != 0 || areaPacked != Pointer(instance + 0x338) || difficulty != ReadInt(instance + 0x538) || player != ReadPlayer(world, names))
            {
                throw new InvalidDataException("KH0.2: scene changed during the read; retry next poll.");
            }

            return new Snapshot(true, map, area, difficulty, player.Level, player.Hp, player.MaxHp, player.Mp, player.MaxMp);
        }

        private static string WorldToken(string map)
        {
            if (map.Length < 5 || map[2] != '_' || !char.IsAsciiLetterLower(map[0]) || !char.IsAsciiLetterLower(map[1]) || !char.IsAsciiDigit(map[3]) || !char.IsAsciiDigit(map[4]))
            {
                return "";
            }

            return map[..2];
        }

        private sealed record PlayerState(ulong World, ulong Instance, ulong Players, int PlayerCount, int PlayerCapacity, ulong LocalPlayer, ulong Controller, ulong Pawn, ulong PawnVtable, ulong Param, string PawnName, int Level, int Hp, int MaxHp, int Mp, int MaxMp);
        private static PlayerState? ReadPlayer(ulong world, ulong names)
        {
            if (world == 0 || names == 0)
            {
                return null;
            }

            var instance = Pointer(world + 0x158);
            if (instance == 0)
            {
                return null;
            }

            var players = Pointer(instance + 0x50);
            int count = ReadInt(instance + 0x58), capacity = ReadInt(instance + 0x5C);
            if (count < 0 || count > capacity || capacity < 0 || capacity > 8)
            {
                throw new InvalidDataException("KH0.2: invalid local-player array.");
            }

            if (players == 0 || count == 0)
            {
                return null;
            }

            var localPlayer = Pointer(players);
            var controller = localPlayer == 0 ? 0 : Pointer(localPlayer + 0x48);
            if (controller == 0)
            {
                return null;
            }

            var pawn = Pointer(controller + 0x3E8);
            if (pawn == 0 || Pointer(controller + 0x458) != pawn)
            {
                return null;
            }

            var vtable = Pointer(pawn);
            if (vtable == 0 || Pointer(vtable + 0x1430) != addresses["HealthGetter"] || Pointer(vtable + 0x1448) != addresses["MagicGetter"] || Pointer(vtable + 0x1480) != addresses["LevelGetter"] || Pointer(vtable + 0x1488) != addresses["MaxHealthGetter"] || Pointer(vtable + 0x1490) != addresses["MaxMagicGetter"])
            {
                return null;
            }

            var param = Pointer(pawn + 0x4C8);
            if (param == 0)
            {
                return null;
            }

            int level = ReadInt(param + 0x40);
            int hp = ReadInt(pawn + 0x5DC), mp = ReadInt(pawn + 0x5E4);
            int maxHp = ReadInt(param + 0x50), maxMp = ReadInt(param + 0x54);
            if (level < 1 || level > 999 || maxHp < 1 || maxHp > 1000000 || maxMp < 0 || maxMp > 1000000 || hp < 0 || hp > maxHp || mp < 0 || mp > maxMp)
            {
                throw new InvalidDataException("KH0.2: invalid player stats.");
            }

            var pawnName = ReadName(names, Pointer(pawn + 0x20));
            if (Pointer(world + 0x158) != instance || Pointer(instance + 0x50) != players || ReadInt(instance + 0x58) != count || ReadInt(instance + 0x5C) != capacity || Pointer(players) != localPlayer || Pointer(localPlayer + 0x48) != controller || Pointer(controller + 0x3E8) != pawn || Pointer(controller + 0x458) != pawn || Pointer(pawn) != vtable || Pointer(pawn + 0x4C8) != param)
            {
                return null;
            }

            return new PlayerState(world, instance, players, count, capacity, localPlayer, controller, pawn, vtable, param, pawnName, level, hp, maxHp, mp, maxMp);
        }

        private static string ReadName(ulong names, ulong packedName)
        {
            var index = (uint)packedName;
            var number = (uint)(packedName >> 32);
            var count = ReadInt(names + 0x400);
            var chunks = ReadInt(names + 0x404);
            if (count is < 1 or > 0x200000 || chunks is < 1 or > 128 || index >= count || index / 0x4000 >= chunks || number > 1000000)
            {
                throw new InvalidDataException("KH0.2: invalid FName.");
            }

            var chunk = Pointer(names + index / 0x4000 * 8);
            var entry = chunk == 0 ? 0 : Pointer(chunk + index % 0x4000 * 8);
            if (entry == 0)
            {
                throw new InvalidDataException("KH0.2: missing FName entry.");
            }

            var header = unchecked((uint)ReadInt(entry));
            if ((header >> 1) != index)
            {
                throw new InvalidDataException("KH0.2: inconsistent FName entry.");
            }

            var wide = (header & 1) != 0;
            var step = wide ? 2 : 1;
            var bytes = new List<byte>();
            for (var offset = 0; offset < 1024 * step; offset += 32)
            {
                var buffer = Hypervisor.ReadArray(entry + 0x10 + (ulong)offset, 32, true);
                if (buffer == null || buffer.Length != 32)
                {
                    throw new InvalidDataException("KH0.2: unreadable FName.");
                }

                for (var j = 0; j < buffer.Length; j += step)
                {
                    if (buffer[j] == 0 && (!wide || buffer[j + 1] == 0))
                    {
                        var value = (wide ? Encoding.Unicode : Encoding.UTF8).GetString(bytes.ToArray());
                        if (value.Any(char.IsControl))
                        {
                            throw new InvalidDataException("KH0.2: invalid name characters.");
                        }

                        return number == 0 ? value : value + "_" + (number - 1).ToString(CultureInfo.InvariantCulture);
                    }

                    bytes.Add(buffer[j]);
                    if (wide)
                    {
                        bytes.Add(buffer[j + 1]);
                    }
                }
            }

            throw new InvalidDataException("KH0.2: unterminated FName.");
        }

        private static ulong Pointer(ulong address)
        {
            return Hypervisor.Read<ulong>(address, true);
        }

        private static int ReadInt(ulong address)
        {
            return Hypervisor.Read<int>(address, true);
        }

        private static void ResetAddresses()
        {
            cachedProcessId = cachedModuleSize = 0;
            cachedModuleBase = 0;
            addresses = new();
            nextScan = DateTime.MinValue;
        }

        private static void ResolveAddresses()
        {
            var process = Hypervisor.Process ?? throw new InvalidDataException("KH02: game is not attached.");
            var module = process.MainModule ?? throw new InvalidDataException("KH0.2: module not loaded.");
            var moduleBase = (ulong)module.BaseAddress.ToInt64();
            if (cachedProcessId != process.Id || cachedModuleBase != moduleBase || cachedModuleSize != module.ModuleMemorySize)
            {
                ResetAddresses();
                cachedProcessId = process.Id;
                cachedModuleBase = moduleBase;
                cachedModuleSize = module.ModuleMemorySize;
            }

            if (addresses.Count != 0)
            {
                return;
            }

            if (DateTime.UtcNow < nextScan)
            {
                throw new InvalidDataException("KH0.2: waiting to retry patterns.");
            }

            nextScan = DateTime.UtcNow.AddSeconds(30);
            var resolved = new Dictionary<string, ulong>();
            foreach (var entry in Patterns)
            {
                var spec = entry.Value;
                var match = FindUniquePattern(moduleBase, module.ModuleMemorySize, spec.Pattern);
                var target = spec.Length == 0 ? match : checked((ulong)((long)match + spec.Length + ReadInt(match + (ulong)spec.Operand)));
                if (target < moduleBase || target + 8 > moduleBase + (ulong)module.ModuleMemorySize)
                {
                    throw new InvalidDataException("KH0.2: AOB target is outside the module.");
                }

                resolved.Add(entry.Key, target);
            }

            addresses = resolved;
        }

        private static ulong FindUniquePattern(ulong moduleBase, int moduleSize, string signature)
        {
            if (moduleSize < 64)
            {
                throw new InvalidDataException("KH0.2: invalid module size.");
            }

            var peOffset = Hypervisor.Read<uint>(moduleBase + 0x3C, true);
            if (peOffset > (uint)moduleSize - 24)
            {
                throw new InvalidDataException("KH0.2: invalid PE offset.");
            }

            var pe = moduleBase + peOffset;
            if (peOffset > (uint)moduleSize - 24 || Hypervisor.Read<uint>(pe, true) != 0x4550 || Hypervisor.Read<ushort>(pe + 4, true) != 0x8664)
            {
                throw new InvalidDataException("KH0.2: invalid PE header.");
            }

            var sectionCount = Hypervisor.Read<ushort>(pe + 6, true);
            var optionalSize = Hypervisor.Read<ushort>(pe + 20, true);
            var sectionTable = pe + 24 + optionalSize;
            if (sectionTable + (ulong)sectionCount * 40 > moduleBase + (ulong)moduleSize)
            {
                throw new InvalidDataException("KH0.2: invalid section table.");
            }

            var pattern = SignaturePattern.Parse(signature);
            var prefix = pattern.Bytes.TakeWhile(value => value.HasValue).Select(value => value.GetValueOrDefault()).ToArray();
            if (prefix.Length == 0)
            {
                throw new InvalidDataException("KH0.2: pattern needs a fixed prefix.");
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
                    throw new InvalidDataException("KH0.2: invalid executable section.");
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
                            throw new InvalidDataException("KH0.2: manager pattern is ambiguous.");
                        }

                        match = address;
                    }
                }
            }

            return match != 0 ? match : throw new InvalidDataException("KH0.2: manager pattern was not found.");
        }

        private static void InitializeDiscord()
        {
            if (discord == null)
            {
                throw new InvalidOperationException("KH02: presence is not initialized.");
            }

            discord.Initialize();
        }
    }
}
