using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using GangsAPI;
using GangsAPI.Data;
using GangsAPI.Data.Command;
using GangsAPI.Exceptions;
using GangsAPI.Services.Commands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Mock;
using Serilog;

namespace Commands;

public class CommandManager(IServiceProvider provider)
  : MockCommandManager(provider.GetRequiredService<IStringLocalizer>()),
    IPluginBehavior {
  private bool hotReload;
  private BasePlugin? plugin;

  public void Start(BasePlugin? basePlugin, bool baseReload) {
    plugin    = basePlugin;
    hotReload = baseReload;

    if (basePlugin != null) Locale = basePlugin.Localizer;

    RegisterCommand(new GangCommand(provider));
    RegisterCommand(new BalanceCommand(provider));
    RegisterCommand(new StatsCommand(provider));
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
    Task.Run(async () => {
      try { return await ProcessCommand(wrapper, wrappedInfo); } catch (
        GangException e) {
        var msg = e.Message;
        await Server.NextFrameAsync(() => {
          provider.GetRequiredService<ILogger>()
           .Error(e,
              $"Encountered an error when processing command: \"{wrappedInfo.GetCommandString}\" by {wrapper?.Steam}");
        });
        wrappedInfo.ReplySync(string.IsNullOrEmpty(msg) ?
          Locale.Get(MSG.GENERIC_ERROR) :
          Locale.Get(MSG.GENERIC_ERROR_INFO, msg));
        return CommandResult.ERROR;
      }
    });
  }
}