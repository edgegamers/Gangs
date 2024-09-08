using System.Collections;
using GangsAPI.Data.Stat;
using Stats;

namespace GangsTest.API.Services.Stat.Concrete;

public class TestData : IEnumerable<object[]> {
  private readonly IStat[] stats = [
    new BalanceStat(), new DescriptionStat(), new CapacityStat()
  ];

  public IEnumerator<object[]> GetEnumerator() {
    return stats.Select(stat => (object[]) [stat]).GetEnumerator();
  }

  IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
}