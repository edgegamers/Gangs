using System.Collections;
using Commands;
using Commands.gang;
using GangsAPI;
using GangsAPI.Services;
using Mock;

namespace GangsTest.Commands;

public class CommandTestData : IEnumerable<object[]> {
  private static readonly IGangManager manager = new MockGangManager();

  private readonly IBehavior[] behaviors = [
    new CreateCommand(manager), new HelpCommand(), new GangCommand(manager)
  ];

  public CommandTestData() {
    foreach (var behavior in behaviors) behavior.Start();
  }

  public IEnumerator<object[]> GetEnumerator() {
    return behaviors.Select(behavior => (object[]) [behavior]).GetEnumerator();
  }

  IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
}