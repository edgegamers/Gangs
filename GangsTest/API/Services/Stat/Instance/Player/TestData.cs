using GangsAPI.Services.Player;

namespace GangsTest.API.Services.Stat.Instance.Player;

public class TestData : TestParent {
  [Theory]
  [ClassData(typeof(PlayerInstanceManagerData))]
  public async Task Instance_Class_Push(IPlayerStatManager manager) {
    Assert.True(await manager.SetForPlayer(1234567890, TestStat));
    var (success, val) =
      await manager.GetForPlayer<int>(1234567890, TestStat.StatId);
    Assert.True(success);
    Assert.Equal(TestStat.Value, val);
  }
}