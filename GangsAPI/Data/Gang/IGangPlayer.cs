using GangsAPI.Permissions;

namespace GangsAPI.Data.Gang;

/// <summary>
///   A gang player is a player tracked by the gangs plugin.
///   They are not necessarily a member of a gang.
/// </summary>
public interface IGangPlayer {
  /// <summary>
  ///   The SteamID64 of the player.
  /// </summary>
  ulong Steam { get; }

  /// <summary>
  ///   The cached name of the player.
  /// </summary>
  string? Name { get; }

  /// <summary>
  ///   The id of the gang that the player is a member of.
  /// </summary>
  int? GangId { get; }

  /// <summary>
  ///   The rank the player has in the gang (if in one).
  /// </summary>
  IGangRank? Rank { get; }
}