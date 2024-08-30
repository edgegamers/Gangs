using GangsAPI.Services;

namespace GangsTest;

public class StatRegistrationTests {
  [Theory]
  [ClassData(typeof(StatManagerData))]
  public async void Stat_Register(IStatManager mgr) {
    var stat = await StatTestUtil.CreateStat(mgr);
    Assert.NotNull(stat);
    Assert.True(await mgr.RegisterStat(stat),
      "Failed to register new statistic");
  }

  [Theory]
  [ClassData(typeof(StatManagerData))]
  public async void Stat_Register_Duplicate(IStatManager mgr) {
    var stat = await StatTestUtil.CreateStat(mgr);
    Assert.NotNull(stat);
    Assert.True(await mgr.RegisterStat(stat),
      "Failed to register new statistic");
    Assert.False(await mgr.RegisterStat(stat),
      "Failed to recognize pre-registered statistic");
  }

  [Theory]
  [ClassData(typeof(StatManagerData))]
  public async void Stat_Unregistered(IStatManager mgr) {
    Assert.Null(await mgr.GetStat("id"));
    await StatTestUtil.CreateStat(mgr);
    Assert.Null(await mgr.GetStat("id"));
    var stat = await StatTestUtil.CreateStat(mgr);
    Assert.NotNull(stat);
    await mgr.RegisterStat(stat);
    Assert.NotNull(await mgr.GetStat("id"));
  }

  [Theory]
  [ClassData(typeof(StatManagerData))]
  public async void Stat_Unregister(IStatManager mgr) {
    var stat = await StatTestUtil.CreateStat(mgr);
    Assert.NotNull(stat);
    Assert.True(await mgr.RegisterStat(stat),
      "Failed to register new statistic");
    await mgr.UnregisterStat(stat.StatId);
    Assert.Null(await mgr.GetStat(stat.StatId));
  }
}