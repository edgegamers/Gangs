using GangsAPI;
using GangsAPI.Data.Gang;
using GangsAPI.Perks;
using GangsAPI.Services.Gang;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace Stats.Perk;

public class MotdPerk(IServiceProvider provider)
  : BasePerk<string>(provider), IMotdPerk, IPluginBehavior {
  private readonly IGangStatManager gangStats =
    provider.GetRequiredService<IGangStatManager>();

  public override string Value { get; set; } = "";
  public override string StatId => "gang_native_motd";
  public override string Name => "MOTD";
  public override string Description => "The message of the day for the gang.";

  public override async Task<int?> GetCost(IGangPlayer player) {
    if (player.GangId == null || player.GangRank == null) return null;
    var (success, _) =
      await gangStats.GetForGang<string>(player.GangId.Value, StatId);
    if (!success) return 7500;
    return null;
  }

  public override async Task OnPurchase(IGangPlayer player) {
    if (player.GangId == null || player.GangRank == null) return;
    var (success, desc) =
      await gangStats.GetForGang<string>(player.GangId.Value, StatId);
    if (!success) desc = "Use /gang motd <message> to set the MOTD.";
    await gangStats.SetForGang(player.GangId.Value, StatId, desc);
  }

  public async Task<string?> GetMotd(int gangid) {
    var (success, desc) = await gangStats.GetForGang<string>(gangid, StatId);

    return success ? desc : null;
  }

  public async Task<bool> SetMotd(int gangid, string desc,
    IGangPlayer? player = null) {
    var (success, _) = await gangStats.GetForGang<string>(gangid, StatId);
    if (!success) return false;

    await gangStats.SetForGang(gangid, StatId, desc);

    if (player == null) return true;
    var localizer = Provider.GetService<IStringLocalizer>();
    var gangChat  = Provider.GetService<IGangChatPerk>();
    var gang = await (Provider.GetService<IGangManager>()?.GetGang(gangid)
      ?? Task.FromResult<IGang?>(null));
    if (localizer == null || gangChat == null || gang == null) return true;
    await gangChat.SendGangChat(player, gang,
      localizer.Get(MSG.GANG_THING_SET, "MOTD", desc));

    return true;
  }
}