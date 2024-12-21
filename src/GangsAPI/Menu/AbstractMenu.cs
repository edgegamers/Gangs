using CounterStrikeSharp.API.Core;
using GangsAPI.Data;
using GangsAPI.Services.Menu;

namespace GangsAPI.Menu;

public abstract class AbstractMenu<T>(IMenuManager menus,
  Func<PlayerWrapper, string, Task> printer) : IMenu {
  protected readonly IMenuManager Menus = menus;
  protected readonly Func<PlayerWrapper, string, Task> Printer = printer;

  public virtual async Task Open(PlayerWrapper player) {
    var items = await GetItems(player);
    await Show(player, items);
  }

  public virtual Task Close(PlayerWrapper player) { return Task.CompletedTask; }

  public virtual async Task AcceptInput(PlayerWrapper player, int input) {
    if (input == 0) {
      await Menus.CloseMenu(player);
      return;
    }

    var items = await GetItems(player);

    await HandleItemSelection(player, items, input);
  }

  public void Start() { }

  public virtual void Dispose() { }

  public virtual void Start(BasePlugin? plugin, bool hotReload) { }

  // Abstract methods that must be implemented by derived menus
  abstract protected Task<List<T>> GetItems(PlayerWrapper player);

  abstract protected Task HandleItemSelection(PlayerWrapper player,
    List<T> items, int selectedIndex);

  virtual protected async Task Show(PlayerWrapper player, List<T> items) {
    for (var i = 0; i < items.Count; i++) {
      var str = await FormatItem(player, i, items[i]);
      foreach (var s in str.Split('\n')) await Printer.Invoke(player, s);
    }
  }

  // Utility methods
  abstract protected Task<string> FormatItem(PlayerWrapper player, int index,
    T item);
}