using GangsAPI.Data;
using GangsAPI.Data.Gang;
using GangsAPI.Permissions;
using GangsAPI.Services;
using GangsAPI.Services.Player;
using Menu;
using Microsoft.Extensions.DependencyInjection;

namespace Commands.Menus;

public class MembersMenu(IServiceProvider provider, IGang gang)
  : AbstractPagedMenu<(IGangPlayer, IGangRank)>(provider, NativeSenders.Chat) {
  private readonly IPlayerManager players =
    provider.GetRequiredService<IPlayerManager>();

  private readonly IRankManager ranks =
    provider.GetRequiredService<IRankManager>();

  override protected async Task<List<(IGangPlayer, IGangRank)>> GetItems(
    PlayerWrapper _) {
    var members     = (await players.GetMembers(gang)).ToList();
    var memberRanks = await ranks.GetRanks(gang.GangId);

    return members
     .Select(m => (m, memberRanks.First(r => r.Rank == m.GangRank)))
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