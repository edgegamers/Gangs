using GangsAPI.Services;
using GangsImpl.Memory;

namespace GangsTest.GangTests;

public class GangManagerTests {
  [Theory]
  [ClassData(typeof(GangManagerData))]
  public async Task GangManager_Update(IGangManager mgr) {
    var dummy = await GangTestUtil.CreateGang(mgr);
    Assert.NotNull(dummy);
    Assert.True(await mgr.UpdateGang(dummy));
  }

  [Theory]
  [ClassData(typeof(GangManagerData))]
  public async Task GangManager_UpdateName(IGangManager mgr) {
    var dummy = await GangTestUtil.CreateGang(mgr);
    Assert.NotNull(dummy);
    dummy.Name = "new name";
    Assert.True(await mgr.UpdateGang(dummy));
    var updated = await mgr.GetGang(dummy.GangId);
    Assert.NotNull(updated);
    Assert.Equal("new name", updated.Name);
  }

  [Theory]
  [ClassData(typeof(GangManagerData))]
  public async Task GangManager_Update_Fail(IGangManager mgr) {
    var dummy = new MockGang(-1, "nonexistent", 0);
    Assert.False(await mgr.UpdateGang(dummy),
      "Gang manager reported update success for non-created gang");
  }
}