namespace GangsAPI.Data.Stat;

/// <summary>
///   Represents a numerical statistic.
/// </summary>
public interface IStat : IEquatable<IStat> {
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
}

public interface IStat<T> : IStat, IEquatable<IStat<T>> {
  /// <summary>
  ///   The value of the statistic.
  /// </summary>
  T Value { get; set; }

  IStat<T> Clone();
}