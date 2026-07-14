using MultiPresence.Presence;
using System.Reflection;
using System.Timers;

namespace MultiPresence.Core;

public sealed class GamePresenceManager : IDisposable
{
    private static readonly IReadOnlyDictionary<string, string> PresenceTypes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        ["Borderlands 1"] = nameof(BL1), ["Borderlands 2"] = nameof(BL2), ["Call of Duty®"] = nameof(COD),
        ["CODE VEIN"] = nameof(CV), ["Crash Bandicoot 4: It's About Time"] = nameof(CB4), ["Crash Bandicoot N. Sane Trilogy"] = nameof(CBNT),
        ["CRISIS CORE –FINAL FANTASY VII– REUNION"] = nameof(CCFFVII), ["Dark Souls II"] = nameof(DSII), ["Dark Souls III"] = nameof(DSIII),
        ["Dark Souls: Remastered"] = nameof(DSR), ["Death Stranding"] = nameof(DSDC), ["Devil May Cry"] = nameof(DMC1),
        ["Devil May Cry 2"] = nameof(DMC2), ["Devil May Cry 3"] = nameof(DMC3), ["Devil May Cry 4"] = nameof(DMC4),
        ["Devil May Cry 5"] = nameof(DMC5), ["Diablo IV"] = nameof(D4), ["Digimon Story Time Stranger"] = nameof(DSTS),
        ["DmC Devil May Cry"] = nameof(DMC), ["Elden Ring"] = nameof(ER), ["Final Fantasy VII Rebirth"] = nameof(FFVIIRB),
        ["Final Fantasy VII Remake"] = nameof(FFVIIR), ["Final Fantasy X"] = nameof(FFX), ["Final Fantasy XV"] = nameof(FFXV),
        ["Final Fantasy XVI"] = nameof(FFXVI), ["Granblue Fantasy: Relink"] = nameof(GBFR), ["Gunfire Reborn"] = nameof(GFR),
        ["Hello Kitty Island Adventure"] = nameof(HK), ["Hogwarts Legacy"] = nameof(HL), ["Kingdom Hearts Birth by Sleep Final Mix"] = nameof(KHBBS),
        ["Kingdom Hearts Dream Drop Distance"] = nameof(KHDDD), ["Kingdom Hearts Final Mix"] = nameof(KH1), ["Kingdom Hearts II Final Mix"] = nameof(KH2),
        ["Kingdom Hearts III"] = nameof(KH3), ["Kingdom Hearts Re:Chain of Memories"] = nameof(KHCOM), ["Labyrinthine"] = nameof(LR),
        ["Lies of P"] = nameof(LOP), ["Mega Man 11"] = nameof(MM11), ["Mega Man Battle Network"] = nameof(MMBN1),
        ["Mega Man Battle Network 2"] = nameof(MMBN2), ["Mega Man Battle Network 3"] = nameof(MMBN3), ["Mega Man Battle Network 5"] = nameof(MMBN5),
        ["Mega Man Battle Network 6"] = nameof(MMBN6), ["Mega Man X5"] = nameof(MMX5), ["Mega Man X6"] = nameof(MMX6),
        ["Mega Man X7"] = nameof(MMX7), ["Mega Man X8"] = nameof(MMX8), ["Marvel's Spider-Man Remastered"] = nameof(MSMR),
        ["Marvel's Spider-Man: Miles Morales"] = nameof(MSMMMM), ["Overwatch"] = nameof(OW), ["Pangya Reborn"] = nameof(PYRE),
        ["Persona 4 Golden"] = nameof(P4G), ["Persona 5 Strikers"] = nameof(P5S), ["Persona 5 Royal"] = nameof(P5R),
        ["Persona 5 Tactica"] = nameof(P5T), ["Persona 5: The Phantom X"] = nameof(P5X), ["Project Diva Mega Mix+"] = nameof(PDMM),
        ["Rayman"] = nameof(RM), ["Resident Evil"] = nameof(RE), ["Resident Evil 2"] = nameof(RE2R), ["Resident Evil 3"] = nameof(RE3R),
        ["Resident Evil 4 (2005)"] = nameof(RE4), ["Resident Evil 4 Remake"] = nameof(RE4R), ["Resident Evil 5"] = nameof(RE5),
        ["Resident Evil 6"] = nameof(RE6), ["Resident Evil 7"] = nameof(RE7), ["Resident Evil 8"] = nameof(RE8),
        ["Resident Evil Revelations 2"] = nameof(REV2), ["Scott Pilgrim vs The World"] = nameof(SPTG), ["Shin Megami Tensei III"] = nameof(SMT3),
        ["Shin Megami Tensei V"] = nameof(SMT5), ["Sonic Adventure 2"] = nameof(SA2), ["Sonic Adventure DX"] = nameof(SADX),
        ["Sonic Generations"] = nameof(SXSG), ["Stellar Blade"] = nameof(SB), ["Team Fortress 2"] = nameof(TF2),
        ["Temtem: Swarm"] = nameof(TTS), ["The Witcher 3"] = nameof(TWIII), ["TY the Tasmanian Tiger"] = nameof(TY),
        ["Visions of Mana"] = nameof(VOM), ["Ys I Chronicles"] = nameof(YSI), ["Zelda: The Wind Waker HD"] = nameof(WWHD),
        ["Zelda: Twilight Princess HD"] = nameof(TPHD)
    };

    private readonly System.Timers.Timer _timer;
    private readonly BlacklistService _blacklist;
    private readonly SemaphoreSlim _tickGate = new(1, 1);
    private readonly Action<string, bool> _onGameChanged;
    private readonly Action<string> _notify;
    private CancellationTokenSource _cts = new();

    public GamePresenceManager(System.Timers.Timer timer, BlacklistService blacklist, Action<string, bool> onGameChanged, Action<string> notify)
    {
        _timer = timer;
        _blacklist = blacklist;
        _onGameChanged = onGameChanged;
        _notify = notify;
    }

    public void Start()
    {
        _timer.Elapsed += OnElapsed;
        _timer.Interval = 5000;
        _timer.AutoReset = true;
        _timer.Start();
    }

    private async void OnElapsed(object? sender, ElapsedEventArgs e)
    {
        if (!await _tickGate.WaitAsync(0).ConfigureAwait(false))
            return;

        try
        {
            var game = GameDetector.GetGame();
            if (string.IsNullOrWhiteSpace(game))
            {
                _onGameChanged(string.Empty, false);
                return;
            }

            var blocked = await _blacklist.ContainsAsync(game, _cts.Token).ConfigureAwait(false);
            _onGameChanged(game, blocked);
            if (blocked)
                return;

            if (!PresenceTypes.TryGetValue(game, out var typeName))
                return;

            _timer.Stop();
            PlaceholderHelper._startTimestamp = DiscordRPC.Timestamps.Now;

            var steamAppId = Path.Combine("Assets", "steam_appid.txt");
            if (File.Exists(steamAppId))
                File.Delete(steamAppId);

            _notify(game);
            await InvokePresenceAsync(typeName).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception ex)
        {
            RateLimitedLogger.Error("presence-manager", ex);
            _timer.Start();
        }
        finally
        {
            _tickGate.Release();
        }
    }

    private static async Task InvokePresenceAsync(string typeName)
    {
        var type = typeof(BL1).Assembly.GetType($"MultiPresence.Presence.{typeName}", throwOnError: true)!;
        var method = type.GetMethod("DoAction", BindingFlags.Public | BindingFlags.Static)
            ?? throw new MissingMethodException(type.FullName, "DoAction");

        var result = method.Invoke(null, null);
        if (result is Task task)
            await task.ConfigureAwait(false);
    }

    public void Dispose()
    {
        _timer.Stop();
        _timer.Elapsed -= OnElapsed;
        _cts.Cancel();
        _cts.Dispose();
        _tickGate.Dispose();
    }
}
