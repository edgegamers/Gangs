using GangsAPI.Data.Stat;

namespace Stats;

public abstract class BaseStat : IStat {
  public bool Equals(IStat? other) {
    return other is not null && StatId == other.StatId;
  }

  public abstract string StatId { get; }
  public abstract string Name { get; }
  public abstract string? Description { get; }
}

public abstract class BaseStat<V> : IStat<V?> {
  public bool Equals(IStat? other) {
    return other is not null && StatId == other.StatId;
  }

  public abstract string StatId { get; }
  public abstract string Name { get; }
  public abstract string? Description { get; }

  public bool Equals(IStat<V?>? other) {
    return other is not null && StatId == other.StatId;
  }

  public V? Value { get; set; }
  public abstract IStat<V?> Clone();
}