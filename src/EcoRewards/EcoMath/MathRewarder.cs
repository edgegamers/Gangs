using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Timers;
using CounterStrikeSharp.API.Modules.Utils;
using GangsAPI;
using GangsAPI.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace EcoRewards.EcoMath;

public class MathRewarder(IServiceProvider provider)
  : IPluginBehavior, IMathService {
  private readonly Random rng = new();

  private readonly IStringLocalizer locale =
    provider.GetRequiredService<IStringLocalizer>();

  public IMathService.MathParams? Question { get; private set; }

  public void Start(BasePlugin? plugin, bool hotReload) {
    plugin?.AddTimer(60 * 15, () => {
      var players = Utilities.GetPlayers()
       .Where(p => p is { IsBot: false, Team: > CsTeam.Spectator })
       .Select(p => new PlayerWrapper(p))
       .ToList();

      if (players.Count < RewardsCollection.MIN_PLAYERS) return;
      StartMath(generateEquation());

      plugin.AddTimer(30, () => { StopMath(null); },
        TimerFlags.STOP_ON_MAPCHANGE);
    }, TimerFlags.REPEAT);
  }

  [GameEventHandler]
  public HookResult OnSay(EventPlayerChat ev, GameEventInfo info) {
    Server.PrintToChatAll($"OnSay {ev.Text}");

    return HookResult.Continue;
  }

  private IMathService.MathParams generateEquation() {
    EquationBuilder eq  = new(rng.Next(1, 20));
    var             min = rng.Next(1, 10);
    for (var i = 0; i < min; i++) {
      if (i > 1 && rng.NextDouble() < 0.2) break;
      eq = rng.Next(6) switch {
        0 => eq.WithAddition(rng.Next(1, 20)),
        1 => eq.WithSubtraction(rng.Next(1, 20)),
        2 => eq.WithMultiplication(rng.Next(1, 10)),
        3 => eq.WithDivision(rng.Next(1, 10)),
        4 => eq.WithModulus(rng.Next(1, 10)),
        5 => eq.WithExponent(rng.Next(1, 5)),
        _ => eq.WithAddition(rng.Next(1, 20))
      };
    }

    var result = new IMathService.MathParams {
      Equation = eq.Equation,
      Reward =
        (int)Math.Ceiling(eq.Difficulty() * 10
          * Math.Sqrt(Utilities.GetPlayers().Count * 4)),
      Answer = eq.Evaluate()
    };

    return result;
  }

  public void StartMath(IMathService.MathParams? question = null) {
    question ??= generateEquation();
    var players = Utilities.GetPlayers()
     .Where(p => p is { IsBot: false, Team: > CsTeam.Spectator })
     .Select(p => new PlayerWrapper(p))
     .ToList();

    players = players.Where(p => p.Player != null).ToList();

    foreach (var player in players) {
      player.PrintToChat(locale.Get(MSG.MATH_QUERY, question.Value.Reward));
      player.PrintToChat(
        locale.Get(MSG.MATH_QUERYLINE, question.Value.Equation));
    }
  }

  public void StopMath(CCSPlayerController? winner) {
    var players = Utilities.GetPlayers()
     .Where(p => p is { IsBot: false, Team: > CsTeam.Spectator })
     .Select(p => new PlayerWrapper(p))
     .ToList();

    if (winner == null)
      foreach (var player in players)
        player.PrintToChat(locale.Get(MSG.MATH_TIMEOUT, Question?.Answer ?? 0));
    else
      foreach (var player in players)
        player.PrintToChat(locale.Get(MSG.MATH_ANSWERED, winner.PlayerName,
          Question?.Reward ?? 0));
    Question = null;
  }
}