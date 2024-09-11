using GangsAPI.Data;
using GangsAPI.Perks;
using GangsAPI.Services;
using GangsAPI.Services.Menu;
using GangsAPI.Services.Player;
using Menu;
using Microsoft.Extensions.DependencyInjection;

namespace Commands.Menus;

public class PerksMenu(IServiceProvider provider)
  : AbstractPagedMenu<IPerk?>(provider, NativeSenders.Chat) {
  override protected Task<List<IPerk?>> GetItems(PlayerWrapper player) {
    List<IPerk?> perks =
      Provider.GetRequiredService<IPerkManager>().Perks.ToList()!;
    perks.Insert(0, null);
    return Task.FromResult(perks.ToList());
  }

  override protected async Task HandleItemSelection(PlayerWrapper player,
    List<IPerk?> items, int selectedIndex) {
    var perk = items[selectedIndex];
    if (perk == null) return;
    player.PrintToChat($"You selected {perk.Name}");
    var gangPlayer = await Provider.GetRequiredService<IPlayerManager>()
     .GetPlayer(player.Steam);
    if (gangPlayer == null) return;
    var menu = await perk.GetMenu(gangPlayer);
    if (menu == null) return;
    await Provider.GetRequiredService<IMenuManager>().OpenMenu(player, menu);
  }

  override protected Task<string> FormatItem(int index, IPerk? item) {
    if (item == null) return Task.FromResult("Title");
    return Task.FromResult($"{index} {item.Name} - {item.Cost}");
  }
}