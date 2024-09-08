using GangsAPI.Services.Gang;

namespace GangsTest.API.Services.Gang;

public class DeletionTests {
  [Theory]
  [ClassData(typeof(TestData))]
  public async Task Basic(IGangManager mgr) {
    var gang = await mgr.CreateGang("Test Gang", 0);
    Assert.NotNull(gang);
    Assert.Equal("Test Gang", gang.Name);
    await mgr.DeleteGang(gang.GangId);
    Assert.Null(await mgr.GetGang(gang.GangId));
  }

  [Theory]
  [ClassData(typeof(TestData))]
  public async Task Multiple(IGangManager mgr) {
    var gang1 = await mgr.CreateGang("Test Gang", 0);
    var gang2 = await mgr.CreateGang("Other Gang", 1);
    Assert.NotNull(gang1);
    Assert.NotNull(gang2);
    Assert.Equal("Test Gang", gang1.Name);
    await mgr.DeleteGang(gang1.GangId);
    Assert.Null(await mgr.GetGang(gang1.GangId));
    Assert.NotNull(await mgr.GetGang(gang2.GangId));
  }
}