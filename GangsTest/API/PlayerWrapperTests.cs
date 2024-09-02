using CounterStrikeSharp.API.Modules.Admin;
using FuzzDotNet.Core;
using GangsAPI.Data;

namespace GangsTest.API;

public class PlayerWrapperTests {
  private static char USER_CHAR => PermissionCharacters.UserPermissionChar;
  private static char GROUP_CHAR => PermissionCharacters.GroupPermissionChar;

  private readonly PlayerWrapper testPlayer =
    new((ulong)new Random().NextInt64(), "Test Player");

  [Fact]
  public void PlayerWrapper_Init() {
    Assert.Null(testPlayer.Player);
    Assert.NotNull(testPlayer.Data);
    Assert.True(testPlayer.Data.Identity == testPlayer.Steam.ToString());
  }

  [Fact]
  public void PlayerWrapper_Init_Flags() {
    var player = testPlayer.WithFlags(USER_CHAR + "test/flag");
    Assert.NotNull(player.Data);
    Assert.Single(player.Data.Flags);
    Assert.True(player.Data.Flags.ContainsKey("test"));
  }

  [Fact]
  public void PlayerWrapper_Print_Chat() {
    Assert.Empty(testPlayer.ChatOutput);
    testPlayer.PrintToChat("Test Message");
    Assert.Single(testPlayer.ChatOutput);
    Assert.Equal("Test Message", testPlayer.ChatOutput[0]);
  }

  [Fact]
  public void PlayerWrapper_Print_Console() {
    Assert.Empty(testPlayer.ConsoleOutput);
    testPlayer.PrintToConsole("Test Message");
    Assert.Single(testPlayer.ConsoleOutput);
    Assert.Equal("Test Message", testPlayer.ConsoleOutput[0]);
  }

  [Theory]
  [InlineData("test/flag")]
  [InlineData("test/flag/child")]
  [InlineData("#test/flag")]
  [InlineData("_test/flag")]
  [InlineData("@test")]
  [InlineData("@test/")]
  public void PlayerWrapper_Init_InvalidFlag(string flag) {
    Assert.ThrowsAny<ArgumentException>(() => testPlayer.WithFlags(flag));
  }

  [Theory]
  [InlineData("test/flag")]
  [InlineData("test/flag/child")]
  [InlineData("#test/flag")]
  [InlineData("_test/flag")]
  [InlineData("@test")]
  [InlineData("@test/")]
  public void PlayerWrapper_Test_InvalidFlag(string flag) {
    Assert.ThrowsAny<ArgumentException>(() => testPlayer.HasFlags(flag));
  }

  [Fact]
  public void PlayerWrapper_Perm_Flag() {
    var player = testPlayer.WithFlags(USER_CHAR + "test/flag");
    Assert.True(player.HasFlags(USER_CHAR + "test/flag"));
    Assert.False(player.HasFlags(USER_CHAR + "test/other"));
  }

  [Fact]
  public void PlayerWrapper_Perm_FlagChild() {
    var player = testPlayer.WithFlags(USER_CHAR + "test/flag");
    Assert.True(player.HasFlags(USER_CHAR + "test/flag"));
    Assert.True(player.HasFlags(USER_CHAR + "test/flag/child"));
  }

  [Fact]
  public void PlayerWrapper_Perm_FlagParent() {
    var player = testPlayer.WithFlags(USER_CHAR + "test/flag/child");
    Assert.False(player.HasFlags(USER_CHAR + "test/flag"));
    Assert.True(player.HasFlags(USER_CHAR + "test/flag/child"));
  }

  [Fact]
  public void PlayerWrapper_Perm_FlagRoot() {
    var player = testPlayer.WithFlags(USER_CHAR + "test/root");
    Assert.True(player.HasFlags(USER_CHAR + "test/root"));
    Assert.True(player.HasFlags(USER_CHAR + "test/flag"));
    Assert.True(player.HasFlags(USER_CHAR + "test/other"));
  }

  [Fact]
  public void PlayerWrapper_Perm_FlagRootChild() {
    var player = testPlayer.WithFlags(USER_CHAR + "test/root");
    Assert.True(player.HasFlags(USER_CHAR + "test/root"));
    Assert.True(player.HasFlags(USER_CHAR + "test/flag"));
    Assert.True(player.HasFlags(USER_CHAR + "test/other/test"));
  }
}