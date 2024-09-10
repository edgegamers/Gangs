using System.Collections;
using GangsAPI.Services.Commands;
using GangsAPI.Services.Menu;
using GangsImpl;
using GangsTest.TestLocale;
using Mock;

namespace GangsTest.API.Services.Menu;

public class TestData : IEnumerable<object[]> {
  private readonly IMenuManager[] behaviors = [
    new MockMenuManager(),
    new CommandBasedMenuManager(
      new Lazy<ICommandManager>(
        new MockCommandManager(StringLocalizer.Instance)))
  ];

  public IEnumerator<object[]> GetEnumerator() {
    return behaviors.Select(behavior => (object[]) [behavior]).GetEnumerator();
  }

  IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
}