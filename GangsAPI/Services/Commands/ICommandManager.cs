using GangsAPI.Data;

namespace GangsAPI.Services.Commands;

/// <summary>
///   An interface that allows for registering and processing commands.
/// </summary>
public interface ICommandManager {
  /// <summary>
  ///   Registers a command with the manager.
  /// </summary>
  /// <param name="command"></param>
  void RegisterCommand(ICommand command);

  /// <summary>
  ///   Attempts to process a command.
  /// </summary>
  /// <param name="executor"></param>
  /// <param name="args"></param>
  /// <returns>True if the command finished processing successfully.</returns>
  bool ProcessCommand(PlayerWrapper? executor, string[] args);
}