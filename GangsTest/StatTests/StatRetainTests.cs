using GangsAPI.Services;

namespace GangsTest.StatTests;

public class StatRetainTests {
  [Theory]
  [ClassData(typeof(StatManagerData))]
  public async Task Stat_Retain(IStatManager mgr) {
    var foo1 = await StatTestUtil.CreateStat(mgr);
    Assert.NotNull(foo1);
    await mgr.RegisterStat(foo1);
    var foo2 = await StatTestUtil.CreateStat(mgr);
    Assert.Same(foo1, foo2);
  }

  [Theory]
  [ClassData(typeof(StatManagerData))]
  public async Task Stat_Retain_Unregsitered(IStatManager mgr) {
    var foo1 = await StatTestUtil.CreateStat(mgr);
    Assert.NotNull(foo1);
    var foo2 = await StatTestUtil.CreateStat(mgr);
    Assert.NotSame(foo1, foo2);
  }

  [Theory]
  [ClassData(typeof(StatManagerData))]
  public async Task Stat_Retain_NonTrivial_Name(IStatManager mgr) {
    var foo1 = await StatTestUtil.CreateStat(mgr, "foo", "bar");
    Assert.NotNull(foo1);
    await mgr.RegisterStat(foo1);
    var foo2 = await StatTestUtil.CreateStat(mgr, "foo", "foobar");
    Assert.NotNull(foo2);
    Assert.Same(foo1, foo2);
  }

  [Theory]
  [ClassData(typeof(StatManagerData))]
  public async Task Stat_Retain_NonTrivial_Desc(IStatManager mgr) {
    var foo1 = await StatTestUtil.CreateStat(mgr, "foo", desc: "bar");
    Assert.NotNull(foo1);
    await mgr.RegisterStat(foo1);
    var foo2 = await StatTestUtil.CreateStat(mgr, "foo", desc: "foobar");
    Assert.NotNull(foo1);
    Assert.NotNull(foo2);
    Assert.Same(foo1, foo2);
  }

  [Theory]
  [ClassData(typeof(StatManagerData))]
  public async Task Stat_Retain_NonTrivial_Both(IStatManager mgr) {
    var foo1 = await StatTestUtil.CreateStat(mgr, "foo", "foobar", "barfoo");
    Assert.NotNull(foo1);
    await mgr.RegisterStat(foo1);
    var foo2 = await StatTestUtil.CreateStat(mgr, "foo", "barfoo", "foobar");
    Assert.NotNull(foo1);
    Assert.NotNull(foo2);
    Assert.Same(foo1, foo2);
  }
}