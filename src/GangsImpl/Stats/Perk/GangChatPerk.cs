using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using GangsAPI;
using GangsAPI.Data;
using GangsAPI.Data.Gang;
using GangsAPI.Perks;
using GangsAPI.Permissions;
using GangsAPI.Services;
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

  private readonly IRankManager ranks =
    provider.GetRequiredService<IRankManager>();

  private readonly Dictionary<int, List<string>> history = new();

  public override bool Value { get; set; }

  public override string StatId => "gang_native_chat";
  public override string Name => "Gang Chat";

  public override string Description
    => "Chat with your gang members with .[message]";

  public async Task SendGangChat(string name, IGang gang, string message) {
    if (!history.TryGetValue(gang.GangId, out var value)) {
      value                = [];
      history[gang.GangId] = value;
    }

    value.Add($"{name}: {message}");

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

  public void ClearChatHistory(IGang gang) { history.Remove(gang.GangId); }

  public IEnumerable<string> GetChatHistory(IGang gang) {
    return history.TryGetValue(gang.GangId, out var value) ? value : [];
  }

  public override async Task<int?> GetCost(IGangPlayer player) {
    if (player.GangId == null || player.GangRank == null) return null;
    var perk = await gangStats.GetForGang<bool>(player.GangId.Value, StatId);
    return perk ? null : 10000;
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

      var perk =
        await gangStats.GetForGang<bool>(gangPlayer.GangId.Value, StatId);

      if (!perk) {
        wrapper.PrintToChat(localizer.Get(MSG.PERK_MISSING, Name));
        return;
      }

      var (allowed, required) =
        await ranks.CheckRank(gangPlayer, Perm.SEND_GANG_CHAT);
      if (!allowed) {
        wrapper.PrintToChat(localizer.Get(MSG.GENERIC_NOPERM_RANK,
          required.Name));
        return;
      }

      var gang = await gangs.GetGang(gangPlayer.GangId.Value);
      if (gang == null) return;
      await ((IGangChatPerk)this).SendGangChat(gangPlayer, gang, message);
    });
    return HookResult.Handled;
  }
}