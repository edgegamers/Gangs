using GangsAPI.Extensions;
using Stats.Stat.Gang;

namespace GangsTest.Stats.InvitationStat;

public class TestData : TheoryData<InvitationData, int[], ulong[], ulong[]> {
  private static readonly Random RNG = new();

  public TestData() {
    foreach (var n in (int[]) [0, 1, 2, 100]) {
      var invited = genN<ulong>(n);
      var inviter = genN<ulong>(n);
      var dates   = genN<int>(n);
      add(generate(invited, inviter, dates));
      add(generate(invited, inviter, dates, true));
    }
  }

  private void add((InvitationData, int[], ulong[], ulong[]) data) {
    Add(data.Item1, data.Item2, data.Item3, data.Item4);
  }

  private static (InvitationData, int[], ulong[], ulong[]) generate(
    ulong[] invited, ulong[] inviter, int[] dates, bool trailing = false) {
    return trailing ?
      (populateTrailing(dates, invited, inviter), dates, invited, inviter) :
      (populate(dates, invited, inviter), dates, invited, inviter);
  }

  private static InvitationData populate(int[] dates, ulong[] invited,
    ulong[] inviter) {
    var data = new InvitationData {
      InvitedSteams = string.Join(",", invited.Select(i => i.ToString())),
      InviterSteams = string.Join(",", inviter.Select(i => i.ToString())),
      Dates         = string.Join(",", dates.Select(i => i.ToString()))
    };

    return data;
  }

  private static InvitationData populateTrailing(int[] dates, ulong[] invited,
    ulong[] inviter) {
    var data = new InvitationData {
      InvitedSteams = string.Join(",", invited.Select(i => i)) + ",",
      InviterSteams = string.Join(",", inviter.Select(i => i)) + ",",
      Dates         = string.Join(",", dates.Select(i => i)) + ","
    };
    return data;
  }

  private static T[] genN<T>(int n) {
    return typeof(T) switch {
      { } t when t == typeof(int) => Enumerable.Range(0, n)
       .Select(_ => (T)(object)RNG.Next())
       .ToArray(),
      { } t when t == typeof(ulong) => Enumerable.Range(0, n)
       .Select(_ => (T)(object)RNG.NextULong())
       .ToArray(),
      _ => throw new ArgumentException("Invalid type", nameof(T))
    };
  }
}