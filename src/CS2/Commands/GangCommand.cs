using Commands.Gang;
using Commands.Menus;
using CounterStrikeSharp.API.Core;
using GangsAPI;
using GangsAPI.Data;
using GangsAPI.Data.Command;
using GangsAPI.Services.Commands;
using GangsAPI.Services.Gang;
using GangsAPI.Services.Menu;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using static GangsAPI.MSG;

namespace Commands;

public class GangCommand(IServiceProvider provider) : ICommand {
  private readonly IGangManager gangs =
    provider.GetRequiredService<IGangManager>();

  private readonly IMenuManager menus =
    provider.GetRequiredService<IMenuManager>();

  private readonly Dictionary<string, ICommand> sub = new() {
    ["perks"]  = new PerksCommand(provider),
    ["invite"] = new InviteCommand(provider),
    ["create"] = new CreateCommand(provider),
    ["help"]   = new HelpCommand()
  };

  private IStringLocalizer locale =
    provider.GetRequiredService<IStringLocalizer>();

  public void Start(BasePlugin? plugin, bool hotReload) {
    if (plugin != null) locale = plugin.Localizer ?? locale;
  }

  public string Name => "css_gang";
  public string Description => "Master command for gangs";
  public string[] Aliases => ["css_gang", "css_gangs"];

  public async Task<CommandResult> Execute(PlayerWrapper? executor,
    CommandInfoWrapper info) {
    if (info.ArgCount == 0)
      throw new InvalidOperationException(
        "Attempted to execute GangCommand with no arguments");
    if (!Aliases.Contains(info[0]))
      throw new InvalidOperationException(
        $"Attempted to execute GangCommand with invalid name: {info[0]}");

    if (info.ArgCount == 1) {
      if (executor == null) return CommandResult.PLAYER_ONLY;
      var gang = await gangs.GetGang(executor.Steam);

      if (gang == null) {
        info.ReplySync(locale[COMMAND_GANG_NOTINGANG.Key()]);
        return CommandResult.SUCCESS;
      }

      // Open gang menu
      await menus.OpenMenu(executor, new GangMenu(provider, gang));
      return CommandResult.SUCCESS;
    }

    if (!sub.TryGetValue(info[1], out var command))
      return CommandResult.UNKNOWN_COMMAND;

    var newInfo =
      new CommandInfoWrapper(executor, 1, info.Args) {
        CallingContext = info.CallingContext
      };

    var result = await command.Execute(executor, newInfo);
    if (result == CommandResult.PRINT_USAGE)
      foreach (var use in command.Usage)
        info.ReplySync(
          locale.Get(COMMAND_USAGE, $"{Name} {command.Name} {use}"));

    return result;
  }
}