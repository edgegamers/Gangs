using GangsAPI.Services.Gang;
using GangsAPI.Services.Player;
using Mock;
using SQLImpl;
using SQLite;

namespace GangsTest.API.Services.Gang;

public class TestData : TheoryData<IGangManager> {
  private readonly IGangManager[] behaviors = [
    new MockGangManager(playerMgr),
    new MySQLGangManager(playerMgr,
      new MockDBConfig(
        Environment.GetEnvironmentVariable("DB_GANGS_CONNECTION")
        ?? "Host=localhost;User=root;Database=gangs", "gang_unit_test", true)),
    new SQLiteGangManager(playerMgr, "Data Source=:memory:", "gang_unit_test",
      true)
  ];

  public TestData() {
    foreach (var behavior in behaviors) {
      behavior.Start();
      Add(behavior);
    }
  }

  private static IPlayerManager playerMgr => new MockPlayerManager();

  public new IEnumerator<object[]> GetEnumerator() {
    return behaviors.Select(behavior => (object[]) [behavior]).GetEnumerator();
  }
}