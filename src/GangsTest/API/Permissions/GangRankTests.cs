using GangsAPI.Permissions;
using Mock;

namespace GangsTest.API.Permissions;

public class GangRankTests {
  [Fact]
  public void Rank_Owner_HasAllPerms() {
    var rank = new MockGangRank(0, "Owner", IGangRank.Permissions.OWNER);
    foreach (var perm in Enum.GetValues<IGangRank.Permissions>())
      Assert.True(rank.Perms.HasFlag(perm));
  }

  [Fact]
  public void Rank_Admin_HasAllPerms() {
    var rank =
      new MockGangRank(1, "Admin", IGangRank.Permissions.ADMINISTRATOR);
    foreach (var perm in Enum.GetValues<IGangRank.Permissions>())
      if (perm == IGangRank.Permissions.OWNER)
        Assert.False(rank.Perms.HasFlag(perm));
      else
        Assert.True(rank.Perms.HasFlag(perm));
  }

  [Fact]
  public void Rank_Admin_Fields() {
    var rank = new MockGangRank(0, "Owner");
    Assert.Equal("Owner", rank.Name);
    Assert.Equal(0, rank.Rank);
    Assert.Equal(0, (int)rank.Perms);
  }
}