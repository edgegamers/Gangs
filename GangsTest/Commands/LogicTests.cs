using GangsAPI.Data.Command;
using GangsAPI.Services.Commands;

namespace GangsTest.Commands.CommandLogic;

public class LogicTests : ManagerTests.ManagerTests {
  [Theory]
  [ClassData(typeof(ManagerTests.CommandTestData))]
  public async Task Command_Logic(ICommandManager mgr) {
    Assert.True(mgr.RegisterCommand(Dummy));
    Assert.Equal(CommandResult.SUCCESS,
      await mgr.ProcessCommand(TestPlayer, "css_dummy", "foobar"));
  }

  [Theory]
  [ClassData(typeof(ManagerTests.CommandTestData))]
  public async Task Command_Logic_Fail(ICommandManager mgr) {
    Assert.True(mgr.RegisterCommand(Dummy));
    Assert.Equal(CommandResult.FAILURE,
      await mgr.ProcessCommand(TestPlayer, "css_dummy", "barfoo"));
  }
}