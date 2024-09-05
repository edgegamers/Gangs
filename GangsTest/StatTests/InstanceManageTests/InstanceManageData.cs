using System.Collections;
using Mock;

namespace GangsTest.StatTests.InstanceManageTests;

public class InstanceManageData : IEnumerable<object[]> {
  private object[][] behaviors;

  public InstanceManageData() {
    var inst = new MockStatManager();
    behaviors = new object[][] { [inst, new MockInstanceStatManager(inst)] };
  }

  public IEnumerator<object[]> GetEnumerator() {
    return behaviors.Select(behavior => behavior).GetEnumerator();
  }

  IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
}