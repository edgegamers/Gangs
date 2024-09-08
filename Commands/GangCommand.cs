using Commands.Gang;
using CounterStrikeSharp.API.Modules.Utils;
using GangsAPI;
using GangsAPI.Data;
using GangsAPI.Data.Command;
using GangsAPI.Services.Commands;
using GangsAPI.Services.Gang;
using Microsoft.Extensions.Localization;
using static GangsAPI.MSG;

namespace Commands;

public class GangCommand(IGangManager gangMgr, IStringLocalizer locale)
  : ICommand {
  public static readonly string PREFIX =
    $" {ChatColors.Red}GANGS {ChatColors.DarkRed}> {ChatColors.Grey}";

  private readonly Dictionary<string, ICommand> sub = new() {
    // ["delete"] = new DeleteGangCommand(),
    // ["invite"] = new InviteGangCommand(),
    // ["kick"] = new KickGangCommand(),
    // ["leave"] = new LeaveGangCommand(),ggG
    // ["list"] = new ListGangCommand(),
    // ["promote"] = new PromoteGangCommand(),
    // ["demote"] = new DemoteGangCommand(),
    // ["info"] = new InfoGangCommand()
    ["create"] = new CreateCommand(gangMgr), ["help"] = new HelpCommand()
  };

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

    if (executor?.Player != null) {
      info.ReplySync(locale.Get(SOONTM));
      return CommandResult.SUCCESS;
    }

    if (info.ArgCount == 1) {
      if (executor == null) return CommandResult.PLAYER_ONLY;
      var gang = await gangMgr.GetGang(executor.Steam);

      if (gang == null) {
        info.ReplySync(locale[NOT_IN_GANG.Key()]);
        return CommandResult.SUCCESS;
      }

      // Open gang menu
      return CommandResult.SUCCESS;
    }

    if (!sub.TryGetValue(info[1], out var command)) {
      return CommandResult.UNKNOWN_COMMAND;
    }

    var newInfo = new CommandInfoWrapper(info, 1);
    return await command.Execute(executor, newInfo);
  }
}