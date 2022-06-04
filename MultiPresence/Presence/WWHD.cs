using System.Diagnostics;
using DiscordRPC;
using Memory;
using Button = DiscordRPC.Button;
using MultiPresence.Models.WWHD;

namespace MultiPresence.Presence
{
    public class WWHD
    {
        Mem mem = new Mem();
        string process = "Cemu";
        public string _spoof_address = "";
        public bool _spoof = false;
        private static DiscordRpcClient discord;
        public async void DoAction()
        {
            await Task.Delay(10000);
            GetPID();
            long spoof_get = (await mem.AoBScan("17 B3 C0 04 00 00 00 1A 17 B3 C0 28 10 00 1E 70 00 00 00 2D 17 B3 C1 54 10 00 1E 80 00 00 00 1A 00 00 00 2D 17 B3 C3 18 10 00 1E 90 00 00 00 00", true)).FirstOrDefault();
            _spoof_address = spoof_get.ToString("X11");
            _spoof = true;
            discord = new DiscordRpcClient("821476520891383849");
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
                string stage = mem.ReadString($"{_spoof_address}+0xA4");
                string realstage = await Stages.GetRealName(stage);

                discord.UpdateLargeAsset(stage.ToLower(), $"{realstage}");
                discord.UpdateDetails($"Current Location");
                discord.UpdateState($"{realstage}");
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
                    new Button() { Label = $"View in the eShop", Url = "https://www.nintendo.co.uk/Games/Wii-U/The-Legend-of-Zelda-The-Wind-Waker-HD-765386.html" },
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