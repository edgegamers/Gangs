using GangsAPI.Data.Command;
using GangsAPI.Services.Commands;

namespace GangsTest.API.Services.Commands.CommandManager;

public class LogicTests : TestParent {
  [Theory]
  [ClassData(typeof(TestData))]
  public async Task Command_Logic(ICommandManager mgr) {
    Assert.True(mgr.RegisterCommand(Dummy));
    Assert.Equal(CommandResult.SUCCESS,
      await mgr.ProcessCommand(TestPlayer, "css_dummy", "foobar"));
  }

  [Theory]
  [ClassData(typeof(TestData))]
  public async Task Command_Logic_Fail(ICommandManager mgr) {
    Assert.True(mgr.RegisterCommand(Dummy));
    Assert.Equal(CommandResult.ERROR,
      await mgr.ProcessCommand(TestPlayer, "css_dummy", "barfoo"));
  }

  [Theory]
  [ClassData(typeof(TestData))]
  public async Task Command_CaseInsensitive(ICommandManager mgr) {
    mgr.RegisterCommand(Dummy);
    Assert.Equal(CommandResult.SUCCESS,
      await mgr.ProcessCommand(TestPlayer, "CSS_DUMMY", "foobar"));
  }

  [Theory]
  [ClassData(typeof(TestData))]
  public async Task Command_CaseInsensitive2(ICommandManager mgr) {
    mgr.RegisterCommand(Dummy);
    Assert.Equal(CommandResult.SUCCESS,
      await mgr.ProcessCommand(TestPlayer, "CsS_DumMY", "foobar"));
  }
}