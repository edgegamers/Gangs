using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using GangsAPI;
using GangsAPI.Data;
using GangsAPI.Data.Gang;
using GangsAPI.Data.Stat;
using GangsAPI.Perks;
using GangsAPI.Services.Gang;
using GangsAPI.Services.Player;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace Stats.Perk;

public class GangChatPerk : BaseStat<bool>, IGangChatPerk {
  private readonly IGangManager? gangs;
  private readonly IGangStatManager? gangStats;
  private readonly IStringLocalizer? localizer;
  private readonly IPlayerManager? players;

  public GangChatPerk(bool Value) { this.Value = Value; }

  public GangChatPerk(IServiceProvider provider) {
    gangs     = provider.GetService<IGangManager>();
    gangStats = provider.GetService<IGangStatManager>();
    localizer = provider.GetService<IStringLocalizer>();
    players   = provider.GetService<IPlayerManager>();
  }

  public int Cost => 10000;

  public override string StatId => "gang_native_chat";
  public override string Name => "Gang Chat";
  public override string Description => "Whether the gang has a chat.";

  public async Task
    SendGangChat(IGangPlayer player, IGang gang, string message) {
    if (players == null || player.GangId == null) return;
    if (localizer == null) return;
    var members = await players.GetMembers(player.GangId.Value);
    await Server.NextFrameAsync(() => {
      foreach (var member in members) {
        var target = Utilities.GetPlayerFromSteamId(member.Steam);
        if (target == null || !target.IsValid) continue;
        target.PrintToChat(localizer.Get(MSG.GANG_CHAT_FORMAT, gang.Name,
          player.Name ?? player.Steam.ToString(), message));
      }
    });
  }

  public override IStat<bool> Clone() { return new GangChatPerk(Value); }

  [GameEventHandler]
  public HookResult OnChat(EventPlayerChat ev, GameEventInfo _) {
    Server.PrintToChatAll($"Chat: {ev.Text}");
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
      await SendGangChat(gangPlayer, gang, message);
    });
    return HookResult.Handled;
  }

  public Task OnPurchase(IGangPlayer player) {
    Server.NextFrame(() => {
      Server.PrintToChatAll($"Player {player.Name} purchased gang chat.");
    });
    return Task.CompletedTask;
  }
}