using GangsAPI.Data;

namespace Menu;

public class SimplePagedMenu(IServiceProvider provider, List<string> items)
  : AbstractPagedMenu<string>(provider, NativeSenders.Chat) {
  override protected Task<List<string>> GetItems(PlayerWrapper player) {
    return Task.FromResult(items)!;
  }

  override protected Task HandleItemSelection(PlayerWrapper _1, List<string> _2,
    int _3) {
    return Task.CompletedTask;
  }

  override protected Task<string> FormatItem(PlayerWrapper player, int index,
    string item) {
    return Task.FromResult($"{index} {item}")!;
  }
}