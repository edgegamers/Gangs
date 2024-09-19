namespace GangsAPI.Data.Stat;

/// <summary>
///   Represents a statistic.
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

  Type ValueType { get; }
}

/// <summary>
///   Represents an instance of a statistic.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IStat<T> : IStat, IEquatable<IStat<T>> {
  /// <summary>
  ///   The value of the statistic.
  /// </summary>
  T Value { get; set; }

  Type IStat.ValueType => typeof(T);
}