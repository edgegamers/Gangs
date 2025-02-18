using GangsAPI.Services.Player;

namespace GangsTest.API.Services.Stat.Instance.Player;

public class PushTests(IPlayerManager players) : TestParent(players) {
  [Theory]
  [ClassData(typeof(TestData))]
  public async Task Class(IPlayerStatManager manager) {
    Assert.True(await manager.SetForPlayer(1234567890, TestStat));
    var val = await manager.GetForPlayer<int>(1234567890, TestStat.StatId);
    Assert.Equal(TestStat.Value, val);
  }

  [Theory]
  [ClassData(typeof(TestData))]
  public async Task Class_Multiple(IPlayerStatManager manager) {
    Assert.True(
      await manager.SetForPlayer(1234567890, "foo", new Reputation(0, 1)));
    var val = await manager.GetForPlayer<Reputation>(1234567890, "foo");
    Assert.Equal(new Reputation(0, 1), val);
    Assert.True(
      await manager.SetForPlayer(1234567890, "foo", new Reputation(1, 0)));
    var val2 = await manager.GetForPlayer<Reputation>(1234567890, "foo");
    Assert.Equal(new Reputation(1, 0), val2);
  }
}