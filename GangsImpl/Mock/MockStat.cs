using GangsAPI.Data.Stat;

namespace Mock;

public class MockStat(string statId, string name, string? desc = null)
  : IStat, IEquatable<MockStat> {
  public string StatId { get; } = statId;
  public string Name { get; } = name;
  public string? Description { get; } = desc;

  public bool Equals(MockStat? other) {
    return other is not null && ((IStat)this).Equals(other);
  }

  public override bool Equals(object? obj) { return Equals(obj as MockStat); }

  public override int GetHashCode() { return HashCode.Combine(StatId); }
}

public class MockStat<VType>(string statId, string name, string? desc,
  VType value) : MockStat(statId, name, desc), IEquatable<MockStat<VType>> {
  public MockStat(IStat b, VType value) : this(b.StatId, b.Name, b.Description,
    value) { }

  public VType Value { get; set; } = value;

  public bool Equals(MockStat<VType>? other) {
    return other is not null && base.Equals(other);
  }

  public override bool Equals(object? obj) {
    return Equals(obj as MockStat<VType>);
  }

  public override int GetHashCode() {
    return HashCode.Combine(base.GetHashCode());
  }
}