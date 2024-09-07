using System.Runtime.Serialization;
using GangsAPI.Data.Gang;
using GangsAPI.Data.Stat;
using GangsAPI.Services;
using Mock;

namespace GangsTest.StatTests.InstanceManageTests;

public class InstanceGangTests(IGangManager gangMgr) {
  private readonly IGang testGang =
    gangMgr.CreateGang("Test Gang", (ulong)new Random().NextInt64())
     .GetAwaiter()
     .GetResult() ?? throw new InvalidOperationException();

  private readonly IStat<int> testStat = new TestStatInstance();
  private readonly IStat<Reputation> testReputation = new ReputationStat();

  public class Reputation(int Positive, int negative) : IEquatable<Reputation> {
    public int Positive { get; } = Positive;
    public int Negative { get; } = negative;

    public bool Equals(Reputation? other) {
      if (other is null) return false;
      if (ReferenceEquals(this, other)) return true;
      return Positive == other.Positive && Negative == other.Negative;
    }

    public override bool Equals(object? obj) {
      if (obj is null) return false;
      if (ReferenceEquals(this, obj)) return true;
      if (obj.GetType() != GetType()) return false;
      return Equals((Reputation)obj);
    }

    public override int GetHashCode() {
      return HashCode.Combine(Positive, Negative);
    }
  }

  private class ReputationStat : IStat<Reputation> {
    public string StatId => "reputation";
    public string Name => "Reputation";
    public string Description => "The reputation of the gang.";

    public Reputation Value { get; set; } = new(0, 0);

    public IStat<Reputation> Clone() {
      return new ReputationStat { Value = Value };
    }

    public bool Equals(IStat? other) {
      return other is not null && StatId == other.StatId;
    }

    public bool Equals(IStat<Reputation>? other) {
      return other is not null && StatId == other.StatId;
    }

    public override int GetHashCode() { return StatId.GetHashCode(); }
  }

  [Theory]
  [ClassData(typeof(InstanceManageData))]
  public async Task Instance_Class_Push(IGangStatManager manager) {
    Assert.True(await manager.SetForGang(testGang, testReputation));
    var (success, val) =
      await manager.GetForGang<Reputation>(testGang, testReputation.StatId);
    Assert.True(success);
    Assert.Equal(testReputation.Value, val);
  }

  [Theory]
  [ClassData(typeof(InstanceManageData))]
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
  [ClassData(typeof(InstanceManageData))]
  public async Task Instance_Push(IGangStatManager manager) {
    Assert.True(await manager.SetForGang(testGang, testStat));
    var (success, val) =
      await manager.GetForGang<int>(testGang, testStat.StatId);
    Assert.True(success);
    Assert.Equal(testStat.Value, val);
  }

  [Theory]
  [ClassData(typeof(InstanceManageData))]
  public async Task Instance_Push_Default(IGangStatManager manager) {
    testStat.Value = 64;
    Assert.True(await manager.SetForGang(testGang, testStat));
    var (success, val) =
      await manager.GetForGang<int>(testGang, testStat.StatId);
    Assert.True(success);
    Assert.Equal(64, val);
  }

  [Theory]
  [ClassData(typeof(InstanceManageData))]
  public async Task Instance_Push_Changed_StatInst(IGangStatManager manager) {
    testStat.Value = 64;
    Assert.True(await manager.SetForGang(testGang, testStat));
    testStat.Value = 128;
    var (success, val) =
      await manager.GetForGang<int>(testGang, testStat.StatId);
    Assert.True(success);
    Assert.Equal(64, val);
  }

  [Theory]
  [ClassData(typeof(InstanceManageData))]
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
  [ClassData(typeof(InstanceManageData))]
  public async Task Instance_Fetch_Unregistered(IGangStatManager manager) {
    var (success, _) = await manager.GetForGang<int>(testGang, testStat.StatId);
    Assert.False(success);
  }

  [Theory]
  [ClassData(typeof(InstanceManageData))]
  public async Task Instance_Delete(IGangStatManager manager) {
    Assert.True(await manager.SetForGang(testGang, testStat));
    Assert.True(await manager.RemoveFromGang(testGang, testStat.StatId));
    var (success, _) = await manager.GetForGang<int>(testGang, testStat.StatId);
    Assert.False(success);
  }

  [Theory]
  [ClassData(typeof(InstanceManageData))]
  public async Task Instance_Delete_Unregistered(IGangStatManager manager) {
    Assert.False(await manager.RemoveFromGang(testGang, testStat.StatId));
  }

  [Theory]
  [ClassData(typeof(InstanceManageData))]
  public async Task Instance_Primitives(IGangStatManager manager) {
    Assert.True(await manager.SetForGang(testGang, "foobar", 32));
    Assert.True(await manager.SetForGang(testGang, "foobar", 32L));
    Assert.True(await manager.SetForGang(testGang, "foobar", 32f));
    Assert.True(await manager.SetForGang(testGang, "foobar", 32.0));
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