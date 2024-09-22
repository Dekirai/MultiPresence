using DiscordRPC;
using Memory;
using MultiPresence.Models.KHBBS;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class KHBBS
    {
        static Mem mem = new Mem();
        static string process = "KINGDOM HEARTS Birth by Sleep FINAL MIX";
        private static DiscordRpcClient discord;
        private static DiscordStatusUpdater updater;
        public static void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("839545395368820806");
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
                int world_get = mem.ReadByte($"{process}.exe+818120");
                int room_get = mem.ReadByte($"{process}.exe+818121");
                int difficulty_get = mem.ReadByte($"{process}.exe+10FA0881");
                int character_get = mem.ReadByte($"{process}.exe+10F9EE4C");
                int level = mem.ReadByte($"{process}.exe+10F9EEE1");
                var world = await Worlds.GetWorld(world_get);
                var difficulty = await Difficulties.GetDifficulty(difficulty_get);
                var character = await Characters.GetCharacter(character_get);
                var room = await Rooms.GetRoom(world[0]);

                try
                {
                    var placeholders = new Dictionary<string, object>
                    {
                        { "level", level },
                        { "room", room[room_get] },
                        { "world", world[0] },
                        { "world_icon_name", world[1] },
                        { "difficulty", difficulty },
                        { "character", character },
                        { "character_icon_name", character.ToLower() }
                    };
                    string details = updater.UpdateDetails("Kingdom Hearts Birth by Sleep Final Mix", placeholders);
                    string state = updater.UpdateState("Kingdom Hearts Birth by Sleep Final Mix", placeholders);
                    string largeasset = updater.UpdateLargeAsset("Kingdom Hearts Birth by Sleep Final Mix", placeholders);
                    string largeassettext = updater.UpdateLargeAssetText("Kingdom Hearts Birth by Sleep Final Mix", placeholders);
                    string smallasset = updater.UpdateSmallAsset("Kingdom Hearts Birth by Sleep Final Mix", placeholders);
                    string smallassettext = updater.UpdateSmallAssetText("Kingdom Hearts Birth by Sleep Final Mix", placeholders);
                    string button1text = updater.UpdateButton1Text("Kingdom Hearts Birth by Sleep Final Mix", placeholders);
                    string button2text = updater.UpdateButton2Text("Kingdom Hearts Birth by Sleep Final Mix", placeholders);
                    string button1url = updater.UpdateButton1URL("Kingdom Hearts Birth by Sleep Final Mix", placeholders);
                    string button2url = updater.UpdateButton2URL("Kingdom Hearts Birth by Sleep Final Mix", placeholders);
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
                    discord.UpdateState($"{room[0]}");
                    discord.UpdateDetails("");
                    discord.UpdateSmallAsset($"");
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
