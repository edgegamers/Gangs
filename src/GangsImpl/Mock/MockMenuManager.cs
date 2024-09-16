using CounterStrikeSharp.API.Core;
using GangsAPI.Data;
using GangsAPI.Services.Menu;

namespace Mock;

public class MockMenuManager : IMenuManager {
  private readonly Dictionary<ulong, IMenu> activeMenus = new();

  public virtual void Start(BasePlugin? plugin, bool hotReload) { }

  public IMenu? GetActiveMenu(ulong steam) {
    return activeMenus.TryGetValue(steam, out var menu) ? menu : null;
  }

  public Task<bool> OpenMenu(PlayerWrapper player, IMenu menu) {
    if (activeMenus.TryGetValue(player.Steam, out var previous))
      previous.Close(player);
    activeMenus[player.Steam] = menu;
    menu.Open(player);
    return Task.FromResult(true);
  }

  public Task<bool> CloseMenu(PlayerWrapper player) {
    var activeMenu = ((IMenuManager)this).GetActiveMenu(player);
    if (activeMenu == null) return Task.FromResult(false);
    activeMenu.Close(player);
    activeMenus.Remove(player.Steam);
    return Task.FromResult(true);
  }

  public Task<bool> AcceptInput(PlayerWrapper player, int input) {
    var activeMenu = ((IMenuManager)this).GetActiveMenu(player);
    if (activeMenu == null) return Task.FromResult(false);
    activeMenu.AcceptInput(player, input);
    return Task.FromResult(true);
  }
}