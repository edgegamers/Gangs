namespace Stats.Stat;

public class BalanceStat : BaseStat<int> {
  public const string STAT_ID = "gang_native_balance";
  public override string StatId => STAT_ID;
  public override string Name => "Balance";
  public override string Description => "The amount of money the entity has.";
  public override int Value { get; set; } = 0;
}