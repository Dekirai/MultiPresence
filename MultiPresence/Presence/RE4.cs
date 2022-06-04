using System.Diagnostics;
using DiscordRPC;
using Memory;
using Button = DiscordRPC.Button;
using MultiPresence.Models.RE4;

namespace MultiPresence.Presence
{
    public class RE4
    {
        Mem mem = new Mem();
        string process = "bio4";
        private static DiscordRpcClient discord;
        public async void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("982193093388427314");
            InitializeDiscord();
            Thread thread = new Thread(RPC);
            thread.Start();
        }

        private void GetPID()
        {
            int pid = mem.GetProcIdFromName(process);
            bool openProc = false;

            if (pid > 0) openProc = mem.OpenProcess(pid);
        }

        private async void RPC()
        {
            Process[] game = Process.GetProcessesByName(process);
            if (game.Length > 0)
            {
                string area = "";
                int stage = mem.ReadByte($"{process}.exe+85A789");
                int room = mem.ReadByte($"{process}.exe+85A788");
                int weapon = mem.ReadByte($"{process}.exe+870FE4");
                int difficulty = mem.ReadByte($"{process}.exe+862BDC");

                var room_name = await Stages.GetStage(stage);
                var weapon_name = await Weapons.GetWeapon(weapon);
                var difficulty_name = await Difficulties.GetDifficulty(difficulty);

                if (stage == 1)
                    area = "Village";
                if (stage == 2)
                    area = "Castle";
                if (stage == 3)
                    area = "Island";

                if (stage == 1 && room == 32)
                {
                    discord.UpdateLargeAsset($"logo", $"Resident Evil 4");
                    discord.UpdateDetails($"At the Title Screen");
                    discord.UpdateState(null);
                }
                else
                {
                    discord.UpdateLargeAsset($"logo", $"Resident Evil 4");
                    discord.UpdateDetails($"{weapon_name} ({difficulty_name})");
                    discord.UpdateState($"{area}: {room_name[room]}");
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

        private static void InitializeDiscord()
        {
            discord.Initialize();
            discord.SetPresence(new RichPresence()
            {
                Buttons = new Button[]
                {
                    new Button() { Label = $"View on Steam", Url = "https://store.steampowered.com/app/254700/Resident_Evil_4/" },
#if DEBUG
                    new Button() { Label = $"Powered by MultiPresence", Url = "https://github.com/Dekirai/MultiPresence" }
#endif
                },
                Timestamps = new Timestamps()
                {
                    Start = DateTime.UtcNow.AddSeconds(1)
                }
            });
        }
    }
}