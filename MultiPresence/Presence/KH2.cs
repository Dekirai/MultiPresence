using DiscordRPC;
using MultiPresence.Models.KH2;
using MultiPresence.Properties;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;

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
                int battleflag = Hypervisor.Read<byte>(0x2A11404);
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
            int world_get = Hypervisor.Read<byte>(0x717008);
            int room_get = Hypervisor.Read<byte>(0x717009);
            int difficulty_get = Hypervisor.Read<byte>(0x9ABD48);
            int level = Hypervisor.Read<byte>(0x9ABDAF);
            int hp = Hypervisor.Read<int>(0x2A23598);
            int hpmax = Hypervisor.Read<int>(0x2A2359C);
            int mp = Hypervisor.Read<int>(0x2A23718);
            int mpmax = Hypervisor.Read<int>(0x2A2371C);
            int form_get = Hypervisor.Read<byte>(0x09ACDD4);

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
