using CounterStrikeSharp.API.Modules.Commands;
using GangsAPI.Data;
using GangsAPI.Data.Command;
using GangsAPI.Services.Commands;

namespace GangsTest.API.Services.Commands.CommandManager;

public class AliasTests : TestParent {
  [Theory]
  [ClassData(typeof(TestData))]
  public async Task Alias_Success(ICommandManager mgr) {
    mgr.RegisterCommand(new AliasCommand());
    Assert.Equal(CommandResult.SUCCESS,
      await mgr.ProcessCommand(TestPlayer, CommandCallingContext.Chat,
        "css_alias", "foobar"));
    Assert.Equal(CommandResult.SUCCESS,
      await mgr.ProcessCommand(TestPlayer, CommandCallingContext.Chat,
        "css_dummy", "foobar"));
  }

  [Theory]
  [ClassData(typeof(TestData))]
  public async Task Alias_Case(ICommandManager mgr) {
    mgr.RegisterCommand(new AliasCommand());
    Assert.Equal(CommandResult.SUCCESS,
      await mgr.ProcessCommand(TestPlayer, CommandCallingContext.Chat,
        "css_Alias", "foobar"));
    Assert.Equal(CommandResult.SUCCESS,
      await mgr.ProcessCommand(TestPlayer, CommandCallingContext.Chat,
        "css_Dummy", "foobar"));
  }

  private class AliasCommand : DummyCommand {
    public override string[] Aliases => ["css_dummy", "css_alias"];

    public override Task<CommandResult> Execute(PlayerWrapper? executor,
      CommandInfoWrapper info) {
      return Task.FromResult(CommandResult.SUCCESS);
    }
  }
}