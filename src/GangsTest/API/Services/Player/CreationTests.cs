using GangsAPI.Services.Player;

namespace GangsTest.API.Services.Player;

public class CreationTests {
  [Theory]
  [ClassData(typeof(TestData))]
  public async Task Create(IPlayerManager mgr) {
    var player = await mgr.CreatePlayer(0);
    Assert.NotNull(player);
    Assert.Null(player.Name);
  }

  [Theory]
  [ClassData(typeof(TestData))]
  public async Task Create_Named(IPlayerManager mgr) {
    var player = await mgr.CreatePlayer(0, "Test Player");
    Assert.NotNull(player);
    Assert.Equal("Test Player", player.Name);
  }

  [Theory]
  [ClassData(typeof(TestData))]
  public async Task Create_Duplicate(IPlayerManager mgr) {
    var player1 = await mgr.CreatePlayer(0, "Test Player");
    var player2 = await mgr.CreatePlayer(0, "Test Player");
    Assert.NotNull(player1);
    Assert.Equal("Test Player", player1.Name);
    Assert.Equal("Test Player", player2.Name);
  }
}