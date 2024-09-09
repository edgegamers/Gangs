using CounterStrikeSharp.API.Core;
using GangsAPI.Data;
using GangsAPI.Services.Menu;

namespace Mock;

public class MockMenuManager : IMenuManager {
  private readonly Dictionary<ulong, IMenu> activeMenus = new();

  public virtual void Start(BasePlugin? plugin, bool hotReload) { }

  public IMenu? GetActiveMenu(PlayerWrapper player) {
    return activeMenus.TryGetValue(player.Steam, out var menu) ? menu : null;
  }

  public Task<bool> OpenMenu(PlayerWrapper player, IMenu menu) {
    activeMenus[player.Steam] = menu;
    menu.Open(player);
    return Task.FromResult(true);
  }

  public Task<bool> CloseMenu(PlayerWrapper player) {
    var activeMenu = GetActiveMenu(player);
    if (activeMenu == null) return Task.FromResult(false);
    activeMenu.Close(player);
    activeMenus.Remove(player.Steam);
    return Task.FromResult(true);
  }

  public Task<bool> OnInput(PlayerWrapper player, int input) {
    var activeMenu = GetActiveMenu(player);
    if (activeMenu == null) return Task.FromResult(false);
    activeMenu.OnInput(player, input);
    return Task.FromResult(true);
  }
}