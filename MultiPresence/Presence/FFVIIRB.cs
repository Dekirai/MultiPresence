using MultiPresence.Runtime;
using DiscordRPC;
using MultiPresence.Models.FFVIIRB;
using MultiPresence.Infrastructure;
using System.Globalization;
using System.Text;

namespace MultiPresence.Presence
{
    public class FFVIIRB
    {
        private static DiscordRpcClient? discord;
        private static DiscordStatusUpdater? updater;
        private const string namesAddressPattern = "48 8D 05 ?? ?? ?? ?? EB ?? 48 8D 0D ?? ?? ?? ?? E8 ?? ?? ?? ?? C6 05 ?? ?? ?? ?? 01 8B 80 40 09 01 00";
        private const string modeSlotPattern = "48 8B 1D ?? ?? ?? ?? 48 85 DB 74 ?? 48 8B 9B 88 02 00 00 48 85 DB 74 ??";
        private const string NameEntryLayoutPattern = "44 8B C1 0F B7 C1 41 C1 E8 10 44 89 44 24 30 89 44 24 34 48 8B 4C 24 30 4E 8B 5C C2 10 48 C1 E9 20 C1 E1 02 4C 03 D9 66 41 39 5B 04 0F 85 ?? ?? ?? ?? 45 0F B7 43 04 49 8D 4B 06 49 8B C1 49 C1 E8 06";
        private const string gameDataSlotPattern = "48 8B 0D ?? ?? ?? ?? 33 C0 48 85 C9 74 ?? 48 8B 81 90 04 00 00 C3 CC CC";
        private const string saveDataPattern = "4C 8D 15 ?? ?? ?? ?? 4C 0F 45 D7 4D 85 D2 74 ?? 45 8A 92 99 3D 00 00 EB ?? 41 B2 04 E8 ?? ?? ?? ?? 84 C0 74 ??";
        private const string groupOverrideFlagIndexPattern = "8B 15 ?? ?? ?? ?? 8B CA C1 E9 05 8B C2 F7 D8 45 1B C0 44 23 C1 41 81 F8 00 02 00 00 73 ?? 43 8B 8C 81 AC 48 04 00 83 E2 1F 0F A3 D1 0F 92 C0";
        private const string actorStatePattern = "48 8B 89 38 06 00 00 48 85 C9 74 ?? 48 8B 01 FF 50 40 48 85 C0 74 ?? 48 8B 08 48 8B 51 68 48 8D 0D ?? ?? ?? ?? 48 3B D1 75 ?? 8A 80 E8 03 00 00";
        private const string componentStatePattern = "48 8B 91 A8 01 00 00 48 8B C2 48 F7 D8 48 1B C0 48 23 C2 C3";
        private const string characterIdGetterPattern = "0F B6 81 E8 03 00 00 C3";
        private const string recordIndexPattern = "80 FA 0F 73 ?? 41 80 F8 04 73 ?? 41 0F B6 C0 0F B6 D2 48 8D 14 90 48 69 C2 E8 22 00 00 48 05 88 00 00 00 48 03 C1 C3";
        private const string recordSelectionPattern = "8A DA 4C 8B D9 80 FA 0F 72 ?? 33 C0 EB ?? 33 FF 48 39 79 58 75 ?? 41 B2 04 EB ?? 48 39 3D ?? ?? ?? ?? 74 ?? 83 3D ?? ?? ?? ?? 0E 4C 8D 15 ?? ?? ?? ?? 4C 0F 45 D7 4D 85 D2 74 ?? 45 8A 92 99 3D 00 00 EB ?? 41 B2 04 E8 ?? ?? ?? ?? 84 C0 74 ?? 41 80 FA 01 41 0F B6 C2 0F 44 C7 44 8A D0 45 8A C2 8A D3 49 8B CB";
        private const string recordLevelPattern = "48 85 C0 74 ?? 48 8B C8 E8 ?? ?? ?? ?? 48 85 C0 74 ?? 0F B6 80 83 00 00 00 48 83 C4 28 C3";
        private const string recordExperiencePattern = "48 85 C0 74 ?? 8B 80 88 00 00 00 48 83 C4 28 C3 EB ?? B8 38 00 00 00";
        private const string recordHPPattern = "48 85 C0 74 ?? 44 8B 98 A8 03 00 00 48 8B 5C 24 30 41 8B C3 48 83 C4 20 5F C3 CC CC 48 89 5C 24 08";
        private const string recordMPPattern = "48 85 C0 74 ?? 8B 80 AC 03 00 00 48 83 C4 28 C3 EB ?? 48 89 5C 24 08";
        private const string recordBaseMaxHPPattern = "48 85 C0 74 ?? 44 8B 98 94 00 00 00 48 8B 5C 24 30 41 8B C3 48 8B 74 24 38 48 83 C4 20 5F C3 CC";
        private const string recordBaseMaxMPPattern = "48 85 C0 74 ?? 8B 80 98 00 00 00 48 83 C4 28 C3 EB ?? B8 38 00 00 00";
        private const string recordWeaponLevelPattern = "48 85 C0 0F 84 ?? ?? ?? ?? 8B 80 9C 20 00 00 89 44 24 20 8B 54 24 34 44 8B C0 48 8B CB";
        private const string recordWeaponPointsPattern = "8B 88 A4 20 00 00 48 8D 56 10 03 88 A0 20 00 00 45 33 C0 89 4F 0C 48 8D 4D B0";
        private const string maxEffectsPointerPattern = "48 8B 8B F8 02 00 00 B2 4E 8B F8 E8 ?? ?? ?? ?? 48 85 C0 0F 85 ?? ?? ?? ?? 48 8B 5C 24 60 8B C7";
        private const string maxEffectListLayoutPattern = "48 8B 59 38 40 8A FA 48 85 DB 75 ?? 33 C0 48 8B 5C 24 30 48 83 C4 20 5F C3 48 8B 0B 48 85 C9 74 ?? 48 8B 01 FF 50 08 40 3A C7 74 ?? 48 8B 5B 10 EB ?? 48 8B 03";
        private const string maxEffectDataLayoutPattern = "48 8B 41 18 33 D2 48 89 44 24 28 48 8B 41 20 48 89 44 24 30 8A 41 28 48 8D 4C 24 20 88 44 24 38 48 8D 05 ?? ?? ?? ??";
        private const string maxEffectFactorLayoutPattern = "48 8B 49 08 83 79 18 00 7F ?? C5 F8 57 C0 C3 48 83 C1 10 48 8B 01 A8 01 74 ?? 48 D1 F8 48 03 C1 C5 FA 10 00 C3";
        private const string maxMultiplierTypeGettersPattern = "B0 4D C3 CC CC CC CC CC CC CC CC CC CC CC CC CC B0 4E C3";
        private const string maxRoundBiasPattern = "8A 41 28 48 8D 4C 24 20 88 44 24 38 48 8D 05 ?? ?? ?? ?? C5 C8 57 F6 48 89 44 24 20 C4 E1 CA 2A F7 E8 ?? ?? ?? ?? C5 CA 58 CE C4 E2 79 A9 0D ?? ?? ?? ?? BF 01 00 00 00 C5 FA 2D C1 D1 F8 3B C7 0F 43 F8";
        private const string locationSlotPattern = "48 8B 05 ?? ?? ?? ?? 48 8B 80 C8 00 00 00 48 85 C0 74 ?? 48 8B 40 08";
        private const string chapterSlotPattern = "48 8B 0D ?? ?? ?? ?? 48 8B 89 60 02 00 00 E8 ?? ?? ?? ?? 84 C0 0F 84 ?? ?? ?? ?? E9 ?? ?? ?? ?? 0B C2";
        private const string locationAssetMapPattern = "4C 8B 05 ?? ?? ?? ?? 83 F8 FF 74 ?? 48 63 C8 4C 8D 0C 49 4B 39 14 C8 74 ?? 43 8B 44 C8 10 EB ?? 8B C7 8B CF 89 45 50 89 4D 54 EB ??";
        private const string mapAssetRegistryPattern = "4C 8B 05 ?? ?? ?? ?? 48 0F 45 D0 49 2B CD 49 23 CC 8B 04 8A 83 F8 FF 0F 84 ?? ?? ?? ?? 48 63 C8 48 8D 14 49 48 C1 E2 05 46 38 24 02 74 ?? 42 8B 44 02 58 EB ?? 48 63 C8 4C 8D 14 49 49 C1 E2 05 4D 03 D0 41 8B 42 10 41 3B 42 3C 74 ?? 4D 8B 4A 48 44 8B C3 41 C1 E8 10 48 8B C3 48 C1 E8 20 0F B7 D3 42 8D 0C C2 C1 E1 10 03 CA C1 EA 04 03 C8 49 63 42 50 03 D1 49 2B C5 41 03 D0 48 63 CA 48 23 C8 49 8D 42 40 4D 85 C9 4C 0F 44 C8 41 8B 04 89 83 F8 FF 74 ?? 48 63 C8 48 8D 14 49 49 8B 4A 08 48 39 1C D1 0F 84 ?? ?? ?? ?? 8B 44 D1 10 EB ??";
        private const string naviMapCategorySlotPattern = "48 8B 40 08 48 89 45 50 8B 4D 54 8B 45 50 3B 05 ?? ?? ?? ?? 74 ?? 48 8B 55 50 45 33 C0 48 8B 0D ?? ?? ?? ?? E8 ?? ?? ?? ?? 48 8B D8 48 85 C0 74 ?? E8 ?? ?? ?? ?? 48 8B 53 10 4C 8D 40 30 49 63 40 08";
        private const string naviAssetMapLayoutPattern = "4D 8B 12 41 8B 82 B8 00 00 00 41 3B 82 E4 00 00 00 74 ?? 4D 8B 8A F0 00 00 00 44 8B C7 0F B7 C7 8B D0 41 C1 E8 10 C1 EA 04 03 D0 42 8D 0C C0 48 8B C7 48 C1 E8 20 C1 E1 10 03 D1 03 C2 41 03 C0 48 63 C8 49 63 82 F8 00 00 00 49 2B C5 48 23 C8 49 8D 82 E8 00 00 00 4D 85 C9 4C 0F 44 C8 41 8B 04 89 83 F8 FF 74 ?? 48 63 C8 48 8D 14 49 49 8B 8A B0 00 00 00 48 39 3C D1 0F 84 ?? ?? ?? ?? 8B 44 D1 10 EB ??";
        private const string sceneManagerSlotPattern = "48 8B 05 ?? ?? ?? ?? 48 8D 54 24 70 C4 C1 7A 10 8D B8 00 00 00 C5 FA 5C A0 34 03 00 00 C4 C1 7A 10 85 B0 00 00 00 C5 FA 5C 90 30 03 00 00 C5 F2 5C 98 38 03 00 00";
        private const string ComponentLocationLayoutPattern = "40 55 48 8B EC B8 30 00 00 00 E8 ?? ?? ?? ?? 48 2B E0 33 C0 48 39 42 20 0F 95 C0 48 01 42 20 48 8B 81 38 01 00 00 48 85 C0 74 ?? C5 F8 10 88 D0 01 00 00 C5 FA 11 4D DC C4 E3 79 17 4D E0 01 48 8B 45 DC C4 E3 79 17 4D E4 02 C5 FA 10 45 E4";
        private static int cachedProcessId, cachedModuleSize;
        private static ulong cachedModuleBase, namesAddress, modeSlot;
        private static ulong gameDataSlot, saveData, groupOverrideFlagIndex, componentState, characterIdGetter, maxMultiplierTypeGetters, maxRoundBias, locationSlot, chapterSlot, locationAssetMap, mapAssetRegistry, naviMapCategorySlot, sceneManagerSlot;
        private static DateTime nextScan;
        public static void DoAction()
        {
            ResetAddresses();
            GetPID();
            discord = new DiscordRpcClient("1332349500572045312");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/config/Final Fantasy VII Rebirth.json");
            PresenceRuntime.Start(nameof(FFVIIRB), "ff7rebirth_", RPC);
        }

