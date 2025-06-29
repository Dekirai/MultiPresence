using DiscordRPC;
using MultiPresence.Models.RE2;
using System;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Security.AccessControl;

namespace MultiPresence.Presence
{
    public class P4G
    {
        private static DiscordRpcClient? discord;
        private static DiscordStatusUpdater? updater;
        public static void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("1388911702321664031");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/config/Persona 4 Golden.json");
            Thread thread = new Thread(RPC);
            thread.Start();
        }

        private static void GetPID()
        {
            try
            {
                var _myProcess = Process.GetProcessesByName("p4g")[0];
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
            Process[] game = Process.GetProcessesByName("p4g");
            if (game.Length > 0)
            {
                ulong _base = 0x51BCD70;
                int inbattle = Hypervisor.Read<byte>(0xECA358);

                if (inbattle == 1)
                {
                    var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholdersBattle);
                    PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Persona 4 Golden", placeholders, "Battle");
                }
                else
                {
                    var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                    PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Persona 4 Golden", placeholders);
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
            ulong _base = 0x51BCD70;
            ulong _timebase = _base + 0x18DC;
            ulong _protagonistbase = _base + 0xA36;

            int currentime_get = Hypervisor.Read<byte>(_timebase + 0x02);
            short currentday_get = Hypervisor.Read<short>(_timebase + 0x00);
            int level = Hypervisor.Read<byte>(_protagonistbase + 0x244);

            string currenttime = currentime_get switch
            {
                0 => "Early Morning",
                1 => "Morning",
                2 => "Lunch",
                3 => "Afternoon",
                4 => "After School",
                5 => "Evening",
                6 => "Night"
            };

            #region Currentday list
            string currentday = currentday_get switch
            {
                8 => "04/09 (Sat)",
                9 => "04/10 (Sun)",
                10 => "04/11 (Mon)",
                11 => "04/12 (Tue)",
                12 => "04/13 (Wed)",
                13 => "04/14 (Thu)",
                14 => "04/15 (Fri)",
                15 => "04/16 (Sat)",
                16 => "04/17 (Sun)",
                17 => "04/18 (Mon)",
                18 => "04/19 (Tue)",
                19 => "04/20 (Wed)",
                20 => "04/21 (Thu)",
                21 => "04/22 (Fri)",
                22 => "04/23 (Sat)",
                23 => "04/24 (Sun)",
                24 => "04/25 (Mon)",
                25 => "04/26 (Tue)",
                26 => "04/27 (Wed)",
                27 => "04/28 (Thu)",
                28 => "04/29 (Fri)",
                29 => "04/30 (Sat)",
                30 => "05/01 (Sun)",
                31 => "05/02 (Mon)",
                32 => "05/03 (Tue)",
                33 => "05/04 (Wed)",
                34 => "05/05 (Thu)",
                35 => "05/06 (Fri)",
                36 => "05/07 (Sat)",
                37 => "05/08 (Sun)",
                38 => "05/09 (Mon)",
                39 => "05/10 (Tue)",
                40 => "05/11 (Wed)",
                41 => "05/12 (Thu)",
                42 => "05/13 (Fri)",
                43 => "05/14 (Sat)",
                44 => "05/15 (Sun)",
                45 => "05/16 (Mon)",
                46 => "05/17 (Tue)",
                47 => "05/18 (Wed)",
                48 => "05/19 (Thu)",
                49 => "05/20 (Fri)",
                50 => "05/21 (Sat)",
                51 => "05/22 (Sun)",
                52 => "05/23 (Mon)",
                53 => "05/24 (Tue)",
                54 => "05/25 (Wed)",
                55 => "05/26 (Thu)",
                56 => "05/27 (Fri)",
                57 => "05/28 (Sat)",
                58 => "05/29 (Sun)",
                59 => "05/30 (Mon)",
                60 => "05/31 (Tue)",
                61 => "06/01 (Wed)",
                62 => "06/02 (Thu)",
                63 => "06/03 (Fri)",
                64 => "06/04 (Sat)",
                65 => "06/05 (Sun)",
                66 => "06/06 (Mon)",
                67 => "06/07 (Tue)",
                68 => "06/08 (Wed)",
                69 => "06/09 (Thu)",
                70 => "06/10 (Fri)",
                71 => "06/11 (Sat)",
                72 => "06/12 (Sun)",
                73 => "06/13 (Mon)",
                74 => "06/14 (Tue)",
                75 => "06/15 (Wed)",
                76 => "06/16 (Thu)",
                77 => "06/17 (Fri)",
                78 => "06/18 (Sat)",
                79 => "06/19 (Sun)",
                80 => "06/20 (Mon)",
                81 => "06/21 (Tue)",
                82 => "06/22 (Wed)",
                83 => "06/23 (Thu)",
                84 => "06/24 (Fri)",
                85 => "06/25 (Sat)",
                86 => "06/26 (Sun)",
                87 => "06/27 (Mon)",
                88 => "06/28 (Tue)",
                89 => "06/29 (Wed)",
                90 => "06/30 (Thu)",
                91 => "07/01 (Fri)",
                92 => "07/02 (Sat)",
                93 => "07/03 (Sun)",
                94 => "07/04 (Mon)",
                95 => "07/05 (Tue)",
                96 => "07/06 (Wed)",
                97 => "07/07 (Thu)",
                98 => "07/08 (Fri)",
                99 => "07/09 (Sat)",
                100 => "07/10 (Sun)",
                101 => "07/11 (Mon)",
                102 => "07/12 (Tue)",
                103 => "07/13 (Wed)",
                104 => "07/14 (Thu)",
                105 => "07/15 (Fri)",
                106 => "07/16 (Sat)",
                107 => "07/17 (Sun)",
                108 => "07/18 (Mon)",
                109 => "07/19 (Tue)",
                110 => "07/20 (Wed)",
                111 => "07/21 (Thu)",
                112 => "07/22 (Fri)",
                113 => "07/23 (Sat)",
                114 => "07/24 (Sun)",
                115 => "07/25 (Mon)",
                116 => "07/26 (Tue)",
                117 => "07/27 (Wed)",
                118 => "07/28 (Thu)",
                119 => "07/29 (Fri)",
                120 => "07/30 (Sat)",
                121 => "07/31 (Sun)",
                122 => "08/01 (Mon)",
                123 => "08/02 (Tue)",
                124 => "08/03 (Wed)",
                125 => "08/04 (Thu)",
                126 => "08/05 (Fri)",
                127 => "08/06 (Sat)",
                128 => "08/07 (Sun)",
                129 => "08/08 (Mon)",
                130 => "08/09 (Tue)",
                131 => "08/10 (Wed)",
                132 => "08/11 (Thu)",
                133 => "08/12 (Fri)",
                134 => "08/13 (Sat)",
                135 => "08/14 (Sun)",
                136 => "08/15 (Mon)",
                137 => "08/16 (Tue)",
                138 => "08/17 (Wed)",
                139 => "08/18 (Thu)",
                140 => "08/19 (Fri)",
                141 => "08/20 (Sat)",
                142 => "08/21 (Sun)",
                143 => "08/22 (Mon)",
                144 => "08/23 (Tue)",
                145 => "08/24 (Wed)",
                146 => "08/25 (Thu)",
                147 => "08/26 (Fri)",
                148 => "08/27 (Sat)",
                149 => "08/28 (Sun)",
                150 => "08/29 (Mon)",
                151 => "08/30 (Tue)",
                152 => "08/31 (Wed)",
                153 => "09/01 (Thu)",
                154 => "09/02 (Fri)",
                155 => "09/03 (Sat)",
                156 => "09/04 (Sun)",
                157 => "09/05 (Mon)",
                158 => "09/06 (Tue)",
                159 => "09/07 (Wed)",
                160 => "09/08 (Thu)",
                161 => "09/09 (Fri)",
                162 => "09/10 (Sat)",
                163 => "09/11 (Sun)",
                164 => "09/12 (Mon)",
                165 => "09/13 (Tue)",
                166 => "09/14 (Wed)",
                167 => "09/15 (Thu)",
                168 => "09/16 (Fri)",
                169 => "09/17 (Sat)",
                170 => "09/18 (Sun)",
                171 => "09/19 (Mon)",
                172 => "09/20 (Tue)",
                173 => "09/21 (Wed)",
                174 => "09/22 (Thu)",
                175 => "09/23 (Fri)",
                176 => "09/24 (Sat)",
                177 => "09/25 (Sun)",
                178 => "09/26 (Mon)",
                179 => "09/27 (Tue)",
                180 => "09/28 (Wed)",
                181 => "09/29 (Thu)",
                182 => "09/30 (Fri)",
                183 => "10/01 (Sat)",
                184 => "10/02 (Sun)",
                185 => "10/03 (Mon)",
                186 => "10/04 (Tue)",
                187 => "10/05 (Wed)",
                188 => "10/06 (Thu)",
                189 => "10/07 (Fri)",
                190 => "10/08 (Sat)",
                191 => "10/09 (Sun)",
                192 => "10/10 (Mon)",
                193 => "10/11 (Tue)",
                194 => "10/12 (Wed)",
                195 => "10/13 (Thu)",
                196 => "10/14 (Fri)",
                197 => "10/15 (Sat)",
                198 => "10/16 (Sun)",
                199 => "10/17 (Mon)",
                200 => "10/18 (Tue)",
                201 => "10/19 (Wed)",
                202 => "10/20 (Thu)",
                203 => "10/21 (Fri)",
                204 => "10/22 (Sat)",
                205 => "10/23 (Sun)",
                206 => "10/24 (Mon)",
                207 => "10/25 (Tue)",
                208 => "10/26 (Wed)",
                209 => "10/27 (Thu)",
                210 => "10/28 (Fri)",
                211 => "10/29 (Sat)",
                212 => "10/30 (Sun)",
                213 => "10/31 (Mon)",
                214 => "11/01 (Tue)",
                215 => "11/02 (Wed)",
                216 => "11/03 (Thu)",
                217 => "11/04 (Fri)",
                218 => "11/05 (Sat)",
                219 => "11/06 (Sun)",
                220 => "11/07 (Mon)",
                221 => "11/08 (Tue)",
                222 => "11/09 (Wed)",
                223 => "11/10 (Thu)",
                224 => "11/11 (Fri)",
                225 => "11/12 (Sat)",
                226 => "11/13 (Sun)",
                227 => "11/14 (Mon)",
                228 => "11/15 (Tue)",
                229 => "11/16 (Wed)",
                230 => "11/17 (Thu)",
                231 => "11/18 (Fri)",
                232 => "11/19 (Sat)",
                233 => "11/20 (Sun)",
                234 => "11/21 (Mon)",
                235 => "11/22 (Tue)",
                236 => "11/23 (Wed)",
                237 => "11/24 (Thu)",
                238 => "11/25 (Fri)",
                239 => "11/26 (Sat)",
                240 => "11/27 (Sun)",
                241 => "11/28 (Mon)",
                242 => "11/29 (Tue)",
                243 => "11/30 (Wed)",
                244 => "12/01 (Thu)",
                245 => "12/02 (Fri)",
                246 => "12/03 (Sat)",
                247 => "12/04 (Sun)",
                248 => "12/05 (Mon)",
                249 => "12/06 (Tue)",
                250 => "12/07 (Wed)",
                251 => "12/08 (Thu)",
                252 => "12/09 (Fri)",
                253 => "12/10 (Sat)",
                254 => "12/11 (Sun)",
                255 => "12/12 (Mon)",
                256 => "12/13 (Tue)",
                257 => "12/14 (Wed)",
                258 => "12/15 (Thu)",
                259 => "12/16 (Fri)",
                260 => "12/17 (Sat)",
                261 => "12/18 (Sun)",
                262 => "12/19 (Mon)",
                263 => "12/20 (Tue)",
                264 => "12/21 (Wed)",
                265 => "12/22 (Thu)",
                266 => "12/23 (Fri)",
                267 => "12/24 (Sat)",
                268 => "12/25 (Sun)",
                269 => "12/26 (Mon)",
                270 => "12/27 (Tue)",
                271 => "12/28 (Wed)",
                272 => "12/29 (Thu)",
                273 => "12/30 (Fri)",
                274 => "12/31 (Sat)",
                275 => "01/01 (Sun)",
                276 => "01/02 (Mon)",
                277 => "01/03 (Tue)",
                278 => "01/04 (Wed)",
                279 => "01/05 (Thu)",
                280 => "01/06 (Fri)",
                281 => "01/07 (Sat)",
                282 => "01/08 (Sun)",
                283 => "01/09 (Mon)",
                284 => "01/10 (Tue)",
                285 => "01/11 (Wed)",
                286 => "01/12 (Thu)",
                287 => "01/13 (Fri)",
                288 => "01/14 (Sat)",
                289 => "01/15 (Sun)",
                290 => "01/16 (Mon)",
                291 => "01/17 (Tue)",
                292 => "01/18 (Wed)",
                293 => "01/19 (Thu)",
                294 => "01/20 (Fri)",
                295 => "01/21 (Sat)",
                296 => "01/22 (Sun)",
                297 => "01/23 (Mon)",
                298 => "01/24 (Tue)",
                299 => "01/25 (Wed)",
                300 => "01/26 (Thu)",
                301 => "01/27 (Fri)",
                302 => "01/28 (Sat)",
                303 => "01/29 (Sun)",
                304 => "01/30 (Mon)",
                305 => "01/31 (Tue)",
                306 => "02/01 (Wed)",
                307 => "02/02 (Thu)",
                308 => "02/03 (Fri)",
                309 => "02/04 (Sat)",
                310 => "02/05 (Sun)",
                311 => "02/06 (Mon)",
                312 => "02/07 (Tue)",
                313 => "02/08 (Wed)",
                314 => "02/09 (Thu)",
                315 => "02/10 (Fri)",
                316 => "02/11 (Sat)",
                317 => "02/12 (Sun)",
                318 => "02/13 (Mon)",
                319 => "02/14 (Tue)",
                320 => "02/15 (Wed)",
                321 => "02/16 (Thu)",
                322 => "02/17 (Fri)",
                323 => "02/18 (Sat)",
                324 => "02/19 (Sun)",
                325 => "02/20 (Mon)",
                326 => "02/21 (Tue)",
                327 => "02/22 (Wed)",
                328 => "02/23 (Thu)",
                329 => "02/24 (Fri)",
                330 => "02/25 (Sat)",
                331 => "02/26 (Sun)",
                332 => "02/27 (Mon)",
                333 => "02/28 (Tue)",
                334 => "03/01 (Wed)",
                335 => "03/02 (Thu)",
                336 => "03/03 (Fri)",
                337 => "03/04 (Sat)",
                338 => "03/05 (Sun)",
                339 => "03/06 (Mon)",
                340 => "03/07 (Tue)",
                341 => "03/08 (Wed)",
                342 => "03/09 (Thu)",
                343 => "03/10 (Fri)",
                344 => "03/11 (Sat)",
                345 => "03/12 (Sun)",
                346 => "03/13 (Mon)",
                347 => "03/14 (Tue)",
                348 => "03/15 (Wed)",
                349 => "03/16 (Thu)",
                350 => "03/17 (Fri)",
                351 => "03/18 (Sat)",
                352 => "03/19 (Sun)",
                353 => "03/20 (Mon)",
                354 => "03/21 (Tue)",
                355 => "03/22 (Wed)",
                356 => "03/23 (Thu)",
                357 => "03/24 (Fri)",
                358 => "03/25 (Sat)",
                359 => "03/26 (Sun)",
                360 => "03/27 (Mon)",
                361 => "03/28 (Tue)",
                362 => "03/29 (Wed)",
                363 => "03/30 (Thu)",
                364 => "03/31 (Fri)",
                365 => "04/01 (Sat)",
                366 => "04/02 (Sun)",
                367 => "04/03 (Mon)",
                368 => "04/04 (Tue)",
                369 => "04/05 (Wed)",
                370 => "04/06 (Thu)",
                371 => "04/07 (Fri)"
            };
            #endregion

            return new Dictionary<string, object>
            {
                { "currenttime", currenttime },
                { "currentday", currentday },
                { "money", _base },
                { "level", level }
            };
        }

        private static async Task<Dictionary<string, object>> GeneratePlaceholdersBattle()
        {
            ulong _base = 0x51BCD70;
            ulong _protagonistbase = _base + 0xA36;
            ulong _battlehp = _base + 0xD00;
            ulong _battlesp = _base + 0xD02;

            int hp = Hypervisor.Read<short>(_battlehp - 0x84);
            int sp = Hypervisor.Read<short>(_battlesp - 0x84);
            int level = Hypervisor.Read<byte>(_protagonistbase + 0x244);

            return new Dictionary<string, object>
            {
                { "hp", hp },
                { "sp", sp },
                { "level", level }
            };
        }

        private static void InitializeDiscord()
        {
            discord.Initialize();
        }
    }
}