using GangsAPI.Data;
using GangsAPI.Services.Commands;
using GangsTest.Commands.ManagerTests;

namespace GangsTest.Commands.CommandPerms;

public class PermissionTests : ManagerTests.ManagerTests {
  private class ElevatedCommand(string[] flags, string[] groups) : ICommand {
    public string Name => "css_elevated";
    public string? Description => "Elevated command for testing";
    public string[] RequiredFlags { get; } = flags;
    public string[] RequiredGroups { get; } = groups;

    public bool Execute(PlayerWrapper? executor, CommandInfoWrapper info) {
      return true;
    }
  }

  [Theory]
  [ClassData(typeof(ManagerData))]
  public void Permission_Pass(ICommandManager mgr) {
    mgr.RegisterCommand(Dummy);
    Assert.True(mgr.ProcessCommand(TestPlayer, "css_dummy", "foobar"));
  }

  [Theory]
  [ClassData(typeof(ManagerData))]
  public void Permission_Pass_Flag_Console(ICommandManager mgr) {
    mgr.RegisterCommand(new ElevatedCommand(["@test/flag"], []));
    Assert.True(mgr.ProcessCommand(null, "css_elevated"),
      "Command manager failed to allow console");
  }

  [Theory]
  [ClassData(typeof(ManagerData))]
  public void Permission_Pass_Group_Console(ICommandManager mgr) {
    mgr.RegisterCommand(new ElevatedCommand([], ["#test/group"]));
    Assert.True(mgr.ProcessCommand(null, "css_elevated"),
      "Command manager failed to allow console");
  }

  [Theory]
  [ClassData(typeof(ManagerData))]
  public void Permission_Fail_Flag(ICommandManager mgr) {
    mgr.RegisterCommand(new ElevatedCommand(["@test/flag"], []));
    Assert.False(mgr.ProcessCommand(TestPlayer, "css_elevated"),
      "Command manager failed to check flags");
  }

  [Theory]
  [ClassData(typeof(ManagerData))]
  public void Permission_Fail_Group(ICommandManager mgr) {
    mgr.RegisterCommand(new ElevatedCommand([], ["#test/group"]));
    Assert.False(mgr.ProcessCommand(TestPlayer, "css_elevated"),
      "Command manager failed to check groups");
  }

  [Theory]
  [ClassData(typeof(ManagerData))]
  public void Permission_Fail_Both_Flag(ICommandManager mgr) {
    mgr.RegisterCommand(new ElevatedCommand(["@test/flag"], ["#test/group"]));
    Assert.False(
      mgr.ProcessCommand(TestPlayer.WithFlags("@test/flag"), "css_elevated"),
      "Command manager allowed player with flag but command required group as well");
  }

  [Theory]
  [ClassData(typeof(ManagerData))]
  public void Permission_Fail_Both_Group(ICommandManager mgr) {
    mgr.RegisterCommand(new ElevatedCommand(["@test/flag"], ["#test/group"]));
    Assert.False(
      mgr.ProcessCommand(TestPlayer.WithGroups("@test/flag"), "css_elevated"),
      "Command manager allowed player with flag but command required group as well");
  }

  [Theory]
  [ClassData(typeof(ManagerData))]
  public void Permission_Pass_Flag(ICommandManager mgr) {
    var elevatedPlayer = TestPlayer.WithFlags("@test/flag");
    mgr.RegisterCommand(new ElevatedCommand(["@test/flag"], []));
    Assert.True(mgr.ProcessCommand(elevatedPlayer, "css_elevated"),
      "Command manager did not allow player with proper flag");
  }

  [Theory]
  [ClassData(typeof(ManagerData))]
  public void Permission_Pass_Group(ICommandManager mgr) {
    var elevatedPlayer = TestPlayer.WithGroups("#test/group");
    mgr.RegisterCommand(new ElevatedCommand([], ["#test/group"]));
    Assert.True(mgr.ProcessCommand(elevatedPlayer, "css_elevated"),
      "Command manager did not allow player with proper group");
  }

  [Theory]
  [ClassData(typeof(ManagerData))]
  public void Permission_Pass_Both(ICommandManager mgr) {
    var elevatedPlayer =
      TestPlayer.WithFlags("@test/flag").WithGroups("#test/group");
    mgr.RegisterCommand(new ElevatedCommand(["@test/flag"], ["#test/group"]));
    Assert.True(mgr.ProcessCommand(elevatedPlayer, "css_elevated"),
      "Command manager did not allow player with proper group");
  }
}