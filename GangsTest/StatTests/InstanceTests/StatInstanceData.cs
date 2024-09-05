using System.Collections;
using GangsAPI.Data.Stat;
using Stats;

namespace GangsTest.StatTests.InstanceTests;

public class StatInstanceData : IEnumerable<object[]> {
  private readonly IStat[] stats = [new GangBankStat()];

  public IEnumerator<object[]> GetEnumerator() {
    return stats.Select(stat => (object[]) [stat]).GetEnumerator();
  }

  IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
}