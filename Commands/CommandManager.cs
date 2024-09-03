using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using GangsAPI;
using GangsAPI.Data;
using GangsAPI.Data.Command;
using GangsAPI.Services;
using GangsAPI.Services.Commands;
using Mock;

namespace Commands;

public class CommandManager(IGangManager gangMgr)
  : MockCommandManager, IPluginBehavior {
  private BasePlugin? plugin;

  public void Start(BasePlugin? basePlugin, bool hotReload) {
    plugin = basePlugin;

    RegisterCommand(new GangCommand(gangMgr));
  }

  public override bool RegisterCommand(ICommand command) {
    base.RegisterCommand(command);
    plugin?.AddCommand(command.Name, command.Description ?? string.Empty,
      (player, info) => {
        var wrapper = player == null ? null : new PlayerWrapper(player);
        var args    = info.GetCommandString.Split(" ");
        Server.NextFrameAsync(async () => {
          await ProcessCommand(wrapper, args);
        });
      });
    return true;
  }
}