using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Cvars;
using GangsAPI;
using GangsAPI.Data;
using GangsAPI.Services;
using GangsAPI.Services.Commands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Timer = CounterStrikeSharp.API.Modules.Timers.Timer;

namespace Raffle;

public class RaffleManager(IServiceProvider provider)
  : IPluginBehavior, IRaffleManager {
  public static FakeConVar<float> CV_RAFFLE_CHANCE =
    new("cs2_gangs_raffle_chance", "The chance of a raffle starting per round",
      0.1f);

  public static FakeConVar<int> CV_RAFFLE_COOLDOWN =
    new("cs2_gangs_raffle_cooldown", "Minimum number of rounds between raffles",
      2);

  public static FakeConVar<int> CV_RAFFLE_MINIMUM =
    new("cs2_gangs_raffle_min", "Minimum amount per player", 2);

  public static FakeConVar<int> CV_RAFFLE_MAXIMUM =
    new("cs2_gangs_raffle_max", "Maximum amount per player", 2);

  public static FakeConVar<float> CV_RAFFLE_DURATION =
    new("cs2_gangs_raffle_duration", "Time to give playeres to enter raffle",
      30);

  private static readonly Random rng = new();
  private int cooldownRounds;

  private Timer? entryTimer;
  private BasePlugin? plugin;

  private readonly IStringLocalizer locale =
    provider.GetRequiredService<IStringLocalizer>();

  private readonly IEcoManager eco = provider.GetRequiredService<IEcoManager>();

  public void Start(BasePlugin? plugin, bool hotReload) {
    if (plugin == null) return;
    this.plugin = plugin;
    var cmd = provider.GetRequiredService<ICommandManager>();
    cmd.RegisterCommand(new RaffleCommand(provider));
    cmd.RegisterCommand(new StartRaffleCommand(provider));
  }

  public Raffle? Raffle { get; private set; }

  public bool StartRaffle(int buyIn) {
    if (Raffle != null || plugin == null) return false;
    Raffle = new Raffle(buyIn);

    SetEntriesOpen(CV_RAFFLE_DURATION.Value);
    return true;
  }

  public bool AreEntriesOpen() { return entryTimer != null; }

  public void SetEntriesOpen(float seconds) {
    entryTimer?.Kill();
    if (plugin == null || Raffle == null) return;
    Server.PrintToChatAll(locale.Get(MSG.RAFFLE_BEGIN, Raffle?.BuyIn ?? 0));

    entryTimer = plugin.AddTimer(seconds, () => {
      entryTimer = null;
      if (Raffle == null) return;

      Server.PrintToChatAll(locale.Get(MSG.RAFFLE_PRE_ANNOUNCE, Raffle.Value,
        Raffle.TotalPlayers));

      plugin.AddTimer(5, DrawWinner);
    });
  }

  public void DrawWinner() {
    if (Raffle == null || plugin == null) return;
    var    players = Raffle.TotalPlayers;
    var    total   = Raffle.Value;
    ulong? winner;
    do { winner = Raffle.GetWinner(); } while (winner != null
      && Raffle.TotalPlayers > 0);

    if (winner == null) {
      Server.PrintToChatAll(locale.Get(MSG.GENERIC_ERROR_INFO,
        "Could not find a winner"));
      Raffle = null;
      return;
    }

    var name = Utilities.GetPlayerFromSteamId(winner.Value)?.PlayerName
      ?? winner.ToString() ?? "";

    var wrapper = new PlayerWrapper(winner.Value, name);

    Task.Run(async () => await eco.Grant(wrapper, total, true, "Raffle"));

    Server.PrintToChatAll(locale.Get(MSG.RAFFLE_WINNER, name,
      (1.0f / players).ToString("P1")));
    Raffle = null;
  }

  [GameEventHandler]
  public HookResult OnRoundStart(EventRoundStart ev, GameEventInfo info) {
    if (RoundUtil.IsWarmup()) return HookResult.Continue;
    if (cooldownRounds > 0) {
      cooldownRounds--;
      return HookResult.Continue;
    }

    if (rng.NextDouble() > CV_RAFFLE_CHANCE.Value) return HookResult.Continue;
    var amo = rng.Next(CV_RAFFLE_MINIMUM.Value, CV_RAFFLE_MAXIMUM.Value);
    StartRaffle(amo);
    cooldownRounds = CV_RAFFLE_COOLDOWN.Value;
    return HookResult.Continue;
  }
}