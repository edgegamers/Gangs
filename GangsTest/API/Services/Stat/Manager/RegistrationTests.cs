using GangsAPI.Services;

namespace GangsTest.API.Services.Stat.Manager;

public class RegistrationTests {
  [Theory]
  [ClassData(typeof(TestData))]
  public async Task Stat_Register(IStatManager mgr) {
    var stat = await mgr.CreateStat("id", "name", "desc");
    Assert.NotNull(stat);
    Assert.True(await mgr.RegisterStat(stat),
      "Failed to register new statistic");
  }

  [Theory]
  [ClassData(typeof(TestData))]
  public async Task Stat_Register_Duplicate(IStatManager mgr) {
    var stat = await mgr.CreateStat("id", "name", "desc");
    Assert.NotNull(stat);
    Assert.True(await mgr.RegisterStat(stat),
      "Failed to register new statistic");
    Assert.False(await mgr.RegisterStat(stat),
      "Failed to recognize pre-registered statistic");
  }

  [Theory]
  [ClassData(typeof(TestData))]
  public async Task Stat_Unregistered(IStatManager mgr) {
    Assert.Null(await mgr.GetStat("id"));
    await mgr.CreateStat("id", "name", "desc");
    Assert.Null(await mgr.GetStat("id"));
    var stat = await mgr.CreateStat("id", "name", "desc");
    Assert.NotNull(stat);
    await mgr.RegisterStat(stat);
    Assert.NotNull(await mgr.GetStat("id"));
  }

  [Theory]
  [ClassData(typeof(TestData))]
  public async Task Stat_Unregister(IStatManager mgr) {
    var stat = await mgr.CreateStat("id", "name", "desc");
    Assert.NotNull(stat);
    Assert.True(await mgr.RegisterStat(stat),
      "Failed to register new statistic");
    await mgr.UnregisterStat(stat.StatId);
    Assert.Null(await mgr.GetStat(stat.StatId));
  }
}