using GangsAPI.Data;
using GangsAPI.Data.Command;

namespace GangsTest.API;

public class CommandWrapperTests {
  private readonly PlayerWrapper testPlayer =
    new((ulong)new Random().NextInt64(), "Test Player");

  [Fact]
  public void CommandWrapper_Player() {
    var info = new CommandInfoWrapper(testPlayer, 0, "test");
    Assert.NotNull(info.CallingPlayer);
    Assert.Equal(testPlayer.Steam, info.CallingPlayer.Steam);
    Assert.Equal("Test Player", info.CallingPlayer.Name);
  }

  [Fact]
  public void CommandWrapper_Console() {
    var info = new CommandInfoWrapper(null, 0, "test");
    Assert.Null(info.CallingPlayer);
  }

  [Fact]
  public void ArgCount_Single() {
    Assert.Equal(1, new CommandInfoWrapper(testPlayer, 0, "test").ArgCount);
  }

  [Fact]
  public void ArgCount_Single_Space() {
    Assert.Equal(1, new CommandInfoWrapper(testPlayer, 0, "test ing").ArgCount);
  }

  [Fact]
  public void Offset_ArgCount_Single() {
    Assert.Equal(0, new CommandInfoWrapper(testPlayer, 1, "test").ArgCount);
  }

  [Fact]
  public void Offset_ArgCount_Space() {
    Assert.Equal(0, new CommandInfoWrapper(testPlayer, 1, "test ing").ArgCount);
  }

  [Fact]
  public void Offset_ArgCount_One() {
    Assert.Equal(1,
      new CommandInfoWrapper(testPlayer, 1, "test", "ing").ArgCount);
  }

  [Fact]
  public void ArgString_Single() {
    Assert.Equal("", new CommandInfoWrapper(testPlayer, 0, "test").ArgString);
  }

  [Fact]
  public void ArgString_Single_Space() {
    Assert.Equal("ing",
      new CommandInfoWrapper(testPlayer, 0, "test ing").ArgString);
  }

  [Fact]
  public void ArgString_Multi() {
    Assert.Equal("ing",
      new CommandInfoWrapper(testPlayer, 0, "test", "ing").ArgString);
  }

  [Fact]
  public void ArgString_Multi_Space() {
    Assert.Equal("ing foobar",
      new CommandInfoWrapper(testPlayer, 0, "test", "ing", "foobar").ArgString);
  }

  [Fact]
  public void CommandString_Simple() {
    Assert.Equal("test ing foobar",
      new CommandInfoWrapper(testPlayer, 0, "test", "ing", "foobar")
       .GetCommandString);
  }

  [Fact]
  public void CommandString_SubCommand() {
    Assert.Equal("css_gang",
      new CommandInfoWrapper(testPlayer, 0, "css_gang", "create", "foobar")[0]);
  }

  [Fact]
  public void CommandString_SubCommand_Sub() {
    Assert.Equal("create",
      new CommandInfoWrapper(testPlayer, 1, "css_gang", "create", "foobar")[0]);
  }

  [Fact]
  public void CommandString_SubCommand_Sub1() {
    Assert.Equal("foobar",
      new CommandInfoWrapper(testPlayer, 1, "css_gang", "create", "foobar")[1]);
  }

  [Fact]
  public void CommandString_SubCommand_Sub2() {
    Assert.Equal("create foobar",
      new CommandInfoWrapper(testPlayer, 1, "css_gang", "create", "foobar")
       .GetCommandString);
  }
}