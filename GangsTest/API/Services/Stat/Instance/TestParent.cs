using GangsAPI.Data.Stat;

namespace GangsTest.API.Services.Stat.Instance;

public class TestParent {
  protected readonly TestIntStat TestStat = new();

  protected class ReputationStat : IStat<Reputation> {
    public string StatId => "reputation";
    public string Name => "Reputation";
    public string Description => "How popular are we?";

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

  protected class TestIntStat : IStat<int> {
    public string StatId => "test_stat";
    public string Name => "Test Stat";
    public string Description => "A test stat.";
    public int Value { get; set; } = 32;

    public IStat<int> Clone() { return new TestIntStat { Value = Value }; }

    public bool Equals(IStat? other) {
      return other is not null && StatId == other.StatId;
    }

    public bool Equals(IStat<int>? other) {
      return other is not null && StatId == other.StatId;
    }

    public override int GetHashCode() { return StatId.GetHashCode(); }
  }

  protected class Reputation(int Positive, int negative)
    : IEquatable<Reputation> {
    public Reputation() : this(0, 0) { }
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
}