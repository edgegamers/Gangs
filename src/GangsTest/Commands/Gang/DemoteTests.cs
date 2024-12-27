using System.Diagnostics;
using Commands.Gang;
using CounterStrikeSharp.API.Modules.Commands;
using GangsAPI;
using GangsAPI.Data.Command;
using GangsAPI.Permissions;
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

  [Fact]
  public async Task Demote_WithoutPlayer_ShouldFail() {
    await Gangs.CreateGang("Test Gang", TestPlayer);
    var result = await Commands.ProcessCommand(TestPlayer,
      CommandCallingContext.Chat, Command.Name, "FakePlayer");
    Assert.Equal(CommandResult.INVALID_ARGS, result);
    Assert.Contains(Locale.Get(MSG.GENERIC_PLAYER_NOT_FOUND, "FakePlayer"),
      TestPlayer.ChatOutput);
  }

  [Fact]
  public async Task Demote_WithoutPerms_ShouldFail() {
    var otherPlayer     = TestUtil.CreateFakePlayer();
    var otherGangPlayer = await Players.GetPlayer(otherPlayer.Steam);
    var gang            = await Gangs.CreateGang("Test Gang", TestPlayer);

    Assert.NotNull(otherGangPlayer);
    Assert.NotNull(gang);

    otherGangPlayer.GangId   = gang.GangId;
    otherGangPlayer.GangRank = 100;
    await Players.UpdatePlayer(otherGangPlayer);

    var result = await Commands.ProcessCommand(otherPlayer,
      CommandCallingContext.Chat, Command.Name, TestPlayer.Name!);
    Assert.Equal(CommandResult.NO_PERMISSION, result);
    Assert.Contains(Locale.Get(MSG.GENERIC_NOPERM_RANK, "Manager"),
      otherPlayer.ChatOutput);
  }

  [Theory]
  [InlineData(30, 10, "Owner")]
  [InlineData(30, 30, "Co-Owner")]
  [InlineData(50, 30, "Manager")]
  [InlineData(100, 30, "Manager")]
  public async Task Demote_WithBadRank_ShouldFail(int rankA, int rankB,
    string rank) {
    var playerA     = TestUtil.CreateFakePlayer("PlayerA");
    var aGangPlayer = await Players.CreatePlayer(playerA.Steam, playerA.Name);
    var playerB     = TestUtil.CreateFakePlayer("PlayerB");
    var bGangPlayer = await Players.CreatePlayer(playerB.Steam, playerB.Name);
    var gang        = await Gangs.CreateGang("Test Gang", TestPlayer);
    var gangOwner   = await Players.GetPlayer(TestPlayer.Steam);

    Assert.NotNull(aGangPlayer);
    Assert.NotNull(bGangPlayer);
    Assert.NotNull(gangOwner);
    Assert.NotNull(gang);

    aGangPlayer.GangId   = gang.GangId;
    aGangPlayer.GangRank = rankA;
    await Players.UpdatePlayer(aGangPlayer);

    bGangPlayer.GangId   = gang.GangId;
    bGangPlayer.GangRank = rankB;
    await Players.UpdatePlayer(bGangPlayer);

    var result = await Commands.ProcessCommand(playerA,
      CommandCallingContext.Chat, Command.Name, playerB.Name!);
    Debug.WriteLine(playerA.ChatOutput);
    Assert.Equal(CommandResult.NO_PERMISSION, result);
    Assert.Contains(Locale.Get(MSG.GENERIC_NOPERM_RANK, rank),
      playerA.ChatOutput);
  }

  [Fact]
  public async Task Demote_WithSelf_ShouldFail() {
    var gangOwner =
      await Players.CreatePlayer(TestPlayer.Steam, TestPlayer.Name);
    await Players.UpdatePlayer(gangOwner);
    await Gangs.CreateGang("Test Gang", TestPlayer);

    var result = await Commands.ProcessCommand(TestPlayer,
      CommandCallingContext.Chat, Command.Name, TestPlayer.Name!);
    Assert.Equal(CommandResult.NO_PERMISSION, result);
    Assert.Contains(Locale.Get(MSG.RANK_CANNOT_OWNER, "demote yourself"),
      TestPlayer.ChatOutput);
  }

  [Theory]
  [InlineData(0, 10, "Manager")]
  [InlineData(0, 30, "Officer")]
  [InlineData(0, 50, "Member")]
  [InlineData(10, 30, "Officer")]
  [InlineData(10, 50, "Member")]
  [InlineData(30, 50, "Member")]
  public async Task Demote_WithValidRank_ShouldPass(int rankA, int rankB,
    string rank) {
    var playerA     = TestUtil.CreateFakePlayer("PlayerA");
    var aGangPlayer = await Players.CreatePlayer(playerA.Steam, playerA.Name);
    var playerB     = TestUtil.CreateFakePlayer("PlayerB");
    var bGangPlayer = await Players.CreatePlayer(playerB.Steam, playerB.Name);
    var gang        = await Gangs.CreateGang("Test Gang", TestPlayer);
    var gangOwner   = await Players.GetPlayer(TestPlayer.Steam);

    Assert.NotNull(aGangPlayer);
    Assert.NotNull(bGangPlayer);
    Assert.NotNull(gangOwner);
    Assert.NotNull(gang);

    aGangPlayer.GangId   = gang.GangId;
    aGangPlayer.GangRank = rankA;
    await Players.UpdatePlayer(aGangPlayer);

    bGangPlayer.GangId   = gang.GangId;
    bGangPlayer.GangRank = rankB;
    await Players.UpdatePlayer(bGangPlayer);

    var result = await Commands.ProcessCommand(playerA,
      CommandCallingContext.Chat, Command.Name, playerB.Name!);

    bGangPlayer = await Players.GetPlayer(bGangPlayer.Steam);
    Assert.NotNull(bGangPlayer);

    var newRank = await Ranks.GetRank(bGangPlayer);
    Assert.NotNull(newRank);
    Assert.Equal(rank, newRank.Name);

    var exp = Locale.Get(MSG.RANK_DEMOTE_SUCCESS, playerB.Name!, rank);
    exp = $"{playerA.Name}: {exp}";
    Assert.Equal(CommandResult.SUCCESS, result);
    Assert.Contains(exp, GangChat.GetChatHistory(gang));
  }
}