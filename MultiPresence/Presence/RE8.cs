using MultiPresence.Runtime;
using MultiPresence.Infrastructure;
using MultiPresence.Models.RE8;
using DiscordRPC;

namespace MultiPresence.Presence
{
    public class RE8
    {
        private static readonly (string Name, string Pattern, int Operand, int Length)[] Patterns = [("Scene", "48 8B 15 ?? ?? ?? ?? 4C 39 40 18 49 0F 45 D0 EB ?? 49 8B D0 48 8B 41 50 48 83 78 18 00 75 ?? 48 85 D2 75 ?? 45 33 C0 41 8D 50 38 E8 ?? ?? ?? ?? 33 C0 48 83 C4 28 C3 48 8B 42 58 48 83 C4 28", 3, 7), ("Map", "48 0F 45 1D ?? ?? ?? ?? 66 89 6C 24 24 66 44 89 74 24 26 4C 89 7C 24 28 48 85 DB 74 ?? 4C 8D 44 24 20 48 8B D3 E8 ?? ?? ?? ?? EB ?? 48 39 1D ?? ?? ?? ?? 89 74 24 30 48 0F 45 1D ?? ?? ?? ??", 4, 8), ("RoseMap", "48 8B 05 ?? ?? ?? ?? C3 CC CC CC CC CC CC CC CC 48 83 EC 38 8B 44 24 60", 3, 7), ("RoseMode", "48 8B 05 ?? ?? ?? ?? C3 CC CC CC CC CC CC CC CC 48 83 EC 38 0F 10 02", 3, 7), ("MercenariesMode", "48 8B 15 ?? ?? ?? ?? 48 83 78 18 00 74 ?? 45 32 C0 EB ?? 48 85 D2 74 ?? 48 83 7A 10 00 74 ?? 32 C0 EB ?? B0 01 0F B6 C0 85 C0 74 ?? 45 32 C0 EB ?? 48 85 D2 75 ?? 45 33 C0 41 8D 50 38 E8 ?? ?? ?? ?? 48 8B 15 ?? ?? ?? ?? 45 32 C0 EB ?? 80 7A 60 00", 3, 7), ("SceneViewLayout", "48 8B 42 58 C3 CC CC CC CC CC CC CC CC CC CC CC 48 89 5C 24 08", 0, 0), ("SceneChapterLayout", "48 8B 42 60 C3 CC CC CC CC CC CC CC CC CC CC CC", 0, 0), ("SceneTransitionLayout", "83 BA 88 00 00 00 00 0F 95 C0 C3 CC CC CC CC CC", 0, 0), ("MapRoomLayout", "48 8B 82 D8 00 00 00 C3 CC CC CC CC CC CC CC CC", 0, 0), ("RoseMapRoomLayout", "48 8B 82 F8 00 00 00 C3 CC CC CC CC CC CC CC CC", 0, 0), ("MapReadyLayout", "80 BA EC 00 00 00 00 0F 95 C0 C3 CC CC CC CC CC", 0, 0), ("RoseMapReadyLayout", "80 BA 0C 01 00 00 00 0F 95 C0 C3 CC CC CC CC CC", 0, 0), ("RoomGuidLayout", "41 8B 40 48 41 0F B7 50 4C 45 0F B7 48 4E 4D 8B 40 50", 0, 0), ("RoomMapIdLayout", "8B 42 5C C3 CC CC CC CC CC CC CC CC CC CC CC CC", 0, 0), ("RoomNameHashLayout", "8B 42 60 C3 CC CC CC CC CC CC CC CC CC CC CC CC", 0, 0), ("RoseModeLayout", "80 7A 51 00 0F 95 C0 C3 CC CC CC CC CC CC CC CC", 0, 0), ("MercenariesFlagsLayout", "80 7A 60 00 41 0F 95 C0 48 8B 43 50 48 83 78 18 00", 0, 0), ("MercenaryStageLayout", "8B 42 64 C3 CC CC CC CC CC CC CC CC CC CC CC CC", 0, 0), ("MercenariesFlagLayout", "80 7A 61 00 0F 95 C1 48 8B 43 50 48 83 78 18 00 75 ?? 84 C9 0F 95 C0", 0, 0), ("Difficulty", "48 8B 05 ?? ?? ?? ?? C3 CC CC CC CC CC CC CC CC 48 89 5C 24 08 48 89 74 24 10 57 48 83 EC 20 48 8B FA 45 33 C0 48 8B 15 ?? ?? ?? ?? 48 8B F1 E8 ?? ?? ?? ?? 48 8B D8 48 8B 46 50 48 83 78 18 00 75 ?? C7 43 10 00 00 00 00 48 8B 46 50 48 83 78 18 00 74 ?? 33 C0 48 8B 5C 24 30 48 8B 74 24 38 48 83 C4 20 5F C3 8B 43 08 85 C0 79 ?? 48 89 7B 20 48 8B C3 48 8B 5C 24 30 48 8B 74 24 38 48 83 C4 20 5F C3 48 3B 7B 20 74 ?? 48 85 FF 74 ?? 48 8B CF E8 ?? ?? ?? ?? 48 8B 4B 20 48 8B C1 F0 48 0F B1 7B 20 74 ?? 0F 0D 4B 20 66 0F 1F 44 00 00 48 8B 4B 20 48 8B C1 F0 48 0F B1 7B 20 75 ?? 48 85 C9 74 ?? E8 ?? ?? ?? ?? 48 8B 74 24 38 48 8B C3 48 8B 5C 24 30 48 83 C4 20 5F C3 CC CC CC CC 44 85 82 30 01 00 00", 3, 7), ("RoseDifficulty", "48 8B 05 ?? ?? ?? ?? C3 CC CC CC CC CC CC CC CC 48 89 5C 24 08 48 89 74 24 10 57 48 83 EC 20 48 8B 05 ?? ?? ?? ?? 45 33 C0 48 8B 15 ?? ?? ?? ?? 48 8B F1 48 8B 58 60 E8 ?? ?? ?? ?? 4C 8B C3 48 8B D0 48 8B CE 48 8B F8 E8 ?? ?? ?? ?? 48 8B 4E 50 33 C0 48 8B 5C 24 30 48 8B 74 24 38 48 39 41 18 48 0F 45 F8 48 8B C7 48 83 C4 20 5F C3 CC CC 8B 12", 3, 7), ("DifficultyValueLayout", "48 8B 42 78 48 85 C0 75 ?? 45 33 C0 41 8D 50 38 E8 ?? ?? ?? ?? 33 C0 48 83 C4 28 C3 48 8B 40 28 48 85 C0 74 ?? 8B 40 10", 0, 0), ("RoseDifficultyValueLayout", "48 8B 42 68 48 85 C0 75 ?? 45 33 C0 41 8D 50 38 E8 ?? ?? ?? ?? 33 C0 48 83 C4 28 C3 48 8B 40 28 48 85 C0 74 ?? 8B 40 10", 0, 0), ("MercenaryScore", "48 8B 0D ?? ?? ?? ?? 48 85 C9 74 ?? 48 83 79 10 00 74 ?? 32 C0 EB ?? B0 01 0F B6 C0 85 C0 75 ?? 48 85 C9 75 ?? 45 33 C0 8D 50 38 48 8B CF E8 ?? ?? ?? ?? EB ?? 48 8B 41 60 48 85 C0 74 ?? 48 8B 58 10 48 8B 47 50 48 83 78 18 00 75 ?? 48 85 DB 75 ??", 3, 7), ("ScoreLayout", "4C 8B 42 60 4D 85 C0 74 ?? 48 8B 41 50 48 83 78 18 00 75 ?? 41 8B 40 18 C3 33 C0 C3", 0, 0), ("AreaScoreLayout", "4C 8B 42 60 4D 85 C0 74 ?? 48 8B 41 50 48 83 78 18 00 75 ?? 41 8B 40 1C C3 33 C0 C3", 0, 0), ("MoneyLayout", "48 8B 46 50 48 8B 58 18 48 85 DB 74 ?? 48 89 78 18 8B 6C 24 40 EB ?? 8B 6D 48 48 8B DF BF 24 00 00 00", 0, 0), ("Player", "48 8B 0D ?? ?? ?? ?? 48 85 C9 74 ?? 48 83 79 10 00 75 ?? B0 01 0F B6 C0 85 C0 0F 84 ?? ?? ?? ??", 3, 7), ("ConfigureType", "48 8B 15 ?? ?? ?? ?? 48 8B 08 E8 ?? ?? ?? ?? 84 C0 75 ?? 48 8B DD 48 8B 46 50 48 39 68 18 0F 85 ?? ?? ?? ?? 48 85 DB", 3, 7), ("GunType", "48 8B 15 ?? ?? ?? ?? 48 8B 08 E8 ?? ?? ?? ?? 84 C0 75 ?? 33 DB 48 8B C3 48 8B 5C 24 30 48 83 C4 20 5F C3 48 8B 5C 24 30 33 C0 48 83 C4 20 5F C3 CC CC CC CC CC CC CC CC CC CC 48 83 3D ?? ?? ?? ?? 00", 3, 7), ("ComponentListLayout", "48 8B 59 18 48 8B FA 48 85 DB 74 ?? 48 8B 03 48 8B CB FF 50 18 48 3B C7 74 ?? 48 8B D7 48 8B C8 E8 ?? ?? ?? ?? 84 C0 75 ?? 33 C0 EB ?? B8 01 00 00 00 84 C0 75 ?? 48 8B 5B 18 48 85 DB 75 ?? 33 C0", 0, 0), ("TypeParentLayout", "48 8B 41 38 48 85 C0 74 ?? 0F 1F 44 00 00 48 3B C2 74 ?? 48 8B 40 38 48 85 C0 75 ??", 0, 0), ("ConfigureReferenceLayout", "48 8B 42 78 C3 CC", 0, 0), ("ReferenceStatusLayout", "48 8B 9F 90 00 00 00 48 85 DB 74 ?? 48 8B 03 48 8B 15 ?? ?? ?? ??", 0, 0), ("StatusReferenceLayout", "48 8B F2 48 8B F9 48 8B 92 C8 00 00 00 48 85 D2 75 ?? 33 DB 8B C3 EB ?? 48 8B 02 48 8B 48 E8 48 8B 41 28 48 8B CF FF D0 48 8B 4F 50 33 DB 48 39 59 18 48 0F 45 C3 48 8B 4F 50 48 8B 51 18 48 85 D2 75 ?? 48 85 C0 74 ?? 4C 8B 86 C8 00 00 00 4D 85 C0 74 ?? 49 8B 00 49 8B D0 48 8B 48 E8 48 8B 41 28 48 8B CF FF D0 48 8B 4F 50 48 8B 51 18 48 85 D2 48 0F 45 C3 48 8B D8 48 85 D2 75 ?? 48 85 DB 75 ?? 45 33 C0 8D 53 38 48 8B CF E8 ?? ?? ?? ?? 32 C0 48 8B 5C 24 30 48 8B 74 24 38 48 83 C4 20 5F C3 48 8B 03 48 8B D3 48 8B 48 E0 48 8B 01", 0, 0), ("ReferenceDamageLayout", "48 8B 42 68 C3 CC CC CC CC CC CC CC CC CC CC CC 48 83 C2 10", 0, 0), ("DamageHealthLayout", "48 8B 43 48 48 85 C0 74 ?? F3 0F 10 40 10 EB ?? 0F 57 C0", 0, 0), ("HealthMaxGetter", "F3 0F 10 42 10 C3 CC CC CC CC CC CC CC CC CC CC 48 8B 82 F0 00 00 00", 0, 0), ("HealthCurrentGetter", "F3 0F 10 42 14 C3 CC", 0, 0), ("ReferenceResponserLayout", "48 8B 9A 60 01 00 00 48 83 78 18 00 74 ?? 33 C0 48 83 C4 20", 0, 0), ("DyingGetter", "80 BA 04 01 00 00 00 0F 95 C0 C3", 0, 0), ("CautionGetter", "80 BA EC 00 00 00 00 0F 95 C0 C3", 0, 0), ("ReferenceEquipLayout", "48 8B 42 18 48 85 C0 74 ?? 48 8B 80 A8 00 00 00 EB ?? 48 8B C7 48 85 C9 75 ?? 48 85 C0 74 ?? 48 8B 52 18 48 85 D2 74 ?? 48 8B 92 A8 00 00 00 EB ?? 48 8B D7 48 8B CB 48 85 D2 75 ?? 45 33 C0 41 8D 50 38 E8 ?? ?? ?? ?? 33 C0", 0, 0), ("DualWieldGetter", "80 BA 9E 00 00 00 00 0F 95 C0 C3 CC CC CC CC CC 48 89 5C 24 08", 0, 0), ("LeftHandGetter", "80 BA 9F 00 00 00 00 0F 95 C0 C3", 0, 0), ("EquipSelectionLayout", "85 C9 74 ?? 48 8B 43 30 48 8B 5C 24 30 48 8B 74 24 38 48 83 C4 20 5F C3 48 8B 43 28 48 8B 5C 24 30 48 8B 74 24 38 48 83 C4 20 5F C3", 0, 0), ("GunWorkLayout", "48 8B 47 58 48 85 C0 75 ?? 8D 51 38 45 33 C0 48 8B CB E8 ?? ?? ?? ?? 32 C0 48 8B 5C 24 30 48 83 C4 20 5F C3 83 78 58 00 48 8B 5C 24 30 0F 9E C0 48 83 C4 20 5F C3", 0, 0), ("ItemIdGetter", "8B 42 3C C3 CC CC CC", 0, 0), ("GunBulletIndexLayout", "44 8B 87 84 01 00 00 48 8B D7 48 8B CB E8 ?? ?? ?? ?? 8B F0", 0, 0), ("BulletListLayout", "48 8B 42 30 48 85 C0 74 ?? 48 83 78 28 00 74 ?? 48 8B 48 28 48 85 C9 75 ?? 45 33 C0 48 8B CB 41 8D 50 38 E8 ?? ?? ?? ?? 33 C0 48 83 C4 20 5B C3 48 8B 49 30 48 85 C9 74 ?? 83 79 18 00 7E ?? 48 8B 40 28 48 85 C0 74 ?? 48 8B 40 30 48 85 C0 74 ?? 44 3B 40 18 7D ?? 48 8B 48 10", 0, 0), ("BulletEntryLayout", "44 3B 40 18 7D ?? 48 8B 48 10 48 85 C9 75 ?? 8D 51 38 45 33 C0 48 8B CB E8 ?? ?? ?? ?? EB ?? 44 3B 41 1C 72 ?? 45 33 C0 48 8B CB 41 8D 50 37 E8 ?? ?? ?? ?? EB ?? 49 63 C0 48 8B 4C C1 20 EB ?? 48 8B 15 ?? ?? ?? ??", 0, 0), ("BulletIdLayout", "48 85 C9 0F 84 ?? ?? ?? ?? 8B 41 10 48 83 C4 20 5B C3 48 89 5C 24 18", 0, 0), ("InventoryListLayout", "48 89 5C 24 18 55 56 41 56 48 83 EC 50 48 8B 41 50 33 F6 41 0F B6 E9 45 8B F0 48 8B D9 48 39 70 18 74 ?? 33 C0 48 8B 9C 24 80 00 00 00 48 83 C4 50 41 5E 5E 5D C3 4C 8B 42 18 48 89 7C 24 70 40 38 B4 24 90 00 00 00 74 ?? 4C 8B 42 20 8B FE 4D 85 C0 75 ?? BA 38 00 00 00 E8 ?? ?? ?? ??", 0, 0), ("ReserveRulesLayout", "48 8B 40 58 48 85 C0 74 ?? 40 38 70 39 75 ?? 40 84 ED 75 ?? 48 85 C0 74 ?? 40 38 70 64 75 ?? 48 85 C0 74 ?? 44 39 70 3C 75 ?? 48 85 C0 74 ?? 03 78 4C", 0, 0), ("ObjectNameLayout", "48 8B 42 50 C3 CC CC CC", 0, 0), ("PawnNameLayout", "48 8B 05 ?? ?? ?? ?? 48 8B D9 48 8B 52 30 4C 8B 80 D0 00 00 00 E8 ?? ?? ?? ??", 0, 0), ("Inventory", "4C 8B 15 ?? ?? ?? ?? 4D 85 C9 74 ?? 48 8B 41 50 4D 8B C2 48 39 50 18 4C 0F 45 C2 EB ?? 4C 8B C2 48 8B 41 50 48 83 78 18 00 75 ?? 4D 85 C0 74 ?? 49 83 78 10 00 74 ?? 32 C0 EB ?? B0 01 0F B6 C0 85 C0 75 ?? 4D 85 C9 49 0F 45 D2 48 85 D2 75 ?? 45 33 C0 8D 50 38 E8 ?? ?? ?? ?? 33 C0", 3, 7), ("InventoryActiveLayout", "48 8B 42 60 48 83 C4 28 C3 CC CC CC CC CC CC CC CC 48 89 5C 24 08 57 48 83 EC 30 48 8B FA", 0, 0), ("ItemSpecIdGetter", "8B 42 10 C3 CC CC CC CC CC CC CC CC CC CC CC CC 48 83 EC 28", 0, 0), ("CPShop", "48 0F 45 3D ?? ?? ?? ?? 48 85 FF 75 ?? 45 33 C0 41 8D 50 38 E8 ?? ?? ?? ?? 48 8B 7C 24 30 32 C0 48 8B 5C 24 38 48 83 C4 20 5E C3 44 8B 05 ?? ?? ?? ??", 4, 8), ("CPShopReady", "48 39 3D ?? ?? ?? ?? 48 8B CB 48 0F 45 3D ?? ?? ?? ?? 48 85 FF 75 ?? 45 33 C0 41 8D 50 38 E8 ?? ?? ?? ?? 48 8B 7C 24 30 32 C0 48 8B 5C 24 38 48 83 C4 20 5E C3 44 8B 05 ?? ?? ?? ??", 3, 7), ("Entitlement", "48 8B 15 ?? ?? ?? ?? 48 85 D2 74 ?? 44 8B 40 0C 48 8B CB E8 ?? ?? ?? ?? 0F B6 C8 48 8B 43 50 48 39 68 18", 3, 7), ("EntitlementReady", "48 39 2D ?? ?? ?? ?? 74 ?? 48 8B 15 ?? ?? ?? ?? 48 85 D2 74 ?? 44 8B 40 0C 48 8B CB E8 ?? ?? ?? ??", 3, 7), ("InfiniteSuppress", "48 8B 0D ?? ?? ?? ?? 48 85 C9 74 ?? 48 83 79 10 00 74 ?? 32 C0 EB ?? B0 01 0F B6 C0 85 C0 74 ?? 32 C9 EB ?? 48 85 C9 75 ?? 8D 51 38 45 33 C0 48 8B CB E8 ?? ?? ?? ?? 32 C9 EB ?? 80 79 61 00 0F 95 C1 48 8B 43 50 48 83 78 18 00 74 ?? 32 C0 48 8B 5C 24 38", 3, 7), ("InfiniteOptions", "48 0F 45 05 ?? ?? ?? ?? 48 85 C0 74 ?? 48 8B 50 58 48 85 D2 75 ?? 45 33 C0 8D 57 38 E8 ?? ?? ?? ?? 8B CF", 4, 8), ("InfiniteOptionsReady", "48 39 05 ?? ?? ?? ?? 48 0F 45 05 ?? ?? ?? ?? 48 85 C0 74 ?? 48 8B 50 58 48 85 D2 75 ?? 45 33 C0 8D 57 38 E8 ?? ?? ?? ?? 8B CF", 3, 7), ("InfiniteOptionKey", "44 8B 05 ?? ?? ?? ?? 45 33 C9 48 8B 52 18 E8 ?? ?? ?? ?? 48 8B 4B 50 48 39 79 18 75 ?? 48 85 C0 74 ?? 8B 48 18 EB ?? 44 8B 46 3C", 3, 7), ("InfiniteEnabledValue", "3B 0D ?? ?? ?? ?? 75 ?? 48 39 3D ?? ?? ?? ?? 48 8B CB 48 0F 45 3D ?? ?? ?? ?? 48 85 FF 75 ?? 45 33 C0", 2, 6), ("InfiniteItemMatch", "48 8B 41 50 48 83 78 18 00 75 ?? 8B 42 10 41 39 80 A8 00 00 00 75 ?? B0 01 48 83 C4 28 C3 41 39 80 AC 00 00 00", 0, 0), ("InfinitePurchased", "4D 85 F6 0F 84 ?? ?? ?? ?? 40 38 68 14 0F 84 ?? ?? ?? ?? 40 38 68 15 0F 85 ?? ?? ?? ?? 4C 8B 40 18", 0, 0), ("EntitlementLookup", "48 8B 4E 18 48 85 C9 75 ?? 8D 51 38 45 33 C0 48 8B CF E8 ?? ?? ?? ?? 32 C0 48 8B 5C 24 30 48 8B 74 24 38 48 83 C4 20 5F C3 3B 41 1C 72 ?? 45 33 C0 48 8B CF 41 8D 50 37 E8 ?? ?? ?? ?? 32 C0 48 8B 5C 24 30 48 8B 74 24 38 48 83 C4 20 5F C3 48 8B 74 24 38 48 98", 0, 0), ("PlayerObjects", "4C 8B 0D ?? ?? ?? ?? 48 8B F9 4D 85 C0 74 ?? 48 8B 41 50 49 8B C9 48 39 58 18 48 0F 45 CB EB ??", 3, 7), ("PlayerObjectsReady", "4C 8B 05 ?? ?? ?? ?? 33 DB 4C 8B 0D ?? ?? ?? ?? 48 8B F9 4D 85 C0 74 ?? 48 8B 41 50 49 8B C9 48 39 58 18", 3, 7), ("PlayerObjectsDictionaryLayout", "48 8B 57 60 45 33 C9 45 8B C7 48 8B CD E8 ?? ?? ?? ?? 48 8B F8 48 8B 45 50 48 8B 70 18 48 85 F6 74 ?? 48 89 58 18 48 8B 7C 24 60 EB ?? 48 85 FF", 0, 0), ("PlayerObjectEntriesLayout", "48 8B 56 18 48 85 D2 0F 84 ?? ?? ?? ?? 3B 5A 1C 73 ?? 48 63 C3 48 8D 0C 40 8B 42 08 48 8D 2C CD 00 00 00 00 48 8D 4A 20 48 03 CD 85 C0 78 ?? 49 0B CC 49 23 CD 44 39 31 75 ?? 48 8B 46 18 48 8B 56 30 48 85 C0 0F 84 ?? ?? ?? ?? 3B 58 1C 0F 83 ?? ?? ?? ?? 48 8D 48 20 8B 40 08 48 03 CD 85 C0 78 ?? 49 0B CC 49 23 CD 44 8B 41 08", 0, 0), ("PlayerHash0", "41 B8 23 65 F8 70 E8 ?? ?? ?? ?? 48 8B D0 48 8B 47 50 48 8B 48 18 48 85 C9", 0, 0), ("PlayerHash1", "41 B8 10 EF 0A 46 E8 ?? ?? ?? ?? 48 8B D0 48 8B 47 50 48 8B 48 18 48 85 C9", 0, 0), ("PlayerHash2", "41 B8 52 05 B3 F1 E8 ?? ?? ?? ?? 48 8B D0 48 8B 47 50 48 8B 48 18 48 85 C9", 0, 0), ("PlayerHash3", "41 B8 AF C0 E4 98 E8 ?? ?? ?? ?? 48 8B D0 48 8B 47 50 48 8B 48 18 48 85 C9", 0, 0), ("PlayerHash4", "41 B8 5E 2E 02 C7 E8 ?? ?? ?? ?? 48 8B D0 48 8B 47 50 48 8B 48 18 48 85 C9", 0, 0), ("PlayerHash5", "41 B8 76 9F FB C4 E8 ?? ?? ?? ?? 48 8B D0 48 8B 47 50 48 8B 48 18 48 85 C9", 0, 0), ];
        private static DiscordRpcClient? discord;
        private static DiscordStatusUpdater? updater;
        private static int cachedProcessId, cachedModuleSize;
        private static ulong cachedModuleBase;
        private static Dictionary<string, ulong> addresses = new Dictionary<string, ulong>();
        private static DateTime nextScan;
        public static void DoAction()
        {
            ResetAddresses();
            GetPID();
            discord = new DiscordRpcClient("1402217219546677248");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/config/Resident Evil 8.json");
            PresenceRuntime.Start(nameof(RE8), "re8", RPC);
        }

