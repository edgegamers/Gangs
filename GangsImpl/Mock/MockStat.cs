using GangsAPI.Data.Stat;

namespace Mock;

public class MockStat(string statId, string name, string? desc = null) : IStat {
  public bool Equals(MockStat? other) {
    return other is not null && ((IStat)this).Equals(other);
  }

  public string StatId { get; } = statId;
  public string Name { get; } = name;
  public string? Description { get; } = desc;

  public bool Equals(IStat? other) {
    return other is not null && StatId == other.StatId;
  }
  public override bool Equals(object? obj) { return Equals(obj as MockStat); }

  public override int GetHashCode() { return HashCode.Combine(StatId); }
}

public class MockStat<TVType>(string statId, string name, string? desc,
  TVType value) : MockStat(statId, name, desc), IEquatable<MockStat<TVType>> {
  public MockStat(IStat b, TVType value) : this(b.StatId, b.Name, b.Description,
    value) { }

  public TVType Value { get; set; } = value;

  public bool Equals(MockStat<TVType>? other) {
    return other is not null && base.Equals(other);
  }

  public override bool Equals(object? obj) {
    return Equals(obj as MockStat<TVType>);
  }

  public override int GetHashCode() {
    return HashCode.Combine(base.GetHashCode());
  }
}