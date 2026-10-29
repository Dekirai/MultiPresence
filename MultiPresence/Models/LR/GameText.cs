namespace MultiPresence.Models.LR
{
    public static class GameText
    {
        public static string GetCaseName(int id)
        {
            return Cases.TryGetValue(id, out var value) ? value.Name : "";
        }

        public static string GetCaseAsset(int id)
        {
            return Cases.TryGetValue(id, out var value) ? value.Asset : "logo";
        }

        public static string GetDifficulty(int id)
        {
            return Difficulties.TryGetValue(id, out var value) ? value : "";
        }

        public static string GetCaseType(int id)
        {
            return CaseTypes.TryGetValue(id, out var value) ? value : "";
        }

        public static string GetMapSize(int id)
        {
            return MapSizes.TryGetValue(id, out var value) ? value : "";
        }

        public static int GetChapter(string? scene)
        {
            return scene != null && Chapters.TryGetValue(scene, out var value) ? value : 0;
        }

        public static string GetObjective(string? key)
        {
            return key != null && Objectives.TryGetValue(key, out var value) ? value : "";
        }

        private static readonly Dictionary<int, (string Name, string Asset)> Cases = new Dictionary<int, (string Name, string Asset)>
        {
            { 0, ("Unkept Hedges", "unkept") },
            { 1, ("Kept Hedges", "kept") },
            { 2, ("Forest", "forest") },
            { 3, ("Brick", "brick") },
            { 4, ("Trenches", "trenches") },
            { 5, ("Cornfield", "cornfield") },
            { 6, ("Manor", "manor") },
            { 7, ("Crypts", "crypts") },
            { 8, ("Sewers", "sewers") },
            { 9, ("Carnival", "carnival") },
            { 10, ("Mines", "mines") },
            { 11, ("Snowy Hedges", "snowy") },
            { 12, ("Ruins", "ruins") },
            { 13, ("Dead Forest", "dead") },
            { 14, ("Fog City", "fog") },
            { 15, ("Bamboo Forest", "bamboo") },
            { 16, ("Backrooms", "backrooms") },
            { 17, ("Swamp", "swamp") },
            { 18, ("Castle", "castle") },
            { 19, ("Sand Dunes", "logo") },
            { 20, ("Egyptian Temple", "logo") },
        };

        private static readonly Dictionary<int, string> Difficulties = new Dictionary<int, string>
        {
            { 0, "Easy" },
            { 1, "Normal" },
            { 2, "Hard" },
            { 3, "Very Hard" },
            { 4, "Extreme" },
        };

        private static readonly Dictionary<int, string> CaseTypes = new Dictionary<int, string>
        {
            { 0, "Normal" },
            { 1, "Rare" },
            { 2, "Hardcore" },
        };

        private static readonly Dictionary<int, string> MapSizes = new Dictionary<int, string>
        {
            { 0, "Very Small" },
            { 1, "Small" },
            { 2, "Medium" },
            { 3, "Large" },
            { 4, "Very Large" },
            { 5, "Giant" },
        };

        private static readonly Dictionary<string, int> Chapters = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
        {
            { "Zone 1", 1 },
            { "Zone 2", 2 },
            { "Zone 3", 3 },
            { "Zone 4", 4 },
            { "Zone 5", 5 },
            { "Zone 6", 6 },
        };

        private static readonly Dictionary<string, string> Objectives = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            { "find_way_through_gate", "Find a way through the gate" },
            { "leave_maze", "Leave maze" },
            { "objective_blue_valve", "Find and turn the Blue water valve" },
            { "objective_check_dock", "Check the dock" },
            { "objective_crank_puzzle_K", "Arrange the metal discs" },
            { "objective_create_route", "Create a route across the sawmill" },
            { "objective_create_thermite", "Create thermite:" },
            { "objective_createpoison", "Create a poison to get into the greenhouse" },
            { "objective_destroyeffigies", "Find and Destroy the 3 Effigies" },
            { "objective_dlc_pillars_puzzle", "Align the pillars in the correct order" },
            { "objective_dlc_scales_puzzle", "Find the Anubis scales and balance the weights" },
            { "objective_ducks_puzzle_J", "Win the Carnival Game" },
            { "objective_escape_backrooms", "Escape the backrooms" },
            { "objective_escape_swamp", "Escape swamp" },
            { "objective_find_aluminium", "- Aluminium Oxide" },
            { "objective_find_blowtorch", "Find a Blowtorch" },
            { "objective_find_boat_fuel", "Find fuel for the boat" },
            { "objective_find_boat_keys", "Find keys for the boat" },
            { "objective_find_cranks", "Find cranks" },
            { "objective_find_dock", "Find the dock" },
            { "objective_find_gasoline", "Find Gasoline" },
            { "objective_find_gate_valve", "Find gate valve" },
            { "objective_find_incense_sticks", "Find incense sticks" },
            { "objective_find_iron", "- Iron Filings" },
            { "objective_find_joan", "Find Joan" },
            { "objective_find_light_source", "Find an alternative light source" },
            { "objective_find_lighter", "Find a lighter" },
            { "objective_find_magnesium", "- Magnesium Strips" },
            { "objective_find_mine", "Find a way into the mine" },
            { "objective_find_radio_codes", "Find radio codes" },
            { "objective_find_skulls", "Find skulls" },
            { "objective_find_sluice_gate_parts", "Find sluice gate parts" },
            { "objective_find_tile_O", "Find the missing manhole plate" },
            { "objective_find_well", "Find the well" },
            { "objective_findattic", "Find your way into the attic" },
            { "objective_finddagger", "Investigate the Attic" },
            { "objective_findhouse", "Explore the Forest" },
            { "objective_findkeys", "Find # keys" },
            { "objective_findseals", "Find # seals" },
            { "objective_findtablet", "Find another tablet and place it in the other pedestal" },
            { "objective_findtabletusage", "Find a use for the tablet" },
            { "objective_findworksite", "Find the worksite in the maze" },
            { "objective_follow_joan", "Follow Joan" },
            { "objective_green_valve", "Find and turn the Green water valve" },
            { "objective_insert_gate_valve", "Insert gate valve" },
            { "objective_insert_incense_sticks", "Insert incense sticks" },
            { "objective_insert_radio_codes", "Insert radio codes" },
            { "objective_insert_skulls", "Insert skulls into pedestal" },
            { "objective_insertkeys", "Insert # keys" },
            { "objective_insertseals", "Insert # seals" },
            { "objective_investigaterestricted", "Investigate the restricted part" },
            { "objective_keycard", "Find a keycard for the door" },
            { "objective_manor_paintings", "Find correct paintings order" },
            { "objective_mirrors_puzzle_M", "Align the mirrors" },
            { "objective_objectiveupdated", "OBJECTIVE UPDATED" },
            { "objective_open_bronze_doors", "Open the bronze doors" },
            { "objective_open_gate", "Find a way to open the gate" },
            { "objective_opencrypt", "Complete the ritual" },
            { "objective_pedestalpuzzle", "Figure out the correct pedestal order" },
            { "objective_pipe_puzzle", "Find and align the drainage pipes" },
            { "objective_pipe_puzzle_N", "Get the water flowing" },
            { "objective_pour_gasoline", "Pour Gasoline in the well" },
            { "objective_pulllevers", "Pull # levers" },
            { "objective_red_valve", "Find and turn the Red water valve" },
            { "objective_refuel_boat", "Refuel the boat" },
            { "objective_repair_sluice_gates", "Repair the sluice gates" },
            { "objective_runtogate", "Go through the exit gate" },
            { "objective_set_fire", "Set fire to the well" },
            { "objective_shrine_puzzle", "Find and complete the shrine puzzle" },
            { "objective_solvecryptex", "Solve the cryptex" },
            { "objective_tile_puzzle_O", "Open the manhole" },
            { "objective_unlockcryptex", "Find a way to unlock the cryptex" },
            { "objective_use_thermite", "Place and Light Thermite Bomb" },
            { "objective_yellow_valve", "Find and turn the Yellow water valve" },
            { "tutorial_objective_items", "Pick up all objects from the safehouse" },
            { "tutorial_objective_solve_puzzle", "Solve the puzzle and pull the lever" },
        };
    }
}
