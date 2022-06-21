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
        string location = "";
        string area = "";
        public string _main_address = "";
        private static DiscordRpcClient discord;
        public async void DoAction()
        {
            await Task.Delay(7500);
            GetPID();
            long main_get = (await mem.AoBScan("00 00 00 00 00 4C 6F 61 64 69 6E 67 20", true)).FirstOrDefault();
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
            string[] stage = mem.ReadString($"{_main_address}+D").Split(':');
            if (game.Length > 0)
            {
                try
                {
                    location = stage[0];
                    area = $"{stage[0]}-{stage[1]}"; //Used for a future update
                }
                catch
                {
                    location = mem.ReadString($"{_main_address}+D");
                }
                int form = mem.ReadByte($"{_main_address}+12369");
                string realstage = await Stages.MapName(area);
                string hearts = await Hearts.GetHearts(mem.ReadByte($"{_main_address}+0x1234E"));

                if (location == "Opening Scene" || location == "Name Scene")
                {
                    discord.UpdateLargeAsset("logo", $"The Legend of Zelda: Twilight Princess HD");
                    discord.UpdateDetails($"At the Title Sceen");
                    discord.UpdateState($"");
                }
                else
                {
                    if (form == 0)
                        discord.UpdateLargeAsset("link", "Running around as a Human");
                    else
                        discord.UpdateLargeAsset("wolf", "Running around as a Wolf");
                    discord.UpdateDetails($"Health: {hearts}");
                    discord.UpdateState($"{realstage}");
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