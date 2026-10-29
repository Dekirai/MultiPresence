using MultiPresence.Runtime;
using DiscordRPC;
using MultiPresence.Models.CB4;
using MultiPresence.Infrastructure;
using System.Text;

namespace MultiPresence.Presence
{
    public class CB4
    {
        private static DiscordRpcClient? discord;
        private static DiscordStatusUpdater? updater;
        private const string ProcessName = "CrashBandicoot4";
        private const string GameName = "Crash Bandicoot 4: It's About Time";
        private static readonly Dictionary<string, (string Pattern, int Operand, int Length)> Patterns = new Dictionary<string, (string Pattern, int Operand, int Length)>
        {
            { "World", ("48 8B 0D ?? ?? ?? ?? 48 39 81 98 01 00 00 75 ?? 48 8B CB E8 ?? ?? ?? ??", 3, 7) },
            { "Names", ("48 8D 0D ?? ?? ?? ?? E8 ?? ?? ?? ?? 48 8B D8 C6 05 ?? ?? ?? ?? 01 0F 10 07", 3, 7) },
            { "TotalCrates", ("8B 81 30 04 00 00 C3", 0, 0) },
            { "ScoredCrates", ("8B 81 38 04 00 00 03 81 34 04 00 00 C3", 0, 0) },
            { "Deaths", ("8B 81 A8 05 00 00 41 89 00 C3", 0, 0) },
            { "Mode", ("48 8B 42 20 45 33 C9 48 85 C0 41 0F 95 C1 4C 03 C8 4C 89 4A 20 0F B6 81 B8 00 00 00 41 88 00 C3", 0, 0) }
        };
        private static Dictionary<string, ulong> addresses = new Dictionary<string, ulong>();
        private static readonly Dictionary<uint, string> names = new Dictionary<uint, string>();
        private static readonly Dictionary<(ulong, string), uint> properties = new Dictionary<(ulong, string), uint>();
        private static int cachedProcessId, cachedModuleSize;
        private static ulong cachedModuleBase, namesPool;
        private static DateTime nextScan;

        public static void DoAction()
        {
            ResetAddresses();
            GetPID();
            discord = new DiscordRpcClient("1395058700892377190");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/Config/Crash Bandicoot 4.json");
            PresenceRuntime.Start(nameof(CB4), ProcessName, RPC);
        }

        private static void GetPID()
        {
            ProcessMonitor.TryAttach(ProcessName);
        }

