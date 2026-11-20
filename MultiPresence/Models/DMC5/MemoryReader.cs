namespace MultiPresence.Models.DMC5
{
    internal static partial class MemoryReader
    {
        private const string MissionPattern = "48 8B 05 ?? ?? ?? ?? 48 85 C0 0F 84 ?? ?? ?? ?? 8B 80 80 00 00 00 48 8D 54 24 58 89 44 24 58 83 F8 0A";
        private const string DifficultyPattern = "48 8B 05 ?? ?? ?? ?? 48 85 C0 74 ?? 8B 80 88 00 00 00 EB ?? 3B 47 1C";
        private const string CharacterPattern = "48 8B 05 ?? ?? ?? ?? 83 FA 03 7C ?? 48 85 C0 74 ?? C7 80 BC 00 00 00 04 00 00 00";
        private const string ModePattern = "48 8B 05 ?? ?? ?? ?? 48 85 C0 74 ?? 83 B8 90 00 00 00 01 0F 85";
        private const string PausePattern = "48 8B 05 ?? ?? ?? ?? 48 85 C0 74 ?? 83 B8 24 01 00 00 01 0F 94 C0 84 C0 75";
        private const string FlowPattern = "48 8B 05 ?? ?? ?? ?? 48 85 C0 74 ?? 83 78 54 0F 75 ?? 83 B8 8C 00 00 00 0F 74";
        private const string OrbsPattern = "48 8B 05 ?? ?? ?? ?? 48 85 C0 0F 84 ?? ?? ?? ?? 48 8B 40 58 48 85 C0 75 ?? 45 33 C0 8D 50 38 48 8B CB E8 ?? ?? ?? ?? EB ?? 83 78 1C 04 77 ?? 45 33 C0 48 8B CB 41 8D 50 37 E8 ?? ?? ?? ?? EB ?? 48 8B 50 40 48 85 D2 74 ?? 41 B8 01 00 00 00";
        private const string PlayerPattern = "48 8B 05 ?? ?? ?? ?? 48 85 C0 74 ?? 48 8B 40 60 48 85 C0 74 ?? 48 8B 40 10 33 FF";
        private const uint GameManagerType = 0x43FEF87B;
        private const uint FlowManagerType = 0x42792D5E;
        private const uint PlayerManagerType = 0xE4F35BBC;
        private const uint InventoryManagerType = 0xFD8D2D35;
        private static readonly uint[] HealthChain = [0x1F8, 0x260, 0x10, 0x20];
        private static ulong gameSlot, flowSlot, orbSlot, playerSlot;
        private static int missionOffset, difficultyOffset, characterOffset, modeOffset, pauseOffset, nowFlowOffset, nextFlowOffset, manualPlayerOffset, processId;
        private static DateTime nextScan;

        // app.FlowManager.FlowID
        private enum Flow
        {
            Demo = 12,
            Title = 13,
            MainMenu = 14,
            Episode = 15,
            Player = 16,
            Start = 17,
            Shop = 20,
            Custom = 21,
            Game = 22,
            Result = 23,
            Weapon = 24,
            Reward = 25,
            ThankYou = 26
        }

        public static void Reset()
        {
            gameSlot = flowSlot = orbSlot = playerSlot = 0;
            missionOffset = difficultyOffset = characterOffset = modeOffset = pauseOffset = 0;
            nowFlowOffset = nextFlowOffset = manualPlayerOffset = processId = 0;
            nextScan = DateTime.MinValue;
        }

        public static GameSnapshot ReadSnapshot()
        {
            ResolveAddresses();
            ulong flowManager = Pointer(flowSlot);
            ulong game = Pointer(gameSlot);
            if (flowManager == 0 || game == 0) return new GameSnapshot { State = "Loading" };
            if (!ManagedTypeIs(flowManager, FlowManagerType)) throw Invalid("flow manager");
            if (!ManagedTypeIs(game, GameManagerType)) throw Invalid("game manager");

            int flow = Hypervisor.Read<int>(flowManager + (ulong)nowFlowOffset, true);
            if (Hypervisor.Read<int>(flowManager + (ulong)nextFlowOffset, true) != flow) return new GameSnapshot { State = "Loading" };
            string state;
            switch ((Flow)flow)
            {
                case Flow.Demo or Flow.Title or Flow.MainMenu or Flow.Episode:
                    return new GameSnapshot { State = "Main Menu" };
                case Flow.Player or Flow.Start or Flow.Shop or Flow.Custom:
                    state = "Mission Menu";
                    break;
                case Flow.Result or Flow.Weapon or Flow.Reward or Flow.ThankYou:
                    state = "Mission End";
                    break;
                case Flow.Game:
                    state = "Default";
                    break;
                default:
                    return new GameSnapshot { State = "Loading" };
            }

            int mission = Hypervisor.Read<int>(game + (ulong)missionOffset, true);
            int difficulty = Hypervisor.Read<int>(game + (ulong)difficultyOffset, true);
            int character = Hypervisor.Read<int>(game + (ulong)characterOffset, true);
            int mode = Hypervisor.Read<int>(game + (ulong)modeOffset, true);
            if (mission is < 0 or > 99 || difficulty is < 0 or > 7 || character is < 0 or > 4 || mode is < 0 or > 5) return new GameSnapshot { State = "Loading" };

            int? health = null, maxHealth = null;
            if (state == "Default")
            {
                ulong players = Pointer(playerSlot);
                if (players == 0) return new GameSnapshot { State = "Loading" };
                if (!ManagedTypeIs(players, PlayerManagerType)) throw Invalid("player manager");
                var (current, max) = ReadHealth(Pointer(players + (ulong)manualPlayerOffset));
                if (max == 0) return new GameSnapshot { State = "Loading" };
                health = current;
                maxHealth = max;
                // app.GameManager.Mode: Story, SecretMission, Ranking, BloodyPalace, Training, Gallery; PauseRequest 1 = MenuPause.
                state = mode switch
                {
                    3 => "Bloody Palace",
                    4 => "The Void",
                    _ => Hypervisor.Read<int>(game + (ulong)pauseOffset, true) == 1 ? "Pause Menu" : "Default"
                };
            }

            int? red = null, blue = null, purple = null, gold = null;
            ulong inventory = Pointer(orbSlot);
            if (inventory != 0)
            {
                if (!ManagedTypeIs(inventory, InventoryManagerType)) throw Invalid("inventory manager");
                // app.InventoryManager: RedOrbGetPoint +0x60, BlueOrbPieceGetNum +0x78, PurpleOrbPieceGetNum +0x88, GoldOrbGetNum +0x98.
                var counters = Hypervisor.ReadArray(inventory + 0x60, 0x3C, true);
                red = BitConverter.ToInt32(counters, 0);
                blue = BitConverter.ToInt32(counters, 0x18);
                purple = BitConverter.ToInt32(counters, 0x28);
                gold = BitConverter.ToInt32(counters, 0x38);
                if (red < 0 || blue < 0 || purple < 0 || gold < 0) return new GameSnapshot { State = "Loading" };
            }

            bool story = mode is 0 or 2 or 5;
            var snapshot = new GameSnapshot
            {
                State = state,
                Mode = mode switch
                {
                    1 => "Secret Mission",
                    3 => "Bloody Palace",
                    4 => "The Void",
                    _ => "Story"
                },
                Mission = story ? mission : null,
                MissionTitle = story ? Missions.Title(mission) : "",
                Difficulty = difficulty switch
                {
                    0 => "Human",
                    1 => "Devil Hunter",
                    2 => "Son of Sparda",
                    3 => "Dante Must Die",
                    4 => "Heaven or Hell",
                    5 => "Hell and Hell",
                    7 => "Legendary Dark Knight",
                    _ => ""
                },
                Character = character switch
                {
                    0 => "Nero",
                    1 => "Dante",
                    2 => "V",
                    3 or 4 => "Vergil",
                    _ => ""
                },
                Health = health,
                MaxHealth = maxHealth,
                RedOrbs = red,
                BlueOrbs = blue,
                PurpleOrbs = purple,
                GoldOrbs = gold
            };
            if (Pointer(flowSlot) != flowManager || Pointer(gameSlot) != game || Pointer(orbSlot) != inventory ||
                Hypervisor.Read<int>(flowManager + (ulong)nowFlowOffset, true) != flow ||
                Hypervisor.Read<int>(game + (ulong)missionOffset, true) != mission)
            {
                return new GameSnapshot { State = "Loading" };
            }
            return snapshot;
        }

        private static (int Health, int MaxHealth) ReadHealth(ulong player)
        {
            ulong current = player;
            foreach (uint offset in HealthChain)
            {
                if (current == 0 || !Hypervisor.TryRead<ulong>(current + offset, out current, true)) return (0, 0);
            }
            if (current == 0 || !Hypervisor.TryRead<float>(current + 0x10, out var health, true) ||
                !Hypervisor.TryRead<float>(current + 0x14, out var maxHealth, true) || !float.IsFinite(health) ||
                !float.IsFinite(maxHealth) || maxHealth is < 1 or > 1000000 || health < 0 || health > maxHealth)
            {
                return (0, 0);
            }
            return ((int)MathF.Ceiling(health), (int)MathF.Ceiling(maxHealth));
        }

        private static bool ManagedTypeIs(ulong instance, uint expectedHash)
        {
            ulong info = Pointer(instance);
            ulong type = info == 0 ? 0 : Pointer(info);
            return type != 0 && Hypervisor.Read<uint>(type + 8, true) == expectedHash;
        }

        private static ulong Pointer(ulong address) => Hypervisor.Read<ulong>(address, true);

        private static InvalidDataException Invalid(string field) => new("Devil May Cry 5: invalid " + field + ".");
    }
}
