using GangsAPI.Services;

namespace GangsTest.API.Services.Stat.Manager;

public class RetainTests {
  [Theory]
  [ClassData(typeof(TestData))]
  public async Task Stat_Retain(IStatManager mgr) {
    var foo1 = await mgr.CreateStat("id", "name", "desc");
    Assert.NotNull(foo1);
    await mgr.RegisterStat(foo1);
    var foo2 = await mgr.CreateStat("id", "name", "desc");
    Assert.Same(foo1, foo2);
  }

  [Theory]
  [ClassData(typeof(TestData))]
  public async Task Stat_Retain_Unregsitered(IStatManager mgr) {
    var foo1 = await mgr.CreateStat("id", "name", "desc");
    Assert.NotNull(foo1);
    var foo2 = await mgr.CreateStat("id", "name", "desc");
    Assert.NotSame(foo1, foo2);
  }

  [Theory]
  [ClassData(typeof(TestData))]
  public async Task Stat_Retain_NonTrivial_Name(IStatManager mgr) {
    var foo1 = await mgr.CreateStat("foo", "bar");
    Assert.NotNull(foo1);
    await mgr.RegisterStat(foo1);
    var foo2 = await mgr.CreateStat("foo", "foobar");
    Assert.NotNull(foo2);
    Assert.Same(foo1, foo2);
  }

  [Theory]
  [ClassData(typeof(TestData))]
  public async Task Stat_Retain_NonTrivial_Desc(IStatManager mgr) {
    var foo1 = await mgr.CreateStat("foo", "name", "bar");
    Assert.NotNull(foo1);
    await mgr.RegisterStat(foo1);
    var foo2 = await mgr.CreateStat("foo", "name", "foobar");
    Assert.NotNull(foo1);
    Assert.NotNull(foo2);
    Assert.Same(foo1, foo2);
  }

  [Theory]
  [ClassData(typeof(TestData))]
  public async Task Stat_Retain_NonTrivial_Both(IStatManager mgr) {
    var foo1 = await mgr.CreateStat("foo", "foobar", "barfoo");
    Assert.NotNull(foo1);
    await mgr.RegisterStat(foo1);
    var foo2 = await mgr.CreateStat("foo", "barfoo", "foobar");
    Assert.NotNull(foo1);
    Assert.NotNull(foo2);
    Assert.Same(foo1, foo2);
  }
}