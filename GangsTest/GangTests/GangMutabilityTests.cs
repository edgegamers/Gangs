using GangsAPI.Services;
using Mock;

namespace GangsTest.GangTests;

public class GangMutabilityTests {
  [Theory]
  [ClassData(typeof(GangManagerData))]
  public async Task GangMutability_Name(IGangManager mgr) {
    var dummy = await GangTestUtil.CreateGang(mgr);
    Assert.NotNull(dummy);
    Assert.Equal("name", dummy.Name);
    dummy.Name = "foobar";
    Assert.Equal("foobar", dummy.Name);
    dummy = await mgr.GetGang(dummy.GangId);
    Assert.NotNull(dummy);
    Assert.Equal("name", dummy.Name);
  }

  [Theory]
  [ClassData(typeof(GangManagerData))]
  public async Task GangMutability_Update_Name(IGangManager mgr) {
    var dummy = await GangTestUtil.CreateGang(mgr);
    Assert.NotNull(dummy);
    Assert.Equal("name", dummy.Name);
    dummy.Name = "foobar";
    Assert.Equal("foobar", dummy.Name);
    await mgr.UpdateGang(dummy);
    dummy = await mgr.GetGang(dummy.GangId);
    Assert.NotNull(dummy);
    Assert.Equal("foobar", dummy.Name);
  }
}