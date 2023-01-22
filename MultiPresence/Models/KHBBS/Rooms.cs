namespace MultiPresence.Models.KHBBS
{
    public class Rooms
    {
        public static async Task<string[]> GetRoom(string world)
        {
            List<string> getroom = new List<string>();

            switch (world)
            {
                case "The Land of Departure":
                    getroom.Add("The Land of Departure");
                    getroom.Add("Forecourt");
                    getroom.Add("Great Hall");
                    getroom.Add("Ventus's Room");
                    getroom.Add("Ventus's Room");
                    getroom.Add("Mountain Path");
                    getroom.Add("Summit");
                    getroom.Add("Forecourt");
                    getroom.Add("Forecourt");
                    getroom.Add("Great Hall");
                    getroom.Add("Summit Ruins");
                    getroom.Add("Chamber of Waking");
                    getroom.Add("Castle Oblivion");
                    getroom.Add("Character Selection");
                    getroom.Add("Forecourt Ruins");
                    getroom.Add("Mountain Path");
                    getroom.Add("Summit");

                    break;

                case "Dwarf Woodlands":
                    getroom.Add("Dwarf Woodlands");
                    getroom.Add("Mine Entrance");
                    getroom.Add("The Mine");
                    getroom.Add("Vault");
                    getroom.Add("Magic Mirror Chamber");
                    getroom.Add("Underground Waterway");
                    getroom.Add("Courtyard");
                    getroom.Add("Flower Glade");
                    getroom.Add("Deep Woods");
                    getroom.Add("Inside the Mirror");
                    getroom.Add("Cottage Clearing");
                    getroom.Add("The Cottage");
                    getroom.Add("Mountain Trail");
                    break;

                case "Castle of Dreams":
                    getroom.Add("Castle of Dreams");
                    getroom.Add("Cinderalla's Room");
                    getroom.Add("Mousehole");
                    getroom.Add("Wardrobe Room");
                    getroom.Add("Entrance");
                    getroom.Add("The Chateau");
                    getroom.Add("Forest");
                    getroom.Add("Palace Courtyard");
                    getroom.Add("Corridor");
                    getroom.Add("Ballroom");
                    getroom.Add("Foyer");
                    getroom.Add("Passage");
                    getroom.Add("Antechamber");
                    getroom.Add("Wardrobe Room");
                    getroom.Add("Mousehole");
                    getroom.Add("Wardrobe Room");
                    break;

                case "Enchanted Dominion":
                    getroom.Add("Enchanted Dominion");
                    getroom.Add("Dungeon Cell");
                    getroom.Add("Gates");
                    getroom.Add("Maleficent's Throne");
                    getroom.Add("Dungeon");
                    getroom.Add("Hall");
                    getroom.Add("Forbidden Mountain");
                    getroom.Add("Waterside");
                    getroom.Add("Forest Clearing");
                    getroom.Add("Bridge");
                    getroom.Add("Bridge");
                    getroom.Add("Audience Chamber");
                    getroom.Add("Audience Chamber");
                    getroom.Add("RESERVED");
                    getroom.Add("Hallway");
                    getroom.Add("Aurora's Chamber");
                    getroom.Add("Tower Room");
                    getroom.Add("Hall");
                    getroom.Add("Aurora's Chamber");
                    getroom.Add("Hall");
                    getroom.Add("Hall");
                    getroom.Add("Hall");
                    getroom.Add("Hall");
                    getroom.Add("Hall");
                    getroom.Add("Hall");
                    getroom.Add("Hall");
                    getroom.Add("Hall");
                    getroom.Add("Hall");
                    getroom.Add("Hall");
                    getroom.Add("Hall");
                    getroom.Add("Hall");
                    getroom.Add("Hall");
                    getroom.Add("Hall");
                    getroom.Add("Hall");
                    getroom.Add("Hall");
                    getroom.Add("Hall");
                    getroom.Add("Hall");
                    getroom.Add("Hall");
                    getroom.Add("Hall");
                    getroom.Add("Gates");
                    break;

                case "The Mysterious Tower":
                    getroom.Add("The Mysterious Tower");
                    getroom.Add("Sorcerer's Chamber");
                    getroom.Add("Mysterious Tower");
                    getroom.Add("Tower Entrance");
                    getroom.Add("Sorcerer's Chamber");
                    break;

                case "Radiant Garden":
                    getroom.Add("Radiant Garden");
                    getroom.Add("Outer Gardens");
                    getroom.Add("Entryway");
                    getroom.Add("Central Square");
                    getroom.Add("Aqueduct");
                    getroom.Add("Castle Town");
                    getroom.Add("Reactor");
                    getroom.Add("Fountain Court");
                    getroom.Add("Merlin's House");
                    getroom.Add("Gardens");
                    getroom.Add("Front Doors");
                    getroom.Add("Purification Facility");
                    getroom.Add("Outer Gardens");
                    getroom.Add("Central Square");
                    getroom.Add("Central Square");
                    break;

                case "Event":
                    getroom.Add("FOR EVENTS ONLY");
                    getroom.Add("Louie's Ruins Court");
                    getroom.Add("Louie's Ruins Path");
                    getroom.Add("Crossroad");
                    getroom.Add("UG Ruins Entrance");
                    getroom.Add("UG Ruins Passage 1");
                    getroom.Add("UG Ruins Passage 2");
                    getroom.Add("UG Courtyard");
                    getroom.Add("Jungle Near Ruins");
                    getroom.Add("Eminence");
                    getroom.Add("Jungle");
                    getroom.Add("Man-Village River");
                    getroom.Add("Bog");
                    break;

                case "Olympus Coliseum":
                    getroom.Add("Olympus Coliseum");
                    getroom.Add("Coliseum Gates");
                    getroom.Add("Vestibule");
                    getroom.Add("West Bracket");
                    getroom.Add("East Bracket");
                    getroom.Add("Town Near Thebes");
                    getroom.Add("East Bracket");
                    break;

                case "Deep Space":
                    getroom.Add("Deep Space");
                    getroom.Add("Turo Pirson Block");
                    getroom.Add("Turo Transporter");
                    getroom.Add("Durgon Transporter");
                    getroom.Add("Ship Corridor");
                    getroom.Add("Control Room");
                    getroom.Add("Containment Pod");
                    getroom.Add("Ship Hub");
                    getroom.Add("Machinery Bay");
                    getroom.Add("Launch Deck");
                    getroom.Add("Ship Exterior");
                    getroom.Add("Outer Space");
                    getroom.Add("Ship Corridor");
                    getroom.Add("Lanes Between");
                    getroom.Add("Machinery Bay Access");
                    break;

                case "Destiny Islands":
                    getroom.Add("Destiny Islands");
                    getroom.Add("Island Beach");
                    getroom.Add("Island Beach");
                    getroom.Add("Island Beach");
                    getroom.Add("Main Island Beach");
                    break;

                case "Never Land":
                    getroom.Add("Never Land");
                    getroom.Add("Cove");
                    getroom.Add("Cliff Path");
                    getroom.Add("Mermaid Lagoon");
                    getroom.Add("Seacoast");
                    getroom.Add("Jungle Clearing");
                    getroom.Add("Peter's Hideout");
                    getroom.Add("Gully");
                    getroom.Add("Indian Camp");
                    getroom.Add("Rainbow Falls: Base");
                    getroom.Add("Rainbow Falls: Ascent");
                    getroom.Add("Rainbow Falls: Crest");
                    getroom.Add("Skull Rock: Entrance");
                    getroom.Add("Skull Rock: Cavern");
                    getroom.Add("Night Sky");
                    break;

                case "Disney Town":
                    getroom.Add("Disney Town");
                    getroom.Add("Library");
                    getroom.Add("Main Plaza");
                    getroom.Add("Fruitball Court");
                    getroom.Add("Racecourse A");
                    getroom.Add("Raceway");
                    getroom.Add("Gizmo Gallery");
                    getroom.Add("Pete's Rec Room");
                    getroom.Add("Racecourse B");
                    getroom.Add("Racecourse C");
                    getroom.Add("Racecourse D");
                    getroom.Add("Lanes Between");
                    getroom.Add("Raceway Registration");
                    getroom.Add("Ice Cream");
                    getroom.Add("Fruitball");
                    getroom.Add("Race: Castle Course");
                    break;

                case "The Keyblade Graveyard":
                    getroom.Add("The Keyblade Graveyard");
                    getroom.Add("Badlands");
                    getroom.Add("Seat of War");
                    getroom.Add("Twister Trench");
                    getroom.Add("Eye of the Storm");
                    getroom.Add("Eye of the Storm");
                    getroom.Add("Eye of the Storm");
                    getroom.Add("Fissure");
                    getroom.Add("Keyblade Graveyard");
                    getroom.Add("Keyblade Graveyard");
                    getroom.Add("Keyblade Graveyard");
                    getroom.Add("Will's Cage");
                    getroom.Add("Keyblade Graveyard");
                    break;

                case "Mirage Arena":
                    getroom.Add("Mirage Arena");
                    getroom.Add("Hub");
                    getroom.Add("Coliseum");
                    getroom.Add("Arena");
                    getroom.Add("Badlands");
                    getroom.Add("Pinball");
                    getroom.Add("Ship Hub");
                    getroom.Add("Mousehole");
                    getroom.Add("Forest");
                    getroom.Add("Skull Rock");
                    getroom.Add("Audience Chamber");
                    getroom.Add("Forecourt");
                    getroom.Add("Summit");
                    getroom.Add("Launch Deck");
                    getroom.Add("Ship Exterior");
                    getroom.Add("Arena");
                    getroom.Add("");
                    getroom.Add("");
                    getroom.Add("");
                    getroom.Add("");
                    getroom.Add("");
                    break;

                case "Command Board":
                    getroom.Add("Command Board");
                    getroom.Add("Land of Departure BG");
                    getroom.Add("");
                    getroom.Add("Cinderella BG");
                    getroom.Add("");
                    getroom.Add("");
                    getroom.Add("");
                    getroom.Add("");
                    getroom.Add("");
                    getroom.Add("");
                    getroom.Add("Lilo & Stitch BG");
                    getroom.Add("");
                    getroom.Add("Peter Pan BG");
                    getroom.Add("Disney Castle BG");
                    getroom.Add("");
                    getroom.Add("");
                    getroom.Add("");
                    getroom.Add("");
                    getroom.Add("");
                    getroom.Add("Peter Pan BG");
                    break;

                case "World Map":
                    getroom.Add("World Map");
                    getroom.Add("World Map");
                    break;

                case "The Hundred Acre Wood":
                    getroom.Add("The Hundred Acre Wood");
                    getroom.Add("The Hundred Acre Wood");
                    break;

                case "The Badlands":
                    getroom.Add("The Badlands");
                    getroom.Add("The Badlands");
                    break;

                case "Main Menu":
                    getroom.Add("Main Menu");
                    getroom.Add("Main Menu");
                    break;

            }
            return getroom.ToArray();
        }
    }
}