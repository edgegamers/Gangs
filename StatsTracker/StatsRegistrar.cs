using CounterStrikeSharp.API.Core;
using GangsAPI;
using GangsAPI.Services;
using Microsoft.Extensions.DependencyInjection;
using Stats.Stat;

namespace StatsTracker;

public class StatsRegistrar(IServiceProvider provider) : IPluginBehavior {
  public void Start(BasePlugin? plugin, bool hotReload) {
    var stats = provider.GetRequiredService<IStatManager>();
    stats.RegisterStat(new PlaytimeStat());
    stats.RegisterStat(new KDRStat());
    stats.RegisterStat(new RoundStats());
  }
}