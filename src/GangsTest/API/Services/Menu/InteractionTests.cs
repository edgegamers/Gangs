using GangsAPI.Services.Menu;

namespace GangsTest.API.Services.Menu;

public class InteractionTests(IMenuManager unused) : TestParent(unused) {
  [Theory]
  [ClassData(typeof(TestData))]
  public async Task Generic_Interactions(IMenuManager mgr) {
    await mgr.OpenMenu(TestPlayer, TestMenu);
    Assert.True(await mgr.AcceptInput(TestPlayer, 1));
    Assert.Contains("You pressed 1", TestPlayer.ChatOutput);
  }

  [Theory]
  [ClassData(typeof(TestData))]
  public async Task Generic_Interactions_Close(IMenuManager mgr) {
    var menu = new TestMenuClass(mgr);
    await mgr.OpenMenu(TestPlayer, menu);
    await mgr.AcceptInput(TestPlayer, 5);
    Assert.Contains("You pressed 5", TestPlayer.ChatOutput);
    Assert.Null(mgr.GetActiveMenu(TestPlayer));
  }
}