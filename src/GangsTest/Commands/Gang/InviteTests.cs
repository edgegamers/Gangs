using Commands.Gang;
using CounterStrikeSharp.API.Modules.Commands;
using GangsAPI;
using GangsAPI.Data;
using GangsAPI.Data.Command;
using GangsAPI.Extensions;
using GangsAPI.Permissions;
using GangsAPI.Services;
using GangsAPI.Services.Gang;
using GangsAPI.Services.Player;
using GangsAPI.Services.Server;
using GangsTest.API.Services.Commands.Command;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Stats.Stat.Gang;

namespace GangsTest.Commands.Gang;

public class InviteTests(IServiceProvider provider)
  : TestParent(provider, new InviteCommand(provider)) {
  private readonly IGangManager gangs =
    provider.GetRequiredService<IGangManager>();

  private readonly IGangStatManager gangStats =
    provider.GetRequiredService<IGangStatManager>();

  private readonly string inviteId = new InvitationStat().StatId;

  private readonly IStringLocalizer localizer =
    provider.GetRequiredService<IStringLocalizer>();

  private readonly IPlayerManager players =
    provider.GetRequiredService<IPlayerManager>();

  private readonly IRankManager ranks =
    provider.GetRequiredService<IRankManager>();

  private readonly IServerProvider server =
    provider.GetRequiredService<IServerProvider>();

  [Fact]
  public async Task Invite_NonPlayer() {
    Assert.Equal(CommandResult.PLAYER_ONLY,
      await Commands.ProcessCommand(null, CommandCallingContext.Chat,
        "invite"));
  }

  [Fact]
  public async Task Invite_NotInGang() {
    Assert.Equal(CommandResult.SUCCESS,
      await Commands.ProcessCommand(TestPlayer, CommandCallingContext.Console,
        "invite", "123"));
    Assert.Contains(localizer.Get(MSG.NOT_IN_GANG), TestPlayer.ConsoleOutput);
  }

  [Fact]
  public async Task Invite_Self() {
    await players.CreatePlayer(TestPlayer.Steam, TestPlayer.Name);
    await gangs.CreateGang("Test Gang", TestPlayer);
    Assert.Equal(CommandResult.SUCCESS,
      await Commands.ProcessCommand(TestPlayer, CommandCallingContext.Console,
        "invite", TestPlayer.Steam.ToString()));
    Assert.Equal(
      [localizer.Get(MSG.COMMAND_INVITE_IN_YOUR_GANG, TestPlayer.Name!)],
      TestPlayer.ConsoleOutput);
  }

  [Fact]
  public async Task Invite_InvalidSteam() {
    await players.CreatePlayer(TestPlayer.Steam, TestPlayer.Name);
    await gangs.CreateGang("Test Gang", TestPlayer);
    Assert.Equal(CommandResult.SUCCESS,
      await Commands.ProcessCommand(TestPlayer, CommandCallingContext.Console,
        "invite", "123"));
    Assert.Equal([localizer.Get(MSG.GENERIC_STEAM_NOT_FOUND, "123")],
      TestPlayer.ConsoleOutput);
  }

  [Fact]
  public async Task Invite_NoPermission() {
    await players.CreatePlayer(TestPlayer.Steam, TestPlayer.Name);
    var gang = await gangs.CreateGang("Test Gang", TestPlayer);
    Assert.NotNull(gang);

    var guest = await players.CreatePlayer(new Random().NextULong(), "Guest");
    guest.GangId   = gang.GangId;
    guest.GangRank = (await ranks.GetJoinRank(gang)).Rank;
    await players.UpdatePlayer(guest);

    var guestWrapper = new PlayerWrapper(guest);

    var needed = await ranks.GetRankNeeded(gang.GangId, Perm.INVITE_OTHERS);

    Assert.NotNull(needed);

    Assert.Equal(CommandResult.NO_PERMISSION,
      await Commands.ProcessCommand(guestWrapper, CommandCallingContext.Console,
        "invite", "123"));
    Assert.Equal([localizer.Get(MSG.GENERIC_NOPERM_RANK, needed.Name)],
      guestWrapper.ConsoleOutput);
  }

  [Fact]
  public async Task Invite_Offline() {
    await players.CreatePlayer(TestPlayer.Steam, TestPlayer.Name);
    var toInvite =
      await players.CreatePlayer(new Random().NextULong(), "Invite Me!");
    var gang = await gangs.CreateGang("Test Gang", TestPlayer);
    Assert.NotNull(gang);

    Assert.Equal(CommandResult.SUCCESS,
      await Commands.ProcessCommand(TestPlayer, CommandCallingContext.Console,
        "invite", toInvite.Steam.ToString()));
    Assert.Equal(
      [localizer.Get(MSG.COMMAND_INVITE_SUCCESS, toInvite.Name!, gang.Name)],
      TestPlayer.ConsoleOutput);
  }

  [Fact]
  public async Task Invite_AlreadyInvited() {
    await players.CreatePlayer(TestPlayer.Steam, TestPlayer.Name);
    var toInvite =
      await players.CreatePlayer(new Random().NextULong(), "Invite Me!");
    var gang = await gangs.CreateGang("Test Gang", TestPlayer);
    Assert.NotNull(gang);

    var data = new InvitationData();
    data.AddInvitation(TestPlayer.Steam, toInvite.Steam);
    await gangStats.SetForGang(gang.GangId, inviteId, data);

    Assert.Equal(CommandResult.SUCCESS,
      await Commands.ProcessCommand(TestPlayer, CommandCallingContext.Console,
        "invite", toInvite.Steam.ToString()));
    Assert.Equal(
      [localizer.Get(MSG.COMMAND_INVITE_ALREADY_INVITED, toInvite.Name!)],
      TestPlayer.ConsoleOutput);
  }

  [Fact]
  public async Task Invite_Twice() {
    await players.CreatePlayer(TestPlayer.Steam, TestPlayer.Name);
    var toInvite =
      await players.CreatePlayer(new Random().NextULong(), "Invite Me!");
    var gang = await gangs.CreateGang("Test Gang", TestPlayer);
    Assert.NotNull(gang);
    var inviteSuccess = localizer.Get(MSG.COMMAND_INVITE_SUCCESS,
      toInvite.Name!, gang.Name);
    Assert.Equal(CommandResult.SUCCESS,
      await Commands.ProcessCommand(TestPlayer, CommandCallingContext.Console,
        "invite", toInvite.Steam.ToString()));

    Assert.NotNull(gang);

    Assert.Equal(CommandResult.SUCCESS,
      await Commands.ProcessCommand(TestPlayer, CommandCallingContext.Console,
        "invite", toInvite.Steam.ToString()));
    Assert.Equal(
    [
      inviteSuccess,
      localizer.Get(MSG.COMMAND_INVITE_ALREADY_INVITED, toInvite.Name!)
    ], TestPlayer.ConsoleOutput);
  }

  [Fact]
  public async Task Invite_Already_In_AGang() {
    await players.CreatePlayer(TestPlayer.Steam, TestPlayer.Name);
    var gang = await gangs.CreateGang("Test Gang", TestPlayer);

    var otherOwner =
      await players.CreatePlayer(new Random().NextULong(), "Other Owner");
    var otherGang = await gangs.CreateGang("Other Gang", otherOwner);

    Assert.NotNull(gang);
    Assert.NotNull(otherGang);

    Assert.Equal(CommandResult.SUCCESS,
      await Commands.ProcessCommand(TestPlayer, CommandCallingContext.Console,
        "invite", otherOwner.Steam.ToString()));
    Assert.Equal(
      [localizer.Get(MSG.COMMAND_INVITE_ALREADY_IN_GANG, otherOwner.Name!)],
      TestPlayer.ConsoleOutput);
  }

  [Fact]
  public async Task Invite_Already_In_YourGang() {
    await players.CreatePlayer(TestPlayer.Steam, TestPlayer.Name);
    var gang = await gangs.CreateGang("Test Gang", TestPlayer);
    Assert.NotNull(gang);

    var guest = await players.CreatePlayer(new Random().NextULong(), "Guest");
    guest.GangId   = gang.GangId;
    guest.GangRank = (await ranks.GetJoinRank(gang)).Rank;
    await players.UpdatePlayer(guest);

    Assert.Equal(CommandResult.SUCCESS,
      await Commands.ProcessCommand(TestPlayer, CommandCallingContext.Console,
        "invite", guest.Steam.ToString()));
    Assert.Equal([localizer.Get(MSG.COMMAND_INVITE_IN_YOUR_GANG, guest.Name!)],
      TestPlayer.ConsoleOutput);
  }

  [Fact]
  public async Task Invite_Online() {
    await players.CreatePlayer(TestPlayer.Steam, TestPlayer.Name);
    var gang = await gangs.CreateGang("Test Gang", TestPlayer);
    Assert.NotNull(gang);

    var guest = await players.CreatePlayer(new Random().NextULong(), "Guest");
    var guestWrapper = new PlayerWrapper(guest);
    await server.AddPlayer(guestWrapper);

    Assert.Equal(CommandResult.SUCCESS,
      await Commands.ProcessCommand(TestPlayer, CommandCallingContext.Console,
        "invite", guest.Name!));
    Assert.Equal(
      [localizer.Get(MSG.COMMAND_INVITE_SUCCESS, guest.Name!, gang.Name)],
      TestPlayer.ConsoleOutput);
  }
}