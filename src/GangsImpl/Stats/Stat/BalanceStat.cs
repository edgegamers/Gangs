namespace Stats.Stat;

public class BalanceStat : BaseStat<int> {
  public override string StatId => "gang_native_balance";
  public override string Name => "Balance";
  public override string Description => "The amount of money the entity has.";
  public override int Value { get; set; } = 0;
}