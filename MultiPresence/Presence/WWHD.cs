using System.Diagnostics;
using DiscordRPC;
using Memory;
using MultiPresence.Models.WWHD;

namespace MultiPresence.Presence
{
    public class WWHD
    {
        static Mem mem = new Mem();
        static string process = "Cemu";
        public static string _main_address = "";
        public static string _spoof_address = "";
        private static DiscordRpcClient discord;
        public static async void DoAction()
        {
            await Task.Delay(7000);
            GetPID();
            long main_get = (await mem.AoBScan("14 50 ?? A4 14 50 ?? 18 00 F0 2E 1C 00 00 00 00 14 50 ?? 70 14 50 ?? 0C 14 ?? ?? 1C 10 14 5B ?? 10 00 66 C8 10 14 5C ?? 10 14 5C ?? 14 5B ?? C4 00 F0 2E 1C 14 50 ?? 70 14 5B ?? EC 14 5B ?? EC 00 00 00 00 00 00 00 38 14 50 ?? EC 14 50 ?? 98 14 5B ?? 64 14 5B ?? 74 00 00 00 02 00 00 00 04 00 00 00 01 14 50 ?? 70 14 5B ?? C8 14 50 ?? B0 10 15 E6", true)).FirstOrDefault();
            long spoof_get = (await mem.AoBScan("17 B3 C0 04 00 00 00 1A 17 B3 C0 28 10 00 1E 70 00 00 00 2D 17 B3 C1 54 10 00 1E 80 00 00 00 1A 00 00 00 2D 17 B3 C3 18 10 00 1E 90 00 00 00 00", true)).FirstOrDefault();
            _main_address = main_get.ToString("X11");
            _spoof_address = spoof_get.ToString("X11");
            discord = new DiscordRpcClient("983295791504429116");
            InitializeDiscord();
            Thread thread = new Thread(RPC);
            thread.Start();
        }

        private static void GetPID()
        {
            int pid = mem.GetProcIdFromName(process);
            bool openProc = false;

            if (pid > 0) openProc = mem.OpenProcess(pid);
        }

        private static async void RPC()
        {
            Process[] game = Process.GetProcessesByName(process);
            if (game.Length > 0)
            {
                string stage = mem.ReadString($"{_spoof_address}+0xA4");
                string realstage = await Stages.GetRealName(stage);
                string hearts = await Hearts.GetHearts(mem.ReadByte($"{_main_address}+0xCF"));
                int rupees_source = mem.Read2Byte($"{_main_address}+D0");
                byte[] getbytes = BitConverter.GetBytes(rupees_source);
                Array.Reverse(getbytes);
                int rupees = BitConverter.ToInt16(getbytes, 2);

                //discord.UpdateLargeAsset(stage.ToLower(), $"{realstage}");
                discord.UpdateLargeAsset("name", $"The Legend of Zelda: The Wind Waker HD");
                discord.UpdateDetails($"Health: {hearts} ❤ | Rupees: {rupees}");
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
                Timestamps = new Timestamps()
                {
                    Start = DateTime.UtcNow.AddSeconds(1)
                }
            });
        }
    }
}