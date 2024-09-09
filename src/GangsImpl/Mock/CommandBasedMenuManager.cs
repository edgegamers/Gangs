using CounterStrikeSharp.API.Core;
using GangsAPI.Data;
using GangsAPI.Data.Command;
using GangsAPI.Services.Commands;
using GangsAPI.Services.Menu;

namespace Mock;

public class CommandBasedMenuManager(ICommandManager cmd) : MockMenuManager {
  private class DigitCommand(IMenuManager manager, int index) : ICommand {
    public string Name => $"css_{index}";

    public async Task<CommandResult> Execute(PlayerWrapper? executor,
      CommandInfoWrapper info) {
      if (executor == null) return CommandResult.PLAYER_ONLY;
      await manager.AcceptInput(executor, index);
      return CommandResult.SUCCESS;
    }
  }

  public override void Start(BasePlugin? plugin, bool hotReload) {
    for (var i = 0; i < 10; i++) cmd.RegisterCommand(new DigitCommand(this, i));
  }
}