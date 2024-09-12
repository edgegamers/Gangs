using CounterStrikeSharp.API.Core;
using GangsAPI.Data.Stat;
using GangsAPI.Services;
using Stats.Stat.Player;

namespace StatsTracker;

public class StatManager(IServiceProvider provider) : IStatManager {
  public void Start(BasePlugin? plugin, bool hotReload) {
    // var stats = provider.GetRequiredService<IStatManager>();
    // stats.RegisterStat<PlaytimeData>(new PlaytimeStat());
    // stats.RegisterStat(new KDRStat());
    // stats.RegisterStat(new RoundStats());
    Stats = new List<IStat> {
      new PlaytimeStat(), new KDRStat(), new RoundStats()
    };
  }

  public IEnumerable<IStat> Stats { get; private set; } = [];
}