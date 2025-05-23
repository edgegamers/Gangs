﻿using Commands.Gang;
using CounterStrikeSharp.API.Modules.Commands;
using GangsAPI;
using GangsAPI.Data;
using GangsAPI.Data.Command;
using GangsAPI.Extensions;
using GangsAPI.Perks;
using GangsAPI.Permissions;
using GangsAPI.Services;
using GangsAPI.Services.Gang;
using GangsAPI.Services.Player;
using GangsAPI.Services.Server;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Stats.Stat.Gang;

namespace GangsTest.Commands.Gang;

public class InviteTests(IServiceProvider provider)
  : TestParent(provider, new InviteCommand(provider)) {
  [Fact]
  public async Task Invite_NonPlayer() {
    Assert.Equal(CommandResult.PLAYER_ONLY,
      await Commands.ProcessCommand(null, CommandCallingContext.Chat,
        "invite"));
  }

  [Fact]
  public async Task Invite_NotInGang() {
    var result = await Commands.ProcessCommand(TestPlayer,
      CommandCallingContext.Console, "invite", "123");
    Assert.Equal(CommandResult.ERROR, result);
    Assert.Contains(Locale.Get(MSG.NOT_IN_GANG), TestPlayer.ConsoleOutput);
  }

  [Fact]
  public async Task Invite_Self() {
    await Players.CreatePlayer(TestPlayer.Steam, TestPlayer.Name);
    await Gangs.CreateGang("Test Gang", TestPlayer);
    Assert.Equal(CommandResult.SUCCESS,
      await Commands.ProcessCommand(TestPlayer, CommandCallingContext.Console,
        "invite", TestPlayer.Steam.ToString()));
    Assert.Equal(
      [Locale.Get(MSG.COMMAND_INVITE_IN_YOUR_GANG, TestPlayer.Name!)],
      TestPlayer.ConsoleOutput);
  }

  [Fact]
  public async Task Invite_InvalidSteam() {
    await Players.CreatePlayer(TestPlayer.Steam, TestPlayer.Name);
    await Gangs.CreateGang("Test Gang", TestPlayer);
    Assert.Equal(CommandResult.SUCCESS,
      await Commands.ProcessCommand(TestPlayer, CommandCallingContext.Console,
        "invite", "123"));
    Assert.Equal([Locale.Get(MSG.GENERIC_STEAM_NOT_FOUND, "123")],
      TestPlayer.ConsoleOutput);
  }

  [Fact]
  public async Task Invite_NoPermission() {
    await Players.CreatePlayer(TestPlayer.Steam, TestPlayer.Name);
    var gang = await Gangs.CreateGang("Test Gang", TestPlayer);
    Assert.NotNull(gang);

    var guest = await Players.CreatePlayer(new Random().NextULong(), "Guest");
    guest.GangId   = gang.GangId;
    guest.GangRank = (await Ranks.GetJoinRank(gang)).Rank;
    await Players.UpdatePlayer(guest);

    var guestWrapper = new PlayerWrapper(guest);

    var needed = await Ranks.GetRankNeeded(gang.GangId, Perm.INVITE_OTHERS);

    Assert.NotNull(needed);

    Assert.Equal(CommandResult.NO_PERMISSION,
      await Commands.ProcessCommand(guestWrapper, CommandCallingContext.Console,
        "invite", "123"));
    Assert.Equal([Locale.Get(MSG.GENERIC_NOPERM_RANK, needed.Name)],
      guestWrapper.ConsoleOutput);
  }

  [Fact]
  public async Task Invite_Offline() {
    await Players.CreatePlayer(TestPlayer.Steam, TestPlayer.Name);
    var toInvite =
      await Players.CreatePlayer(new Random().NextULong(), "Invite Me!");
    var gang = await Gangs.CreateGang("Test Gang", TestPlayer);
    Assert.NotNull(gang);

    Assert.Equal(CommandResult.SUCCESS,
      await Commands.ProcessCommand(TestPlayer, CommandCallingContext.Console,
        "invite", toInvite.Steam.ToString()));
    Assert.Equal(
      [Locale.Get(MSG.COMMAND_INVITE_SUCCESS, toInvite.Name!, gang.Name)],
      TestPlayer.ConsoleOutput);
  }

  [Fact]
  public async Task Invite_AlreadyInvited() {
    await Players.CreatePlayer(TestPlayer.Steam, TestPlayer.Name);
    var toInvite =
      await Players.CreatePlayer(new Random().NextULong(), "Invite Me!");
    var gang = await Gangs.CreateGang("Test Gang", TestPlayer);
    Assert.NotNull(gang);

    var data = new InvitationData();
    data.AddInvitation(TestPlayer.Steam, toInvite.Steam);
    await GangStats.SetForGang(gang.GangId, InvitationStat.STAT_ID, data);

    Assert.Equal(CommandResult.SUCCESS,
      await Commands.ProcessCommand(TestPlayer, CommandCallingContext.Console,
        "invite", toInvite.Steam.ToString()));
    Assert.Equal(
      [Locale.Get(MSG.COMMAND_INVITE_ALREADY_INVITED, toInvite.Name!)],
      TestPlayer.ConsoleOutput);
  }

  [Fact]
  public async Task Invite_Twice() {
    await Players.CreatePlayer(TestPlayer.Steam, TestPlayer.Name);
    var toInvite =
      await Players.CreatePlayer(new Random().NextULong(), "Invite Me!");
    var gang = await Gangs.CreateGang("Test Gang", TestPlayer);
    Assert.NotNull(gang);
    var inviteSuccess = Locale.Get(MSG.COMMAND_INVITE_SUCCESS,
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
      Locale.Get(MSG.COMMAND_INVITE_ALREADY_INVITED, toInvite.Name!)
    ], TestPlayer.ConsoleOutput);
  }

  [Fact]
  public async Task Invite_Already_In_AGang() {
    await Players.CreatePlayer(TestPlayer.Steam, TestPlayer.Name);
    var gang = await Gangs.CreateGang("Test Gang", TestPlayer);

    var otherOwner =
      await Players.CreatePlayer(new Random().NextULong(), "Other Owner");
    var otherGang = await Gangs.CreateGang("Other Gang", otherOwner);

    Assert.NotNull(gang);
    Assert.NotNull(otherGang);

    Assert.Equal(CommandResult.SUCCESS,
      await Commands.ProcessCommand(TestPlayer, CommandCallingContext.Console,
        "invite", otherOwner.Steam.ToString()));
    Assert.Equal(
      [Locale.Get(MSG.COMMAND_INVITE_ALREADY_IN_GANG, otherOwner.Name!)],
      TestPlayer.ConsoleOutput);
  }

  [Fact]
  public async Task Invite_Already_In_YourGang() {
    await Players.CreatePlayer(TestPlayer.Steam, TestPlayer.Name);
    var gang = await Gangs.CreateGang("Test Gang", TestPlayer);
    Assert.NotNull(gang);

    var guest = await Players.CreatePlayer(new Random().NextULong(), "Guest");
    guest.GangId   = gang.GangId;
    guest.GangRank = (await Ranks.GetJoinRank(gang)).Rank;
    await Players.UpdatePlayer(guest);

    Assert.Equal(CommandResult.SUCCESS,
      await Commands.ProcessCommand(TestPlayer, CommandCallingContext.Console,
        "invite", guest.Steam.ToString()));
    Assert.Equal([Locale.Get(MSG.COMMAND_INVITE_IN_YOUR_GANG, guest.Name!)],
      TestPlayer.ConsoleOutput);
  }

  [Fact]
  public async Task Invite_Online() {
    await Players.CreatePlayer(TestPlayer.Steam, TestPlayer.Name);
    var gang = await Gangs.CreateGang("Test Gang", TestPlayer);
    Assert.NotNull(gang);

    var guest = await Players.CreatePlayer(new Random().NextULong(), "Guest");
    var guestWrapper = new PlayerWrapper(guest);
    await Server.AddPlayer(guestWrapper);

    Assert.Equal(CommandResult.SUCCESS,
      await Commands.ProcessCommand(TestPlayer, CommandCallingContext.Console,
        "invite", guest.Name!));
    Assert.Equal(
      [Locale.Get(MSG.COMMAND_INVITE_SUCCESS, guest.Name!, gang.Name)],
      TestPlayer.ConsoleOutput);
  }

  [Fact]
  public async Task Invite_Cancel_When_Max_Capacity() {
    // Create executor and gang
    await Players.CreatePlayer(TestPlayer.Steam, TestPlayer.Name);
    var gang = await Gangs.CreateGang("Test Gang", TestPlayer);
    Assert.NotNull(gang);
    
    // Send player that we want to cancel the invite to an invite
    var playerToCancel = await Players.CreatePlayer(new Random().NextULong(), "PlayerToCancel");
    await Players.UpdatePlayer(playerToCancel);
    Assert.Equal(CommandResult.SUCCESS,
      await Commands.ProcessCommand(TestPlayer, CommandCallingContext.Console,
        "invite", playerToCancel.Steam.ToString()));
    
    // Assume gang capacity and current member count
    var capacity = 20;
    var members  = 1;
    
    // Fill gang to capacity starting at the current member count
    for (int i = members; i < capacity; ++i) {
      var guest = await Players.CreatePlayer(new Random().NextULong(), "Guest" + i);
      guest.GangId   = gang.GangId;
      guest.GangRank = (await Ranks.GetJoinRank(gang)).Rank;
      await Players.UpdatePlayer(guest);
    }
 
    // Attempt to cancel invite to player
    Assert.Equal(CommandResult.SUCCESS,
      await Commands.ProcessCommand(TestPlayer, CommandCallingContext.Console,
        "invite", "cancel", playerToCancel.Name!));
  }
}