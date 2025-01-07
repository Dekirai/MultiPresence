using DiscordRPC;
using MultiPresence.Models.CCFFVII;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class CCFFVII
    {

        private static DiscordRpcClient? discord;
        private static DiscordStatusUpdater? updater;
        public static void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("1271522314248523837");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("config.json");
            Thread thread = new Thread(RPC);
            thread.Start();
        }

        private static void GetPID()
        {
            try
            {
                var _myProcess = Process.GetProcessesByName("CCFF7R-Win64-Shipping")[0];
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
            Process[] game = Process.GetProcessesByName("CCFF7R-Win64-Shipping");
            if (game.Length > 0)
            {
                int level = Hypervisor.Read<byte>(0x71B3F36);
                int gil = Hypervisor.Read<int>(0x71B3FB0);
                int difficulty_get = Hypervisor.Read<byte>(0x71B3FD7);

                var difficulty = await Difficulties.GetDifficulty(difficulty_get);

                int hp = Hypervisor.Read<int>(0x71B3F10);
                int maxhp = Hypervisor.Read<int>(0x71B3F14);
                int mp = Hypervisor.Read<int>(0x71B3F1C);
                int maxmp = Hypervisor.Read<int>(0x71B3F20);
                int ap = Hypervisor.Read<int>(0x71B3F28);
                int maxap = Hypervisor.Read<int>(0x71B3F2C);

                int hp_mission = Hypervisor.Read<int>(0x718DC28);
                int maxhp_mission = Hypervisor.Read<int>(0x718DC2C);
                int mp_mission = Hypervisor.Read<int>(0x718DC30);
                int maxmp_mission = Hypervisor.Read<int>(0x718DC34);
                int ap_mission = Hypervisor.Read<int>(0x718DC38);
                int maxap_mission = Hypervisor.Read<int>(0x718DC3C);

                var placeholders = new Dictionary<string, object>
                {
                    { "level", level },
                    { "gil", gil },
                    { "difficulty", difficulty },
                    { "hp", hp },
                    { "maxhp", maxhp },
                    { "mp", mp },
                    { "maxmp", maxmp },
                    { "ap", ap },
                    { "maxap", maxap },
                    { "hp_mission", hp_mission },
                    { "maxhp_mission", maxhp_mission },
                    { "mp_mission", mp_mission },
                    { "maxmp_mission", maxmp_mission },
                    { "ap_mission", ap_mission },
                    { "maxap_mission", maxap_mission }
                };

                if (hp_mission > 0)
                {
                    string details = updater.UpdateDetails("CRISIS CORE –FINAL FANTASY VII– REUNION", placeholders, "Mission");
                    string state = updater.UpdateState("CRISIS CORE –FINAL FANTASY VII– REUNION", placeholders, "Mission");
                    string largeasset = updater.UpdateLargeAsset("CRISIS CORE –FINAL FANTASY VII– REUNION", placeholders, "Mission");
                    string largeassettext = updater.UpdateLargeAssetText("CRISIS CORE –FINAL FANTASY VII– REUNION", placeholders, "Mission");
                    string smallasset = updater.UpdateSmallAsset("CRISIS CORE –FINAL FANTASY VII– REUNION", placeholders, "Mission");
                    string smallassettext = updater.UpdateSmallAssetText("CRISIS CORE –FINAL FANTASY VII– REUNION", placeholders, "Mission");
                    string button1text = updater.UpdateButton1Text("CRISIS CORE –FINAL FANTASY VII– REUNION", placeholders, "Mission");
                    string button2text = updater.UpdateButton2Text("CRISIS CORE –FINAL FANTASY VII– REUNION", placeholders, "Mission");
                    string button1url = updater.UpdateButton1URL("CRISIS CORE –FINAL FANTASY VII– REUNION", placeholders, "Mission");
                    string button2url = updater.UpdateButton2URL("CRISIS CORE –FINAL FANTASY VII– REUNION", placeholders, "Mission");
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
                else
                {
                    string details = updater.UpdateDetails("CRISIS CORE –FINAL FANTASY VII– REUNION", placeholders);
                    string state = updater.UpdateState("CRISIS CORE –FINAL FANTASY VII– REUNION", placeholders);
                    string largeasset = updater.UpdateLargeAsset("CRISIS CORE –FINAL FANTASY VII– REUNION", placeholders);
                    string largeassettext = updater.UpdateLargeAssetText("CRISIS CORE –FINAL FANTASY VII– REUNION", placeholders);
                    string smallasset = updater.UpdateSmallAsset("CRISIS CORE –FINAL FANTASY VII– REUNION", placeholders);
                    string smallassettext = updater.UpdateSmallAssetText("CRISIS CORE –FINAL FANTASY VII– REUNION", placeholders);
                    string button1text = updater.UpdateButton1Text("CRISIS CORE –FINAL FANTASY VII– REUNION", placeholders);
                    string button2text = updater.UpdateButton2Text("CRISIS CORE –FINAL FANTASY VII– REUNION", placeholders);
                    string button1url = updater.UpdateButton1URL("CRISIS CORE –FINAL FANTASY VII– REUNION", placeholders);
                    string button2url = updater.UpdateButton2URL("CRISIS CORE –FINAL FANTASY VII– REUNION", placeholders);
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