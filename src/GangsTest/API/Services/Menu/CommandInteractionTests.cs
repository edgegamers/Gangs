using CounterStrikeSharp.API.Modules.Commands;
using GangsAPI.Data.Command;
using GangsAPI.Services.Commands;
using GangsImpl;

namespace GangsTest.API.Services.Menu;

public class CommandInteractionTests(ICommandManager cmds)
  : TestParent(new CommandBasedMenuManager(new Lazy<ICommandManager>(cmds))) {
  [Fact]
  public async Task Command_Interactions() {
    await MenuManager.OpenMenu(TestPlayer, TestMenu);
    Assert.Equal(CommandResult.SUCCESS,
      await cmds.ProcessCommand(TestPlayer, CommandCallingContext.Chat,
        "css_1"));
    Assert.Contains("You pressed 1", TestPlayer.ChatOutput);
  }

  [Fact]
  public async Task Command_Interactions_Multiple() {
    await MenuManager.OpenMenu(TestPlayer, TestMenu);
    Assert.Equal(CommandResult.SUCCESS,
      await cmds.ProcessCommand(TestPlayer, CommandCallingContext.Chat,
        "css_1"));
    Assert.Contains("You pressed 1", TestPlayer.ChatOutput);
    Assert.Equal(CommandResult.SUCCESS,
      await cmds.ProcessCommand(TestPlayer, CommandCallingContext.Chat,
        "css_3"));
    Assert.Contains("You pressed 3", TestPlayer.ChatOutput);
  }

  [Fact]
  public async Task Command_Interactions_Close() {
    await MenuManager.OpenMenu(TestPlayer, TestMenu);
    Assert.Equal(CommandResult.SUCCESS,
      await cmds.ProcessCommand(TestPlayer, CommandCallingContext.Chat,
        "css_5"));
    Assert.Contains("You pressed 5", TestPlayer.ChatOutput);
    Assert.Null(MenuManager.GetActiveMenu(TestPlayer));
  }
}