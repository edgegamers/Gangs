using Commands.Gang;
using GangsAPI.Data;
using GangsAPI.Data.Command;
using GangsAPI.Services.Commands;
using GangsAPI.Services.Gang;

namespace GangsTest.API.Services.Commands.Command.Concrete;

public class CreateTests(ICommandManager commands, IGangManager gangMgr)
  : TestParent(commands, new CreateCommand(gangMgr)) {
  private readonly PlayerWrapper player = new((ulong)new Random().NextInt64(),
    "Test Player");

  [Fact]
  public async Task Create_NonPlayer() {
    Assert.Equal(CommandResult.PLAYER_ONLY,
      await Commands.ProcessCommand(null, "create"));
  }

  [Fact]
  public async Task Create_NoName() {
    Assert.Equal(CommandResult.INVALID_ARGS,
      await Commands.ProcessCommand(player, "create"));
    Assert.Contains("Please provide a name for the gang", player.ConsoleOutput);
  }

  [Fact]
  public async Task Create_Simple() {
    var gang = await gangMgr.GetGang(player.Steam);
    Assert.Null(gang);
    Assert.Equal(CommandResult.SUCCESS,
      await Commands.ProcessCommand(player, "create", "foobar"));
    gang = await gangMgr.GetGang(player.Steam);
    Assert.NotNull(gang);
    Assert.Equal("foobar", gang.Name);
    Assert.Contains($"Gang 'foobar' (#{gang.GangId}) created successfully",
      player.ConsoleOutput);
  }

  [Fact]
  public async Task Create_MultiWord() {
    var gang = await gangMgr.GetGang(player.Steam);
    Assert.Null(gang);
    Assert.Equal(CommandResult.SUCCESS,
      await Commands.ProcessCommand(player, "create", "foo bar"));
    gang = await gangMgr.GetGang(player.Steam);
    Assert.NotNull(gang);
    Assert.Equal("foo bar", gang.Name);
  }

  [Fact]
  public async Task Create_MultiParam() {
    var gang = await gangMgr.GetGang(player.Steam);
    Assert.Null(gang);
    Assert.Equal(CommandResult.SUCCESS,
      await Commands.ProcessCommand(player, "create", "foo bar", "baz"));
    gang = await gangMgr.GetGang(player.Steam);
    Assert.NotNull(gang);
    Assert.Equal("foo bar baz", gang.Name);
  }

  [Fact]
  public async Task Create_Already_Ganged() {
    Assert.Equal(CommandResult.SUCCESS,
      await Commands.ProcessCommand(player, "create", "foo bar"));
    Assert.Equal(CommandResult.FAILURE,
      await Commands.ProcessCommand(player, "create", "bar foo"));
    Assert.Contains("You are already in a gang", player.ConsoleOutput);
  }

  [Fact]
  public async Task Create_Already_Ganged_Uncached() {
    Assert.Equal(CommandResult.SUCCESS,
      await Commands.ProcessCommand(player, "create", "foo bar"));
    Assert.Equal(CommandResult.FAILURE,
      await Commands.ProcessCommand(player, "create", "bar foo"));
    Assert.Contains("You are already in a gang", player.ConsoleOutput);
  }

  [Fact]
  public async Task Create_Duplicate_Name() {
    var other =
      new PlayerWrapper((ulong)new Random().NextInt64(), "Other Player");
    Assert.Equal(CommandResult.SUCCESS,
      await Commands.ProcessCommand(player, "create", "foo bar"));
    Assert.Equal(CommandResult.FAILURE,
      await Commands.ProcessCommand(other, "create", "foo bar"));
    Assert.Contains("Gang 'foo bar' already exists", other.ConsoleOutput);
  }
}