using GangsAPI.Data;
using GangsAPI.Data.Gang;
using GangsAPI.Services.Gang;
using GangsAPI.Services.Menu;
using GangsAPI.Services.Player;
using Menu;
using Stats;

namespace Commands.Menus;

public class OutgoingInvitesMenu(IMenuManager menuMgr, IGang gang,
  IGangStatManager gangStatMgr, IPlayerManager playerMgr)
  : AbstractPagedMenu<InvitationEntry>(menuMgr, NativeSenders.Chat) {
  private readonly InvitationStat invitationStat = new();

  override protected async Task<List<InvitationEntry>> GetItems() {
    var (success, invites) =
      (await gangStatMgr.GetForGang<InvitationData>(gang,
        invitationStat.StatId));
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

  override protected string FormatItem(int index, InvitationEntry item) {
    return $"[{index + 1}] {item.Steam} by {item.Inviter} on {item.Date}";
  }
}