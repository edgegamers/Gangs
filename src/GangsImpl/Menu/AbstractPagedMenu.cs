using GangsAPI.Data;
using GangsAPI.Services.Menu;

namespace Menu;

public abstract class AbstractPagedMenu<T>(IMenuManager menuMgr,
  int itemsPerPage = 5) : IMenu {
  protected readonly IMenuManager menuMgr = menuMgr;
  protected readonly int itemsPerPage = itemsPerPage;

  public async Task Open(PlayerWrapper player) {
    var items = await GetItems();
    int totalPages =
      (items.Count + itemsPerPage - 1)
      / itemsPerPage; // Calculate number of pages
    await ShowPage(player, items, 1, totalPages);
  }

  public Task Close(PlayerWrapper player) { return Task.CompletedTask; }

  public async Task AcceptInput(PlayerWrapper player, int input) {
    var items      = await GetItems();
    int totalPages = (items.Count + itemsPerPage - 1) / itemsPerPage;

    if (input == 0) {
      await Close(player);
      return;
    }

    // Handle page navigation
    if (input == 9 && HasNextPage(player)) {
      int currentPage = GetCurrentPage(player);
      await ShowPage(player, items, currentPage + 1, totalPages);
    } else if (input == 8 && HasPreviousPage(player)) {
      int currentPage = GetCurrentPage(player);
      await ShowPage(player, items, currentPage - 1, totalPages);
    } else { await HandleItemSelection(player, items, input); }
  }

  // Abstract methods that must be implemented by derived menus
  abstract protected Task<List<T>> GetItems();

  abstract protected Task HandleItemSelection(PlayerWrapper player,
    List<T> items, int selectedIndex);

  virtual protected Task ShowPage(PlayerWrapper player, List<T> items,
    int currentPage, int totalPages) {
    int startIndex = (currentPage - 1) * itemsPerPage;
    var pageItems  = items.Skip(startIndex).Take(itemsPerPage).ToList();

    if (totalPages != 1) player.PrintToChat($"Page {currentPage}/{totalPages}");
    for (int i = 0; i < pageItems.Count; i++) {
      player.PrintToChat($"{FormatItem(i, pageItems[i])}");
    }

    // Display navigation options
    if (currentPage > 1) player.PrintToChat("8. Previous Page");
    if (currentPage < totalPages) player.PrintToChat("9. Next Page");
    player.PrintToChat("0. Close Menu");
    return Task.CompletedTask;
  }

  // Utility methods
  abstract protected string FormatItem(int index, T item);

  private bool HasNextPage(PlayerWrapper player)
    => GetCurrentPage(player) < GetTotalPages(player);

  private bool HasPreviousPage(PlayerWrapper player)
    => GetCurrentPage(player) > 1;

  virtual protected int GetCurrentPage(PlayerWrapper player)
    => 1; // Override this to track pages per player

  virtual protected int GetTotalPages(PlayerWrapper player)
    => 1; // Override this to track pages per player
}