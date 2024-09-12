using CounterStrikeSharp.API.Modules.Utils;

namespace Stats.Stat.Player;

public class KDRStat : BaseStat<KDRData> {
  public override string StatId => "gang_native_kdr";
  public override string Name => "KDR";
  public override string? Description => "Kill-Death Ratio";
}

public class KDRData {
  public int TKills { get; set; }
  public int TDeaths { get; set; }
  public int CTKills { get; set; }
  public int CTDeaths { get; set; }

  public override string ToString() {
    var deaths = TDeaths + CTDeaths;
    if (deaths == 0) return $"{ChatColors.LightRed}{TKills}\n{CTKills}";
    var line = $"CT KDR: {CTKills}/{CTDeaths} ({CTKills / CTDeaths:F2})\n";
    line += $"T KDR: {TKills}/{TDeaths} ({TKills / TDeaths:F2})";
    return line;
  }
}