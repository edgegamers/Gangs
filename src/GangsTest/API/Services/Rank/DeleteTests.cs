using GangsAPI.Extensions;
using GangsAPI.Permissions;
using GangsAPI.Services;
using GangsAPI.Services.Player;

namespace GangsTest.API.Services.Rank;

public class DeleteTests : TestParent {
  [Theory]
  [ClassData(typeof(TestData))]
  public async Task Delete(IRankManager mgr, IPlayerManager _) {
    var rank = await mgr.CreateRank(GangId, "Test Rank", 1, Perm.KICK_OTHERS);
    Assert.NotNull(rank);
    Assert.True(await mgr.DeleteRank(GangId, rank.Rank,
      IRankManager.DeleteStrat.CANCEL));
    Assert.Null(await mgr.GetRank(GangId, rank.Rank));
  }


  [Theory]
  [ClassData(typeof(TestData))]
  public async Task Delete_Cancel_Demote(IRankManager mgr,
    IPlayerManager players) {
    var rank = await mgr.CreateRank(GangId, "Test Rank", 1, Perm.KICK_OTHERS);
    Assert.NotNull(rank);
    var player = await players.CreatePlayer(new Random().NextULong(),
      "Test Player");
    player.GangId   = GangId;
    player.GangRank = rank.Rank;
    await players.UpdatePlayer(player);

    Assert.False(await mgr.DeleteRank(GangId, rank.Rank,
      IRankManager.DeleteStrat.DEMOTE_FAIL));
    Assert.NotNull(await mgr.GetRank(GangId, rank.Rank));
  }

  [Theory]
  [ClassData(typeof(TestData))]
  public async Task Delete_Owner(IRankManager mgr, IPlayerManager _) {
    await mgr.AssignDefaultRanks(TestGang);
    Assert.NotNull(await mgr.GetRank(TestGang, 0));
    Assert.False(await mgr.DeleteRank(GangId, 0,
      IRankManager.DeleteStrat.DEMOTE_KICK));
  }

  [Theory]
  [ClassData(typeof(TestData))]
  public async Task Delete_With_Players_Cancel(IRankManager mgr,
    IPlayerManager players) {
    var lowestRank = mgr.CreateRank(GangId, "Lowest Rank", 100, Perm.NONE);
    Assert.NotNull(lowestRank);

    var officer =
      await players.CreatePlayer(new Random().NextULong(), "Test Officer");
    Assert.NotNull(officer);
    officer.GangId   = GangId;
    officer.GangRank = 100;

    Assert.True(await players.UpdatePlayer(officer));
    Assert.False(await mgr.DeleteRank(GangId, 100,
      IRankManager.DeleteStrat.CANCEL));
    Assert.Single(await players.GetMembers(GangId));
  }

  [Theory]
  [ClassData(typeof(TestData))]
  public async Task Delete_With_Players_Kick(IRankManager mgr,
    IPlayerManager players) {
    var lowestRank = await mgr.CreateRank(GangId, "Lowest Rank", 100,
      Perm.NONE);
    Assert.NotNull(lowestRank);

    var officer =
      await players.CreatePlayer(new Random().NextULong(), "Test Officer");
    Assert.NotNull(officer);
    officer.GangId   = GangId;
    officer.GangRank = lowestRank.Rank;

    Assert.True(await players.UpdatePlayer(officer));
    Assert.True(await mgr.DeleteRank(GangId, 100,
      IRankManager.DeleteStrat.DEMOTE_KICK));
    Assert.Empty(await players.GetMembers(GangId));
  }

  [Theory]
  [ClassData(typeof(TestData))]
  public async Task Delete_With_Inferior_Rank(IRankManager mgr,
    IPlayerManager players) {
    var highRank = await mgr.CreateRank(GangId, "High Rank", 50, Perm.NONE);
    var lowestRank = await mgr.CreateRank(GangId, "Lowest Rank", 100,
      Perm.NONE);
    Assert.NotNull(highRank);
    Assert.NotNull(lowestRank);

    var officer =
      await players.CreatePlayer(new Random().NextULong(), "Test Officer");
    Assert.NotNull(officer);
    officer.GangId   = GangId;
    officer.GangRank = highRank.Rank;

    Assert.True(await players.UpdatePlayer(officer));
    Assert.True(await mgr.DeleteRank(GangId, highRank.Rank,
      IRankManager.DeleteStrat.DEMOTE_FAIL));
    Assert.Equal(lowestRank.Rank,
      (await players.GetPlayer(officer.Steam))?.GangRank);
  }
}