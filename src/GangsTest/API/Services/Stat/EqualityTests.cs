using GangsAPI.Services;
using GangsTest.API.Services.Stat.Manager;
using TestData = GangsTest.API.Services.Rank.TestData;

namespace GangsTest.API.Services.Stat;

public class EqualityTests {
  [Theory]
  [ClassData(typeof(TestData))]
  public async Task Stat_Equality_SameEverything(IStatManager mgr) {
    var foo1 = await mgr.CreateStat("id", "name", "desc");
    var foo2 = await mgr.CreateStat("id", "name", "desc");
    Assert.Equal(foo1, foo2);
    Assert.StrictEqual(foo1, foo2);
  }

  [Theory]
  [ClassData(typeof(TestData))]
  public async Task Stat_Equality_DiffName(IStatManager mgr) {
    var foo1 = await mgr.CreateStat("id", "name", "desc");
    var foo2 = await mgr.CreateStat("id", "foo", "desc");
    Assert.Equal(foo1, foo2);
    Assert.StrictEqual(foo1, foo2);
  }

  [Theory]
  [ClassData(typeof(TestData))]
  public async Task Stat_Equality_DiffDesc(IStatManager mgr) {
    var foo1 = await mgr.CreateStat("id", "name", "desc");
    var foo2 = await mgr.CreateStat("id", "name", "foo");
    Assert.Equal(foo1, foo2);
    Assert.StrictEqual(foo1, foo2);
  }

  [Theory]
  [ClassData(typeof(TestData))]
  public async Task Stat_Equality_DiffBoth(IStatManager mgr) {
    var foo1 = await mgr.CreateStat("id", "name", "desc");
    var foo2 = await mgr.CreateStat("id", "foo", "bar");
    Assert.Equal(foo1, foo2);
    Assert.StrictEqual(foo1, foo2);
  }

  [Theory]
  [ClassData(typeof(TestData))]
  public async Task Stat_Equality_DiffIDs(IStatManager mgr) {
    var foo1 = await mgr.CreateStat("foo", "name", "desc");
    var foo2 = await mgr.CreateStat("bar", "name", "desc");
    Assert.NotStrictEqual(foo1, foo2);
    Assert.NotEqual(foo1, foo2);
  }
}