using GangsAPI.Data;
using GangsAPI.Services.Menu;

namespace GangsTest.API.Services.Menu;

public abstract class TestParent {
  protected class TestMenuClass : IMenu {
    public Task Open(PlayerWrapper player) {
      player.PrintToChat("Test Menu Title");
      player.PrintToChat("1 | Test Menu Option");
      return Task.CompletedTask;
    }

    public Task Close(PlayerWrapper player) { return Task.CompletedTask; }

    public Task OnInput(PlayerWrapper player, int input) {
      player.PrintToChat($"You pressed {input}");
      return Task.CompletedTask;
    }
  }

  protected IMenu TestMenu = new TestMenuClass();

  protected PlayerWrapper TestPlayer =
    new PlayerWrapper((ulong)new Random().NextInt64(), "Test Player");
}