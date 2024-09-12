using CounterStrikeSharp.API.Modules.Admin;
using GangsAPI.Data;

namespace GangsTest.API.Data;

public class PlayerWrapperTests {
  private static char USER_CHAR => PermissionCharacters.UserPermissionChar;
  private static char GROUP_CHAR => PermissionCharacters.GroupPermissionChar;

  private const string testMessage = "Test Message";

  private readonly PlayerWrapper fakePlayer = TestUtil.CreateFakePlayer();

  [Fact]
  public void WithFlags_Initializes_Properly() {
    fakePlayer.WithFlags(USER_CHAR + "test/flag");
    Assert.NotNull(fakePlayer.Data);
    Assert.Single(fakePlayer.Data.Flags);
    Assert.True(fakePlayer.Data.Flags.ContainsKey("test"));
  }

  [Fact]
  public void PrintToChat_SingleMsg_OutputsProperly() {
    fakePlayer.PrintToChat(testMessage);
    Assert.Equal([testMessage], fakePlayer.ChatOutput);
  }

  [Fact]
  public void PrintToConsole_SingleMsg_OutputsProperly() {
    fakePlayer.PrintToConsole(testMessage);
    Assert.Equal([testMessage], fakePlayer.ConsoleOutput);
  }

  [Fact]
  public void PrintToCenter_SingleMsg_OutputsProperly() {
    fakePlayer.PrintToConsole(testMessage);
    Assert.Equal([testMessage], fakePlayer.ConsoleOutput);
  }

  [Fact]
  public void PrintMultiple_OutputsProperly() {
    fakePlayer.PrintToConsole(testMessage + " A");
    fakePlayer.PrintToChat(testMessage + " B");
    Assert.Equal([testMessage + " A"], fakePlayer.ConsoleOutput);
    Assert.Equal([testMessage + " B"], fakePlayer.ChatOutput);
  }

  [Fact]
  public void PrintToChat_MultiMsg_IsInOrder() {
    fakePlayer.PrintToChat(testMessage + " 1");
    fakePlayer.PrintToChat(testMessage + " 2");
    Assert.Equal([testMessage + " 1", testMessage + " 2"],
      fakePlayer.ChatOutput);
  }

  [Fact]
  public void PrintToConsole_MultiMsg_IsInOrder() {
    fakePlayer.PrintToConsole(testMessage + " 1");
    fakePlayer.PrintToConsole(testMessage + " 2");
    Assert.Equal([testMessage + " 1", testMessage + " 2"],
      fakePlayer.ConsoleOutput);
  }

  [Fact]
  public void PrintToCenter_MultiMsg_IsInOrder() {
    fakePlayer.PrintToCenter(testMessage + " 1");
    fakePlayer.PrintToCenter(testMessage + " 2");
    Assert.Equal([testMessage + " 1", testMessage + " 2"],
      fakePlayer.CenterOutput);
  }

  [Theory]
  [InlineData("test/flag")]
  [InlineData("test/flag/child")]
  [InlineData("#test/flag")]
  [InlineData("_test/flag")]
  [InlineData("@test")]
  [InlineData("@test/")]
  public void WithFlags_WithInvalidFlags_ThrowsArgException(string flag) {
    Assert.ThrowsAny<ArgumentException>(() => fakePlayer.WithFlags(flag));
  }

  [Theory]
  [InlineData("test/flag")]
  [InlineData("test/flag/child")]
  [InlineData("#test/flag")]
  [InlineData("_test/flag")]
  [InlineData("@test")]
  [InlineData("@test/")]
  public void HasFlags_WithInvalidFlags_ThrowsArgException(string flag) {
    Assert.ThrowsAny<ArgumentException>(() => fakePlayer.HasFlags(flag));
  }

  [Fact]
  public void HasFlags_WithSimplePerm_Passes() {
    fakePlayer.WithFlags(USER_CHAR + "test/flag");
    Assert.False(fakePlayer.HasFlags(USER_CHAR + "test/other"));
  }

  [Fact]
  public void HasFlags_WithSimplePerm_DoesNotGrantOther() {
    fakePlayer.WithFlags(USER_CHAR + "test/flag");
    Assert.False(fakePlayer.HasFlags(USER_CHAR + "test/other"));
  }

  [Fact]
  public void HasFlags_WithSimplePerm_GrantsChildPerm() {
    fakePlayer.WithFlags(USER_CHAR + "test/flag");
    Assert.True(fakePlayer.HasFlags(USER_CHAR + "test/flag/child"));
  }

  [Fact]
  public void HasFlags_WithChildPerm_GrantsChildPerm() {
    fakePlayer.WithFlags(USER_CHAR + "test/flag/child");
    Assert.True(fakePlayer.HasFlags(USER_CHAR + "test/flag/child"));
  }

  [Fact]
  public void HasFlags_WithChildPerm_DoesNotGrantParentPerm() {
    fakePlayer.WithFlags(USER_CHAR + "test/flag/child");
    Assert.False(fakePlayer.HasFlags(USER_CHAR + "test/flag"));
  }

  [Fact]
  public void HasFlags_WithRoot_Passes() {
    fakePlayer.WithFlags(USER_CHAR + "test/root");
    Assert.True(fakePlayer.HasFlags(USER_CHAR + "test/other"));
  }

  [Fact]
  public void HasFlags_WithRoot_PassesChildren() {
    fakePlayer.WithFlags(USER_CHAR + "test/root");
    Assert.True(fakePlayer.HasFlags(USER_CHAR + "test/other/test"));
  }

  [Fact]
  public void HasFlags_WithRootChild_DoesNotGrantParentPerm() {
    fakePlayer.WithFlags(USER_CHAR + "test/flag/root");
    Assert.False(fakePlayer.HasFlags(USER_CHAR + "test/flag"));
  }
}