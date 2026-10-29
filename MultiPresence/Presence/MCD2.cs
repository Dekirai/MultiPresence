using DiscordRPC;
using MultiPresence.Infrastructure;
using MultiPresence.Models.MCD2;
using MultiPresence.Runtime;
using System.Text;

namespace MultiPresence.Presence
{
    public class MCD2
    {
        private static DiscordRpcClient? discord;
        private static DiscordStatusUpdater? updater;
        private const string ProcessName = "Dungeons-Win64-Shipping";
        private const string GameName = "Minecraft Dungeons II";
        private const string WorldPattern = "48 8B 05 ?? ?? ?? ?? 48 85 C0 74 ?? 48 8B 48 38 48 85 C9 74 ?? 48 8B 01 48 FF A0 18 03 00 00";
        private const string NamesPattern = "48 8D 05 ?? ?? ?? ?? 48 8B D1 48 8B C8 48 83 C4 20 5B E9 ?? ?? ?? ??";
        private const string AreaLayoutPattern = "41 0F 10 00 41 8B 40 10 48 89 91 B8 00 00 00 0F 11 81 C0 00 00 00 89 81 D0 00 00 00 C6 81 04 01 00 00 01 C3";
        private static int cachedProcessId;
        private static ulong cachedModuleBase, cachedWorld, worldSlot, namesPool;
        private static DateTime nextScan;
        private static readonly Dictionary<uint, string> names = new Dictionary<uint, string>();
        private static readonly Dictionary<(ulong Class, string Name), ulong> fields = new Dictionary<(ulong Class, string Name), ulong>();

        public static void DoAction()
        {
            ResetAddresses();
            GetPID();
            discord = new DiscordRpcClient("1563088056142008380");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/Config/Minecraft Dungeons II.json");
            PresenceRuntime.Start(nameof(MCD2), ProcessName, RPC);
        }

        private static void GetPID()
        {
            ProcessMonitor.TryAttach(ProcessName);
        }

