using GangsAPI.Data.Stat;
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

  /// <summary>
  ///   The last time the player was seen by the plugin.
  /// </summary>
  DateTime? LastSeen {
    get {
      var stat = GetStat("gang_native_lastseen") as IStat<DateTime>;
      return stat?.Value;
    }

    set {
      if (GetStat("gang_native_lastseen") is IStat<DateTime?> stat)
        stat.Value = value;
    }
  }

  ISet<IStat> Stats { get; }
  ISet<IStat> Perks { get; }

  int? Balance {
    get {
      var stat = GetStat("gang_native_balance") as IStat<int>;
      return stat?.Value;
    }

    set {
      ArgumentNullException.ThrowIfNull(value);
      if (GetStat("gang_native_balance") is IStat<int> stat)
        stat.Value = value.Value;
    }
  }

  IStat? GetStat(string statId) {
    return Stats.FirstOrDefault(stat => stat.StatId == statId);
  }
}