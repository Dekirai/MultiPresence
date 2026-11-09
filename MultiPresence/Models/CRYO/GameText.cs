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

        public static string GetContractTitle(string objectiveId)
        {
            if (!objectiveId.StartsWith("contract_", StringComparison.Ordinal))
            {
                return "";
            }

            var id = objectiveId[9..];
            return GetText(ContractTitleKeys.TryGetValue(id, out var key) ? key : id + "_title");
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

        private static readonly Dictionary<string, string> ContractTitleKeys = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            { "contract_confidential_hdd_hell", "contract_confidential_hdd_title" },
            { "contract_delta7_escape_helicopter_hell", "contract_delta7_escape_helicopter_title" },
            { "contract_delta7_escape_panicroom_hell", "contract_delta7_escape_panicroom_title" },
            { "contract_delta7_escape_snowtruck_hell", "contract_delta7_escape_snowtruck_title" },
            { "contract_erebus_escape_helicopter_hell", "contract_erebus_escape_helicopter_title" },
            { "contract_erebus_escape_liferaft_hell", "contract_erebus_escape_liferaft_title" },
            { "contract_erebus_escape_submarine_hell", "contract_erebus_escape_submarine_title" },
            { "contract_fungus_experiments_dvd_hell", "contract_fungus_experiments_dvd_title" },
            { "contract_mini_doner_machine_hell", "contract_mini_doner_machine_title" },
            { "contract_patient_samples_hell", "contract_patient_samples_title" },
            { "contract_senior_engineer_documents_hell", "contract_senior_engineer_documents_title" },
            { "contract_water_quality_sample", "item_water_quality_sample" },
            { "contract_water_quality_sample_hell", "item_water_quality_sample" },
        };

        private static readonly Dictionary<string, string> Texts = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            { "Body Flashlight", "Body Flashlight" },
            { "Changing Room", "Changing Room" },
            { "Chat Messages", "Chat Messages" },
            { "Panic Room", "Panic Room" },
            { "Start the Game", "Start the Game" },
            { "Use the Tablet", "Use the Tablet" },
            { "contract_activate_glowsticks_title", "Glow in the dark" },
            { "contract_chain_lock_doors_title", "Barricade" },
            { "contract_clean_run_title", "Clean run" },
            { "contract_confidential_hdd_step", "Retrieve the Confidential HDD Drive from the Server Room and escape with it" },
            { "contract_confidential_hdd_title", "Confidential data" },
            { "contract_controlled_burn_step", "Light 5 fires with the Flaregun" },
            { "contract_controlled_burn_title", "Controlled burn" },
            { "contract_delta7_escape_helicopter_step", "Escape Station Delta-7 with the Helicopter" },
            { "contract_delta7_escape_helicopter_title", "Air evac" },
            { "contract_delta7_escape_panicroom_step", "Escape Station Delta-7 through the Panic Room" },
            { "contract_delta7_escape_panicroom_title", "Lockdown" },
            { "contract_delta7_escape_snowtruck_step", "Escape Station Delta-7 with the Snow Truck" },
            { "contract_delta7_escape_snowtruck_title", "Road trip" },
            { "contract_delta7_every_escape_title", "Every way out" },
            { "contract_delta7_lock_monster_step_escape", "Escape Station Delta-7" },
            { "contract_delta7_lock_monster_step_lock", "Lock a monster in the Panic Room" },
            { "contract_delta7_lock_monster_title", "Quarantine" },
            { "contract_delta7_maintenance_reports_step", "Upload the repair reports from the maintenance office computer" },
            { "contract_delta7_maintenance_reports_title", "Paper trail" },
            { "contract_delta7_speedrun_step", "Escape Station Delta-7 in less than 15 minutes" },
            { "contract_delta7_speedrun_title", "Against the clock" },
            { "contract_delta7_water_pressure_step", "Shut off the water valves" },
            { "contract_delta7_water_pressure_title", "Under pressure" },
            { "contract_distract_monsters_title", "Decoy" },
            { "contract_erebus_comms_logs_step", "Upload the communication logs from the Communications Office computer" },
            { "contract_erebus_comms_logs_title", "Radio silence" },
            { "contract_erebus_escape_helicopter_step", "Escape the Erebus with the Helicopter" },
            { "contract_erebus_escape_helicopter_title", "Flight deck" },
            { "contract_erebus_escape_liferaft_step", "Escape the Erebus with the Liferaft" },
            { "contract_erebus_escape_liferaft_title", "Abandon ship" },
            { "contract_erebus_escape_submarine_step", "Escape the Erebus with the Submarine" },
            { "contract_erebus_escape_submarine_title", "Deep dive" },
            { "contract_erebus_every_escape_title", "Any port in a storm" },
            { "contract_erebus_open_15_containers_title", "Customs inspection" },
            { "contract_erebus_speedrun_step", "Escape the Erebus in less than 15 minutes" },
            { "contract_erebus_speedrun_title", "Sinking fast" },
            { "contract_escape_3_times_title", "Frequent flyer" },
            { "contract_extinguish_10_fires_title", "Fire brigade" },
            { "contract_find_10_collectibles_title", "Scavenger hunt" },
            { "contract_fix_vehicle_parts_title", "Mechanic" },
            { "contract_full_crew_extracted_title", "Nobody left behind" },
            { "contract_fungus_experiments_dvd_step", "Retrieve the Fungus Experiments Results DVD from one of the Labs and escape with it" },
            { "contract_fungus_experiments_dvd_title", "Fungal research" },
            { "contract_hard_shift_step", "Escape on Hard difficulty or higher" },
            { "contract_hard_shift_title", "Hard shift" },
            { "contract_hell_shift_step", "Escape on Hell difficulty" },
            { "contract_hell_shift_title", "Hell shift" },
            { "contract_hell_streak_step", "Escape 3 times on Hell difficulty" },
            { "contract_hell_streak_title", "Hell week" },
            { "contract_mini_doner_machine_step", "Retrieve Ozan's Mini Doner Machine from the kitchen and escape with it" },
            { "contract_mini_doner_machine_title", "Kebab emergency" },
            { "contract_monster_escape_feral_step", "Escape while the Feral is hunting" },
            { "contract_monster_escape_feral_title", "Untamed" },
            { "contract_monster_escape_hound_step", "Escape while the Hound is hunting" },
            { "contract_monster_escape_hound_title", "Off the leash" },
            { "contract_monster_escape_lantern_step", "Escape while the Lantern is hunting" },
            { "contract_monster_escape_lantern_title", "Don't follow the light" },
            { "contract_monster_escape_neuralis_step", "Escape while the Neuralis is hunting" },
            { "contract_monster_escape_neuralis_title", "Clear mind" },
            { "contract_monster_escape_stalker_step", "Escape while the Stalker is hunting" },
            { "contract_monster_escape_stalker_title", "Eyes in the dark" },
            { "contract_mutator_escape_step", "Escape with any Mutator active" },
            { "contract_mutator_escape_title", "Field test" },
            { "contract_open_drawers_title", "Scavenger" },
            { "contract_patient_samples_step", "Retrieve the Patient Samples from the medical cold storage and escape with them" },
            { "contract_patient_samples_title", "Cold chain" },
            { "contract_revive_teammates_title", "Field medic" },
            { "contract_security_audit_title", "Security audit" },
            { "contract_senior_engineer_documents_step", "Retrieve the Senior Engineer Documents from one of the Lodgings rooms and escape with them" },
            { "contract_senior_engineer_documents_title", "Engineering secrets" },
            { "contract_split_up_step", "Escape through 2 different routes (at least 2 players)" },
            { "contract_split_up_title", "Split up" },
            { "contract_stun_with_flaregun_title", "Flare test" },
            { "contract_stun_with_light_title", "Light discipline" },
            { "contract_water_quality_sample_step", "Recover the Water Quality Sample from the Water Treatment Center" },
            { "item_water_quality_sample", "Water Quality Sample" },
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
            { "objective_contracts", "Contracts" },
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
