using MultiPresence.Runtime;
using MultiPresence.Infrastructure;
using DiscordRPC;
using MultiPresence.Models.RE4R;

namespace MultiPresence.Presence
{
    public class RE4R
    {
        private const string FlowPattern = "48 8B 15 ?? ?? ?? ?? 48 8B CE E8 ?? ?? ?? ?? 84 C0 0F 94 C0 EB 4A 48 8B 15 ?? ?? ?? ?? 48 8B 9A E0 00 00 00";
        private const string FlowLayoutPattern = "48 83 EC 28 48 8B 52 28 48 85 D2 74 1B 48 8B 52 10 48 8B 02 4C 8B 40 F0 49 8B 40 78 FF D0 84 C0";
        private const string LevelMapPattern = "48 8B 15 ?? ?? ?? ?? E8 ?? ?? ?? ?? 85 C0 75 0A B8 01 00 00 00 48 83 C4 28 C3 8D 48 FF BA 02 00 00 00";
        private const string StageLayoutPattern = "8B 82 A8 01 00 00 C3 CC CC CC CC CC CC CC CC CC C7 42 20 FF FF FF FF 48 C7 42 14 FF FF FF FF C3";
        private const string CharacterPattern = "48 8B 15 ?? ?? ?? ?? E9 ?? ?? ?? ?? 48 89 5C 24 08 57 48 83 EC 20 48 8B 02 48 8B F9 48 8B CA 48 8B DA";
        private const string CampaignPattern = "48 8B 05 ?? ?? ?? ?? 81 78 30 54 56 00 00 0F 9D C0 C3 CC CC CC CC CC CC CC CC CC CC CC CC CC CC";
        private const string MapPattern = "48 8B 05 ?? ?? ?? ?? 8B 40 78 C3 CC CC CC CC CC 48 89 5C 24 08 57 48 83 EC 20 48 8B BA F0 00 00 00";
        private const string PurposePattern = "48 8B 15 ?? ?? ?? ?? 48 8B CB E8 ?? ?? ?? ?? 4C 8B 05 ?? ?? ?? ?? 48 8D 4D 07 44 8B C8 48 8B D3";
        private const string MercenariesPattern = "4C 8B 0D ?? ?? ?? ?? 4C 8B C7 48 8B 15 ?? ?? ?? ?? 48 8B CE 48 8B 1D ?? ?? ?? ?? 44 88 74 24 20 E8 ?? ?? ?? ?? 4C 8B C0 48 8B D3 48 8B CE E8 ?? ?? ?? ?? 4C 8B 0D ?? ?? ?? ??";
        private const string ScoreLayoutPattern = "48 8B 42 18 48 63 C8 48 8B 40 10 48 99 48 F7 F9 C3 CC CC CC CC CC CC CC CC CC CC CC CC CC CC CC";
        private static DiscordRpcClient? discord;
        private static DiscordStatusUpdater? updater;
        private static int cachedProcessId, cachedModuleSize;
        private static ulong cachedModuleBase, characterSlot, campaignSlot, flowSlot, mapSlot, levelMapSlot, purposeSlot, mercenariesSlot, mercenaryCallbackSlot;
        private static DateTime nextScan;
        public static void DoAction()
        {
            ResetAddresses();
            GetPID();
            discord = new DiscordRpcClient("1343156987021754399");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/config/Resident Evil 4 Remake.json");
            PresenceRuntime.Start(nameof(RE4R), "re4", RPC);
        }

        private static void GetPID()
        {
            ProcessMonitor.TryAttach("re4");
        }

