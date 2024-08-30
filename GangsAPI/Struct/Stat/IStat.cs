namespace GangsAPI.Struct;

/// <summary>
/// Represents a numerical statistic.
/// </summary>
public interface IStat {
  /// <summary>
  /// The unique identifier of the statistic.
  /// </summary>
  string StatId { get; }

  /// <summary>
  /// The name of the statistic.
  /// </summary>
  string Name { get; }

  /// <summary>
  /// A description of the statistic.
  /// </summary>
  string? Description { get; }
}

public interface IStat<K, T> : IStat {
  /// <summary>
  /// A key identifying an instance of the statistic.
  /// </summary>
  K Key { get; init; }

  /// <summary>
  /// The value of the statistic.
  /// </summary>
  T Value { get; set; }
}