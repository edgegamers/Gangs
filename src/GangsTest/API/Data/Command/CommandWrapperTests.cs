using GangsAPI.Data;
using GangsAPI.Data.Command;
using GangsAPI.Extensions;

namespace GangsTest.API.Data.Command;

public class CommandWrapperTests {
  private const string fakePlayerName = "Test Player";
  private const string testCommand_Single = "test";
  private const string testCommand_Multi = "test ing";
  private static readonly string[] testCommand_Params = ["a", "b", "c"];

  private static readonly string[] testCommand_Gang = [
    "css_gang", "create", "foobar"
  ];

  // private readonly PlayerWrapper fakePlayer =
  //   new(new Random().NextULong(), fakePlayerName);

  [Fact]
  public void Constructor_WithPlayerWrapper_AssignsPlayer() {
    var player = TestUtil.CreateFakePlayer();
    var info   = new CommandInfoWrapper(player, 0, testCommand_Single);
    Assert.NotNull(info.CallingPlayer);
  }

  [Fact]
  public void Constructor_WithPlayerWrapper_AssignsSteam() {
    var player = TestUtil.CreateFakePlayer();
    var info   = new CommandInfoWrapper(player, 0, testCommand_Single);
    Assert.NotNull(info.CallingPlayer);
    Assert.Equal(player.Steam, info.CallingPlayer.Steam);
  }

  [Fact]
  public void Constructor_WithPlayerWrapper_AssignsName() {
    var player = TestUtil.CreateFakePlayer();
    var info   = new CommandInfoWrapper(player, 0, testCommand_Single);
    Assert.NotNull(info.CallingPlayer);
    Assert.Equal(fakePlayerName, info.CallingPlayer.Name);
  }

  [Fact]
  public void Constructor_WithConsole_AssignsNullFields() {
    var info = new CommandInfoWrapper(null, 0, testCommand_Single);
    Assert.Null(info.CallingPlayer);
  }

  [Fact]
  public void Constructor_WithWrapper_OffsetsArgCount() {
    var player  = TestUtil.CreateFakePlayer();
    var info    = new CommandInfoWrapper(player, 0, testCommand_Params);
    var wrapped = new CommandInfoWrapper(player, 1, info.Args);
    Assert.Equal(testCommand_Params.Length, info.ArgCount);
    Assert.Equal(testCommand_Params.Length - 1, wrapped.ArgCount);
  }

  [Fact]
  public void Constructor_WithWrapper_OffsetsArgs() {
    var player  = TestUtil.CreateFakePlayer();
    var info    = new CommandInfoWrapper(player, 0, testCommand_Params);
    var wrapped = new CommandInfoWrapper(player, 1, info.Args);
    Assert.Equal(testCommand_Params.Length, info.ArgCount);
    Assert.Equal(testCommand_Params.Skip(1), wrapped.Args);
  }

  [Fact]
  public void Constructor_WithParams_OffsetsArgCount() {
    var player  = TestUtil.CreateFakePlayer();
    var info    = new CommandInfoWrapper(player, 1, testCommand_Params);
    var wrapped = new CommandInfoWrapper(player, 1, info.Args);
    Assert.Equal(testCommand_Params.Skip(1).Count(), info.ArgCount);
    Assert.Equal(testCommand_Params.Skip(2).Count(), wrapped.ArgCount);
  }

  [Fact]
  public void Constructor_WithParams_OffsetsArgs() {
    var player  = TestUtil.CreateFakePlayer();
    var info    = new CommandInfoWrapper(player, 1, testCommand_Params);
    var wrapped = new CommandInfoWrapper(player, 1, info.Args);
    Assert.Equal(2, info.ArgCount);
    Assert.Equal(testCommand_Params[2], wrapped[0]);
  }

  [Fact]
  public void Constructor_WithSingleParam_SetsArgCountToOne() {
    var player = TestUtil.CreateFakePlayer();
    var info   = new CommandInfoWrapper(player, 0, testCommand_Single);
    Assert.Equal(1, info.ArgCount);
  }

  [Fact]
  public void Constructor_WithSingleParamSpaced_SetsArgCountToOne() {
    var player = TestUtil.CreateFakePlayer();
    var info   = new CommandInfoWrapper(player, 0, testCommand_Multi);
    Assert.Equal(1, info.ArgCount);
  }

  [Fact]
  public void Constructor_WithOffsetAndSingleParam_SetsArgCountToZero() {
    var player = TestUtil.CreateFakePlayer();
    var info   = new CommandInfoWrapper(player, 1, testCommand_Single);
    Assert.Equal(0, info.ArgCount);
  }

  [Fact]
  public void Constructor_WithOffsetAndSingleParamSpaced_SetsArgCountToZero() {
    var player = TestUtil.CreateFakePlayer();
    var info   = new CommandInfoWrapper(player, 1, testCommand_Multi);
    Assert.Equal(0, info.ArgCount);
  }

  [Fact]
  public void Constructor_WithOffsetAndParams_SetsArgCountToOne() {
    var player = TestUtil.CreateFakePlayer();
    var info   = new CommandInfoWrapper(player, 2, testCommand_Params);
    Assert.Equal(1, info.ArgCount);
  }

  [Theory]
  [InlineData(0, 0)]
  [InlineData(0, 1)]
  [InlineData(0, 2)]
  [InlineData(1, 0)]
  [InlineData(1, 1)]
  [InlineData(2, 0)]
  public void Getter_WithOffset_ReturnsExpectedIndex(int offset, int index) {
    var player = TestUtil.CreateFakePlayer();
    var info   = new CommandInfoWrapper(player, offset, testCommand_Gang);
    Assert.Equal(testCommand_Gang[index + offset], info[index]);
  }

  [Theory]
  [InlineData(0)]
  [InlineData(1)]
  [InlineData(2)]
  public void CommandString_WithOffset_ReturnsCorrectString(int index) {
    var player   = TestUtil.CreateFakePlayer();
    var info     = new CommandInfoWrapper(player, index, testCommand_Gang);
    var expected = string.Join(" ", testCommand_Gang.Skip(index));
    Assert.Equal(expected, info.GetCommandString);
  }
}