using CounterStrikeSharp.API.Modules.Commands;
using GangsAPI;
using GangsAPI.Data;
using GangsAPI.Data.Command;
using GangsAPI.Services.Commands;
using Microsoft.Extensions.Localization;

namespace GangsTest.API.Services.Commands.CommandManager;

public class PlayerOnlyTests(IStringLocalizer locale) : TestParent {
  [Theory]
  [ClassData(typeof(TestData))]
  public async Task Command_PlayerOnly(ICommandManager mgr) {
    mgr.RegisterCommand(new PlayerOnlyCommand());
    Assert.Equal(CommandResult.PLAYER_ONLY,
      await mgr.ProcessCommand(TestPlayer, CommandCallingContext.Console,
        "css_player"));
    Assert.Contains(locale.Get(MSG.GENERIC_PLAYER_ONLY),
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