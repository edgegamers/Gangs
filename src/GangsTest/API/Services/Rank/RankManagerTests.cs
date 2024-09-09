using GangsAPI.Permissions;
using GangsAPI.Services;

namespace GangsTest.API.Services.Rank;

public class RankManagerTests : TestParent {
  [Theory]
  [ClassData(typeof(TestData))]
  public async Task Create(IRankManager mgr) {
    var rank = await mgr.CreateRank(GangId, "Test Rank", 1,
      IGangRank.Permissions.KICK_OTHERS);
    Assert.NotNull(rank);
    Assert.Equal("Test Rank", rank.Name);
  }

  [Theory]
  [ClassData(typeof(TestData))]
  public async Task Create_Fetch(IRankManager mgr) {
    await mgr.CreateRank(GangId, "Test Rank", 1,
      IGangRank.Permissions.KICK_OTHERS);
    var rank = await mgr.GetRank(GangId, 1);
    Assert.NotNull(rank);
    Assert.Equal("Test Rank", rank.Name);
  }

  [Theory]
  [ClassData(typeof(TestData))]
  public async Task Create_DefaultRanks(IRankManager mgr) {
    await mgr.AssignDefaultRanks(GangId);
    var ranks = await mgr.GetRanks(GangId);
    Assert.NotEmpty(ranks);
  }
}