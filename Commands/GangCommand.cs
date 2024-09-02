using Commands.gang;
using GangsAPI.Data;
using GangsAPI.Data.Command;
using GangsAPI.Services.Commands;
using Mock;

namespace Commands;

public class GangCommand : ICommand {
  public string Name => "css_gang";
  public string? Description => "Master command for gangs";

  private Dictionary<string, ICommand> sub = new() {
    // ["create"] = new CreateGangCommand(),
    // ["delete"] = new DeleteGangCommand(),
    // ["invite"] = new InviteGangCommand(),
    // ["kick"] = new KickGangCommand(),
    // ["leave"] = new LeaveGangCommand(),
    // ["list"] = new ListGangCommand(),
    // ["promote"] = new PromoteGangCommand(),
    // ["demote"] = new DemoteGangCommand(),
    // ["info"] = new InfoGangCommand()
    ["help"] = new HelpCommand()
  };

  public async Task<CommandResult> Execute(PlayerWrapper? executor,
    CommandInfoWrapper info) {
    if (info.ArgCount == 0 || info[0] != Name) {
      if (info.ArgCount == 0)
        throw new InvalidOperationException(
          "Attempted to execute GangCommand with no arguments");
      throw new InvalidOperationException(
        $"Attempted to execute GangCommand with invalid name: {info[0]}");
    }

    if (info.ArgCount == 1) return CommandResult.INVALID_ARGS;

    if (!sub.TryGetValue(info[1], out var command)) {
      // print usage
      return CommandResult.UNKNOWN_COMMAND;
    }

    var newInfo = new CommandInfoWrapper(info, 1);
    return await command.Execute(executor, newInfo);
  }
}