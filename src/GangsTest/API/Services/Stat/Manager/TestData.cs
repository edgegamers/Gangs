using System.Collections;
using GangsAPI;
using Mock;
using SQLImpl;
using SQLite;

namespace GangsTest.API.Services.Stat.Manager;

public class TestData : IEnumerable<object[]> {
  private readonly IBehavior[] behaviors = [
    new MockStatManager(),
    new MySQLStatManager(new TestDBConfig()),
    new SQLiteStatManager("Data Source=:memory:", "gang_unit_test", true)
  ];

  public TestData() {
    foreach (var behavior in behaviors) behavior.Start();
  }

  public IEnumerator<object[]> GetEnumerator() {
    return behaviors.Select(behavior => (object[]) [behavior]).GetEnumerator();
  }

  IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
}