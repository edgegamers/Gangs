using GangsAPI.Services;
using GangsAPI.Services.Gang;
using GangsAPI.Services.Player;
using Microsoft.Extensions.DependencyInjection;
using Mock;
using SQLImpl;
using SQLite;

namespace GangsTest.API.Services.Gang;

public class TestData : TheoryData<IGangManager> {
  private static readonly IServiceCollection services = new ServiceCollection();

  private readonly IGangManager[] behaviors = [
    new MockGangManager(players, ranks),
    new MySQLGangManager(services.BuildServiceProvider(), new TestDBConfig()),
    new SQLiteGangManager(services.BuildServiceProvider(),
      "Data Source=:memory:", "gang_unit_test", true)
  ];

  static TestData() { services.ConfigureServices(); }

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