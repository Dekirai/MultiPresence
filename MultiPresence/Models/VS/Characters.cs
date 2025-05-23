﻿namespace MultiPresence.Models.VS
{
    public class Characters
    {
        public static async Task<string> GetCharacter(int character)
        {
            string getcharacter;

            switch (character)
            {
                case 1:
                    getcharacter = "Antonio Belpaese";
                    break;
                case 2:
                    getcharacter = "Imelda Belpaese";
                    break;
                case 3:
                    getcharacter = "Pasqualina Belpaese";
                    break;
                case 4:
                    getcharacter = "Gennaro Belpaese";
                    break;
                case 5:
                    getcharacter = "Christine Davain";
                    break;
                case 6:
                    getcharacter = "Arca Ladonna";
                    break;
                case 7:
                    getcharacter = "Porta Ladonna";
                    break;
                case 8:
                    getcharacter = "Exdash Exiviiq";
                    break;
                case 9:
                    getcharacter = "Mortaccio";
                    break;
                case 10:
                    getcharacter = "Yatta Cavallo";
                    break;
                case 11:
                    getcharacter = "Poe Ratcho";
                    break;
                case 13:
                    getcharacter = "Lama Ladonna";
                    break;
                case 14:
                    getcharacter = "Dommario";
                    break;
                case 15:
                    getcharacter = "Krochi Freetto";
                    break;
                case 16:
                    getcharacter = "Suor Clerici";
                    break;
                case 18:
                    getcharacter = "Poppea Pecorina";
                    break;
                case 19:
                    getcharacter = "Pugnala Provola";
                    break;
                case 20:
                    getcharacter = "Giovanna Grana";
                    break;
                case 21:
                    getcharacter = "Concetta Cacioatta";
                    break;
                case 22:
                    getcharacter = "Zi'Assunta Belpaese";
                    break;
                case 25:
                    getcharacter = "Toastie";
                    break;
                case 26:
                    getcharacter = "Minnah Mannarah";
                    break;
                case 28:
                    getcharacter = "Bat Robbert";
                    break;
                case 29:
                    getcharacter = "Leda";
                    break;
                case 30:
                    getcharacter = "O'Sole Meeo";
                    break;
                case 39:
                    getcharacter = "Divano Thelma";
                    break;
                case 40:
                    getcharacter = "Bianca Ramba";
                    break;
                case 45:
                    getcharacter = "Mask of the Red Death";
                    break;
                case 46:
                    getcharacter = "MissingN";
                    break;
                case 47:
                    getcharacter = "Iguana Gallo Valletto";
                    break;
                case 48:
                    getcharacter = "Queen Sigma";
                    break;
                case 49:
                    getcharacter = "Avatar Infernas";
                    break;
                case 52:
                    getcharacter = "Peppino";
                    break;
                case 53:
                    getcharacter = "Smith IV";
                    break;
                case 54:
                    getcharacter = "Boon Marrabbio";
                    break;
                case 59:
                    getcharacter = "Sir Ambrojoe";
                    break;
                case 60:
                    getcharacter = "Gains Boros";
                    break;
                case 61:
                    getcharacter = "Big Trouser";
                    break;
                case 62:
                    getcharacter = "Cosmo Pavone";
                    break;
                case 63:
                    getcharacter = "Gyorunton";
                    break;
                case 64:
                    getcharacter = "Random";
                    break;
                case 65:
                    getcharacter = "Scorej-Oni";
                    break;
                case 66:
                    getcharacter = "Miang Moonspell";
                    break;
                case 67:
                    getcharacter = "Menya Moonspell";
                    break;
                case 68:
                    getcharacter = "Syuuto Moonspell";
                    break;
                case 69:
                    getcharacter = "Babi-Onna";
                    break;
                case 70:
                    getcharacter = "McCoy-Oni";
                    break;
                case 71:
                    getcharacter = "Megalo Menya Moonspell";
                    break;
                case 72:
                    getcharacter = "Megalo Syuuto Moonspell";
                    break;
                case 73:
                    getcharacter = "Gav'Et-Oni";
                    break;
                case 75:
                    getcharacter = "Eleanor Uziron";
                    break;
                case 76:
                    getcharacter = "Luminaire Foscari";
                    break;
                case 77:
                    getcharacter = "Genevieve Gruyére";
                    break;
                case 78:
                    getcharacter = "Je-Ne-Viv";
                    break;
                case 79:
                    getcharacter = "Maruto Cuts";
                    break;
                case 80:
                    getcharacter = "Keitha Muort";
                    break;
                case 81:
                    getcharacter = "Rottin'Ghoul";
                    break;
                case 84:
                    getcharacter = "Sammy";
                    break;
                case 85:
                    getcharacter = "She-Moon Eeta";
                    break;
                case 86:
                    getcharacter = "Space Dude";
                    break;
                case 89:
                    getcharacter = "Santa Ladonna";
                    break;
                case 93:
                    getcharacter = "Gyoruntin";
                    break;
                case 100:
                    getcharacter = "Bats Bats Bats";
                    break;
                case 101:
                    getcharacter = "Rose De Infernas";
                    break;
                case 109:
                    getcharacter = "Crewmate Dino";
                    break;
                case 110:
                    getcharacter = "Impostor Rina";
                    break;
                case 111:
                    getcharacter = "Ghost Lino";
                    break;
                case 112:
                    getcharacter = "Guardian Pina";
                    break;
                case 113:
                    getcharacter = "Engineer Gino";
                    break;
                case 114:
                    getcharacter = "Scientist Mina";
                    break;
                case 115:
                    getcharacter = "Shapeshifter Nino";
                    break;
                case 116:
                    getcharacter = "Horse";
                    break;
                case 119:
                    getcharacter = "Megalo Impostor Rina";
                    break;
                case 120:
                    getcharacter = "Bill Rizer";
                    break;
                case 121:
                    getcharacter = "Lance Bean";
                    break;
                case 122:
                    getcharacter = "Brad Fang";
                    break;
                case 123:
                    getcharacter = "Browny";
                    break;
                case 124:
                    getcharacter = "Lucia Zero";
                    break;
                case 125:
                    getcharacter = "Probotector";
                    break;
                case 126:
                    getcharacter = "Sheena Etranzi";
                    break;
                case 127:
                    getcharacter = "Stanley";
                    break;
                case 128:
                    getcharacter = "Ariana";
                    break;
                case 129:
                    getcharacter = "Colonel Bahamut";
                    break;
                case 130:
                    getcharacter = "Simondo Belmont";
                    break;
                case 131:
                    getcharacter = "Newt Plissken";
                    break;
                case 143:
                    getcharacter = "Tsunanori Mido";
                    break;
                case 148:
                    getcharacter = "Ameya Aisling";
                    break;
                case 149:
                    getcharacter = "Bonnie Blair";
                    break;
                case 150:
                    getcharacter = "Formina Franklyn";
                    break;
                case 151:
                    getcharacter = "Diva No. 5 \"Retro Pod\"";
                    break;
                case 152:
                    getcharacter = "Siugnas";
                    break;
                case 153:
                    getcharacter = "Mr. S";
                    break;
                case 154:
                    getcharacter = "Final Emperor";
                    break;
                case 155:
                    getcharacter = "Macha Alter Ego";
                    break;
                case 156:
                    getcharacter = "Dolores";
                    break;
                case 157:
                    getcharacter = "Lita Caryx";
                    break;
                case 158:
                    getcharacter = "Kugutsu : Musashi";
                    break;
                case 163:
                    getcharacter = "Lolo, Hiss, Meow, and Purr";
                    break;
                case 164:
                    getcharacter = "Malevolent Door Spirit";
                    break;
                case 165:
                    getcharacter = "Imakoo";
                    break;
                case 166:
                    getcharacter = "Kina";
                    break;
                case 202:
                    getcharacter = "Alucard";
                    break;
                case 206:
                    getcharacter = "Charlotte Aulin";
                    break;
                case 211:
                    getcharacter = "Eric Lecarde";
                    break;
                case 213:
                    getcharacter = "Grant Danasty";
                    break;
                case 214:
                    getcharacter = "Hector";
                    break;
                case 217:
                    getcharacter = "Jonathan Morris";
                    break;
                case 218:
                    getcharacter = "John Morris";
                    break;
                case 219:
                    getcharacter = "Julius Belmont";
                    break;
                case 221:
                    getcharacter = "Juste Belmont";
                    break;
                case 222:
                    getcharacter = "Leon Belmont";
                    break;
                case 224:
                    getcharacter = "Maria Renard";
                    break;
                case 229:
                    getcharacter = "Richter Belmont";
                    break;
                case 231:
                    getcharacter = "Shanoa";
                    break;
                case 232:
                    getcharacter = "Simon Belmont";
                    break;
                case 233:
                    getcharacter = "Soma Cruz";
                    break;
                case 234:
                    getcharacter = "Sonia Belmont";
                    break;
                case 238:
                    getcharacter = "Sypha Belnades";
                    break;
                case 239:
                    getcharacter = "Trevor Belmont";
                    break;
                case 240:
                    getcharacter = "Yoko Belnades";
                    break;
                case 241:
                    getcharacter = "Christopher Belmont";
                    break;
                case 242:
                    getcharacter = "Sara Trantoul";
                    break;
                case 243:
                    getcharacter = "Dario Bossi";
                    break;
                case 244:
                    getcharacter = "Dmitrii Blinov";
                    break;
                case 245:
                    getcharacter = "Celia Fortner";
                    break;
                case 246:
                    getcharacter = "Rinaldo Gandolfi";
                    break;
                case 247:
                    getcharacter = "Mina Hakuba";
                    break;
                case 248:
                    getcharacter = "Quincy Morris";
                    break;
                case 249:
                    getcharacter = "Maxim Kischine";
                    break;
                case 250:
                    getcharacter = "Lisa Tepes";
                    break;
                case 251:
                    getcharacter = "Vincent Dorin";
                    break;
                case 252:
                    getcharacter = "Brauner";
                    break;
                case 253:
                    getcharacter = "Barlowe";
                    break;
                case 254:
                    getcharacter = "Wind";
                    break;
                case 256:
                    getcharacter = "Master Librarian";
                    break;
                case 257:
                    getcharacter = "Hammer";
                    break;
                case 258:
                    getcharacter = "Graham Jones";
                    break;
                case 259:
                    getcharacter = "Soleil Belmont";
                    break;
                case 260:
                    getcharacter = "Joachim Armster";
                    break;
                case 261:
                    getcharacter = "Cave Troll";
                    break;
                case 262:
                    getcharacter = "Ferryman";
                    break;
                case 263:
                    getcharacter = "Malphas";
                    break;
                case 264:
                    getcharacter = "Walter Bernhard";
                    break;
                case 265:
                    getcharacter = "Frozenshade";
                    break;
                case 266:
                    getcharacter = "Count Olrox";
                    break;
                case 267:
                    getcharacter = "Alamaric Sniper";
                    break;
                case 268:
                    getcharacter = "Fleaman";
                    break;
                case 269:
                    getcharacter = "Blackmore";
                    break;
                case 270:
                    getcharacter = "Jonathan Morris";
                    break;
                case 271:
                    getcharacter = "Charlotte Aulin";
                    break;
                case 272:
                    getcharacter = "Stella Lecarde";
                    break;
                case 273:
                    getcharacter = "Loretta Lecarde";
                    break;
                case 274:
                    getcharacter = "Blue Crescent Moon Cornell";
                    break;
                case 275:
                    getcharacter = "Megalo Elizabeth Bartley";
                    break;
                case 276:
                    getcharacter = "Megalo Death";
                    break;
                case 277:
                    getcharacter = "Keremet";
                    break;
                case 278:
                    getcharacter = "Megalo Dracula";
                    break;
                case 279:
                    getcharacter = "Megalo Olrox";
                    break;

                default:
                    getcharacter = "Unknown";
                    break;
            }
            return getcharacter;
        }
    }
}