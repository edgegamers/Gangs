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
  : AbstractPagedMenu<(string?, ulong)>(menuMgr) {
  private readonly InvitationStat invitationStat = new();

  override protected async Task<List<(string?, ulong)>> GetItems() {
    var (success, invites) =
      (await gangStatMgr.GetForGang<string>(gang, invitationStat.StatId));
    if (!success) return [];

    var steams = new InvitationStat() { Value = invites }.GetAsSteams();

    List<(string?, ulong)> items = [];

    foreach (var steam in steams) {
      var player = await playerMgr.GetPlayer(steam);
      if (player is null) continue;

      items.Add((player.Name, steam));
    }

    return items;
  }

  override protected Task HandleItemSelection(PlayerWrapper player,
    List<(string?, ulong)> items, int selectedIndex) {
    var (name, steam) = items[selectedIndex - 1];
    player.PrintToChat($"{name} - {steam}");
    return Task.CompletedTask;
  }

  override protected string FormatItem(int index, (string?, ulong) item) {
    return $"{item.Item1 ?? item.Item2.ToString()}";
  }
}