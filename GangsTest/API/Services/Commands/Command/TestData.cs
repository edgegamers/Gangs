using System.Collections;
using Commands;
using Commands.Gang;
using GangsAPI;
using GangsAPI.Services;
using GangsAPI.Services.Gang;
using GangsAPI.Services.Player;
using Mock;

namespace GangsTest.API.Services.Commands.Command;

public class TestData : IEnumerable<object[]> {
  private static readonly IPlayerManager playerMgr = new MockPlayerManager();
  private static readonly IGangManager manager = new MockGangManager(playerMgr);

  private readonly IBehavior[] behaviors = [
    new CreateCommand(manager), new HelpCommand(), new GangCommand(manager)
  ];

  public TestData() {
    foreach (var behavior in behaviors) behavior.Start();
  }

  public IEnumerator<object[]> GetEnumerator() {
    return behaviors.Select(behavior => (object[]) [behavior]).GetEnumerator();
  }

  IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
}