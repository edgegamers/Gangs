using GangsAPI.Data.Gang;

namespace Mock;

public class MockGang(int id, string name) : IGang {
  public int GangId { get; protected init; } = id;
  public string Name { get; set; } = name;

  public object Clone() {
    var clone = new MockGang(GangId, Name);
    return clone;
  }
}