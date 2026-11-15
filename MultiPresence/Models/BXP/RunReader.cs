namespace MultiPresence.Models.BXP
{
    internal static partial class MemoryReader
    {
        private static GameSnapshot ReadRun(GameSnapshot snapshot, ulong battle)
        {
            if (battle == 0 || ClassName(battle) != "BattleSaveData") return new GameSnapshot { State = "Loading" };
            ulong character = Ptr(battle + 0x10);
            if (character == 0 || ClassName(character) != "CharBattleInst") return new GameSnapshot { State = "Loading" };

            int stageId = Value<int>(battle + 0x18);
            var stage = stageId >= 0 && stageId < Stages.Length ? Stages[stageId] : default;
            int difficulty = Value<int>(battle + 0x1C);
            int upgradeLevel = Value<int>(battle + 0x98);
            int kills = Value<int>(battle + 0xF0);
            float health = Value<float>(battle + 0x90);
            ulong upgrades = Singleton("UpgradeMgr");
            int maxHealth = upgrades == 0 ? 0 : Value<int>(upgrades + 0xD8);
            string name = ReadRunCharacters(character);
            int? gold = ReadRunGold(battle);
            if (Ptr(battle + 0x10) != character || Value<int>(battle + 0x18) != stageId) return new GameSnapshot { State = "Loading" };

            return snapshot with
            {
                Stage = stage.Name ?? "",
                Location = stage.Name ?? "",
                Layer = stage.Layer,
                Character = name,
                Level = upgradeLevel is >= 0 and < int.MaxValue ? upgradeLevel + 1 : null,
                Speed = difficulty switch
                {
                    1 => "Normal", 2 => "Fast", 3 => "Fast+", 4 => "Fast++",
                    >= 5 => $"Fast+{difficulty - 2}", _ => ""
                },
                Health = health >= 0 && health < int.MaxValue ? (int)MathF.Ceiling(health) : null,
                MaxHealth = maxHealth > 0 ? maxHealth : null,
                Kills = kills >= 0 ? kills : null,
                RunGold = gold
            };
        }

        private static int? ReadRunGold(ulong battle)
        {
            ulong cost = Ptr(battle + 0xB8);
            if (cost == 0 || ClassName(cost) != "Cost") return null;
            ulong amounts = Ptr(cost + 0x10);
            if (amounts == 0 || ClassName(amounts) != "Int32[]" || Value<ulong>(amounts + 0x18) == 0) return null;
            int gold = Value<int>(amounts + 0x20);
            return gold >= 0 && Ptr(cost + 0x10) == amounts && Ptr(battle + 0xB8) == cost ? gold : null;
        }

        private static string ReadRunCharacters(ulong character)
        {
            int primary = Value<int>(character + 0x10);
            string name = CharacterName(primary);
            ulong combined = Ptr(character + 0x18);
            if (combined == 0 || name.Length == 0) return name;
            int count = Value<int>(combined + 0x18);
            if (count < 1 || count > Characters.Length) return name;
            ulong items = Ptr(combined + 0x10);
            if (items == 0 || Value<ulong>(items + 0x18) < (ulong)count) return name;
            int version = Value<int>(combined + 0x1C);
            var names = new List<string> { name };
            foreach (int id in Hypervisor.Read<int>(items + 0x20, count, true))
            {
                string partner = CharacterName(id);
                if (partner.Length > 0 && !names.Contains(partner)) names.Add(partner);
            }
            return Ptr(character + 0x18) == combined && Value<int>(combined + 0x1C) == version ? string.Join(" & ", names) : name;
        }

        private static string CharacterName(int id) => id >= 0 && id < Characters.Length ? Characters[id] : "";

        private static readonly (string Name, int? Layer)[] Stages =
        [
            ("THE BONExYARD", 1), ("THE SNOWYxSHORES", 2), ("THE GORYxGRASSLANDS", 5),
            ("THE SMOLDERINGxDEPTHS", 6), ("THE HEAVENLYxGATES", 7), ("THE VASTxVOID", 8),
            ("THE FUNGALxFOREST", 4), ("THE LIMINALxDESERT", 3), ("THE FLOODEDxSEWERS", 1),
            ("THE SHADOWEDxALLEYS", 2), ("THE CASTLExRAMPARTS", 3), ("THE OUTSTRETCHEDxSPIRE", 4)
        ];

        private static readonly string[] Characters =
        [
            "The Warrior", "The Repentant", "The Itchy Finger", "The Tunneller", "The Tiptoer",
            "The Cogitator", "The Tactician", "The Spendthrift", "The Embedded", "The Radical",
            "The Empty Nester", "The Shade", "The Cohabitants", "The Physicist", "The Shieldbearer",
            "The Makeshift Sisyphus", "The Flagellant", "The Juggler", "The Hoary Hoarder", "The Falconer",
            "The Carouser", "The Ballbearer", "The False Messiah", "The Sentinel", "The Wallbreaker",
            "The Recollector", "The Scandalmonger", "The Scrupulous Scoundrel", "The Potager", "The Right Hand Man", "The Conduit"
        ];
    }
}
