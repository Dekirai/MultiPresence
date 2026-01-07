# MultiPresence 🎮

[![Discord](https://img.shields.io/discord/1343222000008560700?label=Join%20Discord&logo=discord&color=7289DA)](https://discord.gg/AC6rVgV2Jj) [![License](https://img.shields.io/github/license/Dekirai/MultiPresence)](LICENSE) [![Release](https://img.shields.io/github/v/release/Dekirai/MultiPresence)](https://github.com/Dekirai/MultiPresence/releases)

> **MultiPresence** enables Discord Rich Presence for games that don’t natively support it.

---

## ✨ Features

- **Universal support** for games without native Rich Presence  
- **Extensible** via memory readers and configs  
- **Blacklist** unwanted games  
- **Custom placeholders** for personalized presence text  
- **Tray integration** for easy access and autostart  

---

## ⚙️ Requirements

- **OS:** Windows 10/11 (64-bit)  
- **Runtime:** [.NET 9 Desktop Runtime](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)  

---

## 🚀 Installation

1. Download the latest release from [GitHub Releases](https://github.com/Dekirai/MultiPresence/releases).  
2. Extract `MultiPresence.zip` to a folder of your choice.  
3. Ensure [.NET 9 Desktop Runtime](https://dotnet.microsoft.com/en-us/download/dotnet/9.0) is installed.  

---

## ▶️ Quick Start

1. Run **`MultiPresence.exe`**.  
2. If your game doesn’t show up, try **"Run as administrator"**.  
3. Right-click the tray icon to open:  
   - **Config** (edit your placeholders)  
   - **Autostart** (launch on Windows startup)  

---

## 🛠 Configuration

### Placeholders

Customize your presence text:  
1. Download the latest [Config.zip](https://github.com/Dekirai/MultiPresence/raw/refs/heads/main/Config.zip)
2. Paste the "Config" folder into the "Assets" folder
3. Find the JSON of your game and open it with any Editor
4. See the [Placeholder Guide](https://github.com/Dekirai/MultiPresence/wiki/How-to-use-placeholders) for full details.  

<details>
<summary>📹 Placeholder Demo (Outdated Version)</summary>

[![Placeholder Demo](https://img.youtube.com/vi/x0avvmvQ5BQ/0.jpg)](https://www.youtube.com/watch?v=x0avvmvQ5BQ)

</details>

---

## 🎮 Supported Games

> **Note:** Supported platforms have been confirmed; games may work on other platforms.

<details>
<summary>View Game List</summary>

| Game                                        | Platform                                                                                                                         | Notes                                |
|---------------------------------------------|----------------------------------------------------------------------------------------------------------------------------------|--------------------------------------|
| Call of Duty ²                              | [Steam](https://store.steampowered.com/app/1938090/Call_of_Duty/)                                                                |                                      |
| Code Vein                                   | [Steam](https://store.steampowered.com/app/678960/CODE_VEIN/)                                                                    | Only base game tested                |
| Crash Bandicoot 4: It's About Time          | [Steam](https://store.steampowered.com/app/1378990/Crash_Bandicoot_4_Its_About_Time/)                                            |                                      |
| Crash Bandicoot N. Sane Trilogy             | [Steam](https://store.steampowered.com/app/731490/Crash_Bandicoot_N_Sane_Trilogy/)                                               | Supports all games                   |
| Crisis Core -Final Fantasy VII- Reunion     | [Steam](https://store.steampowered.com/app/1608070/CRISIS_CORE_FINAL_FANTASY_VII_REUNION/)                                       |                                      |
| Dark Souls Remastered                       | [Steam](https://store.steampowered.com/app/570940/DARK_SOULS_REMASTERED/)                                                        |                                      |
| Dark Souls II SotFS                         | [Steam](https://store.steampowered.com/app/335300/DARK_SOULS_II_Scholar_of_the_First_Sin/)                                       |                                      |
| Dark Souls III                              | [Steam](https://store.steampowered.com/app/374320/DARK_SOULS_III/)                                                               |                                      |
| Death Stranding Director's Cut              | [Steam](https://store.steampowered.com/app/1850570/DEATH_STRANDING_DIRECTORS_CUT/)                                               |                                      |
| Devil May Cry HD Collection                 | [Steam](https://store.steampowered.com/app/631510/Devil_May_Cry_HD_Collection/)                                                  | Supports all games                   |
| Devil May Cry 4                             | [Steam](https://store.steampowered.com/app/329050/Devil_May_Cry_4_Special_Edition/)                                              |                                      |
| Devil May Cry 5                             | [Steam](https://store.steampowered.com/app/601150/Devil_May_Cry_5/)                                                              |                                      |
| Digimon Story Time Stranger                 | [Steam](https://store.steampowered.com/app/1984270/Digimon_Story_Time_Stranger/)                                                 |                                      |
| DmC Devil May Cry                           | [Steam](https://store.steampowered.com/app/220440/DmC_Devil_May_Cry/)                                                            | Only base game tested                |
| Elden Ring                                  | [Steam](https://store.steampowered.com/app/1245620/ELDEN_RING/)                                                                  | Only with EAC disabled               |
| Final Fantasy VII Rebirth                   | [Steam](https://store.steampowered.com/app/2909400/FINAL_FANTASY_VII_REBIRTH/)                                                   |                                      |
| Final Fantasy VII Remake                    | [Steam](https://store.steampowered.com/app/1462040/FINAL_FANTASY_VII_REMAKE_INTERGRADE/)                                         | Only base game tested                |
| Final Fantasy X                             | [Steam](https://store.steampowered.com/app/359870/FINAL_FANTASY_XX2_HD_Remaster/)                                                | Enter a new room to make it work     |
| Final Fantasy XV                            | [Steam](https://store.steampowered.com/app/637650/FINAL_FANTASY_XV_WINDOWS_EDITION/)                                             |                                      |
| Final Fantasy XVI                           | [Steam](https://store.steampowered.com/app/2515020/FINAL_FANTASY_XVI/)                                                           | Only base game tested                |
| Granblue Fantasy: Relink                    | [Steam](https://store.steampowered.com/app/881020/Granblue_Fantasy_Relink/)                                                      |                                      |
| Gunfire Reborn ²                            | [Steam](https://store.steampowered.com/app/1217060/Gunfire_Reborn/)                                                              |                                      |
| Hatsune Miku: Project DIVA Mega Mix+        | [Steam](https://store.steampowered.com/app/1761390/Hatsune_Miku_Project_DIVA-Mega-Mix/)                                          |                                      |
| Hello Kitty Island Adventure ²              | [Steam](https://store.steampowered.com/app/2495100/Hello_Kitty_Island_Adventure/)                                                |                                      |
| Hogwarts Legacy ²                           | [Steam](https://store.steampowered.com/app/990080/Hogwarts_Legacy/)                                                              |                                      |
| Kingdom Hearts Birth by Sleep Final Mix     | [Steam](https://store.steampowered.com/app/2552430/KINGDOM_HEARTS_HD_1525_ReMIX/)                                                |                                      |
| Kingdom Hearts Dream Drop Distance          | [Steam](https://store.steampowered.com/app/2552440/KINGDOM_HEARTS_HD_28_Final_Chapter_Prologue/)                                 |                                      |
| Kingdom Hearts Re:Chain of Memories         | [Steam](https://store.steampowered.com/app/2552430/KINGDOM_HEARTS_HD_1525_ReMIX/)                                                |                                      |
| Kingdom Hearts Final Mix                    | [Steam](https://store.steampowered.com/app/2552430/KINGDOM_HEARTS_HD_1525_ReMIX/) & EGS                                          |                                      |
| Kingdom Hearts II Final Mix                 | [Steam](https://store.steampowered.com/app/2552430/KINGDOM_HEARTS_HD_1525_ReMIX/) & EGS                                          |                                      |
| Kingdom Hearts III ¹                        | [Steam](https://store.steampowered.com/app/2552450/KINGDOM_HEARTS_III__Re_Mind_DLC/)                                             |                                      |
| Labyrinthine ²                              | [Steam](https://store.steampowered.com/app/1302240/Labyrinthine/)                                                                |                                      |
| Lies of P                                   | [Steam](https://store.steampowered.com/app/1627720/Lies_of_P/)                                                                   |                                      |
| Marvel's Spider-Man Remastered              | [Steam](https://store.steampowered.com/app/1817070/Marvels_SpiderMan_Remastered/)                                                | Only base game tested                |
| Marvel's Spider-Man: Miles Morales          | [Steam](https://store.steampowered.com/app/1817190/Marvels_SpiderMan_Miles_Morales/)                                             |                                      |
| Mega Man 11                                 | [Steam](https://store.steampowered.com/app/742300/Mega_Man_11/)                                                                  |                                      |
| Mega Man Battle Network Collection Vol. 1   | [Steam](https://store.steampowered.com/app/1798010/Mega_Man_Battle_Network_Legacy_Collection_Vol_1/)                             | Supports all games                   |
| Mega Man Battle Network Collection Vol. 2   | [Steam](https://store.steampowered.com/app/1798020/Mega_Man_Battle_Network_Legacy_Collection_Vol_2/)                             | Supports all games                   |
| Mega Man X Legacy Collection 2              | [Steam](https://store.steampowered.com/app/743900/Mega_Man_X_Legacy_Collection_2/)                                               | Supports all games, no "X Challenge" |
| Overwatch 2 ²                               | [Steam](https://store.steampowered.com/app/2357570/Overwatch_2/)                                                                 |                                      |
| Pangya Reborn                               | [Private Server](https://www.pangyareborn.com/)                                                                                  |                                      |
| Persona 4 Golden                            | [Steam](https://store.steampowered.com/app/1113000/Persona_4_Golden/)                                                            |                                      |
| Persona 5 Strikers                          | [Steam](https://store.steampowered.com/app/1382330/Persona_5_Strikers/)                                                          |                                      |
| Persona 5 Royal                             | [Steam](https://store.steampowered.com/app/1687950/Persona_5_Royal/)                                                             |                                      |
| Persona 5: The Phantom X                    | [Steam](https://store.steampowered.com/app/3061570/Persona5_The_Phantom_X/)                                                      | Only displays your UID               |
| Resident Evil 1 Remake                      | [Steam](https://store.steampowered.com/app/304240/Resident_Evil/)                                                                |                                      |
| Resident Evil 2 Remake                      | [Steam](https://store.steampowered.com/app/883710/Resident_Evil_2/)                                                              |                                      |
| Resident Evil 4 ¹                           | [Steam](https://store.steampowered.com/app/254700/Resident_Evil_4/)                                                              |                                      |
| Resident Evil 4 Remake                      | [Steam](https://store.steampowered.com/app/2050650/Resident_Evil_4/)                                                             |                                      |
| Resident Evil 5                             | [Steam](https://store.steampowered.com/app/21690/Resident_Evil_5/)                                                               |                                      |
| Resident Evil 6                             | [Steam](https://store.steampowered.com/app/221040/Resident_Evil_6/)                                                              |                                      |
| Resident Evil 7                             | [Steam](https://store.steampowered.com/app/418370/Resident_Evil_7_Biohazard/)                                                    |                                      |
| Resident Evil 8                             | [Steam](https://store.steampowered.com/app/1196590/Resident_Evil_Village/)                                                       |                                      |
| Resident Evil Revelations 2                 | [Steam](https://store.steampowered.com/app/287290/Resident_Evil_Revelations_2/)                                                  | Raid Mode only                       |
| Scott Pilgrim vs The World                  | [Steam](https://store.steampowered.com/app/2215260/Scott_Pilgrim_vs_The_World_The_Game__Complete_Edition/)                       |                                      |
| Sonic Adventure DX                          | [Steam](https://store.steampowered.com/app/71250/Sonic_Adventure_DX/)                                                            | Probably won't work with mods        |
| Sonic Adventure 2                           | [Steam](https://store.steampowered.com/app/213610/Sonic_Adventure_2/)                                                            |                                      |
| Sonic x Shadow Generations                  | [Steam](https://store.steampowered.com/app/2513280/SONIC_X_SHADOW_GENERATIONS/)                                                  | Only Sonic Generations for now       |
| Stellar Blade ²                             | [Steam](https://store.steampowered.com/app/3489700/Stellar_Blade/)                                                               |                                      |
| Team Fortress 2 ²                           | [Steam](https://store.steampowered.com/app/440/Team_Fortress_2/)                                                                 |                                      |
| Temtem: Swarm ²                             | [Steam](https://store.steampowered.com/app/2510960/Temtem_Swarm/)                                                                |                                      |
| The Witcher 3: Wild Hunt                    | [Steam](https://store.steampowered.com/app/292030/The_Witcher_3_Wild_Hunt/)                                                      | Only base game tested                |
| TY the Tasmanian Tiger                      | [Steam](https://store.steampowered.com/app/411960/TY_the_Tasmanian_Tiger/)                                                       |                                      |
| Visions of Mana                             | [Steam](https://store.steampowered.com/app/2490990/Visions_of_Mana/)                                                             |                                      |
| Ys I Chronicles+                            | [Steam](https://store.steampowered.com/app/223810/Ys_I__II_Chronicles/)                                                          |                                      |
| Zelda: The Wind Waker HD ¹                  | [Cemu](https://wiki.cemu.info/wiki/The_Legend_of_Zelda:_The_Wind-Waker_HD)                                                       | Works on EUR and USA                 |
| Zelda: Twilight Princess HD ¹               | [Cemu](https://wiki.cemu.info/wiki/The_Legend_of_Zelda:_Twilight-Princess_HD)                                                    | Works on EUR and USA                 |

</details>

---

## Information

¹: Requires pattern search.  
²: Requires **MultiPresenceGame.exe** & **steam_api64.dll**.

> [!IMPORTANT]  
> A pattern search can cause a short lag on your machine because the program is currently scanning a lot of memory to find the right address to work with.  
> Depending on the game, it can take a long time to find the address, so be patient.

<details>
<summary>CEMU</summary>
You have to disable the "Discord Presence" option found in Options -> General settings.  
</details>

The presences are always expected to work on the latest version of the game.  
Games on lower versions will never be supported on purpose. They will always use the latest version from their game store.  
If a game receives a patch, I'll do my best to update it as soon as possible.

---

## 📄 License

Distributed under the MIT License. See [LICENSE](LICENSE) for more information.
