using DiscordRPC;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class P5X
    {
        private static DiscordRpcClient? discord;
        private static DiscordStatusUpdater? updater;
        private static string uid;
        public static void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("1390679033074942154");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/config/Persona 5 The Phantom X.json");
            Thread thread = new Thread(RPC);
            thread.Start();
        }

        private static void GetPID()
        {
            try
            {
                var _myProcess = Process.GetProcessesByName("p5x")[0];
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
            Process[] game = Process.GetProcessesByName("p5x");
            if (game.Length > 0)
            {
                try
                {
                    ulong _finduid;
                    _finduid = (ulong)Hypervisor.FindSignature("72 6F 6C 65 49 64 00 11 00 00 00 0B");

                    uid = Hypervisor.ReadString(_finduid + 0xC, 12, true);

                    if (uid.Length > 10)
                    {
                        var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                        PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Persona 5: The Phantom X", placeholders);
                    }
                    else
                    {
                        discord.SetPresence(new RichPresence()
                        {
                            Details = "In Main Menu",
                            State = "",
                            Assets = new Assets()
                            {
                                LargeImageKey = "logo",
                                LargeImageText = "Persona 5: The Phantom X"
                            },
                            Timestamps = PlaceholderHelper._startTimestamp
                        });
                    }
                }
                catch
                {
                    discord.SetPresence(new RichPresence()
                    {
                        Details = "In Main Menu",
                        State = "",
                        Assets = new Assets()
                        {
                            LargeImageKey = "logo",
                            LargeImageText = "Persona 5: The Phantom X"
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

        private static async Task<Dictionary<string, object>> GeneratePlaceholders()
        {
            return new Dictionary<string, object>
            {
                { "uid", uid }
            };
        }

        private static void InitializeDiscord()
        {
            discord.Initialize();
        }
    }
}