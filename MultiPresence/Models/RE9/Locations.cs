using System.Collections.Frozen;

namespace MultiPresence.Models.RE9
{
    public static partial class Locations
    {
        public static string Get(string id)
        {
            return Places.TryGetValue(id, out var p) ? (p.Room.Length == 0 ? p.Area : p.Area.Length == 0 ? p.Room : p.Area + " / " + p.Room) : string.Empty;
        }

        public static string GetArea(string id)
        {
            return Places.TryGetValue(id, out var p) ? p.Area : string.Empty;
        }

        public static string GetRoom(string id)
        {
            return Places.TryGetValue(id, out var p) ? p.Room : string.Empty;
        }

        private static readonly FrozenDictionary<string, (string Area, string Room)> Places = new Dictionary<string, (string Area, string Room)>(StringComparer.Ordinal)
        {
            { "Place_0000", ("Wrenwood", "Main Street") },
            { "Place_0100", ("The Wrenwood Hotel", "Employee Entrance") },
            { "Place_0101", ("The Wrenwood Hotel", "Lobby") },
            { "Place_0102", ("The Wrenwood Hotel", "Room 204") },
            { "Place_0200", ("Chronic Care Center", "Room 203") },
            { "Place_0201", ("Chronic Care Center", "Room 201") },
            { "Place_0202", ("Chronic Care Center", "Elevator Hallway") },
            { "Place_0203", ("Chronic Care Center", "Rehabilitation Ward") },
            { "Place_0204", ("Chronic Care Center", "Central Hall") },
            { "Place_0205", ("Chronic Care Center", "Guard Office") },
            { "Place_0206", ("Chronic Care Center", "Custodian's Office") },
            { "Place_0207", ("Chronic Care Center", "Medical Supply Room") },
            { "Place_0208", ("Chronic Care Center", "Attic") },
            { "Place_0209", ("Chronic Care Center", "") },
            { "Place_0210", ("Chronic Care Center", "Lounge") },
            { "Place_0211", ("Chronic Care Center", "Underground") },
            { "Place_0212", ("Chronic Care Center", "Security Room") },
            { "Place_0213", ("Chronic Care Center", "Director's Office") },
            { "Place_0214", ("Chronic Care Center", "Roof") },
            { "Place_0215", ("Chronic Care Center", "Chapel") },
            { "Place_0300", ("Research Facility", "Elevator") },
            { "Place_0301", ("Research Facility", "Reception") },
            { "Place_0400", ("Water Treatment", "Plant Entrance") },
            { "Place_0401", ("Water Treatment", "Storage Bay") },
            { "Place_0500", ("Raccoon City", "Main Street") },
            { "Place_0501", ("Raccoon City", "Subway Station") },
            { "Place_0502", ("Raccoon City", "Central Camp") },
            { "Place_0503", ("Raccoon City", "West Point") },
            { "Place_0504", ("Raccoon City", "Highway") },
            { "Place_0505", ("Raccoon City", "Raccoon City Center") },
            { "Place_0506", ("Raccoon City", "") },
            { "Place_0600", ("Orphanage", "Bedroom") },
            { "Place_0700", ("ARK", "Loading Dock") },
            { "Place_0701", ("ARK", "Dump") },
            { "Place_0702", ("ARK", "Gene Vault") },
            { "Place_0216", ("Chronic Care Center", "Processing") },
            { "Place_0507", ("Raccoon City", "") },
            { "Place_0508", ("Raccoon City", "NF Building") },
            { "Place_0509", ("Raccoon City", "Side Street") },
            { "Place_0601", ("Orphanage", "Director's Room") },
            { "Place_0602", ("Orphanage", "Underground Chapel") },
            { "Place_0510", ("Raccoon City", "Blackthorn Station") },
            { "Place_0511", ("Raccoon City", "Blackthorn Station Platform") },
            { "Place_0512", ("Raccoon City", "Tunnel") },
            { "Place_0513", ("Raccoon City", "Service Entrance") },
            { "Place_0703", ("ARK", "Staff Room") },
            { "Place_0704", ("ARK", "Server Room") },
            { "Place_0705", ("ARK", "Service Corridor") },
            { "Place_0706", ("ARK", "Entrance") },
            { "Place_0707", ("ARK", "Bioweapon Repository 12") },
            { "Place_0708", ("ARK", "Systems Management") },
            { "Place_0709", ("ARK", "B4 Staff Room") },
            { "Place_0710", ("ARK", "") },
            { "Place_0711", ("ARK", "") },
            { "Place_0712", ("ARK", "Central Portal") },
            { "Place_0800", ("R.P.D.", "Main Hall") },
            { "Place_0103", ("The Wrenwood Hotel", "Lounge & Bar") },
            { "Place_0217", ("Chronic Care Center", "Central Hall") },
            { "Place_0218", ("Chronic Care Center", "Chairman's Office") },
            { "Place_0219", ("Chronic Care Center", "Blood Lab") },
            { "Place_0220", ("Chronic Care Center", "Office") },
            { "Place_0221", ("Chronic Care Center", "Lead Researcher's Office") },
            { "Place_0222", ("Chronic Care Center", "Garage") },
            { "Place_0223", ("Chronic Care Center", "Security Manager's Office") },
            { "Place_0224", ("Chronic Care Center", "Medication Room") },
            { "Place_0225", ("Chronic Care Center", "Kitchenette") },
            { "Place_0226", ("Chronic Care Center", "Pantry") },
            { "Place_0227", ("Chronic Care Center", "Holding Cells") },
            { "Place_0228", ("Chronic Care Center", "Inspection") },
            { "Place_0229", ("Chronic Care Center", "Collection Pool") },
            { "Place_0230", ("Chronic Care Center", "Courtyard") },
            { "Place_0231", ("Chronic Care Center", "Helipad") },
            { "Place_0232", ("Chronic Care Center", "Emergency Exit") },
            { "Place_0302", ("Research Facility", "Control Room") },
            { "Place_0303", ("Research Facility", "Emergency Route") },
            { "Place_0402", ("Water Treatment", "Roof Hatch Controls") },
            { "Place_0403", ("Water Treatment", "B1") },
            { "Place_0514", ("Raccoon City", "Ridgewood Station") },
            { "Place_0515", ("Raccoon City", "Logistics Warehouse") },
            { "Place_0516", ("Raccoon City", "Underground Parking Garage") },
            { "Place_0517", ("Raccoon City", "Sewage Facility") },
            { "Place_0518", ("Raccoon City", "Gas Station") },
            { "Place_0519", ("Raccoon City", "Cedarbrook Apartments") },
            { "Place_0520", ("Raccoon City", "Cedarbrook Apartments Roof") },
            { "Place_0521", ("Raccoon City", "Willis Tower") },
            { "Place_0522", ("Raccoon City", "Grimstone Building Rooftop") },
            { "Place_0523", ("Raccoon City", "Grimstone Building Basement") },
            { "Place_0524", ("Raccoon City", "Gun Shop Kendo") },
            { "Place_0525", ("Raccoon City", "Orphanage") },
            { "Place_0603", ("Orphanage", "Main Hall") },
            { "Place_0604", ("Orphanage", "Research Lab") },
            { "Place_0713", ("ARK", "Lab Entrance") },
            { "Place_0714", ("ARK", "Operations Room") },
            { "Place_0715", ("ARK", "Central Refining: Lowest Level") },
            { "Place_0801", ("R.P.D.", "East Wing") },
            { "Place_0716", ("ARK", "Dump Control Room") },
            { "Place_0717", ("ARK", "Lobby") },
            { "Place_0718", ("ARK", "Bioweapon Repository 11") },
            { "Place_0719", ("ARK", "Archives") }
        }.ToFrozenDictionary(StringComparer.Ordinal);
    }
}
