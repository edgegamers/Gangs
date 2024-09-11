using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Timers;
using GangsAPI;
using GangsAPI.Data;
using GangsAPI.Services.Player;
using Microsoft.Extensions.DependencyInjection;
using Stats.Stat;

namespace StatsTracker;

public class PlaytimeStatsTracker(IServiceProvider provider) : IPluginBehavior {
  private readonly string statId = new PlaytimeStat().StatId;

  private readonly IPlayerManager players =
    provider.GetRequiredService<IPlayerManager>();

  private readonly IPlayerStatManager playerStats =
    provider.GetRequiredService<IPlayerStatManager>();

  public void Start(BasePlugin? plugin, bool hotReload) {
    plugin?.AddTimer(60.0f, increment, TimerFlags.REPEAT);
  }

  private void increment() {
    var playerList = Utilities.GetPlayers()
     .Select(p => new PlayerWrapper(p))
     .ToList();
    foreach (var player in playerList) {
      Task.Run(async () => {
        var gangPlayer = await players.GetPlayer(player.Steam);
        if (gangPlayer == null) return;
        var (_, stat) =
          await playerStats.GetForPlayer<PlaytimeData>(player.Steam, statId);
        stat ??= new PlaytimeData();
        stat.MinutesPlayed++;
        stat.LastPlayed =
          (ulong)DateTime.Now.Subtract(DateTime.UnixEpoch).TotalSeconds;
        await playerStats.SetForPlayer(player.Steam, statId, stat);
      });
    }
  }
}