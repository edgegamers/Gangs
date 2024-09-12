using GangsAPI.Data.Gang;

namespace Stats.Perk;

public class CapacityStat(IServiceProvider provider)
  : BasePerk<int>(provider) {
  public override string StatId => "gang_native_capacity";
  public override string Name => "Capacity";
  public override string Description => "The capacity of the gang.";

  public override int Cost => Value > 15 ? -1 : (Value - 4) * 10000;
  public override int Value { get; set; } = 5;

  public override Task OnPurchase(IGangPlayer player) {
    throw new NotImplementedException();
  }
}