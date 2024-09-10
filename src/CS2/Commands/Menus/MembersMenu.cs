using GangsAPI.Data;
using GangsAPI.Data.Gang;
using GangsAPI.Permissions;
using GangsAPI.Services;
using GangsAPI.Services.Menu;
using GangsAPI.Services.Player;
using Menu;

namespace Commands.Menus;

public class MembersMenu(IGang gang, IPlayerManager players,
  IMenuManager menus, IRankManager ranks)
  : AbstractPagedMenu<(IGangPlayer, IGangRank)>(menus, NativeSenders.Chat) {
  override protected async Task<List<(IGangPlayer, IGangRank)>> GetItems(
    PlayerWrapper _) {
    var members = (await players.GetMembers(gang)).ToList();
    var memberRanks   = await ranks.GetRanks(gang.GangId);

    return members.Select(m => (m, memberRanks.First(r => r.Rank == m.GangRank)))
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