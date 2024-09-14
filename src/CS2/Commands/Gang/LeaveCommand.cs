using GangsAPI.Data;
using GangsAPI.Data.Command;
using GangsAPI.Services.Commands;

namespace Commands.Gang;

public class LeaveCommand : ICommand {
  public string Name => "leave";
  public Task<CommandResult> Execute(PlayerWrapper? executor, CommandInfoWrapper info) { throw new NotImplementedException(); }
}