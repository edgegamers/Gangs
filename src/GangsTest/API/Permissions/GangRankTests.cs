using GangsAPI.Permissions;
using Mock;

namespace GangsTest.API.Permissions;

public class GangRankTests {
  [Fact]
  public void Rank_Owner_HasAllPerms() {
    var rank = new MockGangRank(0, "Owner", Perm.OWNER);
    foreach (var perm in Enum.GetValues<Perm>())
      Assert.True(rank.Permissions.HasFlag(perm));
  }

  [Fact]
  public void Rank_Admin_HasAllPerms() {
    var rank = new MockGangRank(1, "Admin", Perm.ADMINISTRATOR);
    foreach (var perm in Enum.GetValues<Perm>())
      if (perm == Perm.OWNER)
        Assert.False(rank.Permissions.HasFlag(perm));
      else
        Assert.True(rank.Permissions.HasFlag(perm));
  }

  [Fact]
  public void Rank_Admin_Fields() {
    var rank = new MockGangRank(0, "Owner");
    Assert.Equal("Owner", rank.Name);
    Assert.Equal(0, rank.Rank);
    Assert.Equal(0, (int)rank.Permissions);
  }
}