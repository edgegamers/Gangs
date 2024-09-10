using CounterStrikeSharp.API.Modules.Utils;
using GangsAPI;
using GangsAPI.Data;
using GangsAPI.Data.Gang;
using GangsAPI.Permissions;
using GangsAPI.Services;
using GangsAPI.Services.Gang;
using GangsAPI.Services.Menu;
using GangsAPI.Services.Player;
using Microsoft.Extensions.Localization;
using Stats;
using IMenu = GangsAPI.Services.Menu.IMenu;

namespace Commands.Menus;

public class GangMenu(IGang gang, IPlayerManager playerMgr,
  IRankManager rankMgr, IMenuManager menuMgr, IGangStatManager gangStatManager,
  IStringLocalizer localizer) : IMenu {
  private readonly InvitationStat invitationStat = new();
  private IList<IGangPlayer> members = new List<IGangPlayer>();

  public async Task Open(PlayerWrapper player) {
    members = (await playerMgr.GetMembers(gang)).ToList();

    var member = members.FirstOrDefault(p => p.Steam == player.Steam);

    if (member is { GangRank: null }) {
      player.PrintToChat(localizer.Get(MSG.GENERIC_ERROR_INFO,
        "You do not have an associated rank within this gang"));
      return;
    }

    var rank = member == null ?
      Perm.NONE :
      (await rankMgr.GetRank(gang.GangId, member.GangRank.Value))!.Permissions;

    player.PrintToChat(" ");
    player.PrintToChat(
      $" {ChatColors.Red}Gangs {ChatColors.Grey}- {ChatColors.Green}{gang.Name}");
    player.PrintToChat($" {ChatColors.Grey}------------------------");

    await addMemberItem(rank, player);
    await addInviteItem(rank, player);
  }

  private Task addMemberItem(Perm rank, PlayerWrapper player) {
    var memberPrefix = (rank & Perm.VIEW_MEMBERS) != 0 ?
      ChatColors.DarkRed + "1" :
      ChatColors.LightRed + "X";
    player.PrintToChat(
      $" {memberPrefix} | {ChatColors.Yellow}{members.Count}{ChatColors.LightRed} Members");
    return Task.CompletedTask;
  }

  private async Task addInviteItem(Perm rank, PlayerWrapper player) {
    if (!rank.HasFlag(Perm.INVITE_OTHERS)) return;

    var (success, invites) =
      await gangStatManager.GetForGang<string>(gang, invitationStat.StatId);
    if (!success || invites == null) return;

    player.PrintToChat($"{invites.Length} Invites");
  }

  public Task Close(PlayerWrapper player) { return Task.CompletedTask; }

  public async Task AcceptInput(PlayerWrapper player, int input) {
    switch (input) {
      case 1:
        player.Player?.ExecuteClientCommandFromServer("css_gang members");
        break;
      case 2:
        player.Player?.ExecuteClientCommandFromServer("css_gang invites");
        break;
    }
  }
}