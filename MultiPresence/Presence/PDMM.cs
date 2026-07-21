#nullable disable
using MultiPresence.Runtime;
using DiscordRPC;
using MultiPresence.Models.PDMM;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class PDMM
    {
        private static DiscordRpcClient discord;
        private static DiscordStatusUpdater updater;

        public static void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("1373346929479647252");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/config/Project Diva Mega Mix+.json");
            PresenceRuntime.Start(nameof(PDMM), "DivaMegaMix", RPC);
        }

        private static void GetPID()
        {
            try
            {
                ProcessMonitor.TryAttach("DivaMegaMix");
            }
            catch
            {
                //nothing?
            }
        }

        private static async Task RPC()
        {
            if (ProcessMonitor.IsRunning("DivaMegaMix"))
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
            }
            else
            {
                discord.Deinitialize();
                updater.Dispose();
                PresenceRuntime.RequestDetection();
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
                4 => "Extra Extreme",
                _ => "Unknown"
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
                3 => "Extreme",
                _ => "Unknown"
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