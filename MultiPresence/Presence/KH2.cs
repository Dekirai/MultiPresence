using DiscordRPC;
using Memory;
using MultiPresence.Models.KH2;
using MultiPresence.Properties;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class KH2
    {
        static Mem mem = new Mem();
        static string process = "KINGDOM HEARTS II FINAL MIX";
        private static DiscordRpcClient discord;
        private static DiscordStatusUpdater updater;
        public static string difficulty = "";
        public static string[] room = null;
        public static string[] world = null;
        public static void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("826145131152408625");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("config.json");
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
                int world_get = mem.ReadByte($"{process}.exe+717008");
                int room_get = mem.ReadByte($"{process}.exe+717009");
                int difficulty_get = mem.ReadByte($"{process}.exe+9ABCC8");
                int level = mem.ReadByte($"{process}.exe+9ABD2F");

                try
                {
                    if (Settings.Default.langDE == true)
                    {
                        world = await Worlds.GetWorldDE(world_get);
                        room = await Rooms.GetRoomDE(world[0]);
                        difficulty = await Difficulties.GetDifficultyDE(difficulty_get);
                    }
                    else
                    {
                        world = await Worlds.GetWorld(world_get);
                        room = await Rooms.GetRoom(world[0]);
                        difficulty = await Difficulties.GetDifficulty(difficulty_get);
                    }

                    var placeholders = new Dictionary<string, object>
                    {
                        { "level", level },
                        { "room", room[room_get] },
                        { "world", world[0] },
                        { "world_icon_name", world[1] },
                        { "difficulty", difficulty }
                    };

                    string details = updater.UpdateDetails("Kingdom Hearts II Final Mix", placeholders);
                    string state = updater.UpdateState("Kingdom Hearts II Final Mix", placeholders);
                    string largeasset = updater.UpdateLargeAsset("Kingdom Hearts II Final Mix", placeholders);
                    string largeassettext = updater.UpdateLargeAssetText("Kingdom Hearts II Final Mix", placeholders);
                    string smallasset = updater.UpdateSmallAsset("Kingdom Hearts II Final Mix", placeholders);
                    string smallassettext = updater.UpdateSmallAssetText("Kingdom Hearts II Final Mix", placeholders);
                    string button1text = updater.UpdateButton1Text("Kingdom Hearts II Final Mix", placeholders);
                    string button2text = updater.UpdateButton2Text("Kingdom Hearts II Final Mix", placeholders);
                    string button1url = updater.UpdateButton1URL("Kingdom Hearts II Final Mix", placeholders);
                    string button2url = updater.UpdateButton2URL("Kingdom Hearts II Final Mix", placeholders);
                    discord.UpdateLargeAsset(largeasset, largeassettext);
                    discord.UpdateSmallAsset(smallasset, smallassettext);
                    discord.UpdateDetails(details);
                    discord.UpdateState(state);

                    if (button1url.Length > 0 && button2url.Length == 0)
                    {
                        discord.UpdateButtons(new DiscordRPC.Button[]
                        {
                            new DiscordRPC.Button() { Label = button1text, Url = button1url }
                        });
                    }
                    else if (button1url.Length > 0 && button2url.Length > 0)
                    {
                        discord.UpdateButtons(new DiscordRPC.Button[]
                        {
                            new DiscordRPC.Button() { Label = button1text, Url = button1url },
                            new DiscordRPC.Button() { Label = button2text, Url = button2url }
                        });
                    }
                    else
                    {
                        discord.UpdateButtons(null);
                    }
                }
                catch
                {
                    discord.UpdateLargeAsset("logo", "Kingdom Hearts II Final Mix");
                    discord.UpdateDetails($"In Main Menu");
                    discord.UpdateState("");
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
