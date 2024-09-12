namespace Stats.Stat.Player;

public class PlaytimeStat : BaseStat<PlaytimeData> {
  public override string StatId => "gang_native_playtime";
  public override string Name => "Playtime";

  public override string Description
    => "Total playtime and last played timestamp.";

  public override PlaytimeData? Value { get; set; }
}

public class PlaytimeData {
  public int MinutesT { get; set; }
  public int MinutesCT { get; set; }
  public int MinutesSpec { get; set; }
  public ulong LastPlayed { get; set; }

  public override string ToString() {
    return $"T: {MinutesT} CT: {MinutesCT} Spec: {MinutesSpec}";
  }
}