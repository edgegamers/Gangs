using GangsAPI.Data;
using GangsAPI.Services.Commands;

namespace Mock;

public class MockCommandManager : ICommandManager {
  private readonly Dictionary<string, ICommand> commands = new();

  public bool RegisterCommand(ICommand command) {
    return commands.TryAdd(command.Name, command);
  }

  public bool UnregisterCommand(ICommand command) {
    return commands.Remove(command.Name);
  }

  public bool ProcessCommand(PlayerWrapper? executor, string[] args) {
    if (args.Length == 0) return false;
    if (!commands.TryGetValue(args[0], out var command)) return false;

    if (executor == null)
      return command.Execute(executor,
        new CommandInfoWrapper(executor?.Player, args));

    if (command.RequiredFlags.Any(flag => !executor.HasFlags(flag)))
      return false;

    foreach (var group in command.RequiredGroups) {
      if (executor.Data == null) return false;
      if (!executor.Data.Groups.Contains(group)) return false;
    }

    return command.Execute(executor,
      new CommandInfoWrapper(executor?.Player, args));
  }
}