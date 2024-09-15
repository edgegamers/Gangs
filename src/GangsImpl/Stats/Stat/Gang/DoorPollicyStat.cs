using GangsAPI.Data.Gang;

namespace Stats.Stat.Gang;

public class DoorPollicyStat : BaseStat<DoorPolicy> {
  public override string StatId => "gang_door_policy";
  public override string Name => "Door Policy";
  public override string? Description => "The door policy of the gang.";
  public override DoorPolicy Value { get; set; } = DoorPolicy.REQUEST_ONLY;
}