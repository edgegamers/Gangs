using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Utils;
using GangsAPI.Data;
using GangsAPI.Data.Gang;
using GangsAPI.Exceptions;
using GangsAPI.Perks;
using GangsAPI.Permissions;
using GangsAPI.Services;
using GangsAPI.Services.Commands;
using GangsAPI.Services.Gang;
using GangsAPI.Services.Menu;
using GangsAPI.Services.Player;
using Microsoft.Extensions.DependencyInjection;
using Stats.Stat.Gang;
using IMenu = GangsAPI.Services.Menu.IMenu;

namespace Commands.Menus;

public class GangMenu(IServiceProvider provider, IGang gang) : IMenu {
  private readonly ICommandManager commands =
    provider.GetRequiredService<ICommandManager>();

  private readonly string doorPolicyId = new DoorPolicyStat().StatId;

  private readonly IGangStatManager gangStatManager =
    provider.GetRequiredService<IGangStatManager>();

  private readonly string invitationStatId = new InvitationStat().StatId;

  private readonly IMenuManager menus =
    provider.GetRequiredService<IMenuManager>();

  private readonly IPlayerManager players =
    provider.GetRequiredService<IPlayerManager>();

  private readonly IRankManager ranks =
    provider.GetRequiredService<IRankManager>();

  private IList<IGangPlayer> members = new List<IGangPlayer>();

  public async Task Open(PlayerWrapper player) {
    members = (await players.GetMembers(gang)).ToList();

    var member = members.FirstOrDefault(p => p.Steam == player.Steam);

    if (member is { GangRank: null })
      throw new GangException("no associated rank within gang");

    var rank = member == null ?
      Perm.NONE :
      (await ranks.GetRank(gang.GangId, member.GangRank.Value))!.Permissions;

    player.PrintToChat(" ");
    var title =
      $" {ChatColors.DarkRed}!GANGS {ChatColors.Default}- {ChatColors.Grey}{gang.Name}";

    var capacity = provider.GetService<ICapacityPerk>();

    if (capacity != null) {
      var cap = await capacity.GetCapacity(gang);
      title +=
        $" {ChatColors.LightRed}[{ChatColors.Yellow}{members.Count}{ChatColors.Grey}/{ChatColors.Orange}{cap}{ChatColors.LightRed}]";
    }

    var desc = provider.GetService<IMotdPerk>();
    if (desc != null) {
      var motd                = await desc.GetMotd(gang);
      if (motd != null) title += $"\\n {ChatColors.Grey}{motd}";
    }

    player.PrintToChat(title);

    await addMemberItem(rank, player);
    await addInviteItem(rank, player);
    await addPerkItem(rank, player);
    addPermItem(rank, player);
    addRankItem(rank, player);

    player.PrintToChat($" {ChatColors.Grey}... or /gang help");
  }

  public Task Close(PlayerWrapper player) { return Task.CompletedTask; }

  public async Task AcceptInput(PlayerWrapper player, int input) {
    switch (input) {
      case 1:
        await commands.ProcessCommand(player, CommandCallingContext.Chat,
          "css_gang", "members");
        break;
      case 2:
        await commands.ProcessCommand(player, CommandCallingContext.Chat,
          "css_gang", "invites");
        break;
      case 3:
        await commands.ProcessCommand(player, CommandCallingContext.Chat,
          "css_gang", "perks");
        break;
      case 4:
        await commands.ProcessCommand(player, CommandCallingContext.Chat,
          "css_gang", "perms");
        break;
      case 5:
        await commands.ProcessCommand(player, CommandCallingContext.Chat,
          "css_gang", "ranks");
        break;
      case 0:
        await menus.CloseMenu(player);
        break;
    }
  }

  private Task addMemberItem(Perm rank, PlayerWrapper player) {
    var memberPrefix = (rank & Perm.VIEW_MEMBER_DETAILS) != 0 ?
      ChatColors.DarkRed + "1" :
      ChatColors.LightRed + "X";
    player.PrintToChat(
      $" {memberPrefix}{ChatColors.Grey}. {ChatColors.Yellow}{members.Count}{ChatColors.LightRed} Members");
    return Task.CompletedTask;
  }

  private async Task addInviteItem(Perm rank, PlayerWrapper player) {
    if (!rank.HasFlag(Perm.INVITE_OTHERS)) return;

    var invites =
      await gangStatManager.GetForGang<InvitationData>(gang, invitationStatId);
    if (invites == null) invites = new InvitationData();

    var entry =
      $" {ChatColors.DarkRed}2{ChatColors.Grey}. {ChatColors.Yellow}{invites.GetInvitedSteams().Count} {ChatColors.LightRed}Invite";

    var doorPolicy =
      await gangStatManager.GetForGang<DoorPolicy>(gang, doorPolicyId);
    if (invites.GetInvitedSteams().Count != 1) entry += "s";

    if (doorPolicy != DoorPolicy.REQUEST_ONLY) {
      player.PrintToChat(entry);
      return;
    }

    var count = invites.GetRequestedSteams().Count;

    player.PrintToChat(entry
      + $" {ChatColors.Grey}| {count} {ChatColors.LightRed}Request{(count != 1 ? "s" : "")}");
  }

  private Task addPerkItem(Perm _, PlayerWrapper player) {
    player.PrintToChat(
      $" {ChatColors.DarkRed}3{ChatColors.Grey}. {ChatColors.LightRed}Perks");
    return Task.CompletedTask;
  }

  private void addPermItem(Perm rank, PlayerWrapper player) {
    if (!rank.HasFlag(Perm.MANAGE_RANKS)) return;
    player.PrintToChat(
      $" {ChatColors.DarkRed}4{ChatColors.Grey}. {ChatColors.LightRed}Permissions");
  }

  private void addRankItem(Perm rank, PlayerWrapper player) {
    if (!rank.HasFlag(Perm.CREATE_RANKS)) return;
    player.PrintToChat(
      $" {ChatColors.DarkRed}5{ChatColors.Grey}. {ChatColors.LightRed}Ranks");
  }
}