using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Utils;
using GangsAPI;
using GangsAPI.Data;
using GangsAPI.Permissions;
using GangsAPI.Services;
using Microsoft.Extensions.DependencyInjection;

namespace EcoRewards;

public class RoundWinListener(IServiceProvider provider) : IPluginBehavior {
  private readonly IEcoManager eco = provider.GetRequiredService<IEcoManager>();

  [GameEventHandler]
  public HookResult OnWin(EventRoundEnd ev, GameEventInfo info) {
    var winners = Utilities.GetPlayers()
     .Where(p => !p.IsBot && p.Team == (CsTeam)ev.Winner)
     .Select(p => new PlayerWrapper(p))
     .ToList();

    if (winners.Count == 0) return HookResult.Continue;
    var toDistribute = 100;
    var each         = (int)Math.Ceiling(toDistribute / (double)winners.Count);
    foreach (var winner in winners)
      Task.Run(async () => await eco.Grant(winner, each, reason: "Round Win"));

    return HookResult.Continue;
  }

  [GameEventHandler]
  public HookResult OnMVP(EventRoundMvp ev, GameEventInfo info) {
    var player = ev.Userid;
    if (player == null || !player.IsValid) return HookResult.Continue;

    var mvp = new PlayerWrapper(player);
    Task.Run(async () => await eco.Grant(mvp, 20, reason: "MVP"));

    return HookResult.Continue;
  }
}