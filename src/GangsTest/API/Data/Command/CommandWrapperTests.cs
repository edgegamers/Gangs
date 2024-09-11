using GangsAPI.Data;
using GangsAPI.Data.Command;
using GangsAPI.Extensions;

namespace GangsTest.API.Data.Command;

public class CommandWrapperTests {
  private readonly PlayerWrapper testPlayer =
    new(new Random().NextUInt(), "Test Player");

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
  public void Wrapper_Wrapper() {
    var info = new CommandInfoWrapper(testPlayer, 0, "a", "b", "c");
    Assert.Equal(3, info.ArgCount);
    var wrapped = new CommandInfoWrapper(testPlayer, 1, info.Args);
    Assert.Equal(2, wrapped.ArgCount);
    Assert.Equal((string[]) ["b", "c"], wrapped.Args);
  }

  [Fact]
  public void Wrapper_Wrapper_Offset() {
    var info = new CommandInfoWrapper(testPlayer, 1, "a", "b", "c");
    Assert.Equal(2, info.ArgCount);
    var wrapped = new CommandInfoWrapper(testPlayer, 1, info.Args);
    Assert.Equal(1, wrapped.ArgCount);
    Assert.Equal("c", wrapped[0]);
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