        private static async Task RPC()
        {
            if (discord == null || updater == null)
            {
                throw new InvalidOperationException("CB4: presence is not initialized.");
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
                        ShowStatus("In Menus / Loading");
                        return;
                    }

                    var placeholders = await PlaceholderHelper.GetPlaceholders(() => GeneratePlaceholders(snapshot));
                    PlaceholderHelper.UpdateDiscordStatus(discord, updater, GameName, placeholders, snapshot.Mode);
                }
                catch (Exception exception) when (exception is not OutOfMemoryException and not StackOverflowException)
                {
                    AppLog.Warning("Crash Bandicoot 4 memory is not ready.", exception);
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

        private static void ShowStatus(string text)
        {
            if (discord == null)
            {
                throw new InvalidOperationException("CB4: presence is not initialized.");
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

        private sealed record Snapshot(bool IsInGame, string LevelId, string Level, string World, string WorldTime, string Map, string Character, string Mode, int CurrentCrates, int MaxCrates, int Deaths, int WumpasRetro, int WumpasModern, int Lives);

        private static Snapshot EmptySnapshot()
        {
            return new Snapshot(false, "", "", "", "", "", "", "", 0, 0, 0, 0, 0, -1);
        }

        private static Task<Dictionary<string, object>> GeneratePlaceholders(Snapshot snapshot)
        {
            if (!snapshot.IsInGame)
            {
                return Task.FromResult(new Dictionary<string, object>());
            }

            return Task.FromResult(new Dictionary<string, object>
            {
                { "level", snapshot.Level },
                { "levelid", snapshot.LevelId },
                { "map", snapshot.Level },
                { "mapid", snapshot.Map },
                { "world", snapshot.World },
                { "worldtime", snapshot.WorldTime },
                { "location", snapshot.World + " / " + snapshot.Level },
                { "character", snapshot.Character },
                { "mode", snapshot.Mode },
                { "currentcrates", snapshot.CurrentCrates },
                { "maxcrates", snapshot.MaxCrates },
                { "deaths", snapshot.Deaths },
                { "wumpas", snapshot.Mode == "Retro" ? snapshot.WumpasRetro : snapshot.WumpasModern },
                { "wumpasretro", snapshot.WumpasRetro },
                { "wumpasmodern", snapshot.WumpasModern },
                { "lives", snapshot.Mode == "Retro" ? snapshot.Lives >= 0 ? snapshot.Lives.ToString() : "Unknown" : "" }
            });
        }

        private static Snapshot ReadSnapshot()
        {
            ResolveAddresses();
            var world = Pointer(addresses["World"]);
            if (world == 0 || !IsA(world, "World"))
            {
                return EmptySnapshot();
            }

            var map = ObjectName(world);
            var gameState = Pointer(world + Property(world, "GameState", 8));
            var instance = Pointer(world + Property(world, "OwningGameInstance", 8));
            if (gameState == 0 || instance == 0 || !IsA(gameState, "LavaGameState"))
            {
                return EmptySnapshot();
            }

            var playersAddress = instance + Property(instance, "LocalPlayers", 16);
            var players = ReadArray(playersAddress, 4);
            if (players.Count == 0)
            {
                return EmptySnapshot();
            }

            var localPlayer = Pointer(players.Data);
            var controller = Pointer(localPlayer + Property(localPlayer, "PlayerController", 8));
            if (controller == 0)
            {
                return EmptySnapshot();
            }

            var pawn = Pointer(controller + Property(controller, "Pawn", 8));
            var playerState = Pointer(controller + Property(controller, "PlayerState", 8));
            if (pawn == 0 || playerState == 0 || Pointer(controller + Property(controller, "AcknowledgedPawn", 8)) != pawn || !IsA(playerState, "LavaPlayerState"))
            {
                return EmptySnapshot();
            }

            var character = GetCharacter(pawn);
            if (character.Length == 0)
            {
                return EmptySnapshot();
            }

            var rulesAddress = gameState + Property(gameState, "ActiveObjectRules", 16);
            var rules = ReadArray(rulesAddress, 128);
            ulong difficulty = 0, characterRule = 0;
            var buddy = false;
            for (var index = 0; index < rules.Count; index++)
            {
                var rule = Pointer(rules.Data + (ulong)index * 8);
                if (rule == 0)
                {
                    continue;
                }

                if (IsA(rule, "ObjectRules_SelectDifficulty"))
                {
                    difficulty = rule;
                }
                else if (IsA(rule, "ObjectRules_PlayerCharacter"))
                {
                    characterRule = rule;
                    buddy = ReadBoolean(rule, "bIsBuddyPath");
                }
            }

            if (difficulty == 0 || characterRule == 0)
            {
                return EmptySnapshot();
            }

            buddy = buddy || HasBuddyPathOverride(world, gameState);
            if (!Locations.TryGetForMap(map, buddy, out var level) && !Locations.TryGetForMap(map, out level))
            {
                return EmptySnapshot();
            }

            var difficultyId = Hypervisor.Read<byte>(difficulty + 0xB8, true);
            var mode = difficultyId switch { 1 => "Modern", 2 => "Retro", _ => "" };
            if (mode.Length == 0)
            {
                return EmptySnapshot();
            }

            var maxCrates = ReadInt(gameState + 0x430);
            var crates = checked(ReadInt(gameState + 0x434) + ReadInt(gameState + 0x438));
            var deaths = ReadInt(gameState + 0x5A8);
            var collectedWumpas = ReadInt(playerState + Property(playerState, "CollectedWumpa", 4));
            var wumpasRetro = collectedWumpas >= 100 ? 0 : collectedWumpas;
            var wumpasModern = checked(ReadInt(playerState + Property(playerState, "ScoredWumpa", 4)) + ReadInt(playerState + Property(playerState, "TempScoredWumpa", 4)));
            var lives = mode == "Retro" ? ReadLives(instance) : -1;
            if (maxCrates is < 0 or > 10000 || crates < 0 || crates > maxCrates || deaths is < 0 or > 1000000 || wumpasRetro is < 0 or > 1000000 || wumpasModern is < 0 or > 1000000)
            {
                throw new InvalidDataException("CB4: invalid level counters.");
            }

            if (world != Pointer(addresses["World"]) || map != ObjectName(world) || gameState != Pointer(world + Property(world, "GameState", 8)) || pawn != Pointer(controller + Property(controller, "Pawn", 8)) || pawn != Pointer(controller + Property(controller, "AcknowledgedPawn", 8)) || character != GetCharacter(pawn) || playerState != Pointer(controller + Property(controller, "PlayerState", 8)) || rules != ReadArray(rulesAddress, 128) || difficultyId != Hypervisor.Read<byte>(difficulty + 0xB8, true))
            {
                throw new InvalidDataException("CB4: scene changed during the read; retry next poll.");
            }

            return new Snapshot(true, level.Id, level.Name, level.World, level.WorldTime, map, character, mode, crates, maxCrates, deaths, wumpasRetro, wumpasModern, lives);
        }

        private static string GetCharacter(ulong pawn)
        {
            if (IsA(pawn, "BP_CP3701_C"))
            {
                var skin = Pointer(pawn + Property(pawn, "SkinAsset", 8));
                if (skin == 0 || !IsA(skin, "TfbCombatCharacterSkinAsset"))
                {
                    return "";
                }

                var character = Hypervisor.Read<byte>(skin + Property(skin, "SPCharacterOwner", 1), true);
                return character switch { 0 => "Crash Bandicoot", 1 => "Coco Bandicoot", _ => "" };
            }

            if (IsA(pawn, "BP_CP3702_C"))
            {
                return "Dingodile";
            }

            if (IsA(pawn, "BP_CP3705_C"))
            {
                return "Dr. Neo Cortex";
            }

            return IsA(pawn, "BP_CP3706_C") ? "Tawna" : "";
        }

        private static bool ReadBoolean(ulong instance, string name)
        {
            return Hypervisor.Read<byte>(instance + Property(instance, name, 1), true) != 0;
        }

        private static (ulong Data, int Count, int Capacity) ReadArray(ulong address, int maximum)
        {
            var data = Pointer(address);
            var count = ReadInt(address + 8);
            var capacity = ReadInt(address + 12);
            if (count < 0 || count > capacity || capacity > maximum || (count > 0 && data == 0))
            {
                throw new InvalidDataException("CB4: invalid array.");
            }

            return (data, count, capacity);
        }

        private static int ReadLives(ulong instance)
        {
            var maps = new List<(ulong Address, byte[] Header)>();
            ulong manager = 0;
            foreach (var entry in ReadSaveMap(instance + 0xB8, 24, 64, maps))
            {
                var candidate = Pointer(entry + 8);
                if (candidate != 0 && IsA(candidate, "LavaSaveManager"))
                {
                    manager = candidate;
                    break;
                }
            }

            if (manager == 0)
            {
                return -1;
            }

            var slotAddress = manager + Property(manager, "CurrentSlotSaveData", 8);
            var slot = Pointer(slotAddress);
            if (slot == 0 || !IsA(slot, "TfbAsyncSaveGamePair"))
            {
                return -1;
            }

            var gameAddress = slot + Property(slot, "SaveGameObject", 8);
            var game = Pointer(gameAddress);
            if (game == 0 || !IsA(game, "TfbSaveGame"))
            {
                return -1;
            }

            var groups = game + Property(game, "SaveGroupNameToSaveGroup", 80);
            var player = FindSaveEntry(groups, 96, "Player", maps);
            var group = player == 0 ? 0 : FindSaveEntry(player + 8, 96, "Lives", maps);
            var value = group == 0 ? 0 : FindSaveEntry(group + 8, 32, "Lives", maps);
            if (value == 0)
            {
                return -1;
            }

            var data = Pointer(value + 8);
            int length = ReadInt(value + 16), capacity = ReadInt(value + 20);
            if (data == 0 || length < 5 || length > 32 || capacity < length || capacity > 256)
            {
                return -1;
            }

            var bytes = Hypervisor.ReadArray(data, length, true);
            var characters = BitConverter.ToInt32(bytes, 0);
            string text;
            if (characters >= 2 && characters <= 12 && length == characters + 4 && bytes[^1] == 0)
            {
                text = System.Text.Encoding.ASCII.GetString(bytes, 4, characters - 1);
            }
            else if (characters <= -2 && characters >= -12 && length == 4 - characters * 2 &&
                bytes[^1] == 0 && bytes[^2] == 0)
            {
                text = System.Text.Encoding.Unicode.GetString(bytes, 4, (-characters - 1) * 2);
            }
            else
            {
                return -1;
            }

            if (!int.TryParse(text, System.Globalization.NumberStyles.None, System.Globalization.CultureInfo.InvariantCulture, out var lives) ||
                lives < 0 || lives > 999 || Pointer(slotAddress) != slot || Pointer(gameAddress) != game ||
                Pointer(value + 8) != data || ReadInt(value + 16) != length || ReadInt(value + 20) != capacity ||
                !bytes.AsSpan().SequenceEqual(Hypervisor.ReadArray(data, length, true)))
            {
                return -1;
            }

            foreach (var map in maps)
            {
                if (!map.Header.AsSpan().SequenceEqual(Hypervisor.ReadArray(map.Address, map.Header.Length, true)))
                {
                    return -1;
                }
            }

            return lives;
        }

        private static ulong FindSaveEntry(ulong map, int stride, string key, List<(ulong Address, byte[] Header)> maps)
        {
            foreach (var entry in ReadSaveMap(map, stride, 256, maps))
            {
                if (ReadInt(entry + 4) == 0 && ReadName(unchecked((uint)ReadInt(entry))) == key)
                {
                    return entry;
                }
            }

            return 0;
        }

        private static List<ulong> ReadSaveMap(ulong map, int stride, int limit, List<(ulong Address, byte[] Header)> maps)
        {
            var result = new List<ulong>();
            var header = Hypervisor.ReadArray(map, 80, true);
            var data = BitConverter.ToUInt64(header, 0);
            int count = BitConverter.ToInt32(header, 8), capacity = BitConverter.ToInt32(header, 12);
            int bitCount = BitConverter.ToInt32(header, 0x28), bitCapacity = BitConverter.ToInt32(header, 0x2C);
            int freeCount = BitConverter.ToInt32(header, 0x34);
            if (count < 0 || count > limit || capacity < count || capacity > limit * 4 ||
                bitCount != count || bitCapacity < bitCount || bitCapacity > limit * 4 ||
                freeCount < 0 || freeCount > count || (count > 0 && (data < 0x10000 || data > 0x7FFFFFFFFFFF)))
            {
                return result;
            }

            var bitData = BitConverter.ToUInt64(header, 0x20);
            if ((bitCapacity > 128 && (bitData < 0x10000 || bitData > 0x7FFFFFFFFFFF)) ||
                (bitCapacity <= 128 && bitData != 0))
            {
                return result;
            }

            var flags = bitCapacity > 128 ? Hypervisor.ReadArray(bitData, ((bitCount + 31) / 32) * 4, true) : header[0x10..0x20];
            for (var index = 0; index < count; index++)
            {
                if ((flags[index / 8] & (1 << (index % 8))) != 0)
                {
                    result.Add(data + (ulong)(index * stride));
                }
            }

            if (result.Count != count - freeCount || !header.AsSpan().SequenceEqual(Hypervisor.ReadArray(map, 80, true)))
            {
                return new List<ulong>();
            }

            maps.Add((map, header));
            if (bitCapacity > 128 && flags.Length > 0)
            {
                maps.Add((bitData, flags));
            }
            return result;
        }

        private static bool HasBuddyPathOverride(ulong world, ulong gameState)
        {
            var options = ReadArray(world + 0x438, 128);
            for (var index = 0; index < options.Count; index++)
            {
                var option = ReadLocationString(options.Data + (ulong)index * 16);
                var separator = option.IndexOf('=');
                var key = separator < 0 ? option : option.Substring(0, separator);
                if (string.Equals(key, "BuddyPath", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            var manager = Pointer(gameState + Property(gameState, "GameRulesManager", 8));
            if (manager == 0 || !IsA(manager, "TfbGameRulesManager"))
            {
                return false;
            }

            var layers = ReadArray(manager + Property(manager, "GameRuleLayers", 16), 128);
            for (var index = 0; index < layers.Count; index++)
            {
                var layer = Pointer(layers.Data + (ulong)index * 8);
                if (layer != 0 && IsA(layer, "GameRules_BuddyPath"))
                {
                    return true;
                }
            }
            return false;
        }

        private static string ReadLocationString(ulong address)
        {
            var value = ReadArray(address, 1024);
            if (value.Count == 0)
            {
                return "";
            }

            return System.Text.Encoding.Unicode.GetString(Hypervisor.ReadArray(value.Data, value.Count * 2, true)).TrimEnd('\0');
        }


        private static uint Property(ulong instance, string name, int elementSize)
        {
            var initialClass = Pointer(instance + 0x10);
            if (properties.TryGetValue((initialClass, name), out var cached))
            {
                return cached;
            }

            var visitedClasses = new HashSet<ulong>();
            for (var cls = initialClass; cls != 0 && visitedClasses.Count < 32; cls = Pointer(cls + 0x40))
            {
                if (!visitedClasses.Add(cls))
                {
                    break;
                }

                var size = Hypervisor.Read<uint>(cls + 0x50, true);
                var visitedFields = new HashSet<ulong>();
                for (var field = Pointer(cls + 0x48); field != 0 && visitedFields.Count < 2048; field = Pointer(field + 0x28))
                {
                    if (!visitedFields.Add(field))
                    {
                        break;
                    }

                    if (ObjectName(field) != name)
                    {
                        continue;
                    }

                    var offset = Hypervisor.Read<uint>(field + 0x44, true);
                    if (ReadInt(field + 0x30) != 1 || ReadInt(field + 0x34) != elementSize || offset < 0x28 || size > 0x100000 || (ulong)offset + (ulong)elementSize > size)
                    {
                        throw new InvalidDataException("CB4: reflected property layout changed: " + name);
                    }

                    properties[(initialClass, name)] = offset;
                    return offset;
                }
            }

            throw new InvalidDataException("CB4: required Unreal property not found: " + name);
        }

        private static bool IsA(ulong instance, string expectedClass)
        {
            var visited = new HashSet<ulong>();
            for (var cls = Pointer(instance + 0x10); cls != 0 && visited.Count < 32; cls = Pointer(cls + 0x40))
            {
                if (!visited.Add(cls))
                {
                    return false;
                }

                if (ObjectName(cls) == expectedClass)
                {
                    return true;
                }
            }

            return false;
        }

        private static string ObjectName(ulong instance)
        {
            var name = ReadName(Hypervisor.Read<uint>(instance + 0x18, true));
            var number = Hypervisor.Read<uint>(instance + 0x1C, true);
            return number == 0 ? name : $"{name}_{number - 1}";
        }

        private static string ReadName(uint index)
        {
            if (names.TryGetValue(index, out var cached))
            {
                return cached;
            }

            var block = index >> 16;
            var currentBlock = Hypervisor.Read<uint>(namesPool + 8, true);
            if (currentBlock >= 8192 || block > currentBlock)
            {
                throw new InvalidDataException("CB4: invalid FName block.");
            }

            var blockData = Pointer(namesPool + 0x10 + block * 8);
            if (blockData == 0)
            {
                throw new InvalidDataException("CB4: missing FName block.");
            }

            var offset = (index & 0xFFFF) * 2;
            var header = Hypervisor.Read<ushort>(blockData + offset, true);
            var length = header >> 6;
            var wide = (header & 1) != 0;
            var byteCount = length * (wide ? 2 : 1);
            if (length == 0 || length > 1023 || offset + 2 + byteCount > 131072 || (block == currentBlock && offset + 2 + byteCount > Hypervisor.Read<uint>(namesPool + 12, true)))
            {
                throw new InvalidDataException("CB4: invalid FName entry.");
            }

            var bytes = Hypervisor.ReadArray(blockData + offset + 2, byteCount, true);
            var name = (wide ? Encoding.Unicode : Encoding.UTF8).GetString(bytes);
            if (name.Contains('\0'))
            {
                throw new InvalidDataException("CB4: invalid FName text.");
            }

            names[index] = name;
            return name;
        }

        private static int ReadInt(ulong address)
        {
            return Hypervisor.Read<int>(address, true);
        }

        private static ulong Pointer(ulong address)
        {
            return Hypervisor.Read<ulong>(address, true);
        }

        private static void ResetAddresses()
        {
            cachedProcessId = cachedModuleSize = 0;
            cachedModuleBase = namesPool = 0;
            addresses.Clear();
            names.Clear();
            properties.Clear();
            nextScan = DateTime.MinValue;
        }

        private static void ResolveAddresses()
        {
            var process = Hypervisor.Process ?? throw new InvalidDataException("CB4: game is not attached.");
            var module = process.MainModule ?? throw new InvalidDataException("CB4: module not loaded.");
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
                throw new InvalidDataException("CB4: waiting to retry patterns.");
            }

            nextScan = DateTime.UtcNow.AddSeconds(30);
            var resolved = new Dictionary<string, ulong>();
            foreach (var pattern in Patterns)
            {
                var match = FindUniquePattern(moduleBase, module.ModuleMemorySize, pattern.Value.Pattern);
                var target = pattern.Value.Length == 0 ? match : checked((ulong)((long)match + pattern.Value.Length + ReadInt(match + (ulong)pattern.Value.Operand)));
                if (target < moduleBase || target + 8 > moduleBase + (ulong)module.ModuleMemorySize)
                {
                    throw new InvalidDataException("CB4: pattern target outside module.");
                }

                resolved.Add(pattern.Key, target);
            }

            namesPool = resolved["Names"];
            names.Clear();
            if (ReadName(0) != "None")
            {
                throw new InvalidDataException("CB4: FName layout changed.");
            }

            addresses = resolved;
        }

        private static ulong FindUniquePattern(ulong moduleBase, int moduleSize, string signature)
        {
            if (moduleSize < 64)
            {
                throw new InvalidDataException("CB4: invalid module size.");
            }

            var peOffset = Hypervisor.Read<uint>(moduleBase + 0x3C, true);
            if (peOffset > (uint)moduleSize - 24)
            {
                throw new InvalidDataException("CB4: invalid PE offset.");
            }

            var pe = moduleBase + peOffset;
            if (peOffset > (uint)moduleSize - 24 || Hypervisor.Read<uint>(pe, true) != 0x4550 || Hypervisor.Read<ushort>(pe + 4, true) != 0x8664)
            {
                throw new InvalidDataException("CB4: invalid PE header.");
            }

            var sectionCount = Hypervisor.Read<ushort>(pe + 6, true);
            var optionalSize = Hypervisor.Read<ushort>(pe + 20, true);
            var sectionTable = pe + 24 + optionalSize;
            if (sectionTable + (ulong)sectionCount * 40 > moduleBase + (ulong)moduleSize)
            {
                throw new InvalidDataException("CB4: invalid section table.");
            }

            var pattern = SignaturePattern.Parse(signature);
            var prefix = pattern.Bytes.TakeWhile(value => value.HasValue).Select(value => value.GetValueOrDefault()).ToArray();
            if (prefix.Length == 0)
            {
                throw new InvalidDataException("CB4: pattern needs a fixed prefix.");
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
                    throw new InvalidDataException("CB4: invalid executable section.");
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
                            throw new InvalidDataException("CB4: manager pattern is ambiguous.");
                        }

                        match = address;
                    }
                }
            }

            return match != 0 ? match : throw new InvalidDataException("CB4: manager pattern was not found.");
        }

        private static void InitializeDiscord()
        {
            if (discord == null)
            {
                throw new InvalidOperationException("CB4: presence is not initialized.");
            }

            discord.Initialize();
        }
    }
}
