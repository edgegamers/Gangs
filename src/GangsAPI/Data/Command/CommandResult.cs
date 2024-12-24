namespace GangsAPI.Data.Command;

public enum CommandResult {
  /// <summary>
  ///   The command ran successfully
  /// </summary>
  SUCCESS,

  /// <summary>
  ///   The command ran into an error
  /// </summary>
  ERROR,

  /// <summary>
  ///   The command was improperly formatted or otherwise
  ///   was not recognized by the command handler
  /// </summary>
  UNKNOWN_COMMAND,

  /// <summary>
  ///   The command has an invalid number of arguments
  /// </summary>
  INVALID_ARGS,

  /// <summary>
  ///   Similar to <see cref="INVALID_ARGS" />, but prompts
  ///   the Manager to print the usage of the command
  /// </summary>
  PRINT_USAGE,

  /// <summary>
  ///   The executor of the command did not have
  ///   the required permissions. This may either
  ///   be due to a pre-check by the manager, or an
  ///   "execution" check by the command itself.
  /// </summary>
  NO_PERMISSION,

  /// <summary>
  ///   This command can only be executed by a player
  ///   (i.e. not from the console)
  /// </summary>
  PLAYER_ONLY
}