using GangsAPI.Services.Gang;
using Mock;

namespace GangsTest.API.Services.Gang;

public class UpdateTests {
  [Theory]
  [ClassData(typeof(TestData))]
  public async Task GangManager_Update(IGangManager mgr) {
    var dummy = await mgr.CreateGang("name", 0);
    Assert.NotNull(dummy);
    Assert.True(await mgr.UpdateGang(dummy));
  }

  [Theory]
  [ClassData(typeof(TestData))]
  public async Task GangManager_UpdateName(IGangManager mgr) {
    var dummy = await mgr.CreateGang("name", 0);
    Assert.NotNull(dummy);
    dummy.Name = "new name";
    Assert.True(await mgr.UpdateGang(dummy));
    var updated = await mgr.GetGang(dummy.GangId);
    Assert.NotNull(updated);
    Assert.Equal("new name", updated.Name);
  }

  [Theory]
  [ClassData(typeof(TestData))]
  public async Task GangManager_Update_Fail(IGangManager mgr) {
    var dummy = new MockGang(-1, "nonexistent");
    Assert.False(await mgr.UpdateGang(dummy),
      "Gang manager reported update success for non-created gang");
  }
}