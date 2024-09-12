using GangsAPI.Data.Stat;

namespace Stats.Stat;

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
}