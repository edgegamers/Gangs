using GangsAPI.Data.Command;
using GangsAPI.Services.Commands;
using Mock;

namespace GangsTest.API.Services.Menu;

public class CommandInteractionTests(ICommandManager cmdMgr)
  : TestParent(new CommandBasedMenuManager(cmdMgr)) {
  [Fact]
  public async Task Command_Interactions() {
    await MenuManager.OpenMenu(TestPlayer, TestMenu);
    Assert.Equal(CommandResult.SUCCESS,
      await cmdMgr.ProcessCommand(TestPlayer, "css_1"));
    Assert.Contains("You pressed 1", TestPlayer.ChatOutput);
  }

  [Fact]
  public async Task Command_Interactions_Multiple() {
    await MenuManager.OpenMenu(TestPlayer, TestMenu);
    Assert.Equal(CommandResult.SUCCESS,
      await cmdMgr.ProcessCommand(TestPlayer, "css_1"));
    Assert.Contains("You pressed 1", TestPlayer.ChatOutput);
    Assert.Equal(CommandResult.SUCCESS,
      await cmdMgr.ProcessCommand(TestPlayer, "css_3"));
    Assert.Contains("You pressed 3", TestPlayer.ChatOutput);
  }

  [Fact]
  public async Task Command_Interactions_Close() {
    await MenuManager.OpenMenu(TestPlayer, TestMenu);
    Assert.Equal(CommandResult.SUCCESS,
      await cmdMgr.ProcessCommand(TestPlayer, "css_5"));
    Assert.Contains("You pressed 5", TestPlayer.ChatOutput);
    Assert.Null(MenuManager.GetActiveMenu(TestPlayer));
  }
}