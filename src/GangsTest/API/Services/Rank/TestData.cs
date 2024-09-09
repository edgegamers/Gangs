using GangsAPI.Services;
using GangsAPI.Services.Player;
using Mock;
using SQLImpl;

namespace GangsTest.API.Services.Rank;

public class TestData : TheoryData<IRankManager, IPlayerManager> {
  private static readonly IPlayerManager playerMgr = new MockPlayerManager();

  private readonly IRankManager[] behaviors = [
    new MockRankManager(playerMgr),
    new MySQLRankManager(playerMgr, new TestDBConfig())
  ];

  public TestData() {
    foreach (var behavior in behaviors) {
      behavior.Start();
      Add(behavior, playerMgr);
    }
  }
}