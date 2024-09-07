using GangsAPI.Data;
using GangsAPI.Data.Command;
using GangsAPI.Services.Commands;

namespace GangsTest.API.Services.Commands.CommandManager;

public class PlayerOnlyTests : TestParent {
  [Theory]
  [ClassData(typeof(TestData))]
  public async Task Command_PlayerOnly(ICommandManager mgr) {
    mgr.RegisterCommand(new PlayerOnlyCommand());
    Assert.Equal(CommandResult.PLAYER_ONLY,
      await mgr.ProcessCommand(TestPlayer, "css_player"));
    Assert.Contains("This command can only be executed by a player",
      TestPlayer.ConsoleOutput);
  }

  private class PlayerOnlyCommand : ICommand {
    public string Name => "css_player";

    public Task<CommandResult> Execute(PlayerWrapper? executor,
      CommandInfoWrapper info) {
      return Task.FromResult(CommandResult.PLAYER_ONLY);
    }
  }
}