using GangsAPI.Data;
using GangsAPI.Data.Command;
using GangsAPI.Services.Commands;

namespace GangsTest.Commands;

public class PermissionTests : ManagerTests.ManagerTests {
  [Theory]
  [ClassData(typeof(ManagerTests.CommandManagerData))]
  public async Task Permission_Pass(ICommandManager mgr) {
    mgr.RegisterCommand(Dummy);
    Assert.Equal(CommandResult.SUCCESS,
      await mgr.ProcessCommand(TestPlayer, "css_dummy", "foobar"));
  }

  [Theory]
  [ClassData(typeof(ManagerTests.CommandManagerData))]
  public async Task Permission_Pass_Flag_Console(ICommandManager mgr) {
    mgr.RegisterCommand(new ElevatedCommand(["@test/flag"], []));
    Assert.Equal(CommandResult.SUCCESS,
      await mgr.ProcessCommand(null, "css_elevated"));
  }

  [Theory]
  [ClassData(typeof(ManagerTests.CommandManagerData))]
  public async Task Permission_Pass_Group_Console(ICommandManager mgr) {
    mgr.RegisterCommand(new ElevatedCommand([], ["#test/group"]));
    Assert.Equal(CommandResult.SUCCESS,
      await mgr.ProcessCommand(null, "css_elevated"));
  }

  [Theory]
  [ClassData(typeof(ManagerTests.CommandManagerData))]
  public async Task Permission_Fail_Flag(ICommandManager mgr) {
    mgr.RegisterCommand(new ElevatedCommand(["@test/flag"], []));
    Assert.Equal(CommandResult.NO_PERMISSION,
      await mgr.ProcessCommand(TestPlayer, "css_elevated"));
  }

  [Theory]
  [ClassData(typeof(ManagerTests.CommandManagerData))]
  public async Task Permission_Fail_Group(ICommandManager mgr) {
    mgr.RegisterCommand(new ElevatedCommand([], ["#test/group"]));
    Assert.Equal(CommandResult.NO_PERMISSION,
      await mgr.ProcessCommand(TestPlayer, "css_elevated"));
  }

  [Theory]
  [ClassData(typeof(ManagerTests.CommandManagerData))]
  public async Task Permission_Fail_Both_Flag(ICommandManager mgr) {
    mgr.RegisterCommand(new ElevatedCommand(["@test/flag"], ["#test/group"]));
    Assert.Equal(CommandResult.NO_PERMISSION,
      await mgr.ProcessCommand(TestPlayer, "css_elevated"));
  }

  [Theory]
  [ClassData(typeof(ManagerTests.CommandManagerData))]
  public async Task Permission_Fail_Both_Group(ICommandManager mgr) {
    mgr.RegisterCommand(new ElevatedCommand(["@test/flag"], ["#test/group"]));
    Assert.Equal(CommandResult.NO_PERMISSION,
      await mgr.ProcessCommand(TestPlayer, "css_elevated"));
  }

  [Theory]
  [ClassData(typeof(ManagerTests.CommandManagerData))]
  public async Task Permission_Pass_Flag(ICommandManager mgr) {
    var elevatedPlayer = TestPlayer.WithFlags("@test/flag");
    mgr.RegisterCommand(new ElevatedCommand(["@test/flag"], []));
    Assert.Equal(CommandResult.SUCCESS,
      await mgr.ProcessCommand(elevatedPlayer, "css_elevated"));
  }

  [Theory]
  [ClassData(typeof(ManagerTests.CommandManagerData))]
  public async Task Permission_Pass_Group(ICommandManager mgr) {
    var elevatedPlayer = TestPlayer.WithGroups("#test/group");
    mgr.RegisterCommand(new ElevatedCommand([], ["#test/group"]));
    Assert.Equal(CommandResult.SUCCESS,
      await mgr.ProcessCommand(elevatedPlayer, "css_elevated"));
  }

  [Theory]
  [ClassData(typeof(ManagerTests.CommandManagerData))]
  public async Task Permission_Pass_Both(ICommandManager mgr) {
    var elevatedPlayer =
      TestPlayer.WithFlags("@test/flag").WithGroups("#test/group");
    mgr.RegisterCommand(new ElevatedCommand(["@test/flag"], ["#test/group"]));
    Assert.Equal(CommandResult.SUCCESS,
      await mgr.ProcessCommand(elevatedPlayer, "css_elevated"));
  }

  private class ElevatedCommand(string[] flags, string[] groups) : ICommand {
    public string Name => "css_elevated";
    public string Description => "Elevated command for testing";
    public string[] RequiredFlags { get; } = flags;
    public string[] RequiredGroups { get; } = groups;

    public Task<CommandResult> Execute(PlayerWrapper? executor,
      CommandInfoWrapper info) {
      return Task.FromResult(CommandResult.SUCCESS);
    }
  }
}