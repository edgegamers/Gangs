using Commands.Gang;
using GangsAPI;
using GangsAPI.Data;
using GangsAPI.Data.Command;
using GangsAPI.Services.Commands;
using GangsAPI.Services.Gang;
using GangsTest.API.Services.Commands.Command;
using Microsoft.Extensions.Localization;

namespace GangsTest.Commands.Gang;

public class CreateTests(ICommandManager commands, IGangManager gangs,
  IStringLocalizer locale)
  : TestParent(commands, new CreateCommand(gangs, locale)) {
  private readonly PlayerWrapper player = new((ulong)new Random().NextInt64(),
    "Test Player");

  [Fact]
  public async Task Create_NonPlayer() {
    Assert.Equal(CommandResult.PLAYER_ONLY,
      await Commands.ProcessCommand(null, "create"));
  }

  [Fact]
  public async Task Create_NoName() {
    Assert.Equal(CommandResult.PRINT_USAGE,
      await Commands.ProcessCommand(player, "create"));
    Assert.Contains(locale.Get(MSG.COMMAND_USAGE, "create " + Command.Usage[0]),
      player.ConsoleOutput);
  }

  [Fact]
  public async Task Create_Simple() {
    var gang = await gangs.GetGang(player.Steam);
    Assert.Null(gang);
    Assert.Equal(CommandResult.SUCCESS,
      await Commands.ProcessCommand(player, "create", "foobar"));
    gang = await gangs.GetGang(player.Steam);
    Assert.NotNull(gang);
    Assert.Equal("foobar", gang.Name);
    Assert.Contains($"Gang 'foobar' (#{gang.GangId}) created successfully",
      player.ConsoleOutput);
  }

  [Fact]
  public async Task Create_MultiWord() {
    var gang = await gangs.GetGang(player.Steam);
    Assert.Null(gang);
    Assert.Equal(CommandResult.SUCCESS,
      await Commands.ProcessCommand(player, "create", "foo bar"));
    gang = await gangs.GetGang(player.Steam);
    Assert.NotNull(gang);
    Assert.Equal("foo bar", gang.Name);
  }

  [Fact]
  public async Task Create_MultiParam() {
    var gang = await gangs.GetGang(player.Steam);
    Assert.Null(gang);
    Assert.Equal(CommandResult.SUCCESS,
      await Commands.ProcessCommand(player, "create", "foo bar", "baz"));
    gang = await gangs.GetGang(player.Steam);
    Assert.NotNull(gang);
    Assert.Equal("foo bar baz", gang.Name);
  }

  [Fact]
  public async Task Create_Already_Ganged() {
    Assert.Equal(CommandResult.SUCCESS,
      await Commands.ProcessCommand(player, "create", "foo bar"));
    Assert.Equal(CommandResult.ERROR,
      await Commands.ProcessCommand(player, "create", "bar foo"));
    Assert.Contains(locale.Get(MSG.ALREADY_IN_GANG), player.ConsoleOutput);
  }

  [Fact]
  public async Task Create_Duplicate_Name() {
    var other =
      new PlayerWrapper((ulong)new Random().NextInt64(), "Other Player");
    Assert.Equal(CommandResult.SUCCESS,
      await Commands.ProcessCommand(player, "create", "foo bar"));
    Assert.Equal(CommandResult.ERROR,
      await Commands.ProcessCommand(other, "create", "foo bar"));
    // Assert.Contains("Gang 'foo bar' already exists", other.ConsoleOutput);
    Assert.Contains(
      locale.Get(MSG.COMMAND_GANG_CREATE_ALREADY_EXISTS, "foo bar"),
      other.ConsoleOutput);
  }
}