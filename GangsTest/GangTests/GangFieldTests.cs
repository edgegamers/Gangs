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

  [Theory]
  [ClassData(typeof(GangManagerData))]
  public async Task Gang_Fields_Members(IGangManager mgr) {
    var dummy = await GangTestUtil.CreateGang(mgr);
    Assert.NotNull(dummy);
    Assert.NotNull(dummy.Members);
    Assert.Single(dummy.Members);
  }

  [Theory]
  [ClassData(typeof(GangManagerData))]
  public async Task Gang_Fields_Iteration(IGangManager mgr) {
    var dummy = await GangTestUtil.CreateGang(mgr);
    Assert.NotNull(dummy);
    var count = dummy.Count();
    Assert.Equal(1, count);
  }
}