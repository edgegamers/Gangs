namespace GangsAPI.Data.Gang;

/// <summary>
///   A gang player is a player tracked by the gangs plugin.
///   They are not necessarily a member of a gang.
///
///   Schema:
///     Steam BIGINT PRIMARY KEY
///     Name STRING
///     GangId INT
///     GangRank INT
/// </summary>
public interface IGangPlayer {
  /// <summary>
  ///   The SteamID64 of the player.
  /// </summary>
  ulong Steam { get; }

  /// <summary>
  ///   The cached name of the player.
  /// </summary>
  string? Name { get; set; }

  /// <summary>
  ///   The id of the gang that the player is a member of.
  ///   Null if the player is not in a gang.
  /// 
  /// Note: Both GangId and <see cref="GangRank"/> must be null or non-null.
  /// </summary>
  int? GangId { get; set; }

  /// <summary>
  /// The rank of the player in the gang,
  /// lower numbers are higher ranks, with 0 being the owner.
  /// Null if the player is not in a gang.
  ///
  /// Note: Both <see cref="GangId"/> and GangRank must be null or non-null.
  /// </summary>
  int? GangRank { get; set; }
}