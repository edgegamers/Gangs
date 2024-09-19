using GangsAPI.Data.Stat;

namespace Stats.Stat;

public abstract class BaseStat : IStat {
  public bool Equals(IStat? other) {
    return other is not null && StatId == other.StatId;
  }

  public abstract string StatId { get; }
  public abstract string Name { get; }
  public abstract string? Description { get; }
  public abstract Type ValueType { get; }
}

public abstract class BaseStat<V> : BaseStat, IStat<V?> {
  public override Type ValueType => typeof(V);

  public bool Equals(IStat<V?>? other) {
    return other is not null && StatId == other.StatId;
  }

  public abstract V? Value { get; set; }
}