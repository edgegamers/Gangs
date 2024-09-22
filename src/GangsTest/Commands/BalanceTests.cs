using Commands;
using CounterStrikeSharp.API.Modules.Commands;
using GangsAPI;
using GangsAPI.Data.Command;
using GangsAPI.Services.Player;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Stats.Stat;

namespace GangsTest.Commands;

public class BalanceTests(IServiceProvider provider) : TestParent(provider,
  new BalanceCommand(provider)) {
  private static readonly string STAT_ID = new BalanceStat().StatId;

  [Fact]
  public async Task None() {
    Assert.Equal("css_balance", Command.Name);
    Assert.Equal(CommandResult.SUCCESS,
      await Commands.ProcessCommand(TestPlayer, CommandCallingContext.Console,
        Command.Name));
    Assert.Contains(Locale.Get(MSG.COMMAND_BALANCE_NONE),
      TestPlayer.ConsoleOutput);
  }

  [Fact]
  public async Task One() {
    await PlayerStats.SetForPlayer(TestPlayer.Steam, STAT_ID, 1);
    Assert.Equal(CommandResult.SUCCESS,
      await Commands.ProcessCommand(TestPlayer, CommandCallingContext.Console,
        Command.Name));
    Assert.Contains(Locale.Get(MSG.COMMAND_BALANCE, 1),
      TestPlayer.ConsoleOutput);
  }

  [Theory]
  [InlineData(2)]
  [InlineData(5)]
  [InlineData(10000)]
  [InlineData(-1)]
  [InlineData(-1000)]
  public async Task Multiple(int bal) {
    await PlayerStats.SetForPlayer(TestPlayer.Steam, STAT_ID, bal);
    Assert.Equal(CommandResult.SUCCESS,
      await Commands.ProcessCommand(TestPlayer, CommandCallingContext.Console,
        Command.Name));
    Assert.Contains(Locale.Get(MSG.COMMAND_BALANCE, bal),
      TestPlayer.ConsoleOutput);
  }
}