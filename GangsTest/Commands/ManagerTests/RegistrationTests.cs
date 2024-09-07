using GangsAPI.Data.Command;
using GangsAPI.Services.Commands;

namespace GangsTest.Commands.ManagerTests;

public class RegistrationTests : ManagerTests {
  [Theory]
  [ClassData(typeof(CommandManagerData))]
  public async Task Command_Register(ICommandManager mgr) {
    Assert.True(mgr.RegisterCommand(Dummy));
    Assert.Equal(CommandResult.SUCCESS,
      await mgr.ProcessCommand(TestPlayer, "css_dummy", "foobar"));
  }

  [Theory]
  [ClassData(typeof(CommandManagerData))]
  public async Task Command_Unregistered(ICommandManager mgr) {
    Assert.Equal(CommandResult.UNKNOWN_COMMAND,
      await mgr.ProcessCommand(TestPlayer, "css_dummy"));
  }

  [Theory]
  [ClassData(typeof(CommandManagerData))]
  public async Task Command_Unregister(ICommandManager mgr) {
    Assert.True(mgr.RegisterCommand(Dummy));
    Assert.True(mgr.UnregisterCommand(Dummy));
    Assert.Equal(CommandResult.UNKNOWN_COMMAND,
      await mgr.ProcessCommand(TestPlayer, "css_dummy"));
  }

  [Theory]
  [ClassData(typeof(CommandManagerData))]
  public void Command_Unregister_Unregistered(ICommandManager mgr) {
    Assert.False(mgr.UnregisterCommand(Dummy));
  }
}