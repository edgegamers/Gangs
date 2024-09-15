using GangsAPI;
using GangsAPI.Data.Gang;
using GangsAPI.Perks;
using GangsAPI.Services.Gang;
using GangsAPI.Services.Menu;
using Microsoft.Extensions.DependencyInjection;

namespace Stats.Perk;

public class MotdPerk(IServiceProvider provider)
  : BasePerk<string>(provider), IMotdPerk, IPluginBehavior {
  public override string StatId => "gang_native_motd";
  public override string Name => "MOTD";
  public override string? Description => "The message of the day for the gang.";

  private readonly IGangStatManager gangStats =
    provider.GetRequiredService<IGangStatManager>();

  public override async Task<int?> GetCost(IGangPlayer player) {
    if (player.GangId == null || player.GangRank == null) return null;
    var (success, _) =
      await gangStats.GetForGang<string>(player.GangId.Value, StatId);
    if (!success) return 1000;
    return null;
  }

  public override async Task OnPurchase(IGangPlayer player) {
    if (player.GangId == null || player.GangRank == null) return;
    var (success, desc) =
      await gangStats.GetForGang<string>(player.GangId.Value, StatId);
    if (!success) desc = "";
    await gangStats.SetForGang(player.GangId.Value, StatId, desc);
  }

  public override string Value { get; set; } = "";

  public async Task<string?> GetMotd(int gangid) {
    var (success, desc) = await gangStats.GetForGang<string>(gangid, StatId);

    return success ? desc : null;
  }

  public override Task<IMenu?> GetMenu(IGangPlayer player) {
    return base.GetMenu(player);
  }
}