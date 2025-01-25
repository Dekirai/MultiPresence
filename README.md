# MultiPresence
> [!WARNING]  
> The project is currently not actively maintained and updates are rather infrequent.

## About MultiPresence
MultiPresence is designed for games that don't use Discord's "Rich Presence" feature.  
It can be extended to a wide range of games, as long as it's not too complicated to read the game's memory.  
This means that online games, especially those with anti-cheat, are less likely to be added.

## Requirements  
- Windows 10/11 64 Bit
- [.NET 6 Runtime](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)

## How to use it
All you have to do is run **MultiPresence.exe** and you're ready to go.  
If a game is not showing properly on discord, try running MultiPresence as administrator.    

Using the context menu (right-click on the tray icon), you can easily open the config and blacklist file.  
You can also choose to launch MultiPresence with Windows, so you don't have to open it every time you switch on your PC.
 
## Blacklist (Optional)
MultiPresence also allows you to block certain games from being detected.  
You can use a file called "blacklist.json" to block games.  
A ready-made file and a tutorial can be found [here](https://github.com/Dekirai/MultiPresence/wiki/Blacklist).

## Placeholder (Optional)
You can customize your presence to your liking using a config.json  
A ready-made file and a tutorial can be found [here](https://github.com/Dekirai/MultiPresence/wiki/Config).
 
## Supported games
|Game|Platform|Notes|
|--|--|--|
|Call of Duty²|[Steam](https://store.steampowered.com/app/1938090/Call_of_Duty/)||
|Crisis Core -Final Fantasy VII- Reunion|[Steam](https://store.steampowered.com/app/1608070/CRISIS_CORE_FINAL_FANTASY_VII_REUNION/)||
|Final Fantasy VII Rebirth|[Steam](https://store.steampowered.com/app/2909400/FINAL_FANTASY_VII_REBIRTH/)||
|Final Fantasy VII Remake|[Steam](https://store.steampowered.com/app/1462040/FINAL_FANTASY_VII_REMAKE_INTERGRADE/)|Only base game tested|
|Final Fantasy XVI|[Steam](https://store.steampowered.com/app/2515020/FINAL_FANTASY_XVI/)|Only base game tested|
|Hogwarts Legacy²|[Steam](https://store.steampowered.com/app/990080/Hogwarts_Legacy/)||
|Kingdom Hearts Birth by Sleep Final Mix|[Steam](https://store.steampowered.com/app/2552430/KINGDOM_HEARTS_HD_1525_ReMIX/)||
|Kingdom Hearts Dream Drop Distance|[Steam](https://store.steampowered.com/app/2552440/KINGDOM_HEARTS_HD_28_Final_Chapter_Prologue/)||
|Kingdom Hearts Final Mix|[Steam](https://store.steampowered.com/app/2552430/KINGDOM_HEARTS_HD_1525_ReMIX/)||
|Kingdom Hearts II Final Mix|[Steam](https://store.steampowered.com/app/2552430/KINGDOM_HEARTS_HD_1525_ReMIX/)||
|Kingdom Hearts III¹|[Steam](https://store.steampowered.com/app/2552450/KINGDOM_HEARTS_III__Re_Mind_DLC/)||
|Marvel's Spider-Man Remastered|[Steam](https://store.steampowered.com/app/1817070/Marvels_SpiderMan_Remastered/)|Only base game tested|
|Marvel's Spider-Man: Miles Morales|[Steam](https://store.steampowered.com/app/1817190/Marvels_SpiderMan_Miles_Morales/)| |
|Mega Man 11|[Steam](https://store.steampowered.com/app/742300/Mega_Man_11/)| |
|Mega Man Battle Network 6|[Steam](https://store.steampowered.com/app/1798020/Mega_Man_Battle_Network_Legacy_Collection_Vol_2/)| |
|Overwatch 2²|[Steam](https://store.steampowered.com/app/2357570/Overwatch_2/)||
|Pangya Reborn|[Private Server](https://www.pangyareborn.com/)| |
|Resident Evil|[Steam](https://store.steampowered.com/app/304240/Resident_Evil/)| |
|Resident Evil 4 (2005)¹|[Steam](https://store.steampowered.com/app/254700/Resident_Evil_4/)|Currently broken|
|Resident Evil 5|[Steam](https://store.steampowered.com/app/21690/Resident_Evil_5/)| |
|Resident Evil 6|[Steam](https://store.steampowered.com/app/221040/Resident_Evil_6/)| |
|Resident Evil Revelations 2|[Steam](https://store.steampowered.com/app/287290/Resident_Evil_Revelations_2/)|Raid Mode only|
|Sonic Adventure 2|[Steam](https://store.steampowered.com/app/213610/Sonic_Adventure_2/)| |
|Streets of Rage Remake|[Windows](https://sorr.forumotion.net/t838-new-streets-of-rage-remake-v5-2-download-and-info)|Only game detection|
|Temtem: Swarm²|[Steam](https://store.steampowered.com/app/2510960/Temtem_Swarm/)||
|TY the Tasmanian Tiger|[Steam](https://store.steampowered.com/app/411960/TY_the_Tasmanian_Tiger/)| |
|Vampire Survivors|[Steam](https://store.steampowered.com/app/1794680/Vampire_Survivors/)| |
|Visions of Mana|[Steam](https://store.steampowered.com/app/2490990/Visions_of_Mana/)| |
|Zelda: The Wind Waker HD¹|[Cemu](https://wiki.cemu.info/wiki/The_Legend_of_Zelda:_The_Wind_Waker_HD)|Works on EUR and USA|
|Zelda: Twilight Princess HD¹|[Cemu](https://wiki.cemu.info/wiki/The_Legend_of_Zelda:_Twilight_Princess_HD)|Works on EUR and USA|

## Information
¹: Requires pattern search.  
²: Requires **MultiPresenceGame.exe** & **steam_api64.dll**.
> [!IMPORTANT]  
> A pattern search can cause a short lag on your machine because the program is currently scanning a lot of memory to find the right address to work with.  
> Depending on the game, it can take a long time to find the address, so be patient.  
<details>
<summary>CEMU</summary>
You have to disable the "Discord Presence" option found in Options -> General settings.<br />
</details>  

The presences are always expected to work on the latest version of the game.  
Games on lower versions will never be supported. They will always use the latest version from their game store.  
If a game receives a patch, I'll do my best to update it as soon as possible.       
