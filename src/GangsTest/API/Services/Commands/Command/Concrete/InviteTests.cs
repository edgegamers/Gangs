using Commands.Gang;
using GangsAPI;
using GangsAPI.Data;
using GangsAPI.Data.Command;
using GangsAPI.Extensions;
using GangsAPI.Permissions;
using GangsAPI.Services;
using GangsAPI.Services.Commands;
using GangsAPI.Services.Gang;
using GangsAPI.Services.Player;
using Microsoft.Extensions.Localization;
using Stats;

namespace GangsTest.API.Services.Commands.Command.Concrete;

public class InviteTests(ICommandManager cmdMgr, IGangManager gangMgr,
  IPlayerManager playerMgr, IRankManager rankMgr, IGangStatManager gangStatMgr,
  IStringLocalizer localizer) : TestParent(cmdMgr,
  new InviteCommand(gangMgr, playerMgr, rankMgr, gangStatMgr, localizer)) {
  private readonly string inviteId = new InvitationStat().StatId;

  [Fact]
  public async Task Invite_NonPlayer() {
    Assert.Equal(CommandResult.PLAYER_ONLY,
      await Commands.ProcessCommand(null, "invite"));
  }

  [Fact]
  public async Task Invite_NoArgument() {
    Assert.Equal(CommandResult.PRINT_USAGE,
      await Commands.ProcessCommand(TestPlayer, "invite"));
    Assert.Contains(
      localizer.Get(MSG.COMMAND_USAGE, $"{Command.Name} {Command.Usage[0]}"),
      TestPlayer.ConsoleOutput);
  }

  [Fact]
  public async Task Invite_NotInGang() {
    Assert.Equal(CommandResult.SUCCESS,
      await Commands.ProcessCommand(TestPlayer, "invite", "123"));
    Assert.Contains(localizer.Get(MSG.NOT_IN_GANG), TestPlayer.ConsoleOutput);
  }

  [Fact]
  public async Task Invite_Self() {
    await playerMgr.CreatePlayer(TestPlayer.Steam, TestPlayer.Name);
    await gangMgr.CreateGang("Test Gang", TestPlayer);
    Assert.Equal(CommandResult.SUCCESS,
      await Commands.ProcessCommand(TestPlayer, "invite",
        TestPlayer.Steam.ToString()));
    Assert.Equal(
      [localizer.Get(MSG.COMMAND_INVITE_IN_YOUR_GANG, TestPlayer.Name!)],
      TestPlayer.ConsoleOutput);
  }

  [Fact]
  public async Task Invite_InvalidSteam() {
    await playerMgr.CreatePlayer(TestPlayer.Steam, TestPlayer.Name);
    await gangMgr.CreateGang("Test Gang", TestPlayer);
    Assert.Equal(CommandResult.SUCCESS,
      await Commands.ProcessCommand(TestPlayer, "invite", "123"));
    Assert.Equal([localizer.Get(MSG.GENERIC_STEAM_NOT_FOUND, "123")],
      TestPlayer.ConsoleOutput);
  }

  [Fact]
  public async Task Invite_NoPermission() {
    await playerMgr.CreatePlayer(TestPlayer.Steam, TestPlayer.Name);
    var gang = await gangMgr.CreateGang("Test Gang", TestPlayer);
    Assert.NotNull(gang);

    var guest = await playerMgr.CreatePlayer(new Random().NextUInt(), "Guest");
    guest.GangId   = gang.GangId;
    guest.GangRank = (await rankMgr.GetJoinRank(gang)).Rank;
    await playerMgr.UpdatePlayer(guest);

    var guestWrapper = new PlayerWrapper(guest);

    var needed = await rankMgr.GetRankNeeded(gang.GangId, Perm.INVITE_OTHERS);

    Assert.NotNull(needed);

    Assert.Equal(CommandResult.NO_PERMISSION,
      await Commands.ProcessCommand(guestWrapper, "invite", "123"));
    Assert.Equal([localizer.Get(MSG.GENERIC_NOPERM_RANK, needed.Name)],
      guestWrapper.ConsoleOutput);
  }

  [Fact]
  public async Task Invite_Offline() {
    await playerMgr.CreatePlayer(TestPlayer.Steam, TestPlayer.Name);
    var toInvite =
      await playerMgr.CreatePlayer(new Random().NextUInt(), "Invite Me!");
    var gang = await gangMgr.CreateGang("Test Gang", TestPlayer);
    Assert.NotNull(gang);

    Assert.Equal(CommandResult.SUCCESS,
      await Commands.ProcessCommand(TestPlayer, "invite",
        toInvite.Steam.ToString()));
    Assert.Equal(
      [localizer.Get(MSG.COMMAND_INVITE_SUCCESS, toInvite.Name!, gang.Name)],
      TestPlayer.ConsoleOutput);
  }

  [Fact]
  public async Task Invite_AlreadyInvited() {
    await playerMgr.CreatePlayer(TestPlayer.Steam, TestPlayer.Name);
    var toInvite =
      await playerMgr.CreatePlayer(new Random().NextUInt(), "Invite Me!");
    var gang = await gangMgr.CreateGang("Test Gang", TestPlayer);
    Assert.NotNull(gang);

    var data = new InvitationData();
    data.AddInvitation(TestPlayer.Steam, toInvite.Steam);
    await gangStatMgr.SetForGang(gang.GangId, inviteId, data);

    Assert.Equal(CommandResult.SUCCESS,
      await Commands.ProcessCommand(TestPlayer, "invite",
        toInvite.Steam.ToString()));
    Assert.Equal(
      [localizer.Get(MSG.COMMAND_INVITE_ALREADY_INVITED, toInvite.Name!)],
      TestPlayer.ConsoleOutput);
  }

  [Fact]
  public async Task Invite_Twice() {
    await playerMgr.CreatePlayer(TestPlayer.Steam, TestPlayer.Name);
    var toInvite =
      await playerMgr.CreatePlayer(new Random().NextUInt(), "Invite Me!");
    var gang = await gangMgr.CreateGang("Test Gang", TestPlayer);
    Assert.NotNull(gang);
    var inviteSuccess = localizer.Get(MSG.COMMAND_INVITE_SUCCESS,
      toInvite.Name!, gang.Name);
    Assert.Equal(CommandResult.SUCCESS,
      await Commands.ProcessCommand(TestPlayer, "invite",
        toInvite.Steam.ToString()));

    Assert.NotNull(gang);

    Assert.Equal(CommandResult.SUCCESS,
      await Commands.ProcessCommand(TestPlayer, "invite",
        toInvite.Steam.ToString()));
    Assert.Equal(
    [
      inviteSuccess,
      localizer.Get(MSG.COMMAND_INVITE_ALREADY_INVITED, toInvite.Name!)
    ], TestPlayer.ConsoleOutput);
  }

  [Fact]
  public async Task Invite_Already_In_AGang() {
    await playerMgr.CreatePlayer(TestPlayer.Steam, TestPlayer.Name);
    var gang = await gangMgr.CreateGang("Test Gang", TestPlayer);

    var otherOwner =
      await playerMgr.CreatePlayer(new Random().NextUInt(), "Other Owner");
    var otherGang = await gangMgr.CreateGang("Other Gang", otherOwner);

    Assert.NotNull(gang);
    Assert.NotNull(otherGang);

    Assert.Equal(CommandResult.SUCCESS,
      await Commands.ProcessCommand(TestPlayer, "invite",
        otherOwner.Steam.ToString()));
    Assert.Equal(
      [localizer.Get(MSG.COMMAND_INVITE_ALREADY_IN_GANG, otherOwner.Name!)],
      TestPlayer.ConsoleOutput);
  }

  [Fact]
  public async Task Invite_Already_In_YourGang() {
    await playerMgr.CreatePlayer(TestPlayer.Steam, TestPlayer.Name);
    var gang = await gangMgr.CreateGang("Test Gang", TestPlayer);
    Assert.NotNull(gang);

    var guest = await playerMgr.CreatePlayer(new Random().NextUInt(), "Guest");
    guest.GangId   = gang.GangId;
    guest.GangRank = (await rankMgr.GetJoinRank(gang)).Rank;
    await playerMgr.UpdatePlayer(guest);

    Assert.Equal(CommandResult.SUCCESS,
      await Commands.ProcessCommand(TestPlayer, "invite",
        guest.Steam.ToString()));
    Assert.Equal([localizer.Get(MSG.COMMAND_INVITE_IN_YOUR_GANG, guest.Name!)],
      TestPlayer.ConsoleOutput);
  }
}