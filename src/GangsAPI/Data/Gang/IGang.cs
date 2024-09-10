namespace GangsAPI.Data.Gang;

/// <summary>
///   Represents an instance of a gang.
///
///   Schema:
///     GangID INT PRIMARY KEY
///     Name STRING
/// </summary>
public interface IGang : IEqualityComparer<IGang>, IEquatable<IGang>,
  ICloneable {
  /// <summary>
  ///   The unique identifier of the gang.
  ///   All gangs have a unique identifier, and
  ///   thus equality of gangs is determined by this value.
  /// </summary>
  int GangId { get; }

  /// <summary>
  ///   The name of the gang.
  ///   Gang names are not necessarily unique.
  ///   (Up to implementation)
  /// </summary>
  string Name { get; set; }

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