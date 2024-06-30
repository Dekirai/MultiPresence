using System.Diagnostics;
using DiscordRPC;
using Memory;
using MultiPresence.Models.TPHD;

namespace MultiPresence.Presence
{
    public class TPHD
    {
        static Mem mem = new Mem();
        static string process = "Cemu";
        static string location = "";
        static string area = "";
        public static string _main_address = "";
        private static DiscordRpcClient discord;
        public static async void DoAction()
        {
            await Task.Delay(5500);
            GetPID();
            long main_get = (await mem.AoBScan("00 00 00 00 00 4C 6F 61 64 69 6E 67 20", true)).FirstOrDefault();
            _main_address = main_get.ToString("X11");
            discord = new DiscordRpcClient("983296451453022220");
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
            string[] stage = mem.ReadString($"{_main_address}+D").Split(':');
            if (game.Length > 0)
            {

                try
                {
                    location = stage[0];
                    area = $"{stage[0]}-{stage[1]}";
                }
                catch
                {
                    location = mem.ReadString($"{_main_address}+D");
                }

                int form = mem.ReadByte($"{_main_address}+12369");
                int poes = mem.ReadByte($"{_main_address}+12455");
                int rupees_source = mem.Read2Byte($"{_main_address}+1234F");
                byte[] getbytes = BitConverter.GetBytes(rupees_source);
                Array.Reverse(getbytes);
                int rupees = BitConverter.ToInt16(getbytes, 2);
                string realstage = await Stages_Old.MapName(location);
                string hearts = await Hearts.GetHearts(mem.ReadByte($"{_main_address}+0x1234E"));
                int hearts_max = mem.ReadByte($"{_main_address}+0x1234C") / 5;

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
                    discord.UpdateDetails($"Health: {hearts}/{hearts_max} ❤️ | Rupees: {rupees}");
                    discord.UpdateState($"{realstage} | Poes: {poes}/60");
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
                Timestamps = new Timestamps()
                {
                    Start = DateTime.UtcNow.AddSeconds(1)
                }
            });
        }
    }
}