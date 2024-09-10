using System.Collections;
using GangsAPI;
using GangsAPI.Services;
using GangsAPI.Services.Gang;
using GangsAPI.Services.Menu;
using GangsAPI.Services.Player;
using GangsAPI.Services.Server;
using GangsTest.TestLocale;
using Mock;

namespace GangsTest.API.Services.Commands.CommandManager;

public class TestData : IEnumerable<object[]> {
  private static readonly IPlayerManager players = new MockPlayerManager();
  private static readonly IMenuManager menus = new MockMenuManager();

  private static readonly IPlayerStatManager playerStats =
    new MockInstanceStatManager();

  private static readonly IGangStatManager gangStats =
    new MockInstanceStatManager();

  private static readonly IRankManager ranks = new MockRankManager(players);
  private static readonly IServerProvider server = new MockServerProvider();
  private static readonly ITargeter targeter = new MockTargeter(server);

  private readonly IBehavior[] behaviors = [
    new MockCommandManager(StringLocalizer.Instance),
    new global::Commands.CommandManager(new MockGangManager(players, ranks),
      playerStats, players, menus, ranks, gangStats, targeter,
      StringLocalizer.Instance)
  ];

  public TestData() {
    foreach (var behavior in behaviors) behavior.Start();
  }

  public IEnumerator<object[]> GetEnumerator() {
    return behaviors.Select(behavior => (object[]) [behavior]).GetEnumerator();
  }

  IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
}