using GangsAPI.Services;

namespace GangsTest.StatTests.ManageTests;

public class MockStatManagerTests {
  [Theory]
  [ClassData(typeof(StatManagerData))]
  public async Task Stat_Create(IStatManager mgr) {
    var dummy = await mgr.CreateStat("dummy", "name");
    Assert.NotNull(dummy);
  }

  [Theory]
  [ClassData(typeof(StatManagerData))]
  public async Task Stat_Create_Multiple(IStatManager mgr) {
    var foo = await StatTestUtil.CreateStat(mgr, "foo");
    var bar = await StatTestUtil.CreateStat(mgr, "bar");
    Assert.NotSame(foo, bar);
    Assert.NotNull(foo);
    Assert.NotNull(bar);
    Assert.Equal("foo", foo.StatId);
    Assert.Equal("bar", bar.StatId);
  }
}