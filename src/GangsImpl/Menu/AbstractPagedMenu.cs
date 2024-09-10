using GangsAPI.Data;
using GangsAPI.Services.Menu;

namespace Menu;

public abstract class AbstractPagedMenu<T>(IMenuManager menuMgr,
  Func<PlayerWrapper, string, Task> printer, int itemsPerPage = 5)
  : AbstractMenu<T>(menuMgr, printer) {
  public override async Task Open(PlayerWrapper player) {
    var items = await GetItems(player);
    var totalPages =
      (items.Count + itemsPerPage - 1)
      / itemsPerPage; // Calculate number of pages
    await ShowPage(player, items, 1, totalPages);
  }

  public override async Task AcceptInput(PlayerWrapper player, int input) {
    var items      = await GetItems(player);
    var totalPages = (items.Count + itemsPerPage - 1) / itemsPerPage;

    switch (input) {
      case 0:
        await Close(player);
        return;
      // Handle page navigation
      case 9 when HasNextPage(player): {
        var currentPage = GetCurrentPage(player);
        await ShowPage(player, items, currentPage + 1, totalPages);
        break;
      }
      case 8 when HasPreviousPage(player): {
        var currentPage = GetCurrentPage(player);
        await ShowPage(player, items, currentPage - 1, totalPages);
        break;
      }
      default:
        await HandleItemSelection(player, items, input);
        break;
    }
  }

  override abstract protected Task HandleItemSelection(PlayerWrapper player,
    List<T> items, int selectedIndex);

  virtual protected async Task ShowPage(PlayerWrapper player, List<T> items,
    int currentPage, int totalPages) {
    var startIndex = (currentPage - 1) * itemsPerPage;
    var pageItems  = items.Skip(startIndex).Take(itemsPerPage).ToList();

    if (totalPages != 1) player.PrintToChat($"Page {currentPage}/{totalPages}");
    for (var i = 0; i < pageItems.Count; i++)
      await Printer.Invoke(player, await FormatItem(i, pageItems[i]));

    // Display navigation options
    if (currentPage > 1) await Printer.Invoke(player, "8. Previous Page");
    if (currentPage < totalPages) await Printer.Invoke(player, "9. Next Page");
    await Printer.Invoke(player, "0. Close Menu");
  }

  private bool HasNextPage(PlayerWrapper player) {
    return GetCurrentPage(player) < GetTotalPages(player);
  }

  private bool HasPreviousPage(PlayerWrapper player) {
    return GetCurrentPage(player) > 1;
  }

  virtual protected int GetCurrentPage(PlayerWrapper player) {
    return 1;
    // Override this to track pages per player
  }

  virtual protected int GetTotalPages(PlayerWrapper player) {
    return 1;
    // Override this to track pages per player
  }
}