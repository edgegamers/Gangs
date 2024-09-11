using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using GangsAPI;
using GangsAPI.Data;
using GangsAPI.Data.Gang;
using GangsAPI.Data.Stat;
using GangsAPI.Perks;
using GangsAPI.Services.Gang;
using GangsAPI.Services.Menu;
using GangsAPI.Services.Player;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace Stats.Perk;

public class GangChatPerk : BasePerk, IGangChatPerk {
  private readonly IGangManager? gangs;
  private readonly IGangStatManager? gangStats;
  private readonly IStringLocalizer? localizer;
  private readonly IPlayerManager? players;

  public GangChatPerk(bool Value) : base(null) { this.Value = Value; }

  public GangChatPerk(IServiceProvider provider) : base(provider) {
    gangs     = provider.GetService<IGangManager>();
    gangStats = provider.GetService<IGangStatManager>();
    localizer = provider.GetService<IStringLocalizer>();
    players   = provider.GetService<IPlayerManager>();
  }

  public override int Cost => 10000;

  public override async Task OnPurchase(IGangPlayer player) {
    Value = true;
    if (player.GangId == null || player.GangRank == null) return;
    if (gangStats == null || gangs == null || localizer == null) return;
    await gangStats.SetForGang(player.GangId.Value, this);
    var gang = await gangs.GetGang(player.GangId.Value);
    if (gang == null) return;
    var str = localizer.Get(MSG.PERK_PURCHASED,
      player.Name ?? player.Steam.ToString(), Name);
    await ((IGangChatPerk)this).SendGangChat(gang, str);
  }

  public override string StatId => "gang_native_chat";
  public override string Name => "Gang Chat";
  public override string Description => "Whether the gang has a chat.";

  public async Task SendGangChat(string name, IGang gang, string message) {
    if (localizer == null || players == null) return;
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

  [GameEventHandler]
  public HookResult OnChat(EventPlayerChat ev, GameEventInfo _) {
    if (!ev.Text.StartsWith('.')) return HookResult.Continue;
    var player = Utilities.GetPlayerFromUserid(ev.Userid);
    if (player == null || !player.IsValid) return HookResult.Continue;

    if (players == null || gangStats == null || localizer == null
      || gangs == null) {
      player.PrintToChat("Gang chat is not available.");
      return HookResult.Continue;
    }

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
        wrapper.PrintToChat(localizer.Get(MSG.PERK_MISSING, "Gang Chat"));
        return;
      }

      var gang = await gangs.GetGang(gangPlayer.GangId.Value);
      if (gang == null) return;
      await ((IGangChatPerk)this).SendGangChat(gangPlayer, gang, message);
    });
    return HookResult.Handled;
  }


  public bool Equals(IStat<bool>? other) {
    return other is not null && StatId == other.StatId;
  }

  public bool Value { get; set; }
}