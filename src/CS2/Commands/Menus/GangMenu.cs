using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Modules.Utils;
using GangsAPI;
using GangsAPI.Data;
using GangsAPI.Data.Gang;
using GangsAPI.Permissions;
using GangsAPI.Services;
using GangsAPI.Services.Gang;
using GangsAPI.Services.Player;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Stats;
using IMenu = GangsAPI.Services.Menu.IMenu;

namespace Commands.Menus;

public class GangMenu(IServiceProvider provider, IGang gang) : IMenu {
  private readonly InvitationStat invitationStat = new();
  private IList<IGangPlayer> members = new List<IGangPlayer>();

  private readonly IGangManager gangs =
    provider.GetRequiredService<IGangManager>();

  private readonly IPlayerManager players =
    provider.GetRequiredService<IPlayerManager>();

  private readonly IStringLocalizer localizer =
    provider.GetRequiredService<IStringLocalizer>();

  private readonly IRankManager ranks =
    provider.GetRequiredService<IRankManager>();

  private readonly IGangStatManager gangStatManager =
    provider.GetRequiredService<IGangStatManager>();

  public async Task Open(PlayerWrapper player) {
    members = (await players.GetMembers(gang)).ToList();

    var member = members.FirstOrDefault(p => p.Steam == player.Steam);

    if (member is { GangRank: null }) {
      player.PrintToChat(localizer.Get(MSG.GENERIC_ERROR_INFO,
        "You do not have an associated rank within this gang"));
      return;
    }

    var rank = member == null ?
      Perm.NONE :
      (await ranks.GetRank(gang.GangId, member.GangRank.Value))!.Permissions;

    player.PrintToChat(" ");
    player.PrintToChat(
      $" {ChatColors.DarkRed}!GANGS {ChatColors.Default}- {ChatColors.Grey}{gang.Name}");

    await addMemberItem(rank, player);
    await addInviteItem(rank, player);
    await addPerkItem(rank, player);
  }

  public Task Close(PlayerWrapper player) { return Task.CompletedTask; }

  public async Task AcceptInput(PlayerWrapper player, int input) {
    await Server.NextFrameAsync(() => {
      switch (input) {
        case 1:
          player.Player?.ExecuteClientCommandFromServer("css_gang members");
          break;
        case 2:
          player.Player?.ExecuteClientCommandFromServer("css_gang invites");
          break;
        case 3:
          player.Player?.ExecuteClientCommandFromServer("css_gang perks");
          break;
      }
    });
  }

  private Task addMemberItem(Perm rank, PlayerWrapper player) {
    var memberPrefix = (rank & Perm.VIEW_MEMBERS) != 0 ?
      ChatColors.DarkRed + "1" :
      ChatColors.LightRed + "X";
    player.PrintToChat(
      $" {memberPrefix} {ChatColors.Grey}| {ChatColors.Yellow}{members.Count}{ChatColors.LightRed} Members");
    return Task.CompletedTask;
  }

  private Task addPerkItem(Perm rank, PlayerWrapper player) {
    player.PrintToChat(
      $" {ChatColors.DarkRed}3 {ChatColors.Grey}| {ChatColors.LightRed}Perks");
    return Task.CompletedTask;
  }

  private async Task addInviteItem(Perm rank, PlayerWrapper player) {
    if (!rank.HasFlag(Perm.INVITE_OTHERS)) return;

    var (success, invites) =
      await gangStatManager.GetForGang<InvitationData>(gang,
        invitationStat.StatId);
    if (!success || invites == null) return;

    player.PrintToChat(
      $" {ChatColors.DarkRed}2 | {ChatColors.Yellow}{invites.GetInvitedSteams().Count}{ChatColors.LightRed} Invite"
      + (invites.GetInvitedSteams().Count == 1 ? "" : "s"));
  }
}