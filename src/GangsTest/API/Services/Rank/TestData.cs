using GangsAPI.Services;
using GangsAPI.Services.Player;
using Mock;
using SQLImpl;

namespace GangsTest.API.Services.Rank;

public class TestData : TheoryData<IRankManager, IPlayerManager> {
  private static readonly IPlayerManager players = new MockPlayerManager();

  private readonly IRankManager[] behaviors = [
    new MockRankManager(players),
    new MySQLRankManager(players, new TestDBConfig())
  ];

  public TestData() {
    foreach (var behavior in behaviors) {
      behavior.Start();
      Add(behavior, players);
    }
  }
}