namespace MultiPresence.Models.KH2
{
    public class Rooms
    {
        public static async Task<string[]> GetRoom(string world)
        {
            List<string> getroom = new List<string>();

            switch (world)
            {
                case "World Map":
                    getroom.Add("World Map");
                    break;

                case "Twilight Town":
                    getroom.Add("The Empty Realm");
                    getroom.Add("Roxas' Room");
                    getroom.Add("The Usual Spot");
                    getroom.Add("Back Alley");
                    getroom.Add("Sandlot");
                    getroom.Add("Sandlot");
                    getroom.Add("Market Street: Station Heights");
                    getroom.Add("Market Street: Tram Common");
                    getroom.Add("Station Plaza");
                    getroom.Add("Central Station");
                    getroom.Add("Sunset Terrace");
                    getroom.Add("Sunset Station");
                    getroom.Add("Sunset Hill");
                    getroom.Add("The Woods");
                    getroom.Add("The Old Mansion");
                    getroom.Add("Mansion: Foyer");
                    getroom.Add("Mansion: Dining Room");
                    getroom.Add("Mansion: Library");
                    getroom.Add("Mansion: The White Room");
                    getroom.Add("Mansion: Basement Hall");
                    getroom.Add("Mansion: Basement Hall");
                    getroom.Add("Mansion: Computer Room");
                    getroom.Add("Mansion: Basement Corridor");
                    getroom.Add("Mansion: Pod Room");
                    getroom.Add("On the Train");
                    getroom.Add("The Tower");
                    getroom.Add("Tower: Entryway");
                    getroom.Add("Tower: Sorcerer's Loft");
                    getroom.Add("Tower: Wardrobe");
                    getroom.Add("Tower: Star Chamber");
                    getroom.Add("Tower: Moon Chamber");
                    getroom.Add("Tower: Wayward Stairs");
                    getroom.Add("Station of Serenity");
                    getroom.Add("Station of Calling");
                    getroom.Add("Station of Awakening");
                    getroom.Add("The Mysterial Train");
                    getroom.Add("Tunnelway");
                    getroom.Add("Underground Concouse");
                    getroom.Add("Tower: Wayward Stairs");
                    getroom.Add("Tower: Wayward Stairs");
                    getroom.Add("Betwixt and Between");
                    getroom.Add("The Old Mansion");
                    break;

                case "Hollow Bastion":
                    getroom.Add("Villain's Vale");
                    getroom.Add("The Dark Depths");
                    getroom.Add("The Great Maw");
                    getroom.Add("Crystal Fissure");
                    getroom.Add("Castle Gate");
                    getroom.Add("Ansem's Study");
                    getroom.Add("Postern");
                    getroom.Add("Restoration Site");
                    getroom.Add("Bailey");
                    getroom.Add("Borough");
                    getroom.Add("Marketplace");
                    getroom.Add("Corridors");
                    getroom.Add("Heartless Manufactory");
                    getroom.Add("Merlin's House");
                    getroom.Add("Castle Oblivion");
                    getroom.Add("Ansem's Study");
                    getroom.Add("Ravine Trail");
                    getroom.Add("The Great Maw");
                    getroom.Add("Restoration Site");
                    getroom.Add("Bailey");
                    getroom.Add("Corridors");
                    getroom.Add("Cavern of Remembrance: Depths");
                    getroom.Add("Cavern of Remembrance: Mining Area");
                    getroom.Add("Cavern of Remembrance: Engine Chamber");
                    getroom.Add("Cavern of Remembrance: Mineshaft");
                    getroom.Add("Transport to Remembrance");
                    getroom.Add("Garden of Assemblage");
                    getroom.Add("Underground Facility");
                    getroom.Add("Memory's Contortion");
                    getroom.Add("The World of Nothing");
                    getroom.Add("Hall of Empty Melodies");
                    getroom.Add("The Great Maw");
                    getroom.Add("Front Mansion");
                    getroom.Add("Station of Remembrance");
                    getroom.Add("Destiny Islands");
                    getroom.Add("Addled Impasse");
                    getroom.Add("Mansion: Basement Hall");
                    getroom.Add("Havoc's Divide");
                    getroom.Add("Station of Oblivion");
                    break;

                case "Beast's Castle":
                    getroom.Add("Entrance Hall");
                    getroom.Add("Parlor");
                    getroom.Add("Belle's Room");
                    getroom.Add("The Beast's Room");
                    getroom.Add("Ballroom");
                    getroom.Add("Ballroom");
                    getroom.Add("Courtyard");
                    getroom.Add("The East Wing");
                    getroom.Add("The West Hall");
                    getroom.Add("The West Wing");
                    getroom.Add("Dungeon");
                    getroom.Add("Undercroft");
                    getroom.Add("Secret Passage");
                    getroom.Add("Bridge");
                    getroom.Add("Ballroom");
                    getroom.Add("Bridge");
                    break;

                case "The Land of Dragon's":
                    getroom.Add("Bamboo Grove");
                    getroom.Add("Encampment");
                    getroom.Add("Checkpoint");
                    getroom.Add("Mountain Trail");
                    getroom.Add("Village");
                    getroom.Add("Village Cave");
                    getroom.Add("Ridge");
                    getroom.Add("Summit");
                    getroom.Add("Imperial Square");
                    getroom.Add("Palace Gate");
                    getroom.Add("Antechamber");
                    getroom.Add("Throne Room");
                    getroom.Add("Village");
                    break;

                case "Olympus Coliseum":
                    getroom.Add("The Coliseum");
                    getroom.Add("Coliseum Gates");
                    getroom.Add("Coliseum Gates");
                    getroom.Add("Underworld Entrance");
                    getroom.Add("Coliseum Foyer");
                    getroom.Add("Valley of the Dead");
                    getroom.Add("Hades' Chamber");
                    getroom.Add("Cave of the Dead: Entrance");
                    getroom.Add("Well of Captivity");
                    getroom.Add("The Underdrome");
                    getroom.Add("Cave of the Dead: Inner Chamber");
                    getroom.Add("Underworld Caverns: Entrance");
                    getroom.Add("The Lock");
                    getroom.Add("The Underdrome");
                    getroom.Add("Coliseum Gates");
                    getroom.Add("Cave of the Dead: Passage");
                    getroom.Add("Underworld Caverns: The Lost Road");
                    getroom.Add("Underworld Caverns: Atrium");
                    getroom.Add("Coliseum Gates");
                    getroom.Add("The Underdrome");
                    break;

                case "Port Royal":
                    getroom.Add("Rampart");
                    getroom.Add("Harbor");
                    getroom.Add("Town");
                    getroom.Add("The Interceptor");
                    getroom.Add("The Interceptor: Ship's Hold");
                    getroom.Add("The Black Pearl");
                    getroom.Add("The Black Pearl: Captain's Stateroom");
                    getroom.Add("The Interceptor");
                    getroom.Add("Isla de Muerta: Rock Face");
                    getroom.Add("Isla de Muerta: Cave Mouth");
                    getroom.Add("Isla de Muerta: Treasure Heap");
                    getroom.Add("Ship Graveyard: The Interceptor's Hold");
                    getroom.Add("Isla de Muerta: Powder Store");
                    getroom.Add("Isla de Muerta: Moonlight Nook");
                    getroom.Add("Ship Graveyard: Seadrift Keep");
                    getroom.Add("Ship Graveyard: Seadrift Row");
                    getroom.Add("Isla de Muerta: Rock Face");
                    getroom.Add("Isla de Muerta: Treasure Heap");
                    getroom.Add("The Black Pearl");
                    getroom.Add("The Black Pearl");
                    getroom.Add("The Black Pearl");
                    getroom.Add("The Interceptor");
                    getroom.Add("The Interceptor");
                    getroom.Add("The Black Pearl: Captain's Stateroom");
                    getroom.Add("Harbor");
                    getroom.Add("Isla de Muerta: Rock Face");
                    break;

                case "Disney Castle":
                    getroom.Add("Audience Chamber");
                    getroom.Add("Library");
                    getroom.Add("Colonnade");
                    getroom.Add("Courtyard");
                    getroom.Add("The Hall of the Cornerstone");
                    getroom.Add("The Hall of the Cornerstone");
                    getroom.Add("Gummi Hangar");
                    getroom.Add("The Wilderness");
                    break;

                case "Halloween Town":
                    getroom.Add("Halloween Town Square");
                    getroom.Add("Dr. Finklestein's Lab");
                    getroom.Add("Graveyard");
                    getroom.Add("Curly Hill");
                    getroom.Add("Hinterlands");
                    getroom.Add("Yuletide Hill");
                    getroom.Add("Candy Cane Lane");
                    getroom.Add("Christmas Tree Plaza");
                    getroom.Add("Santa's House");
                    getroom.Add("Toy Factory: Shipping and Receiving");
                    getroom.Add("Toy Factory: The Wrapping Room");
                    break;

                case "Agrabah":
                    getroom.Add("Agrabah");
                    getroom.Add("Bazaar");
                    getroom.Add("The Peddler's Shop");
                    getroom.Add("The Palace");
                    getroom.Add("Vault");
                    getroom.Add("Above Agrabah");
                    getroom.Add("Palace Walls");
                    getroom.Add("The Cavern of Wonders: Entrance");
                    getroom.Add("???");
                    getroom.Add("The Cavern of Wonders: Stone Guardians");
                    getroom.Add("The Cavern of Wonders: Treasure Room");
                    getroom.Add("Ruined Chamber");
                    getroom.Add("The Cavern of Wonders: Valley of Stone");
                    getroom.Add("The Cavern of Wonders: Chasm of Challenges");
                    getroom.Add("Sandswept Ruins");
                    getroom.Add("The Peddler's Shop");
                    break;

                case "Space Paranoids":
                    getroom.Add("Pit Cell");
                    getroom.Add("Canyon");
                    getroom.Add("Game Grid");
                    getroom.Add("Dataspace");
                    getroom.Add("I/O Tower: Hallway");
                    getroom.Add("I/O Tower: Communications Room");
                    getroom.Add("Simulation Hangar");
                    getroom.Add("Solar Sailer Simulation");
                    getroom.Add("Central Computer Mesa");
                    getroom.Add("Central Computer Core");
                    getroom.Add("Solar Sailer Simulation");
                    getroom.Add("Solar Sailer Simulation");
                    break;

                case "Pride Lands":
                    getroom.Add("Pride Rock");
                    getroom.Add("Stone Hollow");
                    getroom.Add("The King's Den");
                    getroom.Add("Wildebeest Valley");
                    getroom.Add("The Savannah");
                    getroom.Add("Elephant Graveyard");
                    getroom.Add("Gorge");
                    getroom.Add("Wastelands");
                    getroom.Add("Jungle");
                    getroom.Add("Oasis");
                    getroom.Add("Pride Rock");
                    getroom.Add("Oasis");
                    getroom.Add("Overlook");
                    getroom.Add("Peak");
                    getroom.Add("Scar's Darkness");
                    getroom.Add("The Savannah");
                    getroom.Add("Wildebeest Valley");
                    break;

                case "Atlantica":
                    getroom.Add("Triton's Throne");
                    getroom.Add("Ariel's Grotto");
                    getroom.Add("Undersea Courtyard");
                    getroom.Add("Undersea Courtyard");
                    getroom.Add("The Palace: Performance Hall");
                    getroom.Add("Sunken Ship");
                    getroom.Add("The Shore");
                    getroom.Add("The Shore");
                    getroom.Add("The Shore");
                    getroom.Add("Wrath of the Sea");
                    getroom.Add("Wedding Ship");
                    break;

                case "100 Acre Wood":
                    getroom.Add("The Hundred Acre Wood");
                    getroom.Add("Starry Hill");
                    getroom.Add("Pooh Bear's House");
                    getroom.Add("Rabbit's House");
                    getroom.Add("Piglet's House");
                    getroom.Add("Kanga's House");
                    getroom.Add("A Windsday Tale");
                    getroom.Add("The Honey Hunt");
                    getroom.Add("Blossom Valley");
                    getroom.Add("The Spooky Cave");
                    break;

                case "Timeless River":
                    getroom.Add("Cornerstone Hill");
                    getroom.Add("Pier");
                    getroom.Add("Waterway");
                    getroom.Add("Wharf");
                    getroom.Add("Lilliput");
                    getroom.Add("Building Site");
                    getroom.Add("Scene of the Fire");
                    getroom.Add("Mickey's House");
                    break;

                case "The World That Never Was":
                    getroom.Add("Where Nothing Gathers");
                    getroom.Add("Alley to Between");
                    getroom.Add("Fragment Crossing");
                    getroom.Add("Memory's Skyscraper");
                    getroom.Add("The Brink of Despair");
                    getroom.Add("The Soundless Prison");
                    getroom.Add("Nothing's Call");
                    getroom.Add("Crooked Ascension (Bottom)");
                    getroom.Add("Crooked Ascension (Top)");
                    getroom.Add("Twilight's View");
                    getroom.Add("Hall of Empty Melodies (Bottom)");
                    getroom.Add("Hall of Empty Melodies (Top)");
                    getroom.Add("Naught's Skyway");
                    getroom.Add("Proof of Existence");
                    getroom.Add("Havoc's Divide");
                    getroom.Add("Addled Impasse");
                    getroom.Add("Naught's Approach");
                    getroom.Add("Ruin and Creation's Passage");
                    getroom.Add("The Altar of Naught");
                    getroom.Add("Memory's Contortion");
                    getroom.Add("The World of Nothing");
                    getroom.Add("Station of Awakening");
                    getroom.Add("The World of Nothing");
                    getroom.Add("The World of Nothing");
                    getroom.Add("The World of Nothing");
                    getroom.Add("The World of Nothing");
                    getroom.Add("The World of Nothing");
                    getroom.Add("The World of Nothing");
                    getroom.Add("The World of Nothing");
                    getroom.Add("The Dark Margin");
                    break;

                case "Main Menu":
                    getroom.Add("Main Menu");
                    break;

            }
            return getroom.ToArray();
        }

