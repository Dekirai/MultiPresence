# Config

Here is a working config.json file.  
You can either copy and paste this code into a newly created file called config.json or download it [here](https://github.com/user-attachments/files/16415956/config.json).  
You can also remove any games you don't want in this file to keep it cleaner, you don't need to have everything in there.  
For example, if you only want to play Kingdom Hearts II, you can remove everything but Kingdom Hearts II.  
Note that the file is not required for MultiPresence to work, it's just optional so that users can edit the details and state to their liking.  
A list of placeholders for each game can be found [here](https://github.com/Dekirai/MultiPresence/tree/main/Placeholders).

```json
{
   "Games": {
       "asobiSW": {
           "Default": {
               "Details": "{nickname} (Lv. {level})",
               "State": "Playing as {character}"
           }
       },
       "Kingdom Hearts Birth by Sleep Final Mix": {
           "Default": {
               "Details": "Lv. {level} ({difficulty})",
               "State": "{room}"
           }
       },
       "Kingdom Hearts Dream Drop Distance": {
           "Default": {
               "Details": "Lv. {level} ({difficulty})",
               "State": "{room}"
           }
       },
       "Kingdom Hearts Final Mix": {
           "Default": {
               "Details": "Lv. {level} ({difficulty})",
               "State": "{room}"
           }
       },
       "Kingdom Hearts II Final Mix": {
           "Default": {
               "Details": "Lv. {level} ({difficulty})",
               "State": "{room}"
           }
       },
       "Kingdom Hearts III": {
           "World_Map": {
               "Details": "Playing on {difficulty}",
               "State": "{room}"
           },
           "Gummi_Ship": {
               "Details": "Gummi Lv. {gummilevel} ({difficulty})",
               "State": "{room}"
           },
           "In_World": {
               "Details": "Lv. {level} ({difficulty})",
               "State": "{room}"
           }
       },
       "Marvel's Spider-Man Remastered": {
           "Default": {
               "Details": "Health: {health} (Level {level})",
               "State": "Swinging in {location}"
           }
       },
       "Marvel's Spider-Man: Miles Morales": {
           "Default": {
               "Details": "Health: {health} (Level {level})",
               "State": "Swinging in {location}"
           }
       },
       "Mega Man 11": {
           "Default": {
               "Details": "Lives: {lives} ({difficulty})",
               "State": "{stage}"
           }
       },
       "Mega Man Battle Network 6": {
           "Default": {
               "Details": "HP: {hp}/{maxhp}",
               "State": "{location}"
           },
           "In_Battle": {
               "Details": "HP: {hp_battle}/{maxhp_battle}",
               "State": "{location} (In Battle)"
           }
       },
       "Pangya Reborn": {
           "Ingame_Match": {
               "Details": "{nickname} - {level}",
               "State": "{mode} — H{currenthole}/{maxholes}"
           },
           "Ingame_Tourney": {
               "Details": "{nickname} - {level}",
               "State": "{mode} — H{currenthole}/{maxholes} — Score: {score}"
           },
           "Ingame_Lounge": {
               "Details": "{nickname} - {level}",
               "State": "{mode}"
           },
           "Waiting_Room": {
               "Details": "{nickname} - {level}",
               "State": "{mode}"
           },
           "Lobby": {
               "Details": "{nickname} - {level}",
               "State": "In Lobby"
           }
       },
       "Resident Evil": {
           "Default": {
               "Details": "{floor}",
               "State": "{room}"
           }
       },
       "Resident Evil 4 (2005)": {
           "Title_Screen": {
               "Details": "At the Title Screen",
               "State": ""
           },
           "Main": {
               "Details": "Weapon: {weapon}",
               "State": "Chapter {chapter}: {room}"
           },
           "Mercenaries": {
               "Details": "Score: {score}",
               "State": "Playing as '{character}' on '{room}'"
           },
           "Assignment_Ada": {
               "Details": "Weapon: {weapon}",
               "State": "{room}"
           },
           "Separate_Ways": {
               "Details": "Weapon: {weapon}",
               "State": "Chapter {chapter}: {room}"
           }
       },
       "Resident Evil 5": {
           "Default": {
               "Details": "{chapter}",
               "State": "{room}"
           }
       },
       "Resident Evil 6": {
           "Booting": {
               "Details": "Starting the game...",
               "State": ""
           },
           "Ingame": {
               "Details": "{chapter}",
               "State": "{room}"
           },
           "Main_Menu": {
               "Details": "In Main Menu",
               "State": ""
           },
           "Saving": {
               "Details": "Saving the game...",
               "State": ""
           },
           "Cutscene": {
               "Details": "{chapter}",
               "State": "In a cutscene"
           }
       },
       "Resident Evil Revelations 2": {
           "Lobby": {
               "Details": "Raid Mode: In Lobby",
               "State": "{character} (Lv. {level})"
           },
           "Ingame": {
               "Details": "Raid Mode: Mission {chapter}-0{mission}",
               "State": "{character} (Lv. {level})"
           }
       }
   }
}
```
