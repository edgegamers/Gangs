using GangsAPI.Services;
using GangsTest.API.Services.Stat.Manager;

namespace GangsTest.API.Services.Stat;

public class PersistenceTests {
  [Theory]
  [ClassData(typeof(TestData))]
  public async Task Persist_Cache_Clear(IStatManager mgr) {
    Assert.Empty(await mgr.GetStats());
    var dummy = await mgr.CreateStat("id", "name", "desc");
    Assert.NotNull(dummy);
    Assert.True(await mgr.RegisterStat(dummy));
    Assert.Single(await mgr.GetStats());
    mgr.ClearCache();
    Assert.Empty(await mgr.GetStats());
  }

  [Theory]
  [ClassData(typeof(TestData))]
  public async Task Persist_Cache_Load(IStatManager mgr) {
    Assert.Empty(await mgr.GetStats());
    var dummy = await mgr.CreateStat("id", "name", "desc");
    Assert.NotNull(dummy);
    Assert.True(await mgr.RegisterStat(dummy));
    Assert.Single(await mgr.GetStats());
    mgr.ClearCache();
    Assert.Empty(await mgr.GetStats());
    await mgr.Load();
    Assert.Single(await mgr.GetStats());
  }
}