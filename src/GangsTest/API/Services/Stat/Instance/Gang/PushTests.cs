using GangsAPI.Data.Stat;
using GangsAPI.Services.Gang;

namespace GangsTest.API.Services.Stat.Instance.Gang;

public class PushTests(IGangManager gangMgr) : TestParent(gangMgr) {
  private readonly IStat<Reputation> testReputation = new ReputationStat();

  [Theory]
  [ClassData(typeof(TestData))]
  public async Task Class(IGangStatManager manager) {
    Assert.True(await manager.SetForGang(TestGang, testReputation));
    var (success, val) =
      await manager.GetForGang<Reputation>(TestGang, testReputation.StatId);
    Assert.True(success);
    Assert.Equal(testReputation.Value, val);
  }

  [Theory]
  [ClassData(typeof(TestData))]
  public async Task Class_Multiple(IGangStatManager manager) {
    Assert.True(
      await manager.SetForGang(TestGang, "foo", new Reputation(0, 1)));
    var (success, val) = await manager.GetForGang<Reputation>(TestGang, "foo");
    Assert.True(success);
    Assert.Equal(new Reputation(0, 1), val);
    Assert.True(
      await manager.SetForGang(TestGang, "foo", new Reputation(1, 0)));
    var (success2, val2) =
      await manager.GetForGang<Reputation>(TestGang, "foo");
    Assert.True(success2);
    Assert.Equal(new Reputation(1, 0), val2);
  }

  [Theory]
  [ClassData(typeof(TestData))]
  public async Task Primitive(IGangStatManager manager) {
    Assert.True(await manager.SetForGang(TestGang, TestStat));
    var (success, val) =
      await manager.GetForGang<int>(TestGang, TestStat.StatId);
    Assert.True(success);
    Assert.Equal(TestStat.Value, val);
  }

  [Theory]
  [ClassData(typeof(TestData))]
  public async Task Primitive_Unchanged(IGangStatManager manager) {
    TestStat.Value = 64;
    Assert.True(await manager.SetForGang(TestGang, TestStat));
    var (success, val) =
      await manager.GetForGang<int>(TestGang, TestStat.StatId);
    Assert.True(success);
    Assert.Equal(64, val);
  }

  [Theory]
  [ClassData(typeof(TestData))]
  public async Task Primitive_Changed(IGangStatManager manager) {
    TestStat.Value = 64;
    Assert.True(await manager.SetForGang(TestGang, TestStat));
    TestStat.Value = 128;
    var (success, val) =
      await manager.GetForGang<int>(TestGang, TestStat.StatId);
    Assert.True(success);
    Assert.Equal(64, val);
  }
}