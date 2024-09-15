using GangsAPI;
using GangsAPI.Data;
using GangsAPI.Extensions;
using GangsAPI.Services.Server;
using Microsoft.Extensions.Localization;

namespace GangsTest.API.Services.Server;

public class TargeterTests(IStringLocalizer locale) : TestParent {
  [Theory]
  [ClassData(typeof(TargeterTestData))]
  public async Task Multi_NoPlayers(IServerProvider _, IPlayerTargeter playerTargeter) {
    Assert.Empty(await playerTargeter.GetTarget("@all", TestPlayer));
  }

  [Theory]
  [ClassData(typeof(TargeterTestData))]
  public async Task Multi_Self_Me(IServerProvider server, IPlayerTargeter playerTargeter) {
    await server.AddPlayer(TestPlayer);
    var players = (await playerTargeter.GetTarget("@me", TestPlayer)).ToList();
    Assert.Equal([TestPlayer], players);
  }

  [Theory]
  [ClassData(typeof(TargeterTestData))]
  public async Task
    Multi_Self_Name(IServerProvider server, IPlayerTargeter playerTargeter) {
    await server.AddPlayer(TestPlayer);
    var players =
      (await playerTargeter.GetTarget(TestPlayer.Name!, TestPlayer)).ToList();
    Assert.Equal([TestPlayer], players);
  }

  [Theory]
  [ClassData(typeof(TargeterTestData))]
  public async Task Multi_Other_NotMe(IServerProvider server,
    IPlayerTargeter playerTargeter) {
    await server.AddPlayer(TestPlayer);
    var other = new PlayerWrapper(new Random().NextULong(), "Other Player");
    await server.AddPlayer(other);
    var players = (await playerTargeter.GetTarget("@!me", TestPlayer)).ToList();
    Assert.Equal([other], players);
  }

  [Theory]
  [ClassData(typeof(TargeterTestData))]
  public async Task
    Multi_Other_Name(IServerProvider server, IPlayerTargeter playerTargeter) {
    await server.AddPlayer(TestPlayer);
    var other = new PlayerWrapper(new Random().NextULong(), "Other Player");
    await server.AddPlayer(other);
    var players = (await playerTargeter.GetTarget("Other", TestPlayer)).ToList();
    Assert.Equal([other], players);
  }

  [Theory]
  [ClassData(typeof(TargeterTestData))]
  public async Task Single_NoPlayers(IServerProvider _, IPlayerTargeter playerTargeter) {
    Assert.Null(await playerTargeter.GetSingleTarget("@all", TestPlayer));
  }

  [Theory]
  [ClassData(typeof(TargeterTestData))]
  public async Task Single_Me_All(IServerProvider server, IPlayerTargeter playerTargeter) {
    await server.AddPlayer(TestPlayer);
    Assert.Equal(TestPlayer,
      await playerTargeter.GetSingleTarget("@all", TestPlayer));
  }

  [Theory]
  [ClassData(typeof(TargeterTestData))]
  public async Task Single_Me_Me(IServerProvider server, IPlayerTargeter playerTargeter) {
    await server.AddPlayer(TestPlayer);
    Assert.Equal(TestPlayer,
      await playerTargeter.GetSingleTarget("@me", TestPlayer));
  }

  [Theory]
  [ClassData(typeof(TargeterTestData))]
  public async Task Multi_Fail_Prints(IServerProvider _, IPlayerTargeter playerTargeter) {
    await playerTargeter.GetTarget("@all", TestPlayer, locale);
    Assert.Equal([locale.Get(MSG.GENERIC_PLAYER_NOT_FOUND, "@all")],
      TestPlayer.ChatOutput);
  }

  [Theory]
  [ClassData(typeof(TargeterTestData))]
  public async Task Single_Fail_Prints(IServerProvider _, IPlayerTargeter playerTargeter) {
    await playerTargeter.GetSingleTarget("@all", TestPlayer, locale);
    Assert.Equal([locale.Get(MSG.GENERIC_PLAYER_NOT_FOUND, "@all")],
      TestPlayer.ChatOutput);
  }

  [Theory]
  [ClassData(typeof(TargeterTestData))]
  public async Task Single_FailMulti_Prints(IServerProvider server,
    IPlayerTargeter playerTargeter) {
    await server.AddPlayer(TestPlayer);
    await server.AddPlayer(new PlayerWrapper(new Random().NextULong(),
      "Other Player"));
    await playerTargeter.GetSingleTarget("@all", TestPlayer, locale);
    Assert.Equal([locale.Get(MSG.GENERIC_PLAYER_FOUND_MULTIPLE, "@all")],
      TestPlayer.ChatOutput);
  }
}