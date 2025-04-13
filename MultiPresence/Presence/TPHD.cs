using DiscordRPC;
using MultiPresence.Models.TPHD;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class TPHD
    {
        static string location = "";
        static string area = "";
        public static ulong _main_address = 0;
        private static DiscordRpcClient? discord;
        public static async void DoAction()
        {
            await Task.Delay(5500);
            GetPID();
            _main_address = (ulong)Hypervisor.FindSignature("00 00 00 00 00 4C 6F 61 64 69 6E 67 20");
            discord = new DiscordRpcClient("983296451453022220");
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
            string[] stage = Hypervisor.ReadString(_main_address + 0x0D, 32, true).Split(':');
            if (game.Length > 0)
            {

                try
                {
                    location = stage[0];
                    area = $"{stage[0]}-{stage[1]}";
                }
                catch
                {
                    location = Hypervisor.ReadString(_main_address + 0x0D, 32, true);
                }

                int form = Hypervisor.Read<byte>(_main_address + 0x12369, true);
                int poes = Hypervisor.Read<byte>(_main_address + 0x12455, true);
                int rupees_source = Hypervisor.Read<byte>(_main_address + 0x1234F, true);
                byte[] getbytes = BitConverter.GetBytes(rupees_source);
                Array.Reverse(getbytes);
                int rupees = BitConverter.ToInt16(getbytes, 2);
                string realstage = await Stages_Old.MapName(location);
                string hearts = await Hearts.GetHearts(Hypervisor.Read<byte>(_main_address + 0x1234E, true));
                int hearts_max = Hypervisor.Read<byte>(_main_address + 0x1234C, true) / 5;

                if (location == "Opening Scene" || location == "Name Scene")
                {
                    discord.SetPresence(new RichPresence()
                    {
                        Details = "In Main Menu",
                        State = "",
                        Assets = new Assets()
                        {
                            LargeImageKey = "logo",
                            LargeImageText = "The Legend of Zelda: Twilight Princess HD"
                        },
                        Timestamps = PlaceholderHelper._startTimestamp
                    });
                }
                else
                {
                    discord.SetPresence(new RichPresence()
                    {
                        Details = $"Health: {hearts}/{hearts_max} ❤️ | Rupees: {rupees}",
                        State = $"{realstage} | Poes: {poes}/60",
                        Assets = new Assets()
                        {
                            LargeImageKey = form == 0 ? "link" : "wolf",
                            LargeImageText = form == 0 ? "In Human Form" : "In Wolf Form"
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
                MainForm.gameUpdater.Start();
            }
        }

        private static void InitializeDiscord()
        {
            discord.Initialize();
        }
    }
}