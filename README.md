

# MultiPresence
Let your friends see what you are currently doing in your active game!      

(Server is still in building phase)  
![Discord Banner](https://discord.com/api/guilds/1269723725528432712/widget.png?style=banner2)

## About MultiPresence
MultiPresence is designed for games that don't use Discord's "Rich Presence" feature.  
It can be extended to a wide range of games, as long as it's not too complicated to read the game's memory.  
This means that online games, especially those with anti-cheat, are less likely to be added.

## Requirements  
- Windows 10/11 64 Bit
- Administrator rights to read the memory
- [.NET 6 Runtime](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)

## How to use it
All you have to do is run the exe and you're ready to go.    

To run the program with Windows on startup, please follow these steps

 1. Create a new file called MultiPresence.vbs.
 2. Locate your StartUp folder (**C:\Users\YOURUSERNAME\AppData\Roaming\Microsoft\Windows\Start Menu\Programs\Startup**)
 3. Place the created file in the StartUp folder.
 4. Open the file with a text editor
 5. Paste this code into it:  
```
Set WshShell = CreateObject("WScript.Shell" )
WshShell.Run """C:\Path\To\MultiPresence\MultiPresence.exe""", 0 'Must quote command if it has spaces; must escape quotes
Set WshShell = Nothing
```
6. Change the path to the location where you placed MultiPresence.exe.
 
 That's it! Now every time you start Windows, the program will start automatically!    
 
## Blacklist (Optional)
MultiPresence also allows you to block certain games from being detected.  
As of version 1.6.0, you can use a file called "blacklist.json" to block games.  
A ready-made file and a tutorial can be found [here](https://github.com/Dekirai/MultiPresence/wiki/Blacklist).

## Placeholder (Optional)
Introduced in Release 20, you can customize your presence to your liking using a config.json  
A ready-made file and a tutorial can be found [here](https://github.com/Dekirai/MultiPresence/blob/main/Placeholders/%5BPRE-MADE%20CONFIG%5D.md).
 
## Supported games
|Game|Platform|Information|
|--|--|--|
|[asobiSW](https://github.com/Dekirai/MultiPresence/)|[Private Server](https://playasobi.gg/)| |
|[Kingdom Hearts Birth by Sleep Final Mix](https://github.com/Dekirai/MultiPresence/wiki/Kingdom-Hearts-Birth-by-Sleep-Final-Mix)|[Steam](https://store.steampowered.com/app/2552430/KINGDOM_HEARTS_HD_1525_ReMIX/)|Only works on Steam|
|[Kingdom Hearts Dream Drop Distance](https://github.com/Dekirai/MultiPresence/wiki/Kingdom-Hearts-Dream-Drop-Distance)|[Steam](https://store.steampowered.com/app/2552440/KINGDOM_HEARTS_HD_28_Final_Chapter_Prologue/)|Only works on Steam|
|[Kingdom Hearts Final Mix](https://github.com/Dekirai/MultiPresence/wiki/Kingdom-Hearts-Final-Mix)|[Steam](https://store.steampowered.com/app/2552430/KINGDOM_HEARTS_HD_1525_ReMIX/)|Only works on Steam|
|[Kingdom Hearts II Final Mix](https://github.com/Dekirai/MultiPresence/wiki/Kingdom-Hearts-II-Final-Mix)|[Steam](https://store.steampowered.com/app/2552430/KINGDOM_HEARTS_HD_1525_ReMIX/)|Only works on Steam|
|[Kingdom Hearts III¹](https://github.com/Dekirai/MultiPresence/wiki/Kingdom-Hearts-III)|[Steam](https://store.steampowered.com/app/2552450/KINGDOM_HEARTS_III__Re_Mind_DLC/)|Only works on Steam|
|[Marvel's Spider-Man Remastered](https://github.com/Dekirai/MultiPresence/wiki/Marvel's-Spider%E2%80%90Man-Remastered)|[Steam](https://store.steampowered.com/app/1817070/Marvels_SpiderMan_Remastered/)| |
|[Marvel's Spider-Man: Miles Morales](https://github.com/Dekirai/MultiPresence/wiki/Marvel's-Spider%E2%80%90Man:-Miles-Morales)|[Steam](https://store.steampowered.com/app/1817190/Marvels_SpiderMan_Miles_Morales/)| |
|[Mega Man 11](https://github.com/Dekirai/MultiPresence/wiki/Mega-Man-11)|[Steam](https://store.steampowered.com/app/742300/Mega_Man_11/)| |
|[Mega Man Battle Network 6](https://github.com/Dekirai/MultiPresence/wiki/Mega-Man-Battle-Network-6)|[Steam](https://store.steampowered.com/app/1798020/Mega_Man_Battle_Network_Legacy_Collection_Vol_2/)| |
|[Pangya Reborn](https://github.com/Dekirai/MultiPresence/wiki/Pangya-Reborn)|[Private Server](https://www.pangyareborn.com/)| |
|[Resident Evil](https://github.com/Dekirai/MultiPresence/wiki/Resident-Evil)|[Steam](https://store.steampowered.com/app/304240/Resident_Evil/)| |
|[Resident Evil 4 (2005)¹](https://github.com/Dekirai/MultiPresence/wiki/Resident-Evil-4)|[Steam](https://store.steampowered.com/app/254700/Resident_Evil_4/)| |
|[Resident Evil 5](https://github.com/Dekirai/MultiPresence/wiki/Resident-Evil-5)|[Steam](https://store.steampowered.com/app/21690/Resident_Evil_5/)| |
|[Resident Evil 6](https://github.com/Dekirai/MultiPresence/wiki/Resident-Evil-6)|[Steam](https://store.steampowered.com/app/221040/Resident_Evil_6/)| |
|[Resident Evil Revelations 2](https://github.com/Dekirai/MultiPresence/wiki/Resident-Evil-Revelations-2)|[Steam](https://store.steampowered.com/app/287290/Resident_Evil_Revelations_2/)|Raid Mode only|
|[Sonic Adventure 2](https://github.com/Dekirai/MultiPresence/wiki/Sonic-Adventure-2)|[Steam](https://store.steampowered.com/app/213610/Sonic_Adventure_2/)| |
|[TY the Tasmanian Tiger](https://github.com/Dekirai/MultiPresence/wiki/TY-the-Tasmanian-Tiger)|[Steam](https://store.steampowered.com/app/411960/TY_the_Tasmanian_Tiger/)| |
|[Zelda: The Wind Waker HD¹](https://github.com/Dekirai/MultiPresence/wiki/Zelda:-The-Wind-Waker-HD)|[Cemu](https://wiki.cemu.info/wiki/The_Legend_of_Zelda:_The_Wind_Waker_HD)|Works on EUR and USA|
|[Zelda: Twilight Princess HD¹](https://github.com/Dekirai/MultiPresence/wiki/Zelda:-Twilight-Princess-HD)|[Cemu](https://wiki.cemu.info/wiki/The_Legend_of_Zelda:_Twilight_Princess_HD)|Works on EUR and USA|

## Information
¹: Requires pattern search.
> [!IMPORTANT]  
> A pattern search can cause a short lag on your machine because the program is currently scanning a lot of memory to find the right address to work with.  
<details>
<summary>CEMU</summary>
You have to disable the "Discord Presence" option found in Options -> General settings.<br />
</details>

If you are interested in which games support English and/or German language, you can check [here](https://github.com/Dekirai/MultiPresence/wiki/Translation).  

The presences are always expected to work on the latest version of the game.  
Games on lower versions will never be supported. They will always use the latest version from their game store.  
If a game receives a patch, I'll do my best to update it as soon as possible.       

Clicking on a game's name will take you to it's wiki, which contains all the features and previews.
