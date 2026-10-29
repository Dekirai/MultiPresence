// Room names adapted from Squirrelies/RE2RRandomizer, commit
// 8d4f57f3165f0c55ce9986b63455f4dbc91aafea, src/Database/Database.cpp.
// IDs cross-checked against SpeedrunTooling/SRTPluginProviderRE2, commit
// e2a24e221e7a9afa1d2802035803c853671ec5b5, PlayerManager.cs.
// Only known map IDs receive room names; unknown values remain visible.
/*
MIT License

Copyright (c) 2024 Travis J. Gutjahr

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
using System.Collections.Frozen;

namespace MultiPresence.Models.RE2
{
    public static class Locations
    {
        public static string GetRegion(int id)
        {
            return LocationNames.GetValueOrDefault(id) ?? $"Unknown area ({id})";
        }

        public static string GetRoom(int id)
        {
            return MapNames.GetValueOrDefault(id) ?? $"Unknown room ({id})";
        }

        public static string GetFullLocation(int locationId, int mapId)
        {
            return $"{GetRegion(locationId)} / {GetRoom(mapId)}";
        }

        public static readonly FrozenDictionary<int, string> LocationNames = new Dictionary<int, string>
        {
            { 0, "None" },
            { 1, "Raccoon City" },
            { 2, "Factory" },
            { 3, "NEST" },
            { 4, "Mountain" },
            { 5, "Opening" },
            { 6, "Orphanage" },
            { 7, "Police Station" },
            { 8, "Water Treatment Plant" },
            { 9, "Sewers" },
            { 10, "Playground" },
            { 11, "NEST (DLC)" },
            { 12, "Ada (DLC)" },
            { 13, "Hunk (DLC)" },
            { 14, "Opening" },
            { 15, "Gas Station" },
            { 16, "Police Station" },
            { 17, "Sewers" },
            { 18, "Water Treatment Plant" },
            { 19, "Cutscene (EV011)" },
            { 20, "Cutscene (EV050)" },
            { 21, "NEST Escape" },
            { 22, "Transport" },
            { 23, "Gas Station" },
            { 24, "Orphanage" },
            { 25, "Orphanage Approach" },
            { 26, "Alligator Area" },
            { 27, "Title Screen" },
            { 28, "Movie" },
            { 29, "Police Station Basement" },
            { 30, "Opening" },
            { 31, "Game Over" },
            { 32, "Results" },
            { 33, "Ending" }
        }.ToFrozenDictionary();
        public static readonly FrozenDictionary<int, string> MapNames = new Dictionary<int, string>
        {
            { 0, "None" },
            { 112, "Main Hall" },
            { 118, "Safety Deposit Room" },
            { 119, "West Office" },
            { 120, "Darkroom" },
            { 122, "Boiler Room Path" },
            { 123, "Boiler Room" },
            { 125, "West Hallway (4-214)" },
            { 126, "Cable Car Platform" },
            { 127, "Reception" },
            { 128, "Security Room" },
            { 129, "Cafeteria/Kitchen" },
            { 130, "HVAC Duct" },
            { 131, "Nap Room" },
            { 133, "Monitor Room" },
            { 134, "Pump Room" },
            { 135, "Duct" },
            { 136, "Boiler Room" },
            { 219, "Entrance" },
            { 220, "Reception" },
            { 222, "Records Room" },
            { 224, "Operations Room" },
            { 225, "West Hallway (4-215)" },
            { 226, "West Hallway (4-216)" },
            { 227, "East Courtyard" },
            { 229, "East Hallway (4-303)" },
            { 230, "East Office" },
            { 231, "East Hallway Closet" },
            { 232, "Press Room" },
            { 233, "Interrogation Room" },
            { 234, "Observation Room" },
            { 235, "East Break Room" },
            { 237, "Women's Bathroom" },
            { 238, "Watchman's Room" },
            { 239, "East Hallway (4-313)" },
            { 240, "East Hallway (4-314)" },
            { 243, "Women's Locker Room" },
            { 244, "Men's Locker Room" },
            { 245, "S.T.A.R.S. Office" },
            { 248, "Lounge" },
            { 249, "Library" },
            { 250, "Linen Room" },
            { 252, "West Hallway (Staircase)" },
            { 253, "West Hallway (S.T.A.R.S. Office)" },
            { 254, "Waiting Room" },
            { 255, "East Hallway (4-502)" },
            { 256, "Art Room" },
            { 257, "Chief's Office" },
            { 258, "Private Collection Hallway" },
            { 259, "Private Collection Room" },
            { 260, "Roof" },
            { 261, "East Staircase" },
            { 263, "Clock Tower" },
            { 264, "West Hallway (4-603)" },
            { 265, "West Storage Room" },
            { 268, "Balcony" },
            { 269, "East Storage Room" },
            { 270, "Spade Key Room" },
            { 271, "East Storage Passage" },
            { 272, "Hallway" },
            { 273, "Morgue" },
            { 274, "Generator Room" },
            { 275, "Firing Range" },
            { 277, "Parking Garage" },
            { 279, "Kennel" },
            { 282, "Jail" },
            { 310, "Hallway" },
            { 312, "Exhaust Duct" },
            { 313, "Sewer (3-603)" },
            { 315, "Sewer (Upper Waterway)" },
            { 316, "Workroom" },
            { 317, "Sewer (Lower Waterway 1)" },
            { 318, "Sewer (3-614)" },
            { 319, "Treatment Room" },
            { 320, "Office" },
            { 321, "Sewer (3-617)" },
            { 322, "Sewer (3-620)" },
            { 323, "Sewer (Lower Waterway 2)" },
            { 325, "Organ Trail" },
            { 326, "Supplies Storage Entrance" },
            { 327, "Supplies Storage Room" },
            { 329, "Sewer (3-627)" },
            { 330, "Cable Car/Control Room" },
            { 332, "Monitor Room" },
            { 335, "Proposed Water Purification Room" },
            { 338, "Garbage Room" },
            { 339, "Incinerator" },
            { 340, "Garbage Room" },
            { 341, "Incinerator Exit" },
            { 347, "Gas Station" },
            { 350, "Secret Room" },
            { 351, "Raccoon City Streets (Kendo's Shop)" },
            { 352, "Underground Stairs" },
            { 353, "Machinery Room" },
            { 356, "Elevator Control Room" },
            { 358, "Raccoon City Streets (A Start)" },
            { 359, "Front Hall" },
            { 360, "Hallway" },
            { 361, "Director's Room" },
            { 363, "Bedroom" },
            { 364, "Nursery" },
            { 365, "Bedroom Hallway" },
            { 366, "Medical Observation Room" },
            { 367, "Bathroom" },
            { 370, "Main Shaft" },
            { 371, "Lobby" },
            { 372, "Biotesting Lab" },
            { 373, "Escape Platform/Tyrant Fight" },
            { 377, "Sewer (1-620)" },
            { 378, "Sewer (1-621)" },
            { 379, "Alligator Fight" },
            { 380, "Alligator Aftermath" },
            { 381, "Underground Passage" },
            { 382, "Sewer (1-611)" },
            { 383, "Sewer (1-612)" },
            { 384, "Sewer (1-613)" },
            { 385, "Sewer (1-614)" },
            { 386, "Main Shaft Elevator" },
            { 405, "Secret Save Room Elevator" },
            { 406, "Kendo's Shop/Walkway" },
            { 407, "Sewer Entrance" },
            { 408, "Orphanage Approach" },
            { 409, "Jail Intake" },
            { 410, "Greenhouse" },
            { 411, "Weed Hallway" },
            { 412, "Drug Testing Lab" },
            { 413, "Storage Room" },
            { 414, "Low-Temp Testing Lab" },
            { 415, "Lounge" },
            { 416, "Server Room" },
            { 417, "Maintenance Room" },
            { 418, "Bioreactor Decontamination" },
            { 419, "Bioreactor Room" },
            { 420, "P-4 Level Testing Lab" },
            { 421, "Train Turntable" },
            { 422, "Train Platform" },
            { 423, "Train Railcar" },
            { 429, "Sewer (3-604)" },
            { 431, "West Hallway (4-217)" },
            { 432, "Presentation Room" },
            { 433, "Greenhouse Control Room" }
        }.ToFrozenDictionary();
    }
}
