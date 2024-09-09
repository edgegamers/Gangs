using GangsAPI.Permissions;
using GangsAPI.Services;

namespace GangsTest.API.Services.Rank;

public class DeleteTests : TestParent {
  [Theory]
  [ClassData(typeof(TestData))]
  public async Task Delete(IRankManager mgr) {
    var rank = await mgr.CreateRank(GangId, "Test Rank", 1,
      IGangRank.Permissions.KICK_OTHERS);
    Assert.NotNull(rank);
    Assert.True(await mgr.DeleteRank(GangId, rank.Rank));
    Assert.Null(await mgr.GetRank(GangId, rank.Rank));
  }

  [Theory]
  [ClassData(typeof(TestData))]
  public async Task Delete_Owner(IRankManager mgr) {
    await mgr.AssignDefaultRanks(TestGang);
    Assert.NotNull(await mgr.GetRank(TestGang, 0));
    Assert.False(await mgr.DeleteRank(GangId, 0));
  }
}