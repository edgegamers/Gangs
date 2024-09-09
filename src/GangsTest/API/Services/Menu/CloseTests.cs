using GangsAPI.Services.Menu;

namespace GangsTest.API.Services.Menu;

public class CloseTests(IMenuManager mgr) : TestParent(mgr) {
  [Theory]
  [ClassData(typeof(TestData))]
  public void Closes(IMenuManager mgr) {
    mgr.OpenMenu(TestPlayer, TestMenu);
    Assert.NotNull(mgr.GetActiveMenu(TestPlayer));
    mgr.CloseMenu(TestPlayer);
    Assert.Null(mgr.GetActiveMenu(TestPlayer));
  }

  [Theory]
  [ClassData(typeof(TestData))]
  public void Closes_Multiple(IMenuManager mgr) {
    var other = new TestMenuClass(mgr, "Foobar Title");
    mgr.OpenMenu(TestPlayer, TestMenu);
    mgr.OpenMenu(TestPlayer, other);
    mgr.CloseMenu(TestPlayer);
    Assert.Null(mgr.GetActiveMenu(TestPlayer));
  }

  [Theory]
  [ClassData(typeof(TestData))]
  public async Task Close_Returns_True(IMenuManager mgr) {
    await mgr.OpenMenu(TestPlayer, TestMenu);
    Assert.True(await mgr.CloseMenu(TestPlayer));
  }

  [Theory]
  [ClassData(typeof(TestData))]
  public async Task Close_Returns_False(IMenuManager mgr) {
    Assert.False(await mgr.CloseMenu(TestPlayer));
  }
}