using CounterStrikeSharp.API.Modules.Utils;
using GangsAPI.Data;
using GangsAPI.Services.Menu;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace GangsAPI.Menu;

public abstract class AbstractPagedMenu<T>(IServiceProvider provider,
  Func<PlayerWrapper, string, Task> printer, int itemsPerPage = 7)
  : AbstractMenu<T>(provider.GetRequiredService<IMenuManager>(), printer) {
  protected readonly Dictionary<ulong, int> CurrentPages = new();
  protected readonly int ItemsPerPage = itemsPerPage;

  private readonly string left =
    $"{ChatColors.DarkRed}<- {ChatColors.LightYellow}/8";

  protected readonly IStringLocalizer Localizer =
    provider.GetRequiredService<IStringLocalizer>();

  protected readonly IServiceProvider Provider = provider;

  private readonly string right =
    $"{ChatColors.LightYellow}/9 {ChatColors.DarkRed}->";

  public override async Task Open(PlayerWrapper player) {
    var items = await GetItems(player);
    var totalPages =
      (items.Count + ItemsPerPage - 1)
      / ItemsPerPage; // Calculate number of pages
    var pageItems = items.Take(ItemsPerPage).ToList();
    await ShowPage(player, pageItems, GetCurrentPage(player), totalPages);
  }

  public override async Task AcceptInput(PlayerWrapper player, int input) {
    var start       = (GetCurrentPage(player) - 1) * ItemsPerPage;
    var allItems    = await GetItems(player);
    var pageItems   = allItems.Skip(start).Take(ItemsPerPage).ToList();
    var totalPages  = (allItems.Count + ItemsPerPage - 1) / ItemsPerPage;
    var currentPage = GetCurrentPage(player);

    switch (input) {
      case 0:
        await Menus.CloseMenu(player);
        break;
      // Handle page navigation
      case 8 when HasPreviousPage(player): {
        pageItems = allItems.Skip((currentPage - 2) * ItemsPerPage)
         .Take(ItemsPerPage)
         .ToList();
        await ShowPage(player, pageItems, currentPage - 1, totalPages);
        break;
      }
      case 9 when HasNextPage(player): {
        pageItems = allItems.Skip(currentPage * ItemsPerPage)
         .Take(ItemsPerPage)
         .ToList();
        await ShowPage(player, pageItems, currentPage + 1, totalPages);
        break;
      }
      default:
        await HandleItemSelection(player, pageItems, input - 1);
        break;
    }
  }

  override abstract protected Task HandleItemSelection(PlayerWrapper player,
    List<T> items, int selectedIndex);

  virtual protected async Task ShowPage(PlayerWrapper player, List<T> items,
    int currentPage, int totalPages) {
    await ShowPaged(player, items, currentPage < totalPages, currentPage > 1);
    var pageStr = "";
    if (totalPages != 1)
      pageStr =
        $"{ChatColors.Grey}Page {ChatColors.Yellow}{currentPage}{ChatColors.Default}/{ChatColors.LightYellow}{totalPages}{ChatColors.Default} | ";

    // Display navigation options
    await Printer(player, $"0. {pageStr}{ChatColors.LightRed}Close");

    CurrentPages[player.Steam] = currentPage;
  }

  virtual protected async Task ShowPaged(PlayerWrapper player, List<T> items,
    bool hasNext, bool hasPrev) {
    for (var i = 0; i < items.Count; i++) {
      var str = await FormatItem(player, i + 1, items[i]);
      if (i == 0)
        str = hasNext switch {
          true when hasPrev  => $"{left} {ChatColors.Default}{str} {right}",
          true               => $"{str} {right}",
          false when hasPrev => $"{left} {str}",
          _                  => str
        };

      await Printer.Invoke(player, str);
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