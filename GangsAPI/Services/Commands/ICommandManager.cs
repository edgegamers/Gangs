using GangsAPI.Data;
using GangsAPI.Data.Command;

namespace GangsAPI.Services.Commands;

/// <summary>
///   An interface that allows for registering and processing commands.
/// </summary>
public interface ICommandManager : IPluginBehavior {
  /// <summary>
  ///   Registers a command with the manager.
  /// </summary>
  /// <param name="command">True if the command was successfully registered.</param>
  bool RegisterCommand(ICommand command);

  /// <summary>
  ///  Unregisters a command from the manager.
  /// </summary>
  /// <param name="command">True if the command was successfully unregistered.</param>
  bool UnregisterCommand(ICommand command);

  /// <summary>
  ///   Attempts to process a command.
  /// </summary>
  /// <param name="executor"></param>
  /// <param name="args"></param>
  /// <returns>True if the command finished processing successfully.</returns>
  Task<CommandResult> ProcessCommand(PlayerWrapper? executor,
    params string[] args);
}