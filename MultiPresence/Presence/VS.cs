using DiscordRPC;
using MultiPresence.Models.VS;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class VS
    {
        static string process = "VampireSurvivors";
        private static DiscordRpcClient? discord;
        private static DiscordStatusUpdater? updater;
        public static string adventure = "";

        public static void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("1283783524423438409");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/config/Vampire Survivors.json");
            Thread thread = new Thread(RPC);
            thread.Start();
        }

        private static void GetPID()
        {
            try
            {
                var _myProcess = Process.GetProcessesByName("VampireSurvivors")[0];
                if (_myProcess.Id > 0)
                    Hypervisor.AttachProcess(_myProcess);
            }
            catch
            {
                //nothing?
            }
        }

        private static async void RPC()
        {
            Process[] game = Process.GetProcessesByName(process);
            if (game.Length > 0)
            {
                var characterid_base = Hypervisor.GetPointer64(0x050B7678, [0x408, 0x40, 0x50, 0x44], false, "GameAssembly.dll");
                var characterid_adventure_base = Hypervisor.GetPointer64(0x050B7678, [0x408, 0x40, 0x60, 0x44], false, "GameAssembly.dll");
                var time = Hypervisor.Read<float>(Hypervisor.GetPointer64(0x01B28678, [0x8, 0x0, 0x30, 0x70, 0x60, 0x28, 0x3C0], false, "UnityPlayer.dll"), true);
                var health = Hypervisor.Read<float>(Hypervisor.GetPointer64(0x01B3BFF8, [0xB8, 0x60, 0x78, 0x28, 0x20, 0xC0, 0x1C0], false, "UnityPlayer.dll"), true);

                var stageid_adventure = Hypervisor.Read<int>(characterid_adventure_base + 0x04, true);

                try
                {
                    if (time <= 0 && health == 0)
                    {
                        if (stageid_adventure > 0)
                        {

                            var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholdersAdventure);
                            PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Vampire Survivors", placeholders, "Default_Adventure");
                        }
                        else
                        {
                            var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                            PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Vampire Survivors", placeholders);
                        }
                    }
                    else
                    {
                        if (stageid_adventure > 0)
                        {
                            var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholdersAdventureIngame);
                            PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Vampire Survivors", placeholders, "Ingame_Adventure");
                        }
                        else
                        {
                            var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholdersIngame);
                            PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Vampire Survivors", placeholders, "Ingame");
                        }
                    }
                }
                catch
                {
                    discord.SetPresence(new RichPresence()
                    {
                        Details = "In Main Menu",
                        State = "",
                        Assets = new Assets()
                        {
                            LargeImageKey = "logo",
                            LargeImageText = "Vampire Survivors"
                        },
                        Timestamps = PlaceholderHelper._startTimestamp
                    });
                }

                await Task.Delay(3000);
                Thread thread = new Thread(RPC);
                thread.Start();
            }
            else
            {
                discord.Deinitialize();
                MainForm.gameUpdater.Start();
            }
        }

        private static async Task<Dictionary<string, object>> GeneratePlaceholders()
        {
            var characterid_base = Hypervisor.GetPointer64(0x050B7678, [0x408, 0x40, 0x50, 0x44], false, "GameAssembly.dll");
            var coins = Hypervisor.Read<float>(characterid_base + 0x34, true);

            return new Dictionary<string, object>
            {
                { "coins", (int)Math.Floor(coins) }
            };
        }

        private static async Task<Dictionary<string, object>> GeneratePlaceholdersAdventure()
        {
            var characterid_adventure_base = Hypervisor.GetPointer64(0x050B7678, [0x408, 0x40, 0x60, 0x44], false, "GameAssembly.dll");
            var coins_adventure = Hypervisor.Read<float>(characterid_adventure_base + 0x34, true);
            var stageid_adventure = Hypervisor.Read<int>(characterid_adventure_base + 0x04, true);

            if (stageid_adventure >= 1001 && stageid_adventure <= 1007)
                adventure = "Legacy of the Moonspell";
            else if (stageid_adventure >= 1011 && stageid_adventure <= 1016)
                adventure = "A Garlic Paradise";
            else if (stageid_adventure >= 1021 && stageid_adventure <= 1026)
                adventure = "World of Light and Dark";
            else if (stageid_adventure >= 1031 && stageid_adventure <= 1036)
                adventure = "Emergency Meeting";
            else if (stageid_adventure >= 1041 && stageid_adventure <= 1046)
                adventure = "Operation Guns";
            else
                adventure = "Adventure";

            return new Dictionary<string, object>
            {
                { "coins", (int)Math.Floor(coins_adventure) },
                { "adventure", adventure },
            };
        }

        private static async Task<Dictionary<string, object>> GeneratePlaceholdersAdventureIngame()
        {
            List<string> modesList = new List<string>();
            var characterid_adventure_base = Hypervisor.GetPointer64(0x050B7678, [0x408, 0x40, 0x60, 0x44], false, "GameAssembly.dll");

            var health = Hypervisor.Read<float>(Hypervisor.GetPointer64(0x01B3BFF8, [0xB8, 0x60, 0x78, 0x28, 0x20, 0xC0, 0x1C0], false, "UnityPlayer.dll"), true);
            var level = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x01B3BFF8, [0xB8, 0x60, 0x78, 0x28, 0x20, 0xC0, 0x1F4], false, "UnityPlayer.dll"), true);
            var time = Hypervisor.Read<float>(Hypervisor.GetPointer64(0x01B28678, [0x8, 0x0, 0x30, 0x70, 0x60, 0x28, 0x3C0], false, "UnityPlayer.dll"), true);

            var characterid_adventure = Hypervisor.Read<byte>(characterid_adventure_base, true);
            var stageid_adventure = Hypervisor.Read<int>(characterid_adventure_base + 0x04, true);
            var coinsingame_adventure = Hypervisor.Read<float>(characterid_adventure_base + 0x38, true);
            var kills_adventure = Hypervisor.Read<int>(characterid_adventure_base + 0x3C, true);
            var isHyper_adventure = Hypervisor.Read<byte>(characterid_adventure_base + 0x08, true);
            var isHurry_adventure = Hypervisor.Read<byte>(characterid_adventure_base + 0x09, true);
            var hasArcanas_adventure = Hypervisor.Read<byte>(characterid_adventure_base + 0x0A, true);
            var hasLimitbreak_adventure = Hypervisor.Read<byte>(characterid_adventure_base + 0x0B, true);
            var isInverse_adventure = Hypervisor.Read<byte>(characterid_adventure_base + 0x0C, true);
            var isEndless_adventure = Hypervisor.Read<byte>(characterid_adventure_base + 0x0D, true);
            var hasRandomEvents_adventure = Hypervisor.Read<byte>(characterid_adventure_base + 0x14, true);
            var hasRandomItems_adventure = Hypervisor.Read<byte>(characterid_adventure_base + 0x15, true);

            TimeSpan timeSpan = TimeSpan.FromSeconds(time);
            string formattedTime = string.Format("{0:D2}:{1:D2}", (int)timeSpan.TotalMinutes, timeSpan.Seconds);

            var character_adventure = await Characters.GetCharacter(characterid_adventure);
            var stage_adventure = await Stages.GetStages(stageid_adventure);

            if (isHyper_adventure == 1)
                modesList.Add("Hyper");
            if (isHurry_adventure == 1)
                modesList.Add("Hurry");
            if (hasArcanas_adventure == 1)
                modesList.Add("Arcanas");
            if (hasLimitbreak_adventure == 1)
                modesList.Add("Limit Break");
            if (isInverse_adventure == 1)
                modesList.Add("Inverse");
            if (isEndless_adventure == 1)
                modesList.Add("Endless");
            if (hasRandomEvents_adventure == 1)
                modesList.Add("Random Events");
            if (hasRandomItems_adventure == 1)
                modesList.Add("Random LevelUp");

            string modes = string.Join(", ", modesList);
            if (modesList.Count == 0)
                modes = "None";

            if (stageid_adventure >= 1001 && stageid_adventure <= 1007)
                adventure = "Legacy of the Moonspell";
            else if (stageid_adventure >= 1011 && stageid_adventure <= 1016)
                adventure = "A Garlic Paradise";
            else if (stageid_adventure >= 1021 && stageid_adventure <= 1026)
                adventure = "World of Light and Dark";
            else if (stageid_adventure >= 1031 && stageid_adventure <= 1036)
                adventure = "Emergency Meeting";
            else if (stageid_adventure >= 1041 && stageid_adventure <= 1046)
                adventure = "Operation Guns";
            else if (stageid_adventure >= 1052 && stageid_adventure <= 1056)
                adventure = "To End an Ice Age";
            else if (stageid_adventure >= 1058 && stageid_adventure <= 1063)
                adventure = "Tides of the Foscari";
            else
                adventure = "Adventure";

            return new Dictionary<string, object>
            {
                { "character", character_adventure },
                { "stage", stage_adventure },
                { "coinsingame", (int)Math.Floor(coinsingame_adventure) },
                { "kills", kills_adventure },
                { "health", (int)Math.Floor(health) },
                { "level", level },
                { "time", formattedTime },
                { "adventure", adventure },
                { "modes", modes }
            };
        }

        private static async Task<Dictionary<string, object>> GeneratePlaceholdersIngame()
        {
            List<string> modesList = new List<string>();
            var characterid_base = Hypervisor.GetPointer64(0x050B7678, [0x408, 0x40, 0x50, 0x44], false, "GameAssembly.dll");

            var health = Hypervisor.Read<float>(Hypervisor.GetPointer64(0x01AE0888, [0x348, 0xC8, 0x40, 0x110, 0x60, 0x60, 0x1F0], false, "UnityPlayer.dll"), true);
            var level = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x01B3BFF8, [0xB8, 0x60, 0x78, 0x28, 0x20, 0xC0, 0x1F4], false, "UnityPlayer.dll"), true);
            var time = Hypervisor.Read<float>(Hypervisor.GetPointer64(0x01B28678, [0x8, 0x0, 0x30, 0x70, 0x60, 0x28, 0x3C0], false, "UnityPlayer.dll"), true);

            var characterid = Hypervisor.Read<byte>(characterid_base, true);
            var stageid = Hypervisor.Read<int>(characterid_base + 0x04, true);
            var coins = Hypervisor.Read<float>(characterid_base + 0x34, true);
            var coinsingame = Hypervisor.Read<float>(characterid_base + 0x38, true);
            var kills = Hypervisor.Read<int>(characterid_base + 0x3C, true);
            var isHyper = Hypervisor.Read<byte>(characterid_base + 0x08, true);
            var isHurry = Hypervisor.Read<byte>(characterid_base + 0x09, true);
            var hasArcanas = Hypervisor.Read<byte>(characterid_base + 0x0A, true);
            var hasLimitbreak = Hypervisor.Read<byte>(characterid_base + 0x0B, true);
            var isInverse = Hypervisor.Read<byte>(characterid_base + 0x0C, true);
            var isEndless = Hypervisor.Read<byte>(characterid_base + 0x0D, true);
            var hasRandomEvents = Hypervisor.Read<byte>(characterid_base + 0x14, true);
            var hasRandomItems = Hypervisor.Read<byte>(characterid_base + 0x15, true);

            TimeSpan timeSpan = TimeSpan.FromSeconds(time);
            string formattedTime = string.Format("{0:D2}:{1:D2}", (int)timeSpan.TotalMinutes, timeSpan.Seconds);

            var character = await Characters.GetCharacter(characterid);
            var stage = await Stages.GetStages(stageid);

            if (isHyper == 1)
                modesList.Add("Hyper");
            if (isHurry == 1)
                modesList.Add("Hurry");
            if (hasArcanas == 1)
                modesList.Add("Arcanas");
            if (hasLimitbreak == 1)
                modesList.Add("Limit Break");
            if (isInverse == 1)
                modesList.Add("Inverse");
            if (isEndless == 1)
                modesList.Add("Endless");
            if (hasRandomEvents == 1)
                modesList.Add("Random Events");
            if (hasRandomItems == 1)
                modesList.Add("Random LevelUp");

            string modes = string.Join(", ", modesList);
            if (modesList.Count == 0)
                modes = "None";

            return new Dictionary<string, object>
            {
                { "character", character },
                { "stage", stage },
                { "coinsingame", (int)Math.Floor(coinsingame) },
                { "kills", kills },
                { "health", (int)Math.Floor(health) },
                { "level", level },
                { "time", formattedTime },
                { "modes", modes }
            };
        }

        private static void InitializeDiscord()
        {
            discord.Initialize();
        }
    }
}
