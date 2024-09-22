using Commands.Gang;
using CounterStrikeSharp.API.Modules.Commands;
using GangsAPI;
using GangsAPI.Data.Command;
using GangsAPI.Services.Gang;
using Microsoft.Extensions.DependencyInjection;

namespace GangsTest.Commands.Gang;

public class BalanceTests(IServiceProvider provider)
  : TestParent(provider, new BalanceCommand(provider)) {
  private readonly IGangManager gangs =
    provider.GetRequiredService<IGangManager>();

  [Fact]
  public async Task Balance_WithoutGang_PrintsNoGang() {
    Assert.Equal(CommandResult.SUCCESS,
      await Commands.ProcessCommand(TestPlayer, CommandCallingContext.Console,
        Command.Name));
    Assert.Contains(Locale.Get(MSG.NOT_IN_GANG), TestPlayer.ConsoleOutput);
  }

  [Fact]
  public async Task Balance_WithoutCredits_PrintsNoCredits() {
    await gangs.CreateGang("Test Gang", TestPlayer.Steam);
    Assert.Equal(CommandResult.SUCCESS,
      await Commands.ProcessCommand(TestPlayer, CommandCallingContext.Console,
        Command.Name));
    Assert.Contains(Locale.Get(MSG.COMMAND_BALANCE_GANG_NONE, "Test Gang"),
      TestPlayer.ConsoleOutput);
  }
}