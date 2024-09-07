using System.Collections;
using GangsAPI;
using GangsAPI.Services.Player;
using Mock;
using SQLImpl;
using SQLite;

namespace GangsTest.API.Services.Gang;

public class TestData : IEnumerable<object[]> {
  private readonly IBehavior[] behaviors = [
    new MockGangManager(playerMgr),
    new MySQLGangManager(playerMgr,
      Environment.GetEnvironmentVariable("DB_GANGS_CONNECTION")
      ?? "Host=localhost;User=root;Database=gangs", "gang_unit_test", true),
    new SQLiteGangManager(playerMgr, "Data Source=:memory:", "gang_unit_test",
      true)
  ];

  public TestData() {
    foreach (var behavior in behaviors) behavior.Start();
  }

  private static IPlayerManager playerMgr => new MockPlayerManager();

  public IEnumerator<object[]> GetEnumerator() {
    return behaviors.Select(behavior => (object[]) [behavior]).GetEnumerator();
  }

  IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
}