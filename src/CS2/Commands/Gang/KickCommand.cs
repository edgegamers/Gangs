using GangsAPI.Data;
using GangsAPI.Data.Command;
using GangsAPI.Services.Commands;

namespace Commands.Gang;

public class KickCommand : ICommand {
  public string Name => "kick";

  public Task<CommandResult> Execute(PlayerWrapper? executor,
    CommandInfoWrapper info) {
    throw new NotImplementedException();
  }
}