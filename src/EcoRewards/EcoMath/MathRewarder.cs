﻿using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Timers;
using CounterStrikeSharp.API.Modules.Utils;
using GangsAPI;
using GangsAPI.Data;
using GangsAPI.Services;
using GangsAPI.Services.Commands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Timer = CounterStrikeSharp.API.Modules.Timers.Timer;

namespace EcoRewards.EcoMath;

public class MathRewarder(IServiceProvider provider)
  : IPluginBehavior, IMathService {
  private readonly ICommandManager commands =
    provider.GetRequiredService<ICommandManager>();

  private readonly IEcoManager eco = provider.GetRequiredService<IEcoManager>();

  private readonly IStringLocalizer locale =
    provider.GetRequiredService<IStringLocalizer>();

  private readonly Random rng = new();

  private Timer? mathTimer;

  private BasePlugin plugin = null!;

  public IMathService.MathParams? Question { get; private set; }

  public void StartMath(IMathService.MathParams? question = null) {
    Question = question ?? generateEquation();
    var players = Utilities.GetPlayers()
     .Where(p => p is { IsBot: false, Team: > CsTeam.Spectator })
     .Select(p => new PlayerWrapper(p))
     .ToList();

    players = players.Where(p => p.Player != null).ToList();

    foreach (var player in players) {
      player.PrintToChat(locale.Get(MSG.MATH_QUERY, Question.Value.Reward));
      player.PrintToChat(locale.Get(MSG.MATH_QUERYLINE,
        convertToFancy(Question.Value.Equation)));
    }

    mathTimer?.Kill();
    mathTimer = plugin.AddTimer(30, () => { StopMath(null); },
      TimerFlags.STOP_ON_MAPCHANGE);
  }

  public void StopMath(CCSPlayerController? winner) {
    if (Question == null) return;
    var players = Utilities.GetPlayers()
     .Where(p => p is { IsBot: false, Team: > CsTeam.Spectator })
     .Select(p => new PlayerWrapper(p))
     .ToList();

    var ans    = Question?.Answer ?? 0;
    var ansStr = ans % 1 == 0 ? ans.ToString("0") : ans.ToString("0.##");

    if (winner == null) {
      foreach (var player in players)
        player.PrintToChat(locale.Get(MSG.MATH_TIMEOUT, ansStr));
    } else {
      foreach (var player in players)
        player.PrintToChat(locale.Get(MSG.MATH_ANSWERED, winner.PlayerName,
          ansStr, Question?.Reward ?? 0));

      var wrapper = new PlayerWrapper(winner);
      var reward  = Question?.Reward ?? 0;
      Task.Run(async () => await eco.Grant(wrapper, reward, true, "Math"));
    }

    Question = null;
    mathTimer?.Kill();
    mathTimer = null;
  }

  public void Start(BasePlugin? parent, bool hotReload) {
    if (parent != null) plugin = parent;
    plugin?.AddTimer(60 * 10, () => {
      var players = Utilities.GetPlayers()
       .Where(p => p is { IsBot: false, Team: > CsTeam.None })
       .Select(p => new PlayerWrapper(p))
       .ToList();

      if (players.Count < RewardsCollection.MIN_PLAYERS) return;
      try { StartMath(); } catch (InvalidOperationException) { }
    }, TimerFlags.REPEAT);

    commands.RegisterCommand(new StartMathCommand(provider));

    plugin?.AddCommandListener("say", OnSay, HookMode.Post);
  }

  private HookResult OnSay(CCSPlayerController? player, CommandInfo info) {
    if (player == null || !player.IsValid) return HookResult.Continue;
    if (Question == null) return HookResult.Continue;

    var guess = info.ArgString.Trim('"');
    if (!double.TryParse(guess, out var answer)) return HookResult.Continue;

    if (Math.Abs(answer - Question.Value.Answer) > 0.01)
      return HookResult.Continue;

    StopMath(player);
    return HookResult.Continue;
  }

  private IMathService.MathParams generateEquation() {
    while (true) {
      EquationBuilder eq  = new(rng.Next(1, 20));
      var             min = rng.Next(1, 3);
      for (var i = 0; i < min; i++) {
        if (i > 1 && rng.NextDouble() < 0.8) break;
        eq = rng.Next(6) switch {
          0 => eq.WithAddition(rng.Next(-10, 10), rng.Next(3) == 0),
          1 => eq.WithSubtraction(rng.Next(-10, 10), rng.Next(3) == 0),
          2 => eq.WithMultiplication(rng.Next(-5, 5), rng.Next(3) == 0),
          3 => eq.WithDivision(rng.Next(-3, 10), rng.Next(3) == 0),
          4 => eq.WithModulus(rng.Next(2, 5), rng.Next(3) == 0),
          5 => eq.WithExponent(rng.Next(0, 3), rng.Next(3) == 0),
          _ => eq.WithAddition(rng.Next(1, 20), rng.Next(3) == 0)
        };
      }

      var result = new IMathService.MathParams {
        Equation = eq.Equation,
        Reward =
          (int)Math.Ceiling(Math.Pow(eq.Difficulty() * 5, 2)
            * Math.Sqrt(Utilities.GetPlayers().Count * 4)),
        Answer = eq.Evaluate()
      };

      if (!double.IsFinite(result.Answer)) continue;
      if (!double.IsInteger(result.Answer)) continue;

      return result;
    }
  }

  private string convertToFancy(string eq) {
    return eq.Replace('%', '％')
     .Replace("**", "^")
     .Replace('*', '×')
     .Replace(" ", "   ");
  }
}