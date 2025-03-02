using DiscordRPC;
using MultiPresence.Models.SADX;
using System.Diagnostics;
using System.Security.AccessControl;

namespace MultiPresence.Presence
{
    public class SADX
    {
        private static DiscordRpcClient? discord;
        private static DiscordStatusUpdater? updater;
        public static void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("1345421793787117653");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("config.json");
            Thread thread = new Thread(RPC);
            thread.Start();
        }

        private static void GetPID()
        {
            try
            {
                var _myProcess = Process.GetProcessesByName("Sonic Adventure DX")[0];
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
            Process[] game = Process.GetProcessesByName("Sonic Adventure DX");
            if (game.Length > 0)
            {
                int stage = Hypervisor.Read<byte>(0x5739BF0);
                int rings = Hypervisor.Read<int>(0x573EBB8);

                var stage_name = await Stages.GetStage(stage);


                if (stage == 0 && rings > 999)
                {
                    discord.SetPresence(new RichPresence()
                    {
                        Details = "In Main Menu",
                        State = "",
                        Assets = new Assets()
                        {
                            LargeImageKey = "logo",
                            LargeImageText = "Sonic Adventure DX"
                        },
                        Timestamps = PlaceholderHelper._startTimestamp
                    });
                }
                else if (stage == 26 || stage == 33 || stage == 34 || stage == 29 || stage == 32 || stage == 39 || stage == 40 || stage == 41 || stage == 42)
                {
                    var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                    PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Sonic Adventure DX", placeholders, "Adventure Field");
                }
                else
                {
                    var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                    PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Sonic Adventure DX", placeholders, "Level");
                }

                await Task.Delay(1000);
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
            int stage = Hypervisor.Read<byte>(0x5739BF0);
            int character = Hypervisor.Read<byte>(0x58D14EB);
            int lives = Hypervisor.Read<byte>(0x573EA14);
            int rings = Hypervisor.Read<int>(0x573EBB8);

            var stage_name = await Stages.GetStage(stage);
            var character_name = await Characters.GetCharacter(character);

            string charactericon = character switch
            {
                0 => "sonic",
                1 => "tails",
                2 => "knuckles",
                3 => "amy",
                4 => "big",
                5 => "102",
                6 => "super",
                7 => "metal"
            };

            if (stage == 26 || stage == 33 || stage == 34 || stage == 29 || stage == 32)
            {
                return new Dictionary<string, object>
                {
                    { "level", stage_name },
                    { "character_icon", charactericon },
                    { "character", character_name }
                };
            }

            return new Dictionary<string, object>
            {
                { "level", stage_name },
                { "character_icon", charactericon },
                { "character", character_name },
                { "lives", lives },
                { "rings", rings }
            };
        }

        private static void InitializeDiscord()
        {
            discord.Initialize();
        }
    }
}