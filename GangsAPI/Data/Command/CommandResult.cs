namespace GangsAPI.Data.Command;

public enum CommandResult {
  /// <summary>
  ///   The command completed successfully
  /// </summary>
  SUCCESS,

  /// <summary>
  ///   The command encountered an error or other
  ///   scenario that prevented success
  /// </summary>
  FAILURE,

  /// <summary>
  ///   The command was improperly formatted
  /// </summary>
  UNKNOWN_COMMAND,

  /// <summary>
  ///   The command has improper arguments, or
  ///   no sufficient arguments
  /// </summary>
  INVALID_ARGS,

  /// <summary>
  ///   The executor of the command did not have
  ///   the required permissions
  /// </summary>
  NO_PERMISSION,


  /// <summary>
  ///   This command can only be executed by a player
  ///   (i.e. not from the console)
  /// </summary>
  PLAYER_ONLY
}