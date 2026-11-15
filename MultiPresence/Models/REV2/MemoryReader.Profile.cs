using System.Numerics;

namespace MultiPresence.Models.REV2
{
    internal static partial class MemoryReader
    {
        private const string RaidPlayerPattern = "8B 0D ?? ?? ?? ?? 6A 00 E8 ?? ?? ?? ?? 50 E8 ?? ?? ?? ?? 8B 40 08 83 C4 04 8B CE 85 C0";
        private const string PlayerFieldsPattern = "0F B6 85 ?? ?? ?? ?? 50 8D 89 ?? ?? ?? ?? 89 6C 24 18 E8 ?? ?? ?? ?? 0F BE 8D ?? ?? ?? ?? 51 8B CF";
        private const string MoneyPattern = "8B 15 ?? ?? ?? ?? 8B 82 ?? ?? ?? ?? 8B 92 ?? ?? ?? ?? C3";
        private const string HeartFieldsPattern = "8A 46 14 8B 0D ?? ?? ?? ?? 84 C0 74 ?? 8B 91 ?? ?? ?? ?? EB ?? 8B 56 10 84 C0 74 ?? 8B 81 ?? ?? ?? ?? EB ?? 8B 46 0C";
        private const string HeartInventoryPattern = "8B 44 24 04 F3 0F 7E 81 ?? ?? ?? ?? 8B 89 ?? ?? ?? ?? 66 0F D6 00 89 48 08 C2 04 00";
        private const string BadgeTablePattern = "85 C0 78 ?? 3D ?? ?? ?? ?? 73 ?? 8B 54 24 08 83 FA ?? 73 ?? 6B C0 ?? 03 C2 8A 84 08 ?? ?? ?? ?? C2 08 00";
        private const string BadgeSavePattern = "8B 0D ?? ?? ?? ?? 57 50 8D 49 ?? E8 ?? ?? ?? ?? 5F 24 7F 5E C2 04 00";

        private const int StandardGauntletMissions = 8 * 4 * 6;
        private const int FirstOmegaMission = StandardGauntletMissions + 2;
        private const int LobbyMissionSlots = StandardGauntletMissions + 4 + 3 * 4;

        private sealed record ProfileAddresses(uint Raid, uint Save, uint Character, uint Level, uint Money,
            uint BlueHearts, uint RedHearts, uint HeartInventory, BadgeTable? Badges);
        private sealed record BadgeTable(uint Offset, int Characters);

        private static ProfileAddresses ResolveProfile(GameCode code)
        {
            int raid = code.Find(RaidPlayerPattern);
            int fields = code.Find(PlayerFieldsPattern);
            int money = code.Find(MoneyPattern);
            int hearts = code.Find(HeartFieldsPattern);
            int inventory = code.Find(HeartInventoryPattern);
            uint save = code.Global(money, 2);
            uint moneyOffset = code.Operand(money, 8);
            uint heartInventory = code.Operand(inventory, 8);
            if (code.Global(hearts, 5) != save || code.Operand(money, 14) != moneyOffset + 4 ||
                code.Operand(inventory, 14) != heartInventory + 8)
                throw new InvalidDataException("Inconsistent Revelations 2 profile fields.");

            return new ProfileAddresses(code.Global(raid, 2), save, code.Operand(fields, 26), code.Operand(fields, 3),
                moneyOffset, code.Operand(hearts, 15), code.Operand(hearts, 30), heartInventory,
                ResolveBadges(code, save, moneyOffset));
        }

        private static BadgeTable? ResolveBadges(GameCode code, uint save, uint moneyOffset)
        {
            int table = code.FindOptional(BadgeTablePattern);
            int caller = code.FindOptional(BadgeSavePattern);
            if (table < 0 || caller < 0 || code.Operand(caller, 2) != save ||
                caller + 16 + BitConverter.ToInt32(code.Bytes, caller + 12) != table - 4)
                return null;

            uint missions = code.Operand(table, 5);
            int characters = code.Bytes[table + 17];
            ulong offset = code.Bytes[caller + 10] + (ulong)code.Operand(table, 28);
            if (missions is < LobbyMissionSlots or > 1024 || characters is 0 or > 64 || code.Bytes[table + 22] != characters ||
                offset + missions * (uint)characters > moneyOffset)
                return null;

            return new BadgeTable((uint)offset, characters);
        }

        private static int ReadBadges(ulong save, BadgeTable table)
        {
            byte[] awards = Hypervisor.ReadArray(save + table.Offset, LobbyMissionSlots * table.Characters, true);
            int total = 0;
            for (int mission = 0; mission < LobbyMissionSlots; mission++)
            {
                // Omega has missions on Very Hard and Code Red only.
                if (mission >= StandardGauntletMissions && mission < FirstOmegaMission) continue;
                int start = mission * table.Characters;
                int badges = 0;
                for (int character = 0; character < table.Characters; character++)
                    badges |= awards[start + character];
                total += BitOperations.PopCount((uint)(badges & 31));
            }
            return total;
        }

        private static GameSnapshot ReadProfile(ProfileAddresses found, GameSnapshot snapshot)
        {
            ulong raid = Hypervisor.Read<uint>(found.Raid, true);
            ulong save = Hypervisor.Read<uint>(found.Save, true);
            if (raid == 0 || save == 0) return new GameSnapshot { State = "Loading" };

            // The lobby reads either the saved life crystals or the active inventory copy.
            ulong inventory = save + found.HeartInventory;
            bool useSave = Hypervisor.Read<byte>(inventory + 8, true) != 0;
            ulong red = useSave ? save + found.RedHearts : inventory;
            ulong blue = useSave ? save + found.BlueHearts : inventory + 4;
            return snapshot with
            {
                CharacterId = Hypervisor.Read<byte>(raid + found.Character, true),
                Level = Hypervisor.Read<byte>(raid + found.Level, true),
                Money = Hypervisor.Read<long>(save + found.Money, true),
                RedHearts = Hypervisor.Read<int>(red, true),
                BlueHearts = Hypervisor.Read<int>(blue, true),
                Badges = found.Badges == null ? null : ReadBadges(save, found.Badges)
            };
        }
    }
}
