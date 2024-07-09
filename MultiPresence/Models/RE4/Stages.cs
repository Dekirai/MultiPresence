namespace MultiPresence.Models.RE4
{
    public class Stages
    {
        public static async Task<string[]> GetStage(int stage)
        {
            List<string> getstage = new List<string>();

            switch (stage)
            {
                case 0: //Debug Rooms
                    getstage.Add("");
                    getstage.Add("");
                    getstage.Add("ATARI TEST ROOM");
                    getstage.Add("test");
                    getstage.Add("DON BEYA");
                    getstage.Add(""); //Room 5 
                    getstage.Add("");
                    getstage.Add("");
                    getstage.Add("");
                    getstage.Add("");
                    getstage.Add(""); //Room 10
                    getstage.Add("");
                    getstage.Add("");
                    getstage.Add("");
                    getstage.Add("");
                    getstage.Add(""); //Room 15
                    getstage.Add("");
                    getstage.Add("");
                    getstage.Add("");
                    getstage.Add("");
                    getstage.Add(""); //Room 20
                    getstage.Add("EFFECT TEST");
                    getstage.Add("");
                    getstage.Add("");
                    getstage.Add("");
                    getstage.Add(""); //Room 25
                    getstage.Add("");
                    getstage.Add("");
                    getstage.Add("");
                    getstage.Add("");
                    getstage.Add(""); //Room 30
                    getstage.Add("");
                    getstage.Add("EFF TEST0");
                    getstage.Add("EFF TEST1");
                    getstage.Add("EFF TEST2");
                    getstage.Add("EFF TEST3"); //Room 35
                    getstage.Add("EFF TEST4");
                    getstage.Add("EFF TEST5");
                    getstage.Add("EFF TEST5");
                    break;
                case 1: //Village Part
                    getstage.Add("The Road to Pueblo");
                    getstage.Add("Pueblo");
                    getstage.Add("Secret Passage");
                    getstage.Add("Farm");
                    getstage.Add("Ganado Gorge");
                    getstage.Add("Mendez Manor"); //Room 5
                    getstage.Add("House of Confinement");
                    getstage.Add("Ganado Base");
                    getstage.Add("Cemetery");
                    getstage.Add("Quarry");
                    getstage.Add("Swamp"); //Room 10
                    getstage.Add("Lake");
                    getstage.Add("Waterfall");
                    getstage.Add("Lake Cave");
                    getstage.Add("Merchant's Dock");
                    getstage.Add("Village Gate and Gondola"); //Room 15
                    getstage.Add("");
                    getstage.Add("Pueblo");
                    getstage.Add("Secret Passage");
                    getstage.Add("Farm");
                    getstage.Add(""); //Room 20
                    getstage.Add("");
                    getstage.Add("");
                    getstage.Add("Church");
                    getstage.Add("Cemetery");
                    getstage.Add("Quarry"); //Room 25
                    getstage.Add("Swamp");
                    getstage.Add("Lake");
                    getstage.Add("Abandoned Cabin");
                    getstage.Add("Ganado Stronghold");
                    getstage.Add("Gigante Way"); //Room 30
                    getstage.Add("Shed of Enlightenment");
                    break;
                case 2: //Castle Part
                    getstage.Add("Road to the Castle");
                    getstage.Add("Grand Hall");
                    getstage.Add("Castle Ramparts");
                    getstage.Add("Castle Gate");
                    getstage.Add("Great Hall");
                    getstage.Add("Prison"); //Room 5
                    getstage.Add("Theater Hall");
                    getstage.Add("Entry Hall");
                    getstage.Add("Hall of Worship");
                    getstage.Add("Gallery");
                    getstage.Add("Castle Wall"); //Room 10
                    getstage.Add("Bedroom Garden");
                    getstage.Add("Dining Hall");
                    getstage.Add("Study");
                    getstage.Add("Storage");
                    getstage.Add("Exhibition Chamber"); //Room 15
                    getstage.Add("Transport Cart");
                    getstage.Add("Corridor of Royalty");
                    getstage.Add("Weapon Exhibit");
                    getstage.Add("Ballroom");
                    getstage.Add("Exterior Clock Tower"); //Room 20
                    getstage.Add("Throne"); //Needs real name
                    getstage.Add("Armor Exhibit");
                    getstage.Add("Clock Tower");
                    getstage.Add("Throne Room Hall");
                    getstage.Add("Transport Cat"); //Room 25
                    getstage.Add("Underground Ruins");
                    getstage.Add("Mine Rail");
                    getstage.Add("");
                    getstage.Add("Catacombs");
                    getstage.Add(""); //Room 30
                    getstage.Add("");
                    getstage.Add("Mine Entrance");
                    getstage.Add("Service Tunnel");
                    getstage.Add("Lava Room"); //Needs real name
                    getstage.Add("Excavation Site"); //Room 35
                    getstage.Add("Inferno Chamber");
                    getstage.Add("Ruins");
                    getstage.Add("Salazar Statue Room"); //Needs real name
                    getstage.Add("Salazar's Tower");
                    getstage.Add("Tower Apex"); //Room 40
                    getstage.Add("Sewer");
                    getstage.Add("Dock");
                    getstage.Add("Elevator");
                    getstage.Add("Minigame");
                    break;
                case 3: //Island Part
                    getstage.Add("Outer Stronghold");
                    getstage.Add("Cliffside Encampment");
                    getstage.Add("");
                    getstage.Add("Larder");
                    getstage.Add("Security");
                    getstage.Add("Bunker"); //Room 5
                    getstage.Add("Stairwell");
                    getstage.Add("Operating Room");
                    getstage.Add("Freezer");
                    getstage.Add("Research Lab");
                    getstage.Add("Com Tower"); //Room 10
                    getstage.Add("Waste Disposal");
                    getstage.Add("Storage Room");
                    getstage.Add("Basement");
                    getstage.Add("Administration Corridor");
                    getstage.Add("Road to Annex"); //Room 15 //Needs real name (Where Ashley drives the big machine)
                    getstage.Add("Dust"); //Needs real name (Where Leon & Ashley jump into the trash)
                    getstage.Add("Incinerator");
                    getstage.Add("Merchant Room");
                    getstage.Add("");
                    getstage.Add(""); //Room 20
                    getstage.Add("Dozer Crash Site");
                    getstage.Add("Chapel and Furnace");
                    getstage.Add("Machine Room");
                    getstage.Add("Saddler's Quarters");
                    getstage.Add(""); //Room 25
                    getstage.Add("Cavern Entrance");
                    getstage.Add("Cavern");
                    getstage.Add("Krauser's Stronghold");
                    getstage.Add("Militant Camp");
                    getstage.Add(""); //Room 30
                    getstage.Add("");
                    getstage.Add("Fort"); //Needs real name (Where Leon's heli partner dies after)
                    getstage.Add("Cliffside Ruins");
                    getstage.Add("");
                    getstage.Add(""); //Room 35
                    getstage.Add("");
                    getstage.Add("Ancient Passage");
                    getstage.Add("Penitentiary");
                    getstage.Add("Security Checkpoint");
                    getstage.Add(""); //Room 40
                    getstage.Add("Capsule Chamber");
                    getstage.Add("");
                    getstage.Add("");
                    getstage.Add("");
                    getstage.Add(""); //Room 45
                    getstage.Add("");
                    getstage.Add("");
                    getstage.Add("Extraction Chamber");
                    getstage.Add("Construction Yard");
                    getstage.Add("Construction Building"); //Room 50
                    getstage.Add("Underground Water Vein"); //Needs real name (Escaping with the boat)
                    break;
                case 4: //Mercenaries & Assignment Ada
                    getstage.Add("Pueblo");
                    getstage.Add("");
                    getstage.Add("Castle Ramparts");
                    getstage.Add("Militant Grounds");
                    getstage.Add("The Docks");
                    getstage.Add("Outer Stronghold"); //Room 5
                    getstage.Add("Cliffside Encampment");
                    getstage.Add("");
                    getstage.Add("");
                    getstage.Add("");
                    getstage.Add("Stairwell"); //Room 10
                    getstage.Add("Operating Room");
                    getstage.Add("Freezer");
                    getstage.Add("Research Lab");
                    getstage.Add("Com Tower");
                    getstage.Add("Waste Disposal"); //Room 15
                    getstage.Add("Basement");
                    getstage.Add("Administration Corridor");
                    break;
                case 5: //Separate Ways
                    getstage.Add("Pueblo");
                    getstage.Add("Secret Passage");
                    getstage.Add("Cemetery");
                    getstage.Add("Church");
                    getstage.Add("Pueblo");
                    getstage.Add("Farm"); //Room 5
                    getstage.Add("Mendez Manor");
                    getstage.Add("House of Confinement");
                    getstage.Add("Village Gate and Gondola");
                    getstage.Add("Abandoned Cabin");
                    getstage.Add("Gigante Way"); //Room 10
                    getstage.Add("Abandoned Cabin");
                    getstage.Add("Theater Hall");
                    getstage.Add("Castle Wall");
                    getstage.Add("Bedroom Garden");
                    getstage.Add("Gallery"); //Room 15
                    getstage.Add("Exhibition Chamber");
                    getstage.Add("Com Tower");
                    getstage.Add("Tunnelway");
                    getstage.Add("Shipyard");
                    getstage.Add("Shipyard Factory"); //Room 20
                    getstage.Add("Dozer Crash Site");
                    getstage.Add("Chapel and Furnace");
                    getstage.Add("Machine Room");
                    getstage.Add("Militant Camp");
                    getstage.Add("Fort"); //Room 25 //Needs real name (Where Leon's heli partner dies after)
                    getstage.Add("Cavern Entrance");
                    getstage.Add("Cavern");
                    getstage.Add("Security Checkpoint");
                    getstage.Add("Capsule Chamber");
                    getstage.Add("Construction Building"); //Room 30
                    getstage.Add("Movie");
                    getstage.Add("");
                    getstage.Add("");
                    getstage.Add("");
                    getstage.Add(""); //Room 35
                    getstage.Add("");
                    getstage.Add("");
                    getstage.Add("");
                    getstage.Add("");
                    getstage.Add(""); //Room 40
                    getstage.Add("");
                    getstage.Add("");
                    getstage.Add("");
                    getstage.Add("");
                    getstage.Add(""); //Room 45
                    getstage.Add("");
                    getstage.Add("");
                    getstage.Add("E3 Stage");
                    getstage.Add("E3 Stage");
                    getstage.Add("E3 Stage"); //Room 50
                    getstage.Add("");
                    getstage.Add("");
                    break;
                default:
                    getstage.Add("Unknown Location");
                    break;
            }
            return getstage.ToArray();
        }

        public static async Task<string[]> GetStageDE(int stage)
        {
            List<string> getstage = new List<string>();

            switch (stage)
            {
                case 0: //Debug Rooms
                    getstage.Add("");
                    getstage.Add("");
                    getstage.Add("ATARI TEST ROOM");
                    getstage.Add("test");
                    getstage.Add("DON BEYA");
                    getstage.Add(""); //Room 5 
                    getstage.Add("");
                    getstage.Add("");
                    getstage.Add("");
                    getstage.Add("");
                    getstage.Add(""); //Room 10
                    getstage.Add("");
                    getstage.Add("");
                    getstage.Add("");
                    getstage.Add("");
                    getstage.Add(""); //Room 15
                    getstage.Add("");
                    getstage.Add("");
                    getstage.Add("");
                    getstage.Add("");
                    getstage.Add(""); //Room 20
                    getstage.Add("EFFECT TEST");
                    getstage.Add("");
                    getstage.Add("");
                    getstage.Add("");
                    getstage.Add(""); //Room 25
                    getstage.Add("");
                    getstage.Add("");
                    getstage.Add("");
                    getstage.Add("");
                    getstage.Add(""); //Room 30
                    getstage.Add("");
                    getstage.Add("EFF TEST0");
                    getstage.Add("EFF TEST1");
                    getstage.Add("EFF TEST2");
                    getstage.Add("EFF TEST3"); //Room 35
                    getstage.Add("EFF TEST4");
                    getstage.Add("EFF TEST5");
                    getstage.Add("EFF TEST5");
                    break;
                case 1: //Village Part
                    getstage.Add("Der Weg nach Pueblo");
                    getstage.Add("Pueblo");
                    getstage.Add("Geheimer Gang");
                    getstage.Add("Bauernhof");
                    getstage.Add("Ganado-Schlucht");
                    getstage.Add("Mendez' Anwesen"); //Room 5
                    getstage.Add("Haus der Gefangenschaft");
                    getstage.Add("Basis der Ganado");
                    getstage.Add("Friedhof");
                    getstage.Add("Steinbruch");
                    getstage.Add("Sumpf"); //Room 10
                    getstage.Add("See");
                    getstage.Add("Wasserfall");
                    getstage.Add("Höhle am See");
                    getstage.Add("Dock des Händlers");
                    getstage.Add("Dorftor und Gondel"); //Room 15
                    getstage.Add("");
                    getstage.Add("Pueblo");
                    getstage.Add("Geheimer Gang");
                    getstage.Add("Bauernhof");
                    getstage.Add(""); //Room 20
                    getstage.Add("");
                    getstage.Add("");
                    getstage.Add("Kirche");
                    getstage.Add("Friedhof");
                    getstage.Add("Steinbruch"); //Room 25
                    getstage.Add("Sumpf");
                    getstage.Add("See");
                    getstage.Add("Verlassene Hütte");
                    getstage.Add("Festung der Ganado");
                    getstage.Add("Weg des El Gigante"); //Room 30
                    getstage.Add("Schuppen der Erleuchtung");
                    break;
                case 2: //Castle Part
                    getstage.Add("Weg zum Schloss");
                    getstage.Add("Großer Saal");
                    getstage.Add("Burgwälle");
                    getstage.Add("Schlosstor");
                    getstage.Add("Großer Saal");
                    getstage.Add("Gefängnis"); //Room 5
                    getstage.Add("Theatersall");
                    getstage.Add("Eingangshalle");
                    getstage.Add("Anbetungshalle");
                    getstage.Add("Gallerie");
                    getstage.Add("Burgmauer"); //Room 10
                    getstage.Add("Schlafzimmer Garten");
                    getstage.Add("Speisesaal");
                    getstage.Add("Studienzimmer");
                    getstage.Add("Lager");
                    getstage.Add("Ausstellungsraum"); //Room 15
                    getstage.Add("Transportwagen");
                    getstage.Add("Korridor des Königtums");
                    getstage.Add("Waffen-Exponat");
                    getstage.Add("Ballsaal");
                    getstage.Add("Äußerer Uhrenturm"); //Room 20
                    getstage.Add("Thron"); //Needs real name
                    getstage.Add("Rüstungsexponat");
                    getstage.Add("Uhrenturm");
                    getstage.Add("Thronsaal Halle");
                    getstage.Add("Transport Cat"); //Room 25
                    getstage.Add("Unterirdische Ruinen");
                    getstage.Add("Minenbahn");
                    getstage.Add("");
                    getstage.Add("Katakomben");
                    getstage.Add(""); //Room 30
                    getstage.Add("");
                    getstage.Add("Eingang zum Bergwerk");
                    getstage.Add("Service Tunnel");
                    getstage.Add("Laval Raum"); //Needs real name
                    getstage.Add("Ausgrabungsstätte"); //Room 35
                    getstage.Add("Inferno-Kammer");
                    getstage.Add("Ruinen");
                    getstage.Add("Salazar Statuen Raum"); //Needs real name
                    getstage.Add("Salazar's Turm");
                    getstage.Add("Turmspitze"); //Room 40
                    getstage.Add("Sumpf");
                    getstage.Add("Dock");
                    getstage.Add("Fahrstuhl");
                    getstage.Add("Minigame");
                    break;
                case 3: //Island Part
                    getstage.Add("Äußere Festung");
                    getstage.Add("Klippen-Lager");
                    getstage.Add("");
                    getstage.Add("Speisekammer");
                    getstage.Add("Sicherheit");
                    getstage.Add("Bunker"); //Room 5
                    getstage.Add("Treppenhaus");
                    getstage.Add("Operationsraum");
                    getstage.Add("Gefrierschrank");
                    getstage.Add("Forschungslabor");
                    getstage.Add("Kommunkations-Turm"); //Room 10
                    getstage.Add("Abfallentsorgung");
                    getstage.Add("Abstellraum");
                    getstage.Add("Untergeschoss");
                    getstage.Add("Verwaltungskorridor");
                    getstage.Add("Weg zum Anhang"); //Room 15 //Needs real name (Where Ashley drives the big machine)
                    getstage.Add("Staub"); //Needs real name (Where Leon & Ashley jump into the trash)
                    getstage.Add("Verbrennungsanlage");
                    getstage.Add("Kaufmannszimmer");
                    getstage.Add("");
                    getstage.Add(""); //Room 20
                    getstage.Add("Absturzstelle der Planierraupe");
                    getstage.Add("Kapelle und Ofen");
                    getstage.Add("Maschinenraum");
                    getstage.Add("Quartier von Saddler");
                    getstage.Add(""); //Room 25
                    getstage.Add("Kaverneneingang");
                    getstage.Add("Kaverne");
                    getstage.Add("Krausers Festung");
                    getstage.Add("Militärbasis");
                    getstage.Add(""); //Room 30
                    getstage.Add("");
                    getstage.Add("Festung"); //Needs real name (Where Leon's heli partner dies after)
                    getstage.Add("Klippen-Ruinen");
                    getstage.Add("");
                    getstage.Add(""); //Room 35
                    getstage.Add("");
                    getstage.Add("Alte Passage");
                    getstage.Add("Vollzugsanstalt");
                    getstage.Add("Sicherheitskontrollpunkt");
                    getstage.Add(""); //Room 40
                    getstage.Add("Kapselkammer");
                    getstage.Add("");
                    getstage.Add("");
                    getstage.Add("");
                    getstage.Add(""); //Room 45
                    getstage.Add("");
                    getstage.Add("");
                    getstage.Add("Extraktionskammer");
                    getstage.Add("Bauhof");
                    getstage.Add("Bauwerk"); //Room 50
                    getstage.Add("Unterirdische Wasserader"); //Needs real name (Escaping with the boat)
                    break;
                case 4: //Mercenaries & Assignment Ada
                    getstage.Add("Pueblo");
                    getstage.Add("");
                    getstage.Add("Burgwälle");
                    getstage.Add("Militärische Gründe");
                    getstage.Add("Die Docks");
                    getstage.Add("Äußere Festung"); //Room 5
                    getstage.Add("Klippen-Lager");
                    getstage.Add("");
                    getstage.Add("");
                    getstage.Add("");
                    getstage.Add("Treppenhaus"); //Room 10
                    getstage.Add("Operationsraum");
                    getstage.Add("Gefrierschrank");
                    getstage.Add("Forschungslabor");
                    getstage.Add("Kommunikations-Turm");
                    getstage.Add("Abfallentsorgung"); //Room 15
                    getstage.Add("Untergeschoss");
                    getstage.Add("Verwaltungskorridor");
                    break;
                case 5: //Separate Ways
                    getstage.Add("Pueblo");
                    getstage.Add("Geheimer Gang");
                    getstage.Add("Friedhof");
                    getstage.Add("Kirche");
                    getstage.Add("Pueblo");
                    getstage.Add("Bauernhof"); //Room 5
                    getstage.Add("Mendez' Anwesen");
                    getstage.Add("Haus der Gefangenschaft");
                    getstage.Add("Dorftor und Gondel");
                    getstage.Add("Verlassene Hütte");
                    getstage.Add("Weg des El Gigante"); //Room 10
                    getstage.Add("Verlassene Hütte");
                    getstage.Add("Theatersaal");
                    getstage.Add("Burgmauer");
                    getstage.Add("Schlafzimmer Garten");
                    getstage.Add("Galerie"); //Room 15
                    getstage.Add("Ausstellungsraum");
                    getstage.Add("Kommunikations-Turm");
                    getstage.Add("Tunnelweg");
                    getstage.Add("Schiffswerft");
                    getstage.Add("Schiffswerft Fabrik"); //Room 20
                    getstage.Add("Absturzstelle der Planierraupe");
                    getstage.Add("Kapelle und Ofen");
                    getstage.Add("Maschinenraum");
                    getstage.Add("Militärbasis");
                    getstage.Add("Festung"); //Room 25 //Needs real name (Where Leon's heli partner dies after)
                    getstage.Add("Kaverneneingang");
                    getstage.Add("Kaverne");
                    getstage.Add("Sicherheits-Checkpoint");
                    getstage.Add("Kapselkammer");
                    getstage.Add("Bauwerk"); //Room 30
                    getstage.Add("Movie");
                    getstage.Add("");
                    getstage.Add("");
                    getstage.Add("");
                    getstage.Add(""); //Room 35
                    getstage.Add("");
                    getstage.Add("");
                    getstage.Add("");
                    getstage.Add("");
                    getstage.Add(""); //Room 40
                    getstage.Add("");
                    getstage.Add("");
                    getstage.Add("");
                    getstage.Add("");
                    getstage.Add(""); //Room 45
                    getstage.Add("");
                    getstage.Add("");
                    getstage.Add("E3 Stage");
                    getstage.Add("E3 Stage");
                    getstage.Add("E3 Stage"); //Room 50
                    getstage.Add("");
                    getstage.Add("");
                    break;
                default:
                    getstage.Add("Unbekannter Ort");
                    break;
            }
            return getstage.ToArray();
        }
    }
}