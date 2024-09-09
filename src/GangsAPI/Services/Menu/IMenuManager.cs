using GangsAPI.Data;

namespace GangsAPI.Services.Menu;

public interface IMenuManager : IPluginBehavior {
  IMenu? GetActiveMenu(PlayerWrapper player);

  Task<bool> OpenMenu(PlayerWrapper player, IMenu menu);

  Task<bool> CloseMenu(PlayerWrapper player);

  async Task<bool> AcceptInput(PlayerWrapper player, int input) {
    var active = GetActiveMenu(player);
    if (active == null) return false;
    await active.AcceptInput(player, input);
    return true;
  }
}