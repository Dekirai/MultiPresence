using DiscordRPC;
using MultiPresence.Models.KHDDD;
using System;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class KHDDD
    {
        private static DiscordRpcClient? discord;
        private static DiscordStatusUpdater? updater;
        public static void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("906904369151213629");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("config.json");
            Thread thread = new Thread(RPC);
            thread.Start();
        }

        private static void GetPID()
        {
            try
            {
                var _myProcess = Process.GetProcessesByName("KINGDOM HEARTS Dream Drop Distance")[0];
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
            Process[] game = Process.GetProcessesByName("KINGDOM HEARTS Dream Drop Distance");
            if (game.Length > 0)
            {
                int world_get = Hypervisor.Read<byte>(0xA40764);
                int room_get = Hypervisor.Read<byte>(0xA40765);
                int difficulty_get = Hypervisor.Read<byte>(0xA40762);
                int character_get = Hypervisor.Read<byte>(0xA40760);
                int level = Hypervisor.Read<byte>(0xA98034);

                var world = await Worlds.GetWorld(world_get);
                var difficulty = await Difficulties.GetDifficulty(difficulty_get);
                var room = await Rooms.GetRoom(world[0]);
                string character = "";

                if (character_get == 0)
                    character = "Sora";
                else
                    character = "Riku";

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
                        { "character_icon_name", character_get }
                    };
                    string details = updater.UpdateDetails("Kingdom Hearts Dream Drop Distance", placeholders);
                    string state = updater.UpdateState("Kingdom Hearts Dream Drop Distance", placeholders);
                    string largeasset = updater.UpdateLargeAsset("Kingdom Hearts Dream Drop Distance", placeholders);
                    string largeassettext = updater.UpdateLargeAssetText("Kingdom Hearts Dream Drop Distance", placeholders);
                    string smallasset = updater.UpdateSmallAsset("Kingdom Hearts Dream Drop Distance", placeholders);
                    string smallassettext = updater.UpdateSmallAssetText("Kingdom Hearts Dream Drop Distance", placeholders);
                    string button1text = updater.UpdateButton1Text("Kingdom Hearts Dream Drop Distance", placeholders);
                    string button2text = updater.UpdateButton2Text("Kingdom Hearts Dream Drop Distance", placeholders);
                    string button1url = updater.UpdateButton1URL("Kingdom Hearts Dream Drop Distance", placeholders);
                    string button2url = updater.UpdateButton2URL("Kingdom Hearts Dream Drop Distance", placeholders);
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
                    var placeholders = new Dictionary<string, object>
                    {
                        { "level", level },
                        { "difficulty", difficulty },
                        { "world", world[0] },
                        { "world_icon_name", world[1] },
                        { "room", room[room_get] }
                    };
                    string details = updater.UpdateDetails("Kingdom Hearts Dream Drop Distance", placeholders);
                    string state = updater.UpdateState("Kingdom Hearts Dream Drop Distance", placeholders);
                    string largeasset = updater.UpdateLargeAsset("Kingdom Hearts Dream Drop Distance", placeholders);
                    string largeassettext = updater.UpdateLargeAssetText("Kingdom Hearts Dream Drop Distance", placeholders);
                    string smallasset = updater.UpdateSmallAsset("Kingdom Hearts Dream Drop Distance", placeholders);
                    string smallassettext = updater.UpdateSmallAssetText("Kingdom Hearts Dream Drop Distance", placeholders);
                    string button1text = updater.UpdateButton1Text("Kingdom Hearts Dream Drop Distance", placeholders);
                    string button2text = updater.UpdateButton2Text("Kingdom Hearts Dream Drop Distance", placeholders);
                    string button1url = updater.UpdateButton1URL("Kingdom Hearts Dream Drop Distance", placeholders);
                    string button2url = updater.UpdateButton2URL("Kingdom Hearts Dream Drop Distance", placeholders);
                    discord.UpdateLargeAsset(largeasset, largeassettext);
                    discord.UpdateSmallAsset(smallasset, smallassettext);
                    discord.UpdateDetails(details);

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
                    discord.UpdateState($"{room[0]}");
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
