using GangsAPI.Data;
using GangsAPI.Data.Command;
using GangsAPI.Services.Commands;

namespace Commands.Gang;

public class ManagePermsCommand : ICommand {
  public string Name => "manageperms";

  public Task<CommandResult> Execute(PlayerWrapper? executor,
    CommandInfoWrapper info) {
    throw new NotImplementedException();
  }
}