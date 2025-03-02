namespace MultiPresence.Models.RE5
{
    public class Weapons
    {
        public static async Task<string> GetWeapon(int weaponid)
        {
            string weapon;

            switch (weaponid)
            {
                case 0:
                    weapon = "Nothing";
                    break;
                case 256:
                case 311:
                    weapon = "Hand-to-Hand";
                    break;
                case 257:
                case 270:
                case 291:
                case 292:
                case 358:
                    weapon = "Knife";
                    break;
                case 258:
                    weapon = "M92F (HG)";
                    break;
                case 259:
                    weapon = "VZ61 (MG)";
                    break;
                case 260:
                    weapon = "Ithaca M37 (SG)";
                    break;
                case 261:
                    weapon = "S75 (RIF)";
                    break;
                case 262:
                case 365:
                    weapon = "Hand Grenade";
                    break;
                case 263:
                    weapon = "Incendiary Grenade";
                    break;
                case 264:
                case 366:
                    weapon = "Flash Grenade";
                    break;
                case 265:
                    weapon = "SIG 556 (MG)";
                    break;
                case 266:
                    weapon = "Proximity Bomb";
                    break;
                case 267:
                    weapon = "S&W M29 (MAG)";
                    break;
                case 268:
                case 343:
                    weapon = "Grenade Launcher";
                    break;
                case 269:
                case 360:
                    weapon = "Rocket Launcher";
                    break;
                case 271:
                    weapon = "Longbow";
                    break;
                case 272:
                    weapon = "H&K P8 (HG)";
                    break;
                case 273:
                    weapon = "SIG P226 (HG)";
                    break;
                case 275:
                    weapon = "H&K MP5 (MG)";
                    break;
                case 277:
                case 349:
                    weapon = "Gatling Gun";
                    break;
                case 278:
                    weapon = "M3 (SG)";
                    break;
                case 279:
                    weapon = "Jail Breaker (SG)";
                    break;
                case 281:
                    weapon = "Hydra (SG)";
                    break;
                case 282:
                    weapon = "L Hawk (MAG)";
                    break;
                case 283:
                    weapon = "S&2 M500 (MAG)";
                    break;
                case 284:
                    weapon = "H&K PSG-1 (RIF)";
                    break;
                case 285:
                    weapon = "AK-74 (MG)";
                    break;
                case 286:
                    weapon = "M93R (HG)";
                    break;
                case 287:
                    weapon = "Px4 (HG)";
                    break;
                case 288:
                    weapon = "Dragunov SVD (RIF)";
                    break;
                case 289:
                    weapon = "Flamethrower";
                    break;
                case 290:
                case 357:
                    weapon = "Stun Rod";
                    break;
                case 293:
                    weapon = "G. Launcher (EXP)";
                    break;
                case 294:
                    weapon = "G. Launcher (ACD)";
                    break;
                case 295:
                    weapon = "G. Launcher (ICE)";
                    break;
                case 297:
                    weapon = "Samurai Edge (HG)";
                    break;
                case 303:
                    weapon = "Gun Turret";
                    break;
                case 304:
                    weapon = "Lantern";
                    break;
                case 308:
                    weapon = "L.T.D.";
                    break;
                case 309:
                    weapon = "RPG-7 NVS";
                    break;
                case 310:
                    weapon = "Egg (Rotten)";
                    break;
                case 313:
                    weapon = "G. Launcher (FLM)";
                    break;
                case 314:
                    weapon = "G. Launcher (FLS)";
                    break;
                case 315:
                    weapon = "G. Launcher (ELC)";
                    break;
                case 316:
                    weapon = "Egg (White)";
                    break;
                case 317:
                    weapon = "Egg (Brown)";
                    break;
                case 318:
                    weapon = "Egg (Gold)";
                    break;
                case 336:
                case 361:
                    weapon = "Adze";
                    break;
                case 337:
                    weapon = "Stickle";
                    break;
                case 338:
                    weapon = "Bow Gun";
                    break;
                case 339:
                    weapon = "Shovel";
                    break;
                case 340:
                    weapon = "Dynamite";
                    break;
                case 341:
                    weapon = "Machete";
                    break;
                case 342:
                    weapon = "Shotgun";
                    break;
                case 344:
                    weapon = "Giant Axe";
                    break;
                case 345:
                    weapon = "Steel Pipe";
                    break;
                case 346:
                    weapon = "Bottle";
                    break;
                case 347:
                    weapon = "Chainsaw";
                    break;
                case 350:
                    weapon = "Torch";
                    break;
                case 351:
                    weapon = "Spear";
                    break;
                case 352:
                    weapon = "Wooden Shield";
                    break;
                case 353:
                    weapon = "Metal Shield";
                    break;
                case 354:
                    weapon = "Bow";
                    break;
                case 355:
                    weapon = "Shield";
                    break;
                case 356:
                    weapon = "Morning Star";
                    break;
                case 359:
                    weapon = "Handgun";
                    break;
                case 362:
                case 374:
                    weapon = "Machine Gun";
                    break;
                case 363:
                    weapon = "Rifle";
                    break;
                case 364:
                    weapon = "Molotov Cocktail";
                    break;
                case 367:
                    weapon = "Spear";
                    break;
                case 368:
                    weapon = "Chair";
                    break;
                case 369:
                    weapon = "Pickaxe";
                    break;
                case 370:
                    weapon = "Club";
                    break;
                case 371:
                    weapon = "Wrench";
                    break;
                case 372:
                    weapon = "Bomb";
                    break;
                case 373:
                    weapon = "Megaphone";
                    break;
                case 513:
                    weapon = "Handgun Ammo";
                    break;
                case 514:
                    weapon = "Machine Gun Ammo";
                    break;
                case 515:
                    weapon = "Shotgun Shells";
                    break;
                case 516:
                    weapon = "Rifle Ammo";
                    break;
                case 518:
                    weapon = "Explosive Rounds";
                    break;
                case 519:
                    weapon = "Acid Rounds";
                    break;
                case 520:
                    weapon = "Nitrogen Rounds";
                    break;
                case 521:
                    weapon = "Magnum Ammo";
                    break;
                case 522:
                    weapon = "Rocket";
                    break;
                case 523:
                    weapon = "Arrow";
                    break;
                case 526:
                    weapon = "Flame Rounds";
                    break;
                case 527:
                    weapon = "Flash Rounds";
                    break;
                case 528:
                    weapon = "Electric Rounds";
                    break;
                case 529:
                    weapon = "RPG Rounds";
                    break;
                case 769:
                    weapon = "Herb (Green)";
                    break;
                case 770:
                    weapon = "Herb (Red)";
                    break;
                case 771:
                    weapon = "Herb (X)";
                    break;
                case 772:
                    weapon = "First Aid Spray";
                    break;
                case 773:
                    weapon = "Herb (Green)";
                    break;
                case 774:
                    weapon = "Herb (Red)";
                    break;
                case 775:
                    weapon = "Herb (G+G)";
                    break;
                case 776:
                    weapon = "Herb (G+G+G)";
                    break;
                case 777:
                    weapon = "Herb (G+R)";
                    break;
                case 778:
                    weapon = "Herb (G+X)";
                    break;
                case 779:
                    weapon = "Herb (G+R+X)";
                    break;
                case 1025:
                    weapon = "Gold (Large)";
                    break;
                case 1026:
                    weapon = "Gold (Small)";
                    break;
                case 1027:
                    weapon = "Gold Bars";
                    break;
                case 1047:
                    weapon = "Gold Ring";
                    break;
                case 1048:
                    weapon = "Dead Bride's Necklace";
                    break;
                case 1049:
                    weapon = "Venom Fang";
                    break;
                case 1050:
                    weapon = "Antique Clock";
                    break;
                case 1051:
                    weapon = "Chalice (Silver)";
                    break;
                case 1052:
                    weapon = "Chalice (Gold)";
                    break;
                case 1053:
                    weapon = "Idol (Silver)";
                    break;
                case 1054:
                    weapon = "Idol (Gold)";
                    break;
                case 1055:
                    weapon = "Ceremonial Mask";
                    break;
                case 1056:
                    weapon = "Ivory Relief";
                    break;
                case 1057:
                    weapon = "Beetle (Brown)";
                    break;
                case 1058:
                    weapon = "Jewel Beetle";
                    break;
                case 1059:
                    weapon = "Royal Necklace";
                    break;
                case 1060:
                    weapon = "Jewel Bangle";
                    break;
                case 1061:
                    weapon = "Beetle (Gold)";
                    break;
                case 1062:
                    weapon = "Beetle (Emerald Green)";
                    break;
                case 1104:
                    weapon = "Topaz (Pear)";
                    break;
                case 1105:
                    weapon = "Ruby (Pear)";
                    break;
                case 1106:
                    weapon = "Sapphire (Pear)";
                    break;
                case 1107:
                    weapon = "Emerald (Pear)";
                    break;
                case 1108:
                    weapon = "Diamond (Pear)";
                    break;
                case 1111:
                    weapon = "Topaz (Square)";
                    break;
                case 1112:
                    weapon = "Ruby (Square)";
                    break;
                case 1113:
                    weapon = "Sapphire (Square)";
                    break;
                case 1114:
                    weapon = "Emerald (Square)";
                    break;
                case 1115:
                    weapon = "Diamond (Square)";
                    break;
                case 1118:
                    weapon = "Topaz (Oval)";
                    break;
                case 1119:
                    weapon = "Ruby (Oval)";
                    break;
                case 1120:
                    weapon = "Sapphire (Oval)";
                    break;
                case 1121:
                    weapon = "Emerald (Oval)";
                    break;
                case 1122:
                    weapon = "Diamond (Oval)";
                    break;
                case 1125:
                    weapon = "Topaz (Trilliant)";
                    break;
                case 1126:
                    weapon = "Ruby (Trilliant)";
                    break;
                case 1127:
                    weapon = "Sapphire (Trilliant)";
                    break;
                case 1128:
                    weapon = "Emerald (Trilliant)";
                    break;
                case 1129:
                    weapon = "Diamond (Trilliant)";
                    break;
                case 1132:
                    weapon = "Power Stone";
                    break;
                case 1133:
                    weapon = "Lion Heart";
                    break;
                case 1134:
                    weapon = "Blue Enigma";
                    break;
                case 1135:
                    weapon = "Soul Gem";
                    break;
                case 1136:
                    weapon = "Heart of Africa";
                    break;
                case 1139:
                    weapon = "Topaz (Marquise)";
                    break;
                case 1140:
                    weapon = "Ruby (Marquise)";
                    break;
                case 1141:
                    weapon = "Sapphire (Marquise)";
                    break;
                case 1142:
                    weapon = "Emerald (Marquise)";
                    break;
                case 1143:
                    weapon = "Diamond (Marquise)";
                    break;
                case 1146:
                    weapon = "Topaz (Brilliant)";
                    break;
                case 1147:
                    weapon = "Ruby (Brilliant)";
                    break;
                case 1148:
                    weapon = "Sapphire (Brilliant)";
                    break;
                case 1149:
                    weapon = "Emerald (Brilliant)";
                    break;
                case 1150:
                    weapon = "Diamond (Brilliant)";
                    break;
                case 1281:
                case 1282:
                    weapon = "Punisher's Key";
                    break;
                case 1283:
                    weapon = "Furnace Key";
                    break;
                case 1285:
                    weapon = "Executio Ground Key";
                    break;
                case 1286:
                    weapon = "Port Key";
                    break;
                case 1287:
                    weapon = "Hangar Keycard A";
                    break;
                case 1288:
                    weapon = "Hangar Keycard B";
                    break;
                case 1289:
                    weapon = "Old Building Key";
                    break;
                case 1290:
                    weapon = "Tanker Keycard A";
                    break;
                case 1294:
                    weapon = "Sky Emblem";
                    break;
                case 1295:
                    weapon = "Earth Emblem";
                    break;
                case 1296:
                    weapon = "Sea Emblem";
                    break;
                case 1298:
                    weapon = "Shaman Slate";
                    break;
                case 1299:
                    weapon = "Warrior Slate";
                    break;
                case 1300:
                    weapon = "Beast Slate";
                    break;
                case 1301:
                    weapon = "Raptor Slate";
                    break;
                case 1302:
                    weapon = "Crane Keycard";
                    break;
                case 1303:
                    weapon = "Tanker Keycard B";
                    break;
                case 1306:
                    weapon = "Guard's Key";
                    break;
                case 1307:
                    weapon = "Bridge Keycard";
                    break;
                case 1537:
                    weapon = "Melee Vest";
                    break;
                case 1542:
                    weapon = "Bulletproof Vest";
                    break;
                case 1543:
                    weapon = "Slate Map";
                    break;
                case 1544:
                    weapon = "Combo Bonus";
                    break;

                default:
                    weapon = "Unknown Weapon";
                    break;
            }

            return weapon;
        }
    }
}