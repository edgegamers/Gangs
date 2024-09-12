using CounterStrikeSharp.API.Modules.Utils;

namespace Stats.Stat.Player;

public class KDRStat : BaseStat<KDRData> {
  public override string StatId => "gang_native_kdr";
  public override string Name => "KDR";
  public override string? Description => "Kill-Death Ratio";

  public override string ToString() {
    if (Value == null) return $"N/A";
    var deaths = Value.TDeaths + Value.CTDeaths;
    if (deaths == 0) return $"{ChatColors.LightRed}{Value.TKills}";
    var line =
      $"CT KDR: {Value.CTKills}/{Value.CTDeaths} ({Value.CTKills / Value.CTDeaths:F2})\n";
    line +=
      $"T KDR: {Value.TKills}/{Value.TDeaths} ({Value.TKills / Value.TDeaths:F2})";
    return line;
  }
}

public class KDRData {
  public int TKills { get; set; }
  public int TDeaths { get; set; }
  public int CTKills { get; set; }
  public int CTDeaths { get; set; }
}