using GangsAPI.Data;
using GangsAPI.Data.Command;
using GangsAPI.Services.Commands;

namespace Mock;

public class MockCommandManager : ICommandManager {
  private readonly Dictionary<string, ICommand> commands = new();

  public virtual bool RegisterCommand(ICommand command) {
    return commands.TryAdd(command.Name, command);
  }

  public bool UnregisterCommand(ICommand command) {
    return commands.Remove(command.Name);
  }

  public async Task<CommandResult> ProcessCommand(PlayerWrapper? executor,
    params string[] args) {
    if (args.Length == 0) return CommandResult.FAILURE;
    if (!commands.TryGetValue(args[0], out var command))
      return CommandResult.UNKNOWN_COMMAND;

    if (!command.CanExecute(executor)) return CommandResult.NO_PERMISSION;

    var result = CommandResult.FAILURE;
    await Task.Run(async () => {
      result = await command.Execute(executor,
        new CommandInfoWrapper(executor, args: args));
    });

    return result;
  }
}