using Commands.Gang;
using CounterStrikeSharp.API.Modules.Commands;
using GangsAPI;
using GangsAPI.Data.Command;

namespace GangsTest.Commands.Gang;

public class DepositTests(IServiceProvider provider)
  : TestParent(provider, new DepositCommand(provider)) {
  [Fact]
  public async Task Deposit_WithConsole_ShouldFail() {
    var result = await Commands.ProcessCommand(null,
      CommandCallingContext.Console, Command.Name);
    Assert.Equal(CommandResult.PLAYER_ONLY, result);
  }

  [Fact]
  public async Task Deposit_WithoutArgs_ShouldFail() {
    await Gangs.CreateGang("Test Gang", TestPlayer);
    var result = await Commands.ProcessCommand(TestPlayer,
      CommandCallingContext.Console, Command.Name);
    Assert.Equal(CommandResult.PRINT_USAGE, result);
  }

  [Fact]
  public async Task Deposit_WithoutGang_ShouldFail() {
    var result = await Commands.ProcessCommand(TestPlayer,
      CommandCallingContext.Chat, Command.Name, "100");
    Assert.Equal(CommandResult.ERROR, result);
    Assert.Contains(Locale.Get(MSG.NOT_IN_GANG), TestPlayer.ChatOutput);
  }

  [Fact]
  public async Task Deposit_WithoutAmount_ShouldFail() {
    await Gangs.CreateGang("Test Gang", TestPlayer);
    var result = await Commands.ProcessCommand(TestPlayer,
      CommandCallingContext.Chat, Command.Name);
    Assert.Equal(CommandResult.PRINT_USAGE, result);
  }

  [Theory]
  [InlineData("0")]
  [InlineData("Invalid")]
  [InlineData("100.5")]
  [InlineData("-100")]
  [InlineData("Infinity")]
  [InlineData("-Infinity")]
  public async Task Deposit_WithInvalidAmount_ShouldFail(string amount) {
    await Gangs.CreateGang("Test Gang", TestPlayer);
    var result = await Commands.ProcessCommand(TestPlayer,
      CommandCallingContext.Chat, Command.Name, amount);
    var exp = Locale.Get(MSG.COMMAND_INVALID_PARAM, amount,
      "a positive integer");
    Assert.Equal(CommandResult.INVALID_ARGS, result);
    Assert.Contains(exp, TestPlayer.ChatOutput);
  }

  [Fact]
  public async Task Deposit_WithInsufficient_ShouldFail() {
    var gang = await Gangs.CreateGang("Test Gang", TestPlayer);
    Assert.NotNull(gang);
    var result = await Commands.ProcessCommand(TestPlayer,
      CommandCallingContext.Chat, Command.Name, "100");
    Assert.Equal(CommandResult.ERROR, result);
  }

  [Fact]
  public async Task Deposit_WithValidAmount_ShouldSucceed() {
    var gang = await Gangs.CreateGang("Test Gang", TestPlayer);
    Assert.NotNull(gang);
    await Eco.Grant(TestPlayer, 1000);
    var result = await Commands.ProcessCommand(TestPlayer,
      CommandCallingContext.Chat, Command.Name, "100");
    Assert.Equal(CommandResult.SUCCESS, result);
    Assert.Equal(100, await Eco.GetBalance(gang));
    Assert.Equal(900, await Eco.GetBalance(TestPlayer));
  }

  [Theory]
  [InlineData(1)]
  [InlineData(10)]
  [InlineData(1000)]
  public async Task Deposit_WithAll_ShouldSucceed(int amount) {
    var gang = await Gangs.CreateGang("Test Gang", TestPlayer);
    Assert.NotNull(gang);
    await Eco.Grant(TestPlayer, amount);
    var result = await Commands.ProcessCommand(TestPlayer,
      CommandCallingContext.Chat, Command.Name, "all");
    Assert.Equal(CommandResult.SUCCESS, result);
    Assert.Equal(amount, await Eco.GetBalance(gang));
    Assert.Equal(0, await Eco.GetBalance(TestPlayer));
  }

  [Fact]
  public async Task Deposit_WithAllZero_ShouldFail() {
    var gang = await Gangs.CreateGang("Test Gang", TestPlayer);
    Assert.NotNull(gang);
    var result = await Commands.ProcessCommand(TestPlayer,
      CommandCallingContext.Chat, Command.Name, "all");
    Assert.Equal(CommandResult.ERROR, result);
  }
}