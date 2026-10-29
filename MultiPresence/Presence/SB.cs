using MultiPresence.Runtime;
using DiscordRPC;
using MultiPresence.Models.SB;
using System.Text;
using MultiPresence.Infrastructure;

namespace MultiPresence.Presence
{
    public class SB
    {
        private static DiscordRpcClient? discord;
        private static DiscordStatusUpdater? updater;
        private const string WorldPattern = "48 8B 0D ?? ?? ?? ?? 48 85 C9 74 33 48 8B 49 30 48 8B D6 48 85 C9 74 07 48 8B 91 68 02 00 00 0F 2E 8A 70 1F 00 00";
        private const string NamesPattern = "48 83 EC 28 80 3D ?? ?? ?? ?? 00 74 0C 48 8D 05 ?? ?? ?? ?? 48 83 C4 28 C3 48 8D 0D ?? ?? ?? ?? E8 ?? ?? ?? ?? C6 05 ?? ?? ?? ?? 01 48 83 C4 28 C3";
        private const string StatsPattern = "48 8B 05 ?? ?? ?? ?? 48 8B 80 C8 00 00 00 48 85 C0 0F 84 ?? ?? ?? ?? 48 63 48 3C 85 C9 78 0F 3B 48 30 7D 0A 48 8B 40 28 48 8B 14 C8 EB 03";
        private const string StatVtablePattern = "48 8D 05 ?? ?? ?? ?? 48 89 06 48 8D 8E 10 01 00 00 40 88 6E 34 48 8D 05 ?? ?? ?? ?? 48 89 46 10";
        private const string StatArrayVtablePattern = "C6 81 14 02 00 00 03 48 8D 05 ?? ?? ?? ?? 48 89 01 33 F6 48 89 B1 18 02 00 00 48 8B F9";
        private const string StatLayoutPattern = "48 8B 48 60 48 85 C9 75 05 0F 57 C0 EB 09 F3 0F 10 84 91 18 01 00 00 80 7F 2A 01";
        private static int cachedProcessId;
        private static ulong cachedModuleBase, worldSlot, namesPool, statsSlot, statVtable, statArrayVtable;
        private static DateTime nextScan;
        private static readonly Dictionary<uint, string> names = new Dictionary<uint, string>();
        private static readonly Dictionary<(ulong Class, string Name), uint> properties = new Dictionary<(ulong Class, string Name), uint>();
        public static void DoAction()
        {
            ResetAddresses();
            GetPID();
            discord = new DiscordRpcClient("1384106213213737002");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/Config/Stellar Blade.json");
            PresenceRuntime.Start(nameof(SB), "SB-Win64-Shipping", RPC);
        }

        private static void GetPID()
        {
            ProcessMonitor.TryAttach("SB-Win64-Shipping");
        }

