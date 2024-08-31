using GangsAPI.Services.Commands;

namespace GangsTest.Commands.ManagerTests;

public class RegistrationTests : ManagerTests {
  [Theory]
  [ClassData(typeof(ManagerData))]
  public void Command_Register(ICommandManager mgr) {
    Assert.True(mgr.RegisterCommand(Dummy));
    Assert.True(mgr.ProcessCommand(TestPlayer, "css_dummy", "foobar"));
  }

  [Theory]
  [ClassData(typeof(ManagerData))]
  public void Command_Unregistered(ICommandManager mgr) {
    Assert.False(mgr.ProcessCommand(TestPlayer, "css_dummy"));
  }

  [Theory]
  [ClassData(typeof(ManagerData))]
  public void Command_Unregister(ICommandManager mgr) {
    Assert.True(mgr.RegisterCommand(Dummy));
    Assert.True(mgr.UnregisterCommand(Dummy));
    Assert.False(mgr.ProcessCommand(TestPlayer, "css_dummy", "foobar"));
  }

  [Theory]
  [ClassData(typeof(ManagerData))]
  public void Command_Unregister_Unregistered(ICommandManager mgr) {
    Assert.False(mgr.UnregisterCommand(Dummy));
  }
}