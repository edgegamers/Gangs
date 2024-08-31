using GangsAPI.Services;

namespace GangsTest.StatTests;

public class StatEqualityTests {
  [Theory]
  [ClassData(typeof(StatManagerData))]
  public async Task Stat_Equality_SameEverything(IStatManager mgr) {
    var foo1 = await StatTestUtil.CreateStat(mgr);
    var foo2 = await StatTestUtil.CreateStat(mgr);
    Assert.Equal(foo1, foo2);
    Assert.StrictEqual(foo1, foo2);
  }

  [Theory]
  [ClassData(typeof(StatManagerData))]
  public async Task Stat_Equality_DiffName(IStatManager mgr) {
    var foo1 = await StatTestUtil.CreateStat(mgr);
    var foo2 = await StatTestUtil.CreateStat(mgr, name: "foo");
    Assert.Equal(foo1, foo2);
    Assert.StrictEqual(foo1, foo2);
  }

  [Theory]
  [ClassData(typeof(StatManagerData))]
  public async Task Stat_Equality_DiffDesc(IStatManager mgr) {
    var foo1 = await StatTestUtil.CreateStat(mgr);
    var foo2 = await StatTestUtil.CreateStat(mgr, desc: "foo");
    Assert.Equal(foo1, foo2);
    Assert.StrictEqual(foo1, foo2);
  }

  [Theory]
  [ClassData(typeof(StatManagerData))]
  public async Task Stat_Equality_DiffBoth(IStatManager mgr) {
    var foo1 = await StatTestUtil.CreateStat(mgr);
    var foo2 = await StatTestUtil.CreateStat(mgr, name: "foo", desc: "bar");
    Assert.Equal(foo1, foo2);
    Assert.StrictEqual(foo1, foo2);
  }

  [Theory]
  [ClassData(typeof(StatManagerData))]
  public async Task Stat_Equality_DiffIDs(IStatManager mgr) {
    var foo1 = await StatTestUtil.CreateStat(mgr, "foo");
    var foo2 = await StatTestUtil.CreateStat(mgr, "bar");
    Assert.NotStrictEqual(foo1, foo2);
    Assert.NotEqual(foo1, foo2);
  }
}