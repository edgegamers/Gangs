using System.Collections;
using Mock;

namespace GangsTest.StatTests.InstanceManageTests;

public class InstanceManageData : IEnumerable<object[]> {
  private readonly object[] behaviors = [new MockInstanceStatManager()];

  public IEnumerator<object[]> GetEnumerator() {
    return behaviors.Select(behavior => new[] { behavior }).GetEnumerator();
  }

  IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
}