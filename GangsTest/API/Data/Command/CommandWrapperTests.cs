using GangsAPI.Data;
using GangsAPI.Data.Command;

namespace GangsTest.API.Data.Command;

public class CommandWrapperTests {
  private readonly PlayerWrapper testPlayer =
    new((ulong)new Random().NextInt64(), "Test Player");

  [Fact]
  public void Fields_Initialized() {
    var info = new CommandInfoWrapper(testPlayer, 0, "test");
    Assert.NotNull(info.CallingPlayer);
    Assert.Equal(testPlayer.Steam, info.CallingPlayer.Steam);
    Assert.Equal("Test Player", info.CallingPlayer.Name);
  }

  [Fact]
  public void CallingPlayer_IsNull() {
    var info = new CommandInfoWrapper(null, 0, "test");
    Assert.Null(info.CallingPlayer);
  }

  [Fact]
  public void ArgCountAt_Zero() {
    Assert.Equal(1, new CommandInfoWrapper(testPlayer, 0, "test").ArgCount);
  }

  [Fact]
  public void ArgCountAt_One() {
    Assert.Equal(1, new CommandInfoWrapper(testPlayer, 0, "test ing").ArgCount);
  }

  [Fact]
  public void Offset_ArgCountAt_Zero() {
    Assert.Equal(0, new CommandInfoWrapper(testPlayer, 1, "test").ArgCount);
  }

  [Fact]
  public void Offset_ArgCountAt_One() {
    Assert.Equal(0, new CommandInfoWrapper(testPlayer, 1, "test ing").ArgCount);
  }

  [Fact]
  public void Offset_ArgCount_One_Params() {
    Assert.Equal(1,
      new CommandInfoWrapper(testPlayer, 1, "test", "ing").ArgCount);
  }

  [Fact]
  public void ArgString_IsEmpty() {
    Assert.Equal("", new CommandInfoWrapper(testPlayer, 0, "test").ArgString);
  }

  [Fact]
  public void ArgString_SingleWord() {
    Assert.Equal("ing",
      new CommandInfoWrapper(testPlayer, 0, "test ing").ArgString);
  }

  [Fact]
  public void ArgString_SingleWord_Params() {
    Assert.Equal("ing",
      new CommandInfoWrapper(testPlayer, 0, "test", "ing").ArgString);
  }

  [Fact]
  public void ArgString_MultiWord_Params() {
    Assert.Equal("ing foobar",
      new CommandInfoWrapper(testPlayer, 0, "test", "ing", "foobar").ArgString);
  }

  [Fact]
  public void ArgString_MoreWord_Params() {
    Assert.Equal("test ing foobar",
      new CommandInfoWrapper(testPlayer, 0, "test", "ing", "foobar")
       .GetCommandString);
  }

  [Fact]
  public void ArgIndex_Zero_Params() {
    Assert.Equal("css_gang",
      new CommandInfoWrapper(testPlayer, 0, "css_gang", "create", "foobar")[0]);
  }

  [Fact]
  public void Offset_ArgIndex_Zero_Params() {
    Assert.Equal("create",
      new CommandInfoWrapper(testPlayer, 1, "css_gang", "create", "foobar")[0]);
  }

  [Fact]
  public void Offset_ArgIndex_One_Params() {
    Assert.Equal("foobar",
      new CommandInfoWrapper(testPlayer, 1, "css_gang", "create", "foobar")[1]);
  }

  [Fact]
  public void Offset_ArgIndex_Multi_One_Params() {
    Assert.Equal("create foobar",
      new CommandInfoWrapper(testPlayer, 1, "css_gang", "create", "foobar")
       .GetCommandString);
  }
}