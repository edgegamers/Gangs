using GangsAPI.Services;

namespace GangsTest.StatTests;

public class StatFieldTests {
  [Theory]
  [ClassData(typeof(StatManagerData))]
  public async Task Stat_Fields_Name(IStatManager mgr) {
    var dummy = await StatTestUtil.CreateStat(mgr);
    Assert.NotNull(dummy);
    Assert.Equal("name", dummy.Name);
  }

  [Theory]
  [ClassData(typeof(StatManagerData))]
  public async Task Stat_Fields_Id(IStatManager mgr) {
    var dummy = await StatTestUtil.CreateStat(mgr);
    Assert.NotNull(dummy);
    Assert.Equal("id", dummy.StatId);
  }

  [Theory]
  [ClassData(typeof(StatManagerData))]
  public async Task Stat_Fields_Desc_Null(IStatManager statManager) {
    var dummy = await StatTestUtil.CreateStat(statManager, desc: null);
    Assert.NotNull(dummy);
    Assert.Null(dummy.Description);
  }

  [Theory]
  [ClassData(typeof(StatManagerData))]
  public async Task Stat_Fields_Desc_Empty(IStatManager statManager) {
    var dummy = await StatTestUtil.CreateStat(statManager, desc: "");
    Assert.NotNull(dummy);
    Assert.Equal("", dummy.Description);
  }

  [Theory]
  [ClassData(typeof(StatManagerData))]
  public async Task Stat_Fields_Desc_Basic(IStatManager statManager) {
    var dummy = await StatTestUtil.CreateStat(statManager);
    Assert.NotNull(dummy);
    Assert.Equal("desc", dummy.Description);
  }
}