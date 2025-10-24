using DiscordRPC;
using MultiPresence.Models.PDMM;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class PDMM
    {
        private static DiscordRpcClient? discord;
        private static DiscordStatusUpdater? updater;

        public static void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("1373346929479647252");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/config/Project Diva Mega Mix+.json");
            Thread thread = new Thread(RPC);
            thread.Start();
        }

        private static void GetPID()
        {
            try
            {
                var _myProcess = Process.GetProcessesByName("DivaMegaMix")[0];
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
            Process[] game = Process.GetProcessesByName("DivaMegaMix");
            if (game.Length > 0)
            {
                int isIngame = Hypervisor.Read<byte>(0xDB9A84);

                if (isIngame == 1)
                {
                    var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholdersIngame);
                    PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Project Diva Mega Mix+", placeholders, "Ingame");
                }
                else
                {
                    var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                    PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Project Diva Mega Mix+", placeholders);
                }

                await Task.Delay(3000);
                Thread thread = new Thread(RPC);
                thread.Start();
            }
            else
            {
                discord.Deinitialize();
                updater.Dispose();
                MainForm.gameUpdater.Start();
            }
        }

        private static async Task<Dictionary<string, object>> GeneratePlaceholders()
        {
            int difficulty_get = Hypervisor.Read<byte>(0x16E2B90);
            string songid_get = Hypervisor.ReadString(Hypervisor.GetPointer64(0x0CBFF0A0, [0x0F]), 6, true);

            string song = await SongIDs.GetSong(songid_get);

            string difficulty = difficulty_get switch
            {
                0 => "Easy",
                1 => "Normal",
                2 => "Hard",
                3 => "Extreme",
                4 => "Extra Extreme"
            };

            return new Dictionary<string, object>
            {
                { "difficulty", difficulty },
                { "song", song}
            };
        }

        private static async Task<Dictionary<string, object>> GeneratePlaceholdersIngame()
        {
            int difficulty_get = Hypervisor.Read<byte>(0x16E2B90);
            int score = Hypervisor.Read<int>(0x12EF568);
            int life = Hypervisor.Read<int>(0x12EF564);
            string songid_get = Hypervisor.ReadString(Hypervisor.GetPointer64(0x0CBFF0A0, [0x0F]), 6, true);
            int notes_appeared = Hypervisor.Read<int>(0x12EF628);
            int longestcombo = Hypervisor.Read<int>(0x12EF58C);
            int currentcombo = Hypervisor.Read<int>(0x12EEFEC);
            string songname = Hypervisor.ReadString(Hypervisor.GetPointer64(0x0CC0B5F8, [0x0]), 255, true);
            int exextreme = Hypervisor.Read<byte>(0x16E2B94);

            if (songname == null || songname == "")
                songname = await SongIDs.GetSong(songid_get);

            string difficulty = difficulty_get switch
            {
                0 => "Easy",
                1 => "Normal",
                2 => "Hard",
                3 => "Extreme"
            };

            if (exextreme == 1)
                difficulty = "Extra Extreme";

            return new Dictionary<string, object>
            {
                { "difficulty", difficulty },
                { "life", life },
                { "score", score },
                { "song", songname},
                { "notesappeared", notes_appeared },
                { "longestcombo", longestcombo },
                { "currentcombo", currentcombo }
            };
        }

        private static void InitializeDiscord()
        {
            discord.Initialize();
        }
    }
}