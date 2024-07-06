

# MultiPresence
Let your friends see what you are currently doing in your active game!

## About MultiPresence
MultiPresence was made for games that aren't officially supported by Discord's "Rich Presence" feature.  
It can be expanded to tons of games that allows me to easily read the process' memory.  
This means online games, especially with an anti-cheat, will most likely not be added here.

## Requirements  
- Windows 10/11 64 Bit
- Administrator rights to read the memory
- [.NET 6 Runtime](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)

## How to use it
All you gotta do is to run the exe and you are ready to go.    

To run the program with windows on startup, please follow these steps:

 1. Create a new file named MultiPresence.vbs
 2. Find your "StartUp" Folder (**C:\Users\YOURUSERNAME\AppData\Roaming\Microsoft\Windows\Start Menu\Programs\Startup**)
 3. Place the created file in the StartUp Folder
 4. Open the file with an Editor
 5. Paste this code into it:  
```
Set WshShell = CreateObject("WScript.Shell" )
WshShell.Run """C:\Path\To\MultiPresence\MultiPresence.exe""", 0 'Must quote command if it has spaces; must escape quotes
Set WshShell = Nothing
```
6. Modify the Path to the location where you placed MultiPresence.exe
 
 That's it! Now on every windows startup, the program will automatically launch!    
 
