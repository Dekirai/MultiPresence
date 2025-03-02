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
            updater = new DiscordStatusUpdater("config.json");
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
                var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Mega Man 11", placeholders);

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
            int lives = Hypervisor.Read<byte>(Hypervisor.GetPointer64(0x140C3F6C0, [0x3A40]), true);
            int difficulty = Hypervisor.Read<byte>(Hypervisor.GetPointer64(0x140C3F6C0, [0x388C]), true);
            int stage = Hypervisor.Read<byte>(Hypervisor.GetPointer64(0x140B87A20, [0xDF0, 0xA8, 0x18, 0xA0]), true);

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

        private static void InitializeDiscord()
        {
            discord.Initialize();
        }
    }
}