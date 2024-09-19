using GangsAPI.Data;
using GangsAPI.Data.Command;

namespace GangsAPI.Services.Commands;

public interface ICommand : IPluginBehavior {
  string Name { get; }

  string? Description => null;
  string[] Usage => [];
  string[] RequiredFlags => [];
  string[] RequiredGroups => [];
  string[] Aliases => [Name];

  bool CanExecute(PlayerWrapper? executor) {
    if (executor == null) return true;
    if (RequiredFlags.Any(flag => !executor.HasFlags(flag))) return false;
    if (RequiredGroups.Length == 0) return true;
    return executor.Data != null
      && RequiredGroups.All(group => executor.Data.Groups.Contains(group));
  }

  Task<CommandResult> Execute(PlayerWrapper? executor, CommandInfoWrapper info);
}