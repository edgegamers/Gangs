using System.Collections;
using GangsAPI;
using GangsAPI.Services;
using GangsAPI.Services.Menu;
using GangsAPI.Services.Player;
using GangsTest.TestLocale;
using Mock;

namespace GangsTest.API.Services.Commands.CommandManager;

public class TestData : IEnumerable<object[]> {
  private static readonly IPlayerManager playerMgr = new MockPlayerManager();
  private static readonly IMenuManager menuMgr = new MockMenuManager();

  private static readonly IPlayerStatManager playerStatMgr =
    new MockInstanceStatManager();

  private static readonly IRankManager rankMgr = new MockRankManager(playerMgr);

  private readonly IBehavior[] behaviors = [
    new MockCommandManager(StringLocalizer.Instance),
    new global::Commands.CommandManager(new MockGangManager(playerMgr, rankMgr),
      playerStatMgr, playerMgr, menuMgr, rankMgr, StringLocalizer.Instance)
  ];

  public TestData() {
    foreach (var behavior in behaviors) behavior.Start();
  }

  public IEnumerator<object[]> GetEnumerator() {
    return behaviors.Select(behavior => (object[]) [behavior]).GetEnumerator();
  }

  IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
}