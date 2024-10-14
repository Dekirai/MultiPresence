using DiscordRPC;
using Memory;
using MultiPresence.Models.PYRE;
using System.Diagnostics;
using static System.Formats.Asn1.AsnWriter;
using Button = DiscordRPC.Button;

namespace MultiPresence.Presence
{
    public class PYRE
    {
        static Mem mem = new Mem();
        static string process = "ProjectG";
        private static DiscordRpcClient discord;
        private static DiscordStatusUpdater updater;
        public static async void DoAction()
        {
            await Task.Delay(20000);
            GetPID();
            discord = new DiscordRpcClient("1226462236181004338");
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
                string nickname = mem.ReadString($"{process}.exe+00A7D6A4,0x5AC");
                int stage_get = mem.ReadByte($"{process}.exe+A47E2C,0x10950");
                int mode_get = mem.ReadByte($"{process}.exe+A47E2C,0x1094C");
                int level_get = mem.ReadByte($"{process}.exe+00A7D6A4,0x711");
                int room = mem.ReadByte($"{process}.exe+A47E2C,0x1094D");
                int players = mem.ReadByte($"{process}.exe+A47E2C,0x10938");
                int playersmax = mem.ReadByte($"{process}.exe+A47E2C,0x10937");
                int isIngame = mem.ReadByte($"{process}.exe+A47E2C,0x10B27");
                int currenthole = mem.ReadByte($"{process}.exe+A47E2C,0xFFEC");
                int maxholes = mem.ReadByte($"{process}.exe+A47E2C,0x1094B");
                int isOpen = mem.ReadByte($"{process}.exe+A47E2C,0x10934");                

                var stage = await Stages.GetStage(stage_get);
                var mode = await Modes.GetMode(mode_get);
                var level = await Levels.GetLevel(level_get);

                if (mode_get == 23 || mode_get == 255 || mode_get == 40)
                {
                    var placeholders = new Dictionary<string, object>
                    {
                        { "nickname", nickname },
                        { "level", level[0] },
                        { "room", room },
                        { "map", stage[0] },
                        { "map_icon_name", stage_get },
                        { "players", players },
                        { "playersmax", playersmax },
                        { "currenthole", currenthole },
                        { "maxholes", maxholes },
                        { "mode", mode[0] }
                    };
                    discord.UpdateLargeAsset($"logo", $"Pangya Reborn");
                    string details = updater.UpdateDetails("Pangya Reborn", placeholders, "Lobby");
                    string state = updater.UpdateState("Pangya Reborn", placeholders, "Lobby");
                    discord.UpdateDetails(details);
                    discord.UpdateState(state);
                    discord.UpdateSmallAsset("", "");
                }
                else
                {
                    if (isIngame == 1)
                    {
                        discord.UpdateLargeAsset($"{stage_get}", $"{stage[0]}");
                        if (isOpen == 1)
                            discord.UpdateSmallAsset($"playing", $"Room #{room} — {players}/{playersmax} Players");
                        else
                            discord.UpdateSmallAsset($"playing", $"Private Room — {players}/{playersmax} Players");
                        if (mode_get == 0 || mode_get == 1 || mode_get == 7 || mode_get == 10)
                        {
                            var placeholders = new Dictionary<string, object>
                            {
                                { "nickname", nickname },
                                { "level", level[0] },
                                { "room", room },
                                { "map", stage[0] },
                                { "map_icon_name", stage_get },
                                { "players", players },
                                { "playersmax", playersmax },
                                { "currenthole", currenthole },
                                { "maxholes", maxholes },
                                { "mode", mode[0] }
                             };
                            string details = updater.UpdateDetails("Pangya Reborn", placeholders, "Ingame_Match");
                            string state = updater.UpdateState("Pangya Reborn", placeholders, "Ingame_Match");
                            discord.UpdateDetails(details);
                            discord.UpdateState(state);
                        }
                        else if (mode_get == 4)
                        {

                            int score = mem.ReadInt("ProjectG.exe+00B006E8,0x0,0x40,0x18,0x0,0x14,0xC8,0x4F4");
                            var placeholders = new Dictionary<string, object>
                            {
                                { "nickname", nickname },
                                { "level", level[0] },
                                { "room", room },
                                { "map", stage[0] },
                                { "map_icon_name", stage_get },
                                { "players", players },
                                { "playersmax", playersmax },
                                { "currenthole", currenthole },
                                { "maxholes", maxholes },
                                { "score", score },
                                { "mode", mode[0] }
                            };
                            string details = updater.UpdateDetails("Pangya Reborn", placeholders, "Ingame_Tourney");
                            string state = updater.UpdateState("Pangya Reborn", placeholders, "Ingame_Tourney");
                            discord.UpdateDetails(details);
                            discord.UpdateState(state);
                        }
                        else if (mode_get == 2)
                        {
                            var placeholders = new Dictionary<string, object>
                            {
                                { "nickname", nickname },
                                { "level", level[0] },
                                { "room", room },
                                { "map", stage[0] },
                                { "map_icon_name", stage_get },
                                { "players", players },
                                { "playersmax", playersmax },
                                { "currenthole", currenthole },
                                { "maxholes", maxholes },
                                { "mode", mode[0] }
                             };
                            string details = updater.UpdateDetails("Pangya Reborn", placeholders, "Ingame_Lounge");
                            string state = updater.UpdateState("Pangya Reborn", placeholders, "Ingame_Lounge");
                            discord.UpdateDetails(details);
                            discord.UpdateState(state);
                        }
                        else
                        {
                            discord.UpdateDetails($"{nickname} - {level[0]}");
                            discord.UpdateState($"{mode[0]}");
                        }
                    }
                    else
                    {
                        if (mode_get == 2)
                        {
                            var placeholders = new Dictionary<string, object>
                            {
                                { "nickname", nickname },
                                { "level", level[0] },
                                { "room", room },
                                { "map", stage[0] },
                                { "map_icon_name", stage_get },
                                { "players", players },
                                { "playersmax", playersmax },
                                { "currenthole", currenthole },
                                { "maxholes", maxholes },
                                { "mode", mode[0] }
                             };
                            discord.UpdateLargeAsset($"logo", $"Pangya Reborn");
                            string details = updater.UpdateDetails("Pangya Reborn", placeholders, "Lobby");
                            string state = updater.UpdateState("Pangya Reborn", placeholders, "Lobby");
                            discord.UpdateDetails(details);
                            discord.UpdateState(state);
                            discord.UpdateSmallAsset("", "");
                        }
                        else
                        {
                            var placeholders = new Dictionary<string, object>
                            {
                                { "nickname", nickname },
                                { "level", level[0] },
                                { "room", room },
                                { "map", stage[0] },
                                { "map_icon_name", stage_get },
                                { "players", players },
                                { "playersmax", playersmax },
                                { "currenthole", currenthole },
                                { "maxholes", maxholes },
                                { "mode", mode[0] }
                             };
                            string details = updater.UpdateDetails("Pangya Reborn", placeholders, "Waiting_Room");
                            string state = updater.UpdateState("Pangya Reborn", placeholders, "Waiting_Room");
                            discord.UpdateDetails(details);
                            discord.UpdateState(state);
                            discord.UpdateLargeAsset($"{stage_get}", $"{stage[0]}");
                            if (isOpen == 1)
                                discord.UpdateSmallAsset($"waiting", $"Room #{room} — {players}/{playersmax} Players");
                            else
                                discord.UpdateSmallAsset($"waiting", $"Private Room — {players}/{playersmax} Players");
                        }
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
                Buttons = new Button[]
                {
                    new Button() { Label = $"Website", Url = "https://www.pangyareborn.com/" },
                    new Button() { Label = $"Discord", Url = "https://www.discord.com/invite/D9gC9Hhpss" }
                },
                Timestamps = new Timestamps()
                {
                    Start = DateTime.UtcNow.AddSeconds(1)
                }
            });
        }
    }
}