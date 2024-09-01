using Commands;
using GangsAPI.Data.Command;
using GangsAPI.Services.Commands;

namespace GangsTest.Commands;

public class GangCommandTests(ICommandManager commands)
  : CommandTest(commands, new GangCommand()) {
  [Fact]
  public void Gang_TestBase() {
    Assert.Equal("css_gang", Command.Name);
    Assert.Equal(CommandResult.INVALID_ARGS,
      Commands.ProcessCommand(TestPlayer, Command.Name));
  }

  [Fact]
  public void Gang_TestUnknown() {
    Assert.Equal(CommandResult.UNKNOWN_COMMAND,
      Commands.ProcessCommand(TestPlayer, Command.Name, "foobar"));
  }

  [Fact]
  public void Gang_TestHelp() {
    Assert.Equal(CommandResult.SUCCESS,
      Commands.ProcessCommand(TestPlayer, "css_gang", "help"));
  }

  [Fact]
  public void Gang_TestHelp_Single() {
    Assert.Equal(CommandResult.UNKNOWN_COMMAND,
      Commands.ProcessCommand(TestPlayer, "css_gang help"));
  }
}