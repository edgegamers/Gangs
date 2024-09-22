using Commands;
using CounterStrikeSharp.API.Modules.Commands;
using GangsAPI.Data.Command;
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
    Assert.Equal(CommandResult.SUCCESS,
      await Commands.ProcessCommand(TestPlayer.WithFlags("@ego/dssilver"), CommandCallingContext.Chat,
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
}