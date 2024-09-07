using GangsAPI.Services;

namespace GangsTest.API.Services.Stat.Manager;

public class CreationTests {
  [Theory]
  [ClassData(typeof(TestData))]
  public async Task Stat_Create(IStatManager mgr) {
    var dummy = await mgr.CreateStat("dummy", "name");
    Assert.NotNull(dummy);
  }

  [Theory]
  [ClassData(typeof(TestData))]
  public async Task Stat_Create_Multiple(IStatManager mgr) {
    var foo = await mgr.CreateStat("foo", "name", "desc");
    var bar = await mgr.CreateStat("bar", "name", "desc");
    Assert.NotSame(foo, bar);
    Assert.NotNull(foo);
    Assert.NotNull(bar);
    Assert.Equal("foo", foo.StatId);
    Assert.Equal("bar", bar.StatId);
  }
}