using CounterStrikeSharp.API.Modules.Utils;
using GangsAPI.Data;
using GangsAPI.Data.Gang;
using GangsAPI.Permissions;
using GangsAPI.Services;
using GangsAPI.Services.Menu;
using Menu;
using Microsoft.Extensions.DependencyInjection;

namespace Commands.Menus;

public class RankMenu(IServiceProvider provider, IGang gang)
  : AbstractMenu<IGangRank>(provider.GetRequiredService<IMenuManager>(),
    NativeSenders.Chat) {
  private readonly IRankManager ranks =
    provider.GetRequiredService<IRankManager>();

  override protected async Task<List<IGangRank>>
    GetItems(PlayerWrapper player) {
    var gangRanks = await ranks.GetRanks(gang);
    return gangRanks.ToList();
  }

  override protected Task HandleItemSelection(PlayerWrapper player,
    List<IGangRank> items, int selectedIndex) {
    var rank = items[selectedIndex];
    var menu = new RankEditMenu(provider, rank);
    return Menus.OpenMenu(player, menu);
  }

  override protected Task<string> FormatItem(PlayerWrapper player, int index,
    IGangRank item) {
    var result                 = $"{index}. {ChatColors.Blue}{item.Name}";
    if (item.Rank == 0) result = $"{ChatColors.DarkRed}{result}";
    return Task.FromResult(result);
  }
}