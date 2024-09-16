using CounterStrikeSharp.API.Modules.Utils;
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

  override protected async Task HandleItemSelection(PlayerWrapper player,
    List<(IGangPlayer, IGangRank)> items, int selectedIndex) {
    player.PrintToChat($"Got index {selectedIndex}");
    var (member, _) = items[selectedIndex];
    var menu = new MemberMenu(Provider, member);

    await Menus.OpenMenu(player, menu);
  }

  override protected Task<string> FormatItem(PlayerWrapper player, int index,
    (IGangPlayer, IGangRank) item) {
    return Task.FromResult(
      $"{ChatColors.Yellow}{index}. {ChatColors.Green}{item.Item2.Name}{ChatColors.Default}: {ChatColors.LightBlue}{item.Item1.Name}");
  }
}