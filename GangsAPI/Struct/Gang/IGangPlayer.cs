using GangsAPI.Permissions;

namespace GangsAPI.Struct;

/// <summary>
/// A gang player is a player tracked by the gangs plugin.
/// They are not necessarily a member of a gang.
/// </summary>
public interface IGangPlayer {
  /// <summary>
  /// The SteamID64 of the player.
  /// </summary>
  ulong Steam { get; }

  /// <summary>
  /// The cached name of the player.
  /// </summary>
  string? Name { get; }

  /// <summary>
  /// The id of the gang that the player is a member of.
  /// </summary>
  int? GangId { get; }

  /// <summary>
  /// The rank the player has in the gang (if in one).
  /// </summary>
  IGangRank? Rank { get; }

  /// <summary>
  /// The last time the player was seen by the plugin.
  /// </summary>
  DateTime? LastSeen {
    get {
      var stat = GetStat("last_seen") as IStat<ulong, DateTime>;
      return stat?.Value;
    }

    set {
      if (GetStat("last_seen") is IStat<ulong, DateTime?> stat) stat.Value = value;
    }
  }

  ISet<IStat> Stats { get; }

  int? Balance {
    get {
      var stat = GetStat("balance") as IStat<ulong, int>;
      return stat?.Value;
    }

    set {
      ArgumentNullException.ThrowIfNull(value);
      if (GetStat("balance") is IStat<ulong, int> stat) stat.Value = value.Value;
    }
  }

  IStat? GetStat(string statId) {
    return Stats.FirstOrDefault(stat => stat.StatId == statId);
  }
}