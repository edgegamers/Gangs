using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using GangsAPI;
using GangsAPI.Data;
using GangsAPI.Data.Gang;
using GangsAPI.Perks;
using GangsAPI.Services.Gang;
using GangsAPI.Services.Player;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace Stats.Perk;

public class GangChatPerk(IServiceProvider provider)
  : BasePerk<bool>(provider), IGangChatPerk, IPluginBehavior {
  private readonly IGangManager gangs =
    provider.GetRequiredService<IGangManager>();

  private readonly IGangStatManager gangStats =
    provider.GetRequiredService<IGangStatManager>();

  private readonly IStringLocalizer localizer =
    provider.GetRequiredService<IStringLocalizer>();

  private readonly IPlayerManager players =
    provider.GetRequiredService<IPlayerManager>();

  public override bool Value { get; set; }

  public override string StatId => "gang_native_chat";
  public override string Name => "Gang Chat";

  public override string Description
    => "Chat with your gang members with .[message]";

  public async Task SendGangChat(string name, IGang gang, string message) {
    var members = await players.GetMembers(gang);
    await Server.NextFrameAsync(() => {
      foreach (var member in members) {
        var target = Utilities.GetPlayerFromSteamId(member.Steam);
        if (target == null || !target.IsValid) continue;
        target.PrintToChat(localizer.Get(MSG.GANG_CHAT_FORMAT, gang.Name, name,
          message));
      }
    });
  }

  public override async Task<int?> GetCost(IGangPlayer player) {
    if (player.GangId == null || player.GangRank == null) return null;
    var (success, perk) =
      await gangStats.GetForGang<bool>(player.GangId.Value, StatId);
    return success && perk ? null : 1000;
  }

  public override async Task OnPurchase(IGangPlayer player) {
    if (player.GangId == null || player.GangRank == null) return;
    Value = true;
    await gangStats.SetForGang(player.GangId.Value, this);
    var gang = await gangs.GetGang(player.GangId.Value);
    if (gang == null) return;
    var str = localizer.Get(MSG.PERK_PURCHASED,
      player.Name ?? player.Steam.ToString(), Name);
    await ((IGangChatPerk)this).SendGangChat(gang, str);
  }

  [GameEventHandler]
  public HookResult OnChat(EventPlayerChat ev, GameEventInfo _) {
    if (!ev.Text.StartsWith('.')) return HookResult.Continue;
    var player = Utilities.GetPlayerFromUserid(ev.Userid);
    if (player == null || !player.IsValid) return HookResult.Continue;

    var message = ev.Text[1..];
    var wrapper = new PlayerWrapper(player);

    Task.Run(async () => {
      var gangPlayer = await players.GetPlayer(wrapper.Steam);
      if (gangPlayer?.GangId == null || gangPlayer.GangRank == null) {
        wrapper.PrintToChat(localizer.Get(MSG.NOT_IN_GANG));
        return;
      }

      var (success, perk) =
        await gangStats.GetForGang<bool>(gangPlayer.GangId.Value, StatId);

      if (!success || !perk) {
        wrapper.PrintToChat(localizer.Get(MSG.PERK_MISSING, Name));
        return;
      }

      var gang = await gangs.GetGang(gangPlayer.GangId.Value);
      if (gang == null) return;
      await ((IGangChatPerk)this).SendGangChat(gangPlayer, gang, message);
    });
    return HookResult.Handled;
  }
}