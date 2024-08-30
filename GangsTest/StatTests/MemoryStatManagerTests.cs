using GangsAPI.Services;

namespace GangsTest;

public class MemoryStatManagerTests {
  [Theory]
  [ClassData(typeof(StatManagerData))]
  public async void Stat_Create(IStatManager mgr) {
    var dummy = await mgr.CreateStat("dummy", "name");
    Assert.NotNull(dummy);
  }

  [Theory]
  [ClassData(typeof(StatManagerData))]
  public async void Stat_Create_Multiple(IStatManager mgr) {
    var foo = await StatTestUtil.CreateStat(mgr, "foo");
    var bar = await StatTestUtil.CreateStat(mgr, "bar");
    Assert.NotSame(foo, bar);
    Assert.NotNull(foo);
    Assert.NotNull(bar);
    Assert.Equal("foo", foo.StatId);
    Assert.Equal("bar", bar.StatId);
  }
}