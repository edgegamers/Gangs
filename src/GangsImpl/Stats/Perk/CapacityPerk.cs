using GangsAPI;
using GangsAPI.Data.Gang;
using GangsAPI.Perks;
using GangsAPI.Services.Gang;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace Stats.Perk;

public class CapacityPerk(IServiceProvider provider)
  : BasePerk<int>(provider), ICapacityPerk, IPluginBehavior {
  private readonly IGangStatManager gangStats =
    provider.GetRequiredService<IGangStatManager>();

  public override int Value { get; set; } = 1;
  public override string StatId => "gang_native_capacity";
  public override string Name => "Capacity";
  public override string Description => "The capacity of the gang.";

  public override async Task<int?> GetCost(IGangPlayer player) {
    if (player.GangId == null || player.GangRank == null) return null;
    var capacity = await gangStats.GetForGang<int>(player.GangId.Value, StatId);
    if (capacity == 0) capacity = 1;

    if (capacity >= MaxCapacity) return null;

    return getCostFor(capacity + 1);
  }

  public override async Task OnPurchase(IGangPlayer player) {
    if (player.GangId == null || player.GangRank == null) return;
    var capacity = await gangStats.GetForGang<int>(player.GangId.Value, StatId);
    if (capacity == 0) capacity = 1;
    capacity++;
    await gangStats.SetForGang(player.GangId.Value, StatId, capacity);

    var localizer = Provider.GetService<IStringLocalizer>();
    var gangChat  = Provider.GetService<IGangChatPerk>();
    var gang =
      await (Provider.GetService<IGangManager>()?.GetGang(player.GangId.Value)
        ?? Task.FromResult<IGang?>(null));
    if (localizer == null || gangChat == null || gang == null) return;
    var str = localizer.Get(MSG.PERK_PURCHASED,
      player.Name ?? player.Steam.ToString(), $"{Name} ({capacity})");
    await gangChat.SendGangChat(gang, str);
  }

  public async Task<int> GetCapacity(int gangid) {
    var capacity = await gangStats.GetForGang<int>(gangid, StatId);
    if (capacity == 0) capacity = 1;
    return capacity;
  }

  public int MaxCapacity => 15;

  // https://www.desmos.com/calculator/ie4owyajay
  private static int getCostFor(int size) {
    var numerator = 100 * size + 4.9 * Math.Pow(size, 4);
    return (int)(Math.Ceiling(numerator / 500) * 100);
  }
}