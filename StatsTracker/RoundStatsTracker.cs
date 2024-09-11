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
    if (player == null || !player.IsValid || player.IsBot)
      return HookResult.Continue;
    var wrapper = new PlayerWrapper(player);
    Server.PrintToConsole("BEGIN MVP");
    Task.Run(async () => {
      var gangPlayer = await players.GetPlayer(wrapper.Steam);
      if (gangPlayer == null) return;
      var (_, stat) =
        await playerStats.GetForPlayer<RoundData>(wrapper, statId);

      stat ??= new RoundData();

      stat.RoundsMVP++;

      await playerStats.SetForPlayer(wrapper, statId, stat);
    });
    Server.PrintToConsole("END MVP");
    return HookResult.Continue;
  }

  [GameEventHandler]
  public HookResult OnEnd(EventRoundEnd ev, GameEventInfo info) {
    var winningTeam = (CsTeam)ev.Winner;

    var wrapped = Utilities.GetPlayers().Select(p => new PlayerWrapper(p));
    Server.PrintToConsole("SELECTED ALL PLAYERS");

    Server.PrintToConsole("BEGIN TASK");
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

        await Server.NextFrameAsync(()
          => Server.PrintToConsole(
            $"Player {wrapper.Name} has {stat.RoundsWon} rounds won and {stat.RoundsLost} rounds lost"));

        await playerStats.SetForPlayer(wrapper, statId, stat);

        await Server.NextFrameAsync(()
          => Server.PrintToConsole(
            $"Changed player {wrapper.Name} to have {stat.RoundsWon} rounds won and {stat.RoundsLost} rounds lost"));
      }
    });
    Server.PrintToConsole("END TASK");
    return HookResult.Continue;
  }
}