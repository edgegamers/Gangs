using System.Collections;
using GangsAPI.Data.Stat;
using Microsoft.Extensions.DependencyInjection;
using Stats.Perk;
using Stats.Stat;
using Stats.Stat.Gang;
using Stats.Stat.Player;

namespace GangsTest.API.Services.Stat.Concrete;

public class TestData : IEnumerable<object[]> {
  private static readonly IServiceCollection services = new ServiceCollection();

  private readonly IStat[] stats = [
    new BalanceStat(), new DescriptionStat(),
    new CapacityPerk(services.BuildServiceProvider()), new InvitationStat(),
    new KDRStat(), new PlaytimeStat(), new RoundStats()
  ];

  static TestData() { services.ConfigureServices(); }

  public IEnumerator<object[]> GetEnumerator() {
    return stats.Select(stat => (object[]) [stat]).GetEnumerator();
  }

  IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
}