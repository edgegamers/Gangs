using Commands.Gang;
using CounterStrikeSharp.API.Modules.Commands;
using GangsAPI;
using GangsAPI.Data;
using GangsAPI.Data.Command;
using GangsAPI.Extensions;

namespace GangsTest.Commands.Gang;

public class CreateTests(IServiceProvider provider)
  : TestParent(provider, new CreateCommand(provider)) {
  [Fact]
  public async Task Create_NonPlayer() {
    var result =
      await Commands.ProcessCommand(null, CommandCallingContext.Console,
        "create");
    Assert.Equal(CommandResult.PLAYER_ONLY, result);
  }

  [Fact]
  public async Task Create_NoName() {
    var result = await Commands.ProcessCommand(TestPlayer,
      CommandCallingContext.Console, "create");
    var expected = Locale.Get(MSG.COMMAND_USAGE, "create " + Command.Usage[0]);
    Assert.Equal(CommandResult.PRINT_USAGE, result);
    Assert.Contains(expected, TestPlayer.ConsoleOutput);
  }

  [Fact]
  public async Task Create_GangIsNull_BeforeExecution() {
    var gang = await Gangs.GetGang(TestPlayer.Steam);
    Assert.Null(gang);
  }

  [Fact]
  public async Task Create_Simple() {
    var result = await Commands.ProcessCommand(TestPlayer,
      CommandCallingContext.Console, "create", "foobar");
    Assert.Equal(CommandResult.SUCCESS, result);
    var gang = await Gangs.GetGang(TestPlayer.Steam);
    Assert.NotNull(gang);
    Assert.Equal("foobar", gang.Name);

    var expected = Locale.Get(MSG.COMMAND_GANG_CREATE_SUCCESS, "foobar",
      gang.GangId);
    Assert.Equal([expected], TestPlayer.ConsoleOutput);
  }

  [Fact]
  public async Task Create_MultiWord() {
    var result = await Commands.ProcessCommand(TestPlayer,
      CommandCallingContext.Console, "create", "foo bar");
    Assert.Equal(CommandResult.SUCCESS, result);
    var gang = await Gangs.GetGang(TestPlayer.Steam);
    Assert.NotNull(gang);
    Assert.Equal("foo bar", gang.Name);
  }

  [Fact]
  public async Task Create_MultiParam() {
    var result = await Commands.ProcessCommand(TestPlayer,
      CommandCallingContext.Console, "create", "foo bar", "baz");
    Assert.Equal(CommandResult.SUCCESS, result);
    var gang = await Gangs.GetGang(TestPlayer.Steam);
    Assert.NotNull(gang);
    Assert.Equal("foo bar baz", gang.Name);
  }

  [Fact]
  public async Task Create_Already_Ganged() {
    var result = await Commands.ProcessCommand(TestPlayer,
      CommandCallingContext.Console, "create", "foo bar");
    Assert.Equal(CommandResult.SUCCESS, result);
    var secondResult = await Commands.ProcessCommand(TestPlayer,
      CommandCallingContext.Console, "create", "bar foo");

    var createdMessage = Locale.Get(MSG.COMMAND_GANG_CREATE_SUCCESS, "foo bar",
      1);
    var alreadyInMessage = Locale.Get(MSG.ALREADY_IN_GANG);

    Assert.Equal(CommandResult.ERROR, secondResult);
    Assert.Equal([createdMessage, alreadyInMessage], TestPlayer.ConsoleOutput);
  }

  [Theory]
  [InlineData("foo bar", "foo bar")]
  [InlineData("foo bar", "FOO bar")]
  [InlineData("foo bar", "foo bar ")]
  [InlineData("foo bar", "FOO bar ")]
  public async Task Create_WithDuplicateName_DoesNotPass(string first,
    string second) {
    var other = new PlayerWrapper(new Random().NextULong(), "Other Player");
    var result = await Commands.ProcessCommand(TestPlayer,
      CommandCallingContext.Console, "create", first);
    Assert.Equal(CommandResult.SUCCESS, result);
    var secondResult = await Commands.ProcessCommand(other,
      CommandCallingContext.Console, "create", second);
    Assert.Equal(CommandResult.ERROR, secondResult);
    var expected = Locale.Get(MSG.COMMAND_GANG_CREATE_ALREADY_EXISTS, first);
    Assert.Equal([expected], other.ConsoleOutput);
  }

  [Theory]
  [InlineData("")]
  [InlineData(" ")]
  [InlineData("\x01")]
  [InlineData("0123456789ABCDEFG")]
  public async Task Create_WithInvalidName_DoesNotPass(string name) {
    var result = await Commands.ProcessCommand(TestPlayer,
      CommandCallingContext.Console, "create", name);
    Assert.Equal(CommandResult.ERROR, result);
    var expected = Locale.Get(MSG.COMMAND_GANG_CREATE_INVALID);
    Assert.Equal([expected], TestPlayer.ConsoleOutput);
  }
}