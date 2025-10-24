using DiscordRPC;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class DSTS
    {
        private static DiscordRpcClient? discord;
        private static DiscordStatusUpdater? updater;
        public static void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("1430566850659487884");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/config/Digimon Story Time Stranger.json");
            Thread thread = new Thread(RPC);
            thread.Start();
        }

        private static void GetPID()
        {
            try
            {
                var _myProcess = Process.GetProcessesByName("Digimon Story Time Stranger")[0];
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
            Process[] game = Process.GetProcessesByName("Digimon Story Time Stranger");
            if (game.Length > 0)
            {
                string _nickname = Hypervisor.ReadString(Hypervisor.GetPointer64(0x01C32B40, [0x40, 0x10]), 4, true);

                try
                {
                    if (_nickname != "name")
                    {
                        var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                        PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Digimon Story Time Stranger", placeholders);
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
                                LargeImageText = "Digimon Story Time Stranger"
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
                            LargeImageText = "Digimon Story Time Stranger"
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
                updater.Dispose();
                MainForm.gameUpdater.Start();
            }
        }

        private static async Task<Dictionary<string, object>> GeneratePlaceholders()
        {
            string _nickname = Hypervisor.ReadString(Hypervisor.GetPointer64(0x01C32B40, [0x40, 0x10]), 16, true);
            int _yen = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x01C32B40, [0x40, 0x58]), true);
            int _agentlevel = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x01C32B40, [0x40, 0x64]), true);
            int _locationget = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x01C32B40, [0x40, 0x1F0]), true);
            int _difficultyget = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x01C32B40, [0x40, 0x80]), true);
            int _bodytypeget = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x01C32B40, [0x40, 0x1D0]), true);

            string difficulty = _difficultyget switch
            {
                1 => "Story",
                2 => "Balanced",
                3 => "Hard",
                4 => "Mega",
                5 => "Mega+",
                _ => "Unknown"
            };

            string bodytype = _bodytypeget switch
            {
                0 => "Male",
                1 => "Female",
                _ => "Unknown"
            };

            string location = _locationget switch
            {
                0 => "No Text",
                1 => "Character Select",
                101 => "Higashi-Shinjuku: Vision Square",
                102 => "Higashi-Shinjuku: Koshu-Kaido Intersection",
                103 => "Shinjuku: Kabukicho, Theater Square",
                104 => "Shinjuku Underground Town: Subroad",
                105 => "Marunonaka Line: Shinjuku Station",
                106 => "Nakazawa Gourment Coffee",
                107 => "Wall of Hope",
                108 => "Seibu-Shinjuku: Overbridge Intersection",
                109 => "Shinjuku: A Certain Back Alley",
                110 => "Shinjuku Station: East Gate",
                111 => "Shinjuku Underground: Promenade",
                201 => "Central Town",
                202 => "Central Town: Downtown",
                203 => "Central Town: Shanty",
                204 => "Warrior's Watering Hole",
                205 => "Central Isle",
                206 => "Central Tower: Entrance",
                207 => "Central Tower: Steel Maze",
                208 => "Central Tower: Steel Periphery",
                209 => "Central Tower: Steel Maze",
                210 => "Central Tower: Steel Point",
                211 => "Central Town: Shanty",
                212 => "Central Town: Shanty",
                213 => "Central Town: Shanty",
                301 => "Tokyo Metropolitan Government Building: Exterior",
                302 => "Shinjuku Park: Waterfall Plaza",
                303 => "Shinjuku Park: Community Forest",
                304 => "Nishi-Shinjuku: Tocho Chuo-dori Avenue",
                401 => "Gear Forest: Floating Island Plains",
                402 => "Gear Forest: Floating Rock Islands",
                403 => "Gear Forest: Shaded Woods",
                404 => "Gear Forest Village",
                405 => "Gear Forest: Shade Tree Pier",
                406 => "Gear Forest: Sanctuary",
                407 => "Gear Forest: Deep Woods",
                501 => "Factorial Town",
                502 => "Factorial Area",
                503 => "Factorial Tunnel: Mines",
                504 => "Factorial Tunnel: Mines near Core",
                505 => "Factorial Core",
                506 => "Paradise Colosseum",
                507 => "Paradise Colosseum: First Ring",
                508 => "Paradise Colosseum: Second Ring",
                509 => "Paradise Colosseum: Second Ring",
                510 => "Paradise Colosseum: Accessway",
                511 => "Paradise Colosseum: Accessway",
                512 => "Paradise Colosseum: Accessway",
                513 => "Paradise Colosseum: Finals Ring",
                601 => "Terminal Otherside",
                602 => "Palace Otherside",
                603 => "Palace Otherside: West Passage",
                604 => "Palace Otherside: East Passage",
                605 => "Palace Otherside: Throne Room",
                606 => "Guardian Terminal",
                607 => "Guardian Palace",
                608 => "Guardian Palace: Throne ROom",
                609 => "Guardian Rail",
                701 => "Dark Crevasse",
                702 => "Dark Garden",
                703 => "Dark Castle",
                801 => "Temple of Beginnings: Upper Level",
                802 => "Temple of Beginnings: Shrine Room",
                901 => "Akashic Hades",
                902 => "Akashic Chaos",
                903 => "Akashic Forest",
                904 => "Akashic Abyss",
                905 => "Akashic Oblivion",
                906 => "Akashic Ragnarok",
                907 => "Akashic Records: Innermost Depths",
                1001 => "Forgotten Rails",
                1002 => "Giant Pit",
                1003 => "Forgotten Rails",
                1101 => "Shinjuku Underground Waterway: North Block",
                1102 => "Shinjuku Underground Waterway: South Block",
                1103 => "Shinjuku Underground Reservoir",
                1104 => "Shinjuku Underground Waterway: Depths",
                1201 => "Shinjuku Underground Utility Conduit",
                1202 => "Shinjuku Underground Waterway: Lower Level",
                1203 => "No Text",
                1204 => "Rebellion Village",
                1205 => "Rebellion Village: Merukimon's Room",
                1206 => "Rebellion Village: Dr. Simmons' Room",
                1301 => "Special Forces Underground Base: Research Division",
                1302 => "Sepcial Forces Underground Base: Restriced Area",
                1303 => "Special Forces Underground Base: Laboratory",
                1304 => "Special Forces Underground Base: Gantry",
                1305 => "Underground Cable Maintenance Conduit",
                1401 => "Tokyo Metropolitan Government Building: Entrance",
                1403 => "Tokyo Metropolitan Government Building: 36F North Tower",
                1405 => "Tokyo Metropolitan Government Building: 24F South Wing",
                1406 => "Tokyo Metropolitan Government Building: 36F South Tower",
                1407 => "Tokyo Metropolitan Government Building: Roof",
                3002 => "Theatres",
                3003 => "Theatres",
                _ => "Unknown"
            };


            return new Dictionary<string, object>
            {
                { "nickname", _nickname },
                { "yen", _yen },
                { "level", _agentlevel },
                { "location", location },
                { "difficulty", difficulty },
                { "bodytype", bodytype }
            };
        }

        private static void InitializeDiscord()
        {
            discord.Initialize();
        }
    }
}