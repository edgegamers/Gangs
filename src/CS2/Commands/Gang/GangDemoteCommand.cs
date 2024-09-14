using GangsAPI.Data;
using GangsAPI.Data.Command;
using GangsAPI.Services.Commands;

namespace Commands.Gang;

public class GangDemoteCommand : ICommand {
  public string Name => "demote";

  public Task<CommandResult> Execute(PlayerWrapper? executor,
    CommandInfoWrapper info) {
    throw new NotImplementedException();
  }
}