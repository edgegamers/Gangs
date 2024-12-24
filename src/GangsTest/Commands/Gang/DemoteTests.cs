using Commands.Gang;
using CounterStrikeSharp.API.Modules.Commands;
using GangsAPI;
using GangsAPI.Data.Command;
using GangsAPI.Services.Server;

namespace GangsTest.Commands.Gang;

public class DemoteTests(IServiceProvider provider) : TestParent(provider,
  new DemoteCommand(provider)) {
  [Fact]
  public async Task Demote_WithConsole_ShouldFail() {
    var result = await Commands.ProcessCommand(null,
      CommandCallingContext.Console, Command.Name);
    Assert.Equal(CommandResult.PLAYER_ONLY, result);
  }

  [Fact]
  public async Task Demote_WithoutArgs_ShouldFail() {
    await Gangs.CreateGang("Test Gang", TestPlayer);
    var result = await Commands.ProcessCommand(TestPlayer,
      CommandCallingContext.Chat, Command.Name);
    Assert.Equal(CommandResult.PRINT_USAGE, result);
  }

  [Fact]
  public async Task Demote_WithoutGang_ShouldFail() {
    var otherPlayer = TestUtil.CreateFakePlayer();
    var result = await Commands.ProcessCommand(TestPlayer,
      CommandCallingContext.Chat, Command.Name, otherPlayer.Name!);
    Assert.Equal(CommandResult.ERROR, result);
    Assert.Contains(Locale.Get(MSG.NOT_IN_GANG), TestPlayer.ChatOutput);
  }
}