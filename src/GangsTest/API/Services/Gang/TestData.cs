using GangsAPI.Services;
using GangsAPI.Services.Gang;
using GangsAPI.Services.Player;
using Mock;
using SQLImpl;
using SQLite;

namespace GangsTest.API.Services.Gang;

public class TestData : TheoryData<IGangManager> {
  private readonly IGangManager[] behaviors = [
    new MockGangManager(players, ranks),
    new MySQLGangManager(players, ranks, new TestDBConfig()),
    new SQLiteGangManager(players, ranks, "Data Source=:memory:",
      "gang_unit_test", true)
  ];

  public TestData() {
    foreach (var behavior in behaviors) {
      behavior.Start();
      Add(behavior);
    }
  }

  private static IPlayerManager players => new MockPlayerManager();
  private static IRankManager ranks => new MockRankManager(players);

  public new IEnumerator<object[]> GetEnumerator() {
    return behaviors.Select(behavior => (object[]) [behavior]).GetEnumerator();
  }
}