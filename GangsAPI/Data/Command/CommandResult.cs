namespace GangsAPI.Data.Command;

public enum CommandResult {
  SUCCESS,
  FAILURE,
  UNKNOWN_COMMAND,
  INVALID_ARGS,
  NO_PERMISSION,
  PLAYER_ONLY
}