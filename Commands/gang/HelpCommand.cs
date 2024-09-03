using GangsAPI.Data;
using GangsAPI.Data.Command;
using GangsAPI.Services.Commands;

namespace Commands.gang;

public class HelpCommand : ICommand {
  public string Name => "help";
  public string? Description => "Displays help for gangs";

  public Task<CommandResult> Execute(PlayerWrapper? executor,
    CommandInfoWrapper info) {
    info.ReplySync("create [name] - Creates a new gang");
    return Task.FromResult(CommandResult.SUCCESS);
  }
}