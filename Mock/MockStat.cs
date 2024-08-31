using GangsAPI.Data.Stat;

namespace GangsImpl.Memory;

public class MockStat(string statId, string name, string? desc) : IStat {
  public string StatId { get; } = statId;
  public string Name { get; } = name;
  public string? Description { get; } = desc;
}

public class MockStat<KType, VType>(string statId, string name, string? desc,
  KType key, VType value) : MockStat(statId, name, desc) {
  public MockStat(IStat b, KType key, VType value) : this(b.StatId, b.Name,
    b.Description, key, value) { }

  public KType Key { get; init; } = key;
  public VType Value { get; set; } = value;
}

public class MockPlayerStat<VType>(string statId, string name, string? desc,
  VType value) : MockStat(statId, name, desc), IPlayerStat<VType> {
  public MockPlayerStat(IStat b, VType value) : this(b.StatId, b.Name,
    b.Description, value) { }

  public ulong Key { get; init; }
  public VType Value { get; set; } = value;
}

public class MockGangStat<T>(string statId, string name, string? desc, T value)
  : MockStat(statId, name, desc), IGangStat<T> {
  public MockGangStat(IStat b, T value) : this(b.StatId, b.Name, b.Description,
    value) { }

  public int Key { get; init; }
  public T Value { get; set; } = value;
}