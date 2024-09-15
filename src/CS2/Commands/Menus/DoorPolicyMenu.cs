using GangsAPI.Data;
using GangsAPI.Data.Gang;
using GangsAPI.Services.Menu;
using Menu;

namespace Commands.Menus;

public class DoorPolicyMenu(IMenuManager menus,
  Func<PlayerWrapper, string, Task> printer)
  : AbstractMenu<DoorPolicy>(menus, printer) {
  override protected Task<List<DoorPolicy>> GetItems(PlayerWrapper player) {
    return Task.FromResult(Enum.GetValues<DoorPolicy>().ToList());
  }

  override protected Task HandleItemSelection(PlayerWrapper player,
    List<DoorPolicy> items, int selectedIndex) {
    var policy = items[selectedIndex];
    player.PrintToChat($"You picked door policy: {policy}");
    return Task.CompletedTask;
  }

  override protected Task<string> FormatItem(PlayerWrapper player, int index,
    DoorPolicy item) {
    return Task.FromResult(item.ToString());
  }
}