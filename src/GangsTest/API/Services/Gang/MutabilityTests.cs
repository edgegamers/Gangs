using GangsAPI.Services.Gang;

namespace GangsTest.API.Services.Gang;

public class MutabilityTests {
  [Theory]
  [ClassData(typeof(TestData))]
  public async Task GangMutability_Name(IGangManager mgr) {
    var dummy = await mgr.CreateGang("name", 0);
    Assert.NotNull(dummy);
    Assert.Equal("name", dummy.Name);
    dummy.Name = "foobar";
    Assert.Equal("foobar", dummy.Name);
    dummy = await mgr.GetGang(dummy.GangId);
    Assert.NotNull(dummy);
    Assert.Equal("name", dummy.Name);
  }

  [Theory]
  [ClassData(typeof(TestData))]
  public async Task GangMutability_Update_Name(IGangManager mgr) {
    var dummy = await mgr.CreateGang("name", 0);
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