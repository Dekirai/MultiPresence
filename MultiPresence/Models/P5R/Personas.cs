namespace MultiPresence.Models.P5R
{
    public class Personas
    {
        public static async Task<string> GetPersona(int personaid)
        {
            string persona;

            switch (personaid)
            {
                case 0:
                    persona = "None";
                    break;
                case 59:
                    persona = "Abaddon [Judgement]";
                    break;
                case 131:
                    persona = "Agathion [Chariot]";
                    break;
                case 246:
                    persona = "Agnes [Pristess] (Queen)";
                    break;
                case 248:
                    persona = "Al Azif [Hermit] (Oracle)";
                    break;
                case 47:
                    persona = "Alice [Death]";
                    break;
                case 425:
                    persona = "Alilat [Empress]";
                    break;
                case 89:
                    persona = "Ame-no-Uzume [Lovers]";
                    break;
                case 300:
                    persona = "Ananta [Councillor]";
                    break;
                case 216:
                    persona = "Anat [Pristess] (Queen)";
                    break;
                case 102:
                    persona = "Andras [Devil]";
                    break;
                case 76:
                    persona = "Angel [Justice]";
                    break;
                case 22:
                    persona = "Anubis [Judgement]";
                    break;
                case 85:
                    persona = "Anzu [Hierophant]";
                    break;
                case 101:
                    persona = "Apsaras [Pristess]";
                    break;
                case 153:
                    persona = "Ara Mitama [Chariot]";
                    break;
                case 75:
                    persona = "Arahabaki [Hermit]";
                    break;
                case 55:
                    persona = "Archangel [Justice]";
                    break;
                case 164:
                    persona = "Ardha [Temperance]";
                    break;
                case 186:
                    persona = "Ariadne [Fortune] 🎁";
                    break;
                case 196:
                    persona = "Ariadne Picaro [Fortune] 🎁";
                    break;
                case 201:
                    persona = "Arsene [Fool]";
                    break;
                case 217:
                    persona = "Astarte [Empress] (Noir)";
                    break;
                case 187:
                    persona = "Asterius [Fortune] 🎁";
                    break;
                case 197:
                    persona = "Asterius Picaro [Fortune] 🎁";
                    break;
                case 158:
                    persona = "Asura [Sun]";
                    break;
                case 435:
                    persona = "Atavaka [Faith]";
                    break;
                case 362:
                    persona = "Athena [Chariot] 🎁";
                    break;
                case 368:
                    persona = "Athena Picaro [Chariot] 🎁";
                    break;
                case 163:
                    persona = "Atropos [Fortune]";
                    break;
                case 289:
                    persona = "Attis [Hanged Man]";
                    break;
                case 122:
                    persona = "Baal [Emperor]";
                    break;
                case 136:
                    persona = "Baphomet [Devil]";
                    break;
                case 27:
                    persona = "Barong [Emperor]";
                    break;
                case 2:
                    persona = "Beelzebub [Devil]";
                    break;
                case 62:
                    persona = "Belial [Devil]";
                    break;
                case 142:
                    persona = "Belphegor [Tower]";
                    break;
                case 143:
                    persona = "Berith [Hierophant]";
                    break;
                case 127:
                    persona = "Bicorn [Hermit]";
                    break;
                case 277:
                    persona = "Bishamonten [Hierophant]";
                    break;
                case 74:
                    persona = "Black Frost [Fool]";
                    break;
                case 126:
                    persona = "Black Ooze [Moon]";
                    break;
                case 305:
                    persona = "Black Rider [Tower]";
                    break;
                case 125:
                    persona = "Bugs [Fool]";
                    break;
                case 428:
                    persona = "Byakhee [Moon]";
                    break;
                case 276:
                    persona = "Byakko [Temperance]";
                    break;
                case 436:
                    persona = "Caith Sith [Magician]";
                    break;
                case 202:
                    persona = "Captain Kidd [Chariot] (Skull)";
                    break;
                case 204:
                    persona = "Carmen [Lovers] (Panther)";
                    break;
                case 244:
                    persona = "Celestine [Lovers] (Panther)";
                    break;
                case 240:
                    persona = "Cendrillon [Faith] (Violet)";
                    break;
                case 7:
                    persona = "Cerberus [Chariot]";
                    break;
                case 65:
                    persona = "Chernobog [Death]";
                    break;
                case 79:
                    persona = "Chi You [Chariot]";
                    break;
                case 434:
                    persona = "Chimera [Strength]";
                    break;
                case 144:
                    persona = "Choronzon [Magician]";
                    break;
                case 161:
                    persona = "Clotho [Fortune]";
                    break;
                case 113:
                    persona = "Crystal Skull [Fool] 💎";
                    break;
                case 3:
                    persona = "Cu Chulainn [Faith]";
                    break;
                case 64:
                    persona = "Cybele [Pristess]";
                    break;
                case 282:
                    persona = "Daisoujou [Hierophant]";
                    break;
                case 123:
                    persona = "Dakini [Empress]";
                    break;
                case 12:
                    persona = "Decarabia [Councillor]";
                    break;
                case 243:
                    persona = "Diego [Magician] (Mona)";
                    break;
                case 100:
                    persona = "Dionysus [Councillor]";
                    break;
                case 72:
                    persona = "Dominion [Justice]";
                    break;
                case 9:
                    persona = "Eligor [Emperor]";
                    break;
                case 250:
                    persona = "Ella [Faith] (Violet)";
                    break;
                case 111:
                    persona = "Emperor's Amulet [Hanged Man] 💎";
                    break;
                case 427:
                    persona = "Fafnir [Hermit]";
                    break;
                case 66:
                    persona = "Flauros [Devil]";
                    break;
                case 36:
                    persona = "Forneus [Magician]";
                    break;
                case 272:
                    persona = "Fortuna [Fortune]";
                    break;
                case 259:
                    persona = "Futsunushi [Magician]";
                    break;
                case 95:
                    persona = "Fuu-Ki [Star]";
                    break;
                case 45:
                    persona = "Gabriel [Temperance]";
                    break;
                case 21:
                    persona = "Ganesha [Sun]";
                    break;
                case 139:
                    persona = "Garuda [Star]";
                    break;
                case 275:
                    persona = "Genbu [Temperance]";
                    break;
                case 28:
                    persona = "Girimehkala [Moon]";
                    break;
                case 205:
                    persona = "Goemon [Emperor] (Fox)";
                    break;
                case 245:
                    persona = "Gorokichi [Emperor] (Fox)";
                    break;
                case 54:
                    persona = "Hanuman [Strength]";
                    break;
                case 167:
                    persona = "Hariti [Empress]";
                    break;
                case 429:
                    persona = "Hastur [Star]";
                    break;
                case 214:
                    persona = "Hecate [Lovers] (Panther)";
                    break;
                case 169:
                    persona = "Hecatoncheires [Hanged Man]";
                    break;
                case 281:
                    persona = "Hell Biker [Death]";
                    break;
                case 249:
                    persona = "Hereward [Justice] (Crow)";
                    break;
                case 26:
                    persona = "High Pixie [Fool]";
                    break;
                case 112:
                    persona = "Hope Diamond [Death] 💎";
                    break;
                case 287:
                    persona = "Horus [Sun]";
                    break;
                case 11:
                    persona = "Hua Po [Hanged Man]";
                    break;
                case 39:
                    persona = "Incubus [Devil]";
                    break;
                case 81:
                    persona = "Inugami [Hanged Man]";
                    break;
                case 67:
                    persona = "Ippon-Datara [Hermit]";
                    break;
                case 262:
                    persona = "Ishtar [Lovers]";
                    break;
                case 31:
                    persona = "Isis [Pristess]";
                    break;
                case 183:
                    persona = "Izanagi [Fool] 🎁";
                    break;
                case 193:
                    persona = "Izanagi Picaro [Fool] 🎁";
                    break;
                case 360:
                    persona = "Izanagi-no-Okami [World] 🎁";
                    break;
                case 366:
                    persona = "Izanagi-no-Okami Picaro [World] 🎁";
                    break;
                case 5:
                    persona = "Jack Frost [Magician]";
                    break;
                case 4:
                    persona = "Jack-o'-Lantern [Magician]";
                    break;
                case 96:
                    persona = "Jatayu [Hanged Man]";
                    break;
                case 279:
                    persona = "Jikokuten [Temperance]";
                    break;
                case 206:
                    persona = "Johanna [Pristess] (Queen)";
                    break;
                case 185:
                    persona = "Kaguya [Moon] 🎁";
                    break;
                case 195:
                    persona = "Kaguya Picaro [Moon] 🎁";
                    break;
                case 97:
                    persona = "Kaiwan [Star]";
                    break;
                case 48:
                    persona = "Kali [Empress]";
                    break;
                case 215:
                    persona = "Kamu Susano-o [Emperor] (Fox)";
                    break;
                case 98:
                    persona = "Kelpie [Strength]";
                    break;
                case 78:
                    persona = "Kikuri-Hime [Pristess]";
                    break;
                case 29:
                    persona = "King Frost [Emperor]";
                    break;
                case 93:
                    persona = "Kin-Ki [Chariot]";
                    break;
                case 130:
                    persona = "Kodama [Star]";
                    break;
                case 109:
                    persona = "Koh-i-Noor [Pristess] 💎";
                    break;
                case 254:
                    persona = "Kohryu [Hierophant]";
                    break;
                case 105:
                    persona = "Koppa Tengu [Temperance]";
                    break;
                case 104:
                    persona = "Koropokguru [Hermit]";
                    break;
                case 278:
                    persona = "Koumokuten [Hermit]";
                    break;
                case 91:
                    persona = "Kumbhanda [Hermit]";
                    break;
                case 49:
                    persona = "Kurama Tengu [Hermit]";
                    break;
                case 152:
                    persona = "Kushi Mitama [Councillor]";
                    break;
                case 90:
                    persona = "Kushinada [Lovers]";
                    break;
                case 162:
                    persona = "Lachesis [Fortune]";
                    break;
                case 266:
                    persona = "Lakshmi [Fortune]";
                    break;
                case 32:
                    persona = "Lamia [Empress]";
                    break;
                case 63:
                    persona = "Leanan Sidhe [Lovers]";
                    break;
                case 33:
                    persona = "Legion [Fool]";
                    break;
                case 8:
                    persona = "Lilim [Devil]";
                    break;
                case 41:
                    persona = "Lilith [Moon]";
                    break;
                case 433:
                    persona = "Loa [Hermit]";
                    break;
                case 239:
                    persona = "Loki [Justice] (Crow)";
                    break;
                case 253:
                    persona = "Lucifer [Star]";
                    break;
                case 247:
                    persona = "Lucy [Empress] (Noir)";
                    break;
                case 424:
                    persona = "Macabre [Hanged Man]";
                    break;
                case 159:
                    persona = "Mada [Tower]";
                    break;
                case 184:
                    persona = "Magatsu-Izanagi [Tower] 🎁";
                    break;
                case 194:
                    persona = "Magatsu-Izanagi Picaro [Tower] 🎁";
                    break;
                case 43:
                    persona = "Makami [Temperance]";
                    break;
                case 121:
                    persona = "Mandrake [Death]";
                    break;
                case 13:
                    persona = "Mara [Tower]";
                    break;
                case 330:
                    persona = "Maria [Faith]";
                    break;
                case 285:
                    persona = "Matador [Death]";
                    break;
                case 135:
                    persona = "Melchizedek [Justice]";
                    break;
                case 213:
                    persona = "Mercurius [Magician] (Mona)";
                    break;
                case 189:
                    persona = "Messiah [Judgement] 🎁";
                    break;
                case 190:
                    persona = "Messiah Picaro [Judgement] 🎁";
                    break;
                case 1:
                    persona = "Metatron [Justice]";
                    break;
                case 157:
                    persona = "Michael [Judgement]";
                    break;
                case 207:
                    persona = "Milady [Empress] (Noir)";
                    break;
                case 333:
                    persona = "Mishaguji [Hierophant]";
                    break;
                case 128:
                    persona = "Mithras [Sun]";
                    break;
                case 295:
                    persona = "Mitra [Temperance]";
                    break;
                case 35:
                    persona = "Mokoi [Death]";
                    break;
                case 140:
                    persona = "Moloch [Hanged Man]";
                    break;
                case 23:
                    persona = "Mot [Death]";
                    break;
                case 160:
                    persona = "Mother Harlot [Empress]";
                    break;
                case 70:
                    persona = "Mothman [Moon]";
                    break;
                case 138:
                    persona = "Naga [Hermit]";
                    break;
                case 30:
                    persona = "Narcissus [Lovers]";
                    break;
                case 82:
                    persona = "Nebiros [Devil]";
                    break;
                case 208:
                    persona = "Necronomicon [Hermit] (Oracle)";
                    break;
                case 298:
                    persona = "Neko Shogun [Star]";
                    break;
                case 73:
                    persona = "Nekomata [Magician]";
                    break;
                case 151:
                    persona = "Nigi Mitama [Councillor]";
                    break;
                case 141:
                    persona = "Norn [Fortune]";
                    break;
                case 133:
                    persona = "Nue [Death]";
                    break;
                case 56:
                    persona = "Obariyon [Fool]";
                    break;
                case 50:
                    persona = "Oberon [Emperor]";
                    break;
                case 10:
                    persona = "Odin [Emperor]";
                    break;
                case 255:
                    persona = "Okuninushi [Faith]";
                    break;
                case 92:
                    persona = "Ongyo-Ki [Hermit]";
                    break;
                case 40:
                    persona = "Oni [Strength]";
                    break;
                case 132:
                    persona = "Onmoraki [Moon]";
                    break;
                case 114:
                    persona = "Orichalcum [Faith] 💎";
                    break;
                case 110:
                    persona = "Orlov [Strength] 💎";
                    break;
                case 53:
                    persona = "Orobas [Hierophant]";
                    break;
                case 181:
                    persona = "Orpheus (M) [Fool] 🎁";
                    break;
                case 191:
                    persona = "Orpheus (M) Picaro [Fool] 🎁";
                    break;
                case 365:
                    persona = "Orpheus (F) [Fool] 🎁";
                    break;
                case 371:
                    persona = "Orpheus (F) Picaro [Fool] 🎁";
                    break;
                case 68:
                    persona = "Orthrus [Hanged Man]";
                    break;
                case 14:
                    persona = "Ose [Fool]";
                    break;
                case 286:
                    persona = "Pale Rider [Death]";
                    break;
                case 44:
                    persona = "Parvati [Lovers]";
                    break;
                case 308:
                    persona = "Pazuzu [Devil]";
                    break;
                case 296:
                    persona = "Phoenix [Faith]";
                    break;
                case 134:
                    persona = "Pisaca [Death]";
                    break;
                case 6:
                    persona = "Pixie [Lovers]";
                    break;
                case 80:
                    persona = "Power [Justice]";
                    break;
                case 297:
                    persona = "Principality [Justice]";
                    break;
                case 218:
                    persona = "Prometheus [Hermit] (Oracle)";
                    break;
                case 57:
                    persona = "Queen Mab [Magician]";
                    break;
                case 107:
                    persona = "Queen's Necklace [Empress] 💎";
                    break;
                case 303:
                    persona = "Quetzalcoatl [Sun]";
                    break;
                case 137:
                    persona = "Raja Naga [Temperance]";
                    break;
                case 34:
                    persona = "Rakshasa [Strength]";
                    break;
                case 42:
                    persona = "Rangda [Magician]";
                    break;
                case 363:
                    persona = "Raoul [Fool] 🎁";
                    break;
                case 24:
                    persona = "Raphael [Lovers]";
                    break;
                case 304:
                    persona = "Red Rider [Tower]";
                    break;
                case 106:
                    persona = "Regent [Emperor] 💎";
                    break;
                case 209:
                    persona = "Robin Hood [Justice] (Crow)";
                    break;
                case 154:
                    persona = "Saki Mitama [Lovers]";
                    break;
                case 58:
                    persona = "Sandalphon [Moon]";
                    break;
                case 61:
                    persona = "Sandman [Magician]";
                    break;
                case 18:
                    persona = "Sarasvati [Pristess]";
                    break;
                case 252:
                    persona = "Satan [Judgement]";
                    break;
                case 170:
                    persona = "Satanael [Fool]";
                    break;
                case 25:
                    persona = "Scathach [Pristess]";
                    break;
                case 274:
                    persona = "Seiryu [Councillor]";
                    break;
                case 212:
                    persona = "Seiten Taisei [Chariot] (Skull)";
                    break;
                case 37:
                    persona = "Setanta [Emperor]";
                    break;
                case 261:
                    persona = "Seth [Tower]";
                    break;
                case 60:
                    persona = "Shiisaa [Strength]";
                    break;
                case 51:
                    persona = "Shiki-Ouji [Chariot]";
                    break;
                case 156:
                    persona = "Shiva [Judgement]";
                    break;
                case 437:
                    persona = "Siegfried [Faith]";
                    break;
                case 124:
                    persona = "Silky [Pristess]";
                    break;
                case 77:
                    persona = "Skadi [Pristess]";
                    break;
                case 84:
                    persona = "Slime [Chariot]";
                    break;
                case 292:
                    persona = "Sraosha [Star]";
                    break;
                case 108:
                    persona = "Stone of Scone [Fortune] 💎";
                    break;
                case 69:
                    persona = "Succubus [Moon]";
                    break;
                case 129:
                    persona = "Sudama [Hermit]";
                    break;
                case 94:
                    persona = "Sui-Ki [Moon]";
                    break;
                case 432:
                    persona = "Surt [Magician]";
                    break;
                case 273:
                    persona = "Suzaku [Sun]";
                    break;
                case 88:
                    persona = "Take-Minakata [Hanged Man]";
                    break;
                case 430:
                    persona = "Tam Lin [Faith]";
                    break;
                case 182:
                    persona = "Thanatos [Death] 🎁";
                    break;
                case 192:
                    persona = "Thanatos Picaro [Death] 🎁";
                    break;
                case 15:
                    persona = "Thor [Chariot]";
                    break;
                case 99:
                    persona = "Thoth [Emperor]";
                    break;
                case 431:
                    persona = "Throne [Justice]";
                    break;
                case 426:
                    persona = "Thunderbird [Sun]";
                    break;
                case 38:
                    persona = "Titania [Empress]";
                    break;
                case 283:
                    persona = "Trumpeter [Judgement]";
                    break;
                case 188:
                    persona = "Tsukiyomi [Moon] 🎁";
                    break;
                case 198:
                    persona = "Tsukiyomi Picaro [Moon] 🎁";
                    break;
                case 16:
                    persona = "Unicorn [Faith]";
                    break;
                case 17:
                    persona = "Uriel [Justice]";
                    break;
                case 19:
                    persona = "Valkyrie [Strength]";
                    break;
                case 241:
                    persona = "Vanadis [Faith] (Violet)";
                    break;
                case 299:
                    persona = "Vasuki [Star]";
                    break;
                case 166:
                    persona = "Vishnu [Fool]";
                    break;
                case 331:
                    persona = "Vohu Manah [Councillor]";
                    break;
                case 284:
                    persona = "White Rider [Chariot]";
                    break;
                case 242:
                    persona = "William [Chariot] (Skull)";
                    break;
                case 20:
                    persona = "Yaksini [Empress]";
                    break;
                case 52:
                    persona = "Yamata-no-Orochi [Judgement]";
                    break;
                case 86:
                    persona = "Yatagarasu [Councillor]";
                    break;
                case 87:
                    persona = "Yoshitsune [Tower]";
                    break;
                case 168:
                    persona = "Yurlungur [Sun]";
                    break;
                case 46:
                    persona = "Zaou-Gongen [Strength]";
                    break;
                case 203:
                    persona = "Zorro [Magician] (Mona)";
                    break;
                case 280:
                    persona = "Zouchouten [Strength]";
                    break;

                default:
                    persona = "Unknown persona";
                    break;
            }

            return persona;
        }
    }
}
