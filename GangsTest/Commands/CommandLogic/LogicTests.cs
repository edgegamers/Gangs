using GangsAPI.Data;
using GangsAPI.Data.Command;
using GangsAPI.Services.Commands;
using GangsTest.Commands.ManagerTests;

namespace GangsTest.Commands.CommandLogic;

public class LogicTests : ManagerTests.ManagerTests {
  [Theory]
  [ClassData(typeof(ManagerData))]
  public void Command_Logic(ICommandManager mgr) {
    Assert.True(mgr.RegisterCommand(Dummy));
    Assert.Equal(CommandResult.SUCCESS,
      mgr.ProcessCommand(TestPlayer, "css_dummy", "foobar"));
  }

  [Theory]
  [ClassData(typeof(ManagerData))]
  public void Command_Logic_Fail(ICommandManager mgr) {
    Assert.True(mgr.RegisterCommand(Dummy));
    Assert.Equal(CommandResult.FAILURE,
      mgr.ProcessCommand(TestPlayer, "css_dummy", "barfoo"));
  }
}