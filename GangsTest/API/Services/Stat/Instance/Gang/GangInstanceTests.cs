using GangsAPI.Data.Gang;
using GangsAPI.Data.Stat;
using GangsAPI.Services.Gang;

namespace GangsTest.API.Services.Stat.Instance.Gang;

public class GangTests(IGangManager gangMgr) : TestParent {
  private readonly IGang testGang =
    gangMgr.CreateGang("Test Gang", (ulong)new Random().NextInt64())
     .GetAwaiter()
     .GetResult() ?? throw new InvalidOperationException();

  private readonly IStat<Reputation> testReputation = new ReputationStat();

  [Theory]
  [ClassData(typeof(TestData))]
  public async Task Instance_Class_Push(IGangStatManager manager) {
    Assert.True(await manager.SetForGang(testGang, testReputation));
    var (success, val) =
      await manager.GetForGang<Reputation>(testGang, testReputation.StatId);
    Assert.True(success);
    Assert.Equal(testReputation.Value, val);
  }

  [Theory]
  [ClassData(typeof(TestData))]
  public async Task Instance_Class_Push_Multiple(IGangStatManager manager) {
    Assert.True(
      await manager.SetForGang(testGang, "foo", new Reputation(0, 1)));
    var (success, val) = await manager.GetForGang<Reputation>(testGang, "foo");
    Assert.True(success);
    Assert.Equal(new Reputation(0, 1), val);
    Assert.True(
      await manager.SetForGang(testGang, "foo", new Reputation(1, 0)));
    var (success2, val2) =
      await manager.GetForGang<Reputation>(testGang, "foo");
    Assert.True(success2);
    Assert.Equal(new Reputation(1, 0), val2);
  }

  [Theory]
  [ClassData(typeof(TestData))]
  public async Task Instance_Push(IGangStatManager manager) {
    Assert.True(await manager.SetForGang(testGang, TestStat));
    var (success, val) =
      await manager.GetForGang<int>(testGang, TestStat.StatId);
    Assert.True(success);
    Assert.Equal(TestStat.Value, val);
  }

  [Theory]
  [ClassData(typeof(TestData))]
  public async Task Instance_Push_Default(IGangStatManager manager) {
    TestStat.Value = 64;
    Assert.True(await manager.SetForGang(testGang, TestStat));
    var (success, val) =
      await manager.GetForGang<int>(testGang, TestStat.StatId);
    Assert.True(success);
    Assert.Equal(64, val);
  }

  [Theory]
  [ClassData(typeof(TestData))]
  public async Task Instance_Push_Changed_StatInst(IGangStatManager manager) {
    TestStat.Value = 64;
    Assert.True(await manager.SetForGang(testGang, TestStat));
    TestStat.Value = 128;
    var (success, val) =
      await manager.GetForGang<int>(testGang, TestStat.StatId);
    Assert.True(success);
    Assert.Equal(64, val);
  }

  [Theory]
  [ClassData(typeof(TestData))]
  public async Task Instance_Class_Push_Inst(IGangStatManager manager) {
    testReputation.Value = new Reputation(11, 33);
    Assert.True(await manager.SetForGang(testGang, testReputation));
    var (success, val) =
      await manager.GetForGang<Reputation>(testGang, testReputation.StatId);
    Assert.True(success);
    Assert.Equal(testReputation.Value, val);
    Assert.NotNull(val);
    Assert.Equal(11, val.Positive);
    Assert.Equal(33, val.Negative);
  }

  [Theory]
  [ClassData(typeof(TestData))]
  public async Task Instance_Fetch_Unregistered(IGangStatManager manager) {
    var (success, _) = await manager.GetForGang<int>(testGang, TestStat.StatId);
    Assert.False(success);
  }

  [Theory]
  [ClassData(typeof(TestData))]
  public async Task Instance_Delete(IGangStatManager manager) {
    Assert.True(await manager.SetForGang(testGang, TestStat));
    Assert.True(await manager.RemoveFromGang(testGang, TestStat.StatId));
    var (success, _) = await manager.GetForGang<int>(testGang, TestStat.StatId);
    Assert.False(success);
  }

  [Theory]
  [ClassData(typeof(TestData))]
  public async Task Instance_Delete_Unregistered(IGangStatManager manager) {
    Assert.False(await manager.RemoveFromGang(testGang, TestStat.StatId));
  }

  [Theory]
  [ClassData(typeof(TestData))]
  public async Task Instance_Primitives(IGangStatManager manager) {
    Assert.True(await manager.SetForGang(testGang, "foobar", 32));
    Assert.True(await manager.SetForGang(testGang, "foobar", 32L));
    Assert.True(await manager.SetForGang(testGang, "foobar", 32f));
    Assert.True(await manager.SetForGang(testGang, "foobar", 32.0));
    Assert.True(await manager.SetForGang(testGang, "foobar", (ulong)32));
  }
}