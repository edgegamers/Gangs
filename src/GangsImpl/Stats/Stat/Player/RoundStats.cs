namespace Stats.Stat.Player;

public class RoundStats : BaseStat<RoundData> {
  public override string StatId => "gang_native_rounds";
  public override string Name => "Round Stats";
  public override string Description => "Rounds won, lost and MVPs";

  public override string ToString() {
    if (Value == null) return $"N/A";
    return "(Wins/Losses/MVPs): "
      + $"{Value.RoundsWon}/{Value.RoundsLost}/{Value.RoundsMVP}";
  }
}

public class RoundData {
  public int RoundsWon { get; set; }
  public int RoundsLost { get; set; }
  public int RoundsMVP { get; set; }
}