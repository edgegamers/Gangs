namespace Stats.Stat.Player;

public class RoundStats : BaseStat<RoundData> {
  public override string StatId => "gang_native_rounds";
  public override string Name => "Round Stats";
  public override string Description => "Rounds won, lost and MVPs";
}

public class RoundData {
  public int RoundsWon { get; set; }
  public int RoundsLost { get; set; }
  public int RoundsMVP { get; set; }

  public override string ToString() {
    return "(Wins/Losses/MVPs): " + $"{RoundsWon}/{RoundsLost}/{RoundsMVP}";
  }
}