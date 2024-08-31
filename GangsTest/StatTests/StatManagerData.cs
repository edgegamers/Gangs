using System.Collections;
using GangsAPI;
using GangsImpl.Memory;
using GangsImpl.SQLLite;
using SQLImpl;

namespace GangsTest.StatTests;

public class StatManagerData : IEnumerable<object[]> {
  private readonly IBehavior[] behaviors = [
    new MockStatManager(),
    new SqlStatStatManager(
      Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")
      ?? "Server=localhost;User=root;Database=gangs", "gang_unit_test"),
    new SQLiteStatManager("Data Source=:memory:", "gang_unit_test")
  ];

  public StatManagerData() {
    foreach (var behavior in behaviors) behavior.Start();
  }

  public IEnumerator<object[]> GetEnumerator() {
    return behaviors.Select(behavior => (object[]) [behavior]).GetEnumerator();
  }

  IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
}