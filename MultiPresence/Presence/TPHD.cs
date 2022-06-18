using System.Diagnostics;
using DiscordRPC;
using Memory;
using Button = DiscordRPC.Button;
using MultiPresence.Models.TPHD;
using System.Collections;

namespace MultiPresence.Presence
{
    public class TPHD
    {
        Mem mem = new Mem();
        string process = "Cemu";
        public string _main_address = "";
        private static DiscordRpcClient discord;
        public async void DoAction()
        {
            await Task.Delay(10000);
            GetPID();
            long main_get = (await mem.AoBScan("2D 5C 10 18 2D 68 00 00 00", true)).FirstOrDefault();
            _main_address = main_get.ToString("X11");
            discord = new DiscordRpcClient("983296451453022220");
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
                string stage = mem.ReadString($"{_main_address}+0x52AA");
                string realstage = await Stages.MapName(stage);
                string hearts = await Hearts.GetHearts(mem.ReadByte($"{_main_address}+0xF"));

                discord.UpdateLargeAsset("logo", $"The Legend of Zelda: Twilight Princess HD");
                discord.UpdateDetails($"Health: {hearts}");
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
                    new Button() { Label = $"Open Store Page", Url = "https://www.nintendo.co.uk/Games/Wii-U-games/The-Legend-of-Zelda-Twilight-Princess-HD-1082222.html" },
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