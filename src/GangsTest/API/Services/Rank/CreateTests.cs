using GangsAPI.Permissions;
using GangsAPI.Services;
using GangsAPI.Services.Player;

namespace GangsTest.API.Services.Rank;

public class CreateTests : TestParent {
  [Theory]
  [ClassData(typeof(TestData))]
  public async Task Create(IRankManager mgr, IPlayerManager _) {
    var rank = await mgr.CreateRank(GangId, "Test Rank", 1,
      IGangRank.Permissions.KICK_OTHERS);
    Assert.NotNull(rank);
    Assert.Equal("Test Rank", rank.Name);
  }

  [Theory]
  [ClassData(typeof(TestData))]
  public async Task Create_Duplicate(IRankManager mgr, IPlayerManager _) {
    var rank = await mgr.CreateRank(GangId, "Test Rank", 1,
      IGangRank.Permissions.KICK_OTHERS);
    Assert.NotNull(rank);
    Assert.Null(await mgr.CreateRank(GangId, "New Rank, totally new!", 1,
      IGangRank.Permissions.CREATE_RANKS));
  }

  [Theory]
  [ClassData(typeof(TestData))]
  public async Task Create_Fetch(IRankManager mgr, IPlayerManager _) {
    await mgr.CreateRank(GangId, "Test Rank", 1,
      IGangRank.Permissions.KICK_OTHERS);
    var rank = await mgr.GetRank(GangId, 1);
    Assert.NotNull(rank);
    Assert.Equal("Test Rank", rank.Name);
  }

  [Theory]
  [ClassData(typeof(TestData))]
  public async Task Fetch_All_Single(IRankManager mgr, IPlayerManager _) {
    await mgr.CreateRank(GangId, "Test Rank", 1,
      IGangRank.Permissions.KICK_OTHERS);
    Assert.Single(await mgr.GetRanks(GangId));
  }

  [Theory]
  [ClassData(typeof(TestData))]
  public async Task Fetch_All_Multi(IRankManager mgr, IPlayerManager _) {
    await mgr.CreateRank(GangId, "Test Rank", 1,
      IGangRank.Permissions.KICK_OTHERS);
    await mgr.CreateRank(GangId, "Test Rank", 5,
      IGangRank.Permissions.KICK_OTHERS);
    Assert.Equal(2, (await mgr.GetRanks(GangId)).Count());
  }

  [Theory]
  [ClassData(typeof(TestData))]
  public async Task Create_DefaultRanks(IRankManager mgr, IPlayerManager _) {
    await mgr.AssignDefaultRanks(GangId);
    var ranks = await mgr.GetRanks(GangId);
    Assert.NotEmpty(ranks);
  }
}