namespace MultiPresence.Models.CRYO
{
    internal static class GameText
    {
        public static string GetMap(string scene)
        {
            return Maps.TryGetValue(scene, out var value) ? value : "";
        }

        public static string GetDifficulty(int id)
        {
            return Difficulties.TryGetValue(id, out var value) ? value : "";
        }

        public static string GetObjectiveType(int id)
        {
            return ObjectiveTypes.TryGetValue(id, out var value) ? value : "";
        }

        public static string GetText(string key)
        {
            return Texts.TryGetValue(key, out var value) ? value : "";
        }

        private static readonly Dictionary<string, string> Maps = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Station_Delta_7_AI", "Station Delta-7" },
            { "SHIP_layout", "Erebus" },
        };

        private static readonly Dictionary<int, string> Difficulties = new Dictionary<int, string>
        {
            { 0, "Easy" },
            { 1, "Normal" },
            { 2, "Hard" },
            { 3, "Hell" },
        };

        private static readonly Dictionary<int, string> ObjectiveTypes = new Dictionary<int, string>
        {
            { 0, "Primary" },
            { 1, "Escape" },
            { 2, "Side" },
        };

        private static readonly Dictionary<string, string> Texts = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            { "Body Flashlight", "Body Flashlight" },
            { "Changing Room", "Changing Room" },
            { "Chat Messages", "Chat Messages" },
            { "Panic Room", "Panic Room" },
            { "Start the Game", "Start the Game" },
            { "Use the Tablet", "Use the Tablet" },
            { "objective_autopilot", "Autopilot" },
            { "objective_autopilot_fix", "Fix all ship systems to unlock autopilot" },
            { "objective_autopilot_override", "Authorize autopilot override" },
            { "objective_bilge_pump", "Bilge Pumps" },
            { "objective_bilge_pump_insert", "Insert priming Valve Wheel" },
            { "objective_bilge_pump_switches", "Activate Bilge Pumps power switches" },
            { "objective_bilge_pump_valve", "Turn valve to prime Bilge Pumps" },
            { "objective_broadcast", "Fix Broadcast" },
            { "objective_broadcast_solve", "Find communications and match the transmission frequency" },
            { "objective_comms_uplink", "Communications" },
            { "objective_comms_uplink_calibrate", "Calibrate comms uplink signal" },
            { "objective_comms_uplink_fix", "Resume uplink calibration" },
            { "objective_comms_uplink_start", "Press the communications console button to start uplink calibration" },
            { "objective_comms_uplink_transmitter", "Find and install replacement communications Transmitter unit" },
            { "objective_escape", "Escape" },
            { "objective_escape_main", "Find an escape route" },
            { "objective_gyroscope", "Gyroscope" },
            { "objective_gyroscope_calibrate", "Complete gyroscope calibration" },
            { "objective_gyroscope_power", "Power on the gyroscope" },
            { "objective_gyroscope_unlock", "Find gyroscope room and unlock it using Gyroscope Room Key" },
            { "objective_hdd_collect", "Take out the hard drive" },
            { "objective_hdd_copy", "Copy the CCTV footage using the drive reader in security" },
            { "objective_hdd_error", "Confirm error message in the CCTV system" },
            { "objective_hdd_insert", "Insert the HDD Drive into the CCTV system to copy the footage" },
            { "objective_hdd_reader", "CCTV Footage" },
            { "objective_helicopter", "Helicopter" },
            { "objective_helicopter_chains", "Cut the chains" },
            { "objective_helicopter_deicer", "Use de-icer" },
            { "objective_helicopter_keys", "Unlock the helicopter with keys" },
            { "objective_helicopter_refuel", "Refuel the helicopter" },
            { "objective_liferaft", "Liferaft" },
            { "objective_liferaft_compressor", "Find Air Compressor and plug it in to pump up the raft" },
            { "objective_liferaft_compressor_start", "Start up the compressor" },
            { "objective_liferaft_compressor_unplug", "Unplug compressor hose from the liferaft" },
            { "objective_liferaft_crane", "Use crane to deploy the liferaft" },
            { "objective_liferaft_crane_attach", "Hook liferaft to the crane" },
            { "objective_liferaft_crane_deploy", "Move liferaft overboard into the water" },
            { "objective_liferaft_rope", "Pull the rope to release hook and take off" },
            { "objective_liferaft_tape", "Use Tape to seal the holes" },
            { "objective_main", "Primary" },
            { "objective_panic_room", "Panic Room" },
            { "objective_panic_room_open", "Open the panic room" },
            { "objective_panic_room_open_hide", "Hide in the panic room before it closes" },
            { "objective_panic_room_security", "Unlock the panic room using a laptop from the security room" },
            { "objective_panic_room_security_usb", "Use the USB hardware key to unlock the laptop" },
            { "objective_panic_room_servers", "Restart servers to unlock security room keycard scanner" },
            { "objective_panic_room_servers_other", "Restart the second server before the time runs out" },
            { "objective_power_distribution", "Power Distribution" },
            { "objective_power_distribution_fix", "Restore engine room power" },
            { "objective_power_distribution_off", "Power off non-essential systems in the lower decks" },
            { "objective_power_distribution_on_01", "Cooling pumps" },
            { "objective_power_distribution_on_02", "Steering gear" },
            { "objective_power_distribution_on_03", "Ballast management pumps" },
            { "objective_power_distribution_power_on", "Power on crucial ship systems" },
            { "objective_printer", "Research Notes" },
            { "objective_printer_collect", "Collect the Research Notes" },
            { "objective_printer_collect_item", "Research Notes" },
            { "objective_printer_find", "Find the printer in the Research building" },
            { "objective_printer_ink", "Replace the ink cartridge in the printer" },
            { "objective_printer_jam", "Fix the paper jam" },
            { "objective_printer_print", "Print the notes" },
            { "objective_radar", "Radar Array" },
            { "objective_radar_align", "Find Radar Array terminal and use it to align dishes. Lights turn green when orientation is correct" },
            { "objective_recover_items", "Recover Items" },
            { "objective_samples", "Recover Samples" },
            { "objective_samples_cryo_storage", "Find samples in the Laboratories" },
            { "objective_samples_pickup", "Collect the samples" },
            { "objective_samples_unlock", "Find a way to unlock the sample container" },
            { "objective_ship_helicopter_beartrap", "Unlock the beartrap using Beartrap Tag" },
            { "objective_ship_helicopter_refuel_plug", "Find a Fuel Hose and plug it into the fuel pump and helicopter" },
            { "objective_ship_helicopter_refuel_unplug", "Unplug the hose" },
            { "objective_ship_helicopter_refuel_wait", "Wait until fuel tank is full" },
            { "objective_side", "Side" },
            { "objective_snowtruck", "Snow Truck" },
            { "objective_snowtruck_fusebox", "Use a fuse to open the garage door" },
            { "objective_snowtruck_headlights", "Insert a new headlight" },
            { "objective_snowtruck_keys", "Unlock the truck with keys" },
            { "objective_snowtruck_refuel", "Refuel the truck" },
            { "objective_submarine", "Submarine" },
            { "objective_submarine_battery", "Put in the new battery cell" },
            { "objective_submarine_deploy", "Pull control panel lever to deploy the Submarine" },
            { "objective_submarine_fuse", "Replace the control panel fuse" },
            { "objective_submarine_headlight", "Put in the headlight" },
            { "objective_submarine_weld", "Weld the side panel" },
            { "tutorial_changing_room_enter", "Enter the Changing Room to customize your character" },
            { "tutorial_flashlight_use", "Use your Body Flashlight" },
            { "tutorial_objective_tablet", "Use the Tablet" },
            { "tutorial_objective_tablet_chat", "Chat Messages" },
            { "tutorial_objective_tablet_chat_send", "Press the Chat Icon button at the top of the tablet and send a message to all players in the lobby." },
            { "tutorial_objective_tablet_pull_out", "Pull out your tablet" },
            { "tutorial_objective_tablet_select", "Check 'Chat Messages' objective from the list on the right side. In game your goal will be to complete all of your Primary objectives." },
            { "tutorial_start_game_info", "The level selection is on the whiteboard in the main room." },
        };
    }
}
