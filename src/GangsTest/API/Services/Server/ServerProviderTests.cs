using GangsAPI;
using GangsAPI.Data;
using GangsAPI.Extensions;
using GangsAPI.Services.Server;

namespace GangsTest.API.Services.Server;

public class ServerProviderTests : TestParent {
  [Theory]
  [ClassData(typeof(ServerTestData))]
  public async Task Empty_Players(IServerProvider server) {
    Assert.Empty(await server.GetPlayers());
  }

  [Theory]
  [ClassData(typeof(ServerTestData))]
  public async Task AddOne(IServerProvider server) {
    await server.AddPlayer(TestPlayer);
    Assert.Equal([TestPlayer], await server.GetPlayers());
  }
}