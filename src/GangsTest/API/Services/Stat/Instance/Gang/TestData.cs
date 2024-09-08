using System.Collections;
using GangsAPI.Services.Gang;
using Mock;
using SQLImpl;
using SQLite;

namespace GangsTest.API.Services.Stat.Instance.Gang;

public class TestData : IEnumerable<object[]> {
  private readonly IGangStatManager[] behaviors = [
    new MockInstanceStatManager(),
    new MySQLGangInstanceManager(new MockDBConfig(
      Environment.GetEnvironmentVariable("DB_GANGS_CONNECTION")
      ?? "Host=localhost;User=root;Database=gangs", "gang_inst_test", true)),
    new SQLiteGangInstanceManager("Data Source=:memory:", "gang_inst_stats",
      true)
  ];

  public TestData() {
    foreach (var behavior in behaviors) behavior.Start();
  }

  public IEnumerator<object[]> GetEnumerator() {
    return behaviors.Select(behavior => new[] { behavior }).GetEnumerator();
  }

  IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
}