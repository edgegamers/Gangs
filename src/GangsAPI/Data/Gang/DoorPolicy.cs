namespace GangsAPI.Data.Gang;

public enum DoorPolicy {
  /// <summary>
  /// Anyone can join
  /// </summary>
  OPEN,

  /// <summary>
  /// Must be invited by a member
  /// </summary>
  INVITE_ONLY,

  /// <summary>
  /// May request to join (or be invited)
  /// </summary>
  REQUEST_ONLY
}