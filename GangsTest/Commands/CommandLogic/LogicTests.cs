using GangsAPI.Data;
using GangsAPI.Services.Commands;
using GangsTest.Commands.ManagerTests;

namespace GangsTest.Commands.CommandLogic;

public class LogicTests : ManagerTests.ManagerTests {
  [Theory]
  [ClassData(typeof(ManagerData))]
  public void Command_Logic(ICommandManager mgr) {
    Assert.True(mgr.RegisterCommand(Dummy));
    Assert.True(mgr.ProcessCommand(TestPlayer, "css_dummy", "foobar"));
  }

  [Theory]
  [ClassData(typeof(ManagerData))]
  public void Command_Logic_Fail(ICommandManager mgr) {
    Assert.True(mgr.RegisterCommand(Dummy));
    Assert.False(mgr.ProcessCommand(TestPlayer, "css_dummy", "barfoo"));
  }
}