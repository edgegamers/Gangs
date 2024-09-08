using GangsAPI.Services.Gang;

namespace GangsTest.API.Services.Gang;

public class PersistenceTests {
  [Theory]
  [ClassData(typeof(TestData))]
  public async Task Load_Steam(IGangManager mgr) {
    var id = (ulong)new Random().NextInt64();
    Assert.NotNull(await mgr.CreateGang("Test Gang", id));
    var gang = await mgr.GetGang(id);
    Assert.NotNull(gang);
    Assert.Equal("Test Gang", gang.Name);
  }

  [Theory]
  [ClassData(typeof(TestData))]
  public async Task Load_By_ID(IGangManager mgr) {
    var gang =
      await mgr.CreateGang("Test Gang", (ulong)new Random().NextInt64());
    Assert.NotNull(gang);
    var id = gang.GangId;
    gang = await mgr.GetGang(id);
    Assert.NotNull(gang);
    Assert.Equal("Test Gang", gang.Name);
  }

  [Theory]
  [ClassData(typeof(TestData))]
  public async Task Load_By_ID_Multiple(IGangManager mgr) {
    var gang1 =
      await mgr.CreateGang("Test Gang 1", (ulong)new Random().NextInt64());
    var gang2 =
      await mgr.CreateGang("Test Gang 2", (ulong)new Random().NextInt64());
    var gang3 =
      await mgr.CreateGang("Test Gang 3", (ulong)new Random().NextInt64());
    Assert.NotNull(gang1);
    Assert.NotNull(gang2);
    Assert.NotNull(gang3);
    var id   = gang2.GangId;
    var gang = await mgr.GetGang(id);
    Assert.NotNull(gang);
    Assert.Equal("Test Gang 2", gang.Name);
  }
}