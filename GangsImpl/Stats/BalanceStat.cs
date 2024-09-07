using GangsAPI.Data.Stat;

namespace Stats;

public class BalanceStat : BaseStat<int> {
  public override string StatId => "gang_native_balance";
  public override string Name => "Balance";
  public override string Description => "The amount of money the entity has.";

  public override IStat<int> Clone() {
    return new BalanceStat { Value = Value };
  }
}