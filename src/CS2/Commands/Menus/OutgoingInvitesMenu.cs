using CounterStrikeSharp.API.Modules.Utils;
using GangsAPI.Data;
using GangsAPI.Data.Gang;
using GangsAPI.Services.Gang;
using GangsAPI.Services.Menu;
using GangsAPI.Services.Player;
using Menu;
using Stats;

namespace Commands.Menus;

public class OutgoingInvitesMenu(IMenuManager menus, IGang gang,
  IGangStatManager gangStats, IPlayerManager players)
  : AbstractPagedMenu<InvitationEntry>(menus, NativeSenders.Chat) {
  private readonly InvitationStat invitationStat = new();

  override protected async Task<List<InvitationEntry>>
    GetItems(PlayerWrapper _) {
    var (success, invites) =
      await gangStats.GetForGang<InvitationData>(gang, invitationStat.StatId);
    if (!success || invites == null) return [];

    return invites.GetEntries();
  }

  override protected Task HandleItemSelection(PlayerWrapper player,
    List<InvitationEntry> items, int selectedIndex) {
    var entry = items[selectedIndex];
    Printer.Invoke(player,
      $"Invitation sent to {entry.Steam} by {entry.Inviter} on {entry.Date}");
    return Task.CompletedTask;
  }

  override protected async Task<string> FormatItem(int index,
    InvitationEntry item) {
    var invited = await players.GetPlayer(item.Steam);
    var inviter = await players.GetPlayer(item.Inviter);

    var invitedName = invited?.Name ?? item.Steam.ToString();
    var inviterName = inviter?.Name ?? item.Inviter.ToString();
    var text        = $"{invitedName} by {inviterName} on {item.Date}";

    if (index == 0)
      text = $"{ChatColors.Red} Gang Invitations - {gang.Name}\n" + text;

    return text;
  }
}