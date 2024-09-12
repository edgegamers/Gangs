using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Utils;
using GangsAPI;
using GangsAPI.Data;
using GangsAPI.Services.Player;
using Microsoft.Extensions.DependencyInjection;
using Stats.Stat;
using Stats.Stat.Player;

namespace StatsTracker;

public class RoundStatsTracker(IServiceProvider provider) : IPluginBehavior {
  private readonly IPlayerManager players =
    provider.GetRequiredService<IPlayerManager>();

  private readonly IPlayerStatManager playerStats =
    provider.GetRequiredService<IPlayerStatManager>();

  private readonly string statId = new RoundStats().StatId;

  [GameEventHandler]
  public HookResult OnMVP(EventRoundMvp ev, GameEventInfo info) {
    var player = ev.Userid;
    if (player == null || !player.IsValid || player.IsBot)
      return HookResult.Continue;
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
    var winningTeam = (CsTeam)ev.Winner;

    var wrapped = Utilities.GetPlayers()
     .Select(p => new PlayerWrapper(p))
     .ToList();
    Task.Run(async () => {
      foreach (var wrapper in wrapped) {
        var gangPlayer = await players.GetPlayer(wrapper.Steam);
        if (gangPlayer == null) continue;
        var (_, stat) =
          await playerStats.GetForPlayer<RoundData>(wrapper, statId);

        stat ??= new RoundData();

        if (wrapper.Team == winningTeam)
          stat.RoundsWon++;
        else
          stat.RoundsLost++;

        await playerStats.SetForPlayer(wrapper, statId, stat);
      }
    });
    return HookResult.Continue;
  }
}