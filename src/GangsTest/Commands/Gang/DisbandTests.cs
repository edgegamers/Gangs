using Commands.Gang;
using CounterStrikeSharp.API.Modules.Commands;
using GangsAPI;
using GangsAPI.Data.Command;
using GangsAPI.Permissions;

namespace GangsTest.Commands.Gang;

public class DisbandTests(IServiceProvider provider)
  : TestParent(provider, new DisbandCommand(provider)) {
  [Fact]
  public async Task Disband_WithConsole_ShouldFail() {
    var result = await Commands.ProcessCommand(null,
      CommandCallingContext.Console, Command.Name);
    Assert.Equal(CommandResult.PLAYER_ONLY, result);
  }

  [Fact]
  public async Task Disband_WithoutGang_ShouldFail() {
    var otherPlayer = TestUtil.CreateFakePlayer();
    var result = await Commands.ProcessCommand(TestPlayer,
      CommandCallingContext.Chat, Command.Name, otherPlayer.Name!);
    Assert.Equal(CommandResult.ERROR, result);
    Assert.Contains(Locale.Get(MSG.NOT_IN_GANG), TestPlayer.ChatOutput);
  }

  [Theory]
  [InlineData(10)]
  [InlineData(30)]
  [InlineData(50)]
  [InlineData(100)]
  public async Task Disband_NotOwner_ShouldFail(int rank) {
    var otherPlayer     = TestUtil.CreateFakePlayer();
    var otherGangPlayer = await Players.GetPlayer(otherPlayer.Steam);
    var gang            = await Gangs.CreateGang("Test Gang", TestPlayer);

    Assert.NotNull(otherGangPlayer);
    Assert.NotNull(gang);

    otherGangPlayer.GangId   = gang.GangId;
    otherGangPlayer.GangRank = rank;
    await Players.UpdatePlayer(otherGangPlayer);

    var result = await Commands.ProcessCommand(otherPlayer,
      CommandCallingContext.Chat, Command.Name);
    Assert.Equal(CommandResult.SUCCESS, result);
    Assert.Contains(Locale.Get(MSG.GENERIC_NOPERM_RANK, "Owner"),
      otherPlayer.ChatOutput);
  }

  [Fact]
  public async Task Disband_WithOwner_ShouldAskForConfirmation() {
    await Gangs.CreateGang("Test Gang", TestPlayer);
    var result = await Commands.ProcessCommand(TestPlayer,
      CommandCallingContext.Chat, Command.Name);
    var exp = Locale.Get(MSG.COMMAND_GANG_DISBAND_WARN);
    Assert.Equal(CommandResult.SUCCESS, result);
    Assert.Contains(exp, TestPlayer.ChatOutput);
  }

  [Fact]
  public async Task Disband_WithOwnerAndConfirm_ShouldDisband() {
    var gang = await Gangs.CreateGang("Test Gang", TestPlayer);
    Assert.NotNull(gang);
    var result = await Commands.ProcessCommand(TestPlayer,
      CommandCallingContext.Chat, Command.Name, "confirm");
    Assert.Equal(CommandResult.SUCCESS, result);
    var exp = Locale.Get(MSG.COMMAND_GANG_DISBANDED,
      TestPlayer.Name ?? TestPlayer.Steam.ToString());
    exp = $"SYSTEM: {exp}";
    Assert.Contains(exp, GangChat.GetChatHistory(gang));

    gang = await Gangs.GetGang(gang.GangId);
    Assert.Null(gang);
  }
}