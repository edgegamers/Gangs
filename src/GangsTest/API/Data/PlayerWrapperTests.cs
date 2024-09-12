using CounterStrikeSharp.API.Modules.Admin;

namespace GangsTest.API.Data;

public class PlayerWrapperTests {
  private static char USER_CHAR => PermissionCharacters.UserPermissionChar;
  private static char GROUP_CHAR => PermissionCharacters.GroupPermissionChar;

  private const string testMessage = "Test Message";

  [Fact]
  public void WithFlags_Initializes_Properly() {
    var player = TestUtil.CreateFakePlayer();
    player.WithFlags(USER_CHAR + "test/flag");
    Assert.NotNull(player.Data);
    Assert.Single(player.Data.Flags);
    Assert.True(player.Data.Flags.ContainsKey("test"));
  }

  [Fact]
  public void PrintToChat_SingleMsg_OutputsProperly() {
    var player = TestUtil.CreateFakePlayer();
    player.PrintToChat(testMessage);
    Assert.Equal([testMessage], player.ChatOutput);
  }

  [Fact]
  public void PrintToConsole_SingleMsg_OutputsProperly() {
    var player = TestUtil.CreateFakePlayer();
    player.PrintToConsole(testMessage);
    Assert.Equal([testMessage], player.ConsoleOutput);
  }

  [Fact]
  public void PrintToCenter_SingleMsg_OutputsProperly() {
    var player = TestUtil.CreateFakePlayer();
    player.PrintToConsole(testMessage);
    Assert.Equal([testMessage], player.ConsoleOutput);
  }

  [Fact]
  public void PrintMultiple_OutputsProperly() {
    var player = TestUtil.CreateFakePlayer();
    player.PrintToConsole(testMessage + " A");
    player.PrintToChat(testMessage + " B");
    Assert.Equal([testMessage + " A"], player.ConsoleOutput);
    Assert.Equal([testMessage + " B"], player.ChatOutput);
  }

  [Fact]
  public void PrintToChat_MultiMsg_IsInOrder() {
    var player = TestUtil.CreateFakePlayer();
    player.PrintToChat(testMessage + " 1");
    player.PrintToChat(testMessage + " 2");
    Assert.Equal([testMessage + " 1", testMessage + " 2"], player.ChatOutput);
  }

  [Fact]
  public void PrintToConsole_MultiMsg_IsInOrder() {
    var player = TestUtil.CreateFakePlayer();
    player.PrintToConsole(testMessage + " 1");
    player.PrintToConsole(testMessage + " 2");
    Assert.Equal([testMessage + " 1", testMessage + " 2"],
      player.ConsoleOutput);
  }

  [Fact]
  public void PrintToCenter_MultiMsg_IsInOrder() {
    var player = TestUtil.CreateFakePlayer();
    player.PrintToCenter(testMessage + " 1");
    player.PrintToCenter(testMessage + " 2");
    Assert.Equal([testMessage + " 1", testMessage + " 2"], player.CenterOutput);
  }

  [Theory]
  [InlineData("test/flag")]
  [InlineData("test/flag/child")]
  [InlineData("#test/flag")]
  [InlineData("_test/flag")]
  [InlineData("@test")]
  [InlineData("@test/")]
  public void WithFlags_WithInvalidFlags_ThrowsArgException(string flag) {
    var player = TestUtil.CreateFakePlayer();
    Assert.ThrowsAny<ArgumentException>(() => player.WithFlags(flag));
  }

  [Theory]
  [InlineData("test/flag")]
  [InlineData("test/flag/child")]
  [InlineData("#test/flag")]
  [InlineData("_test/flag")]
  [InlineData("@test")]
  [InlineData("@test/")]
  public void HasFlags_WithInvalidFlags_ThrowsArgException(string flag) {
    var player = TestUtil.CreateFakePlayer();
    Assert.ThrowsAny<ArgumentException>(() => player.HasFlags(flag));
  }

  [Fact]
  public void HasFlags_WithSimplePerm_Passes() {
    var player = TestUtil.CreateFakePlayer();
    player.WithFlags(USER_CHAR + "test/flag");
    Assert.False(player.HasFlags(USER_CHAR + "test/other"));
  }

  [Fact]
  public void HasFlags_WithSimplePerm_DoesNotGrantOther() {
    var player = TestUtil.CreateFakePlayer();
    player.WithFlags(USER_CHAR + "test/flag");
    Assert.False(player.HasFlags(USER_CHAR + "test/other"));
  }

  [Fact]
  public void HasFlags_WithSimplePerm_GrantsChildPerm() {
    var player = TestUtil.CreateFakePlayer();
    player.WithFlags(USER_CHAR + "test/flag");
    Assert.True(player.HasFlags(USER_CHAR + "test/flag/child"));
  }

  [Fact]
  public void HasFlags_WithChildPerm_GrantsChildPerm() {
    var player = TestUtil.CreateFakePlayer();
    player.WithFlags(USER_CHAR + "test/flag/child");
    Assert.True(player.HasFlags(USER_CHAR + "test/flag/child"));
  }

  [Fact]
  public void HasFlags_WithChildPerm_DoesNotGrantParentPerm() {
    var player = TestUtil.CreateFakePlayer();
    player.WithFlags(USER_CHAR + "test/flag/child");
    Assert.False(player.HasFlags(USER_CHAR + "test/flag"));
  }

  [Fact]
  public void HasFlags_WithRoot_Passes() {
    var player = TestUtil.CreateFakePlayer();
    player.WithFlags(USER_CHAR + "test/root");
    Assert.True(player.HasFlags(USER_CHAR + "test/other"));
  }

  [Fact]
  public void HasFlags_WithRoot_PassesChildren() {
    var player = TestUtil.CreateFakePlayer();
    player.WithFlags(USER_CHAR + "test/root");
    Assert.True(player.HasFlags(USER_CHAR + "test/other/test"));
  }

  [Fact]
  public void HasFlags_WithRootChild_DoesNotGrantParentPerm() {
    var player = TestUtil.CreateFakePlayer();
    player.WithFlags(USER_CHAR + "test/flag/root");
    Assert.False(player.HasFlags(USER_CHAR + "test/flag"));
  }
}