﻿using CounterStrikeSharp.API.Modules.Admin;
using GangsAPI.Data;

namespace GangsTest.API.Data;

public class PlayerWrapperTests {
  private readonly PlayerWrapper testPlayer =
    new((ulong)new Random().NextInt64(), "Test Player");

  private static char USER_CHAR => PermissionCharacters.UserPermissionChar;
  private static char GROUP_CHAR => PermissionCharacters.GroupPermissionChar;

  [Fact]
  public void Fields_Initialized() {
    Assert.Null(testPlayer.Player);
    Assert.NotNull(testPlayer.Data);
    Assert.True(testPlayer.Data.Identity == testPlayer.Steam.ToString());
    Assert.Empty(testPlayer.ChatOutput);
    Assert.Empty(testPlayer.ConsoleOutput);
  }

  [Fact]
  public void Flags_Initialized() {
    var player = testPlayer.WithFlags(USER_CHAR + "test/flag");
    Assert.NotNull(player.Data);
    Assert.Single(player.Data.Flags);
    Assert.True(player.Data.Flags.ContainsKey("test"));
  }

  [Fact]
  public void Output_Chat_Single() {
    testPlayer.PrintToChat("Test Message");
    Assert.Single(testPlayer.ChatOutput);
    Assert.Equal("Test Message", testPlayer.ChatOutput[0]);
  }

  [Fact]
  public void Output_Console_Single() {
    testPlayer.PrintToConsole("Test Message");
    Assert.Single(testPlayer.ConsoleOutput);
    Assert.Equal("Test Message", testPlayer.ConsoleOutput[0]);
  }

  [Fact]
  public void Output_Both_Single() {
    testPlayer.PrintToConsole("Test Message A");
    testPlayer.PrintToChat("Test Message B");
    Assert.Single(testPlayer.ConsoleOutput);
    Assert.Single(testPlayer.ChatOutput);
    Assert.Equal("Test Message A", testPlayer.ConsoleOutput[0]);
    Assert.Equal("Test Message B", testPlayer.ChatOutput[0]);
  }

  [Fact]
  public void Output_Chat_Multi() {
    testPlayer.PrintToChat("Test Message 1");
    testPlayer.PrintToChat("Test Message 2");
    Assert.Equal(2, testPlayer.ChatOutput.Count);
    Assert.Equal("Test Message 1", testPlayer.ChatOutput[0]);
    Assert.Equal("Test Message 2", testPlayer.ChatOutput[1]);
  }

  [Fact]
  public void Output_Console_Multi() {
    testPlayer.PrintToConsole("Test Message 1");
    testPlayer.PrintToConsole("Test Message 2");
    Assert.Equal(2, testPlayer.ConsoleOutput.Count);
    Assert.Equal("Test Message 1", testPlayer.ConsoleOutput[0]);
    Assert.Equal("Test Message 2", testPlayer.ConsoleOutput[1]);
  }

  [Theory]
  [InlineData("test/flag")]
  [InlineData("test/flag/child")]
  [InlineData("#test/flag")]
  [InlineData("_test/flag")]
  [InlineData("@test")]
  [InlineData("@test/")]
  public void WithInvalidFlag_Throws(string flag) {
    Assert.ThrowsAny<ArgumentException>(() => testPlayer.WithFlags(flag));
  }

  [Theory]
  [InlineData("test/flag")]
  [InlineData("test/flag/child")]
  [InlineData("#test/flag")]
  [InlineData("_test/flag")]
  [InlineData("@test")]
  [InlineData("@test/")]
  public void HasInvalidFlag_Throws(string flag) {
    Assert.ThrowsAny<ArgumentException>(() => testPlayer.HasFlags(flag));
  }

  [Fact]
  public void Permission_Pass() {
    var player = testPlayer.WithFlags(USER_CHAR + "test/flag");
    Assert.True(player.HasFlags(USER_CHAR + "test/flag"));
    Assert.False(player.HasFlags(USER_CHAR + "test/other"));
  }

  [Fact]
  public void Permission_Pass_Child() {
    var player = testPlayer.WithFlags(USER_CHAR + "test/flag");
    Assert.True(player.HasFlags(USER_CHAR + "test/flag"));
    Assert.True(player.HasFlags(USER_CHAR + "test/flag/child"));
  }

  [Fact]
  public void Permission_Pass_Strict_Child() {
    var player = testPlayer.WithFlags(USER_CHAR + "test/flag/child");
    Assert.False(player.HasFlags(USER_CHAR + "test/flag"));
    Assert.True(player.HasFlags(USER_CHAR + "test/flag/child"));
  }

  [Fact]
  public void Permission_Pass_Root() {
    var player = testPlayer.WithFlags(USER_CHAR + "test/root");
    Assert.True(player.HasFlags(USER_CHAR + "test/root"));
    Assert.True(player.HasFlags(USER_CHAR + "test/flag"));
    Assert.True(player.HasFlags(USER_CHAR + "test/other"));
  }

  [Fact]
  public void Permission_Pass_Strict_Root() {
    var player = testPlayer.WithFlags(USER_CHAR + "test/root");
    Assert.True(player.HasFlags(USER_CHAR + "test/root"));
    Assert.True(player.HasFlags(USER_CHAR + "test/flag"));
    Assert.True(player.HasFlags(USER_CHAR + "test/other/test"));
  }
}