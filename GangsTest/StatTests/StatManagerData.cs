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
      ?? throw new NullReferenceException("DB_CONNECTION_STRING is not set"),
      "gang_unit_test", true),
    new SQLiteStatManager("Data Source=:memory:", "gang_unit_test", true)
  ];

  public StatManagerData() {
    foreach (var behavior in behaviors) behavior.Start();
  }

  public IEnumerator<object[]> GetEnumerator() {
    return behaviors.Select(behavior => (object[]) [behavior]).GetEnumerator();
  }

  IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
}