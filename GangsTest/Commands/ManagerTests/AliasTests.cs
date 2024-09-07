using GangsAPI.Data;
using GangsAPI.Data.Command;
using GangsAPI.Services.Commands;

namespace GangsTest.Commands.ManagerTests;

public class AliasTests : ManagerTests {
  private class AliasCommand : DummyCommand {
    public override string[] Aliases => ["css_dummy", "css_alias"];

    public override Task<CommandResult> Execute(PlayerWrapper? executor,
      CommandInfoWrapper info) {
      return Task.FromResult(CommandResult.SUCCESS);
    }
  }

  [Theory]
  [ClassData(typeof(CommandManagerData))]
  public async Task Command_Alias(ICommandManager mgr) {
    mgr.RegisterCommand(new AliasCommand());
    Assert.Equal(CommandResult.SUCCESS,
      await mgr.ProcessCommand(TestPlayer, "css_alias", "foobar"));
    Assert.Equal(CommandResult.SUCCESS,
      await mgr.ProcessCommand(TestPlayer, "css_dummy", "foobar"));
  }

  [Theory]
  [ClassData(typeof(CommandManagerData))]
  public async Task Command_Alias_Case(ICommandManager mgr) {
    mgr.RegisterCommand(new AliasCommand());
    Assert.Equal(CommandResult.SUCCESS,
      await mgr.ProcessCommand(TestPlayer, "css_Alias", "foobar"));
    Assert.Equal(CommandResult.SUCCESS,
      await mgr.ProcessCommand(TestPlayer, "css_Dummy", "foobar"));
  }
}