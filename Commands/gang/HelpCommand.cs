using CounterStrikeSharp.API.Core.Commands;
using GangsAPI.Data;
using GangsAPI.Data.Command;
using GangsAPI.Services.Commands;

namespace Commands.gang;

public class HelpCommand : ICommand {
  public string Name => "help";
  public string? Description => "Displays help for gangs";

  public CommandResult
    Execute(PlayerWrapper? executor, CommandInfoWrapper info) {
    executor?.PrintToChat("Gang commands:");
    return CommandResult.SUCCESS;
  }
}