        public static async Task<string[]> GetRoomDE(string world)
        {
            List<string> getroom = new List<string>();

            switch (world)
            {
                case "Weltkarte":
                    getroom.Add("Weltkarte");
                    break;

                case "Twilight Town":
                    getroom.Add("Das leere Reich");
                    getroom.Add("Roxas' Raum");
                    getroom.Add("Der Treffpunkt");
                    getroom.Add("Hintere Gasse");
                    getroom.Add("Turnierplatz");
                    getroom.Add("Turnierplatz");
                    getroom.Add("Einkaufspromenade: Anhöhe");
                    getroom.Add("Einkaufspromenade: Tram-Forum");
                    getroom.Add("Banhofsplatz");
                    getroom.Add("Hauptbahnhof");
                    getroom.Add("Abendrot-Viertel");
                    getroom.Add("Abendrot-Bahnhof");
                    getroom.Add("Abendrot-Hügel");
                    getroom.Add("Waldstück");
                    getroom.Add("Das alte Herrenhaus");
                    getroom.Add("Herrenhaus: Empfangshalle");
                    getroom.Add("Herrenhaus: Speisesaal");
                    getroom.Add("Herrenhaus: Bibliothek");
                    getroom.Add("Herrenhaus: Das weiße Zimmer");
                    getroom.Add("Herrenhaus: Kellerraum");
                    getroom.Add("Herrenhaus: Kellerraum");
                    getroom.Add("Herrenhaus: Computer-Raum");
                    getroom.Add("Herrenhaus: Kellergang");
                    getroom.Add("Herrenhaus: Kapselraum");
                    getroom.Add("An Bord des Zuges");
                    getroom.Add("Der Turm");
                    getroom.Add("Turm: Treppenaufgang");
                    getroom.Add("Turm: Studierstube des Zauberers");
                    getroom.Add("Turm: Ankleidezimmer");
                    getroom.Add("Turm: Sternenkammer");
                    getroom.Add("Turm: Mondkammer");
                    getroom.Add("Turm: Die schwebende Treppe");
                    getroom.Add("Ort der inneren Ruhe");
                    getroom.Add("Ort der Berufung");
                    getroom.Add("Ort des Erwachens");
                    getroom.Add("Der geheimnisvolle Zug");
                    getroom.Add("Verbindungstunnel");
                    getroom.Add("Tunnelsystem");
                    getroom.Add("Turm: Die schwebende Treppe");
                    getroom.Add("Turm: Die schwebende Treppe");
                    getroom.Add("Jenseits von hier und jetzt");
                    getroom.Add("Das alte Herrenhaus");
                    break;

                case "Hollow Bastion":
                    getroom.Add("Stützpunkt des Bösen");
                    getroom.Add("Die dunklen Tiefen");
                    getroom.Add("Azurpfad");
                    getroom.Add("Kristallkluft");
                    getroom.Add("Vor den Schlossmauern");
                    getroom.Add("Ansems Arbeitszimmer");
                    getroom.Add("Seiteneingang");
                    getroom.Add("Wiederaufbaugelände");
                    getroom.Add("Außenhof");
                    getroom.Add("Wohnviertel");
                    getroom.Add("Marktplatz");
                    getroom.Add("Korridore");
                    getroom.Add("Herzlosen-Farm");
                    getroom.Add("Merlins Haus");
                    getroom.Add("Schloss des Entfallens");
                    getroom.Add("Ansems Arbeitszimmer");
                    getroom.Add("Pfad am Abgrund");
                    getroom.Add("Azurpfad");
                    getroom.Add("Wiederaufbaugelände");
                    getroom.Add("Außenhof");
                    getroom.Add("Korridore");
                    getroom.Add("Höhle des Gedächtnisses: Tiefen");
                    getroom.Add("Höhle des Gedächtnisses: Mine");
                    getroom.Add("Höhle des Gedächtnisses: Maschinenraum");
                    getroom.Add("Höhle des Gedächtnisses: Minenschacht");
                    getroom.Add("Ruf ins Gedächtnis");
                    getroom.Add("Hof der Zusammenkunft");
                    getroom.Add("Unterirdische Anlage");
                    getroom.Add("Verwerfung der Erinnerung");
                    getroom.Add("Die Welt des Nichts");
                    getroom.Add("Halle der leeren Melodien");
                    getroom.Add("Azurpfad");
                    getroom.Add("Das alte Herrenhaus");
                    getroom.Add("Ort des Gedächtnisses");
                    getroom.Add("Die Insel des Schicksals");
                    getroom.Add("Abweg der Konfusion");
                    getroom.Add("Herrenhaus: Kellerraum");
                    getroom.Add("Kluft des Chaos");
                    getroom.Add("Ort des Entfallens");
                    break;

                case "Das Schloss des Biestes":
                    getroom.Add("Eingangshalle");
                    getroom.Add("Kaminzimmer");
                    getroom.Add("Belles Gemach");
                    getroom.Add("Gemach des Biestes");
                    getroom.Add("Ballsaal");
                    getroom.Add("Ballsaal");
                    getroom.Add("Vorhof");
                    getroom.Add("Ostflügel");
                    getroom.Add("Westhalle");
                    getroom.Add("Westflügel");
                    getroom.Add("Verlies");
                    getroom.Add("Kellergewölbe");
                    getroom.Add("Geheime Passage");
                    getroom.Add("Brücke");
                    getroom.Add("Ballsaal");
                    getroom.Add("Brücke");
                    break;

                case "Das Land der Drachen":
                    getroom.Add("Bambus-Hain");
                    getroom.Add("Rekrutenlager");
                    getroom.Add("Passierstelle");
                    getroom.Add("Hochland-Pass");
                    getroom.Add("Dorf");
                    getroom.Add("Höhle vor dem Dorf");
                    getroom.Add("Berggrat");
                    getroom.Add("Gipfel");
                    getroom.Add("Der Kaiserliche Platz");
                    getroom.Add("Palasttor");
                    getroom.Add("Vorhalle");
                    getroom.Add("Thronsaal");
                    getroom.Add("Dorf");
                    break;

                case "Arena des Olymps":
                    getroom.Add("Die Arena");
                    getroom.Add("Heroen-Platz");
                    getroom.Add("Heroen-Platz");
                    getroom.Add("Eingang zu Unterwelt");
                    getroom.Add("Vorhalle der Arena");
                    getroom.Add("Tal der Toten");
                    getroom.Add("Kammer des Hades");
                    getroom.Add("Höhle der Toten: Eingang");
                    getroom.Add("Brunnen ohne Wiederkehr");
                    getroom.Add("Die Unterwelt-Arena");
                    getroom.Add("Höhle der Toten: Innere Kammer");
                    getroom.Add("Unterwelt-Kavernen: Eingang");
                    getroom.Add("Das Siegel");
                    getroom.Add("Die Unterwelt-Arena");
                    getroom.Add("Heroen-Platz");
                    getroom.Add("Höhle der Toten: Durchgang");
                    getroom.Add("Unterwelt-Kavernen: Der Verlorene Pfad");
                    getroom.Add("Unterwelt-Kavernen: Atrium");
                    getroom.Add("Heroen-Platz");
                    getroom.Add("Die Unterwelt-Arena");
                    break;

                case "Port Royal":
                    getroom.Add("Befestigungsanlage");
                    getroom.Add("Hafen");
                    getroom.Add("Stadt");
                    getroom.Add("Die Interceptor");
                    getroom.Add("Die Interceptor: Laderaum");
                    getroom.Add("Die Black Pearl");
                    getroom.Add("Die Black Pearl: Kapitänsquartier");
                    getroom.Add("Die Interceptor");
                    getroom.Add("Isla de Muerta: Felsweg");
                    getroom.Add("Isla de Muerta: Höhleneingang");
                    getroom.Add("Isla de Muerta: Schatzgrotte");
                    getroom.Add("Schiffsfriedhof: Laderaum der Interceptor");
                    getroom.Add("Isla de Muerta: Schießpulverlager");
                    getroom.Add("Isla de Muerta: Mondlichtung");
                    getroom.Add("Schiffsfriedhof: Wrackfestung");
                    getroom.Add("Schiffsfriedhof: Trümmerkuppel");
                    getroom.Add("Isla de Muerta: Felsweg");
                    getroom.Add("Isla de Muerta: Schatzgrotte");
                    getroom.Add("Die Black Pearl");
                    getroom.Add("Die Black Pearl");
                    getroom.Add("Die Black Pearl");
                    getroom.Add("Die Interceptor");
                    getroom.Add("Die Interceptor");
                    getroom.Add("Die Black Pearl: Kapitänsquartier");
                    getroom.Add("Hafen");
                    getroom.Add("Isla de Muerta: Felsweg");
                    break;

                case "Schloss Disney":
                    getroom.Add("Audienzsaal");
                    getroom.Add("Bibliothek");
                    getroom.Add("Wandelgang");
                    getroom.Add("Innenhof");
                    getroom.Add("Die Halle des Ecksteins");
                    getroom.Add("Die Halle des Ecksteins");
                    getroom.Add("Gummi-Hangar");
                    getroom.Add("Steppe");
                    break;

                case "Halloween Town":
                    getroom.Add("Fallbeil-Platz");
                    getroom.Add("Dr. Finkelsteins Laboratorium");
                    getroom.Add("Friedhof");
                    getroom.Add("Spiralhügel");
                    getroom.Add("Hinterland");
                    getroom.Add("Weihnachtshügel");
                    getroom.Add("Zuckerstangen-Gasse");
                    getroom.Add("Weihnachtsbaumplatz");
                    getroom.Add("Das Haus des Nikolaus");
                    getroom.Add("Spielzeugfabrik: Versandzentrale");
                    getroom.Add("Spielzeugfabrik: Verpackungsraum");
                    break;

                case "Agrabah":
                    getroom.Add("Agrabah");
                    getroom.Add("Basar");
                    getroom.Add("Beim Trödler");
                    getroom.Add("Palastplatz");
                    getroom.Add("Kerkergewölbe");
                    getroom.Add("Über den Dächern von Agrabah");
                    getroom.Add("Vor den Stadtmauern");
                    getroom.Add("Die Wunderhöhle: Eingang");
                    getroom.Add("???");
                    getroom.Add("Die Wunderhöhle: Raum der Steinwächter");
                    getroom.Add("Die Wunderhöhle: Schatzkammer");
                    getroom.Add("Zerstörte Kammer");
                    getroom.Add("Die Wunderhöhle: Tal der Steine");
                    getroom.Add("Die Wunderhöhle: Schlund der Herausforderungen");
                    getroom.Add("Die Wunderhöhle: Ruinen im Sand");
                    getroom.Add("Beim Trödler");
                    break;

                case "Space Paranoids":
                    getroom.Add("Abrufzelle");
                    getroom.Add("Rasterspalte");
                    getroom.Add("Spielraster");
                    getroom.Add("Datenspeicher");
                    getroom.Add("I/O-Tower: Verbindungsgang");
                    getroom.Add("I/O-Tower: Kommunikationsraum");
                    getroom.Add("Simulations-Hangar");
                    getroom.Add("Solarsegler-Simulation");
                    getroom.Add("Plateu zum Systemkern");
                    getroom.Add("Systemkern");
                    getroom.Add("Solarsegler-Simulation");
                    getroom.Add("Solarsegler-Simulation");
                    break;

                case "Das Geweihte Land":
                    getroom.Add("Königsfelsen");
                    getroom.Add("Die Höhle des Weisen");
                    getroom.Add("Die Höhle des Königs");
                    getroom.Add("Gnu-Schlucht");
                    getroom.Add("Die Savanne");
                    getroom.Add("Elefantenfriedhof");
                    getroom.Add("Klamm");
                    getroom.Add("Ödland");
                    getroom.Add("Dschungel");
                    getroom.Add("Oase");
                    getroom.Add("Königsfelsen");
                    getroom.Add("Oase");
                    getroom.Add("Aussichtspunkt");
                    getroom.Add("Gipfel");
                    getroom.Add("Scars Finsternis");
                    getroom.Add("Die Savanne");
                    getroom.Add("Gnu-Schlucht");
                    break;

                case "Atlantica":
                    getroom.Add("König Tritons Thronraum");
                    getroom.Add("Arielles Versteck");
                    getroom.Add("Platz unten im Meer");
                    getroom.Add("Platz unten im Meer");
                    getroom.Add("Palast: Unterseebühne");
                    getroom.Add("Schiffswrack");
                    getroom.Add("Ufer");
                    getroom.Add("Ufer");
                    getroom.Add("Ufer");
                    getroom.Add("Zorn des Meeres");
                    getroom.Add("Hochzeitsschiff");
                    break;

                case "Hundertmorgenwald":
                    getroom.Add("Hundertmorgenwald");
                    getroom.Add("Abschied unterm Sternenhimmel");
                    getroom.Add("Puuhs Haus");
                    getroom.Add("Rabbits Haus");
                    getroom.Add("Ferkels Haus");
                    getroom.Add("Kangas Haus");
                    getroom.Add("Ein windiger Tag");
                    getroom.Add("Abenteuerliche Honigjagd");
                    getroom.Add("Große Sprünge im Blütental");
                    getroom.Add("Abenteuer in der Gruselhöhle");
                    break;

                case "Fluss der Nostalgie":
                    getroom.Add("Der Hügel des Ecksteins");
                    getroom.Add("Landungssteg");
                    getroom.Add("Schleusentor");
                    getroom.Add("Dock");
                    getroom.Add("Liliput");
                    getroom.Add("Baustelle");
                    getroom.Add("Schauplatz des Feuers");
                    getroom.Add("Mickys Haus");
                    break;

                case "The World That Never Was":
                    getroom.Add("Wo Nichts sich sammelt");
                    getroom.Add("Gasse zum Dazwischen");
                    getroom.Add("Übergang der Fragmente");
                    getroom.Add("Wolkenkratzer der Erinnerung");
                    getroom.Add("Abgrund zur Hoffnungslosigkeit");
                    getroom.Add("Das stumme Gefängnis");
                    getroom.Add("Auf des Nichts");
                    getroom.Add("Aufstieg & Fall (Unten)");
                    getroom.Add("Aufstieg & Fall (Oben)");
                    getroom.Add("Stufen der Hoffnung");
                    getroom.Add("Halle der leeren Melodien (Unten)");
                    getroom.Add("Halle der leeren Melodien (Oben)");
                    getroom.Add("Pfad der Nichtigkeit");
                    getroom.Add("Zeugnis der Existenz");
                    getroom.Add("Kluft des Chaos");
                    getroom.Add("Abweg der Konfusion");
                    getroom.Add("Annäherung ans Nichts");
                    getroom.Add("Galerie der Schöpfung und Zerstörung");
                    getroom.Add("Der Alter des Nichts");
                    getroom.Add("Verwerfung der Erinnerung");
                    getroom.Add("Die Welt des Nichts");
                    getroom.Add("Ort des Erwachens");
                    getroom.Add("Die Welt des Nichts");
                    getroom.Add("Die Welt des Nichts");
                    getroom.Add("Die Welt des Nichts");
                    getroom.Add("Die Welt des Nichts");
                    getroom.Add("Die Welt des Nichts");
                    getroom.Add("Die Welt des Nichts");
                    getroom.Add("Die Welt des Nichts");
                    getroom.Add("Der dunkle Rand");
                    break;

                case "Titelbild":
                    getroom.Add("Titelbild");
                    break;

            }
            return getroom.ToArray();
        }
    }
}