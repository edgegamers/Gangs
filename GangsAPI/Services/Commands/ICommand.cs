using GangsAPI.Data;

namespace GangsAPI.Services.Commands;

public interface ICommand {
  string Name { get; }
  string? Description { get; }
  string[] RequiredFlags { get; }
  string[] RequiredGroups { get; }
  bool Execute(PlayerWrapper? executor, CommandInfoWrapper info);
}