using System.Collections;
using GangsAPI;
using GangsAPI.Services.Player;
using Mock;

namespace GangsTest.API.Services.Commands.CommandManager;

public class TestData : IEnumerable<object[]> {
  private static readonly IPlayerManager playerMgr = new MockPlayerManager();

  private readonly IBehavior[] behaviors = [
    new MockCommandManager(),
    new global::Commands.CommandManager(new MockGangManager(playerMgr))
  ];

  public TestData() {
    foreach (var behavior in behaviors) behavior.Start();
  }

  public IEnumerator<object[]> GetEnumerator() {
    return behaviors.Select(behavior => (object[]) [behavior]).GetEnumerator();
  }

  IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
}