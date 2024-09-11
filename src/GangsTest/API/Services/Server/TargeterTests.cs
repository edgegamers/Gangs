using GangsAPI;
using GangsAPI.Data;
using GangsAPI.Extensions;
using GangsAPI.Services.Server;
using Microsoft.Extensions.Localization;

namespace GangsTest.API.Services.Server;

public class TargeterTests(IStringLocalizer locale) : TestParent {
  [Theory]
  [ClassData(typeof(TargeterTestData))]
  public async Task Multi_NoPlayers(IServerProvider _, ITargeter targeter) {
    Assert.Empty(await targeter.GetTarget("@all", TestPlayer));
  }

  [Theory]
  [ClassData(typeof(TargeterTestData))]
  public async Task Multi_Self_Me(IServerProvider server, ITargeter targeter) {
    await server.AddPlayer(TestPlayer);
    var players = (await targeter.GetTarget("@me", TestPlayer)).ToList();
    Assert.Equal([TestPlayer], players);
  }

  [Theory]
  [ClassData(typeof(TargeterTestData))]
  public async Task
    Multi_Self_Name(IServerProvider server, ITargeter targeter) {
    await server.AddPlayer(TestPlayer);
    var players =
      (await targeter.GetTarget(TestPlayer.Name!, TestPlayer)).ToList();
    Assert.Equal([TestPlayer], players);
  }

  [Theory]
  [ClassData(typeof(TargeterTestData))]
  public async Task Multi_Other_NotMe(IServerProvider server,
    ITargeter targeter) {
    await server.AddPlayer(TestPlayer);
    var other = new PlayerWrapper(new Random().NextULong(), "Other Player");
    await server.AddPlayer(other);
    var players = (await targeter.GetTarget("@!me", TestPlayer)).ToList();
    Assert.Equal([other], players);
  }

  [Theory]
  [ClassData(typeof(TargeterTestData))]
  public async Task
    Multi_Other_Name(IServerProvider server, ITargeter targeter) {
    await server.AddPlayer(TestPlayer);
    var other = new PlayerWrapper(new Random().NextULong(), "Other Player");
    await server.AddPlayer(other);
    var players = (await targeter.GetTarget("Other", TestPlayer)).ToList();
    Assert.Equal([other], players);
  }

  [Theory]
  [ClassData(typeof(TargeterTestData))]
  public async Task Single_NoPlayers(IServerProvider _, ITargeter targeter) {
    Assert.Null(await targeter.GetSingleTarget("@all", out var _, TestPlayer));
  }

  [Theory]
  [ClassData(typeof(TargeterTestData))]
  public async Task Single_Me_All(IServerProvider server, ITargeter targeter) {
    await server.AddPlayer(TestPlayer);
    Assert.Equal(TestPlayer,
      await targeter.GetSingleTarget("@all", out var _, TestPlayer));
  }

  [Theory]
  [ClassData(typeof(TargeterTestData))]
  public async Task Single_Me_Me(IServerProvider server, ITargeter targeter) {
    await server.AddPlayer(TestPlayer);
    Assert.Equal(TestPlayer,
      await targeter.GetSingleTarget("@me", out var _, TestPlayer));
  }

  [Theory]
  [ClassData(typeof(TargeterTestData))]
  public async Task Multi_Fail_Prints(IServerProvider _, ITargeter targeter) {
    await targeter.GetTarget("@all", TestPlayer, locale);
    Assert.Equal([locale.Get(MSG.GENERIC_PLAYER_NOT_FOUND, "@all")],
      TestPlayer.ChatOutput);
  }

  [Theory]
  [ClassData(typeof(TargeterTestData))]
  public async Task Single_Fail_Prints(IServerProvider _, ITargeter targeter) {
    await targeter.GetSingleTarget("@all", out var _, TestPlayer, locale);
    Assert.Equal([locale.Get(MSG.GENERIC_PLAYER_NOT_FOUND, "@all")],
      TestPlayer.ChatOutput);
  }

  [Theory]
  [ClassData(typeof(TargeterTestData))]
  public async Task Single_FailMulti_Prints(IServerProvider server,
    ITargeter targeter) {
    await server.AddPlayer(TestPlayer);
    await server.AddPlayer(new PlayerWrapper(new Random().NextULong(),
      "Other Player"));
    await targeter.GetSingleTarget("@all", out var _, TestPlayer, locale);
    Assert.Equal([locale.Get(MSG.GENERIC_PLAYER_FOUND_MULTIPLE, "@all")],
      TestPlayer.ChatOutput);
  }
}