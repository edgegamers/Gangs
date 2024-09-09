using GangsAPI.Services;
using GangsAPI.Services.Gang;
using GangsAPI.Services.Player;
using Mock;
using SQLImpl;
using SQLite;

namespace GangsTest.API.Services.Gang;

public class TestData : TheoryData<IGangManager> {
  private readonly IGangManager[] behaviors = [
    new MockGangManager(playerMgr, rankMgr),
    new MySQLGangManager(playerMgr, rankMgr, new TestDBConfig()),
    new SQLiteGangManager(playerMgr, rankMgr, "Data Source=:memory:",
      "gang_unit_test", true)
  ];

  public TestData() {
    foreach (var behavior in behaviors) {
      behavior.Start();
      Add(behavior);
    }
  }

  private static IPlayerManager playerMgr => new MockPlayerManager();
  private static IRankManager rankMgr => new MockRankManager(playerMgr);

  public new IEnumerator<object[]> GetEnumerator() {
    return behaviors.Select(behavior => (object[]) [behavior]).GetEnumerator();
  }
}