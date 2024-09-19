using Commands.Gang;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Entities;
using GangsAPI;
using GangsAPI.Data;
using GangsAPI.Data.Command;
using GangsAPI.Services.Gang;
using GangsTest.API.Services.Commands.Command;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace GangsTest.Commands.Gang;

public class CreateTests(IServiceProvider provider)
  : TestParent(provider, new CreateCommand(provider)) {
  private readonly IGangManager gangs =
    provider.GetRequiredService<IGangManager>();

  private readonly IStringLocalizer locale =
    provider.GetRequiredService<IStringLocalizer>();

  private readonly PlayerWrapper player =
    new PlayerWrapper((ulong)new Random().NextInt64(), "Test Player").WithFlags(
      "@ego/dssilver");

  [Fact]
  public async Task Create_NonPlayer() {
    Assert.Equal(CommandResult.PLAYER_ONLY,
      await Commands.ProcessCommand(null, CommandCallingContext.Console,
        "create"));
  }

  [Fact]
  public async Task Create_NoName() {
    Assert.Equal(CommandResult.PRINT_USAGE,
      await Commands.ProcessCommand(player, CommandCallingContext.Console,
        "create"));
    Assert.Contains(locale.Get(MSG.COMMAND_USAGE, "create " + Command.Usage[0]),
      player.ConsoleOutput);
  }

  [Fact]
  public async Task Create_Simple() {
    var gang = await gangs.GetGang(player.Steam);
    Assert.Null(gang);
    Assert.Equal(CommandResult.SUCCESS,
      await Commands.ProcessCommand(player, CommandCallingContext.Console,
        "create", "foobar"));
    gang = await gangs.GetGang(player.Steam);
    Assert.NotNull(gang);
    Assert.Equal("foobar", gang.Name);
    Assert.Equal(
      [locale.Get(MSG.COMMAND_GANG_CREATE_SUCCESS, "foobar", gang.GangId)],
      player.ConsoleOutput);
  }

  [Fact]
  public async Task Create_MultiWord() {
    var gang = await gangs.GetGang(player.Steam);
    Assert.Null(gang);
    Assert.Equal(CommandResult.SUCCESS,
      await Commands.ProcessCommand(player, CommandCallingContext.Console,
        "create", "foo bar"));
    gang = await gangs.GetGang(player.Steam);
    Assert.NotNull(gang);
    Assert.Equal("foo bar", gang.Name);
  }

  [Fact]
  public async Task Create_MultiParam() {
    var gang = await gangs.GetGang(player.Steam);
    Assert.Null(gang);
    Assert.Equal(CommandResult.SUCCESS,
      await Commands.ProcessCommand(player, CommandCallingContext.Console,
        "create", "foo bar", "baz"));
    gang = await gangs.GetGang(player.Steam);
    Assert.NotNull(gang);
    Assert.Equal("foo bar baz", gang.Name);
  }

  [Fact]
  public async Task Create_Already_Ganged() {
    Assert.Equal(CommandResult.SUCCESS,
      await Commands.ProcessCommand(player, CommandCallingContext.Console,
        "create", "foo bar"));
    Assert.Equal(CommandResult.ERROR,
      await Commands.ProcessCommand(player, CommandCallingContext.Console,
        "create", "bar foo"));
    Assert.Contains(locale.Get(MSG.ALREADY_IN_GANG), player.ConsoleOutput);
  }

  [Fact]
  public async Task Create_Duplicate_Name() {
    var other =
      new PlayerWrapper((ulong)new Random().NextInt64(), "Other Player")
       .WithFlags("@ego/dssilver");
    Assert.Equal(CommandResult.SUCCESS,
      await Commands.ProcessCommand(player, CommandCallingContext.Console,
        "create", "foo bar"));
    Assert.Equal(CommandResult.ERROR,
      await Commands.ProcessCommand(other, CommandCallingContext.Console,
        "create", "foo bar"));
    // Assert.Contains("Gang 'foo bar' already exists", other.ConsoleOutput);
    Assert.Contains(
      locale.Get(MSG.COMMAND_GANG_CREATE_ALREADY_EXISTS, "foo bar"),
      other.ConsoleOutput);
  }
}