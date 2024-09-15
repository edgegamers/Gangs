namespace GangsAPI.Data.Gang;

public enum DoorPolicy {
  /// <summary>
  /// May request to join (or be invited)
  /// </summary>
  REQUEST_ONLY,

  /// <summary>
  /// Must be invited by a member (cannot request to join)
  /// </summary>
  INVITE_ONLY,

  /// <summary>
  /// Anyone can join
  /// </summary>
  OPEN,
}