        private static void GetPID()
        {
            ProcessMonitor.TryAttach("ff7rebirth_");
        }

        private static async Task RPC()
        {
            if (discord == null || updater == null)
            {
                throw new InvalidOperationException("FFVIIRB: presence is not initialized.");
            }

            if (ProcessMonitor.IsRunning("ff7rebirth_"))
            {
                try
                {
                    if (!Hypervisor.IsAttached && !ProcessMonitor.TryAttach("ff7rebirth_"))
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
                    PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Final Fantasy VII Rebirth", placeholders);
                }
                catch (Exception exception) when (exception is not OutOfMemoryException and not StackOverflowException)
                {
                    AppLog.Warning("Final Fantasy VII Rebirth memory is not ready.", exception);
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

            var character = Characters.GetName(snapshot.CharacterKey);
            if (character.Length == 0)
            {
                character = Characters.GetName(snapshot.CharacterId);
            }

            if (character.Length == 0)
            {
                character = "Character " + snapshot.CharacterId.ToString(CultureInfo.InvariantCulture);
            }

            var chapter = ChapterNumber(snapshot.ChapterKey);
            var chapterName = Chapters.GetName(chapter);
            var sameMap = string.Equals(snapshot.LocationId, snapshot.MapId, StringComparison.OrdinalIgnoreCase);
            var area = sameMap ? Locations.GetArea(snapshot.MapId, snapshot.Layer, snapshot.Group) : "";
            var location = Locations.GetLocation(snapshot.LocationId, sameMap ? snapshot.Layer : -1, sameMap ? snapshot.Group : -1);
            if (location.Length == 0)
            {
                location = string.IsNullOrWhiteSpace(snapshot.LocationId) || snapshot.LocationId == "None" ? "Unknown" : snapshot.LocationId;
            }

            object exp = "Unknown", maxExp = "Unknown", nextExp = "Unknown", maxSp = "Unknown";
            if (Progression.TryGetExperienceBase(snapshot.CharacterKey, snapshot.Level, out var currentBase) && snapshot.TotalExp >= currentBase)
            {
                exp = snapshot.TotalExp - currentBase;
                if (Progression.TryGetExperienceBase(snapshot.CharacterKey, snapshot.Level + 1, out var nextBase) && nextBase >= currentBase)
                {
                    maxExp = nextBase - currentBase;
                    nextExp = Math.Max(0, nextBase - snapshot.TotalExp);
                }
            }

            if (Progression.TryGetWeaponThreshold(snapshot.CharacterKey, snapshot.WeaponLevel, out var threshold))
            {
                maxSp = threshold;
            }

            return Task.FromResult(new Dictionary<string, object> { { "state", "In Game" }, { "character", character }, { "characterid", snapshot.CharacterId }, { "characterkey", snapshot.CharacterKey }, { "level", snapshot.Level }, { "hp", snapshot.Hp }, { "maxhp", snapshot.MaxHp }, { "mp", snapshot.Mp }, { "maxmp", snapshot.MaxMp }, { "sp", snapshot.Sp }, { "maxsp", maxSp }, { "wp", snapshot.Sp }, { "maxwp", maxSp }, { "weaponlevel", snapshot.WeaponLevel }, { "exp", exp }, { "maxexp", maxExp }, { "ep", exp }, { "maxep", maxExp }, { "totalexp", snapshot.TotalExp }, { "nextexp", nextExp }, { "chapter", chapter == 0 ? "Unknown" : chapter }, { "chaptername", chapterName }, { "chapterid", snapshot.ChapterKey }, { "location", location }, { "region", Locations.GetRegion(snapshot.LocationId) }, { "map", Locations.GetName(snapshot.LocationId) }, { "area", area }, { "locationid", snapshot.LocationId }, { "mapid", snapshot.MapId }, { "layerid", snapshot.Layer }, { "groupid", snapshot.Group } });
        }

        private static void ShowStatus(string text)
        {
            if (discord == null)
            {
                throw new InvalidOperationException("FFVIIRB: presence is not initialized.");
            }

            discord.SetPresence(new RichPresence()
            {
                Details = text,
                State = "",
                Assets = new Assets()
                {
                    LargeImageKey = "logo",
                    LargeImageText = "Final Fantasy VII Rebirth"
                },
                Timestamps = PlaceholderHelper._startTimestamp
            });
        }

        private sealed record Snapshot
        {
            public bool IsInGame { get; init; }
            public int CharacterId { get; init; }
            public string CharacterKey { get; init; } = "";
            public int Level { get; init; }
            public int Hp { get; init; }
            public int MaxHp { get; init; }
            public int Mp { get; init; }
            public int MaxMp { get; init; }
            public int Sp { get; init; }
            public int WeaponLevel { get; init; }
            public int TotalExp { get; init; }
            public string ChapterKey { get; init; } = "";
            public string LocationId { get; init; } = "";
            public string MapId { get; init; } = "";
            public int Layer { get; init; }
            public int Group { get; init; }
        }

        private static Snapshot ReadSnapshot()
        {
            ResolveAddresses();
            var mode = Pointer(modeSlot);
            if (!IsA(mode, "EndGameMode"))
            {
                return new Snapshot();
            }

            var stateAddress = Field(mode, "GameState", 8);
            var state = Pointer(stateAddress);
            if (!IsA(state, "EndGameState"))
            {
                return new Snapshot();
            }

            var gameStateAddress = Field(state, "GameState", 1);
            var gameState = ReadByte(gameStateAddress);
            if (gameState is not (6 or 7 or 8 or 10))
            {
                return new Snapshot();
            }

            var levelObject = Pointer(mode + 0x20);
            var world = levelObject == 0 ? 0 : Pointer(levelObject + 0x20);
            if (!IsA(world, "World"))
            {
                return new Snapshot();
            }

            var instanceAddress = Field(world, "OwningGameInstance", 8);
            var instance = Pointer(instanceAddress);
            if (!IsA(instance, "EndGameInstance"))
            {
                return new Snapshot();
            }

            var playersAddress = Field(instance, "LocalPlayers", 16);
            var players = Pointer(playersAddress);
            var count = ReadInt(playersAddress + 8);
            var capacity = ReadInt(playersAddress + 12);
            if (count < 0 || count > capacity || capacity > 8)
            {
                throw new InvalidDataException("FFVIIRB: invalid local players.");
            }

            if (players == 0 || count == 0)
            {
                return new Snapshot();
            }

            var player = Pointer(players);
            if (!IsA(player, "LocalPlayer"))
            {
                return new Snapshot();
            }

            var controllerAddress = Field(player, "PlayerController", 8);
            var controller = Pointer(controllerAddress);
            if (!IsA(controller, "EndPlayerController"))
            {
                return new Snapshot();
            }

            var pawnAddress = Field(controller, "Pawn", 8);
            var acknowledgedAddress = Field(controller, "AcknowledgedPawn", 8);
            var pawn = Pointer(pawnAddress);
            if (pawn == 0 || pawn != Pointer(acknowledgedAddress) || !IsA(pawn, "EndPlayerCharacter"))
            {
                return new Snapshot();
            }

            var component = Pointer(pawn + 0x638);
            var stats = component == 0 ? 0 : Pointer(component + 0x1A8);
            if (stats == 0)
            {
                return new Snapshot();
            }

            if (Pointer(Pointer(component) + 0x30) != componentState || Pointer(Pointer(stats) + 0x68) != characterIdGetter)
            {
                throw new InvalidDataException("FFVIIRB: unsupported current character state type.");
            }

            var characterId = ReadByte(stats + 0x3E8);
            if (characterId >= 15)
            {
                throw new InvalidDataException("FFVIIRB: invalid player ID.");
            }

            var characterKey = ReadName(Pointer(Pointer(pawn + 0x10) + 0x18));
            var data = Pointer(gameDataSlot);
            var table = data == 0 ? 0 : Pointer(data + 0x490);
            if (table == 0 || Pointer(table + 0x58) == 0)
            {
                return new Snapshot();
            }

            var group = ReadStatGroup();
            var record = table + 0x88 + (ulong)(characterId * 4 + group) * 0x22E8;
            var level = ReadByte(record + 0x83);
            var hp = ReadInt(record + 0x3A8);
            var mp = ReadInt(record + 0x3AC);
            var maxHp = ReadMaximum(stats, record, false);
            var maxMp = ReadMaximum(stats, record, true);
            var exp = ReadInt(record + 0x88);
            var weaponLevel = ReadInt(record + 0x209C);
            var sp = checked(ReadInt(record + 0x20A0) + ReadInt(record + 0x20A4));
            if (level is < 1 or > 99 || hp is < 0 or > 10000000 || mp is < 0 or > 1000000 || maxHp is < 1 or > 10000000 || maxMp is < 0 or > 1000000 || exp < 0 || weaponLevel is < 0 or > 99 || sp is < 0 or > 1000000)
            {
                throw new InvalidDataException("FFVIIRB: invalid current character stats.");
            }

            var chapterOwner = Pointer(chapterSlot);
            var chapterManager = chapterOwner == 0 ? 0 : Pointer(chapterOwner + 0x260);
            if (chapterManager == 0)
            {
                return new Snapshot();
            }

            var chapterId = Pointer(chapterManager + 0xC);
            if (ReadByte(chapterManager + 8) != 0 || chapterId != Pointer(chapterManager + 0x2C))
            {
                return new Snapshot();
            }

            var chapterKey = ReadName(chapterId);
            var location = ReadLocation(pawn);
            if (mode != Pointer(modeSlot) || state != Pointer(stateAddress) || gameState != ReadByte(gameStateAddress) || levelObject != Pointer(mode + 0x20) || world != Pointer(levelObject + 0x20) || instance != Pointer(instanceAddress) || players != Pointer(playersAddress) || count != ReadInt(playersAddress + 8) || player != Pointer(players) || controller != Pointer(controllerAddress) || pawn != Pointer(pawnAddress) || pawn != Pointer(acknowledgedAddress) || component != Pointer(pawn + 0x638) || stats != Pointer(component + 0x1A8) || characterId != ReadByte(stats + 0x3E8) || data != Pointer(gameDataSlot) || table != Pointer(data + 0x490) || group != ReadStatGroup() || chapterOwner != Pointer(chapterSlot) || chapterManager != Pointer(chapterOwner + 0x260) || chapterId != Pointer(chapterManager + 0xC) || location != ReadLocation(pawn))
            {
                throw new InvalidDataException("FFVIIRB: scene changed during the read; retry next poll.");
            }

            return new Snapshot()

            {
                IsInGame = true,
                CharacterId = characterId,
                CharacterKey = characterKey,
                Level = level,
                Hp = hp,
                MaxHp = maxHp,
                Mp = mp,
                MaxMp = maxMp,
                Sp = sp,
                WeaponLevel = weaponLevel,
                TotalExp = exp,
                ChapterKey = chapterKey,
                LocationId = location.LocationId,
                MapId = location.MapId,
                Layer = location.Layer,
                Group = location.Group
            };
        }

        private static int ReadStatGroup()
        {
            if (ReadInt(saveData + 0x10) != 14)
            {
                throw new InvalidDataException("FFVIIRB: save data is not ready.");
            }

            var group = ReadByte(saveData + 0x3D99);
            if (group > 3)
            {
                throw new InvalidDataException("FFVIIRB: invalid active stat group.");
            }

            if (group != 1)
            {
                return group;
            }

            var flag = Hypervisor.Read<uint>(groupOverrideFlagIndex, true);
            if (flag >> 5 >= 0x200)
            {
                throw new InvalidDataException("FFVIIRB: invalid group override flag.");
            }

            return (Hypervisor.Read<uint>(saveData + 0x448AC + (flag >> 5) * 4, true) & (1U << (int)(flag & 31))) != 0 ? 0 : 1;
        }

        private sealed record LocationSample(ulong Manager, ulong Record, ulong PackedLocation, ulong Settings, string LocationId, string MapId, int Layer, int Group);
        private static LocationSample ReadLocation(ulong pawn)
        {
            var manager = Pointer(locationSlot);
            var record = manager == 0 ? 0 : Pointer(manager + 0xC8);
            if (record == 0)
            {
                return new LocationSample(manager, 0, 0, 0, "", "", -1, -1);
            }

            var location = Pointer(record + 8);
            var id = ReadName(location);
            var settings = FindNaviMapSettings(location);
            var mapId = settings == 0 ? id : ReadName(Pointer(Field(settings, "LocationId", 8)));
            var(layer, group) = ReadPreciseArea(settings, pawn);
            return new LocationSample(manager, record, location, settings, id, mapId, layer, group);
        }

        private static int ReadMaximum(ulong stats, ulong record, bool mp)
        {
            var baseAddress = record + (mp ? 0x98UL : 0x94UL);
            var maximum = ReadInt(baseAddress);
            var baseMaximum = maximum;
            if (maximum < (mp ? 0 : 1) || maximum > (mp ? 1000000 : 10000000))
            {
                throw new InvalidDataException("FFVIIRB: invalid base maximum.");
            }

            var owner = Pointer(stats + 0x2F8);
            if (owner == 0)
            {
                throw new InvalidDataException("FFVIIRB: missing status-effect collection.");
            }

            var first = Pointer(owner + 0x38);
            var node = first;
            var expectedGetter = maxMultiplierTypeGetters + (mp ? 0x10UL : 0);
            var visited = new HashSet<ulong>();
            var links = new List<(ulong Node, ulong Effect, ulong Next)>();
            ulong selected = 0, selectedType = 0, row = 0, encoded = 0, valueAddress = 0;
            int valueCount = 0;
            float factor = 0;
            while (node != 0)
            {
                if (links.Count >= 128 || !visited.Add(node))
                {
                    throw new InvalidDataException("FFVIIRB: invalid status-effect list.");
                }

                var effect = Pointer(node);
                var next = Pointer(node + 0x10);
                links.Add((node, effect, next));
                if (effect != 0)
                {
                    var type = Pointer(effect);
                    if (type == 0)
                    {
                        throw new InvalidDataException("FFVIIRB: missing status-effect type.");
                    }

                    if (Pointer(type + 8) == expectedGetter)
                    {
                        selected = effect;
                        selectedType = type;
                        row = Pointer(effect + 0x18);
                        if (row == 0)
                        {
                            throw new InvalidDataException("FFVIIRB: missing maximum modifier data.");
                        }

                        valueCount = ReadInt(row + 0x18);
                        if (valueCount > 0)
                        {
                            encoded = Pointer(row + 0x10);
                            valueAddress = (encoded & 1) == 0 ? encoded : checked((ulong)((long)(row + 0x10) + (unchecked((long)encoded) >> 1)));
                            if (valueAddress == 0)
                            {
                                throw new InvalidDataException("FFVIIRB: missing maximum modifier value.");
                            }

                            factor = Hypervisor.Read<float>(valueAddress, true);
                        }

                        if (!float.IsFinite(factor) || factor < 0 || factor > 1000)
                        {
                            throw new InvalidDataException("FFVIIRB: invalid maximum multiplier.");
                        }

                        var bias = Hypervisor.Read<float>(maxRoundBias, true);
                        if (bias != 0.5f)
                        {
                            throw new InvalidDataException("FFVIIRB: unsupported maximum rounding.");
                        }

                        var rounded = MathF.Round(MathF.FusedMultiplyAdd((float)maximum * 2f, factor, bias), MidpointRounding.ToEven);
                        if (!float.IsFinite(rounded) || rounded < 0 || rounded >= int.MaxValue)
                        {
                            throw new InvalidDataException("FFVIIRB: maximum multiplier overflow.");
                        }

                        maximum = Math.Max(1, ((int)rounded) >> 1);
                        break;
                    }
                }

                node = next;
            }

            if (baseMaximum != ReadInt(baseAddress) || owner != Pointer(stats + 0x2F8) || first != Pointer(owner + 0x38) || links.Any(link => Pointer(link.Node) != link.Effect || Pointer(link.Node + 0x10) != link.Next))
            {
                throw new InvalidDataException("FFVIIRB: status effects changed during the read.");
            }

            if (selected != 0 && (selectedType != Pointer(selected) || expectedGetter != Pointer(selectedType + 8) || row != Pointer(selected + 0x18) || valueCount != ReadInt(row + 0x18) || valueCount > 0 && (encoded != Pointer(row + 0x10) || factor != Hypervisor.Read<float>(valueAddress, true))))
            {
                throw new InvalidDataException("FFVIIRB: maximum modifier changed during the read.");
            }

            return maximum;
        }

        private static ulong FindNaviMapSettings(ulong packedLocation)
        {
            var observed = new List<(ulong Address, byte[] Bytes)>();
            byte[] Capture(ulong address, int length)
            {
                if (address == 0)
                {
                    throw new InvalidDataException("FFVIIRB: missing navigation registry address.");
                }

                var bytes = Hypervisor.ReadArray(address, length, true);
                observed.Add((address, bytes));
                return bytes;
            }

            ulong Finish(ulong result)
            {
                foreach (var sample in observed)
                {
                    if (!sample.Bytes.AsSpan().SequenceEqual(Hypervisor.ReadArray(sample.Address, sample.Bytes.Length, true)))
                    {
                        throw new InvalidDataException("FFVIIRB: navigation registry changed during the read.");
                    }
                }

                return result;
            }

            static uint NameHash(ulong name)
            {
                var low = (uint)name & 0xFFFF;
                var high = ((uint)name >> 16);
                return unchecked(((low + high * 8) << 16) + low + (low >> 4) + high + (uint)(name >> 32));
            }

            ulong FindEntry(ulong map, ulong key, bool byteKey = false)
            {
                var header = Capture(map, 0x50);
                var data = BitConverter.ToUInt64(header, 0);
                var slots = BitConverter.ToInt32(header, 8);
                var capacity = BitConverter.ToInt32(header, 0xC);
                var free = BitConverter.ToInt32(header, 0x34);
                if (slots < 0 || slots > capacity || capacity > 100000 || free < 0 || free > slots)
                {
                    throw new InvalidDataException("FFVIIRB: invalid navigation registry map.");
                }

                if (slots == free)
                {
                    return 0;
                }

                var bucketCount = BitConverter.ToInt32(header, 0x48);
                if (data == 0 || bucketCount < 1 || bucketCount > 131072 || (bucketCount & (bucketCount - 1)) != 0)
                {
                    throw new InvalidDataException("FFVIIRB: invalid navigation registry hash table.");
                }

                var hashData = BitConverter.ToUInt64(header, 0x40);
                if (hashData == 0)
                {
                    if (bucketCount != 1)
                    {
                        throw new InvalidDataException("FFVIIRB: invalid inline registry hash table.");
                    }

                    hashData = map + 0x38;
                }

                var hash = byteKey ? (uint)(byte)key : NameHash(key);
                var bucket = checked(hashData + (hash & (uint)(bucketCount - 1)) * 4UL);
                var index = BitConverter.ToInt32(Capture(bucket, 4), 0);
                var stride = byteKey ? 0x60 : 0x18;
                var visited = new HashSet<int>();
                while (index != -1)
                {
                    if (index < 0 || index >= slots || visited.Count >= 4096 || !visited.Add(index))
                    {
                        throw new InvalidDataException("FFVIIRB: invalid navigation registry hash chain.");
                    }

                    var entry = checked(data + (ulong)index * (ulong)stride);
                    var bytes = Capture(entry, stride);
                    var next = BitConverter.ToInt32(bytes, stride - 8);
                    if (next < -1 || next >= slots)
                    {
                        throw new InvalidDataException("FFVIIRB: invalid navigation registry next entry.");
                    }

                    if (byteKey ? bytes[0] == (byte)key : BitConverter.ToUInt64(bytes, 0) == key)
                    {
                        return entry;
                    }

                    index = next;
                }

                return 0;
            }

            var locationEntry = FindEntry(locationAssetMap, packedLocation);
            if (locationEntry == 0)
            {
                return Finish(0);
            }

            var assetName = BitConverter.ToUInt64(Capture(locationEntry + 8, 8), 0);
            var residentEntry = FindEntry(mapAssetRegistry, 0, true);
            if (residentEntry == 0)
            {
                return Finish(0);
            }

            var category = BitConverter.ToUInt64(Capture(naviMapCategorySlot, 8), 0);
            var categoryEntry = FindEntry(residentEntry + 8, category);
            if (categoryEntry == 0)
            {
                return Finish(0);
            }

            var categoryNode = BitConverter.ToUInt64(Capture(categoryEntry + 8, 8), 0);
            if (categoryNode == 0)
            {
                return Finish(0);
            }

            var assetEntry = FindEntry(categoryNode + 0xB0, assetName);
            if (assetEntry == 0)
            {
                return Finish(0);
            }

            var asset = BitConverter.ToUInt64(Capture(assetEntry + 8, 8), 0);
            if (asset == 0)
            {
                return Finish(0);
            }

            var objectHeader = Capture(asset, 0x28);
            if (BitConverter.ToUInt64(objectHeader, 0x18) != assetName || !IsA(asset, "EndNaviMapSettings"))
            {
                throw new InvalidDataException("FFVIIRB: navigation registry returned a different asset.");
            }

            return Finish(asset);
        }

        private static readonly HashSet<ulong> geometryTypes = new HashSet<ulong>();
        private readonly record struct Point(float X, float Y, float Z);
        private static Point ReadPoint(ulong address)
        {
            var bytes = Hypervisor.ReadArray(address, 12, true);
            var result = new Point(BitConverter.ToSingle(bytes, 0), BitConverter.ToSingle(bytes, 4), BitConverter.ToSingle(bytes, 8));
            if (!float.IsFinite(result.X) || !float.IsFinite(result.Y) || !float.IsFinite(result.Z))
            {
                throw new InvalidDataException("FFVIIRB: invalid map coordinates.");
            }

            return result;
        }

        private static ulong FindProperty(ulong type, string name)
        {
            for (var depth = 0; type != 0 && depth < 32; depth++, type = Pointer(type + 0x40))
            {
                var property = Pointer(type + 0x50);
                for (var n = 0; property != 0 && n < 2048; n++, property = Pointer(property + 0x20))
                {
                    if (ReadName(Pointer(property + 0x28)) == name)
                    {
                        return property;
                    }
                }
            }

            throw new InvalidDataException("FFVIIRB: map property missing: " + name);
        }

        private static void ValidateGeometryLayout(ulong settings)
        {
            var type = Pointer(settings + 0x10);
            if (geometryTypes.Contains(type))
            {
                return;
            }

            ulong Check(ulong owner, string name, int offset, int size)
            {
                var property = FindProperty(owner, name);
                if (ReadInt(property + 0x4C) != offset || ReadInt(property + 0x3C) != size)
                {
                    throw new InvalidDataException("FFVIIRB: unsupported map geometry layout: " + name);
                }

                return property;
            }

            var map = FindProperty(type, "NaviMapVolumeInfo");
            var value = Pointer(map + 0x80);
            if (value == 0 || ReadInt(value + 0x3C) != 80 || ReadInt(value + 0x4C) != 8)
            {
                throw new InvalidDataException("FFVIIRB: unsupported map volume stride.");
            }

            var volume = Pointer(value + 0x78);
            Check(volume, "LayerNumber", 0, 4);
            Check(volume, "OpenGroupNumber", 4, 4);
            Check(volume, "VolumeBoundsWithWorldOffset", 0x18, 28);
            var lods = Check(volume, "VolumeLods", 0x38, 16);
            var lodInner = Pointer(lods + 0x78);
            if (ReadInt(lodInner + 0x3C) != 16)
            {
                throw new InvalidDataException("FFVIIRB: unsupported map LOD stride.");
            }

            var lod = Pointer(lodInner + 0x78);
            var polys = Check(lod, "VolumePolysWithWorldOffset", 0, 16);
            var polyInner = Pointer(polys + 0x78);
            if (ReadInt(polyInner + 0x3C) != 68)
            {
                throw new InvalidDataException("FFVIIRB: unsupported triangle stride.");
            }

            var poly = Pointer(polyInner + 0x78);
            Check(poly, "Vert0", 0, 12);
            Check(poly, "Vert1", 12, 12);
            Check(poly, "Vert2", 24, 12);
            geometryTypes.Add(type);
        }

        private static bool ContainsTriangle(Point point, Point a, Point b, Point c)
        {
            static double Cross(Point a, Point b, Point p)
            {
                return ((double)b.X - a.X) * ((double)p.Y - a.Y) - ((double)b.Y - a.Y) * ((double)p.X - a.X);
            }

            if (Cross(a, b, c) == 0)
            {
                return false;
            }

            var ab = Cross(a, b, point);
            var bc = Cross(b, c, point);
            var ca = Cross(c, a, point);
            return ab >= 0 && bc >= 0 && ca >= 0 || ab <= 0 && bc <= 0 && ca <= 0;
        }

        private static (int Layer, int Group) ReadPreciseArea(ulong settings, ulong pawn)
        {
            if (settings == 0 || pawn == 0)
            {
                return (-1, -1);
            }

            ValidateGeometryLayout(settings);
            var rootAddress = Field(pawn, "RootComponent", 8);
            var root = Pointer(rootAddress);
            var scene = Pointer(sceneManagerSlot);
            if (scene == 0 || !IsA(root, "SceneComponent"))
            {
                return (-1, -1);
            }

            var local = ReadPoint(root + 0x1D0);
            var origin = ReadPoint(scene + 0x330);
            var point = new Point(local.X + origin.X, local.Y + origin.Y, local.Z + origin.Z);
            if (!float.IsFinite(point.X) || !float.IsFinite(point.Y) || !float.IsFinite(point.Z))
            {
                throw new InvalidDataException("FFVIIRB: map coordinate overflow.");
            }

            var map = Field(settings, "NaviMapVolumeInfo", 80);
            var entries = Pointer(map);
            var count = ReadInt(map + 8);
            var capacity = ReadInt(map + 12);
            var flagPointer = Pointer(map + 0x20);
            var flags = flagPointer == 0 ? map + 0x10 : flagPointer;
            var flagCount = ReadInt(map + 0x28);
            var flagCapacity = ReadInt(map + 0x2C);
            var freeCount = ReadInt(map + 0x34);
            if (count < 0 || count > capacity || capacity > 8192 || flagCount != count || flagCapacity < flagCount || flagCapacity > 16384 || freeCount < 0 || freeCount > count || count > 0 && entries == 0 || flagPointer == 0 && flagCount > 128)
            {
                throw new InvalidDataException("FFVIIRB: invalid map volume collection.");
            }

            var allocation = Hypervisor.ReadArray(flags, (flagCount + 31) / 32 * 4, true);
            var groups = new HashSet<(int, int)>();
            var allocated = 0;
            for (var index = 0; index < count; index++)
            {
                if ((allocation[index / 8] & (1 << (index % 8))) == 0)
                {
                    continue;
                }

                allocated++;
                var volume = entries + (ulong)index * 96 + 8;
                if (ReadByte(volume + 0x30) == 0)
                {
                    continue;
                }

                var min = ReadPoint(volume + 0x18);
                var max = ReadPoint(volume + 0x24);
                if (min.X > max.X || min.Y > max.Y || min.Z > max.Z)
                {
                    throw new InvalidDataException("FFVIIRB: inverted map volume bounds.");
                }

                if (point.X < min.X || point.X > max.X || point.Y < min.Y || point.Y > max.Y || point.Z < min.Z || point.Z > max.Z)
                {
                    continue;
                }

                var layer = ReadInt(volume);
                var group = ReadInt(volume + 4);
                if (layer < 0 || layer > 1000 || group < 0 || group > 100000)
                {
                    throw new InvalidDataException("FFVIIRB: invalid map area ID.");
                }

                var lods = Pointer(volume + 0x38);
                var lodCount = ReadInt(volume + 0x40);
                var lodCapacity = ReadInt(volume + 0x44);
                if (lodCount < 0 || lodCount > lodCapacity || lodCapacity > 16 || lodCount > 0 && lods == 0)
                {
                    throw new InvalidDataException("FFVIIRB: invalid map LOD array.");
                }

                if (lodCount == 0)
                {
                    continue;
                }

                var triangles = Pointer(lods);
                var triangleCount = ReadInt(lods + 8);
                var triangleCapacity = ReadInt(lods + 12);
                if (triangleCount < 0 || triangleCount > triangleCapacity || triangleCapacity > 65536 || triangleCount > 0 && triangles == 0)
                {
                    throw new InvalidDataException("FFVIIRB: invalid map triangle array.");
                }

                for (var triangle = 0; triangle < triangleCount; triangle++)
                {
                    var t = triangles + (ulong)triangle * 68;
                    if (ContainsTriangle(point, ReadPoint(t), ReadPoint(t + 12), ReadPoint(t + 24)))
                    {
                        groups.Add((layer, group));
                        break;
                    }
                }

                if (lods != Pointer(volume + 0x38) || lodCount != ReadInt(volume + 0x40) || triangles != Pointer(lods) || triangleCount != ReadInt(lods + 8))
                {
                    throw new InvalidDataException("FFVIIRB: map geometry changed during the read.");
                }
            }

            if (allocated != count - freeCount || scene != Pointer(sceneManagerSlot) || root != Pointer(rootAddress) || origin != ReadPoint(scene + 0x330) || entries != Pointer(map) || count != ReadInt(map + 8) || flagPointer != Pointer(map + 0x20) || freeCount != ReadInt(map + 0x34) || !allocation.AsSpan().SequenceEqual(Hypervisor.ReadArray(flags, allocation.Length, true)))
            {
                throw new InvalidDataException("FFVIIRB: active map changed during the read.");
            }

            return groups.Count == 1 ? groups.First() : (-1, -1);
        }

        private static int ChapterNumber(string value)
        {
            return value != null && value.StartsWith("Chapt_", StringComparison.Ordinal) && value.Length >= 10 && int.TryParse(value.AsSpan(6, 4), NumberStyles.None, CultureInfo.InvariantCulture, out var chapter) && chapter is >= 1 and <= 14 ? chapter : 0;
        }

        private static readonly Dictionary<(ulong Type, string Field), int> fieldOffsets = new Dictionary<(ulong Type, string Field), int>();
        private static int FieldOffset(ulong instance, string field, int expectedSize)
        {
            var type = Pointer(instance + 0x10);
            if (type == 0)
            {
                throw new InvalidDataException("FFVIIRB: missing object class.");
            }

            var key = (type, field);
            if (fieldOffsets.TryGetValue(key, out var cached))
            {
                return cached;
            }

            for (var depth = 0; type != 0 && depth < 32; depth++, type = Pointer(type + 0x40))
            {
                var property = Pointer(type + 0x50);
                for (var count = 0; property != 0 && count < 2048; count++, property = Pointer(property + 0x20))
                {
                    if (ReadName(Pointer(property + 0x28)) != field)
                    {
                        continue;
                    }

                    var offset = ReadInt(property + 0x4C);
                    if (offset < 0x28 || offset > 0x100000 || ReadInt(property + 0x3C) != expectedSize)
                    {
                        throw new InvalidDataException("FFVIIRB: unsupported property layout: " + field);
                    }

                    fieldOffsets[key] = offset;
                    return offset;
                }
            }

            throw new InvalidDataException("FFVIIRB: property not found: " + field);
        }

        private static ulong Field(ulong instance, string field, int size)
        {
            return instance + (ulong)FieldOffset(instance, field, size);
        }

        private static bool IsA(ulong instance, string expected)
        {
            if (instance == 0)
            {
                return false;
            }

            var type = Pointer(instance + 0x10);
            for (var depth = 0; type != 0 && depth < 32; depth++, type = Pointer(type + 0x40))
            {
                if (ReadName(Pointer(type + 0x18)) == expected)
                {
                    return true;
                }
            }

            return false;
        }

        private static string ReadName(ulong packedName)
        {
            var index = (uint)packedName;
            var block = index >> 16;
            var currentBlock = Hypervisor.Read<uint>(namesAddress + 8, true);
            var cursor = Hypervisor.Read<uint>(namesAddress + 12, true);
            if (currentBlock >= 8192 || block > currentBlock || cursor > 0x40000)
            {
                throw new InvalidDataException("FFVIIRB: invalid name pool.");
            }

            var start = Pointer(namesAddress + 0x10 + block * 8);
            var offset = (index & 0xFFFF) * 4;
            var limit = block == currentBlock ? cursor : 0x40000;
            if (start == 0 || offset + 6 > limit)
            {
                throw new InvalidDataException("FFVIIRB: invalid name entry.");
            }

            var header = Hypervisor.Read<ushort>(start + offset + 4, true);
            var length = header >> 6;
            var wide = (header & 1) != 0;
            var byteLength = length * (wide ? 2 : 1);
            if (length is < 1 or > 1023 || offset + 6 + byteLength > limit)
            {
                throw new InvalidDataException("FFVIIRB: invalid name length.");
            }

            var value = (wide ? Encoding.Unicode : Encoding.UTF8).GetString(Hypervisor.ReadArray(start + offset + 6, byteLength, true));
            if (value.Any(char.IsControl))
            {
                throw new InvalidDataException("FFVIIRB: invalid name characters.");
            }

            return value;
        }

        private static ulong Pointer(ulong address)
        {
            return Hypervisor.Read<ulong>(address, true);
        }

        private static int ReadInt(ulong address)
        {
            return Hypervisor.Read<int>(address, true);
        }

        private static byte ReadByte(ulong address)
        {
            return Hypervisor.Read<byte>(address, true);
        }

        private static void ResetAddresses()
        {
            cachedProcessId = cachedModuleSize = 0;
            cachedModuleBase = namesAddress = modeSlot = gameDataSlot = saveData = groupOverrideFlagIndex = componentState = characterIdGetter = maxMultiplierTypeGetters = maxRoundBias = locationSlot = chapterSlot = locationAssetMap = mapAssetRegistry = naviMapCategorySlot = sceneManagerSlot = 0;
            nextScan = DateTime.MinValue;
            fieldOffsets.Clear();
            geometryTypes.Clear();
        }

        private static void ResolveAddresses()
        {
            var process = Hypervisor.Process ?? throw new InvalidDataException("FFVIIRB: game is not attached.");
            var module = process.MainModule ?? throw new InvalidDataException("FFVIIRB: module not loaded.");
            var moduleBase = (ulong)module.BaseAddress.ToInt64();
            if (cachedProcessId != process.Id || cachedModuleBase != moduleBase || cachedModuleSize != module.ModuleMemorySize)
            {
                ResetAddresses();
                cachedProcessId = process.Id;
                cachedModuleBase = moduleBase;
                cachedModuleSize = module.ModuleMemorySize;
            }

            if (namesAddress != 0)
            {
                return;
            }

            if (DateTime.UtcNow < nextScan)
            {
                throw new InvalidDataException("FFVIIRB: waiting to retry patterns.");
            }

            nextScan = DateTime.UtcNow.AddSeconds(30);
            ulong Resolve(string pattern, int operand, int length)
            {
                var match = FindUniquePattern(moduleBase, module.ModuleMemorySize, pattern);
                var target = checked((ulong)((long)match + length + ReadInt(match + (ulong)operand)));
                if (target < moduleBase || target + 8 > moduleBase + (ulong)module.ModuleMemorySize)
                {
                    throw new InvalidDataException("FFVIIRB: RIP-relative target is outside the module.");
                }

                return target;
            }

            var namesAddressValue = Resolve(namesAddressPattern, 3, 7);
            var modeSlotValue = Resolve(modeSlotPattern, 3, 7);
            FindUniquePattern(moduleBase, module.ModuleMemorySize, NameEntryLayoutPattern);
            var gameDataSlotValue = Resolve(gameDataSlotPattern, 3, 7);
            var saveDataValue = Resolve(saveDataPattern, 3, 7);
            var groupOverrideFlagIndexValue = Resolve(groupOverrideFlagIndexPattern, 2, 6);
            FindUniquePattern(moduleBase, module.ModuleMemorySize, actorStatePattern);
            var componentStateValue = FindUniquePattern(moduleBase, module.ModuleMemorySize, componentStatePattern);
            var characterIdGetterValue = FindUniquePattern(moduleBase, module.ModuleMemorySize, characterIdGetterPattern);
            FindUniquePattern(moduleBase, module.ModuleMemorySize, recordIndexPattern);
            FindUniquePattern(moduleBase, module.ModuleMemorySize, recordSelectionPattern);
            FindUniquePattern(moduleBase, module.ModuleMemorySize, recordLevelPattern);
            FindUniquePattern(moduleBase, module.ModuleMemorySize, recordExperiencePattern);
            FindUniquePattern(moduleBase, module.ModuleMemorySize, recordHPPattern);
            FindUniquePattern(moduleBase, module.ModuleMemorySize, recordMPPattern);
            FindUniquePattern(moduleBase, module.ModuleMemorySize, recordBaseMaxHPPattern);
            FindUniquePattern(moduleBase, module.ModuleMemorySize, recordBaseMaxMPPattern);
            FindUniquePattern(moduleBase, module.ModuleMemorySize, recordWeaponLevelPattern);
            FindUniquePattern(moduleBase, module.ModuleMemorySize, recordWeaponPointsPattern);
            FindUniquePattern(moduleBase, module.ModuleMemorySize, maxEffectsPointerPattern);
            FindUniquePattern(moduleBase, module.ModuleMemorySize, maxEffectListLayoutPattern);
            FindUniquePattern(moduleBase, module.ModuleMemorySize, maxEffectDataLayoutPattern);
            FindUniquePattern(moduleBase, module.ModuleMemorySize, maxEffectFactorLayoutPattern);
            var maxMultiplierTypeGettersValue = FindUniquePattern(moduleBase, module.ModuleMemorySize, maxMultiplierTypeGettersPattern);
            var maxRoundBiasValue = Resolve(maxRoundBiasPattern, 47, 51);
            var locationSlotValue = Resolve(locationSlotPattern, 3, 7);
            var chapterSlotValue = Resolve(chapterSlotPattern, 3, 7);
            var locationAssetMapValue = Resolve(locationAssetMapPattern, 3, 7);
            var mapAssetRegistryValue = Resolve(mapAssetRegistryPattern, 3, 7);
            var naviMapCategorySlotValue = Resolve(naviMapCategorySlotPattern, 32, 36);
            FindUniquePattern(moduleBase, module.ModuleMemorySize, naviAssetMapLayoutPattern);
            var sceneManagerSlotValue = Resolve(sceneManagerSlotPattern, 3, 7);
            FindUniquePattern(moduleBase, module.ModuleMemorySize, ComponentLocationLayoutPattern);
            namesAddress = namesAddressValue;
            modeSlot = modeSlotValue;
            gameDataSlot = gameDataSlotValue;
            saveData = saveDataValue;
            groupOverrideFlagIndex = groupOverrideFlagIndexValue;
            componentState = componentStateValue;
            characterIdGetter = characterIdGetterValue;
            maxMultiplierTypeGetters = maxMultiplierTypeGettersValue;
            maxRoundBias = maxRoundBiasValue;
            locationSlot = locationSlotValue;
            chapterSlot = chapterSlotValue;
            locationAssetMap = locationAssetMapValue;
            mapAssetRegistry = mapAssetRegistryValue;
            naviMapCategorySlot = naviMapCategorySlotValue;
            sceneManagerSlot = sceneManagerSlotValue;
        }

        private static ulong FindUniquePattern(ulong moduleBase, int moduleSize, string signature)
        {
            if (moduleSize < 64)
            {
                throw new InvalidDataException("FFVIIRB: invalid module size.");
            }

            var peOffset = Hypervisor.Read<uint>(moduleBase + 0x3C, true);
            if (peOffset > (uint)moduleSize - 24)
            {
                throw new InvalidDataException("FFVIIRB: invalid PE offset.");
            }

            var pe = moduleBase + peOffset;
            if (peOffset > (uint)moduleSize - 24 || Hypervisor.Read<uint>(pe, true) != 0x4550 || Hypervisor.Read<ushort>(pe + 4, true) != 0x8664)
            {
                throw new InvalidDataException("FFVIIRB: invalid PE header.");
            }

            var sectionCount = Hypervisor.Read<ushort>(pe + 6, true);
            var optionalSize = Hypervisor.Read<ushort>(pe + 20, true);
            var sectionTable = pe + 24 + optionalSize;
            if (sectionTable + (ulong)sectionCount * 40 > moduleBase + (ulong)moduleSize)
            {
                throw new InvalidDataException("FFVIIRB: invalid section table.");
            }

            var pattern = SignaturePattern.Parse(signature);
            var prefix = pattern.Bytes.TakeWhile(value => value.HasValue).Select(value => value.GetValueOrDefault()).ToArray();
            if (prefix.Length == 0)
            {
                throw new InvalidDataException("FFVIIRB: pattern needs a fixed prefix.");
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
                    throw new InvalidDataException("FFVIIRB: invalid executable section.");
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
                            throw new InvalidDataException("FFVIIRB: manager pattern is ambiguous.");
                        }

                        match = address;
                    }
                }
            }

            return match != 0 ? match : throw new InvalidDataException("FFVIIRB: manager pattern was not found.");
        }

        private static void InitializeDiscord()
        {
            if (discord == null)
            {
                throw new InvalidOperationException("FFVIIRB: presence is not initialized.");
            }

            discord.Initialize();
        }
    }
}
