using DiscordRPC;
using MultiPresence.Models.KH2;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class KH2
    {
        private static DiscordRpcClient? discord;
        private static DiscordStatusUpdater? updater;

        public static void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("826145131152408625");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("config.json");
            Thread thread = new Thread(RPC);
            thread.Start();
        }

        private static void GetPID()
        {
            try
            {
                var _myProcess = Process.GetProcessesByName("KINGDOM HEARTS II FINAL MIX")[0];
                if (_myProcess.Id > 0)
                    Hypervisor.AttachProcess(_myProcess);
            }
            catch
            {
                // Ignore exception
            }
        }

        private static async void RPC()
        {
            Process[] game = Process.GetProcessesByName("KINGDOM HEARTS II FINAL MIX");
            if (game.Length > 0)
            {
                bool isEpicGames = false;
                int battleflag = 0;
                string version = Hypervisor.ReadString(0x9A9330, 4);
                if (version == "KH2J")
                    isEpicGames = true;
                if (isEpicGames)
                    battleflag = Hypervisor.Read<byte>(0x2A10E84);
                else
                    battleflag = Hypervisor.Read<byte>(0x2A11404);
                try
                {
                    if (battleflag == 0)
                    {
                        var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                        PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Kingdom Hearts II Final Mix", placeholders);
                    }
                    else if (battleflag == 1)
                    {
                        var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                        PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Kingdom Hearts II Final Mix", placeholders, "FieldBattle");
                    }
                    else if (battleflag == 2)
                    {
                        var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                        PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Kingdom Hearts II Final Mix", placeholders, "BossBattle");
                    }
                }
                catch
                {
                    discord.UpdateLargeAsset("logo", "Kingdom Hearts II Final Mix");
                    discord.UpdateDetails("In Main Menu");
                    discord.UpdateState("");
                }

                await Task.Delay(300);
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
            bool isEpicGames = false;
            string version = Hypervisor.ReadString(0x9A9330, 4);
            if (version == "KH2J")
                isEpicGames = true;
            int world_get = 0;
            int room_get = 0;
            int difficulty_get = 0;
            int level = 0;
            int hp = 0;
            int hpmax = 0;
            int mp = 0;
            int mpmax = 0;
            int form_get = 0;

            if (isEpicGames)
            {
                world_get = Hypervisor.Read<byte>(0x716DF8);
                room_get = Hypervisor.Read<byte>(0x716DF9);
                difficulty_get = Hypervisor.Read<byte>(0x9ABD48 - 0x580);
                level = Hypervisor.Read<byte>(0x9ABDAF - 0x580);
                hp = Hypervisor.Read<int>(0x2A23598 - 0x580);
                hpmax = Hypervisor.Read<int>(0x2A2359C - 0x580);
                mp = Hypervisor.Read<int>(0x2A23718 - 0x580);
                mpmax = Hypervisor.Read<int>(0x2A2371C - 0x580);
                form_get = Hypervisor.Read<byte>(0x9ACDD4 - 0x580);
            }
            else
            {
                world_get = Hypervisor.Read<byte>(0x717008);
                room_get = Hypervisor.Read<byte>(0x717009);
                difficulty_get = Hypervisor.Read<byte>(0x9ABD48);
                level = Hypervisor.Read<byte>(0x9ABDAF);
                hp = Hypervisor.Read<int>(0x2A23598);
                hpmax = Hypervisor.Read<int>(0x2A2359C);
                mp = Hypervisor.Read<int>(0x2A23718);
                mpmax = Hypervisor.Read<int>(0x2A2371C);
                form_get = Hypervisor.Read<byte>(0x9ACDD4);
            }

            var world = await Worlds.GetWorld(world_get);
            var room = await Rooms.GetRoom(world[0]);
            var difficulty = await Difficulties.GetDifficulty(difficulty_get);
            string form = "";

            if (form_get == 0)
                form = "None";
            else if (form_get == 1)
                form = "Valor";
            else if (form_get == 2)
                form = "Wisdom";
            else if (form_get == 3)
                form = "Limit";
            else if (form_get == 4)
                form = "Master";
            else if (form_get == 5)
                form = "Final";
            else if (form_get == 6)
                form = "Anti";

            return new Dictionary<string, object>
            {
                { "level", level },
                { "hp", hp },
                { "hpmax", hpmax },
                { "mp", mp },
                { "mpmax", mp },
                { "form", form },
                { "room", room[room_get] },
                { "world", world[0] },
                { "world_icon_name", world[1] },
                { "difficulty", difficulty }
            };
        }

        private static void InitializeDiscord()
        {
            discord.Initialize();
            discord.SetPresence(new RichPresence()
            {
                Timestamps = new Timestamps()
                {
                    Start = DateTime.UtcNow.AddSeconds(1)
                }
            });
        }
    }
}
