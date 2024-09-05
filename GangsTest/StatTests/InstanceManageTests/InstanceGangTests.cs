using GangsAPI.Data.Gang;
using GangsAPI.Data.Stat;
using GangsAPI.Services;

namespace GangsTest.StatTests.InstanceManageTests;

public class InstanceGangTests(IGangManager gangMgr) {
  private class TestStatInstance : IStat<int> {
    public string StatId => "test_stat";
    public string Name => "Test Stat";
    public string? Description => "A test stat.";
    public int Value { get; set; } = 32;
  }

  private IGang testGang =
    gangMgr.CreateGang("Test Gang", (ulong)new Random().NextInt64())
     .GetAwaiter()
     .GetResult() ?? throw new InvalidOperationException();

  private readonly IStat<int> testStat = new TestStatInstance();

  [Theory]
  [ClassData(typeof(InstanceManageData))]
  public async Task Instance_Register(IStatManager stat,
    IGangStatManager manager) {
    Assert.True(await stat.RegisterStat(testStat));
    Assert.True(await manager.PushToGang(testGang, testStat));
  }

  [Theory]
  [ClassData(typeof(InstanceManageData))]
  public async Task Instance_Register_Unregistered(IStatManager _,
    IGangStatManager manager) {
    Assert.False(await manager.PushToGang(testGang, testStat));
  }

  [Theory]
  [ClassData(typeof(InstanceManageData))]
  public async Task Instance_Fetch_Registered(IStatManager stat,
    IGangStatManager manager) {
    Assert.True(await stat.RegisterStat(testStat));
    Assert.True(await manager.PushToGang(testGang, testStat));
    var result = await manager.GetForGang(testGang, testStat);
    Assert.NotNull(result);
    Assert.Equal(testStat, result);
  }

  [Theory]
  [ClassData(typeof(InstanceManageData))]
  public async Task Instance_Fetch_Unregistered(IStatManager stat,
    IGangStatManager manager) {
    Assert.True(await stat.RegisterStat(testStat));
    var result = await manager.GetForGang(testGang, testStat);
    Assert.Null(result);
  }
}