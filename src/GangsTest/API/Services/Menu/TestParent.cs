using GangsAPI.Data;
using GangsAPI.Services.Menu;

namespace GangsTest.API.Services.Menu;

public abstract class TestParent {
  protected readonly IMenuManager MenuManager;
  protected readonly IMenu TestMenu;

  protected readonly PlayerWrapper TestPlayer =
    new((ulong)new Random().NextInt64(), "Test Player");

  public TestParent(IMenuManager menuManager) {
    MenuManager = menuManager;
    TestMenu    = new TestMenuClass(menuManager);

    MenuManager.Start();
  }

  protected sealed class TestMenuClass(IMenuManager mgr,
    string title = "Test Menu Title") : IMenu {
    public Task Open(PlayerWrapper player) {
      player.PrintToChat(title);
      player.PrintToChat("1 | Test Menu Option");
      return Task.CompletedTask;
    }

    public Task Close(PlayerWrapper player) { return Task.CompletedTask; }

    public async Task AcceptInput(PlayerWrapper player, int input) {
      player.PrintToChat($"You pressed {input}");
      if (input == 5) await mgr.CloseMenu(player);
    }
  }
}