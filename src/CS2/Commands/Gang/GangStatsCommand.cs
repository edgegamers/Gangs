using GangsAPI.Data;
using GangsAPI.Data.Command;
using GangsAPI.Services.Commands;

namespace Commands.Gang;

public class GangStatsCommand : ICommand {
  public string Name => "stats";

  public Task<CommandResult> Execute(PlayerWrapper? executor,
    CommandInfoWrapper info) {
    throw new NotImplementedException();
  }
}