## Blacklist (Optional)
Multipresence also allows you to block certain games from being detected.  
As of release 1.6.0, you can use a file named "blacklist.json" to block games.  
A downloable premade file and a tutorial can be found [here](https://github.com/Dekirai/MultiPresence/wiki/Blacklist).
 
## Supported games
|Game|Platform|Updated|Information|
|--|--|--|--|
|[Kingdom Hearts Birth by Sleep Final Mix](https://github.com/Dekirai/MultiPresence/wiki/Kingdom-Hearts-Birth-by-Sleep-Final-Mix)|[Steam](https://store.steampowered.com/app/2552430/KINGDOM_HEARTS_HD_1525_ReMIX/)|23/06/2024|Only works on Steam|
|[Kingdom Hearts Dream Drop Distance](https://github.com/Dekirai/MultiPresence/wiki/Kingdom-Hearts-Dream-Drop-Distance)|[Steam](https://store.steampowered.com/app/2552440/KINGDOM_HEARTS_HD_28_Final_Chapter_Prologue/)|24/06/2024|Only works on Steam|
|[Kingdom Hearts Final Mix](https://github.com/Dekirai/MultiPresence/wiki/Kingdom-Hearts-Final-Mix)|[Steam](https://store.steampowered.com/app/2552430/KINGDOM_HEARTS_HD_1525_ReMIX/)|23/06/2024|Only works on Steam|
|[Kingdom Hearts II Final Mix](https://github.com/Dekirai/MultiPresence/wiki/Kingdom-Hearts-II-Final-Mix)|[Steam](https://store.steampowered.com/app/2552430/KINGDOM_HEARTS_HD_1525_ReMIX/)|23/06/2024|Only works on Steam|
|[Kingdom Hearts III*](https://github.com/Dekirai/MultiPresence/wiki/Kingdom-Hearts-III)|[Steam](https://store.steampowered.com/app/2552450/KINGDOM_HEARTS_III__Re_Mind_DLC/)|06/07/2024|Only works on Steam|
|[Mega Man 11](https://github.com/Dekirai/MultiPresence/wiki/Mega-Man-11)|[Steam](https://store.steampowered.com/app/742300/Mega_Man_11/)|03/06/2022| |
|[Mega Man BN6: Cybeast Gregar](https://github.com/Dekirai/MultiPresence)|[Steam](https://store.steampowered.com/app/1798020/Mega_Man_Battle_Network_Legacy_Collection_Vol_2/)|30/06/2024|Work in Progress|
|[Mega Man X: Command Mission](https://github.com/Dekirai/MultiPresence)|[PCSX2 AVX2 QT](https://wiki.pcsx2.net/Mega_Man_X:_Command_Mission)|25/01/2023|Works on PAL & USA|
|[Pangya Reborn](https://github.com/Dekirai/MultiPresence/wiki/Pangya-Reborn)|[Private Server](https://www.pangyareborn.com/)|14/04/2024| |
|[Ratchet: Deadlocked](https://github.com/Dekirai/MultiPresence)|[PCSX2 AVX2 QT](https://wiki.pcsx2.net/Ratchet:_Deadlocked)|25/01/2023|Works only on USA|
|[Resident Evil](https://github.com/Dekirai/MultiPresence/wiki/Resident-Evil)|[Steam](https://store.steampowered.com/app/304240/Resident_Evil/)|28/02/2024| |
|[Resident Evil 4 (2005)*](https://github.com/Dekirai/MultiPresence/wiki/Resident-Evil-4)|[Steam](https://store.steampowered.com/app/254700/Resident_Evil_4/)|28/02/2024| |
|[Resident Evil 5](https://github.com/Dekirai/MultiPresence/wiki/Resident-Evil-5)|[Steam](https://store.steampowered.com/app/21690/Resident_Evil_5/)|27/02/2024| |
|[Resident Evil 6](https://github.com/Dekirai/MultiPresence/wiki/Resident-Evil-6)|[Steam](https://store.steampowered.com/app/221040/Resident_Evil_6/)|28/02/2024| |
|[Resident Evil Revelations 2](https://github.com/Dekirai/MultiPresence/wiki/Resident-Evil-Revelations-2)|[Steam](https://store.steampowered.com/app/287290/Resident_Evil_Revelations_2/)|06/03/2024| Raid Mode only|
|[Sonic Adventure 2](https://github.com/Dekirai/MultiPresence/wiki/Sonic-Adventure-2)|[Steam](https://store.steampowered.com/app/213610/Sonic_Adventure_2/)|04/06/2022| |
|[TY the Tasmanian Tiger](https://github.com/Dekirai/MultiPresence/wiki/TY-the-Tasmanian-Tiger)|[Steam](https://store.steampowered.com/app/411960/TY_the_Tasmanian_Tiger/)|04/06/2022| |
|[Zelda: The Wind Waker HD*](https://github.com/Dekirai/MultiPresence/wiki/Zelda:-The-Wind-Waker-HD)|[Cemu](https://wiki.cemu.info/wiki/The_Legend_of_Zelda:_The_Wind_Waker_HD)|30/06/2024|Works on EUR and USA|
|[Zelda: Twilight Princess HD*](https://github.com/Dekirai/MultiPresence/wiki/Zelda:-Twilight-Princess-HD)|[Cemu](https://wiki.cemu.info/wiki/The_Legend_of_Zelda:_Twilight_Princess_HD)|01/07/2024|Only works on EUR|

## Information

<details>
<summary>CEMU</summary>
You have to disable the "Discord Presence" option found in Options -> General settings.<br />
</details>
<details>
<summary>PCSX2</summary>
MultiPresence trys to fetch the current game from the Window title, so make sure it has access to it.<br />
You have to disable the "Discord Presence" option found in Settings -> Interface<br />
You have to disable the "Render to Separate Window" option found in Settings -> Interface<br />
Make sure to download GetProcAddressEx.dll aswell and place it in the same directory as MultiPresence.exe<br />
</details>

The presences are always expected to work on the latest version of the respective game.  
Games on lower versions are never supported on purpose. It will always use the very latest version from their game store.  
If a game receives a patch, I'll try my best to update it's presence as quickly as possible if needed.       

Click on a games name to view it's wiki containing all features and previews.  
Date format for updates is **dd/mm/yyyy**.   

Some games will **require a memory scan** because the addresses aren't static. This mostly occurs for games on emulators like Cemu or Unity/Unreal Engine games.    

A memory scan might lead to a short lag on your machine because the program is currently scanning a ton of memory to find the proper address to work with.  
If a games name is marked with a * at the end, it means it uses memory scan, just so you know.
