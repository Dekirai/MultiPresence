using System.Diagnostics;
using DiscordRPC;
using Memory;
using Button = DiscordRPC.Button;
using MultiPresence.Models.T7;

namespace MultiPresence.Presence
{
    public class T7
    {
        Mem mem = new Mem();
        string process = "TekkenGame-Win64-Shipping";
        private static DiscordRpcClient discord;
        public async void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("981603437043130379");
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
                int p1 = mem.ReadByte($"{process}.exe+34E72AC");
                int p2 = mem.ReadByte($"{process}.exe+34EA91C");
                int stage = mem.ReadByte($"{process}.exe+34E6200");
                int isBattle = mem.ReadByte($"{process}.exe+34D9024");

                var p1_name = await Characters.GetCharacter(p1);
                var p2_name = await Characters.GetCharacter(p2);
                var stage_name = await Stages.GetStage(stage);

                if (isBattle == 1)
                {
                    discord.UpdateLargeAsset($"logo", $"{stage_name[0]}");
                    discord.UpdateDetails($"{p1_name[0]} VS. {p2_name[0]}");
                    discord.UpdateState($"{stage_name[0]}");
                }
                else
                {
                    discord.UpdateLargeAsset($"logo", $"TEKKEN 7");
                    discord.UpdateDetails($"Idle");
                    discord.UpdateState(null);
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
                    new Button() { Label = $"View on Steam", Url = "https://store.steampowered.com/app/389730/TEKKEN_7/" },
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