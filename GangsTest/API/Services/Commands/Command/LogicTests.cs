using GangsAPI.Data.Command;
using GangsAPI.Services.Commands;
using GangsTest.API.Services.Commands.CommandManager;

namespace GangsTest.API.Services.Commands.Command;

public class LogicTests : CommandManager.TestParent {
  [Theory]
  [ClassData(typeof(CommandManager.TestData))]
  public async Task Command_Logic(ICommandManager mgr) {
    Assert.True(mgr.RegisterCommand(Dummy));
    Assert.Equal(CommandResult.SUCCESS,
      await mgr.ProcessCommand(TestPlayer, "css_dummy", "foobar"));
  }

  [Theory]
  [ClassData(typeof(CommandManager.TestData))]
  public async Task Command_Logic_Fail(ICommandManager mgr) {
    Assert.True(mgr.RegisterCommand(Dummy));
    Assert.Equal(CommandResult.FAILURE,
      await mgr.ProcessCommand(TestPlayer, "css_dummy", "barfoo"));
  }

  [Theory]
  [ClassData(typeof(CommandManager.TestData))]
  public async Task Command_CaseInsensitive(ICommandManager mgr) {
    mgr.RegisterCommand(Dummy);
    Assert.Equal(CommandResult.SUCCESS,
      await mgr.ProcessCommand(TestPlayer, "CSS_DUMMY", "foobar"));
  }

  [Theory]
  [ClassData(typeof(CommandManager.TestData))]
  public async Task Command_CaseInsensitive2(ICommandManager mgr) {
    mgr.RegisterCommand(Dummy);
    Assert.Equal(CommandResult.SUCCESS,
      await mgr.ProcessCommand(TestPlayer, "CsS_DumMY", "foobar"));
  }
}