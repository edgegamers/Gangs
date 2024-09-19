using System.Collections;
using GangsAPI.Services.Player;
using Mock;
using SQLImpl;
using SQLite;

namespace GangsTest.API.Services.Player;

public class TestData : IEnumerable<object[]> {
  private readonly IPlayerManager[] behaviors = [
    new MockPlayerManager(), new MySQLPlayerManager(new TestDBConfig()),
    new SQLitePlayerManager("Data Source=:memory:", "gang_unit_test", true)
  ];

  public TestData() {
    foreach (var behavior in behaviors) behavior.Start();
  }

  public IEnumerator<object[]> GetEnumerator() {
    return behaviors.Select(behavior => (object[]) [behavior]).GetEnumerator();
  }

  IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
}