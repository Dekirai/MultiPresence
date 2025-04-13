using DiscordRPC;
using MultiPresence.Models.WWHD;
using System.Diagnostics;
using System.Windows.Forms;
using System.Xml.Linq;

namespace MultiPresence.Presence
{
    public class WWHD
    {
        public static ulong _main_address = 0;
        public static ulong _spoof_address = 0;
        private static DiscordRpcClient? discord;
        public static async void DoAction()
        {
            await Task.Delay(7000);
            GetPID();
            _main_address = (ulong)Hypervisor.FindSignature("14 50 ?? A4 14 50 ?? 18 00 F0 2E 1C 00 00 00 00 14 50 ?? 70 14 50 ?? 0C 14 ?? ?? 1C 10 14 5B ?? 10 00 66 C8 10 14 5C ?? 10 14 5C ?? 14 5B ?? C4 00 F0 2E 1C 14 50 ?? 70 14 5B ?? EC 14 5B ?? EC 00 00 00 00 00 00 00 38 14 50 ?? EC 14 50 ?? 98 14 5B ?? 64 14 5B ?? 74 00 00 00 02 00 00 00 04 00 00 00 01 14 50 ?? 70 14 5B ?? C8 14 50 ?? B0 10 15 E6");
            _spoof_address = (ulong)Hypervisor.FindSignature("17 B3 C0 04 00 00 00 1A 17 B3 C0 28 10 00 1E 70 00 00 00 2D 17 B3 C1 54 10 00 1E 80 00 00 00 1A 00 00 00 2D 17 B3 C3 18 10 00 1E 90 00 00 00 00");
            discord = new DiscordRpcClient("983295791504429116");
            InitializeDiscord();
            Thread thread = new Thread(RPC);
            thread.Start();
        }

        private static void GetPID()
        {
            try
            {
                var _myProcess = Process.GetProcessesByName("Cemu")[0];
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
            Process[] game = Process.GetProcessesByName("Cemu");
            if (game.Length > 0)
            {
                string stage = Hypervisor.ReadString(_spoof_address + 0xA4, 32, true);
                string realstage = await Stages.GetRealName(stage);
                string hearts = await Hearts.GetHearts(Hypervisor.Read<byte>(_main_address + 0xCF, true));
                int rupees_source = Hypervisor.Read<short>(_main_address + 0xD0, true);
                byte[] getbytes = BitConverter.GetBytes(rupees_source);
                Array.Reverse(getbytes);
                int rupees = BitConverter.ToInt16(getbytes, 2);

                discord.SetPresence(new RichPresence()
                {
                    Details = $"Health: {hearts} ❤️ | Rupees: {rupees}",
                    State = $"{realstage}",
                    Assets = new Assets()
                    {
                        LargeImageKey = "name",
                        LargeImageText = "The Legend of Zelda: The Wind Waker HD"
                    },
                    Timestamps = PlaceholderHelper._startTimestamp
                });

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
        }
    }
}