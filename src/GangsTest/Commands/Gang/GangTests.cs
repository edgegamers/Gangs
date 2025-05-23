﻿using Commands;
using Commands.Gang;
using CounterStrikeSharp.API.Modules.Commands;
using GangsAPI;
using GangsAPI.Data.Command;
using GangsAPI.Extensions;
using GangsAPI.Services.Gang;
using GangsAPI.Services.Menu;
using Microsoft.Extensions.DependencyInjection;

namespace GangsTest.Commands.Gang;

public class GangTests(IServiceProvider provider) : TestParent(provider,
  new GangCommand(provider)) {
  private readonly IGangManager gangs =
    provider.GetRequiredService<IGangManager>();

  private readonly IMenuManager menus =
    provider.GetRequiredService<IMenuManager>();

  [Fact]
  public async Task Gang_TestBase() {
    Assert.Equal("css_gang", Command.Name);
    Assert.Equal(CommandResult.SUCCESS,
      await Commands.ProcessCommand(TestPlayer, CommandCallingContext.Chat,
        Command.Name));
  }

  [Fact]
  public async Task Gang_Test_Create() {
    await Eco.Grant(TestPlayer, 2000);
    Assert.Equal(CommandResult.SUCCESS,
      await Commands.ProcessCommand(TestPlayer, CommandCallingContext.Chat,
        Command.Name, "create", "foobar"));
    Assert.Single(await gangs.GetGangs());
  }

  [Fact]
  public async Task Gang_TestInvalid_Name() {
    await Assert.ThrowsAnyAsync<InvalidOperationException>(async () => {
      await Command.Execute(TestPlayer,
        new CommandInfoWrapper(TestPlayer, 0, "foobar"));
    });
  }

  [Fact]
  public async Task Gang_TestInvalid_Null() {
    await Assert.ThrowsAnyAsync<InvalidOperationException>(async () => {
      await Command.Execute(TestPlayer, new CommandInfoWrapper(TestPlayer));
    });
  }

  [Fact]
  public async Task Gang_TestUnknown() {
    Assert.Equal(CommandResult.UNKNOWN_COMMAND,
      await Commands.ProcessCommand(TestPlayer, CommandCallingContext.Chat,
        Command.Name, "foobar"));
  }

  [Fact]
  public async Task Gang_TestHelp() {
    Assert.Equal(CommandResult.SUCCESS,
      await Commands.ProcessCommand(TestPlayer, CommandCallingContext.Chat,
        "css_gang", "help"));
  }

  [Fact]
  public async Task Gang_TestHelp_Single() {
    Assert.Equal(CommandResult.UNKNOWN_COMMAND,
      await Commands.ProcessCommand(TestPlayer, CommandCallingContext.Chat,
        "css_gang help"));
  }

  [Fact]
  public async Task Does_Not_Open_Menu() {
    Assert.Null(menus.GetActiveMenu(TestPlayer));
    Assert.Equal(CommandResult.SUCCESS,
      await Commands.ProcessCommand(TestPlayer, CommandCallingContext.Chat,
        Command.Name));
    Assert.Null(menus.GetActiveMenu(TestPlayer));
  }

  [Fact]
  public async Task Opens_Menu() {
    await gangs.CreateGang("Test Gang", TestPlayer);
    Assert.Null(menus.GetActiveMenu(TestPlayer));
    Assert.Equal(CommandResult.SUCCESS,
      await Commands.ProcessCommand(TestPlayer, CommandCallingContext.Chat,
        Command.Name));
    Assert.NotNull(menus.GetActiveMenu(TestPlayer));
  }

  [Fact]
  public async Task Gang_Transfer_Ownership()
  {
    // Ensure command is registered
    Commands.RegisterCommand(new TransferCommand(provider));
    
    // Create original owner and owned gang
    await Players.CreatePlayer(TestPlayer.Steam, TestPlayer.Name);
    var owner = await Players.GetPlayer(TestPlayer.Steam);
    Assert.NotNull(owner);
    var gang = await Gangs.CreateGang("Test Gang", owner);
    Assert.NotNull(gang);
    Assert.NotNull(owner.GangRank);
    
    // Create player to transfer ownership to
    var newOwner = await Players.CreatePlayer(new Random().NextULong(), "NewOwner");
    newOwner.GangId   = gang.GangId;
    var rank = await Ranks.GetLowerRank(gang.GangId, owner.GangRank.Value);
    Assert.NotNull(rank);
    newOwner.GangRank = rank.Rank;
    await Players.UpdatePlayer(newOwner);
    
    // Transfer ownership
    Assert.Equal(CommandResult.SUCCESS,
      await Commands.ProcessCommand(TestPlayer, CommandCallingContext.Console,
        "transfer", newOwner.Steam.ToString()));
  }
}