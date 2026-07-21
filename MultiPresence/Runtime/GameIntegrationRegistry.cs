using MultiPresence.Presence;

namespace MultiPresence.Runtime;

internal static class GameIntegrationRegistry
{
    private static readonly IReadOnlyDictionary<string, Func<Task>> Integrations =
        new Dictionary<string, Func<Task>>(StringComparer.OrdinalIgnoreCase)
        {
            ["Borderlands 1"] = Sync(BL1.DoAction),
            ["Borderlands 2"] = Sync(BL2.DoAction),
            ["Call of Duty®"] = COD.DoAction,
            ["CODE VEIN"] = Sync(CV.DoAction),
            ["Crash Bandicoot 4: It's About Time"] = Sync(CB4.DoAction),
            ["Crash Bandicoot N. Sane Trilogy"] = Sync(CBNT.DoAction),
            ["CRISIS CORE –FINAL FANTASY VII– REUNION"] = Sync(CCFFVII.DoAction),
            ["Dark Souls II"] = Sync(DSII.DoAction),
            ["Dark Souls III"] = Sync(DSIII.DoAction),
            ["Dark Souls: Remastered"] = Sync(DSR.DoAction),
            ["Death Stranding"] = Sync(DSDC.DoAction),
            ["Devil May Cry"] = DMC1.DoAction,
            ["Devil May Cry 2"] = DMC2.DoAction,
            ["Devil May Cry 3"] = DMC3.DoAction,
            ["Devil May Cry 4"] = DMC4.DoAction,
            ["Devil May Cry 5"] = DMC5.DoAction,
            ["Diablo IV"] = D4.DoAction,
            ["Digimon Story Time Stranger"] = Sync(DSTS.DoAction),
            ["DmC Devil May Cry"] = DMC.DoAction,
            ["Elden Ring"] = Sync(ER.DoAction),
            ["Final Fantasy VII Rebirth"] = Sync(FFVIIRB.DoAction),
            ["Final Fantasy VII Remake"] = Sync(FFVIIR.DoAction),
            ["Final Fantasy X"] = FFX.DoAction,
            ["Final Fantasy XV"] = FFXV.DoAction,
            ["Final Fantasy XVI"] = FFXVI.DoAction,
            ["Granblue Fantasy: Relink"] = Sync(GBFR.DoAction),
            ["Gunfire Reborn"] = GFR.DoAction,
            ["Hello Kitty Island Adventure"] = HK.DoAction,
            ["Hogwarts Legacy"] = HL.DoAction,
            ["Kingdom Hearts Birth by Sleep Final Mix"] = Sync(KHBBS.DoAction),
            ["Kingdom Hearts Dream Drop Distance"] = Sync(KHDDD.DoAction),
            ["Kingdom Hearts Final Mix"] = Sync(KH1.DoAction),
            ["Kingdom Hearts II Final Mix"] = Sync(KH2.DoAction),
            ["Kingdom Hearts III"] = KH3.DoAction,
            ["Kingdom Hearts Re:Chain of Memories"] = Sync(KHCOM.DoAction),
            ["Labyrinthine"] = LR.DoAction,
            ["Lies of P"] = Sync(LOP.DoAction),
            ["Mega Man 11"] = Sync(MM11.DoAction),
            ["Mega Man Battle Network"] = Sync(MMBN1.DoAction),
            ["Mega Man Battle Network 2"] = Sync(MMBN2.DoAction),
            ["Mega Man Battle Network 3"] = Sync(MMBN3.DoAction),
            ["Mega Man Battle Network 4"] = Sync(MMBN4.DoAction),
            ["Mega Man Battle Network 5"] = Sync(MMBN5.DoAction),
            ["Mega Man Battle Network 6"] = Sync(MMBN6.DoAction),
            ["Mega Man X5"] = Sync(MMX5.DoAction),
            ["Mega Man X6"] = Sync(MMX6.DoAction),
            ["Mega Man X7"] = Sync(MMX7.DoAction),
            ["Mega Man X8"] = Sync(MMX8.DoAction),
            ["Marvel's Spider-Man Remastered"] = Sync(MSMR.DoAction),
            ["Marvel's Spider-Man: Miles Morales"] = Sync(MSMMMM.DoAction),
            ["Overwatch"] = OW.DoAction,
            ["Pangya Reborn"] = Sync(PYRE.DoAction),
            ["Persona 4 Golden"] = Sync(P4G.DoAction),
            ["Persona 5 Royal"] = Sync(P5R.DoAction),
            ["Persona 5 Strikers"] = Sync(P5S.DoAction),
            ["Persona 5 Tactica"] = Sync(P5T.DoAction),
            ["Persona 5: The Phantom X"] = Sync(P5X.DoAction),
            ["Project Diva Mega Mix+"] = Sync(PDMM.DoAction),
            ["Rayman"] = Sync(RM.DoAction),
            ["Resident Evil"] = Sync(RE.DoAction),
            ["Resident Evil 2"] = Sync(RE2R.DoAction),
            ["Resident Evil 3"] = Sync(RE3R.DoAction),
            ["Resident Evil 4 (2005)"] = RE4.DoAction,
            ["Resident Evil 4 Remake"] = Sync(RE4R.DoAction),
            ["Resident Evil 5"] = Sync(RE5.DoAction),
            ["Resident Evil 6"] = Sync(RE6.DoAction),
            ["Resident Evil 7"] = Sync(RE7.DoAction),
            ["Resident Evil 8"] = Sync(RE8.DoAction),
            ["Resident Evil Revelations 2"] = Sync(REV2.DoAction),
            ["Scott Pilgrim vs The World"] = Sync(SPTG.DoAction),
            ["Shin Megami Tensei III"] = Sync(SMT3.DoAction),
            ["Shin Megami Tensei V"] = Sync(SMT5.DoAction),
            ["Sonic Adventure 2"] = Sync(SA2.DoAction),
            ["Sonic Adventure DX"] = Sync(SADX.DoAction),
            ["Sonic Generations"] = Sync(SXSG.DoAction),
            ["Stellar Blade"] = Sync(SB.DoAction),
            ["Team Fortress 2"] = TF2.DoAction,
            ["Temtem: Swarm"] = TTS.DoAction,
            ["The Binding of Isaac: Rebirth"] = Sync(TBOI.DoAction),
            ["The Witcher 3"] = Sync(TWIII.DoAction),
            ["TY the Tasmanian Tiger"] = Sync(TY.DoAction),
            ["Vampire Survivors"] = Sync(VS.DoAction),
            ["Visions of Mana"] = Sync(VOM.DoAction),
            ["Ys I Chronicles"] = Sync(YSI.DoAction),
            ["Zelda: The Wind Waker HD"] = WWHD.DoAction,
            ["Zelda: Twilight Princess HD"] = TPHD.DoAction
        };

    public static bool TryGet(string gameName, out Func<Task>? startAsync) =>
        Integrations.TryGetValue(gameName, out startAsync);

    public static IReadOnlyCollection<string> SupportedGames => Integrations.Keys.ToArray();

    private static Func<Task> Sync(Action action) => () =>
    {
        action();
        return Task.CompletedTask;
    };
}
