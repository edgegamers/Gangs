using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using GangsAPI;
using GangsAPI.Data;
using GangsAPI.Data.Command;
using GangsAPI.Services;
using GangsAPI.Services.Commands;
using GangsAPI.Services.Gang;
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
    var result = base.RegisterCommand(command);
    if (result == false) return false;
    foreach (var alias in command.Aliases)
      plugin?.AddCommand(alias, command.Description ?? string.Empty,
        (player, info) => {
          var wrapper     = player == null ? null : new PlayerWrapper(player);
          var wrappedInfo = new CommandInfoWrapper(info);
          Server.NextFrameAsync(async () => {
            await ProcessCommand(wrapper, wrappedInfo);
          });
        });

    return true;
  }
}