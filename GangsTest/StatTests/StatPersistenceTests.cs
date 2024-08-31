using GangsAPI.Services;

namespace GangsTest.StatTests;

public class StatPersistenceTests {
  [Theory]
  [ClassData(typeof(StatManagerData))]
  public async Task Persist_Cache_Clear(IStatManager mgr) {
    Assert.Empty(await mgr.GetStats());
    var dummy = await StatTestUtil.CreateStat(mgr);
    Assert.NotNull(dummy);
    Assert.True(await mgr.RegisterStat(dummy));
    Assert.Single(await mgr.GetStats());
    mgr.ClearCache();
    Assert.Empty(await mgr.GetStats());
  }

  [Theory]
  [ClassData(typeof(StatManagerData))]
  public async Task Persist_Cache_Load(IStatManager mgr) {
    Assert.Empty(await mgr.GetStats());
    var dummy = await StatTestUtil.CreateStat(mgr);
    Assert.NotNull(dummy);
    Assert.True(await mgr.RegisterStat(dummy));
    Assert.Single(await mgr.GetStats());
    mgr.ClearCache();
    Assert.Empty(await mgr.GetStats());
    await mgr.Load();
    Assert.Single(await mgr.GetStats());
  }
}