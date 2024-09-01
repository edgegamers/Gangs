using GangsAPI.Data;
using GangsAPI.Data.Command;

namespace GangsAPI.Services.Commands;

public interface ICommand : IPluginBehavior {
  string Name { get; }
  string? Description { get; }
  string[] RequiredFlags => [];
  string[] RequiredGroups => [];

  bool CanExecute(PlayerWrapper? executor) {
    if (executor == null) return true;
    if (RequiredFlags.Any(flag => !executor.HasFlags(flag))) return false;
    if (RequiredGroups.Length == 0) return true;
    return executor.Data != null
      && RequiredGroups.All(group => executor.Data.Groups.Contains(group));
  }

  CommandResult Execute(PlayerWrapper? executor, CommandInfoWrapper info);
}