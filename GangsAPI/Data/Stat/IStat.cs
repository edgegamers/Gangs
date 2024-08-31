namespace GangsAPI.Data.Stat;

/// <summary>
///   Represents a numerical statistic.
/// </summary>
public interface IStat : IEqualityComparer<IStat>, IEquatable<IStat> {
  /// <summary>
  ///   The unique identifier of the statistic.
  /// </summary>
  string StatId { get; }

  /// <summary>
  ///   The name of the statistic.
  /// </summary>
  string Name { get; }

  /// <summary>
  ///   A description of the statistic.
  /// </summary>
  string? Description { get; }

  bool IEqualityComparer<IStat>.Equals(IStat? x, IStat? y) {
    if (x is null || y is null) return false;
    return x.StatId == y.StatId;
  }

  int IEqualityComparer<IStat>.GetHashCode(IStat obj) {
    return obj.StatId.GetHashCode();
  }

  bool IEquatable<IStat>.Equals(IStat? other) {
    if (other is null) return false;
    return StatId == other.StatId;
  }
}

public interface IStat<T> : IStat {
  /// <summary>
  ///   The value of the statistic.
  /// </summary>
  T Value { get; set; }
}