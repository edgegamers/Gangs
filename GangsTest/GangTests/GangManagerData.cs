using System.Collections;
using GangsAPI;
using Mock;

namespace GangsTest.GangTests;

public class GangManagerData : IEnumerable<object[]> {
  private readonly IBehavior[] behaviors = [new MockGangManager()];

  public GangManagerData() {
    foreach (var behavior in behaviors) behavior.Start();
  }

  public IEnumerator<object[]> GetEnumerator() {
    return behaviors.Select(behavior => (object[]) [behavior]).GetEnumerator();
  }

  IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
}