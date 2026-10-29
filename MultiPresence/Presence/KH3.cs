using MultiPresence.Runtime;
using DiscordRPC;
using MultiPresence.Models.KH3;
using MultiPresence.Models.KHIII;
using MultiPresence.Infrastructure;
using System.Globalization;
using System.Text;

namespace MultiPresence.Presence
{
    public class KH3
    {
        private static DiscordRpcClient? discord;
        private static DiscordStatusUpdater? updater;
        private const string WorldPattern = "80 7C 24 38 00 48 8B C8 48 8B 05 ?? ?? ?? ?? 48 0F 45 C1 48 83 C4 28 C3";
        private const string NamesPattern = "48 8B 05 ?? ?? ?? ?? 48 85 C0 75 ?? B9 08 04 00 00 48 89 5C 24 20 E8 ?? ?? ?? ?? 48 89 44 24 30";
        private const string DifficultyPattern = "48 8D 0D ?? ?? ?? ?? E8 ?? ?? ?? ?? 48 8D 0D ?? ?? ?? ?? E8 ?? ?? ?? ?? 83 25 ?? ?? ?? ?? FC C7 05 ?? ?? ?? ?? FF FF FF FF 33 DB";
        private const string CurrentAreaLayoutPattern = "48 8D 81 70 08 00 00 C3";
        private const string CurrentAreaWritePattern = "49 8B CE E8 ?? ?? ?? ?? 48 8B 4D 48 48 89 08 49 8B CE E8 ?? ?? ?? ?? 44 89 68 08";
        private const string UObjectNameLayoutPattern = "48 8B 42 20 48 89 44 24 40 48 8B D3 48 8D 4C 24 40 E8 ?? ?? ?? ?? 48 8B C3 48 83 C4 30 5B C3";
        private const string NameEntryLayoutPattern = "48 8D 71 10 33 C0 F6 01 01 48 89 02 48 89 42 08 74 ?? 48 85 F6 74 ?? 66 39 06 74 ??";
        private const string NameIndexLayoutPattern = "8B 03 99 81 E2 FF 3F 00 00 03 C2 44 8B C0 25 FF 3F 00 00 2B C2 48 63 C8 41 C1 F8 0E 49 63 C0 49 8B 04 C1 48 8B 04 C8";
        private const string WorldPlayerLayoutPattern = "48 8B 80 58 01 00 00 EB ?? 49 8B C0 48 85 C0 74 ?? 48 8B 48 50 48 63 40 58 48 8D 14 C1 48 3B CA 74 ?? 48 8B 01 48 85 C0 74 ?? 4C 8B 48 48";
        private const string AcknowledgedPawnLayoutPattern = "48 8B 81 58 04 00 00 C3";
        private const string ControllerPawnLayoutPattern = "48 8B 81 E8 03 00 00 C3";
        private const string HealthLayoutPattern = "8B 81 FC 05 00 00 C3";
        private const string ManaLayoutPattern = "8B 81 04 06 00 00 C3";
        private const string LevelLayoutPattern = "48 8B 81 D0 04 00 00 48 85 C0 74 ?? 8B 40 40 C3 C3";
        private const string ClassHierarchyLayoutPattern = "48 8B 42 18 48 3B C1 74 ?? 48 85 C0 74 ?? 48 3B C1 74 ?? 48 8B 40 48 48 85 C0 75 ?? 49 83 C0 08 EB ?? B0 01 C3 32 C0 C3";
        private const string PlayerHudLayoutPattern = "48 8B 81 70 04 00 00 C3";
        private const string GummiRankValueLayoutPattern = "89 B3 70 01 00 00 89 BB 6C 01 00 00 44 8B C7 48 8D 15 ?? ?? ?? ?? 48 8D 4C 24 28 E8 ?? ?? ?? ?? 90 48 8D 54 24 28 48 8B 8B 40 01 00 00 E8 ?? ?? ?? ?? 90";
        private static int cachedProcessId, cachedModuleSize;
        private static ulong cachedModuleBase, worldSlot, namesSlot, difficultyAddress;
        private static DateTime nextScan;
        public static Task DoAction()
        {
            ResetAddresses();
            GetPID();
            discord = new DiscordRpcClient("827190870724837406");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/Config/Kingdom Hearts III.json");
            PresenceRuntime.Start(nameof(KH3), "KINGDOM HEARTS III", RPC);
            return Task.CompletedTask;
        }

