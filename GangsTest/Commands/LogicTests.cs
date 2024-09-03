using GangsAPI.Data;
using GangsAPI.Data.Command;
using GangsAPI.Services.Commands;
using GangsTest.Commands.ManagerTests;

namespace GangsTest.Commands.CommandLogic;

public class LogicTests : ManagerTests.ManagerTests {
  [Theory]
  [ClassData(typeof(ManagerData))]
  public async Task Command_Logic(ICommandManager mgr) {
    Assert.True(mgr.RegisterCommand(Dummy));
    Assert.Equal(CommandResult.SUCCESS,
      await mgr.ProcessCommand(TestPlayer, "css_dummy", "foobar"));
  }

  [Theory]
  [ClassData(typeof(ManagerData))]
  public async Task Command_Logic_Fail(ICommandManager mgr) {
    Assert.True(mgr.RegisterCommand(Dummy));
    Assert.Equal(CommandResult.FAILURE,
      await mgr.ProcessCommand(TestPlayer, "css_dummy", "barfoo"));
  }
}