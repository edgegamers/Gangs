using System.Collections;
using GangsAPI.Permissions;

namespace GangsAPI.Data.Gang;

/// <summary>
///   Represents an instance of a gang.
/// </summary>
public interface IGang : IEqualityComparer<IGang>, IEquatable<IGang>,
  IEnumerable<ulong>, ICloneable {
  /// <summary>
  ///   The unique identifier of the gang.
  ///   All gangs have a unique identifier.
  /// </summary>
  int GangId { get; }

  /// <summary>
  ///   The name of the gang.
  ///   Gang names are not necessarily unique.
  ///   (Up to implementation)
  /// </summary>
  string Name { get; set; }

  /// <summary>
  ///   The members of the gang.
  ///   All gangs have at least one owner.
  /// </summary>
  IDictionary<ulong, IGangRank> Members { get; }

  /// <summary>
  ///   The ranks of the gang.
  /// </summary>
  ISet<IGangRank> Ranks { get; }

  ulong Owner
    => Members.First(m => m.Value.Perms.HasFlag(IGangRank.Permissions.OWNER))
     .Key;

  IEnumerator IEnumerable.GetEnumerator() {
    return Members.Keys.GetEnumerator();
  }

  IEnumerator<ulong> IEnumerable<ulong>.GetEnumerator() {
    return Members.Keys.GetEnumerator();
  }

  bool IEqualityComparer<IGang>.Equals(IGang? x, IGang? y) {
    if (x is null || y is null) return false;
    return x.GangId == y.GangId;
  }

  int IEqualityComparer<IGang>.GetHashCode(IGang obj) {
    return obj.GangId.GetHashCode();
  }

  bool IEquatable<IGang>.Equals(IGang? other) {
    if (other is null) return false;
    return GangId == other.GangId;
  }
}