        private static void GetPID()
        {
            ProcessMonitor.TryAttach("re8");
        }

        private static async Task RPC()
        {
            if (discord == null || updater == null)
            {
                throw new InvalidOperationException("RE8: presence is not initialized.");
            }

            if (ProcessMonitor.IsRunning("re8"))
            {
                try
                {
                    if (!Hypervisor.IsAttached && !ProcessMonitor.TryAttach("re8"))
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
                    var profile = snapshot.GameMode == "Shadows of Rose" ? "Shadow of Rose" : snapshot.GameMode == "The Mercenaries" ? "Mercenaries" : "Main Story";
                    PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Resident Evil 8", placeholders, profile);
                }
                catch (Exception exception) when (exception is not OutOfMemoryException and not StackOverflowException)
                {
                    AppLog.Warning("Resident Evil Village memory is not ready.", exception);
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

            return Task.FromResult(new Dictionary<string, object> { { "gamemode", snapshot.GameMode }, { "character", snapshot.Character }, { "characterid", snapshot.PawnId }, { "difficulty", snapshot.Difficulty }, { "difficultyid", snapshot.DifficultyId }, { "health", snapshot.Health }, { "maxhealth", snapshot.MaxHealth }, { "healthstatus", snapshot.HealthStatus }, { "healthpercentage", (double)snapshot.Health / snapshot.MaxHealth * 100 }, { "healthstatusemoji", snapshot.HealthStatus switch { "Fine" => "🟢", "Caution" => "🟡", "Danger" or "Dead" => "🔴", _ => "", } }, { "region", snapshot.Region }, { "room", snapshot.Room }, { "location", snapshot.Location }, { "roomid", snapshot.RoomId }, { "roomguid", snapshot.RoomGuid }, { "mapid", snapshot.MapId }, { "weapon", snapshot.Weapon }, { "weaponid", snapshot.WeaponId }, { "ammo", snapshot.Ammo }, { "loadedammo", snapshot.Ammo }, { "reserveammo", snapshot.InfiniteAmmo == true ? "∞" : snapshot.ReserveAmmo < 0 ? "" : snapshot.ReserveAmmo }, { "infiniteammo", snapshot.InfiniteAmmo.HasValue ? snapshot.InfiniteAmmo.Value : "" }, { "money", snapshot.Money }, { "lei", snapshot.Money }, { "score", snapshot.Score }, { "areascore", snapshot.AreaScore } });
        }

        private static void ShowStatus(string text)
        {
            if (discord == null)
            {
                throw new InvalidOperationException("RE8: presence is not initialized.");
            }

            discord.SetPresence(new RichPresence()
            {
                Details = text,
                State = "",
                Assets = new Assets()
                {
                    LargeImageKey = "logo",
                    LargeImageText = "Resident Evil Village"
                },
                Timestamps = PlaceholderHelper._startTimestamp
            });
        }

        private sealed record Snapshot(bool IsInGame, string GameMode, string PawnId, string Character, uint DifficultyId, string Difficulty, float Health, float MaxHealth, string HealthStatus, Guid RoomGuid, uint RoomId, uint MapId, string Region, string Room, string Location, uint WeaponId, string Weapon, int Ammo, int ReserveAmmo, bool? InfiniteAmmo, int Money, int Score, int AreaScore);
        private static Snapshot EmptySnapshot()
        {
            return new Snapshot(false, "", "", "", 0, "", 0, 0, "", Guid.Empty, 0, 0, "", "", "", 0, "", 0, 0, false, 0, 0, 0);
        }

        private static Snapshot ReadSnapshot()
        {
            ResolveAddresses();
            var context = ReadContext();
            if (context == null)
            {
                return EmptySnapshot();
            }

            var player = ReadPlayer();
            if (player == null)
            {
                return EmptySnapshot();
            }

            var location = ReadLocation(context);
            var difficulty = ReadDifficulty(context);
            var score = ReadScore(context);
            if (context != ReadContext() || player != ReadPlayer() || location != ReadLocation(context) || difficulty != ReadDifficulty(context) || score != ReadScore(context))
            {
                throw new InvalidDataException("RE8: scene changed during the read; retry next poll.");
            }

            return new Snapshot(true, context.Mode, player.PawnId, Characters.GetName(player.PawnId), difficulty.Id, Difficulties.GetName(difficulty.Id), player.Health, player.MaxHealth, player.HealthStatus, location.Guid, location.RoomId, location.MapId, location.Region, location.Room, location.Location, player.WeaponId, player.WeaponId == 0 ? "None" : Weapons.GetName(player.WeaponId), player.Ammo, player.ReserveAmmo, player.InfiniteAmmo, player.Money, score.Total, score.Area);
        }

        private sealed record Context(ulong Scene, ulong ViewPointer, string View, ulong ChapterPointer, string Chapter, string Mode, ulong Rose, byte RoseFlag, ulong Mercenaries, byte MercenariesFlag, byte MercenariesGeFlag, uint StageHash);
        private static Context? ReadContext()
        {
            var scene = Pointer(addresses["Scene"]);
            if (!NativeObject(scene) || ReadInt(scene + 0x88) != 0)
            {
                return null;
            }

            var viewPointer = Pointer(scene + 0x58);
            var chapterPointer = Pointer(scene + 0x60);
            var view = ReadString(viewPointer);
            var chapter = ReadString(chapterPointer);
            var mode = Modes.GetName(view);
            if (mode.Length == 0)
            {
                return null;
            }

            var rose = Pointer(addresses["RoseMode"]);
            var roseFlag = NativeObject(rose) ? ReadByte(rose + 0x51) : (byte)0;
            var mercenaries = Pointer(addresses["MercenariesMode"]);
            var mercenariesFlag = NativeObject(mercenaries) ? ReadByte(mercenaries + 0x60) : (byte)0;
            var mercenariesGeFlag = NativeObject(mercenaries) ? ReadByte(mercenaries + 0x61) : (byte)0;
            if (mode == "Shadows of Rose" && roseFlag == 0 || mode == "The Mercenaries" && mercenariesFlag == 0 && mercenariesGeFlag == 0)
            {
                return null;
            }

            var stageHash = mode == "The Mercenaries" ? ReadUInt(mercenaries + 0x64) : 0;
            return new Context(scene, viewPointer, view, chapterPointer, chapter, mode, rose, roseFlag, mercenaries, mercenariesFlag, mercenariesGeFlag, stageHash);
        }

        private sealed record LocationState(ulong Map, byte Ready, ulong Unit, Guid Guid, uint RoomId, uint MapId, string Region, string Room, string Location);
        private static LocationState ReadLocation(Context context)
        {
            var rose = context.Mode == "Shadows of Rose";
            var map = Pointer(addresses[rose ? "RoseMap" : "Map"]);
            var ready = NativeObject(map) ? ReadByte(map + (rose ? 0x10Cu : 0xECu)) : (byte)0;
            var unit = ready == 0 ? 0 : Pointer(map + (rose ? 0xF8u : 0xD8u));
            var guid = unit == 0 ? Guid.Empty : new Guid(Hypervisor.ReadArray(unit + 0x48, 16, true));
            var mapId = unit == 0 ? 0 : ReadUInt(unit + 0x5C);
            var roomId = unit == 0 ? 0 : ReadUInt(unit + 0x60);
            var region = Locations.GetRegion(guid);
            var room = Locations.GetRoom(guid);
            if (region.Length == 0)
            {
                region = Locations.GetRegion(roomId, rose);
            }

            if (room.Length == 0)
            {
                room = Locations.GetRoom(roomId, rose);
            }

            if (region.Length == 0)
            {
                region = Locations.GetMapRegion(mapId, rose);
            }

            var location = region.Length == 0 ? room : room.Length == 0 ? region : $"{region} / {room}";
            if (context.Mode == "The Mercenaries")
            {
                var stage = MercenaryStages.GetName(context.StageHash);
                if (stage.Length != 0)
                {
                    region = stage;
                    location = room.Length == 0 ? stage : $"{stage} / {room}";
                }
            }

            return new LocationState(map, ready, unit, guid, roomId, mapId, region, room, location);
        }

        private sealed record DifficultyState(ulong Manager, ulong Record, ulong Value, uint Id);
        private static DifficultyState ReadDifficulty(Context context)
        {
            if (context.Mode == "The Mercenaries")
            {
                return new DifficultyState(0, 0, 0, 0);
            }

            var rose = context.Mode == "Shadows of Rose";
            var manager = Pointer(addresses[rose ? "RoseDifficulty" : "Difficulty"]);
            var record = NativeObject(manager) ? Pointer(manager + (rose ? 0x68u : 0x78u)) : 0;
            var value = record == 0 ? 0 : Pointer(record + 0x28);
            return new DifficultyState(manager, record, value, value == 0 ? 0 : ReadUInt(value + 0x10));
        }

        private sealed record ScoreState(ulong Manager, ulong Container, int Total, int Area);
        private static ScoreState ReadScore(Context context)
        {
            if (context.Mode != "The Mercenaries")
            {
                return new ScoreState(0, 0, 0, 0);
            }

            var manager = Pointer(addresses["MercenaryScore"]);
            var container = NativeObject(manager) ? Pointer(manager + 0x60) : 0;
            var total = container == 0 ? 0 : ReadInt(container + 0x18);
            var area = container == 0 ? 0 : ReadInt(container + 0x1C);
            if (total < 0 || area < 0)
            {
                throw new InvalidDataException("RE8: invalid Mercenaries score.");
            }

            return new ScoreState(manager, container, total, area);
        }

        private sealed record PlayerState(string Identity, string PawnId, float Health, float MaxHealth, string HealthStatus, uint WeaponId, int Ammo, int ReserveAmmo, bool? InfiniteAmmo, int Money);
        private static PlayerState? ReadPlayer()
        {
            var definition = FindPlayerDefinition();
            if (!NativeObject(definition))
            {
                return null;
            }

            var gameObject = Pointer(definition + 0x10);
            var configure = FindPlayerComponent(gameObject, addresses["ConfigureType"]);
            if (configure == 0)
            {
                return null;
            }

            var reference = Pointer(configure + 0x78);
            if (reference == 0)
            {
                return null;
            }

            var status = Pointer(reference + 0x90);
            if (status == 0 || Pointer(status + 0x10) != gameObject || Pointer(status + 0xC8) != reference)
            {
                throw new InvalidDataException("RE8: inconsistent player ownership.");
            }

            var objectName = Pointer(definition + 0x50);
            var pawnName = objectName == 0 ? 0 : Pointer(objectName + 0x30);
            var pawnId = ReadString(pawnName);
            if (pawnId.Length == 0)
            {
                return null;
            }

            var damage = Pointer(reference + 0x68);
            var healthInfo = damage == 0 ? 0 : Pointer(damage + 0x48);
            if (healthInfo == 0)
            {
                return null;
            }

            var health = ReadFloat(healthInfo + 0x14);
            var maxHealth = ReadFloat(healthInfo + 0x10);
            if (!float.IsFinite(health) || !float.IsFinite(maxHealth) || maxHealth <= 0 || maxHealth > 10_000_000 || health < 0 || health > maxHealth)
            {
                throw new InvalidDataException("RE8: invalid health.");
            }

            var responser = Pointer(reference + 0x160);
            var healthStatus = health <= 0 ? "Dead" : "Unknown";
            if (responser != 0 && Pointer(responser + 0x10) == gameObject)
            {
                var table = Pointer(Pointer(responser) - 0x20);
                if (table != 0 && Pointer(table) == addresses["DyingGetter"] && Pointer(table + 8) == addresses["CautionGetter"])
                {
                    healthStatus = health <= 0 ? "Dead" : ReadByte(responser + 0x104) != 0 ? "Danger" : ReadByte(responser + 0xEC) != 0 ? "Caution" : "Fine";
                }
            }

            var inventoryManager = Pointer(addresses["Inventory"]);
            var inventory = NativeObject(inventoryManager) ? Pointer(inventoryManager + 0x60) : 0;
            var money = inventory == 0 ? 0 : ReadInt(inventory + 0x48);
            if (money < 0)
            {
                throw new InvalidDataException("RE8: invalid money.");
            }

            var equip = Pointer(reference + 0xA8);
            ulong weapon = 0, work = 0, spec = 0, weaponSpec = 0, bulletList = 0, inventoryList = 0;
            uint weaponId = 0;
            var ammo = 0;
            var reserve = -1;
            bool? infinite = false;
            ulong inventoryArray = 0;
            var inventoryCount = 0;
            var selectedBullet = -1;
            if (equip != 0)
            {
                if (Pointer(equip + 0x10) != gameObject)
                {
                    throw new InvalidDataException("RE8: equipment owner changed.");
                }

                var statusTable = Pointer(Pointer(status) - 0x38);
                if (statusTable == 0 || Pointer(statusTable + 0x5F8) != addresses["DualWieldGetter"] || Pointer(statusTable + 0x608) != addresses["LeftHandGetter"])
                {
                    throw new InvalidDataException("RE8: unsupported weapon-selection implementation.");
                }

                var left = ReadByte(status + 0x9E) != 0 && ReadByte(status + 0x9F) != 0;
                weapon = Pointer(equip + (left ? 0x30u : 0x28u));
                if (weapon != 0)
                {
                    if (Pointer(weapon + 0x70) != gameObject)
                    {
                        throw new InvalidDataException("RE8: weapon owner changed.");
                    }

                    work = Pointer(weapon + 0x58);
                    spec = Pointer(weapon + 0x30);
                    if (work == 0 || spec == 0)
                    {
                        throw new InvalidDataException("RE8: weapon data is loading.");
                    }

                    weaponId = ReadUInt(work + 0x3C);
                    if (ReadUInt(spec + 0x10) != weaponId)
                    {
                        throw new InvalidDataException("RE8: weapon identity changed.");
                    }

                    if (IsPlayerType(weapon, addresses["GunType"]))
                    {
                        ammo = ReadInt(work + 0x58);
                        if (ammo < 0 || ammo > 10_000_000)
                        {
                            throw new InvalidDataException("RE8: invalid magazine.");
                        }

                        selectedBullet = ReadInt(weapon + 0x184);
                        weaponSpec = Pointer(spec + 0x28);
                        bulletList = weaponSpec == 0 ? 0 : Pointer(weaponSpec + 0x30);
                        var bullets = ReadPlayerList(bulletList, 32);
                        uint bulletId = 0;
                        if (selectedBullet >= 0 && selectedBullet < bullets.Count)
                        {
                            var bullet = Pointer(bullets.Array + 0x20 + (ulong)selectedBullet * 8);
                            if (bullet != 0)
                            {
                                bulletId = ReadUInt(bullet + 0x10);
                            }
                        }
                        else if (bullets.Count != 0)
                        {
                            throw new InvalidDataException("RE8: invalid selected ammunition.");
                        }

                        reserve = 0;
                        if (bulletId != 0)
                        {
                            if (inventory == 0)
                            {
                                throw new InvalidDataException("RE8: inventory is loading.");
                            }

                            inventoryList = Pointer(inventory + 0x18);
                            var items = ReadPlayerList(inventoryList, 2048);
                            inventoryArray = items.Array;
                            inventoryCount = items.Count;
                            long total = 0;
                            for (var index = 0; index < items.Count; index++)
                            {
                                var item = Pointer(items.Array + 0x20 + (ulong)index * 8);
                                if (item == 0)
                                {
                                    throw new InvalidDataException("RE8: null inventory item.");
                                }

                                var itemWork = Pointer(item + 0x58);
                                if (itemWork == 0)
                                {
                                    throw new InvalidDataException("RE8: inventory item is loading.");
                                }

                                if (ReadByte(itemWork + 0x39) != 0 || ReadByte(itemWork + 0x64) != 0 || ReadUInt(itemWork + 0x3C) != bulletId)
                                {
                                    continue;
                                }

                                var count = ReadInt(itemWork + 0x4C);
                                if (count < 0 || count > 10_000_000)
                                {
                                    throw new InvalidDataException("RE8: invalid ammunition stack.");
                                }

                                total += count;
                            }

                            reserve = checked((int)total);
                            if (ReadPlayerList(inventoryList, 2048) != items)
                            {
                                throw new InvalidDataException("RE8: inventory changed during enumeration.");
                            }
                        }

                        infinite = ReadInfiniteItem(weaponId);
                        if (ReadPlayerList(bulletList, 32) != bullets)
                        {
                            throw new InvalidDataException("RE8: ammunition specification changed.");
                        }
                    }
                }
            }

            var identity = string.Join("/", definition, gameObject, configure, reference, status, objectName, pawnName, damage, healthInfo, responser, inventoryManager, inventory, equip, weapon, work, spec, weaponSpec, bulletList, inventoryList, inventoryArray, inventoryCount, selectedBullet);
            return new PlayerState(identity, pawnId, health, maxHealth, healthStatus, weaponId, ammo, reserve, infinite, money);
        }

        private static ulong FindPlayerComponent(ulong gameObject, ulong typeSlot)
        {
            var component = Pointer(gameObject + 0x18);
            var seen = new HashSet<ulong>();
            for (var index = 0; component != 0 && index < 512; index++)
            {
                if (!seen.Add(component) || Pointer(component + 0x10) != gameObject)
                {
                    throw new InvalidDataException("RE8: invalid component chain.");
                }

                if (IsPlayerType(component, typeSlot))
                {
                    return component;
                }

                component = Pointer(component + 0x18);
            }

            if (component != 0)
            {
                throw new InvalidDataException("RE8: component limit exceeded.");
            }

            return 0;
        }

        private static bool IsPlayerType(ulong value, ulong typeSlot)
        {
            if (value == 0)
            {
                return false;
            }

            var wantedType = Pointer(typeSlot);
            var actualType = Pointer(Pointer(value));
            if (wantedType == 0 || actualType < cachedModuleBase || actualType + 0x50 > cachedModuleBase + (ulong)cachedModuleSize)
            {
                throw new InvalidDataException("RE8: invalid managed type.");
            }

            var wanted = Pointer(wantedType + 0x40);
            if (wanted == 0)
            {
                throw new InvalidDataException("RE8: native type is loading.");
            }

            var current = Pointer(actualType + 0x40);
            var seen = new HashSet<ulong>();
            for (var depth = 0; current != 0 && depth < 32; depth++)
            {
                if (!seen.Add(current))
                {
                    throw new InvalidDataException("RE8: cyclic type hierarchy.");
                }

                if (current == wanted)
                {
                    return true;
                }

                current = Pointer(current + 0x38);
            }

            if (current != 0)
            {
                throw new InvalidDataException("RE8: type hierarchy limit exceeded.");
            }

            return false;
        }

        private static (ulong Array, int Count) ReadPlayerList(ulong list, int maximum)
        {
            if (list == 0)
            {
                return (0, 0);
            }

            var array = Pointer(list + 0x10);
            var count = ReadInt(list + 0x18);
            var capacity = array == 0 ? 0 : ReadInt(array + 0x1C);
            if (count < 0 || count > maximum || capacity < count || capacity > maximum)
            {
                throw new InvalidDataException("RE8: invalid managed list.");
            }

            return (array, count);
        }

        private static bool? ReadInfiniteItem(uint itemId)
        {
            if (itemId == 0)
            {
                return false;
            }

            var suppress = Pointer(addresses["InfiniteSuppress"]);
            if (NativeObject(suppress) && ReadByte(suppress + 0x61) != 0)
            {
                return false;
            }

            if (Pointer(addresses["InfiniteOptionsReady"]) == 0)
            {
                return null;
            }

            var options = Pointer(addresses["InfiniteOptions"]);
            if (options == 0)
            {
                return null;
            }

            var optionContainer = Pointer(options + 0x58);
            if (optionContainer == 0)
            {
                return null;
            }

            var optionDictionary = Pointer(optionContainer + 0x18);
            if (optionDictionary == 0)
            {
                return null;
            }

            var optionEntry = InfiniteDictionaryEntry(optionDictionary, ReadUInt(addresses["InfiniteOptionKey"]), 24);
            var optionValue = optionEntry == 0 ? 0 : Pointer(optionEntry + 0x10);
            var enabledValue = ReadUInt(addresses["InfiniteEnabledValue"]);
            if (optionValue == 0 || ReadUInt(optionValue + 0x18) != enabledValue)
            {
                return false;
            }

            if (Pointer(addresses["CPShopReady"]) == 0)
            {
                return null;
            }

            var shop = Pointer(addresses["CPShop"]);
            if (shop == 0)
            {
                return null;
            }

            var groups = Pointer(shop + 0x58);
            if (groups == 0)
            {
                return null;
            }

            uint? unlockId = null;
            foreach (var group in InfiniteReferenceList(groups))
            {
                if (group == 0)
                {
                    throw new InvalidDataException("RE8: missing infinite-ammo group.");
                }

                var entries = Pointer(group + 0x20);
                if (entries == 0)
                {
                    throw new InvalidDataException("RE8: missing infinite-ammo items.");
                }

                foreach (var entry in InfiniteReferenceList(entries))
                {
                    if (entry == 0)
                    {
                        throw new InvalidDataException("RE8: missing infinite-ammo item.");
                    }

                    if (ReadUInt(entry + 0xA8) == itemId || ReadUInt(entry + 0xAC) == itemId)
                    {
                        unlockId = ReadUInt(entry + 0x10);
                        break;
                    }
                }

                if (unlockId.HasValue)
                {
                    break;
                }
            }

            if (!unlockId.HasValue)
            {
                return false;
            }

            var purchase = Pointer(shop + 0x60);
            if (purchase == 0)
            {
                return null;
            }

            var purchases = Pointer(purchase + 0x20);
            if (purchases == 0)
            {
                return null;
            }

            foreach (var entry in InfiniteReferenceList(purchases))
            {
                if (entry == 0)
                {
                    throw new InvalidDataException("RE8: missing purchased item.");
                }

                if (ReadUInt(entry + 0x10) != unlockId.Value)
                {
                    continue;
                }

                if (ReadByte(entry + 0x14) == 0)
                {
                    return false;
                }

                if (ReadByte(entry + 0x15) != 0)
                {
                    return true;
                }

                var required = Pointer(entry + 0x18);
                if (required == 0)
                {
                    return null;
                }

                if (Pointer(addresses["EntitlementReady"]) == 0)
                {
                    return null;
                }

                var entitlement = Pointer(addresses["Entitlement"]);
                if (entitlement == 0)
                {
                    return null;
                }

                var dictionary = Pointer(entitlement + 0x68);
                if (dictionary == 0)
                {
                    return null;
                }

                var(array, count) = InfiniteListHeader(required);
                for (var index = 0; index < count; index++)
                {
                    var id = ReadUInt(array + 0x20 + (ulong)index * 4);
                    var found = InfiniteDictionaryEntry(dictionary, id, 16);
                    if (found != 0 && ReadByte(found + 0xC) != 0)
                    {
                        return true;
                    }
                }

                if (array != Pointer(required + 0x10) || count != ReadInt(required + 0x18))
                {
                    throw new InvalidDataException("RE8: entitlement list changed.");
                }

                return false;
            }

            return false;
        }

        private static (ulong Array, int Count) InfiniteListHeader(ulong list)
        {
            var array = Pointer(list + 0x10);
            var count = ReadInt(list + 0x18);
            var capacity = array == 0 ? 0 : ReadInt(array + 0x1C);
            if (count < 0 || count > capacity || capacity > 4096 || (count != 0 && array == 0))
            {
                throw new InvalidDataException("RE8: invalid infinite-ammo list.");
            }

            return (array, count);
        }

        private static ulong[] InfiniteReferenceList(ulong list)
        {
            var(array, count) = InfiniteListHeader(list);
            var result = new ulong[count];
            for (var index = 0; index < count; index++)
            {
                result[index] = Pointer(array + 0x20 + (ulong)index * 8);
            }

            if (array != Pointer(list + 0x10) || count != ReadInt(list + 0x18))
            {
                throw new InvalidDataException("RE8: infinite-ammo list changed.");
            }

            return result;
        }

        private static ulong InfiniteDictionaryEntry(ulong dictionary, uint key, uint stride)
        {
            var array = Pointer(dictionary + 0x18);
            var count = ReadInt(dictionary + 0x20);
            var capacity = array == 0 ? 0 : ReadInt(array + 0x1C);
            if (count < 0 || count > capacity || capacity > 4096 || (count != 0 && array == 0))
            {
                throw new InvalidDataException("RE8: invalid infinite-ammo dictionary.");
            }

            ulong found = 0;
            for (var index = 0; index < count; index++)
            {
                var entry = array + 0x20 + (ulong)index * stride;
                if (ReadInt(entry) < 0 || ReadUInt(entry + 8) != key)
                {
                    continue;
                }

                if (found != 0)
                {
                    throw new InvalidDataException("RE8: duplicate infinite-ammo option.");
                }

                found = entry;
            }

            if (array != Pointer(dictionary + 0x18) || count != ReadInt(dictionary + 0x20))
            {
                throw new InvalidDataException("RE8: infinite-ammo dictionary changed.");
            }

            return found;
        }

        private static ulong FindPlayerDefinition()
        {
            var cached = Pointer(addresses["Player"]);
            if (NativeObject(cached))
            {
                return cached;
            }

            if (Pointer(addresses["PlayerObjectsReady"]) == 0)
            {
                return 0;
            }

            var manager = Pointer(addresses["PlayerObjects"]);
            if (!NativeObject(manager))
            {
                return 0;
            }

            var dictionary = Pointer(manager + 0x60);
            if (dictionary == 0)
            {
                return 0;
            }

            var entries = Pointer(dictionary + 0x18);
            var count = ReadInt(dictionary + 0x20);
            var capacity = entries == 0 ? 0 : ReadInt(entries + 0x1C);
            if (count < 0 || count > capacity || capacity > 32768)
            {
                throw new InvalidDataException("RE8: invalid player object dictionary.");
            }

            uint[] hashes = [0x70F86523, 0x460AEF10, 0xF1B30552, 0x98E4C0AF, 0xC7022E5E, 0xC4FB9F76];
            var lists = new ulong[hashes.Length];
            var foundEntries = new ulong[hashes.Length];
            for (var index = 0; index < count; index++)
            {
                var entry = entries + 0x20 + (ulong)index * 24;
                if (ReadInt(entry) < 0)
                {
                    continue;
                }

                var key = ReadUInt(entry + 8);
                var priority = Array.IndexOf(hashes, key);
                if (priority < 0)
                {
                    continue;
                }

                if (foundEntries[priority] != 0)
                {
                    throw new InvalidDataException("RE8: duplicate player key.");
                }

                foundEntries[priority] = entry;
                lists[priority] = Pointer(entry + 0x10);
            }

            ulong result = 0;
            for (var priority = 0; priority < hashes.Length; priority++)
            {
                var list = lists[priority];
                if (list == 0)
                {
                    continue;
                }

                var header = ReadPlayerList(list, 1024);
                if (header.Count == 0)
                {
                    continue;
                }

                var candidate = Pointer(header.Array + 0x20);
                if (!NativeObject(candidate))
                {
                    continue;
                }

                var objectName = Pointer(candidate + 0x50);
                if (objectName == 0 || ReadUInt(objectName + 0x28) != hashes[priority])
                {
                    throw new InvalidDataException("RE8: player dictionary identity changed.");
                }

                if (ReadPlayerList(list, 1024) != header || Pointer(header.Array + 0x20) != candidate)
                {
                    throw new InvalidDataException("RE8: player object list changed.");
                }

                result = candidate;
                break;
            }

            if (Pointer(addresses["PlayerObjects"]) != manager || Pointer(manager + 0x60) != dictionary || Pointer(dictionary + 0x18) != entries || ReadInt(dictionary + 0x20) != count)
            {
                throw new InvalidDataException("RE8: player registry changed.");
            }

            for (var priority = 0; priority < hashes.Length; priority++)
            {
                if (foundEntries[priority] != 0 && (ReadInt(foundEntries[priority]) < 0 || ReadUInt(foundEntries[priority] + 8) != hashes[priority] || Pointer(foundEntries[priority] + 0x10) != lists[priority]))
                {
                    throw new InvalidDataException("RE8: player registry entry changed.");
                }
            }

            return result;
        }

        private static int ReadInt(ulong address)
        {
            return Hypervisor.Read<int>(address, true);
        }

        private static uint ReadUInt(ulong address)
        {
            return Hypervisor.Read<uint>(address, true);
        }

        private static ulong Pointer(ulong address)
        {
            return Hypervisor.Read<ulong>(address, true);
        }

        private static byte ReadByte(ulong address)
        {
            return Hypervisor.Read<byte>(address, true);
        }

        private static float ReadFloat(ulong address)
        {
            return Hypervisor.Read<float>(address, true);
        }

        private static bool NativeObject(ulong address)
        {
            return address != 0 && Pointer(address + 0x10) != 0;
        }

        private static string ReadString(ulong address)
        {
            if (address == 0)
            {
                return "";
            }

            var length = ReadInt(address + 0x10);
            if (length < 0 || length > 256)
            {
                throw new InvalidDataException("RE8: invalid managed string.");
            }

            return System.Text.Encoding.Unicode.GetString(Hypervisor.ReadArray(address + 0x14, length * 2, true));
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
            var process = Hypervisor.Process ?? throw new InvalidDataException("RE8: game is not attached.");
            var module = process.MainModule ?? throw new InvalidDataException("RE8: module not loaded.");
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
                throw new InvalidDataException("RE8: waiting to retry patterns.");
            }

            nextScan = DateTime.UtcNow.AddSeconds(30);
            var found = new Dictionary<string, ulong>();
            foreach (var(name, pattern, operand, length)in Patterns)
            {
                var match = FindUniquePattern(moduleBase, module.ModuleMemorySize, pattern);
                var target = length == 0 ? match : checked((ulong)((long)match + length + ReadInt(match + (ulong)operand)));
                if (target < moduleBase || target + 8 > moduleBase + (ulong)module.ModuleMemorySize)
                {
                    throw new InvalidDataException("RE8: pattern target is outside the module.");
                }

                found.Add(name, target);
            }

            addresses = found;
        }

        private static ulong FindUniquePattern(ulong moduleBase, int moduleSize, string signature)
        {
            if (moduleSize < 64)
            {
                throw new InvalidDataException("RE8: invalid module size.");
            }

            var peOffset = Hypervisor.Read<uint>(moduleBase + 0x3C, true);
            if (peOffset > (uint)moduleSize - 24)
            {
                throw new InvalidDataException("RE8: invalid PE offset.");
            }

            var pe = moduleBase + peOffset;
            if (peOffset > (uint)moduleSize - 24 || Hypervisor.Read<uint>(pe, true) != 0x4550 || Hypervisor.Read<ushort>(pe + 4, true) != 0x8664)
            {
                throw new InvalidDataException("RE8: invalid PE header.");
            }

            var sectionCount = Hypervisor.Read<ushort>(pe + 6, true);
            var optionalSize = Hypervisor.Read<ushort>(pe + 20, true);
            var sectionTable = pe + 24 + optionalSize;
            if (sectionTable + (ulong)sectionCount * 40 > moduleBase + (ulong)moduleSize)
            {
                throw new InvalidDataException("RE8: invalid section table.");
            }

            var pattern = SignaturePattern.Parse(signature);
            var prefix = pattern.Bytes.TakeWhile(value => value.HasValue).Select(value => value.GetValueOrDefault()).ToArray();
            if (prefix.Length == 0)
            {
                throw new InvalidDataException("RE8: pattern needs a fixed prefix.");
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
                    throw new InvalidDataException("RE8: invalid executable section.");
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
                            throw new InvalidDataException("RE8: manager pattern is ambiguous.");
                        }

                        match = address;
                    }
                }
            }

            return match != 0 ? match : throw new InvalidDataException("RE8: manager pattern was not found.");
        }

        private static void InitializeDiscord()
        {
            if (discord == null)
            {
                throw new InvalidOperationException("RE8: presence is not initialized.");
            }

            discord.Initialize();
        }
    }
}
