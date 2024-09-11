using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Utils;
using GangsAPI;
using GangsAPI.Data;
using GangsAPI.Services.Player;
using Microsoft.Extensions.DependencyInjection;
using Stats.Stat;

namespace StatsTracker;

public class KDRStatsTracker(IServiceProvider provider) : IPluginBehavior {
  private readonly IPlayerManager players =
    provider.GetRequiredService<IPlayerManager>();

  private readonly IPlayerStatManager playerStats =
    provider.GetRequiredService<IPlayerStatManager>();

  private readonly string statId = new KDRStat().StatId;

  [GameEventHandler]
  public HookResult OnDeath(EventPlayerDeath ev, GameEventInfo info) {
    var player = ev.Userid;
    if (player == null || !player.IsValid || player.IsBot)
      return HookResult.Continue;
    var wrapper = new PlayerWrapper(player);

    var killer = ev.Attacker;
    var killerWrapper =
      killer != null && killer is { IsValid: true, IsBot: false } ?
        new PlayerWrapper(killer) :
        null;

    Task.Run(async () => {
      var gangPlayer = await players.GetPlayer(wrapper.Steam);
      if (gangPlayer == null) return;
      var (_, stat) = await playerStats.GetForPlayer<KDRData>(wrapper, statId);

      stat ??= new KDRData();

      if (wrapper.Team == CsTeam.Terrorist)
        stat.TDeaths++;
      else
        stat.CTDeaths++;

      await playerStats.SetForPlayer(wrapper.Steam, statId, stat);

      if (killerWrapper == null) return;

      gangPlayer = await players.GetPlayer(killerWrapper.Steam);
      if (gangPlayer == null) return;

      if (killerWrapper.Team == CsTeam.Terrorist)
        stat.TKills++;
      else
        stat.CTKills++;

      await playerStats.SetForPlayer(wrapper.Steam, statId, stat);
    });

    return HookResult.Continue;
  }
}