using Commands;
using GangsAPI;
using GangsAPI.Data.Command;
using GangsAPI.Services.Commands;
using GangsAPI.Services.Player;
using GangsTest.TestLocale;
using Microsoft.Extensions.Localization;
using Stats;

namespace GangsTest.API.Services.Commands.Command.Concrete;

public class BalanceTests(ICommandManager commands, IPlayerStatManager statMgr,
  IStringLocalizer locale) : TestParent(commands,
  new BalanceCommand(statMgr, StringLocalizer.Instance)) {
  private static readonly string STAT_ID = new BalanceStat().StatId;

  [Fact]
  public async Task None() {
    Assert.Equal("css_balance", Command.Name);
    Assert.Equal(CommandResult.SUCCESS,
      await Commands.ProcessCommand(TestPlayer, Command.Name));
    Assert.Contains(locale.Get(MSG.COMMAND_BALANCE_NONE),
      TestPlayer.ConsoleOutput);
  }

  [Fact]
  public async Task One() {
    await statMgr.SetForPlayer(TestPlayer.Steam, STAT_ID, 1);
    Assert.Equal(CommandResult.SUCCESS,
      await Commands.ProcessCommand(TestPlayer, Command.Name));
    Assert.Contains(locale.Get(MSG.COMMAND_BALANCE, 1),
      TestPlayer.ConsoleOutput);
  }

  [Theory]
  [InlineData(2)]
  [InlineData(5)]
  [InlineData(10000)]
  [InlineData(-1)]
  [InlineData(-1000)]
  public async Task Multiple(int bal) {
    await statMgr.SetForPlayer(TestPlayer.Steam, STAT_ID, bal);
    Assert.Equal(CommandResult.SUCCESS,
      await Commands.ProcessCommand(TestPlayer, Command.Name));
    Assert.Contains(locale.Get(MSG.COMMAND_BALANCE_PLURAL, bal),
      TestPlayer.ConsoleOutput);
  }
}