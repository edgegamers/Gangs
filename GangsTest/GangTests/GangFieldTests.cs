using GangsAPI.Services;

namespace GangsTest.GangTests;

public class GangFieldTests {
  [Theory]
  [ClassData(typeof(GangManagerData))]
  public async Task Gang_Fields_Name(IGangManager mgr) {
    var dummy = await GangTestUtil.CreateGang(mgr);
    Assert.NotNull(dummy);
    Assert.Equal("name", dummy.Name);
  }
}