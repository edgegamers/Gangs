using GangsAPI.Data;

namespace GangsTest.API;

public class PlayerWrapperTests {
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
    var player = testPlayer.WithFlags("@test/flag");
    Assert.NotNull(player.Data);
    Assert.True(player.Data.Flags.ContainsKey("@test"));
  }
}