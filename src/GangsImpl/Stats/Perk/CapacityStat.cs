using GangsAPI.Data.Gang;
using GangsAPI.Services.Gang;
using Microsoft.Extensions.DependencyInjection;

namespace Stats.Perk;

public class CapacityStat(IServiceProvider provider) : BasePerk<int>(provider) {
  public override string StatId => "gang_native_capacity";
  public override string Name => "Capacity";
  public override string Description => "The capacity of the gang.";

  public override int Value { get; set; } = 1;

  public override async Task<int?> GetCost(IGangPlayer player) {
    if (player.GangId == null || player.GangRank == null) return null;
    var instance = Provider.GetRequiredService<IGangStatManager>();
    var (success, capacity) =
      await instance.GetForGang<int>(player.GangId.Value, StatId);
    if (!success) capacity = 1;

    if (capacity >= 15) return null;

    return getCostFor(capacity + 1);
  }

  // https://www.desmos.com/calculator/ie4owyajay
  private static int getCostFor(int size) {
    var numerator = 100 * size + Math.Pow(4.9 * size, 4);
    return (int)(Math.Ceiling(numerator / 500) * 100);
  }

  public override async Task OnPurchase(IGangPlayer player) {
    if (player.GangId == null || player.GangRank == null) return;
    var instance = Provider.GetRequiredService<IGangStatManager>();
    var (success, capacity) =
      await instance.GetForGang<int>(player.GangId.Value, StatId);
    if (!success) capacity = 1;
    capacity++;
    await instance.SetForGang(player.GangId.Value, StatId, capacity);
  }
}