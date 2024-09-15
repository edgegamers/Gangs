using GangsAPI.Data;
using GangsAPI.Data.Command;
using GangsAPI.Exceptions;
using GangsAPI.Services.Commands;

namespace Commands.Gang;

public class GangPromoteCommand : ICommand {
  public string Name => "promote";

  public Task<CommandResult> Execute(PlayerWrapper? executor,
    CommandInfoWrapper info) {
    throw new GangException("Not implemented");
  }
}