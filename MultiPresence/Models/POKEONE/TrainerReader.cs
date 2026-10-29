using System.Globalization;

namespace MultiPresence.Models.POKEONE
{
    internal static partial class MemoryReader
    {
        private static readonly Dictionary<string, PatternDefinition> TrainerPatterns = new Dictionary<string, PatternDefinition>
        {
            { "InventoryHandler", new PatternDefinition("A1 ?? ?? ?? ?? 83 C4 10 8B 40 5C 8B 00 85 C0 0F 84 ?? ?? ?? ?? 6A 00 FF 76 18 50 E8 ?? ?? ?? ?? A1 ?? ?? ?? ?? 83 C4 0C", 1) },
            { "PokedexHandler", new PatternDefinition("A1 ?? ?? ?? ?? 83 C4 0C 8B 40 5C 8B 00 85 C0 0F 84 ?? ?? ?? ?? 6A 00 FF 76 1C 50 E8 ?? ?? ?? ?? 8B 4E 28 83 C4 0C 85 C9", 1) }
        };

        private static readonly Dictionary<int, string> BadgeNames = new Dictionary<int, string>
        {
            { 1, "Boulder Badge" },
            { 2, "Cascade Badge" },
            { 3, "Thunder Badge" },
            { 4, "Rainbow Badge" },
            { 5, "Soul Badge" },
            { 6, "Marsh Badge" },
            { 7, "Volcano Badge" },
            { 8, "Earth Badge" },
            { 9, "Zephyr Badge" },
            { 10, "Hive Badge" },
            { 11, "Plain Badge" },
            { 12, "Fog Badge" },
            { 13, "Storm Badge" },
            { 14, "Mineral Badge" },
            { 15, "Glacier Badge" },
            { 16, "Rising Badge" },
            { 17, "Stone Badge" },
            { 18, "Knuckle Badge" },
            { 19, "Dynamo Badge" },
            { 20, "Heat Badge" },
            { 21, "Balance Badge" },
            { 22, "Feather Badge" },
            { 23, "Mind Badge" },
            { 24, "Rain Badge" },
            { 25, "Coal Badge" },
            { 26, "Forest Badge" },
            { 27, "Cobble Badge" },
            { 28, "Fen Badge" },
            { 29, "Relic Badge" },
            { 30, "Mine Badge" },
            { 31, "Icicle Badge" },
            { 32, "Beacon Badge" }
        };

        private static TrainerSnapshot ReadTrainer(FrameRead m, ulong player)
        {
            var playerName = m.String(m.Pointer(player + 0x2C));
            var experience = ReadTrainerPart(player, playerName, ReadTrainerExperience);
            var inventory = ReadTrainerPart(player, playerName, ReadTrainerInventory);
            var badges = ReadTrainerPart(player, playerName, ReadTrainerBadges);
            var pokedex = ReadTrainerPart(player, playerName, ReadTrainerPokedex);
            return new TrainerSnapshot()
            {
                Experience = experience.Experience,
                ExpRequired = experience.ExpRequired,
                ExpPercent = experience.ExpPercent,
                Money = inventory.Money,
                Tokens = inventory.Tokens,
                GameCoins = inventory.GameCoins,
                BattlePoints = inventory.BattlePoints,
                BadgeCount = badges.BadgeCount,
                Badges = badges.Badges,
                PokedexSeen = pokedex.PokedexSeen,
                PokedexCaught = pokedex.PokedexCaught
            };
        }

        private static TrainerSnapshot ReadTrainerPart(ulong player, string playerName, Func<FrameRead, ulong, TrainerSnapshot> read)
        {
            try
            {
                var memory = new FrameRead();
                var currentPlayer = Singleton(memory, "PlayerHandler", 4, ("Username", 0x2C), ("LoggedIn", 0x30));
                if (currentPlayer != player || !memory.Bool(player + 0x30) || memory.String(memory.Pointer(player + 0x2C)) != playerName)
                {
                    return new TrainerSnapshot();
                }

                var snapshot = read(memory, player);
                memory.Validate();
                return snapshot;
            }
            catch (Exception exception) when (exception is InvalidDataException or IOException or InvalidOperationException or ArgumentException or OverflowException)
            {
                return new TrainerSnapshot();
            }
        }

        private static TrainerSnapshot ReadTrainerExperience(FrameRead m, ulong player)
        {
            m.Object(player, "PlayerHandler", "", ("Level", 0x48));
            var level = m.RequiredPointer(player + 0x48);
            m.Object(level, "Level", "PSXAPI.Response", ("<TotalExp>k__BackingField", 0xC),
                ("<StartExp>k__BackingField", 0x10), ("<RequiredExp>k__BackingField", 0x14));
            var total = TrainerUInt(m, level + 0xC);
            var start = TrainerUInt(m, level + 0x10);
            var required = TrainerUInt(m, level + 0x14);
            if (total < start || required < total)
            {
                throw Invalid("trainer experience");
            }

            var experience = total - start;
            var expRequired = required - start;
            return new TrainerSnapshot()
            {
                Experience = experience,
                ExpRequired = expRequired,
                ExpPercent = expRequired == 0 ? null : Math.Round(100m * experience / expRequired, 2)
            };
        }

