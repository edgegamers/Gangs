using GangsAPI.Data;

namespace Menu;

public class SimplePagedMenu(IServiceProvider provider, List<string> items)
  : AbstractPagedMenu<string>(provider, NativeSenders.Chat) {
  override protected Task<List<string?>> GetItems(PlayerWrapper player) {
    return Task.FromResult(items)!;
  }

  override protected Task HandleItemSelection(PlayerWrapper player,
    List<string?> items, int selectedIndex) {
    return Task.CompletedTask;
  }

  override protected Task<string> FormatItem(PlayerWrapper player, int index,
    string? item) {
    return Task.FromResult(item ?? "null")!;
  }
}