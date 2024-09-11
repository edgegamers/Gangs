using GangsAPI.Data;
using GangsAPI.Perks;
using GangsAPI.Services.Menu;
using Menu;
using Microsoft.Extensions.DependencyInjection;

namespace Commands.Menus;

public class PerksMenu(IServiceProvider provider)
  : AbstractPagedMenu<IPerk>(provider, NativeSenders.Chat) {
  override protected Task<List<IPerk>> GetItems(PlayerWrapper player) {
    return Task.FromResult(Provider.GetServices<IPerk>().ToList());
  }

  override protected Task HandleItemSelection(PlayerWrapper player,
    List<IPerk> items, int selectedIndex) {
    var perk = items[selectedIndex];
    player.PrintToChat($"You selected {perk.Name}");
    return Task.CompletedTask;
  }

  override protected Task<string> FormatItem(int index, IPerk item) {
    return Task.FromResult($"{item.Name} - {item.Cost}");
  }
}