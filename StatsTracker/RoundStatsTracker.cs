using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Utils;
using GangsAPI;
using GangsAPI.Data;
using GangsAPI.Services.Gang;
using GangsAPI.Services.Player;
using Microsoft.Extensions.DependencyInjection;
using Stats.Stat;

namespace StatsTracker;

public class RoundStatsTracker(IServiceProvider provider) : IPluginBehavior {
  private readonly IPlayerStatManager playerStats =
    provider.GetRequiredService<IPlayerStatManager>();

  private readonly IPlayerManager players =
    provider.GetRequiredService<IPlayerManager>();

  private readonly string statId = new RoundStats().StatId;

  [GameEventHandler]
  public HookResult OnMVP(EventRoundMvp ev, GameEventInfo info) {
    var player = ev.Userid;
    if (player == null || !player.IsValid) return HookResult.Continue;
    var wrapper = new PlayerWrapper(player);
    Task.Run(async () => {
      var gangPlayer = await players.GetPlayer(wrapper.Steam);
      if (gangPlayer == null) return;
      var (_, stat) =
        await playerStats.GetForPlayer<RoundData>(wrapper, statId);

      stat ??= new RoundData();

      stat.RoundsMVP++;

      await playerStats.SetForPlayer(wrapper, statId, stat);
    });
    return HookResult.Continue;
  }

  [GameEventHandler]
  public HookResult OnEnd(EventRoundEnd ev, GameEventInfo info) {
    // var player = ev.
    var winningTeam = (CsTeam)ev.Winner;
    var winners = Utilities.GetPlayers()
     .Where(p => p.Team == winningTeam)
     .Select(p => new PlayerWrapper(p))
     .ToList();
    var losers = Utilities.GetPlayers()
     .Where(p => p.Team != winningTeam && p.Team > CsTeam.Spectator)
     .Select(p => new PlayerWrapper(p))
     .ToList();

    var wrapped = Utilities.GetPlayers().Select(p => new PlayerWrapper(p));

    Task.Run(async () => {
      foreach (var wrapper in wrapped) {
        var gangPlayer = await players.GetPlayer(wrapper.Steam);
        if (gangPlayer == null) continue;
        var (_, stat) =
          await playerStats.GetForPlayer<RoundData>(wrapper, statId);

        stat ??= new RoundData();

        if (winners.Any(p => p.Steam == wrapper.Steam)) {
          stat.RoundsWon++;
        } else if (losers.Any(p => p.Steam == wrapper.Steam)) {
          stat.RoundsLost++;
        }

        await playerStats.SetForPlayer(wrapper, statId, stat);
      }
    });
    return HookResult.Continue;
  }
}