        private static TrainerSnapshot ReadTrainerInventory(FrameRead m, ulong player)
        {
            var inventory = OptionalTrainerSingleton(m, "InventoryHandler", ("Money", 0x24), ("Tokens", 0x28),
                ("GameCoins", 0x48), ("BattlePoints", 0x4C), ("MoneyLabel", 0x14), ("GoldLabel", 0x18));
            if (inventory == 0)
            {
                return new TrainerSnapshot();
            }

            var money = TrainerUInt(m, inventory + 0x24);
            var tokens = TrainerUInt(m, inventory + 0x28);
            if (TrainerLabelNumber(m, m.RequiredPointer(inventory + 0x14)) != money ||
                TrainerLabelNumber(m, m.RequiredPointer(inventory + 0x18)) != tokens)
            {
                throw Invalid("inventory currency labels");
            }

            return new TrainerSnapshot()
            {
                Money = money,
                Tokens = tokens,
                GameCoins = TrainerUInt(m, inventory + 0x48),
                BattlePoints = TrainerUInt(m, inventory + 0x4C)
            };
        }

        private static TrainerSnapshot ReadTrainerBadges(FrameRead m, ulong player)
        {
            if (ReadTrainerInventory(m, player).Money == null)
            {
                return new TrainerSnapshot();
            }

            var inventory = OptionalTrainerSingleton(m, "InventoryHandler", ("Badges", 0x40));
            if (inventory == 0)
            {
                return new TrainerSnapshot();
            }

            var array = m.Pointer(inventory + 0x40);
            if (array == 0)
            {
                return new TrainerSnapshot()
                {
                    BadgeCount = 0
                };
            }

            var count = m.Int(array + 12);
            if (count is < 0 or > 64)
            {
                throw Invalid("badge list size");
            }

            var ids = new HashSet<int>();
            var bytes = m.Bytes(array + 16, count * 4);
            for (var index = 0; index < count; index++)
            {
                var id = BitConverter.ToInt32(bytes, index * 4);
                if (id <= 0)
                {
                    continue;
                }

                if (!BadgeNames.ContainsKey(id))
                {
                    throw Invalid("badge identity");
                }

                ids.Add(id);
            }

            return new TrainerSnapshot()
            {
                BadgeCount = ids.Count,
                Badges = string.Join(", ", ids.Order().Select(id => BadgeNames[id]))
            };
        }

        private static TrainerSnapshot ReadTrainerPokedex(FrameRead m, ulong player)
        {
            var pokedex = OptionalTrainerSingleton(m, "PokedexHandler", ("SeenLabel", 0x38), ("CaughtLabel", 0x3C));
            if (pokedex == 0)
            {
                return new TrainerSnapshot();
            }

            var seen = TrainerLabelNumber(m, m.RequiredPointer(pokedex + 0x38));
            var caught = TrainerLabelNumber(m, m.RequiredPointer(pokedex + 0x3C));
            if (seen > 10000 || caught > seen)
            {
                throw Invalid("Pokedex counters");
            }

            return new TrainerSnapshot()
            {
                PokedexSeen = (int)seen,
                PokedexCaught = (int)caught
            };
        }

        private static ulong OptionalTrainerSingleton(FrameRead m, string name, params (string Name, int Offset)[] fields)
        {
            return addresses.TryGetValue(name, out _) ? Singleton(m, name, 0, fields) : 0;
        }

        private static uint TrainerUInt(FrameRead m, ulong address)
        {
            return BitConverter.ToUInt32(m.Bytes(address, 4));
        }

        private static uint TrainerLabelNumber(FrameRead m, ulong label)
        {
            m.Object(label, "UILabel", "", ("mText", 0x130));
            m.RequiredPointer(label + 8);
            var text = m.String(m.Pointer(label + 0x130));
            if (text.Length == 0 || text.Any(character => character is not (>= '0' and <= '9') and not '.' and not ',' and not ' ' and not '\u00A0' and not '\u202F'))
            {
                throw Invalid("trainer counter text");
            }

            var digits = new string(text.Where(character => character is >= '0' and <= '9').ToArray());
            return uint.TryParse(digits, NumberStyles.None, CultureInfo.InvariantCulture, out var value)
                ? value : throw Invalid("trainer counter value");
        }
    }
}
