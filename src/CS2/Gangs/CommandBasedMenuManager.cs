using CounterStrikeSharp.API.Core;
using GangsAPI.Data;
using GangsAPI.Data.Command;
using GangsAPI.Services.Commands;
using GangsAPI.Services.Menu;
using Mock;

namespace GangsImpl;

public class CommandBasedMenuManager(Lazy<ICommandManager> provider)
  : MockMenuManager {
  public override void Start(BasePlugin? plugin, bool hotReload) {
    if (plugin == null) {
      for (var i = 0; i < 10; i++)
        provider.Value.RegisterCommand(new DigitCommand(this, i));
      return;
    }

    for (var i = 0; i < 10; i++) {
      var index = i;
      plugin.AddCommandListener($"css_{i}", (p, _) => AcceptInput(p, index));
    }
  }

  private HookResult AcceptInput(CCSPlayerController? player, int index) {
    if (player == null) return HookResult.Continue;
    var activeMenu = GetActiveMenu(player.SteamID);

    var wrapper = new PlayerWrapper(player);
    Task.Run(() => activeMenu?.AcceptInput(wrapper, index));
    return HookResult.Continue;
  }

  private class DigitCommand(IMenuManager manager, int index) : ICommand {
    public string Name => $"css_{index}";

    public async Task<CommandResult> Execute(PlayerWrapper? executor,
      CommandInfoWrapper info) {
      if (executor == null) return CommandResult.PLAYER_ONLY;
      await manager.AcceptInput(executor, index);
      return CommandResult.SUCCESS;
    }
  }
}