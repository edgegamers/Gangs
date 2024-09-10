using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using GangsAPI;
using GangsAPI.Data;
using GangsAPI.Data.Command;
using GangsAPI.Services;
using GangsAPI.Services.Commands;
using GangsAPI.Services.Gang;
using GangsAPI.Services.Menu;
using GangsAPI.Services.Player;
using Microsoft.Extensions.Localization;
using Mock;

namespace Commands;

public class CommandManager(IGangManager gangMgr,
  IPlayerStatManager playerStatMgr, IPlayerManager playerMgr,
  IMenuManager menuMgr, IRankManager rankMgr, IGangStatManager gangStatMgr,
  IStringLocalizer testLocale)
  : MockCommandManager(testLocale), IPluginBehavior {
  private bool hotReload;
  private BasePlugin? plugin;

  public void Start(BasePlugin? basePlugin, bool hotReload) {
    plugin         = basePlugin;
    this.hotReload = hotReload;

    if (basePlugin != null) Locale = basePlugin.Localizer;

    RegisterCommand(new GangCommand(gangMgr, playerMgr, menuMgr, rankMgr,
      gangStatMgr, Locale));
    RegisterCommand(new BalanceCommand(playerStatMgr, Locale));
  }

  public override bool RegisterCommand(ICommand command) {
    command.Start(plugin, hotReload);
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
        wrappedInfo.ReplySync(Locale.Get(MSG.GENERIC_PLAYER_ONLY));
    });
  }
}