        private static async Task RPC()
        {
            if (discord == null || updater == null)
            {
                throw new InvalidOperationException("MCD2: presence is not initialized.");
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
                    PlaceholderHelper.UpdateDiscordStatus(discord, updater, GameName, placeholders);
                }
                catch (Exception exception) when (exception is not OutOfMemoryException and not StackOverflowException)
                {
                    AppLog.Warning("Minecraft Dungeons II memory is not ready.", exception);
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

        private sealed record Snapshot(bool IsInGame, int Level, float Hp, float MaxHp, string Location, string Quest, string Objective, int ItemPower);

        private static Snapshot EmptySnapshot()
        {
            return new Snapshot(false, 0, 0, 0, "", "", "", 0);
        }

        private static Task<Dictionary<string, object>> GeneratePlaceholders(Snapshot snapshot)
        {
            if (!snapshot.IsInGame)
            {
                return Task.FromResult(new Dictionary<string, object>());
            }

            return Task.FromResult(new Dictionary<string, object> { { "state", "In Game" }, { "level", snapshot.Level }, { "hp", Math.Ceiling(snapshot.Hp) }, { "maxhp", Math.Ceiling(snapshot.MaxHp) }, { "health", Math.Ceiling(snapshot.Hp) }, { "maxhealth", Math.Ceiling(snapshot.MaxHp) }, { "location", snapshot.Location }, { "quest", snapshot.Quest }, { "objective", snapshot.Objective }, { "itempower", snapshot.ItemPower } });
        }

        private static void ShowStatus(string text)
        {
            if (discord == null)
            {
                throw new InvalidOperationException("MCD2: presence is not initialized.");
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

        private static Snapshot ReadSnapshot()
        {
            ResolveAddresses();
            var world = Pointer(worldSlot);
            if (cachedWorld != world)
            {
                fields.Clear();
                cachedWorld = world;
            }

            if (world == 0 || !IsA(world, "World"))
            {
                return EmptySnapshot();
            }

            var instance = ObjectProperty(world, "OwningGameInstance");
            if (instance == 0)
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
            var controller = ObjectProperty(localPlayer, "PlayerController");
            var pawn = ObjectProperty(controller, "AcknowledgedPawn");
            if (pawn == 0 || !IsA(pawn, "PlayerCharacter") || ObjectProperty(controller, "Pawn") != pawn)
            {
                return EmptySnapshot();
            }

            var ability = ObjectProperty(pawn, "AbilitySystemComponent");
            var inventory = ObjectProperty(pawn, "InventoryManagerComponent");
            var playerWorld = ObjectProperty(pawn, "PlayerWorldStateComponent");
            if (ability == 0 || inventory == 0 || playerWorld == 0)
            {
                return EmptySnapshot();
            }

            var attributes = ReadArray(ability + Property(ability, "SpawnedAttributes", 16), 128);
            ulong health = 0, experience = 0;
            for (var index = 0; index < attributes.Count; index++)
            {
                var attribute = Pointer(attributes.Data + (ulong)index * 8);
                if (IsA(attribute, "ATR_Health"))
                {
                    health = attribute;
                }
                else if (IsA(attribute, "ATR_XP"))
                {
                    experience = attribute;
                }
            }

            if (health == 0 || experience == 0)
            {
                return EmptySnapshot();
            }

            var hp = ReadAttribute(health, "Health");
            var maxHp = ReadAttribute(health, "HealthMax");
            var levelValue = ReadAttribute(experience, "Level");
            if (!float.IsFinite(hp) || !float.IsFinite(maxHp) || hp < 0 || maxHp <= 0 || maxHp > 10000000 || hp > maxHp + 1 || !float.IsFinite(levelValue) || levelValue < 1 || levelValue > 10000 || levelValue != MathF.Truncate(levelValue))
            {
                throw new InvalidDataException("MCD2: invalid player attributes.");
            }

            var area = ReadName(Hypervisor.Read<uint>(playerWorld + 0xB8, true));
            var location = Locations.GetName(area);
            if (location.Length == 0)
            {
                location = ObjectName(world);
            }

            var itemPower = ReadItemPower(inventory);
            var quest = ReadQuest(world, controller);
            if (world != Pointer(worldSlot) || instance != ObjectProperty(world, "OwningGameInstance") || players.Data != Pointer(playersAddress) || localPlayer != Pointer(players.Data) || controller != ObjectProperty(localPlayer, "PlayerController") || pawn != ObjectProperty(controller, "AcknowledgedPawn") || pawn != ObjectProperty(controller, "Pawn") || ability != ObjectProperty(pawn, "AbilitySystemComponent") || playerWorld != ObjectProperty(pawn, "PlayerWorldStateComponent") || area != ReadName(Hypervisor.Read<uint>(playerWorld + 0xB8, true)))
            {
                return EmptySnapshot();
            }

            return new Snapshot(true, (int)levelValue, hp, maxHp, location, quest.Title, quest.Objective, itemPower);
        }

        private static int ReadItemPower(ulong inventory)
        {
            var inventoryType = Pointer(inventory + 0x10);
            var replicatedField = Field(inventoryType, "ReplicatedItems");
            var containerType = Pointer(replicatedField + 0x78);
            var slotsField = Field(containerType, "Items", 16);
            var slotElement = Pointer(slotsField + 0x80);
            var slotType = Pointer(slotElement + 0x78);
            var slotSize = ReadInt(slotType + 0x58);
            var slotTagOffset = InventoryFieldOffset(slotType, Field(slotType, "TypeTag", 8));
            var entriesField = Field(slotType, "ItemInventoryEntries", 16);
            var entryElement = Pointer(entriesField + 0x80);
            var entryType = Pointer(entryElement + 0x78);
            var entrySize = ReadInt(entryType + 0x58);
            var equippedOffset = InventoryFieldOffset(entryType, Field(entryType, "EquippedSlot", 8));
            var itemField = Field(entryType, "ItemData");
            var itemType = Pointer(itemField + 0x78);
            var generatorField = Field(itemType, "GeneratorData");
            var generatorType = Pointer(generatorField + 0x78);
            var valuesField = Field(generatorType, "PowerGeneratorValues");
            var valuesType = Pointer(valuesField + 0x78);
            var powerField = Field(valuesType, "ItemPower", 4);
            var powerOffset = InventoryFieldOffset(entryType, itemField)
                + InventoryFieldOffset(itemType, generatorField)
                + InventoryFieldOffset(generatorType, valuesField)
                + InventoryFieldOffset(valuesType, powerField);

            if (slotSize <= 0 || slotSize > 4096 || entrySize <= 0 || entrySize > 4096 ||
                slotSize != ReadInt(slotElement + 0x34) || entrySize != ReadInt(entryElement + 0x34) ||
                powerOffset + 4 > (ulong)entrySize)
            {
                throw new InvalidDataException("Invalid inventory structure.");
            }

            var slotsAddress = inventory + InventoryFieldOffset(inventoryType, replicatedField)
                + InventoryFieldOffset(containerType, slotsField);
            var slots = ReadInventoryArray(slotsAddress);
            var entriesOffset = InventoryFieldOffset(slotType, entriesField);
            long total = 0;
            int count = 0;

            for (int index = 0; index < slots.Count; index++)
            {
                var slot = slots.Data + (ulong)(index * slotSize);
                var slotTag = ReadName((uint)ReadInt(slot + slotTagOffset));
                if (!IsPowerEquipmentSlot(slotTag))
                {
                    continue;
                }

                var entries = ReadInventoryArray(slot + entriesOffset);
                for (int itemIndex = 0; itemIndex < entries.Count; itemIndex++)
                {
                    var item = entries.Data + (ulong)(itemIndex * entrySize);
                    var equippedTag = ReadName((uint)ReadInt(item + equippedOffset));
                    if (!string.Equals(equippedTag, slotTag, StringComparison.Ordinal))
                    {
                        continue;
                    }

                    var power = Hypervisor.Read<float>(item + powerOffset, true);
                    if (!float.IsFinite(power))
                    {
                        throw new InvalidDataException("Invalid item power.");
                    }

                    if (power < 0f)
                    {
                        continue;
                    }

                    var rounded = MathF.Round(2f * power + 0.5f, MidpointRounding.ToEven);
                    if (rounded >= int.MaxValue)
                    {
                        throw new InvalidDataException("Invalid item power.");
                    }

                    int itemPower = (int)rounded >> 1;
                    if (itemPower >= 1)
                    {
                        total += itemPower;
                        count++;
                    }
                }
            }

            return count == 0 ? 1 : checked((int)(total / count));
        }

        private static ulong InventoryFieldOffset(ulong structure, ulong field)
        {
            int structureSize = ReadInt(structure + 0x58);
            int offset = ReadInt(field + 0x48);
            int size = ReadInt(field + 0x34);
            if (structureSize <= 0 || structureSize > 1048576 || offset < 0 ||
                size <= 0 || size > structureSize || offset > structureSize - size)
            {
                throw new InvalidDataException("Invalid inventory field.");
            }

            return (ulong)offset;
        }

        private static (ulong Data, int Count) ReadInventoryArray(ulong address)
        {
            int count = ReadInt(address + 8);
            int capacity = ReadInt(address + 12);
            if (count < 0 || count > 4096 || capacity < count || capacity > 4096)
            {
                throw new InvalidDataException("Invalid inventory array.");
            }

            if (count == 0)
            {
                return (0, 0);
            }

            var data = Pointer(address);
            if (data == 0)
            {
                throw new InvalidDataException("Invalid inventory array.");
            }

            return (data, count);
        }

        private static bool IsPowerEquipmentSlot(string tag)
        {
            return tag.StartsWith("SW.ItemSlot.Equipment.", StringComparison.Ordinal)
                && !string.Equals(tag, "SW.ItemSlot.Equipment.Talisman", StringComparison.Ordinal)
                && !tag.StartsWith("SW.ItemSlot.Equipment.Talisman.", StringComparison.Ordinal)
                && !string.Equals(tag, "SW.ItemSlot.Equipment.Cosmetic", StringComparison.Ordinal)
                && !tag.StartsWith("SW.ItemSlot.Equipment.Cosmetic.", StringComparison.Ordinal);
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
                throw new InvalidDataException("MCD2: invalid FName block.");
            }

            var blockData = Pointer(namesPool + 0x10 + block * 8);
            if (blockData == 0)
            {
                throw new InvalidDataException("MCD2: missing FName block.");
            }

            var offset = (index & 0xFFFF) * 2;
            var header = Hypervisor.Read<ushort>(blockData + offset, true);
            var length = header >> 6;
            var wide = (header & 1) != 0;
            var byteCount = length * (wide ? 2 : 1);
            if (length == 0 || length > 1023 || offset + 2 + byteCount > 131072 || (block == currentBlock && offset + 2 + byteCount > Hypervisor.Read<uint>(namesPool + 12, true)))
            {
                throw new InvalidDataException("MCD2: invalid FName entry.");
            }

            var bytes = Hypervisor.ReadArray(blockData + offset + 2, byteCount, true);
            var name = (wide ? Encoding.Unicode : Encoding.UTF8).GetString(bytes);
            if (name.Contains('\0'))
            {
                throw new InvalidDataException("MCD2: invalid FName text.");
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

        private static ulong FindUniquePattern(ulong moduleBase, int moduleSize, string signature)
        {
            if (moduleSize < 64)
            {
                throw new InvalidDataException("MCD2: invalid module size.");
            }

            var peOffset = Hypervisor.Read<uint>(moduleBase + 0x3C, true);
            if (peOffset > (uint)moduleSize - 24)
            {
                throw new InvalidDataException("MCD2: invalid PE offset.");
            }

            var pe = moduleBase + peOffset;
            if (peOffset > (uint)moduleSize - 24 || Hypervisor.Read<uint>(pe, true) != 0x4550 || Hypervisor.Read<ushort>(pe + 4, true) != 0x8664)
            {
                throw new InvalidDataException("MCD2: invalid PE header.");
            }

            var sectionCount = Hypervisor.Read<ushort>(pe + 6, true);
            var optionalSize = Hypervisor.Read<ushort>(pe + 20, true);
            var sectionTable = pe + 24 + optionalSize;
            if (sectionTable + (ulong)sectionCount * 40 > moduleBase + (ulong)moduleSize)
            {
                throw new InvalidDataException("MCD2: invalid section table.");
            }

            var pattern = SignaturePattern.Parse(signature);
            var prefix = pattern.Bytes.TakeWhile(value => value.HasValue).Select(value => value.GetValueOrDefault()).ToArray();
            if (prefix.Length == 0)
            {
                throw new InvalidDataException("MCD2: pattern needs a fixed prefix.");
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
                    throw new InvalidDataException("MCD2: invalid executable section.");
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
                            throw new InvalidDataException("MCD2: manager pattern is ambiguous.");
                        }

                        match = address;
                    }
                }
            }

            return match != 0 ? match : throw new InvalidDataException("MCD2: manager pattern was not found.");
        }


        private static float ReadAttribute(ulong attribute, string name)
        {
            var field = Field(Pointer(attribute + 0x10), name, 16);
            var structure = Pointer(field + 0x78);
            var value = Field(structure, "CurrentValue", 4);
            return Hypervisor.Read<float>(attribute + FieldOffset(field) + FieldOffset(value), true);
        }

        private static (string Title, string Objective) ReadQuest(ulong world, ulong controller)
        {
            var proxy = FindWorldSubsystem(world, "QuestRunnerProxySubsystem");
            if (proxy == 0)
            {
                return ("", "");
            }

            var runner = Pointer(proxy + 0x48);
            if (runner == 0 || !IsA(runner, "QuestRunner"))
            {
                return ("", "");
            }

            var focused = ObjectProperty(runner, "FocusedQuest");
            var playerState = ObjectProperty(controller, "PlayerState");
            if (focused == 0 || playerState == 0)
            {
                return ("", "");
            }

            var questId = ReadString(focused + Property(focused, "ID", 16));
            var owners = ReadArray(runner + Property(runner, "PlayerFocusedQuests", 16), 16);
            var playerIndex = ReadInt(playerState + 0xC);
            var owned = false;
            for (var index = 0; index < owners.Count; index++)
            {
                var entry = owners.Data + (ulong)index * 24;
                if (ReadInt(entry) == playerIndex && ReadString(entry + 8) == questId)
                {
                    owned = true;
                    break;
                }
            }

            if (!owned || questId.Length == 0)
            {
                return ("", "");
            }

            var descriptor = ObjectProperty(focused, "ReferenceQuest");
            if (descriptor == 0)
            {
                return ("", "");
            }

            var title = ReadText(descriptor + Property(descriptor, "UiLabel", 16));
            var objectives = new List<string>();
            CollectObjectives(ObjectProperty(focused, "Tasks"), objectives, new HashSet<ulong>(), 0);
            if (focused != ObjectProperty(runner, "FocusedQuest"))
            {
                return ("", "");
            }

            return (title, string.Join(" / ", objectives.Distinct(StringComparer.Ordinal)));
        }

        private static void CollectObjectives(ulong task, List<string> objectives, HashSet<ulong> visited, int depth)
        {
            if (task == 0 || depth > 16 || visited.Count >= 256 || !visited.Add(task) || objectives.Count >= 8)
            {
                return;
            }

            var state = Field(Pointer(task + 0x10), "State");
            if (Hypervisor.Read<byte>(task + FieldOffset(state), true) != 1)
            {
                return;
            }

            var previousCount = objectives.Count;
            if (IsA(task, "QuestTasks"))
            {
                var children = ReadArray(task + Property(task, "Tasks", 16), 256);
                for (var index = 0; index < children.Count; index++)
                {
                    CollectObjectives(Pointer(children.Data + (ulong)index * 8), objectives, visited, depth + 1);
                }
            }

            if (objectives.Count == previousCount)
            {
                var goal = ObjectProperty(task, "ReferenceGoal");
                if (goal == 0 && IsA(task, "QuestTasks"))
                {
                    goal = ObjectProperty(task, "ReferenceGoals");
                }

                if (goal != 0)
                {
                    var text = ReadText(goal + Property(goal, "UiLabel", 16));
                    if (text.Length > 0)
                    {
                        objectives.Add(text);
                    }
                }
            }
        }

        private static ulong FindWorldSubsystem(ulong world, string className)
        {
            var map = world + 0x930;
            var slots = ReadArray(map, 256);
            var bitCount = ReadInt(map + 0x28);
            if (bitCount != slots.Count)
            {
                throw new InvalidDataException("MCD2: invalid world subsystem map.");
            }

            var bits = Pointer(map + 0x20);
            if (bits == 0)
            {
                bits = map + 0x10;
            }

            for (var index = 0; index < slots.Count; index++)
            {
                if ((Hypervisor.Read<uint>(bits + (ulong)(index / 32) * 4, true) & (1u << (index % 32))) == 0)
                {
                    continue;
                }

                var subsystem = Pointer(slots.Data + (ulong)index * 24 + 8);
                if (subsystem != 0 && Pointer(subsystem + 0x20) == world && IsA(subsystem, className))
                {
                    return subsystem;
                }
            }

            return 0;
        }

        private static string ReadText(ulong address)
        {
            var data = Pointer(address);
            if (data == 0)
            {
                return "";
            }

            var typeFunction = Pointer(Pointer(data) + 0x58);
            var code = Hypervisor.ReadArray(typeFunction, 3, true);
            if (code[0] == 0xB0 && code[1] == 11 && code[2] == 0xC3)
            {
                var reference = Pointer(data + 0x18);
                var entry = reference == 0 ? 0 : Pointer(reference + 0x20);
                return entry == 0 ? "" : ReadString(entry + 0x10);
            }

            if (code[0] == 0x32 && code[1] == 0xC0 && code[2] == 0xC3)
            {
                return ReadString(data + 0x20);
            }

            return "";
        }

        private static string ReadString(ulong address)
        {
            var value = ReadArray(address, 4096);
            if (value.Count == 0)
            {
                return "";
            }

            var bytes = Hypervisor.ReadArray(value.Data, value.Count * 2, true);
            if (bytes[^1] != 0 || bytes[^2] != 0)
            {
                throw new InvalidDataException("MCD2: invalid string terminator.");
            }

            return Encoding.Unicode.GetString(bytes, 0, bytes.Length - 2);
        }

        private static (ulong Data, int Count) ReadArray(ulong address, int limit)
        {
            var data = Pointer(address);
            var count = ReadInt(address + 8);
            var capacity = ReadInt(address + 12);
            if (count < 0 || count > limit || capacity < count || capacity > Math.Max(limit * 4, 16) || (count > 0 && data == 0))
            {
                throw new InvalidDataException("MCD2: invalid array.");
            }

            return (data, count);
        }

        private static ulong Field(ulong structure, string name, int expectedSize = 0)
        {
            if (fields.TryGetValue((structure, name), out var cached))
            {
                return cached;
            }

            var visitedClasses = new HashSet<ulong>();
            for (var cls = structure; cls != 0 && visitedClasses.Count < 32; cls = Pointer(cls + 0x40))
            {
                if (!visitedClasses.Add(cls))
                {
                    break;
                }

                var size = Hypervisor.Read<uint>(cls + 0x58, true);
                var visitedFields = new HashSet<ulong>();
                for (var field = Pointer(cls + 0x50); field != 0 && visitedFields.Count < 2048; field = Pointer(field + 0x18))
                {
                    if (!visitedFields.Add(field))
                    {
                        break;
                    }

                    if (ReadName(Hypervisor.Read<uint>(field + 0x20, true)) != name)
                    {
                        continue;
                    }

                    var elementSize = ReadInt(field + 0x34);
                    if (ReadInt(field + 0x30) != 1 || elementSize <= 0 || (expectedSize > 0 && elementSize != expectedSize) || size > 0x100000 || FieldOffset(field) + (ulong)elementSize > size)
                    {
                        throw new InvalidDataException("MCD2: reflected property layout changed: " + name);
                    }

                    fields[(structure, name)] = field;
                    return field;
                }
            }

            throw new InvalidDataException("MCD2: required Unreal property not found: " + name);
        }

        private static uint FieldOffset(ulong field)
        {
            return Hypervisor.Read<uint>(field + 0x48, true);
        }

        private static uint Property(ulong instance, string name, int elementSize)
        {
            return FieldOffset(Field(Pointer(instance + 0x10), name, elementSize));
        }

        private static ulong ObjectProperty(ulong instance, string name)
        {
            return instance == 0 ? 0 : Pointer(instance + Property(instance, name, 8));
        }

        private static void ResetAddresses()
        {
            cachedProcessId = 0;
            cachedModuleBase = cachedWorld = worldSlot = namesPool = 0;
            nextScan = DateTime.MinValue;
            names.Clear();
            fields.Clear();
        }

        private static void ResolveAddresses()
        {
            var process = Hypervisor.Process ?? throw new InvalidDataException("MCD2: game is not attached.");
            var module = process.MainModule ?? throw new InvalidDataException("MCD2: module not loaded.");
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
                throw new InvalidDataException("MCD2: waiting to retry patterns.");
            }

            nextScan = DateTime.UtcNow.AddSeconds(30);
            ulong Resolve(string pattern)
            {
                var match = FindUniquePattern(moduleBase, module.ModuleMemorySize, pattern);
                var target = checked((ulong)((long)match + 7 + ReadInt(match + 3)));
                if (target < moduleBase || target + 8 > moduleBase + (ulong)module.ModuleMemorySize)
                {
                    throw new InvalidDataException("MCD2: RIP-relative target outside module.");
                }

                return target;
            }

            var world = Resolve(WorldPattern);
            var pool = Resolve(NamesPattern);
            FindUniquePattern(moduleBase, module.ModuleMemorySize, AreaLayoutPattern);
            namesPool = pool;
            if (ReadName(0) != "None")
            {
                namesPool = 0;
                names.Clear();
                throw new InvalidDataException("MCD2: FName layout changed.");
            }

            worldSlot = world;
        }

        private static void InitializeDiscord()
        {
            if (discord == null)
            {
                throw new InvalidOperationException("MCD2: presence is not initialized.");
            }

            discord.Initialize();
        }
    }
}
