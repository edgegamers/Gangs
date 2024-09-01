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
  };

  public CommandResult
    Execute(PlayerWrapper? executor, CommandInfoWrapper info) {
    if (info.ArgCount == 0) return CommandResult.FAILURE;
    if (!sub.TryGetValue(info[0], out var command)) {
      // print usage
      return CommandResult.INVALID_ARGS;
    }

    return CommandResult.SUCCESS;
  }
}