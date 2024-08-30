using System.Collections;
using GangsAPI;
using GangsImpl.Memory;

namespace GangsTest;

public class StatManagerData : IEnumerable<object[]> {
  private readonly IBehavior[] behaviors = [new MemoryStatManager()];

  public StatManagerData() {
    foreach (var behavior in behaviors) behavior.Start();
  }

  public IEnumerator<object[]> GetEnumerator() {
    return behaviors.Select(behavior => (object[]) [behavior]).GetEnumerator();
  }

  IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
}