        private static async Task RPC()
        {
            if (discord == null || updater == null)
            {
                throw new InvalidOperationException("RE4R: presence is not initialized.");
            }

            if (ProcessMonitor.IsRunning("re4"))
            {
                try
                {
                    if (!Hypervisor.IsAttached && !ProcessMonitor.TryAttach("re4"))
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
                    PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Resident Evil 4", placeholders, GetMode(snapshot.CampaignKey));
                }
                catch (Exception exception) when (exception is not OutOfMemoryException and not StackOverflowException)
                {
                    AppLog.Warning("Resident Evil 4 memory is not ready.", exception);
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

            var mode = GetMode(snapshot.CampaignKey);
            var chapter = await Chapters.GetChapter(snapshot.ChapterId);
            var mainChapter = mode == "Main Story" && int.TryParse(chapter, out var number) ? number : 0;
            var location = Locations.Get(snapshot.StageId, snapshot.SectionId, 0, mainChapter, snapshot.SectionId > 0);
            var mercMap = snapshot.MercenaryStage is >= 0 and <= 3 ? new[]
            {
                "Village",
                "Castle",
                "Island",
                "The Docks"
            }[snapshot.MercenaryStage] : (snapshot.ChapterId / 1000) switch
            {
                41 => "Village",
                42 => "Castle",
                43 => "Island",
                44 => "The Docks",
                _ => "Unknown map",
            };
            var percentage = Math.Floor((double)snapshot.Health / snapshot.MaxHealth * 100);
            var objective = GameText.Get(snapshot.ObjectiveGuid);
            return new Dictionary<string, object>
            {
                { "character", mode == "Mercenaries" && snapshot.MercenaryCharacter is >= 0 and <= 5 ? new[] { "Leon", "Luis", "Krauser", "HUNK", "Ada", "Wesker" }[snapshot.MercenaryCharacter] : GetCharacter(snapshot.CharacterId, snapshot.CampaignKey, snapshot.ChapterId) },
                { "characterid", snapshot.CharacterId },
                { "costumeid", snapshot.MercenaryCostume },
                { "mercenarycharacterid", snapshot.MercenaryCharacter },
                { "mercenarystageid", snapshot.MercenaryStage },
                { "state", mode },
                { "mode", mode },
                { "campaignkey", snapshot.CampaignKey },
                { "chapter", chapter },
                { "chapterid", snapshot.ChapterId },
                { "difficulty", await Difficulties.GetDifficulty(snapshot.DifficultyId) },
                { "difficultyid", snapshot.DifficultyId },
                { "stageid", snapshot.StageId },
                { "sectionid", snapshot.SectionId },
                { "sectiongroup", snapshot.SectionGroup },
                { "mapsceneid", snapshot.MapSceneId },
                { "region", mode == "Mercenaries" ? mercMap : location.Region },
                { "room", mode == "Mercenaries" ? mercMap : location.Room },
                { "location", mode == "Mercenaries" ? mercMap : location.FullLocation },
                { "locationsource", mode == "Mercenaries" ? "Mercenaries stage" : location.Source },
                { "locationconfidence", mode == "Mercenaries" ? "Known map" : location.Confidence },
                { "objective", objective },
                { "objectiveid", snapshot.ObjectiveId },
                { "objectiveguid", snapshot.ObjectiveGuid.ToString() },
                { "health", snapshot.Health },
                { "maxhealth", snapshot.MaxHealth },
                { "healthpercentage", percentage },
                { "healthstatus", snapshot.Health <= 0 ? "Dead" : percentage > 75 ? "Fine" : percentage > 25 ? "Caution" : "Danger" },
                { "map", mode == "Mercenaries" ? mercMap : location.FullLocation },
                { "score", snapshot.Score }
            };
        }

        private static void ShowStatus(string text)
        {
            if (discord == null)
            {
                throw new InvalidOperationException("RE4R: presence is not initialized.");
            }

            discord.SetPresence(new RichPresence()
            {
                Details = text,
                State = "",
                Assets = new Assets()
                {
                    LargeImageKey = "logo",
                    LargeImageText = "Resident Evil 4"
                },
                Timestamps = PlaceholderHelper._startTimestamp
            });
        }

        private sealed record Snapshot(bool IsInGame, int CampaignKey, int ChapterId, int DifficultyId, int CharacterId, int StageId, int SectionId, int SectionGroup, int MapSceneId, int Health, int MaxHealth, int ObjectiveId, Guid ObjectiveGuid, int Score, int MercenaryCharacter = -1, int MercenaryCostume = -1, int MercenaryStage = -1);
        private static Snapshot EmptySnapshot()
        {
            return new Snapshot(false, 0, 0, 0, -1, -1, -1, -1, 0, 0, 0, 0, Guid.Empty, 0);
        }

        private static Snapshot ReadSnapshot()
        {
            ResolveAddresses();
            var flow = Pointer(flowSlot);
            if (!IsInGame(flow))
            {
                return EmptySnapshot();
            }

            var campaign = Pointer(campaignSlot);
            var player = Follow(characterSlot, 0x90, 0x40);
            if (campaign == 0 || player == 0)
            {
                return EmptySnapshot();
            }

            var campaignKey = ReadInt(campaign + 0x20);
            if (GetMode(campaignKey) == "Unknown mode")
            {
                throw new InvalidDataException("RE4: unknown campaign key.");
            }

            var hp = Pointer(player + 0x148);
            if (hp == 0)
            {
                return EmptySnapshot();
            }

            var health = ReadInt(hp + 0x14);
            var maxHealth = ReadInt(hp + 0x10);
            if (maxHealth <= 0 || maxHealth > 10000000 || health < 0 || health > maxHealth)
            {
                throw new InvalidDataException("RE4: invalid hit-point controller.");
            }

            var stageId = ReadInt(player + 0x1A8);
            var sections = Follow(levelMapSlot, 0x20);
            var section = sections != 0 && ReadInt(sections + 0x1C)is> 0 and <= 4 ? Pointer(sections + 0x20) : 0;
            var map = Pointer(mapSlot);
            var purpose = Follow(purposeSlot, 0x10);
            var objectiveId = purpose == 0 ? 0 : ReadInt(purpose + 0x10);
            var objectiveGuid = purpose == 0 ? Guid.Empty : Hypervisor.Read<Guid>(purpose + 0x18, true);
            var score = 0;
            var mercenaryCharacter = -1;
            var mercenaryCostume = -1;
            var mercenaryStage = -1;
            if (GetMode(campaignKey) == "Mercenaries")
            {
                var callback = Follow(mercenariesSlot, 0x20);
                if (callback == 0 || ReadInt(callback + 0x10) != 1 || Pointer(callback + 0x28) != Pointer(mercenaryCallbackSlot))
                {
                    throw new InvalidDataException("RE4: Mercenaries controller is not ready.");
                }

                var controller = Pointer(callback + 0x18);
                if (controller == 0)
                {
                    return EmptySnapshot();
                }

                mercenaryCharacter = ReadInt(controller + 0x114);
                mercenaryCostume = ReadInt(controller + 0x118);
                mercenaryStage = ReadInt(controller + 0xA8);
                var counter = Pointer(controller + 0xF8);
                if (counter == 0)
                {
                    return EmptySnapshot();
                }

                var wrapper = Pointer(counter + 0x18);
                if (wrapper == 0)
                {
                    return EmptySnapshot();
                }

                var divisor = unchecked((int)(uint)wrapper);
                var encoded = Hypervisor.Read<long>(wrapper + 0x10, true);
                if (divisor == 0 || (encoded == long.MinValue && divisor == -1))
                {
                    throw new InvalidDataException("RE4: invalid score wrapper.");
                }

                score = checked((int)(encoded / divisor));
                if (score < 0 || callback != Follow(mercenariesSlot, 0x20) || controller != Pointer(callback + 0x18) || counter != Pointer(controller + 0xF8))
                {
                    throw new InvalidDataException("RE4: Mercenaries controller changed during the read.");
                }
            }

            var snapshot = new Snapshot(true, campaignKey, ReadInt(campaign + 0x30), ReadInt(campaign + 0x28), ReadInt(player + 0x38), stageId, section == 0 ? -1 : ReadInt(section + 0x10), section == 0 ? -1 : ReadInt(section + 0x14), map == 0 ? 0 : ReadInt(map + 0x78), health, maxHealth, objectiveId, objectiveGuid, score, mercenaryCharacter, mercenaryCostume, mercenaryStage);
            if (flow != Pointer(flowSlot) || !IsInGame(flow) || campaign != Pointer(campaignSlot) || player != Follow(characterSlot, 0x90, 0x40) || hp != Pointer(player + 0x148) || snapshot.CampaignKey != ReadInt(campaign + 0x20) || snapshot.ChapterId != ReadInt(campaign + 0x30) || stageId != ReadInt(player + 0x1A8) || sections != Follow(levelMapSlot, 0x20) || (section != 0 && (section != Pointer(sections + 0x20) || snapshot.SectionId != ReadInt(section + 0x10))) || purpose != Follow(purposeSlot, 0x10) || (purpose != 0 && (objectiveId != ReadInt(purpose + 0x10) || objectiveGuid != Hypervisor.Read<Guid>(purpose + 0x18, true))))
            {
                throw new InvalidDataException("RE4: scene changed during the read; retry next poll.");
            }

            return snapshot;
        }

        private static bool IsInGame(ulong manager)
        {
            if (manager == 0)
            {
                return false;
            }

            var control = Pointer(manager + 0x28);
            if (control == 0)
            {
                return false;
            }

            var state = Pointer(control + 0x10);
            if (state == 0)
            {
                return false;
            }

            var vtable = Pointer(state);
            if (vtable < 0x10)
            {
                return false;
            }

            var dispatch = Pointer(vtable - 0x10);
            if (dispatch == 0)
            {
                return false;
            }

            var getter = Pointer(dispatch + 0x78);
            if (getter < cachedModuleBase || getter + 3 > cachedModuleBase + (ulong)cachedModuleSize)
            {
                throw new InvalidDataException("RE4: invalid game-state getter.");
            }

            var bytes = Hypervisor.ReadArray(getter, 3, true);
            if (bytes[0] == 0xB0 && bytes[1] == 1 && bytes[2] == 0xC3)
            {
                return true;
            }

            if (bytes[0] == 0x32 && bytes[1] == 0xC0 && bytes[2] == 0xC3)
            {
                return false;
            }

            throw new InvalidDataException("RE4: unrecognized game-state getter.");
        }

        private static string GetMode(int key)
        {
            return key switch
            {
                >= 1 and <= 21 => "Main Story",
                >= 22 and <= 33 => "Separate Ways",
                >= 34 and <= 65 => "Mercenaries",
                _ => "Unknown mode",
            };
        }

        private static string GetCharacter(int id, int campaignKey, int chapterId)
        {
            if (id == 100000)
            {
                return "Leon";
            }

            if (id == 110000)
            {
                return "Ashley";
            }

            if (id == 380000)
            {
                return "Ada";
            }

            if (GetMode(campaignKey) == "Mercenaries")
            {
                return (chapterId % 1000) switch
                {
                    100 => "Leon",
                    200 => "Luis",
                    300 => "Krauser",
                    400 => "HUNK",
                    500 => "Ada",
                    600 => "Wesker",
                    _ => $"Character {id}",
                };
            }

            return $"Character {id}";
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
            cachedProcessId = cachedModuleSize = 0;
            cachedModuleBase = characterSlot = campaignSlot = flowSlot = mapSlot = levelMapSlot = purposeSlot = mercenariesSlot = mercenaryCallbackSlot = 0;
            nextScan = DateTime.MinValue;
        }

        private static void ResolveAddresses()
        {
            var process = Hypervisor.Process ?? throw new InvalidDataException("RE4R: game is not attached.");
            var module = process.MainModule ?? throw new InvalidDataException("RE4: module not loaded.");
            var moduleBase = (ulong)module.BaseAddress.ToInt64();
            if (cachedProcessId != process.Id || cachedModuleBase != moduleBase)
            {
                ResetAddresses();
                cachedProcessId = process.Id;
                cachedModuleBase = moduleBase;
                cachedModuleSize = module.ModuleMemorySize;
            }

            if (flowSlot != 0)
            {
                return;
            }

            if (DateTime.UtcNow < nextScan)
            {
                throw new InvalidDataException("RE4: waiting to retry patterns.");
            }

            nextScan = DateTime.UtcNow.AddSeconds(30);
            ulong Resolve(string pattern, int operand = 3, int length = 7)
            {
                var match = FindUniquePattern(moduleBase, module.ModuleMemorySize, pattern);
                var target = checked((ulong)((long)match + length + ReadInt(match + (ulong)operand)));
                if (target < moduleBase || target + 8 > moduleBase + (ulong)module.ModuleMemorySize)
                {
                    throw new InvalidDataException("RE4: RIP-relative target is outside the module.");
                }

                return target;
            }

            var character = Resolve(CharacterPattern);
            var campaign = Resolve(CampaignPattern);
            var flow = Resolve(FlowPattern);
            var map = Resolve(MapPattern);
            var levelMap = Resolve(LevelMapPattern);
            var purpose = Resolve(PurposePattern);
            var mercenaries = Resolve(MercenariesPattern, 23, 27);
            var mercenaryCallback = Resolve(MercenariesPattern);
            FindUniquePattern(moduleBase, module.ModuleMemorySize, FlowLayoutPattern);
            FindUniquePattern(moduleBase, module.ModuleMemorySize, StageLayoutPattern);
            FindUniquePattern(moduleBase, module.ModuleMemorySize, ScoreLayoutPattern);
            characterSlot = character;
            campaignSlot = campaign;
            mapSlot = map;
            levelMapSlot = levelMap;
            purposeSlot = purpose;
            mercenariesSlot = mercenaries;
            mercenaryCallbackSlot = mercenaryCallback;
            flowSlot = flow;
        }

        private static ulong FindUniquePattern(ulong moduleBase, int moduleSize, string signature)
        {
            if (moduleSize < 64)
            {
                throw new InvalidDataException("RE4: invalid module size.");
            }

            var peOffset = Hypervisor.Read<uint>(moduleBase + 0x3C, true);
            if (peOffset > (uint)moduleSize - 24)
            {
                throw new InvalidDataException("RE4: invalid PE offset.");
            }

            var pe = moduleBase + peOffset;
            if (peOffset > (uint)moduleSize - 24 || Hypervisor.Read<uint>(pe, true) != 0x4550 || Hypervisor.Read<ushort>(pe + 4, true) != 0x8664)
            {
                throw new InvalidDataException("RE4: invalid PE header.");
            }

            var sectionCount = Hypervisor.Read<ushort>(pe + 6, true);
            var optionalSize = Hypervisor.Read<ushort>(pe + 20, true);
            var sectionTable = pe + 24 + optionalSize;
            if (sectionTable + (ulong)sectionCount * 40 > moduleBase + (ulong)moduleSize)
            {
                throw new InvalidDataException("RE4: invalid section table.");
            }

            var pattern = SignaturePattern.Parse(signature);
            var prefix = pattern.Bytes.TakeWhile(value => value.HasValue).Select(value => value.GetValueOrDefault()).ToArray();
            if (prefix.Length == 0)
            {
                throw new InvalidDataException("RE4: pattern needs a fixed prefix.");
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
                    throw new InvalidDataException("RE4: invalid executable section.");
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
                            throw new InvalidDataException("RE4: manager pattern is ambiguous.");
                        }

                        match = address;
                    }
                }
            }

            return match != 0 ? match : throw new InvalidDataException("RE4: manager pattern was not found.");
        }

        private static void InitializeDiscord()
        {
            if (discord == null)
            {
                throw new InvalidOperationException("RE4R: presence is not initialized.");
            }

            discord.Initialize();
        }
    }
}
