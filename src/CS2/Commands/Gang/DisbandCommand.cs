using GangsAPI.Data;
using GangsAPI.Data.Command;
using GangsAPI.Services.Commands;

namespace Commands.Gang;

public class DisbandCommand : ICommand {
  public string Name => "disband";
  public Task<CommandResult> Execute(PlayerWrapper? executor, CommandInfoWrapper info) { throw new NotImplementedException(); }
}