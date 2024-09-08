using CounterStrikeSharp.API.Modules.Commands;
using GangsAPI;
using GangsAPI.Data;
using GangsAPI.Data.Command;
using GangsAPI.Services.Commands;
using Microsoft.Extensions.Localization;

namespace Mock;

public class MockCommandManager(IStringLocalizer locale) : ICommandManager {
  protected readonly Dictionary<string, ICommand> Commands = new();
  protected IStringLocalizer Locale = locale;

  public virtual bool RegisterCommand(ICommand command) {
    return command.Aliases.All(alias => Commands.TryAdd(alias, command));
  }

  public bool UnregisterCommand(ICommand command) {
    return command.Aliases.All(alias => Commands.Remove(alias));
  }

  public async Task<CommandResult> ProcessCommand(PlayerWrapper? executor,
    params string[] args) {
    var info = new CommandInfoWrapper(executor, 0, args);
    return await ProcessCommand(executor, info);
  }

  public async Task<CommandResult> ProcessCommand(PlayerWrapper? executor,
    CommandInfo sourceInfo) {
    var info = new CommandInfoWrapper(sourceInfo);
    return await ProcessCommand(executor, info);
  }

  public virtual async Task<CommandResult> ProcessCommand(
    PlayerWrapper? executor, CommandInfoWrapper sourceInfo) {
    if (sourceInfo.ArgCount == 0) return CommandResult.ERROR;
    var result = CommandResult.ERROR;

    if (!Commands.TryGetValue(sourceInfo[0], out var command)) {
      sourceInfo.ReplySync("Unknown command: " + sourceInfo[0]);
      return CommandResult.UNKNOWN_COMMAND;
    }

    if (!command.CanExecute(executor)) return CommandResult.NO_PERMISSION;

    await Task.Run(async () => {
      result = await command.Execute(executor, sourceInfo);
    });

    switch (result) {
      case CommandResult.PLAYER_ONLY:
        sourceInfo.ReplySync(Locale.Get(MSG.GENERIC_PLAYER_ONLY));
        break;
      case CommandResult.PRINT_USAGE: {
        foreach (var use in command.Usage)
          sourceInfo.ReplySync(Locale.Get(MSG.COMMAND_USAGE,
            $"{sourceInfo[0]} {use}"));
        break;
      }
    }

    return result;
  }
}