        private static async Task RPC()
        {
            if (discord == null || updater == null)
            {
                throw new InvalidOperationException("SB: presence is not initialized.");
            }

            if (ProcessMonitor.IsRunning("SB-Win64-Shipping"))
            {
                try
                {
                    if (!Hypervisor.IsAttached && !ProcessMonitor.TryAttach("SB-Win64-Shipping"))
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
                    PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Stellar Blade", placeholders);
                }
                catch (Exception exception) when (exception is not OutOfMemoryException and not StackOverflowException)
                {
                    AppLog.Warning("Stellar Blade memory is not ready.", exception);
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

            var location = Locations.GetName(snapshot.MapName);
            var character = snapshot.CharacterClass.StartsWith("CH_P_EVE_", StringComparison.OrdinalIgnoreCase) ? "EVE" : snapshot.CharacterClass;
            return Task.FromResult(new Dictionary<string, object> { { "state", "In Game" }, { "character", character }, { "characterclass", snapshot.CharacterClass }, { "location", location }, { "region", location }, { "mapname", snapshot.MapName }, { "actorindex", snapshot.ActorIndex }, { "health", snapshot.Health }, { "maxhealth", snapshot.MaxHealth }, { "healthpercentage", Math.Floor(100.0 * snapshot.Health / snapshot.MaxHealth) }, { "shield", snapshot.Shield }, { "maxshield", snapshot.MaxShield }, { "shieldpercentage", snapshot.MaxShield > 0 ? Math.Floor(100.0 * snapshot.Shield / snapshot.MaxShield) : 0 }, { "attackpower", snapshot.AttackPower }, { "rangedattackpower", snapshot.RangedAttackPower }, { "level", snapshot.SpLevel }, { "splevel", snapshot.SpLevel }, { "spexperience", snapshot.SpExperience }, { "beta", snapshot.Beta }, { "maxbeta", snapshot.MaxBeta }, { "burst", snapshot.Burst }, { "maxburst", snapshot.MaxBurst }, { "tachy", snapshot.Tachy }, { "maxtachy", snapshot.MaxTachy } });
        }

        private static void ShowStatus(string text)
        {
            if (discord == null)
            {
                throw new InvalidOperationException("SB: presence is not initialized.");
            }

            discord.SetPresence(new RichPresence()
            {
                Details = text,
                State = "",
                Assets = new Assets()
                {
                    LargeImageKey = "logo",
                    LargeImageText = "Stellar Blade"
                },
                Timestamps = PlaceholderHelper._startTimestamp
            });
        }

        private sealed record Snapshot(bool IsInGame, string MapName, string CharacterClass, int ActorIndex, float Health, float MaxHealth, float Shield, float MaxShield, float AttackPower, float RangedAttackPower, float SpLevel, float SpExperience, float Beta, float MaxBeta, float Burst, float MaxBurst, float Tachy, float MaxTachy);
        private static Snapshot EmptySnapshot()
        {
            return new Snapshot(false, "", "", -1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
        }

        private static Snapshot ReadSnapshot()
        {
            ResolveAddresses();
            var world = Pointer(worldSlot);
            if (world == 0 || !IsA(world, "World"))
            {
                return EmptySnapshot();
            }

            var mapName = ObjectName(world);
            if (Locations.IsMenu(mapName))
            {
                return EmptySnapshot();
            }

            var instanceField = Property(world, "OwningGameInstance", 8);
            var instance = Pointer(world + instanceField);
            if (instance == 0)
            {
                return EmptySnapshot();
            }

            var playersField = Property(instance, "LocalPlayers", 16);
            var players = Pointer(instance + playersField);
            var playerCount = ReadInt(instance + playersField + 8);
            var playerCapacity = ReadInt(instance + playersField + 12);
            if (players == 0 || playerCount == 0)
            {
                return EmptySnapshot();
            }

            if (playerCount < 0 || playerCount > 4 || playerCapacity < playerCount || playerCapacity > 16)
            {
                throw new InvalidDataException("SB: invalid local-player array.");
            }

            var localPlayer = Pointer(players);
            if (localPlayer == 0)
            {
                return EmptySnapshot();
            }

            var controllerField = Property(localPlayer, "PlayerController", 8);
            var controller = Pointer(localPlayer + controllerField);
            if (controller == 0)
            {
                return EmptySnapshot();
            }

            var pawnField = Property(controller, "AcknowledgedPawn", 8);
            var pawn = Pointer(controller + pawnField);
            if (pawn == 0 || !IsA(pawn, "SBCharacter"))
            {
                return EmptySnapshot();
            }

            var characterClass = ObjectName(Pointer(pawn + 0x10));
            var singleton = Pointer(statsSlot);
            if (singleton == 0)
            {
                return EmptySnapshot();
            }

            var manager = Pointer(singleton + 0xC8);
            if (manager == 0)
            {
                return EmptySnapshot();
            }

            var count = ReadInt(manager + 0x30);
            var capacity = ReadInt(manager + 0x34);
            var actorIndex = ReadInt(manager + 0x3C);
            if (count == 0 || actorIndex < 0)
            {
                return EmptySnapshot();
            }

            if (count < 0 || count > 65536 || capacity < count || capacity > 131072 || actorIndex >= count)
            {
                throw new InvalidDataException("SB: invalid current-actor array.");
            }

            var actors = Pointer(manager + 0x28);
            if (actors == 0)
            {
                return EmptySnapshot();
            }

            var actor = Pointer(actors + (ulong)actorIndex * 8);
            if (actor == 0)
            {
                return EmptySnapshot();
            }

            if (Pointer(actor) != statVtable || Pointer(actor + 0x110) != statArrayVtable)
            {
                throw new InvalidDataException("SB: actor/stat layout does not match native signatures.");
            }

            var stats = Hypervisor.Read<float>(actor + 0x118, 131, true);
            float Stat(int index)
            {
                var value = stats[index];
                if (!float.IsFinite(value) || value < 0 || value > 100000000)
                {
                    throw new InvalidDataException("SB: invalid actor stat.");
                }

                return value;
            }

            var health = Stat(1);
            var maxHealth = Stat(2);
            var shield = Stat(7);
            var maxShield = Stat(8);
            if (maxHealth <= 0 || health > maxHealth || shield > maxShield)
            {
                throw new InvalidDataException("SB: invalid health/shield state.");
            }

            var snapshot = new Snapshot(true, mapName, characterClass, actorIndex, health, maxHealth, shield, maxShield, Stat(26), Stat(29), Stat(105), Stat(106), Stat(14), Stat(15), Stat(18), Stat(19), Stat(22), Stat(23));
            if (world != Pointer(worldSlot) || mapName != ObjectName(world) || instance != Pointer(world + instanceField) || players != Pointer(instance + playersField) || localPlayer != Pointer(players) || controller != Pointer(localPlayer + controllerField) || pawn != Pointer(controller + pawnField) || singleton != Pointer(statsSlot) || manager != Pointer(singleton + 0xC8) || actors != Pointer(manager + 0x28) || actorIndex != ReadInt(manager + 0x3C) || actor != Pointer(actors + (ulong)actorIndex * 8))
            {
                throw new InvalidDataException("SB: scene changed during the read; retry next poll.");
            }

            return snapshot;
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

                var size = Hypervisor.Read<uint>(cls + 0x58, true);
                var visitedFields = new HashSet<ulong>();
                for (var field = Pointer(cls + 0x50); field != 0 && visitedFields.Count < 2048; field = Pointer(field + 0x20))
                {
                    if (!visitedFields.Add(field))
                    {
                        break;
                    }

                    if (ReadName(Hypervisor.Read<uint>(field + 0x28, true)) != name)
                    {
                        continue;
                    }

                    var offset = Hypervisor.Read<uint>(field + 0x4C, true);
                    if (ReadInt(field + 0x38) != 1 || ReadInt(field + 0x3C) != elementSize || offset < 0x28 || size > 0x100000 || (ulong)offset + (ulong)elementSize > size)
                    {
                        throw new InvalidDataException("SB: reflected property layout changed: " + name);
                    }

                    properties[(initialClass, name)] = offset;
                    return offset;
                }
            }

            throw new InvalidDataException("SB: required Unreal property not found: " + name);
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
                throw new InvalidDataException("SB: invalid FName block.");
            }

            var blockData = Pointer(namesPool + 0x10 + block * 8);
            if (blockData == 0)
            {
                throw new InvalidDataException("SB: missing FName block.");
            }

            var offset = (index & 0xFFFF) * 2;
            var header = Hypervisor.Read<ushort>(blockData + offset, true);
            var length = header >> 6;
            var wide = (header & 1) != 0;
            var byteCount = length * (wide ? 2 : 1);
            if (length == 0 || length > 1023 || offset + 2 + byteCount > 131072 || (block == currentBlock && offset + 2 + byteCount > Hypervisor.Read<uint>(namesPool + 12, true)))
            {
                throw new InvalidDataException("SB: invalid FName entry.");
            }

            var bytes = Hypervisor.ReadArray(blockData + offset + 2, byteCount, true);
            var name = (wide ? Encoding.Unicode : Encoding.UTF8).GetString(bytes);
            if (name.Contains('\0'))
            {
                throw new InvalidDataException("SB: invalid FName text.");
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
            cachedProcessId = 0;
            cachedModuleBase = worldSlot = namesPool = statsSlot = statVtable = statArrayVtable = 0;
            nextScan = DateTime.MinValue;
            names.Clear();
            properties.Clear();
        }

        private static void ResolveAddresses()
        {
            var process = Hypervisor.Process ?? throw new InvalidDataException("SB: game is not attached.");
            var module = process.MainModule ?? throw new InvalidDataException("SB: module not loaded.");
            var moduleBase = (ulong)module.BaseAddress.ToInt64();
            if (cachedProcessId != process.Id || cachedModuleBase != moduleBase)
            {
                ResetAddresses();
                cachedProcessId = process.Id;
                cachedModuleBase = moduleBase;
            }

            if (worldSlot != 0)
            {
                return;
            }

            if (DateTime.UtcNow < nextScan)
            {
                throw new InvalidDataException("SB: waiting to retry patterns.");
            }

            nextScan = DateTime.UtcNow.AddSeconds(30);
            ulong Resolve(string pattern, int operand, int length)
            {
                var match = FindUniquePattern(moduleBase, module.ModuleMemorySize, pattern);
                var target = checked((ulong)((long)match + length + ReadInt(match + (ulong)operand)));
                if (target < moduleBase || target + 8 > moduleBase + (ulong)module.ModuleMemorySize)
                {
                    throw new InvalidDataException("SB: RIP-relative target outside module.");
                }

                return target;
            }

            var world = Resolve(WorldPattern, 3, 7);
            var pool = Resolve(NamesPattern, 16, 20);
            var stats = Resolve(StatsPattern, 3, 7);
            var vtable = Resolve(StatVtablePattern, 3, 7);
            var arrayVtable = Resolve(StatArrayVtablePattern, 10, 14);
            FindUniquePattern(moduleBase, module.ModuleMemorySize, StatLayoutPattern);
            namesPool = pool;
            if (ReadName(0) != "None")
            {
                namesPool = 0;
                names.Clear();
                throw new InvalidDataException("SB: FName layout changed.");
            }

            statsSlot = stats;
            statVtable = vtable;
            statArrayVtable = arrayVtable;
            worldSlot = world;
        }

        private static ulong FindUniquePattern(ulong moduleBase, int moduleSize, string signature)
        {
            if (moduleSize < 64)
            {
                throw new InvalidDataException("SB: invalid module size.");
            }

            var peOffset = Hypervisor.Read<uint>(moduleBase + 0x3C, true);
            if (peOffset > (uint)moduleSize - 24)
            {
                throw new InvalidDataException("SB: invalid PE offset.");
            }

            var pe = moduleBase + peOffset;
            if (peOffset > (uint)moduleSize - 24 || Hypervisor.Read<uint>(pe, true) != 0x4550 || Hypervisor.Read<ushort>(pe + 4, true) != 0x8664)
            {
                throw new InvalidDataException("SB: invalid PE header.");
            }

            var sectionCount = Hypervisor.Read<ushort>(pe + 6, true);
            var optionalSize = Hypervisor.Read<ushort>(pe + 20, true);
            var sectionTable = pe + 24 + optionalSize;
            if (sectionTable + (ulong)sectionCount * 40 > moduleBase + (ulong)moduleSize)
            {
                throw new InvalidDataException("SB: invalid section table.");
            }

            var pattern = SignaturePattern.Parse(signature);
            var prefix = pattern.Bytes.TakeWhile(value => value.HasValue).Select(value => value.GetValueOrDefault()).ToArray();
            if (prefix.Length == 0)
            {
                throw new InvalidDataException("SB: pattern needs a fixed prefix.");
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
                    throw new InvalidDataException("SB: invalid executable section.");
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
                            throw new InvalidDataException("SB: manager pattern is ambiguous.");
                        }

                        match = address;
                    }
                }
            }

            return match != 0 ? match : throw new InvalidDataException("SB: manager pattern was not found.");
        }

        private static void InitializeDiscord()
        {
            if (discord == null)
            {
                throw new InvalidOperationException("SB: presence is not initialized.");
            }

            discord.Initialize();
        }
    }
}
