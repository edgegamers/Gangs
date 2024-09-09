using GangsAPI.Services;
using GangsTest.API.Services.Stat.Manager;
using TestData = GangsTest.API.Services.Rank.TestData;

namespace GangsTest.API.Services.Stat;

public class FieldTests {
  [Theory]
  [ClassData(typeof(TestData))]
  public async Task Stat_Fields_Name(IStatManager mgr) {
    var dummy = await mgr.CreateStat("id", "name", "desc");
    Assert.NotNull(dummy);
    Assert.Equal("name", dummy.Name);
  }

  [Theory]
  [ClassData(typeof(TestData))]
  public async Task Stat_Fields_Id(IStatManager mgr) {
    var dummy = await mgr.CreateStat("id", "name", "desc");
    Assert.NotNull(dummy);
    Assert.Equal("id", dummy.StatId);
  }

  [Theory]
  [ClassData(typeof(TestData))]
  public async Task Stat_Fields_Desc_Null(IStatManager statManager) {
    var dummy = await statManager.CreateStat("id", "name");
    Assert.NotNull(dummy);
    Assert.Null(dummy.Description);
  }

  [Theory]
  [ClassData(typeof(TestData))]
  public async Task Stat_Fields_Desc_Empty(IStatManager statManager) {
    var dummy = await statManager.CreateStat("id", "name", "");
    Assert.NotNull(dummy);
    Assert.Equal("", dummy.Description);
  }

  [Theory]
  [ClassData(typeof(TestData))]
  public async Task Stat_Fields_Desc_Basic(IStatManager statManager) {
    var dummy = await statManager.CreateStat("id", "name", "desc");
    Assert.NotNull(dummy);
    Assert.Equal("desc", dummy.Description);
  }
}