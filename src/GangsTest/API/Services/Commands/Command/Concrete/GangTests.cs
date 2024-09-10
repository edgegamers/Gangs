using Commands;
using GangsAPI.Data.Command;
using GangsAPI.Services;
using GangsAPI.Services.Commands;
using GangsAPI.Services.Gang;
using GangsAPI.Services.Menu;
using GangsAPI.Services.Player;
using Microsoft.Extensions.Localization;

namespace GangsTest.API.Services.Commands.Command.Concrete;

public class GangTests(ICommandManager commands, IGangManager gangMgr,
  IPlayerManager playerMgr, IMenuManager menuMgr, IRankManager rankMgr,
  IStringLocalizer locale) : TestParent(commands,
  new GangCommand(gangMgr, playerMgr, menuMgr, rankMgr, locale)) {
  [Fact]
  public async Task Gang_TestBase() {
    Assert.Equal("css_gang", Command.Name);
    Assert.Equal(CommandResult.SUCCESS,
      await Commands.ProcessCommand(TestPlayer, Command.Name));
  }

  [Fact]
  public async Task Gang_Test_Create() {
    Assert.Equal(CommandResult.SUCCESS,
      await Commands.ProcessCommand(TestPlayer, Command.Name, "create",
        "foobar"));
    Assert.Single(await gangMgr.GetGangs());
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
      await Commands.ProcessCommand(TestPlayer, Command.Name, "foobar"));
  }

  [Fact]
  public async Task Gang_TestHelp() {
    Assert.Equal(CommandResult.SUCCESS,
      await Commands.ProcessCommand(TestPlayer, "css_gang", "help"));
  }

  [Fact]
  public async Task Gang_TestHelp_Single() {
    Assert.Equal(CommandResult.UNKNOWN_COMMAND,
      await Commands.ProcessCommand(TestPlayer, "css_gang help"));
  }

  [Fact]
  public async Task Does_Not_Open_Menu() {
    Assert.Null(menuMgr.GetActiveMenu(TestPlayer));
    Assert.Equal(CommandResult.SUCCESS,
      await Commands.ProcessCommand(TestPlayer, Command.Name));
    Assert.Null(menuMgr.GetActiveMenu(TestPlayer));
  }

  [Fact]
  public async Task Opens_Menu() {
    await gangMgr.CreateGang("Test Gang", TestPlayer);
    Assert.Null(menuMgr.GetActiveMenu(TestPlayer));
    Assert.Equal(CommandResult.SUCCESS,
      await Commands.ProcessCommand(TestPlayer, Command.Name));
    Assert.NotNull(menuMgr.GetActiveMenu(TestPlayer));
  }
}