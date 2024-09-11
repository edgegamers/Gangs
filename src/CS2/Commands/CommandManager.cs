using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using GangsAPI;
using GangsAPI.Data;
using GangsAPI.Data.Command;
using GangsAPI.Services.Commands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Mock;

namespace Commands;

public class CommandManager(IServiceProvider provider)
  : MockCommandManager(provider.GetRequiredService<IStringLocalizer>()),
    IPluginBehavior {
  private bool hotReload;
  private BasePlugin? plugin;

  public void Start(BasePlugin? basePlugin, bool hotReload) {
    plugin         = basePlugin;
    this.hotReload = hotReload;

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
    Task.Run(async () => await ProcessCommand(wrapper, wrappedInfo));
  }
}