using Commands.Gang;
using CounterStrikeSharp.API.Modules.Commands;
using GangsAPI;
using GangsAPI.Data.Command;
using GangsAPI.Extensions;
using GangsAPI.Services.Gang;
using Microsoft.Extensions.DependencyInjection;
using Stats.Stat;

namespace GangsTest.Commands.Gang;

public class BalanceTests(IServiceProvider provider)
  : TestParent(provider, new BalanceCommand(provider)) {
  private const string STAT_ID = BalanceStat.STAT_ID;

  private readonly IGangManager gangs =
    provider.GetRequiredService<IGangManager>();

  [Fact]
  public async Task Balance_WithoutGang_PrintsNoGang() {
    var result = await Commands.ProcessCommand(TestPlayer,
      CommandCallingContext.Console, Command.Name);
    var expected = Locale.Get(MSG.NOT_IN_GANG);
    Assert.Equal(CommandResult.ERROR, result);
    Assert.Contains(expected, TestPlayer.ConsoleOutput);
  }

  [Fact]
  public async Task Balance_WithoutCredits_PrintsNoCredits() {
    await gangs.CreateGang("Test Gang", TestPlayer.Steam);
    var result = await Commands.ProcessCommand(TestPlayer,
      CommandCallingContext.Console, Command.Name);
    var expected = Locale.Get(MSG.COMMAND_BALANCE_GANG_NONE, "Test Gang");
    Assert.Equal(CommandResult.SUCCESS, result);
    Assert.Contains(expected, TestPlayer.ConsoleOutput);
  }

  [Fact]
  public async Task Balance_WithOtherGang_PrintsCredits() {
    var testGang = await gangs.CreateGang("Test Gang", TestPlayer.Steam);
    var otherGang =
      await gangs.CreateGang("Other Gang", new Random().NextULong());

    Assert.NotNull(testGang);
    Assert.NotNull(otherGang);

    await GangStats.SetForGang(testGang, STAT_ID, 1000);
    await GangStats.SetForGang(otherGang, STAT_ID, 500);

    var result = await Commands.ProcessCommand(TestPlayer,
      CommandCallingContext.Console, Command.Name, "Other Gang");
    var expected = Locale.Get(MSG.COMMAND_BALANCE_GANG, "Other Gang", 500);

    Assert.Equal(CommandResult.SUCCESS, result);
    Assert.Contains(expected, TestPlayer.ConsoleOutput);
  }

  [Theory]
  [InlineData(2)]
  [InlineData(5)]
  [InlineData(10000)]
  [InlineData(-1)]
  [InlineData(-1000)]
  public async Task Balance_WithCredits_PrintsExpected(int bal) {
    var gang = await gangs.CreateGang("Test Gang", TestPlayer.Steam);
    Assert.NotNull(gang);
    await GangStats.SetForGang(gang, STAT_ID, bal);
    Assert.Equal(CommandResult.SUCCESS,
      await Commands.ProcessCommand(TestPlayer, CommandCallingContext.Console,
        Command.Name));
    Assert.Contains(Locale.Get(MSG.COMMAND_BALANCE_GANG, "Test Gang", bal),
      TestPlayer.ConsoleOutput);
  }
}