using GangsAPI.Permissions;
using Mock;

namespace GangsTest.API.Permissions;

public class GangRankTests {
  [Fact]
  public void HasFlag_OnOwner_HasAllPerms() {
    var rank = new MockGangRank(0, "", Perm.OWNER);
    Assert.All(Enum.GetValues<Perm>(),
      perm => Assert.True(rank.Permissions.HasFlag(perm)));
  }

  [Fact]
  public void HasFlag_OnAdmin_HasAllPerms() {
    var rank = new MockGangRank(0, "", Perm.ADMINISTRATOR);
    Assert.All(Enum.GetValues<Perm>(),
      perm => Assert.True(perm == Perm.OWNER
        || rank.Permissions.HasFlag(perm)));
  }
}