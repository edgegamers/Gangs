﻿using GangsAPI.Data;
using GangsAPI.Services.Menu;

namespace Menu;

public abstract class AbstractMenu<T>(IMenuManager menuMgr,
  Func<PlayerWrapper, string, Task> printer) : IMenu {
  protected readonly IMenuManager MenuMgr = menuMgr;
  protected readonly Func<PlayerWrapper, string, Task> Printer = printer;

  public virtual async Task Open(PlayerWrapper player) {
    var items = await GetItems(player);
    await Show(player, items);
  }

  public Task Close(PlayerWrapper player) { return Task.CompletedTask; }

  public virtual async Task AcceptInput(PlayerWrapper player, int input) {
    if (input == 0) {
      await Close(player);
      return;
    }

    var items = await GetItems(player);

    await HandleItemSelection(player, items, input);
  }

  // Abstract methods that must be implemented by derived menus
  abstract protected Task<List<T>> GetItems(PlayerWrapper player);

  abstract protected Task HandleItemSelection(PlayerWrapper player,
    List<T> items, int selectedIndex);

  virtual protected async Task Show(PlayerWrapper player, List<T> items) {
    for (var i = 0; i < items.Count; i++) {
      var str = await FormatItem(i, items[i]);
      await Printer.Invoke(player, str);
    }
  }

  // abstract protected Task SendItem(PlayerWrapper player, string item);

  // Utility methods
  abstract protected Task<string> FormatItem(int index, T item);
}