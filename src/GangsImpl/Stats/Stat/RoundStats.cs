using GangsAPI.Data.Stat;

namespace Stats.Stat;

public class RoundStats : BaseStat<RoundData> {
  public override string StatId => "gang_native_rounds";
  public override string Name => "Round Stats";
  public override string? Description => "Rounds won, lost and MVPs";

  public override IStat<RoundData?> Clone() {
    return new RoundStats() { Value = Value };
  }
}

public class RoundData {
  public int RoundsWon, RoundsLost, RoundsMVP;
}