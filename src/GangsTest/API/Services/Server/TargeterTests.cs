using CounterStrikeSharp.API.Modules.Commands.Targeting;
using GangsAPI;
using Microsoft.VisualStudio.TestPlatform.Common;

namespace GangsTest.API.Services.Server;

public class TargeterTests : TestParent {
  [Theory]
  [ClassData(typeof(TargeterTestData))]
  public async Task GetTarget_NoPlayers(IServerProvider _, ITargeter targeter) {
    Assert.Empty(await targeter.GetTarget("@all", TestPlayer));
  }

  [Theory]
  [ClassData(typeof(TargeterTestData))]
  public async Task GetTarget_OnePlayer(IServerProvider server,
    ITargeter targeter) {
    await server.AddPlayer(TestPlayer);
    var players = (await targeter.GetTarget("@me", TestPlayer)).ToList();
    Assert.Equal([TestPlayer], players);
  }
}