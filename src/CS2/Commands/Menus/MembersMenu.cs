using GangsAPI.Data;
using GangsAPI.Data.Gang;
using GangsAPI.Permissions;
using GangsAPI.Services;
using GangsAPI.Services.Menu;
using GangsAPI.Services.Player;
using Menu;

namespace Commands.Menus;

public class MembersMenu(IGang gang, IPlayerManager playerMgr,
  IMenuManager menuMgr, IRankManager rankMgr)
  : AbstractPagedMenu<(IGangPlayer, IGangRank)>(menuMgr, NativeSenders.Chat) {
  override protected async Task<List<(IGangPlayer, IGangRank)>> GetItems(
    PlayerWrapper _) {
    var members = (await playerMgr.GetMembers(gang)).ToList();
    var ranks   = await rankMgr.GetRanks(gang.GangId);

    return members.Select(m => (m, ranks.First(r => r.Rank == m.GangRank)))
     .ToList();
  }

  override protected Task HandleItemSelection(PlayerWrapper player,
    List<(IGangPlayer, IGangRank)> items, int selectedIndex) {
    var (member, rank) = items[selectedIndex - 1];
    player.PrintToChat(
      $"{member.Name} - {rank.Name} - {member.Steam} - {member.GangRank} - {rank.Permissions.Describe()}");
    return Task.CompletedTask;
  }

  override protected Task<string> FormatItem(int index,
    (IGangPlayer, IGangRank) item) {
    return Task.FromResult($"{item.Item2.Name}: {item.Item1.Name}");
  }
}