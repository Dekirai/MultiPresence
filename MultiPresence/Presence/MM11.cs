using DiscordRPC;
using MultiPresence.Models.MM11;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class MM11
    {
        private static DiscordRpcClient? discord;
        private static DiscordStatusUpdater? updater;
        public static void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("981534050781122570");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/config/Mega Man 11.json");
            Thread thread = new Thread(RPC);
            thread.Start();
        }

        private static void GetPID()
        {
            try
            {
                var _myProcess = Process.GetProcessesByName("game")[0];
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
            Process[] game = Process.GetProcessesByName("game");
            if (game.Length > 0)
            {
                try
                {
                    ulong stageaddress = Hypervisor.GetPointer64(0x140B87A20, [0xDF0, 0xA8, 0x18, 0xA0], true);
                    int loadedsave = Hypervisor.Read<byte>(Hypervisor.GetPointer64(0x140C3F6C0, [0x3A45], true), true);

                    if (stageaddress > 0x100000)
                    {
                        var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                        PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Mega Man 11", placeholders, "Ingame");
                    }
                    else if (loadedsave == 0x01)
                    {
                        var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholdersStageSelect);
                        PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Mega Man 11", placeholders);
                    }
                    else
                    {
                        discord.SetPresence(new RichPresence()
                        {
                            Details = "In Main Menu",
                            State = "",
                            Assets = new Assets()
                            {
                                LargeImageKey = "logo",
                                LargeImageText = "Mega Man 11"
                            },
                            Timestamps = PlaceholderHelper._startTimestamp
                        });
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
                            LargeImageText = "Mega Man 11"
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
                updater.Dispose();
                MainForm.gameUpdater.Start();
            }
        }

        private static async Task<Dictionary<string, object>> GeneratePlaceholders()
        {
            int lives = Hypervisor.Read<byte>(Hypervisor.GetPointer64(0x140C3F6C0, [0x3A40], true), true);
            int difficulty = Hypervisor.Read<byte>(Hypervisor.GetPointer64(0x140C3F6C0, [0x388C], true), true);
            int stage = Hypervisor.Read<byte>(Hypervisor.GetPointer64(0x140B87A20, [0xDF0, 0xA8, 0x18, 0xA0], true), true);

            var stagename = await Stages.GetStage(stage);
            var difficultyname = await Difficulties.GetDifficulty(difficulty);

            return new Dictionary<string, object>
            {
                { "lives", lives },
                { "difficulty", difficultyname[0] },
                { "stage", stagename[0] },
                { "stage_icon_name", stagename[1] }
            };
        }

        private static async Task<Dictionary<string, object>> GeneratePlaceholdersStageSelect()
        {
            int lives = Hypervisor.Read<byte>(Hypervisor.GetPointer64(0x140C3F6C0, [0x3A40], true), true);
            int bolts = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x140C3F6C0, [0x3890], true), true);
            int difficulty = Hypervisor.Read<byte>(Hypervisor.GetPointer64(0x140C3F6C0, [0x388C], true), true);


            var difficultyname = await Difficulties.GetDifficulty(difficulty);

            return new Dictionary<string, object>
            {
                { "lives", lives },
                { "difficulty", difficultyname[0] },
                { "bolts", bolts }
            };
        }

        private static void InitializeDiscord()
        {
            discord.Initialize();
        }
    }
}