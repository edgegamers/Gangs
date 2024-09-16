using CounterStrikeSharp.API.Modules.Utils;
using GangsAPI.Data;
using GangsAPI.Services.Menu;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace Menu;

public abstract class AbstractPagedMenu<T>(IServiceProvider provider,
  Func<PlayerWrapper, string, Task> printer, int itemsPerPage = 7)
  : AbstractMenu<T>(provider.GetRequiredService<IMenuManager>(), printer) {
  protected readonly IStringLocalizer Localizer =
    provider.GetRequiredService<IStringLocalizer>();

  protected readonly IServiceProvider Provider = provider;
  protected readonly int ItemsPerPage = itemsPerPage;
  protected readonly Dictionary<ulong, int> CurrentPages = new();

  public override async Task Open(PlayerWrapper player) {
    var items = await GetItems(player);
    var totalPages =
      (items.Count + ItemsPerPage - 1)
      / ItemsPerPage; // Calculate number of pages
    await ShowPage(player, items, GetCurrentPage(player), totalPages);
  }

  public override async Task AcceptInput(PlayerWrapper player, int input) {
    var items      = await GetItems(player);
    var totalPages = (items.Count + ItemsPerPage - 1) / ItemsPerPage;

    switch (input) {
      case 0:
        await Close(player);
        break;
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
        await HandleItemSelection(player, items, input - 1);
        break;
    }
  }

  override abstract protected Task HandleItemSelection(PlayerWrapper player,
    List<T> items, int selectedIndex);

  virtual protected async Task ShowPage(PlayerWrapper player, List<T> items,
    int currentPage, int totalPages) {
    var startIndex = (currentPage - 1) * ItemsPerPage;
    var pageItems  = items.Skip(startIndex).Take(ItemsPerPage).ToList();

    if (totalPages != 1) player.PrintToChat($"Page {currentPage}/{totalPages}");
    await ShowPaged(player, pageItems, currentPage < totalPages,
      currentPage > 1);

    // Display navigation options
    if (currentPage > 1) await Printer(player, "8. Previous Page");
    if (currentPage < totalPages) await Printer(player, "9. Next Page");
    await Printer(player, "0. Close");

    CurrentPages[player.Steam] = currentPage;
  }

  virtual protected async Task ShowPaged(PlayerWrapper player, List<T> items,
    bool hasNext, bool hasPrev) {
    for (var i = 0; i < items.Count; i++) {
      var str = await FormatItem(player, i + 1, items[i]);
      if (i == 0)
        str = hasNext switch {
          true when hasPrev  => $"<- /8  {str}  /9 ->",
          true               => $"{str}  /9 ->",
          false when hasPrev => $"<- /8  {str}",
          _                  => str
        };

      foreach (var s in str.Split('\n')) await Printer.Invoke(player, s);
    }
  }

  protected bool HasNextPage(PlayerWrapper player) {
    return GetCurrentPage(player) < GetTotalPages(player);
  }

  protected bool HasPreviousPage(PlayerWrapper player) {
    return GetCurrentPage(player) > 1;
  }

  virtual protected int GetCurrentPage(PlayerWrapper player) {
    return CurrentPages.TryGetValue(player.Steam, out var page) ? page : 1;
  }

  virtual protected int GetTotalPages(PlayerWrapper player) {
    return (GetItems(player).GetAwaiter().GetResult().Count + ItemsPerPage - 1)
      / ItemsPerPage;
  }
}