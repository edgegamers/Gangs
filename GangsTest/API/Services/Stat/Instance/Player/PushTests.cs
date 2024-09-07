using GangsAPI.Services.Gang;
using GangsAPI.Services.Player;
using GangsTest.API.Services.Stat.Instance.Gang;

namespace GangsTest.API.Services.Stat.Instance.Player;

public class PushTests(IPlayerManager playerMgr) : TestParent(playerMgr) {
  [Theory]
  [ClassData(typeof(TestData))]
  public async Task Class(IPlayerStatManager manager) {
    Assert.True(await manager.SetForPlayer(1234567890, TestStat));
    var (success, val) =
      await manager.GetForPlayer<int>(1234567890, TestStat.StatId);
    Assert.True(success);
    Assert.Equal(TestStat.Value, val);
  }

  [Theory]
  [ClassData(typeof(TestData))]
  public async Task Class_Multiple(IPlayerStatManager manager) {
    Assert.True(
      await manager.SetForPlayer(1234567890, "foo", new Reputation(0, 1)));
    var (success, val) =
      await manager.GetForPlayer<Reputation>(1234567890, "foo");
    Assert.True(success);
    Assert.Equal(new Reputation(0, 1), val);
    Assert.True(
      await manager.SetForPlayer(1234567890, "foo", new Reputation(1, 0)));
    var (success2, val2) =
      await manager.GetForPlayer<Reputation>(1234567890, "foo");
    Assert.True(success2);
    Assert.Equal(new Reputation(1, 0), val2);
  }
}