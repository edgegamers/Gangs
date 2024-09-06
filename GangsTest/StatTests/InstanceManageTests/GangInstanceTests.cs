using GangsAPI.Data.Gang;
using GangsAPI.Data.Stat;
using GangsAPI.Services;

namespace GangsTest.StatTests.InstanceManageTests;

public class InstanceGangTests(IGangManager gangMgr) {
  private readonly IGang testGang =
    gangMgr.CreateGang("Test Gang", (ulong)new Random().NextInt64())
     .GetAwaiter()
     .GetResult() ?? throw new InvalidOperationException();

  private readonly IStat<int> testStat = new TestStatInstance();

  [Theory]
  [ClassData(typeof(InstanceManageData))]
  public async Task Instance_Push(IGangStatManager manager) {
    Assert.True(await manager.SetForGang(testGang, testStat));
    var success = await manager.GetForGang(testGang, testStat, out var val);
    Assert.True(success);
    Assert.Equal(testStat.Value, val);
  }

  [Theory]
  [ClassData(typeof(InstanceManageData))]
  public async Task Instance_Push_Default(IGangStatManager manager) {
    testStat.Value = 64;
    Assert.True(await manager.SetForGang(testGang, testStat));
    var success = await manager.GetForGang(testGang, testStat, out var val);
    Assert.True(success);
    Assert.Equal(64, val);
  }

  [Theory]
  [ClassData(typeof(InstanceManageData))]
  public async Task Instance_Push_Changed_StatInst(IGangStatManager manager) {
    testStat.Value = 64;
    Assert.True(await manager.SetForGang(testGang, testStat));
    testStat.Value = 128;
    var result = await manager.GetForGang(testGang, testStat);
    Assert.True(result);
    Assert.Equal(64, testStat.Value);
  }

  [Theory]
  [ClassData(typeof(InstanceManageData))]
  public async Task Instance_Fetch_Unregistered(IGangStatManager manager) {
    var result = await manager.GetForGang(testGang, testStat);
    Assert.False(result);
  }

  [Theory]
  [ClassData(typeof(InstanceManageData))]
  public async Task Instance_Delete(IGangStatManager manager) {
    Assert.True(await manager.SetForGang(testGang, testStat));
    Assert.True(await manager.RemoveFromGang(testGang, testStat.StatId));
    var result = await manager.GetForGang(testGang, testStat);
    Assert.False(result);
  }

  [Theory]
  [ClassData(typeof(InstanceManageData))]
  public async Task Instance_Delete_Unregistered(IGangStatManager manager) {
    Assert.False(await manager.RemoveFromGang(testGang, testStat.StatId));
  }

  private class TestStatInstance : IStat<int> {
    public string StatId => "test_stat";
    public string Name => "Test Stat";
    public string Description => "A test stat.";
    public int Value { get; set; } = 32;

    public IStat<int> Clone() { return new TestStatInstance { Value = Value }; }

    public bool Equals(IStat? other) {
      return other is not null && StatId == other.StatId;
    }

    public bool Equals(IStat<int>? other) {
      return other is not null && StatId == other.StatId;
    }

    public override int GetHashCode() { return StatId.GetHashCode(); }
  }
}