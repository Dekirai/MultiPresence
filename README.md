# MultiPresence 🎮

[![Discord](https://img.shields.io/discord/1343222000008560700?label=Join%20Discord&logo=discord&color=7289DA)](https://discord.gg/AC6rVgV2Jj) [![License](https://img.shields.io/github/license/Dekirai/MultiPresence)](LICENSE) [![Release](https://img.shields.io/github/v/release/Dekirai/MultiPresence)](https://github.com/Dekirai/MultiPresence/releases)

> **MultiPresence** enables Discord Rich Presence for games that don’t natively support it. Lightweight, extensible, and community-driven.

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
   - **Blacklist** (exclude games)  
   - **Autostart** (launch on Windows startup)  

---

## 🛠 Configuration

### Blacklist

Prevent specific games from being detected:  
1. Edit `blacklist.json` via the tray menu.  
2. Follow the [Blacklist Guide](https://github.com/Dekirai/MultiPresence/wiki/Blacklist).  

### Placeholders

Customize your presence text:  
1. Open your game’s config from the tray menu.  
2. Use placeholders like `{player}`, `{level}`, etc.  
3. See the [Placeholder Guide](https://github.com/Dekirai/MultiPresence/wiki/How-to-use-placeholders) for full details.  

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
| Crisis Core -Final Fantasy VII- Reunion     | [Steam](https://store.steampowered.com/app/1608070/CRISIS_CORE_FINAL_FANTASY_VII_REUNION/)                                       |                                      |
| Dark Souls Remastered                       | [Steam](https://store.steampowered.com/app/570940/DARK_SOULS_REMASTERED/)                                                        |                                      |
| Dark Souls II SotFS                         | [Steam](https://store.steampowered.com/app/335300/DARK_SOULS_II_Scholar_of_the_First_Sin/)                                       |                                      |
| Dark Souls III                              | [Steam](https://store.steampowered.com/app/374320/DARK_SOULS_III/)                                                               |                                      |
| Devil May Cry HD Collection                 | [Steam](https://store.steampowered.com/app/631510/Devil_May_Cry_HD_Collection/)                                                  | Supports all games                   |
| Devil May Cry 4                             | [Steam](https://store.steampowered.com/app/329050/Devil_May_Cry_4_Special_Edition/)                                              |                                      |
| Devil May Cry 5                             | [Steam](https://store.steampowered.com/app/601150/Devil_May_Cry_5/)                                                              |                                      |
| DmC Devil May Cry                           | [Steam](https://store.steampowered.com/app/220440/DmC_Devil_May_Cry/)                                                            | Only base game tested                |
| Elden Ring                                  | [Steam](https://store.steampowered.com/app/1245620/ELDEN_RING/)                                                                  | Only with EAC disabled               |
| Final Fantasy VII Rebirth                   | [Steam](https://store.steampowered.com/app/2909400/FINAL_FANTASY_VII_REBIRTH/)                                                   |                                      |
| Final Fantasy VII Remake                    | [Steam](https://store.steampowered.com/app/1462040/FINAL_FANTASY_VII_REMAKE_INTERGRADE/)                                         | Only base game tested                |
| Final Fantasy XV                            | [Steam](https://store.steampowered.com/app/637650/FINAL_FANTASY_XV_WINDOWS_EDITION/)                                             |                                      |
| Final Fantasy XVI                           | [Steam](https://store.steampowered.com/app/2515020/FINAL_FANTASY_XVI/)                                                           | Only base game tested                |
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
| Marvel's Spider-Man 2                       | [Steam](https://store.steampowered.com/app/2651280/Marvels_SpiderMan_2/)                                                         | See this [release](https://github.com/Dekirai/MultiPresence/releases/tag/10.02.2025) for notes |
| Marvel's Spider-Man: Miles Morales          | [Steam](https://store.steampowered.com/app/1817190/Marvels_SpiderMan_Miles_Morales/)                                             |                                      |
| Mega Man 11                                 | [Steam](https://store.steampowered.com/app/742300/Mega_Man_11/)                                                                  |                                      |
| Mega Man Battle Network 6                   | [Steam](https://store.steampowered.com/app/1798020/Mega_Man_Battle_Network_Legacy_Collection_Vol_2/)                             |                                      |
| Overwatch 2 ²                               | [Steam](https://store.steampowered.com/app/2357570/Overwatch_2/)                                                                 |                                      |
| Pangya Reborn                               | [Private Server](https://www.pangyareborn.com/)                                                                                  |                                      |
| Persona 4 Golden                            | [Steam](https://store.steampowered.com/app/1113000/Persona_4_Golden/)                                                            |                                      |
| Persona 5 Strikers                          | [Steam](https://store.steampowered.com/app/1382330/Persona_5_Strikers/)                                                          |                                      |
| Resident Evil                               | [Steam](https://store.steampowered.com/app/304240/Resident_Evil/)                                                                |                                      |
| Resident Evil 2                             | [Steam](https://store.steampowered.com/app/883710/Resident_Evil_2/)                                                              |                                      |
| Resident Evil 4 (2005) ¹                    | [Steam](https://store.steampowered.com/app/254700/Resident_Evil_4/)                                                              |                                      |
| Resident Evil 4 Remake                      | [Steam](https://store.steampowered.com/app/2050650/Resident_Evil_4/)                                                             |                                      |
| Resident Evil 5                             | [Steam](https://store.steampowered.com/app/21690/Resident_Evil_5/)                                                               |                                      |
| Resident Evil 6                             | [Steam](https://store.steampowered.com/app/221040/Resident_Evil_6/)                                                              |                                      |
| Resident Evil Revelations 2                 | [Steam](https://store.steampowered.com/app/287290/Resident_Evil_Revelations_2/)                                                  | Raid Mode only                       |
| Scott Pilgrim vs The World                  | [Steam](https://store.steampowered.com/app/2215260/Scott_Pilgrim_vs_The_World_The_Game__Complete_Edition/)                       |                                      |
| Sonic Adventure DX                          | [Steam](https://store.steampowered.com/app/71250/Sonic_Adventure_DX/)                                                            | Probably won't work with mods        |
| Sonic Adventure 2                           | [Steam](https://store.steampowered.com/app/213610/Sonic_Adventure_2/)                                                            |                                      |
| Stellar Blade ²                             | [Steam](https://store.steampowered.com/app/3489700/Stellar_Blade/)                                                               |                                      |
| Temtem: Swarm ²                             | [Steam](https://store.steampowered.com/app/2510960/Temtem_Swarm/)                                                                |                                      |
| The Binding of Isaac: Rebirth               | [Steam](https://store.steampowered.com/app/250900/The_Binding_of_Isaac_Rebirth/)                                                 | Only works on Repentance+            |
| The Witcher 3: Wild Hunt                    | [Steam](https://store.steampowered.com/app/292030/The_Witcher_3_Wild_Hunt/)                                                      | Only base game tested                |
| TY the Tasmanian Tiger                      | [Steam](https://store.steampowered.com/app/411960/TY_the_Tasmanian_Tiger/)                                                       |                                      |
| Vampire Survivors                           | [Steam](https://store.steampowered.com/app/1794680/Vampire_Survivors/)                                                           |                                      |
| Visions of Mana                             | [Steam](https://store.steampowered.com/app/2490990/Visions_of_Mana/)                                                             |                                      |
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
Games on lower versions will never be supported. They will always use the latest version from their game store.  
If a game receives a patch, I'll do my best to update it as soon as possible.

---

## 📄 License

Distributed under the MIT License. See [LICENSE](LICENSE) for more information.
