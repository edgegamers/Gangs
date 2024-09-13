using CounterStrikeSharp.API.Core;
using GangsAPI.Data.Stat;
using GangsAPI.Services;
using Stats.Stat.Player;

namespace StatsTracker;

public class StatManager : IStatManager {
  public void Start(BasePlugin? plugin, bool hotReload) {
    Stats = new List<IStat> {
      new PlaytimeStat(), new KDRStat(), new RoundStats()
    };
  }

  public IEnumerable<IStat> Stats { get; private set; } = [];
}