using System.Collections;
using GangsAPI;
using GangsAPI.Services;
using Mock;
using SQLImpl;
using SQLite;

namespace GangsTest.GangTests;

public class GangManagerData : IEnumerable<object[]> {
  private static readonly IPlayerManager playerMgr = new MockPlayerManager();

  private readonly IBehavior[] behaviors = [
    new MockGangManager(playerMgr),
    new SQLGangManager(playerMgr,
      Environment.GetEnvironmentVariable("DB_GANGS_CONNECTION")
      ?? "Host=localhost;User=root;Database=gangs", "gang_unit_test", true),
    new SQLiteGangManager(playerMgr, "Data Source=:memory:", "gang_unit_test",
      true)
  ];

  public GangManagerData() {
    foreach (var behavior in behaviors) behavior.Start();
  }

  public IEnumerator<object[]> GetEnumerator() {
    return behaviors.Select(behavior => (object[]) [behavior]).GetEnumerator();
  }

  IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
}