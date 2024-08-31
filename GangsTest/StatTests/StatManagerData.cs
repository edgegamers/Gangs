using System.Collections;
using GangsAPI;
using GangsImpl.Memory;
using GangsImpl.SQL;

namespace GangsTest.StatTests;

public class StatManagerData : IEnumerable<object[]> {
  private readonly IBehavior[] behaviors = [
    new MockStatManager(),
    new SQLStatManager("Server=localhost;User=root;Database=gang", "gang_unit_test")
  ];

  public StatManagerData() {
    foreach (var behavior in behaviors) behavior.Start();
  }

  public IEnumerator<object[]> GetEnumerator() {
    return behaviors.Select(behavior => (object[]) [behavior]).GetEnumerator();
  }

  IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
}