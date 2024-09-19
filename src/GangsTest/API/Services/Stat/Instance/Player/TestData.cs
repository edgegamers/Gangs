using System.Collections;
using GangsAPI.Services.Player;
using Mock;
using SQLImpl;

namespace GangsTest.API.Services.Stat.Instance.Player;

public class TestData : IEnumerable<object[]> {
  private readonly IPlayerStatManager[] behaviors = [
    new MockInstanceStatManager(),
    new MySQLPlayerInstanceManager(new TestDBConfig())
  ];

  public TestData() {
    foreach (var behavior in behaviors) behavior.Start();
  }

  public IEnumerator<object[]> GetEnumerator() {
    return behaviors.Select(behavior => new[] { behavior }).GetEnumerator();
  }

  IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
}