        private static void GetPID()
        {
            ProcessMonitor.TryAttach("KINGDOM HEARTS III");
        }

        private static async Task RPC()
        {
            if (discord == null || updater == null)
            {
                throw new InvalidOperationException("KH3: presence is not initialized.");
            }

            if (ProcessMonitor.IsRunning("KINGDOM HEARTS III"))
            {
                try
                {
                    if (!Hypervisor.IsAttached && !ProcessMonitor.TryAttach("KINGDOM HEARTS III"))
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
                    PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Kingdom Hearts III", placeholders, snapshot.Context);
                }
                catch (Exception exception) when (exception is not OutOfMemoryException and not StackOverflowException)
                {
                    AppLog.Warning("Kingdom Hearts III memory is not ready.", exception);
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
                return new Dictionary<string, object>
                {
                    { "state", "Main Menu" }
                };
            }

            var token = WorldToken(snapshot.MapKey);
            var oldWorld = await Worlds.GetWorld(token);
            var world = Locations.GetWorld(token);
            if (world.Length == 0)
            {
                world = oldWorld[0] == "Main Menu" ? token : oldWorld[0];
            }

            var location = Locations.GetLocation(snapshot.AreaKey, snapshot.MapKey);
            var map = Locations.GetMapForArea(snapshot.AreaKey, snapshot.MapKey);
            var matchingArea = Locations.IsCurrentArea(snapshot.AreaKey, snapshot.MapKey);
            var area = matchingArea ? Locations.GetArea(snapshot.AreaKey) : "";
            var subarea = matchingArea ? Locations.GetSubArea(snapshot.AreaKey) : "";
            if (location.Length == 0)
            {
                location = await Rooms.GetRoom(snapshot.MapKey);
                if (location == "Unknown")
                {
                    location = snapshot.MapKey;
                }
            }

            return new Dictionary<string, object>
            {
                { "state", "In Game" },
                { "level", snapshot.Level },
                { "gummilevel", snapshot.GummiLevel < 0 ? "Unknown" : snapshot.GummiLevel },
                { "hp", snapshot.Hp },
                { "mp", snapshot.Mp },
                { "world", world },
                { "world_icon_name", oldWorld[1] },
                { "location", location },
                { "room", location },
                { "map", map },
                { "area", area },
                { "subarea", subarea },
                { "worldid", token },
                { "mapid", snapshot.MapKey },
                { "areaid", snapshot.AreaKey },
                { "difficulty", snapshot.Difficulty switch { 0 => "Beginner", 1 => "Standard", 2 => "Proud", 3 => "Critical", _ => snapshot.Difficulty.ToString(CultureInfo.InvariantCulture), } },
                { "difficultyid", snapshot.Difficulty }
            };
        }

        private static void ShowStatus(string text)
        {
            if (discord == null)
            {
                throw new InvalidOperationException("KH3: presence is not initialized.");
            }

            discord.SetPresence(new RichPresence()
            {
                Details = text,
                State = "",
                Assets = new Assets()
                {
                    LargeImageKey = "logo",
                    LargeImageText = "Kingdom Hearts III"
                },
                Timestamps = PlaceholderHelper._startTimestamp
            });
        }

        private sealed record Snapshot(bool IsInGame, string Context, string MapKey, string AreaKey, int Difficulty, int Level, int GummiLevel, int Hp, int Mp);
        private static Snapshot EmptySnapshot()
        {
            return new Snapshot(false, "", "", "", 0, 0, 0, 0, 0);
        }

        private static Snapshot ReadSnapshot()
        {
            ResolveAddresses();
            var world = Pointer(worldSlot);
            var names = Pointer(namesSlot);
            if (world == 0 || names == 0)
            {
                return EmptySnapshot();
            }

            var worldName = Pointer(world + 0x20);
            var map = ReadName(names, worldName);
            var token = WorldToken(map);
            if (token.Length == 0)
            {
                return EmptySnapshot();
            }

            var instance = Pointer(world + 0x158);
            if (instance == 0)
            {
                return EmptySnapshot();
            }

            var players = Pointer(instance + 0x50);
            var count = ReadInt(instance + 0x58);
            var capacity = ReadInt(instance + 0x5C);
            if (count < 0 || count > capacity || capacity > 8)
            {
                throw new InvalidDataException("KH3: invalid local players.");
            }

            if (players == 0 || count == 0)
            {
                return EmptySnapshot();
            }

            var player = Pointer(players);
            var controller = player == 0 ? 0 : Pointer(player + 0x48);
            if (controller == 0)
            {
                return EmptySnapshot();
            }

            var pawn = Pointer(controller + 0x458);
            var possessedPawn = Pointer(controller + 0x3E8);
            var context = token == "wm" ? "World_Map" : token == "gm" ? "Gummi_Ship" : "In_World";
            var areaName = Pointer(instance + 0x870);
            var area = ReadName(names, areaName);
            var difficulty = ReadInt(difficultyAddress);
            if (difficulty is < 0 or > 3)
            {
                throw new InvalidDataException("KH3: unsupported difficulty.");
            }

            int level = 0, hp = 0, mp = 0, gummiLevel = 0;
            ulong parameters = 0;
            if (context == "In_World")
            {
                if (pawn == 0 || pawn != possessedPawn)
                {
                    return EmptySnapshot();
                }

                parameters = Pointer(pawn + 0x4D0);
                if (parameters == 0)
                {
                    return EmptySnapshot();
                }

                level = ReadInt(parameters + 0x40);
                hp = ReadInt(pawn + 0x5FC);
                mp = ReadInt(pawn + 0x604);
                if (level is < 1 or > 99 || hp is < 0 or > 1000000 || mp is < 0 or > 1000000)
                {
                    throw new InvalidDataException("KH3: invalid current player stats.");
                }
            }

            if (context == "Gummi_Ship")
            {
                gummiLevel = ReadGummiLevel(controller, names);
            }

            if (world != Pointer(worldSlot) || names != Pointer(namesSlot) || worldName != Pointer(world + 0x20) || instance != Pointer(world + 0x158) || areaName != Pointer(instance + 0x870) || players != Pointer(instance + 0x50) || count != ReadInt(instance + 0x58) || player != Pointer(players) || controller != Pointer(player + 0x48) || pawn != Pointer(controller + 0x458) || possessedPawn != Pointer(controller + 0x3E8) || (parameters != 0 && parameters != Pointer(pawn + 0x4D0)) || difficulty != ReadInt(difficultyAddress))
            {
                throw new InvalidDataException("KH3: scene changed during the read; retry next poll.");
            }

            return new Snapshot(true, context, map, area, difficulty, level, gummiLevel, hp, mp);
        }

        private static string WorldToken(string map)
        {
            if (map.Length < 5 || map[2] != '_' || !char.IsAsciiLetterLower(map[0]) || !char.IsAsciiLetterLower(map[1]) || !char.IsAsciiDigit(map[3]) || !char.IsAsciiDigit(map[4]))
            {
                return "";
            }

            return map[..2];
        }

        private static bool IsA(ulong instance, ulong names, string expected)
        {
            if (instance == 0)
            {
                return false;
            }

            var type = Pointer(instance + 0x18);
            for (var depth = 0; type != 0 && depth < 32; depth++)
            {
                if (ReadName(names, Pointer(type + 0x20)) == expected)
                {
                    return true;
                }

                type = Pointer(type + 0x48);
            }

            return false;
        }

        private static int ReadGummiLevel(ulong controller, ulong names)
        {
            var hud = Pointer(controller + 0x470);
            if (!IsA(hud, names, "TresGumiShipHUD"))
            {
                return -1;
            }

            var ui = Pointer(hud + 0x550);
            if (!IsA(ui, names, "TresUIP_HudGummiShip"))
            {
                return -1;
            }

            var rank = Pointer(ui + 0x228);
            if (!IsA(rank, names, "TresUIP_HudGummiShipRank"))
            {
                return -1;
            }

            var level = ReadInt(rank + 0x16C);
            if (level is < -1 or > 999)
            {
                throw new InvalidDataException("KH3: invalid Gummi level.");
            }

            if (hud != Pointer(controller + 0x470) || ui != Pointer(hud + 0x550) || rank != Pointer(ui + 0x228) || level != ReadInt(rank + 0x16C))
            {
                throw new InvalidDataException("KH3: Gummi HUD changed during the read.");
            }

            return level;
        }

        private static string ReadName(ulong names, ulong packedName)
        {
            var index = (uint)packedName;
            var number = (uint)(packedName >> 32);
            var count = ReadInt(names + 0x400);
            var chunks = ReadInt(names + 0x404);
            if (count is < 1 or > 0x200000 || chunks is < 1 or > 128 || index >= count || index / 0x4000 >= chunks || number > 1000000)
            {
                throw new InvalidDataException("KH3: invalid FName.");
            }

            var chunk = Pointer(names + index / 0x4000 * 8);
            var entry = chunk == 0 ? 0 : Pointer(chunk + index % 0x4000 * 8);
            if (entry == 0)
            {
                throw new InvalidDataException("KH3: missing FName entry.");
            }

            var header = unchecked((uint)ReadInt(entry));
            if ((header >> 1) != index)
            {
                throw new InvalidDataException("KH3: inconsistent FName entry.");
            }

            var wide = (header & 1) != 0;
            var step = wide ? 2 : 1;
            var bytes = new List<byte>();
            for (var offset = 0; offset < 1024 * step; offset += 32)
            {
                var buffer = Hypervisor.ReadArray(entry + 0x10 + (ulong)offset, 32, true);
                if (buffer == null || buffer.Length != 32)
                {
                    throw new InvalidDataException("KH3: unreadable FName.");
                }

                for (var j = 0; j < buffer.Length; j += step)
                {
                    if (buffer[j] == 0 && (!wide || buffer[j + 1] == 0))
                    {
                        var value = (wide ? Encoding.Unicode : Encoding.UTF8).GetString(bytes.ToArray());
                        if (value.Any(char.IsControl))
                        {
                            throw new InvalidDataException("KH3: invalid name characters.");
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

            throw new InvalidDataException("KH3: unterminated FName.");
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
            cachedModuleBase = worldSlot = namesSlot = difficultyAddress = 0;
            nextScan = DateTime.MinValue;
        }

        private static void ResolveAddresses()
        {
            var process = Hypervisor.Process ?? throw new InvalidDataException("KH3: game is not attached.");
            var module = process.MainModule ?? throw new InvalidDataException("KH3: module not loaded.");
            var moduleBase = (ulong)module.BaseAddress.ToInt64();
            if (cachedProcessId != process.Id || cachedModuleBase != moduleBase || cachedModuleSize != module.ModuleMemorySize)
            {
                ResetAddresses();
                cachedProcessId = process.Id;
                cachedModuleBase = moduleBase;
                cachedModuleSize = module.ModuleMemorySize;
            }

            if (worldSlot != 0)
            {
                return;
            }

            if (DateTime.UtcNow < nextScan)
            {
                throw new InvalidDataException("KH3: waiting to retry patterns.");
            }

            nextScan = DateTime.UtcNow.AddSeconds(30);
            ulong Resolve(string pattern, int operand, int length)
            {
                var match = FindUniquePattern(moduleBase, module.ModuleMemorySize, pattern);
                var target = checked((ulong)((long)match + length + ReadInt(match + (ulong)operand)));
                if (target < moduleBase || target + 8 > moduleBase + (ulong)module.ModuleMemorySize)
                {
                    throw new InvalidDataException("KH3: RIP-relative target is outside the module.");
                }

                return target;
            }

            var worldSlotValue = Resolve(WorldPattern, 11, 15);
            var namesSlotValue = Resolve(NamesPattern, 3, 7);
            var difficultyAddressValue = Resolve(DifficultyPattern, 3, 7);
            FindUniquePattern(moduleBase, module.ModuleMemorySize, CurrentAreaLayoutPattern);
            FindUniquePattern(moduleBase, module.ModuleMemorySize, CurrentAreaWritePattern);
            FindUniquePattern(moduleBase, module.ModuleMemorySize, UObjectNameLayoutPattern);
            FindUniquePattern(moduleBase, module.ModuleMemorySize, NameEntryLayoutPattern);
            FindUniquePattern(moduleBase, module.ModuleMemorySize, NameIndexLayoutPattern);
            FindUniquePattern(moduleBase, module.ModuleMemorySize, WorldPlayerLayoutPattern);
            FindUniquePattern(moduleBase, module.ModuleMemorySize, AcknowledgedPawnLayoutPattern);
            FindUniquePattern(moduleBase, module.ModuleMemorySize, ControllerPawnLayoutPattern);
            FindUniquePattern(moduleBase, module.ModuleMemorySize, HealthLayoutPattern);
            FindUniquePattern(moduleBase, module.ModuleMemorySize, ManaLayoutPattern);
            FindUniquePattern(moduleBase, module.ModuleMemorySize, LevelLayoutPattern);
            FindUniquePattern(moduleBase, module.ModuleMemorySize, ClassHierarchyLayoutPattern);
            FindUniquePattern(moduleBase, module.ModuleMemorySize, PlayerHudLayoutPattern);
            FindUniquePattern(moduleBase, module.ModuleMemorySize, GummiRankValueLayoutPattern);
            worldSlot = worldSlotValue;
            namesSlot = namesSlotValue;
            difficultyAddress = difficultyAddressValue;
        }

        private static ulong FindUniquePattern(ulong moduleBase, int moduleSize, string signature)
        {
            if (moduleSize < 64)
            {
                throw new InvalidDataException("KH3: invalid module size.");
            }

            var peOffset = Hypervisor.Read<uint>(moduleBase + 0x3C, true);
            if (peOffset > (uint)moduleSize - 24)
            {
                throw new InvalidDataException("KH3: invalid PE offset.");
            }

            var pe = moduleBase + peOffset;
            if (peOffset > (uint)moduleSize - 24 || Hypervisor.Read<uint>(pe, true) != 0x4550 || Hypervisor.Read<ushort>(pe + 4, true) != 0x8664)
            {
                throw new InvalidDataException("KH3: invalid PE header.");
            }

            var sectionCount = Hypervisor.Read<ushort>(pe + 6, true);
            var optionalSize = Hypervisor.Read<ushort>(pe + 20, true);
            var sectionTable = pe + 24 + optionalSize;
            if (sectionTable + (ulong)sectionCount * 40 > moduleBase + (ulong)moduleSize)
            {
                throw new InvalidDataException("KH3: invalid section table.");
            }

            var pattern = SignaturePattern.Parse(signature);
            var prefix = pattern.Bytes.TakeWhile(value => value.HasValue).Select(value => value.GetValueOrDefault()).ToArray();
            if (prefix.Length == 0)
            {
                throw new InvalidDataException("KH3: pattern needs a fixed prefix.");
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
                    throw new InvalidDataException("KH3: invalid executable section.");
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
                            throw new InvalidDataException("KH3: manager pattern is ambiguous.");
                        }

                        match = address;
                    }
                }
            }

            return match != 0 ? match : throw new InvalidDataException("KH3: manager pattern was not found.");
        }

        private static void InitializeDiscord()
        {
            if (discord == null)
            {
                throw new InvalidOperationException("KH3: presence is not initialized.");
            }

            discord.Initialize();
        }
    }
}
