using GangsAPI.Services.Gang;

namespace GangsTest.API.Services.Stat.Instance.Gang;

public class GangTests(IGangManager gangs) : TestParent(gangs) {
  [Theory]
  [ClassData(typeof(TestData))]
  public async Task Instance_Fetch_Unregistered(IGangStatManager manager) {
    var result = await manager.GetForGang<int>(TestGang, TestStat.StatId);
    Assert.Equal(0, result);
  }

  [Theory]
  [ClassData(typeof(TestData))]
  public async Task Instance_Delete(IGangStatManager manager) {
    Assert.True(await manager.SetForGang(TestGang, TestStat));
    Assert.True(await manager.RemoveFromGang(TestGang, TestStat.StatId));
    var result = await manager.GetForGang<int>(TestGang, TestStat.StatId);
    Assert.Equal(0, result);
  }

  [Theory]
  [ClassData(typeof(TestData))]
  public async Task Instance_Delete_Unregistered(IGangStatManager manager) {
    Assert.False(await manager.RemoveFromGang(TestGang, TestStat.StatId));
  }

  [Theory]
  [ClassData(typeof(TestData))]
  public async Task Instance_Primitives(IGangStatManager manager) {
    Assert.True(await manager.SetForGang(TestGang, "foobar", 32));
    Assert.True(await manager.SetForGang(TestGang, "foobar", 32L));
    Assert.True(await manager.SetForGang(TestGang, "foobar", 32f));
    Assert.True(await manager.SetForGang(TestGang, "foobar", 32.0));
    Assert.True(await manager.SetForGang(TestGang, "foobar", (ulong)32));
  }
}