using GangsAPI;
using GangsAPI.Data.Gang;
using GangsAPI.Perks;
using GangsAPI.Services.Gang;
using Microsoft.Extensions.DependencyInjection;

namespace Stats.Perk;

public class CapacityPerk(IServiceProvider provider)
  : BasePerk<int>(provider), ICapacityPerk, IPluginBehavior {
  public override string StatId => "gang_native_capacity";
  public override string Name => "Capacity";
  public override string Description => "The capacity of the gang.";

  public override int Value { get; set; } = 1;

  private readonly IGangStatManager gangStats =
    provider.GetRequiredService<IGangStatManager>();

  public override async Task<int?> GetCost(IGangPlayer player) {
    if (player.GangId == null || player.GangRank == null) return null;
    var (success, capacity) =
      await gangStats.GetForGang<int>(player.GangId.Value, StatId);
    if (!success) capacity = 1;

    if (capacity >= MaxCapacity) return null;

    return getCostFor(capacity + 1);
  }

  // https://www.desmos.com/calculator/ie4owyajay
  private static int getCostFor(int size) {
    var numerator = 100 * size + Math.Pow(4.9 * size, 4);
    return (int)(Math.Ceiling(numerator / 500) * 100);
  }

  public override async Task OnPurchase(IGangPlayer player) {
    if (player.GangId == null || player.GangRank == null) return;
    var (success, capacity) =
      await gangStats.GetForGang<int>(player.GangId.Value, StatId);
    if (!success) capacity = 1;
    capacity++;
    await gangStats.SetForGang(player.GangId.Value, StatId, capacity);
  }

  public async Task<int> GetCapacity(int gangid) {
    var (success, capacity) = await gangStats.GetForGang<int>(gangid, StatId);
    if (!success) capacity = 1;
    return capacity;
  }

  public int MaxCapacity => 15;
}