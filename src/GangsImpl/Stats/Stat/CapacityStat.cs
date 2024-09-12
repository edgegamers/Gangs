using GangsAPI.Data.Stat;

namespace Stats.Stat;

public class CapacityStat : BaseStat<int> {
  public override string StatId => "gang_native_capacity";
  public override string Name => "Capacity";
  public override string Description => "The capacity of the gang.";
}