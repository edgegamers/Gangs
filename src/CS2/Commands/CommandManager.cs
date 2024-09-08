using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using GangsAPI;
using GangsAPI.Data;
using GangsAPI.Data.Command;
using GangsAPI.Services.Commands;
using GangsAPI.Services.Gang;
using Microsoft.Extensions.Localization;
using Mock;

namespace Commands;

public class CommandManager(IGangManager gangMgr, IStringLocalizer locale)
  : MockCommandManager(locale), IPluginBehavior {
  private BasePlugin? plugin;

  public void Start(BasePlugin? basePlugin, bool hotReload) {
    plugin = basePlugin;

    RegisterCommand(new GangCommand(gangMgr, Locale));
  }

  public override bool RegisterCommand(ICommand command) {
    var registration = base.RegisterCommand(command);
    if (registration == false) return false;
    foreach (var alias in command.Aliases)
      plugin?.AddCommand(alias, command.Description ?? string.Empty,
        processInternal);
    return true;
  }

  private void
    processInternal(CCSPlayerController? executor, CommandInfo info) {
    var wrapper     = executor == null ? null : new PlayerWrapper(executor);
    var wrappedInfo = new CommandInfoWrapper(info);
    Server.NextFrameAsync(async () => {
      var result = await ProcessCommand(wrapper, wrappedInfo);
      if (result == CommandResult.PLAYER_ONLY)
        executor?.PrintToChat(Locale.Get(MSG.GENERIC_PLAYER_ONLY));
    });
  }
}